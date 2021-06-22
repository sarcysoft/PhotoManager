using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Caching;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PhotoManager
{
    public partial class CompositeMaker : Form
    {
        private string inputPath;
        private List<string> listPhotos;
        private Dictionary<string, int[]> rgbHashTable;
        private bool hashTableModified = false;

        public class ColourDetails
        {
            public bool Actual;
            public List<int> Photos;
        }

        private const int colourScale = 2;

        private ColourDetails[,,] lookupArray = new ColourDetails[(256 >> colourScale), (256 >> colourScale), (256 >> colourScale)];
        private int[,] pictureGrid;

        private Mat inputMat;
        private Mat outputMat;

        private Mutex outputMutex = new Mutex();

        private Rectangle outputRoi;
        private bool bOuputReady = false;

        private Thread activeThread;

        private int xx = 0;
        private int yy = 0;
        private double scale = 1;
        private int xSize, ySize;
        private int outScale = 2;

        private int photoCount = 0;
        private int colours = 0;
        private int coverage = 0;
        private int virtualCover = 0;

        private int progress = 0;
        private DateTime progressStart;
        private int progressMax = 100;

        private int zoom = 1;
        private int outMult = 1;
        private int zoomScale = 1;
        private int threshold = 1;
        private int minSearchSize = 0;
        private int maxSearchSize = 0;

        private string statusText = "";

        private CancellationTokenSource abortThread = new CancellationTokenSource();

        public CompositeMaker()
        {
            InitializeComponent();
        }

        public void SetSourceFile(string fullpath)
        {
            inputPath = fullpath;
        }

        public void SetPhotoList(List<string> photos)
        {
            listPhotos = photos;
        }

        private void CompositeMaker_Load(object sender, EventArgs e)
        {
            inputMat = CvInvoke.Imread(inputPath);

            var minsize = Math.Min(inputMat.Cols, inputMat.Height);
            Rectangle roi = new Rectangle((inputMat.Cols - minsize) >> 1, (inputMat.Height - minsize) >> 1, minsize, minsize);
            inputMat = new Mat(inputMat, roi);

            var scaledSize = (pictureSource.Width < pictureSource.Height) ? pictureSource.Width : pictureSource.Height;

            Mat scaledMat = new Mat();
            CvInvoke.Resize(inputMat, scaledMat, new Size(scaledSize, scaledSize), 0, 0, Inter.Area);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", scaledMat, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureSource.Image = Image.FromStream(stream);

            scale = (double)minsize / (double)scaledSize;
            threshold = trackThreshold.Value * trackThreshold.Value;
            minSearchSize = (int)numericSearchSize.Value;
            maxSearchSize = (int)(numericMaxSearch.Value = (64 >> colourScale));

            xx = inputMat.Cols / 2;
            yy = inputMat.Height / 2;

            numScale.Value = minsize / 350;
            UpdateSizes(); ;

            trackBar1.Value = 0;

            lblCover.Text = "Building lookup array.";

            activeThread = new Thread(BuildLookupArray);
            activeThread.Start();

            btnCreate.Enabled = false;
            btnUpdate.Enabled = false;

            UpdateTarget();
        }

        private void UpdateTarget()
        {

            if (xx < 0)
            {
                xx = 0;
            }

            if (xx + 1 > inputMat.Cols)
            {
                xx = inputMat.Cols - 1;
            }

            if (yy < 0)
            {
                yy = 0;
            }

            if (yy + 1 > inputMat.Rows)
            {
                yy = inputMat.Rows - 1;
            }

            Rectangle roi = new Rectangle(xx, yy, 1, 1);
            Mat targetMat = new Mat(inputMat, roi);
            CvInvoke.Resize(targetMat, targetMat, new Size(pictureTarget.Width, pictureTarget.Height), 0, 0, Inter.Area);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", targetMat, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureTarget.Image = Image.FromStream(stream);
        }

        private void UpdateBestFit()
        {
            var value = inputMat.GetRawData(yy, xx);
            var best = FindClosest(value[0], value[1], value[2]);

            var bestMat = LoadMat(best, pictureBestFit.Width);

            CvInvoke.Resize(bestMat, bestMat, new Size(pictureBestFit.Width, pictureBestFit.Height), 0, 0, Inter.Area);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", bestMat, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureBestFit.Image = Image.FromStream(stream);

            bestMat = LoadMat(best, 1);

            CvInvoke.Resize(bestMat, bestMat, new Size(pictureBestColour.Width, pictureBestColour.Height), 0, 0, Inter.Area);
            buf = new();
            CvInvoke.Imencode(".jpg", bestMat, buf);
            stream = new MemoryStream(buf.ToArray());
            pictureBestColour.Image = Image.FromStream(stream);


        }

        private void numSegmentSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateTarget();
        }

        private void pictureSource_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            xx = (int)(me.Location.X * scale);
            yy = (int)(me.Location.Y * scale);

            UpdateTarget();
            UpdateBestFit();
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private string GetHash(FileInfo fi)
        {

            string hash = $"{CreateMD5(fi.FullName)}{fi.Length}{fi.CreationTime}";

            return hash;
        }

        private void BuildLookupArray()
        {
            photoCount = 0;
            colours = 0;

            rgbHashTable = new Dictionary<string, int[]> { };

            var iRange = Enumerable.Range(0, listPhotos.Count);

            Dictionary<string, string> hashPhotos = new Dictionary<string, string> { };

            foreach (var i in listPhotos)
            {
                FileInfo fi = new FileInfo(i);

                string hash = GetHash(fi);
                hashPhotos[hash] = i;
            }

            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = abortThread.Token;
            po.MaxDegreeOfParallelism = 7;

            try
            { 
                if (File.Exists("rgbhashtable.json"))
                {
                    using (StreamReader file = new StreamReader("rgbhashtable.json"))
                    {
                        string dictString = file.ReadToEnd();
                        rgbHashTable = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(dictString);

                        foreach(var i in rgbHashTable)
                        {
                            if (hashPhotos.ContainsKey(i.Key))
                            {
                                int[] rgbValue = i.Value; ;
                                if (rgbValue != null)
                                {
                                    int r = rgbValue[0] >> colourScale;
                                    int g = rgbValue[1] >> colourScale;
                                    int b = rgbValue[2] >> colourScale;

                                    if (lookupArray[r, g, b] == null)
                                    {
                                        var details = new ColourDetails();
                                        details.Actual = true;
                                        details.Photos = new List<int> { };
                                        lookupArray[r, g, b] = details;

                                        colours++;
                                    }

                                    photoCount++;
                                    lookupArray[r, g, b].Photos.Add(listPhotos.IndexOf(hashPhotos[i.Key]));
                                    hashPhotos[i.Key] = null;
                                }
                            }
                        }
                    }
                }

                progress = 0;
                progressMax = listPhotos.Count;
                progressStart = DateTime.UtcNow;

                Parallel.ForEach(hashPhotos, po, i =>
                {
                    if (i.Value != null)
                    {
                        try
                        {
                            int[] rgbValue = null;

                            try
                            {
                                if (rgbHashTable.ContainsKey(i.Key))
                                {
                                    rgbValue = rgbHashTable[i.Key];
                                }
                                else
                                {
                                    var image = LoadMat(i.Value, 1, true);

                                    var value = image.GetRawData(0, 0);
                                    rgbValue = new int[] { value[0], value[1], value[2] };
                                    rgbHashTable[i.Key] = rgbValue;
                                    hashTableModified = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            if (rgbValue != null)
                            {
                                try
                                {
                                    int r = rgbValue[0] >> colourScale;
                                    int g = rgbValue[1] >> colourScale;
                                    int b = rgbValue[2] >> colourScale;

                                    if (lookupArray[r, g, b] == null)
                                    {
                                        var details = new ColourDetails();
                                        details.Actual = true;
                                        details.Photos = new List<int> { };
                                        lookupArray[r, g, b] =  details;

                                        colours++;
                                    }

                                    photoCount++;
                                    lookupArray[r, g, b].Photos.Add(listPhotos.IndexOf(i.Value));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    po.CancellationToken.ThrowIfCancellationRequested();

                    coverage = (100 * colours) / ((256 >> colourScale) * (256 >> colourScale) * (256 >> colourScale));

                    progress = photoCount;
                    statusText = $"Assigning image {photoCount} of {listPhotos.Count} to look-up array.";
                });
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            finally
            {
                //abortThread.Dispose();
            }

            Console.WriteLine($"Photos = {photoCount};\nUnique = {colours}");
            Console.WriteLine($"Coverage = {coverage}%");

            SaveHashTable();

            progress = 0;
            statusText = "";
        }

        private int FindMin(int v, int s)
        {
            return (v - s) < 0 ? 0 : v - s;
        }

        private int FindMax(int v, int s)
        {
            var limit = (256 >> colourScale) - 1;
            return (v + s) >= limit ? limit : v + s;
        }

        private int FindClosest(int r, int g, int b)
        {
            int best = -1;

            r >>= colourScale;
            g >>= colourScale;
            b >>= colourScale;

            List<int> lookupList = new List<int>() { };

            int searchSize = minSearchSize;

            while ((lookupList.Count == 0) || ((lookupList.Count * searchSize * searchSize) < threshold) && (searchSize <= maxSearchSize))
            {
                int x1 = FindMin(r, searchSize);
                int x2 = FindMax(r, searchSize);

                int y1 = FindMin(g, searchSize);
                int y2 = FindMax(g, searchSize);

                int z1 = FindMin(b, searchSize);
                int z2 = FindMax(b, searchSize);

                for (var x = x1; x <= x2; x++)
                {
                    for (var y = y1; y <= y2; y++)
                    {
                        for (var z = z1; z <= z2; z++)
                        {
                            var item = lookupArray[x, y, z];
                            if (item != null)
                            {
                                if ((searchSize == 0) || (item.Actual == true))
                                {
                                    if (item.Photos != null)
                                    {
                                        foreach (var photo in item.Photos)
                                        {
                                            //if (!lookupList.Contains(photo))
                                            {
                                                lookupList.Add(photo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                searchSize++;
            }

            if (lookupArray[r, g, b] == null)
            {
                lookupArray[r, g, b] = new();
                lookupArray[r, g, b].Actual = false;
                lookupArray[r, g, b].Photos = lookupList;
                colours++;
                virtualCover = (100 * colours) / ((256 >> colourScale) * (256 >> colourScale) * (256 >> colourScale));
            }

            if (lookupList.Count == 1)
            {
                best = lookupList[0];
            }
            else if (lookupList.Count > 1)
            {
                var rand = new Random();
                int x = rand.Next(0, lookupList.Count);
                best = lookupList[x];
            }
            else
            {
                //Console.WriteLine($"No Match for [{r},{g},{b}]");
            }

            return best;
        }

        private void Create()
        {
            xSize = (int)(inputMat.Cols / outScale);
            ySize = (int)(inputMat.Rows / outScale);
            try
            {
                Mat scaledMat = new();
                CvInvoke.Resize(inputMat, scaledMat, new Size(xSize, ySize), 0, 0, Inter.Area);

                pictureGrid = new int[xSize, ySize];

                var cols = Enumerable.Range(0, xSize);
                var rows = Enumerable.Range(0, ySize);

                statusText = "Creating picture grid.";

                progress = 0;
                progressMax = cols.Count();
                progressStart = DateTime.UtcNow;

                foreach (var col in cols)
                {
                    try
                    {
                        ParallelOptions po = new ParallelOptions();
                        po.CancellationToken = abortThread.Token;
                        //po.MaxDegreeOfParallelism = 7;

                        Parallel.ForEach(rows, po, row =>
                        {
                            var value = scaledMat.GetRawData(row, col);
                            pictureGrid[col, row] = FindClosest(value[0], value[1], value[2]);

                            po.CancellationToken.ThrowIfCancellationRequested();
                        });
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    finally
                    {
                        //abortThread.Dispose();
                    }

                    progress = col;
                    //System.GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            progress = 0;

            UpdateDest();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            btnCreate.Enabled = false;
            btnUpdate.Enabled = false;

            outScale = (int)numScale.Value;
            threshold = trackThreshold.Value * trackThreshold.Value;

            activeThread = new Thread(Create);
            activeThread.Start();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdateSizes();
        }

        public class SectionDetails
        {
            public int xOffset;
            public int yOffset;
            public int xSegs;
            public int ySegs;
        }

        private void UpdateDest()
        {
            zoomScale = zoom * outMult;
            var segs = xSize / zoom;

            var xStart = (xx / outScale) - (segs / 2);
            var yStart = (yy / outScale) - (segs / 2);

            if (xStart < 0)
            {
                xStart = 0;
            }
            if ((xStart + segs) > xSize)
            {
                xStart = xSize - segs;
            }

            if (yStart < 0)
            {
                yStart = 0;
            }
            if ((yStart + segs) > ySize)
            {
                yStart = ySize - segs;
            }            

            try
            {
                if ((segs * zoom) < xSize)
                {
                    segs += 2;
                }

                outputMat = new Mat(new Size(segs * zoomScale, segs * zoomScale), inputMat.Depth, inputMat.NumberOfChannels);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (outputMat != null)
            {

                var outX = xSize * outMult;
                var outY = ySize * outMult;
                outputRoi = new Rectangle((outputMat.Cols - outX) / 2, (outputMat.Rows - outY) / 2, outX, outY);

                progress = 0;
                statusText = "Building output image.";
                DateTime startTime = DateTime.UtcNow;

                try
                {
                    ParallelOptions po = new ParallelOptions();
                    po.CancellationToken = abortThread.Token;
                    po.MaxDegreeOfParallelism = 7;

                    Dictionary<int, List<(int, int)>> pictureSet = new Dictionary<int, List<(int, int)>> { };

                    var cols = Enumerable.Range(0, segs);
                    var rows = Enumerable.Range(0, segs);

                    foreach (var row in rows)
                    {
                        foreach (var col in cols)
                        {
                            var index = pictureGrid[xStart + col, yStart + row];
                            if (!pictureSet.ContainsKey(index))
                            {
                                pictureSet[index] = new List<(int, int)> { };
                            }

                            pictureSet[index].Add((col, row));
                        }
                    }

                    List<KeyValuePair<int, List<(int,int)>>> pictureList = pictureSet.ToList();

                    pictureList.Sort((y, x) => x.Value.Count.CompareTo(y.Value.Count));

                    progressMax = pictureList.Count;
                    progressStart = DateTime.UtcNow;

                    Parallel.ForEach (pictureList, po, pic =>
                    {
                        var image = LoadMat(pic.Key, zoomScale);

                        foreach (var loc in pic.Value)
                        {
                            Rectangle targetRoi = new Rectangle(loc.Item1 * zoomScale, loc.Item2 * zoomScale, zoomScale, zoomScale);
                            Mat localTarget = new Mat(outputMat, targetRoi);
                            //outputMutex.WaitOne();
                            image.CopyTo(localTarget);
                            //outputMutex.ReleaseMutex();
                        }

                        bOuputReady = true;
                        
                        progress++;

                        po.CancellationToken.ThrowIfCancellationRequested();

                    });
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                finally
                {
                    //abortThread.Dispose();
                }

                TimeSpan buildTime = DateTime.UtcNow.Subtract(startTime);
                statusText = $"Build completed in {buildTime.Minutes}m {buildTime.Seconds}s.";
            }

            progress = 0;
        }


        Mutex loadMutex = new Mutex();

        private Mat LoadMat(string path, int zoom = 1, bool build = false)
        {
            Mat tempMat = CvInvoke.Imread(path);

            var minsize = Math.Min(tempMat.Cols, tempMat.Rows);
            Rectangle roi = new((tempMat.Cols - minsize) >> 1, (tempMat.Rows - minsize) >> 1, minsize, minsize);
            tempMat = new Mat(tempMat, roi);

            if (build)
            {
                CvInvoke.Resize(tempMat, tempMat, new Size(zoom, zoom), 0, 0, Inter.Area);
            }
            else
            {
                CvInvoke.Resize(tempMat, tempMat, new Size(zoom, zoom), 0, 0, Inter.Lanczos4);
            }

            return tempMat;
        }

        private Mat LoadMat(int fileNo, int zoom = 1, bool build = false)
        {
            return LoadMat(listPhotos[fileNo], zoom, build);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            zoom = (int)Math.Pow(2, trackBar1.Value);
            labelZoom.Text = $"x{zoom}";
        }

        private void pictureDest_DoubleClick(object sender, EventArgs e)
        {
            CvInvoke.Imshow("Comnposite", outputMat);
        }

        private void UpdateDestPic()
        {
            if (bOuputReady)
            {
                var scaledSize = (pictureDest.Width < pictureDest.Height ? pictureDest.Width : pictureDest.Height);

                Mat croppedMat = new Mat(outputMat, outputRoi);

                //outputMutex.WaitOne();

                Mat scaledMat = new Mat();
                CvInvoke.Resize(croppedMat, scaledMat, new Size(scaledSize, scaledSize), 0, 0, Inter.Area);
                Emgu.CV.Util.VectorOfByte buf = new();
                CvInvoke.Imencode(".jpg", scaledMat, buf);
                MemoryStream stream = new MemoryStream(buf.ToArray());
                bOuputReady = false;

                //outputMutex.ReleaseMutex();

                pictureDest.Image = Image.FromStream(stream);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Maximum = progressMax;
            progressBar1.Value = progress;

            var percent = (progress * 100) / progressMax;

            if (percent > 20)
            {
                var remaining = (100 - percent) * ((DateTime.UtcNow - progressStart) / percent);
                labelStatus.Text = $"{statusText} : {remaining.ToString(@"mm\:ss")}";
            }
            else
            {
                labelStatus.Text = statusText;
            }
            
            lblCover.Text = $"Colour coverage = {coverage}%. Virtual = {virtualCover - coverage}%.";

            if ((activeThread != null) && (!activeThread.IsAlive))
            {
                activeThread = null;
                btnCreate.Enabled = true;
                if (pictureGrid != null)
                {
                    btnUpdate.Enabled = true;
                }
            }


            UpdateDestPic();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Mat croppedMat = new Mat(outputMat, outputRoi);
            var outputName = $"{Path.GetFileNameWithoutExtension(inputPath)}_{xSize}_{outMult}_{DateTime.UtcNow:yyyyMMddHHmmss}";

            CvInvoke.Imwrite($"{outputName}.jpg", croppedMat);
        }

        private void CompositeMaker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (activeThread != null)
            {
                abortThread.Cancel();

                while (activeThread.IsAlive)
                {
                    Thread.Sleep(10);
                }
            }

            SaveHashTable();
        }

        private void SaveHashTable()
        {
            if (hashTableModified)
            {
                using (StreamWriter file = new StreamWriter("rgbhashtable.json"))
                {
                    string dictString = JsonConvert.SerializeObject(rgbHashTable);
                    file.Write(dictString);
                }

                hashTableModified = false;
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            UpdateSizes();
        }

        public void UpdateSizes()
        {
            var cols = (int)(inputMat.Cols / numScale.Value);
            var rows = (int)(inputMat.Rows / numScale.Value);
            labelSize.Text = $"[{cols} x {rows}]";

            if (checkJpg.Checked)
            {
                trackBar3.Maximum = 23170 / rows;
            }
            else
            {
                trackBar3.Maximum = 0x10000 / rows;
            }
            outMult = trackBar3.Value;
            labelMult.Text = $"x{outMult}";
            labelOutSize.Text = $"[{cols* outMult} x {rows* outMult}]";
        }

        private void trackThreshold_Scroll(object sender, EventArgs e)
        {
            threshold = trackThreshold.Value * trackThreshold.Value;
        }

        private void numericSearchSize_ValueChanged(object sender, EventArgs e)
        {
            minSearchSize = (int)numericSearchSize.Value;
        }

        private void numericMaxSearch_ValueChanged(object sender, EventArgs e)
        {
            maxSearchSize = (int)numericMaxSearch.Value;
        }

        private void checkJpg_CheckedChanged(object sender, EventArgs e)
        {
            UpdateSizes();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            btnCreate.Enabled = false;
            btnUpdate.Enabled = false;

            activeThread = new Thread(UpdateDest);
            activeThread.Start();
        }
    }
}
