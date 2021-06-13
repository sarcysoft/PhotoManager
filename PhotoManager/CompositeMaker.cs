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

namespace PhotoManager
{
    public partial class CompositeMaker : Form
    {
        private string inputPath;
        private List<string> listPhotos;

        public class ColourDetails
        {
            public bool Actual;
            public List<string> Photos;
        }

        private ColourDetails[,,] lookupArray = new ColourDetails[256,256,256];
        private Dictionary<string, Mat> pixelMats;
        private string[,] pictureGrid;

        private Mat inputMat;
        private Mat outputMat;

        private int xx = 0;
        private int yy = 0;
        private int scale = 10;
        private int coverage = 0;

        private int progress = 0;
        private int zoom = 1;

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
            var scaleX = inputMat.Cols / pictureSource.Size.Width;
            var scaleY = inputMat.Rows / pictureSource.Size.Height;

            xx = inputMat.Cols / 2;
            yy = inputMat.Rows / 2;

            scale = (scaleX > scaleY) ? scaleX : scaleY;

            Mat image = new Mat();
            CvInvoke.Resize(inputMat, image, new Size(inputMat.Cols / scale, inputMat.Rows / scale), 0, 0, Inter.Area);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", image, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureSource.Image = Image.FromStream(stream);

            labelSize.Text = $"[{(int)(inputMat.Cols / numScale.Value)} x {(int)(inputMat.Rows / numScale.Value)}]";

            trackBar1.Value = 1;

            BuildLookupArray();

            UpdateTarget();
            UpdateBestFit();
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

            Mat target = new Mat(inputMat, roi);
            CvInvoke.Resize(target, target, new Size(pictureTarget.Width, pictureTarget.Height), 0, 0, Inter.Area);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", target, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureTarget.Image = Image.FromStream(stream);
        }

        private void UpdateBestFit()
        {
            var value = inputMat.GetRawData(yy, xx);
            var best = FindClosest(value[0], value[1], value[2]);
            var image = pixelMats[best];
            CvInvoke.Resize(image, image, new Size(pictureBestFit.Width, pictureBestFit.Height), 0, 0, Inter.Area);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", image, buf);
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
            xx = (me.Location.X * scale) - (scale / 2);
            yy = (me.Location.Y * scale) - (scale / 2);

            UpdateTarget();
            UpdateBestFit();
        }

        private void BuildLookupArray()
        {
            int count = 0;
            int unique = 0;

            pixelMats = new Dictionary<string, Mat>() { };

            foreach (var photo in listPhotos)
            {
                try
                {
                    var image = CvInvoke.Imread(photo);
                    CvInvoke.Resize(image, image, new Size(1, 1), 0, 0, Inter.Area);

                    pixelMats[photo] = image;

                    var value = image.GetRawData(0, 0);
                    if (value != null)
                    {
                        var r = value[0];
                        var g = value[1];
                        var b = value[2];
                        if (lookupArray[r,g,b] == null)
                        { 
                            Console.WriteLine($"New colour [{r}, {g}, {b}]");
                            lookupArray[r, g, b] = new ColourDetails();
                            lookupArray[r, g, b].Actual = true;
                            lookupArray[r, g, b].Photos = new List<string> { };
                            unique++;
                        }
                        count++;
                        lookupArray[r,g,b].Photos.Add(photo);
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine($"Photos = {count};\nUnique = {unique}");
            coverage = (100 * unique) / (256 * 256 * 256);
            Console.WriteLine($"Coverage = {coverage}%");
        }

        private int FindMin(int v, int s)
        {
            return v < s ? 0 : v - s;
        }

        private int FindMax(int v, int s)
        {
            return v >= (255 - s) ? 255 : v + s;
        }

        private string FindClosest(int r, int g, int b)
        {
            string path = null;

            List<string> lookupList = new List<string>() { };

            int searchSize = 0;

            while (lookupList.Count == 0)
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
                            if (lookupArray[x,y,z] != null)
                            {
                                if ((searchSize == 0) || (lookupArray[x, y, z].Actual == true))
                                {
                                    lookupList.AddRange(lookupArray[x, y, z].Photos);
                                }
                            }
                        }
                    }
                }
                searchSize++;
            }

            if (searchSize > 1)
            {
                lookupArray[r, g, b] = new();
                lookupArray[r, g, b].Actual = false;
                lookupArray[r, g, b].Photos = lookupList;
            }

