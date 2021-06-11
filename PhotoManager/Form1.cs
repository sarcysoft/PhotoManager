using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoManager
{
    public partial class Form1 : Form
    {
        private List<string> listPhotos;

        private Dictionary<string, PhotoDetails> dictPhotos;
        //public ImageList iconList { get; set; }

        public Dictionary<string, Mat[]> splitMats;
        public Dictionary<string, Mat> imageMats;

        public int segmentSize = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowser.SelectedPath;
            }
        }

        private string GetHash(FileInfo fi)
        {
            byte[] md5Hash;

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fi.FullName))
                {
                    md5Hash = md5.ComputeHash(stream);
                }
            }

            string hash = $"{fi.Length}{BitConverter.ToString(md5Hash).Replace("-", "")}";

            return hash;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            char[] trimChars = new char[] { '\\' };

            listPhotos = new List<string> { };
            dictPhotos = new Dictionary<string, PhotoDetails> { };

            Recurse(txtPath.Text);

            int duplicates = 0;

            //iconList = new();
            imageMats = new();
            splitMats = new();

            //treeFiles.ImageList = iconList;

            treeFiles.Nodes.Clear();

            treeFiles.BeginUpdate();
            treeFiles.Nodes.Add(txtPath.Text.TrimEnd(trimChars), txtPath.Text);

            string lastPath = "";
            TreeNode[] nodes = null;

            foreach (var photo in listPhotos)
            {
                FileInfo fi = new FileInfo(photo);

                PhotoDetails details = new PhotoDetails();
                details.filename = fi.Name;
                details.dateTime = fi.CreationTime;
                details.path = fi.DirectoryName;
                details.size = fi.Length;

                string hash = GetHash(fi);

                if (dictPhotos.ContainsKey(hash))
                {
                    duplicates++;
                }
                else
                {
                    dictPhotos[hash] = details;
                }

                if (details.path != lastPath)
                {
                    nodes = treeFiles.Nodes.Find(details.path.TrimEnd(trimChars), true);
                    if (nodes.Length <= 0)
                    {
                        string tempPathL = txtPath.Text.Trim(trimChars);
                        string tempPathR = details.path.Substring(tempPathL.Length).Trim(trimChars) + "\\";

                        do
                        {
                            string tempPath = tempPathR.Substring(0, tempPathR.IndexOf("\\"));

                            TreeNode[] pathNodes = treeFiles.Nodes.Find(tempPathL + "\\" + tempPath, true);
                            if (pathNodes.Length <= 0)
                            {
                                pathNodes = treeFiles.Nodes.Find(tempPathL, true);
                                if (pathNodes.Length > 0)
                                {
                                    tempPathR = tempPathR.Substring(tempPathR.IndexOf("\\")).TrimStart(trimChars);
                                    tempPathL = tempPathL + "\\" + tempPath;
                                    pathNodes[0].Nodes.Add(tempPathL, tempPath);
                                }
                            }
                            else
                            {
                                tempPathR = tempPathR.Substring(tempPathR.IndexOf("\\")).TrimStart(trimChars);
                                tempPathL = tempPathL + "\\" + tempPath;
                            }

                            nodes = treeFiles.Nodes.Find(details.path.TrimEnd(trimChars), true);
                        }
                        while (nodes.Length <= 0);
                    }
                }

                var fullPath = $"{details.path}\\{details.filename}";

                //int imageIndex = -1;
                try
                {
                    var image = CvInvoke.Imread(fullPath);
                    CvInvoke.Resize(image, image, new Size(segmentSize, segmentSize), 0, 0, Inter.Cubic);

                    imageMats[fullPath] = image;
                    splitMats[fullPath] = image.Split();
                    /*
                    Mat icon = new Mat();
                    CvInvoke.Resize(image, icon, new Size(32, 32), 0, 0, Inter.Cubic);
                    Emgu.CV.Util.VectorOfByte buf = new();
                    CvInvoke.Imencode(".jpg", icon, buf);
                    MemoryStream stream = new MemoryStream(buf.ToArray());
                    iconList.Images.Add(Image.FromStream(stream));
                    imageIndex = iconList.Images.Count - 1;
                    */
                }
                catch (Exception e2)
                {

                }

                TreeNode newNode = new TreeNode(hash);
                newNode.Text = details.filename;
                //newNode.ImageIndex = imageIndex;

                nodes[0].Nodes.Add(newNode);

                lastPath = details.path;
            }

            treeFiles.EndUpdate();
        }

        private void Recurse(string path)
        {
            try
            {
                EnumerationOptions enumerationOptions = new EnumerationOptions();
                enumerationOptions.RecurseSubdirectories = true;

                foreach (var f in Directory.GetFiles(path, "*.jpg", enumerationOptions))
                {
                    listPhotos.Add(f);
                }

                foreach (var f in Directory.GetFiles(path, "*.jpeg", enumerationOptions))
                {
                    listPhotos.Add(f);
                }

                listPhotos.Sort();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ShowImg(string fullPath)
        {
            int scale = 4;
            var image = CvInvoke.Imread(fullPath);
            CvInvoke.Resize(image, image, new Size(image.Cols / scale, image.Rows / scale), 0, 0, Inter.Cubic);
            CvInvoke.Imshow($"Original", image);

            var newImage = CreateComposite(fullPath);
            CvInvoke.Imshow($"Composite", newImage);
        }

        private void treeFiles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                // Look for a file extension.
                if (e.Node.Text.Contains("."))
                {
                    ShowImg(e.Node.FullPath.ToLower());
                }
            }
            // If the file is not found, handle the exception and inform the user.
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("File not found.");
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

        string FindMostSimilar(Mat image)
        {
            double similarity = 0xffffffff;
            string path = "";

            foreach (var img in splitMats)
            {
                var sim = GetSimilarity(image, img.Value);
                if (sim < similarity)
                {
                    path = img.Key;
                    similarity = sim;
                }
            }

            return path;
        }

        void CalculateSegment(int x, int y, Mat A, Mat B)

        {
            var yy = y * segmentSize;
            var xx = x * segmentSize;
            Rectangle roi = new Rectangle(xx, yy, segmentSize, segmentSize);

            Mat srcSeg = new Mat(A, roi);
            Mat dstSeg = new Mat(B, roi);

            var sim = FindMostSimilar(srcSeg);

            imageMats[sim].CopyTo(dstSeg);
        }

        Mat CreateComposite(string fullPath)
        {
            int scale = 4;
            var image = CvInvoke.Imread(fullPath);
            CvInvoke.Resize(image, image, new Size(image.Cols/scale, image.Rows/scale), 0, 0, Inter.Cubic);

            Mat newImage = new Mat(image.Rows, image.Cols, image.Depth, image.NumberOfChannels);

            var xSegments = image.Cols / segmentSize;
            var ySegments = image.Rows / segmentSize;

            for (var y = 0; y < ySegments; y++)
            {
                for (var x = 0; x < xSegments; x++)
                {
                    CalculateSegment(x, y, image, newImage);
                }
            }

            return newImage;
        }
    }

    public class PhotoDetails
    {
        public string filename;
        public string path;
        public DateTime dateTime;
        public long size;
    }

}
