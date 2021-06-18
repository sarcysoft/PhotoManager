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
        private Dictionary<string, byte[]> rgbHashTable;
        private bool hashTableModified = false;

        public class ColourDetails
        {
            public bool Actual;
            public List<int> Photos;
        }

        private const int colourScale = 1;

        private ColourDetails[,,] lookupArray = new ColourDetails[(256 >> colourScale), (256 >> colourScale), (256 >> colourScale)];
        private int[,] pictureGrid;

        private Mat inputMat;
        private Mat outputMat;

        private Mutex outputMutex = new Mutex();

        private Rectangle outputRoi;
        private bool bOuputReady = false;

        private Thread activeThread;
        private List<Thread> precacheThreads;
        private bool precacheActive;

        private int xx = 0;
        private int yy = 0;
        private int scale = 1;
        private int xSize, ySize;
        private int outScale = 2;

        private int photoCount = 0;
        private int colours = 0;
        private int coverage = 0;
        private int virtualCover = 0;


        private int progress = 0;
        private int zoom = 1;
        private int outMult = 1;
        private int zoomScale = 1;

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

            scale = minsize / scaledSize;

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
            var value = inputMat.GetRawData(xx, yy);
            var best = FindClosest(value[0], value[1], value[2]);
            var bestMat = LoadMat(best, pictureBestFit.Width);

            CvInvoke.Resize(bestMat, bestMat, new Size(pictureBestFit.Width, pictureBestFit.Height), 0, 0, Inter.Area);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", bestMat, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureBestFit.Image = Image.FromStream(stream);

        }

        private void numSegmentSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateTarget();
        }

        private void pictureSource_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            xx = me.Location.X * scale;
            yy = me.Location.Y * scale;

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

            rgbHashTable = new Dictionary<string, byte[]> { };

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
            //po.MaxDegreeOfParallelism = 7;

            try
            { 
                if (File.Exists("rgbhashtable.json"))
                {
                    using (StreamReader file = new StreamReader("rgbhashtable.json"))
                    {
                        string dictString = file.ReadToEnd();
                        rgbHashTable = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(dictString);

                        foreach(var i in rgbHashTable)
                        {
                            if (hashPhotos.ContainsKey(i.Key))
                            {
                                byte[] rgbValue = i.Value; ;
                                if (rgbValue != null)
                                {
                                    int r = (int)rgbValue[0] >> colourScale;
                                    int g = (int)rgbValue[1] >> colourScale;
                                    int b = (int)rgbValue[2] >> colourScale;

                                    if (lookupArray[r, g, b] == null)
                                    {
                                        //Console.WriteLine($"New colour [{r}, {g}, {b}]");
                                        lookupArray[r, g, b] = new ColourDetails();
                                        lookupArray[r, g, b].Actual = true;
                                        lookupArray[r, g, b].Photos = new List<int> { };
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

                //iRange = Enumerable.Range(0, listPhotos.Count);

                Parallel.ForEach(hashPhotos, po, i =>
                {
                    if (i.Value != null)
                    {
                        try
                        {
                            byte[] rgbValue = null;

                            try
                            {
                                if (rgbHashTable.ContainsKey(i.Key))
                                {
                                    rgbValue = rgbHashTable[i.Key];
                                }
                                else
                                {
                                    var image = LoadMat(i.Value);

                                    var value = image.GetRawData(0, 0);
                                    rgbValue = new byte[] { value[0], value[1], value[2] };
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
                                    int r = (int)rgbValue[0] >> colourScale;
                                    int g = (int)rgbValue[1] >> colourScale;
                                    int b = (int)rgbValue[2] >> colourScale;

                                    if (lookupArray[r, g, b] == null)
                                    {
                                        //Console.WriteLine($"New colour [{r}, {g}, {b}]");
                                        lookupArray[r, g, b] = new ColourDetails();
                                        lookupArray[r, g, b].Actual = true;
                                        lookupArray[r, g, b].Photos = new List<int> { };
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

                    progress = (photoCount * 100) / listPhotos.Count;
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
            return v < s ? 0 : v - s;
        }

        private int FindMax(int v, int s)
        {
            var limit = (256 >> colourScale) - 1;
            return v >= (limit - s) ? limit : v + s;
        }

        private int FindClosest(int r, int g, int b)
        {
            int best = -1;

            r >>= colourScale;
            g >>= colourScale;
            b >>= colourScale;

            List<int> lookupList = new List<int>() { };

            int searchSize = 0;

            while (lookupList.Count < ((listPhotos.Count < 1000) ? 1 : 50))
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
                                            if (!lookupList.Contains(photo))
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

                    progress = (col * 100) / cols.Count();
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

        Queue<int> precacheQueue = new Queue<int>(10000) { };

        private void PrecacheTask()
        {
            precacheActive = true;

            do
            {
                if (precacheQueue.Count > 0)
                {
                    var index = precacheQueue.Dequeue();
                    LoadMat(index, zoomScale, true);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
            while (precacheActive);
        }

        private void BuildOutput(SectionDetails details, int zoomScale, ParallelOptions po)
        {
            if (outputMat != null)
            {
                Dictionary<int, List<(int, int)>> pictureSet = new Dictionary<int, List<(int, int)>> { };

                var cols = Enumerable.Range(0, details.xSegs);
                var rows = Enumerable.Range(0, details.ySegs);

                foreach (var row in rows)
                {
                    foreach (var col in cols)
                    {
                        var index = pictureGrid[details.xOffset + col, details.yOffset + row];
                        if (!pictureSet.ContainsKey(index))
                        {
                            pictureSet[index] = new List<(int, int)> { };
                            if (precacheQueue.Count < 10000)
                            {
                                precacheQueue.Enqueue(index);
                            }
                        }

                        pictureSet[index].Add((col, row));
                    }
                }

                var localMat = new Mat(new Size(details.xSegs * zoomScale, details.ySegs * zoomScale), inputMat.Depth, inputMat.NumberOfChannels);
                foreach (var pic in pictureSet)
                {
                    var image = LoadMat(pic.Key, zoomScale);

                    foreach (var loc in pic.Value)
                    {
                        Rectangle localRoi = new Rectangle(loc.Item1 * zoomScale, loc.Item2 * zoomScale, zoomScale, zoomScale);
                        Mat localTarget = new Mat(outputMat, localRoi);
                        image.CopyTo(localTarget);
                    }

                    po.CancellationToken.ThrowIfCancellationRequested();
                }

                //outputMutex.WaitOne();

                Rectangle roi = new Rectangle(details.xOffset * zoomScale, details.yOffset * zoomScale, details.xSegs * zoomScale, details.ySegs * zoomScale);
                Mat roiTarget = new Mat(outputMat, roi);
                localMat.CopyTo(roiTarget);

                bOuputReady = true;

                //outputMutex.ReleaseMutex();
            }
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

                int count = 0;
                progress = 0;
                statusText = "Building output image.";
                DateTime startTime = DateTime.UtcNow;

                precacheThreads = new List<Thread>() { };

                for (int i = 0; i < 1; i++)
                {
                    var t = new Thread(PrecacheTask);
                    t.Start();
                    precacheThreads.Add(t);
                }

                try
                {
                    ParallelOptions po = new ParallelOptions();
                    po.CancellationToken = abortThread.Token;
                    po.MaxDegreeOfParallelism = 6;

                    const int sectSize = 5;
                    var sections = segs / sectSize;
                    if (sections * sectSize < segs)
                    {
                        sections++;
                    }

                    var sectionRange = Enumerable.Range(0, sections);
                    List<SectionDetails> sectionList = new() { };

                    foreach(var xSect in sectionRange)
                    {
                        foreach (var ySect in sectionRange)
                        {
                            SectionDetails details = new SectionDetails();
                            details.xOffset = xSect * sectSize;
                            details.yOffset = ySect * sectSize;
                            details.xSegs = (xSect * sectSize < xSize - sectSize) ? sectSize : xSize - (xSect * sectSize);
                            details.ySegs = (ySect * sectSize < ySize - sectSize) ? sectSize : ySize - (ySect * sectSize);

                            sectionList.Add(details);
                        }
                    }

                    Parallel.ForEach(sectionList, po, sect =>
                    {
                        BuildOutput(sect, zoomScale, po);
                        progress = (100 * count++) / (sections * sections);
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
                    precacheActive = false;
                }


                TimeSpan buildTime = DateTime.UtcNow.Subtract(startTime);
                statusText = $"Build completed in {buildTime.Minutes}m {buildTime.Seconds}s.";
            }

            progress = 0;
        }

        ObjectCache bitmapCache = MemoryCache.Default;
        int cacheHit = 0;
        int cacheMiss = 0;

        Mutex loadMutex = new Mutex();

        private Mat LoadMat(string path, int zoom = 1, bool cacheOnly = false)
        {
            var cacheKey = $"{path}_{zoom}";
            Mat tempMat = bitmapCache[cacheKey] as Mat;
            Mat loadedMat = null;

            if (tempMat == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();

                tempMat = CvInvoke.Imread(path);

                var minsize = Math.Min(tempMat.Cols, tempMat.Rows);
                Rectangle roi = new Rectangle((tempMat.Cols - minsize) >> 1, (tempMat.Rows - minsize) >> 1, minsize, minsize);

                CvInvoke.Resize(tempMat, tempMat, new Size(zoom, zoom), 0, 0, Inter.Area);

                bitmapCache.Set(cacheKey, tempMat, policy);

                cacheMiss++;
            }
            else
            {
                cacheHit++;
            }
            /*
            if (!cacheOnly)
            {
                loadMutex.WaitOne();
                loadedMat = tempMat.Clone();
                loadMutex.ReleaseMutex();
            }
            */
            return tempMat;
        }

        private Mat LoadMat(int fileNo, int zoom = 1, bool cacheOnly = false)
        {
            return LoadMat(listPhotos[fileNo], zoom, cacheOnly);
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
            progressBar1.Value = progress;
            labelStatus.Text = statusText;
            
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
            CvInvoke.Imwrite("composite.jpg", croppedMat);
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

            trackBar3.Maximum = 23170 / rows;
            outMult = trackBar3.Value;
            labelMult.Text = $"x{outMult}";
            labelOutSize.Text = $"[{cols* outMult} x {rows* outMult}]";

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