            if (lookupList.Count > 0)
            {
                var rand = new Random();
                int x = rand.Next(0, lookupList.Count);
                path = lookupList[x];
            }

            return path;
        }

        private Thread createThread;

        private void Create()
        {
            int outscale = (int)numScale.Value;

            var xSize = (int)(inputMat.Cols / numScale.Value);
            var ySize = (int)(inputMat.Rows / numScale.Value);
            
            trackBar1.Value = 1;

            outputMat = new Mat(new Size(xSize, ySize), inputMat.Depth, inputMat.NumberOfChannels);
            Mat scaledMat = new();
            CvInvoke.Resize(inputMat, scaledMat, new Size(outputMat.Cols, outputMat.Rows), 0, 0, Inter.Area);

            pictureGrid = new string[outputMat.Cols, outputMat.Rows];


            var rows = Enumerable.Range(0, scaledMat.Rows);
            var cols = Enumerable.Range(0, scaledMat.Cols);

            foreach (var col in cols)
            {
                Parallel.ForEach(rows, row =>
                {
                    var value = scaledMat.GetRawData(row, col);
                    pictureGrid[col, row] = FindClosest(value[0], value[1], value[2]);

                    var image = pixelMats[pictureGrid[col, row]];

                    Rectangle roi = new Rectangle(col * zoom, row * zoom, zoom, zoom);

                    Mat target = new Mat(outputMat, roi);
                    image.CopyTo(target);
                });

                progress = (col * 100) / cols.Count();
            }

            progress = 0;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            btnCreate.Enabled = false;
            btnUpdate.Enabled = false;

            createThread = new Thread(Create);
            createThread.Start();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var cols = (int)(inputMat.Cols / numScale.Value);
            var rows = (int)(inputMat.Rows / numScale.Value);
            labelSize.Text = $"[{cols} x {rows}]";
        }

        private void UpdateDest()
        {
            var x = (outputMat.Cols / 2) - (outputMat.Cols / (2 * zoom));
            var y = (outputMat.Rows / 2) - (outputMat.Rows / (2 * zoom));

            if (outputMat != null)
            {
                var rows = Enumerable.Range(0, outputMat.Rows/zoom);
                var cols = Enumerable.Range(0, outputMat.Cols/zoom);

                foreach(var row in rows)
                {
                    Parallel.ForEach(cols, col =>
                    {
                        Mat image;
                        if (zoom == 1)
                        {
                            image = pixelMats[pictureGrid[col+x, row+y]];
                        }
                        else
                        {
                            image = CvInvoke.Imread(pictureGrid[col+x, row+y]);
                            CvInvoke.Resize(image, image, new Size(zoom, zoom), 0, 0, Inter.Area);
                        }

                        Rectangle roi = new Rectangle(col*zoom, row*zoom, zoom, zoom);

                        Mat target = new Mat(outputMat, roi);
                        image.CopyTo(target);
                    });

                    progress = (row * 100) / rows.Count(); 
                }
            }

            progress = 0;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            labelZoom.Text = $"x{trackBar1.Value}";
            zoom = trackBar1.Value;
        }

        private void pictureDest_DoubleClick(object sender, EventArgs e)
        {
            CvInvoke.Imshow("Composite", outputMat);
        }

        private void UpdateDestPic()
        {
            if (outputMat != null)
            {
                Mat image = new Mat();
                CvInvoke.Resize(outputMat, image, new Size(inputMat.Cols / scale, inputMat.Rows / scale), 0, 0, Inter.Area);
                Emgu.CV.Util.VectorOfByte buf = new();
                CvInvoke.Imencode(".jpg", image, buf);
                MemoryStream stream = new MemoryStream(buf.ToArray());
                pictureDest.Image = Image.FromStream(stream);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = progress;

            if ((createThread != null) && (!createThread.IsAlive))
            {
                createThread = null;
                btnCreate.Enabled = true;
                btnUpdate.Enabled = true;
            }

            if ((updateThread != null) && (!updateThread.IsAlive))
            {
                updateThread = null;
                btnCreate.Enabled = true;
                btnUpdate.Enabled = true;
            }

            UpdateDestPic();
        }

        private Thread updateThread;

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            btnCreate.Enabled = false;
            btnUpdate.Enabled = false;

            updateThread = new Thread(UpdateDest);
            updateThread.Start();
        }
    }
}
