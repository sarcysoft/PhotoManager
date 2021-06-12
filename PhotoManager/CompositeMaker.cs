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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoManager
{
    public partial class CompositeMaker : Form
    {
        private string inputPath;
        private List<string> listPhotos;
        private Dictionary<string, Mat[]> splitMats = new ();

        private List<string>[,,] lookupArray = new List<string>[256,256,256];

        private Mat inputMat;
        private int segmentSize = 8;
        private int xx = 0;
        private int yy = 0;
        private int scale = 1;
        private int coverage = 0;

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

            scale = (scaleX > scaleY) ? scaleX : scaleY;

            Mat image = new Mat();
            CvInvoke.Resize(inputMat, image, new Size(inputMat.Cols / scale, inputMat.Rows / scale), 0, 0, Inter.Cubic);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", image, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureSource.Image = Image.FromStream(stream);

            UpdateTarget();
            UpdateBestFit();

            BuildLookupArray();
        }

        private void UpdateTarget()
        {
            segmentSize = (int)numSegmentSize.Value;

            if (xx < 0)
            {
                xx = 0;
            }

            if (xx + segmentSize > inputMat.Cols)
            {
                xx = inputMat.Cols - segmentSize;
            }

            if (yy < 0)
            {
                yy = 0;
            }

            if (yy + segmentSize > inputMat.Rows)
            {
                yy = inputMat.Rows - segmentSize;
            }

            Rectangle roi = new Rectangle(xx, yy, segmentSize, segmentSize);

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
            var image = CvInvoke.Imread(best);
            CvInvoke.Resize(image, image, new Size(1, 1), 0, 0, Inter.Linear);
            CvInvoke.Resize(image, image, new Size(pictureBestFit.Width, pictureBestFit.Height), 0, 0, Inter.Area);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", image, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureBestFit.Image = Image.FromStream(stream);
        }

        private void numSegmentSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateTarget();
            splitMats = new();
        }

        private void pictureSource_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            xx = (me.Location.X * scale) - (scale / 2);
            yy = (me.Location.Y * scale) - (scale / 2);

            UpdateTarget();
            UpdateBestFit();
        }

        private void pictureBestFit_Click(object sender, EventArgs e)
        {

        }

        private void BuildLookupArray()
        {
            int count = 0;
            int unique = 0;

            foreach (var photo in listPhotos)
            {
                try
                {
                    var image = CvInvoke.Imread(photo);
                    CvInvoke.Resize(image, image, new Size(1, 1), 0, 0, Inter.Linear);
                    //Vec3b bgrPixel = foo.at<Vec3b>(i, j);
                    var value = image.GetRawData(0, 0);
                    if (value != null)
                    {
                        var r = value[0];
                        var g = value[1];
                        var b = value[2];
                        if (lookupArray[r,g,b] == null)
                        {
                            Console.WriteLine($"New colour [{r}, {g}, {b}]");
                            lookupArray[r, g, b] = new List<string> { };
                            unique++;
                        }
                        count++;
                        lookupArray[r,g,b].Add(photo);
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
            return v >= (inputMat.Cols - s) ? inputMat.Cols : v + s;
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
                                lookupList.AddRange(lookupArray[x, y, z]);
                            }
                        }
                    }
                }
                searchSize++;
            }

            if (lookupList.Count > 0)
            {
                var rand = new Random();
                int x = rand.Next(0, lookupList.Count);
                path = lookupList[x];
            }

            return path;
        }
    }
}
