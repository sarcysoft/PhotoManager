using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
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

        private Mat inputMat;
        private int segmentSize = 2;
        private int xx = 0;
        private int yy = 0;
        private int scale = 1;

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
        }

        private void UpdateTarget()
        {
            segmentSize = (int)numSegmentSize.Value;

            if (xx + segmentSize > inputMat.Cols)
            {
                xx = inputMat.Cols - segmentSize;
            }

            if (yy + segmentSize > inputMat.Rows)
            {
                yy = inputMat.Rows - segmentSize;
            }

            Rectangle roi = new Rectangle(xx, yy, segmentSize, segmentSize);

            Mat target = new Mat(inputMat, roi);
            CvInvoke.Resize(target, target, new Size(pictureTarget.Width, pictureTarget.Height), 0, 0, Inter.Cubic);
            Emgu.CV.Util.VectorOfByte buf = new();
            CvInvoke.Imencode(".jpg", target, buf);
            MemoryStream stream = new MemoryStream(buf.ToArray());
            pictureTarget.Image = Image.FromStream(stream);
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
        }

        private void pictureBestFit_Click(object sender, EventArgs e)
        {
            string best = "";
            double similarity = 0xffffffff;
            Rectangle roi = new Rectangle(xx, yy, segmentSize, segmentSize);

            Mat target = new Mat(inputMat, roi);

            foreach (var photo in listPhotos)
            {
                try
                {
                    var image = CvInvoke.Imread(photo);
                    CvInvoke.Resize(image, image, new Size(segmentSize, segmentSize), 0, 0, Inter.Cubic);
                    var imgsplit = image.Split();

                    var sim = GetSimilarity(target, imgsplit);
                    if (sim < similarity)
                    {
                        best = photo;
                        similarity = sim;
                    }
                }
                catch (Exception ex)
                {

                }
            }

            if (best != "")
            {
                var image = CvInvoke.Imread(best);
                CvInvoke.Resize(image, image, new Size(segmentSize, segmentSize), 0, 0, Inter.Cubic);
                CvInvoke.Resize(image, image, new Size(pictureBestFit.Width, pictureBestFit.Height), 0, 0, Inter.Cubic);
                Emgu.CV.Util.VectorOfByte buf = new();
                CvInvoke.Imencode(".jpg", image, buf);
                MemoryStream stream = new MemoryStream(buf.ToArray());
                pictureBestFit.Image = Image.FromStream(stream);
            }
        }

        double GetSimilarity(Mat A, Mat[] B)
        {
            double similarity = 1;

            Mat[] chanA = A.Split();


            for (var i = 0; i < chanA.Length; i++)
            {
                // Calculate the L2 relative error between images.
                double norm = CvInvoke.Norm(chanA[i], B[i], NormType.C);
                // Convert to a reasonable scale, since L2 error is summed across all pixels of the image.
                similarity += (norm / (double)(A.Rows * A.Cols));
            }

            return similarity;
        }
    }
}
