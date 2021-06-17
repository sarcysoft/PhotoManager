//using Emgu.CV;
//using Emgu.CV.CvEnum;
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

        private Bitmap inputBitmap;
        private Bitmap outputBitmap;

        private Mutex outputMutex = new Mutex();

        private Rectangle outputRoi;
        private bool bOuputReady = false;

        private int maxThreads = 7;

        private Thread activeThread;

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
            Image inputImage = Image.FromFile(inputPath);

            var minsize = Math.Min(inputImage.Size.Width, inputImage.Size.Height);
            Rectangle roi = new Rectangle((inputImage.Size.Width - minsize) >> 1, (inputImage.Size.Height - minsize) >> 1, minsize, minsize);

            var scaledSize = (pictureSource.Width < pictureSource.Height) ? pictureSource.Width : pictureSource.Height;

            inputBitmap = new Bitmap(minsize, minsize);
            Bitmap scaledBitmap = new Bitmap(scaledSize, scaledSize);

            using (Graphics g = Graphics.FromImage(inputBitmap))
            {
                Bitmap src = inputImage as Bitmap;
                g.DrawImage(src, new Rectangle(0, 0, inputBitmap.Width, inputBitmap.Height),
                                 roi,
                                 GraphicsUnit.Pixel);
            }

            using (Graphics g = Graphics.FromImage(scaledBitmap))
            {
                g.DrawImage(inputBitmap, new Rectangle(0, 0, scaledSize, scaledSize));
            }

            pictureSource.Image = scaledBitmap;

            scale = minsize / scaledSize;

            xx = inputBitmap.Width / 2;
            yy = inputBitmap.Height / 2;

            numScale.Value = minsize / 250;
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

            if (xx + 1 > inputBitmap.Width)
            {
                xx = inputBitmap.Width - 1;
            }

            if (yy < 0)
            {
                yy = 0;
            }

            if (yy + 1 > inputBitmap.Height)
            {
                yy = inputBitmap.Height - 1;
            }

            Bitmap targetBitmap = new Bitmap(pictureTarget.Width, pictureTarget.Height);
            using (Graphics gfx = Graphics.FromImage(targetBitmap))
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(inputBitmap.GetPixel(xx, yy).ToArgb())))
                {
                    gfx.FillRectangle(brush, 0, 0, pictureTarget.Width, pictureTarget.Height);
                }
            }
            pictureTarget.Image = targetBitmap;
        }

        private void UpdateBestFit()
        {
            var value = inputBitmap.GetPixel(xx, yy);
            var best = FindClosest(value.R, value.G, value.B);

            var image = LoadBitmap(best, pictureBestFit.Width);
            pictureBestFit.Image = image;
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
            po.MaxDegreeOfParallelism = maxThreads;

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
                                    var image = LoadBitmap(i.Value);

                                    var value = image.GetPixel(0, 0);
                                    rgbValue = new byte[] { value.R, value.G, value.B };
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
            xSize = (int)(inputBitmap.Size.Width / outScale);
            ySize = (int)(inputBitmap.Size.Height / outScale);
            try
            {

                Bitmap scaledBitmap = new Bitmap(xSize, xSize);

                using (Graphics g = Graphics.FromImage(scaledBitmap))
                {
                    g.DrawImage(inputBitmap, new Rectangle(0, 0, xSize, xSize));
                }

                pictureGrid = new int[xSize, ySize];

                var cols = Enumerable.Range(0, xSize);
                var rows = Enumerable.Range(0, ySize);

                statusText = "Creating picture grid.";

                var rect = new Rectangle(0, 0, scaledBitmap.Width, scaledBitmap.Height);
                var data = scaledBitmap.LockBits(rect, ImageLockMode.ReadWrite, scaledBitmap.PixelFormat);
                var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8; //bytes per pixel

                var buffer = new byte[data.Width * data.Height * depth];

                //copy pixels to buffer
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

                foreach (var col in cols)
                {
                    try
                    {
                        ParallelOptions po = new ParallelOptions();
                        po.CancellationToken = abortThread.Token;
                        po.MaxDegreeOfParallelism = maxThreads;

                        Parallel.ForEach(rows, po, row =>
                        {
                            var offset = ((row * data.Width) + col) * depth;
                            pictureGrid[col, row] = FindClosest(buffer[offset+2], buffer[offset+1], buffer[offset]);

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

        private void UpdateDest()
        {
            int zoomScale = zoom * outMult;
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

                outputBitmap = new Bitmap(segs * zoomScale, segs * zoomScale);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (outputBitmap != null)
            {
                Dictionary<int, List<(int, int)>> pictureSet = new Dictionary<int, List<(int, int)>> { };

                var cols = Enumerable.Range(0, segs);
                var rows = Enumerable.Range(0, segs);

                progress = 0;

                statusText = "Sorting output images.";

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

                    progress = (row * 100) / rows.Count();
                }

                var outX = xSize * outMult;
                var outY = ySize * outMult;
                outputRoi = new Rectangle((outputBitmap.Width - outX) / 2, (outputBitmap.Height - outY) / 2, outX, outY);

                int count = 0;
                progress = 0;
                statusText = "Building output image.";
                DateTime startTime = DateTime.UtcNow;

                try
                {
                    ParallelOptions po = new ParallelOptions();
                    po.CancellationToken = abortThread.Token;
                    po.MaxDegreeOfParallelism = maxThreads;

                    using (Graphics g = Graphics.FromImage(outputBitmap))
                    {
                        Parallel.ForEach(pictureSet, po, pic =>
                        //foreach(var pic in pictureSet)
                        {
                            var image = LoadBitmap(pic.Key, zoomScale);

                            foreach (var loc in pic.Value)
                            {
                                outputMutex.WaitOne();
                                g.DrawImage(image, new Rectangle(loc.Item1 * zoomScale, loc.Item2 * zoomScale, zoomScale, zoomScale));
                                count++;
                                outputMutex.ReleaseMutex();
                            }

                            bOuputReady = true;

                            po.CancellationToken.ThrowIfCancellationRequested();

                            var currentProg = (count * 100) / (rows.Count() * cols.Count());
                            if (currentProg > progress)
                            {
                                progress = currentProg;
                                var elapsed = DateTime.UtcNow - startTime;
                                var remaining = (elapsed / progress) * (100 - progress);
                                statusText = $"Building output image. ~{remaining.ToString(@"hh\:mm\:ss")} remaining.";
                            }
                        });
                    }
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

        private Bitmap LoadBitmap(string path, int zoom = 1)
        {

            Image inputImage = Image.FromFile(path);

            var minsize = Math.Min(inputImage.Size.Width, inputImage.Size.Height);
            Rectangle roi = new Rectangle((inputImage.Size.Width - minsize) >> 1, (inputImage.Size.Height - minsize) >> 1, minsize, minsize);

            Bitmap loadedBitmap = new Bitmap(zoom, zoom);

            using (Graphics g = Graphics.FromImage(loadedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                Bitmap src = inputImage as Bitmap;
                g.DrawImage(src, new Rectangle(0, 0, zoom, zoom), roi, GraphicsUnit.Pixel);
            }

            return loadedBitmap;
        }

        private Bitmap LoadBitmap(int fileNo, int zoom = 1)
        {
            return LoadBitmap(listPhotos[fileNo], zoom);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            zoom = (int)Math.Pow(2, trackBar1.Value);
            labelZoom.Text = $"x{zoom}";
        }

        private void pictureDest_DoubleClick(object sender, EventArgs e)
        {

        }

        private void UpdateDestPic()
        {
            if (bOuputReady)
            {
                var scaledSize = (pictureDest.Width < pictureDest.Height ? pictureDest.Width : pictureDest.Height);

                Bitmap croppedBitmap = new Bitmap(scaledSize, scaledSize);

                using (Graphics g = Graphics.FromImage(croppedBitmap))
                {
                    outputMutex.WaitOne();

                    g.DrawImage(outputBitmap, new Rectangle(0, 0, scaledSize, scaledSize),
                                     outputRoi,
                                     GraphicsUnit.Pixel);
                
                    bOuputReady = false;
                    
                    outputMutex.ReleaseMutex();
                }

                pictureDest.Image = croppedBitmap;
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
            Bitmap croppedBitmap = new Bitmap(outputRoi.Width, outputRoi.Height);

            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(outputBitmap, new Rectangle(0, 0, outputRoi.Width, outputRoi.Height),
                                 outputRoi,
                                 GraphicsUnit.Pixel);
            }
            croppedBitmap.Save("composite.jpg");
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
            var cols = (int)(inputBitmap.Size.Width / numScale.Value);
            var rows = (int)(inputBitmap.Size.Width / numScale.Value);
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
