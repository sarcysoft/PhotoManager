using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace PhotoManager
{
    public partial class Form1 : Form
    {
        private List<string> listPhotos;

        //private Dictionary<string, PhotoDetails> dictPhotos;
        public ImageList iconList { get; set; }


        public Form1()
        {
            InitializeComponent();
            iconList = new();
            listView1.LargeImageList = iconList;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowser.SelectedPath;
                ScanFolders();
            }
        }

        private string GetHash(FileInfo fi)
        {
#if false
            byte[] md5Hash;

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fi.FullName))
                {
                    md5Hash = md5.ComputeHash(stream);
                }
            }

            string hash = $"{fi.Length}{BitConverter.ToString(md5Hash).Replace("-", "")}";
#else
            string hash = $"{fi.Length}{fi.FullName.GetHashCode()}";
#endif
            return hash;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
        }

        private void ScanFolders()
        { 
            char[] trimChars = new char[] { '\\' };

            listPhotos = new List<string> { };
            //dictPhotos = new Dictionary<string, PhotoDetails> { };

            Recurse(txtPath.Text);

            int duplicates = 0;

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
                /*
                if (dictPhotos.ContainsKey(hash))
                {
                    duplicates++;
                }
                else
                {
                    dictPhotos[hash] = details;
                }
                */

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
                
                foreach (var f in Directory.GetFiles(path, "*.png", enumerationOptions))
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

        private void treeFiles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                // Look for a file extension.
                if (e.Node.Text.Contains("."))
                {
                    var item = new ListViewItem();
                    item.Text = Path.GetFileName(e.Node.FullPath);
                    item.Tag = e.Node.FullPath.ToLower();

                    Mat inputMat = CvInvoke.Imread(e.Node.FullPath);

                    var scaledX = inputMat.Cols / (double)32;
                    var scaledY = inputMat.Rows / (double)32;
                    var scaledSize = (scaledX > scaledY) ? scaledX : scaledY;

                    CvInvoke.Resize(inputMat, inputMat, new Size((int)(inputMat.Cols / scaledSize), (int)(inputMat.Rows / scaledSize)), 0, 0, Inter.Area);
                    Emgu.CV.Util.VectorOfByte buf = new();
                    CvInvoke.Imencode(".jpg", inputMat, buf);
                    MemoryStream stream = new MemoryStream(buf.ToArray());

                    item.ImageIndex = iconList.Images.Count;
                    iconList.Images.Add(Image.FromStream(stream));

                    listView1.Items.Add(item);
                }
            }
            // If the file is not found, handle the exception and inform the user.
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("File not found.");
            }
        }

        private void buttonBuild_Click(object sender, EventArgs e)
        {
            List<string> fileList = new () { };

            foreach(ListViewItem item in listView1.Items)
            {
                fileList.Add(item.Tag.ToString());
            }

            if (fileList.Count > 0)
            {

                try
                {
                    var compDlg = new CompositeMaker();

                    compDlg.SetSourceFiles(fileList);
                    compDlg.SetPhotoList(listPhotos);
                    compDlg.ShowDialog();
                }
                // If the file is not found, handle the exception and inform the user.
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show("File not found.");
                }
            }
        }

        private void treeFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Text.Contains("."))
            {
                Mat inputMat = CvInvoke.Imread(e.Node.FullPath);

                var scaledX = inputMat.Cols / (double)pictureBox1.Width;
                var scaledY = inputMat.Rows / (double)pictureBox1.Height;
                var scaledSize = (scaledX > scaledY) ? scaledX : scaledY;

                CvInvoke.Resize(inputMat, inputMat, new Size((int)(inputMat.Cols/scaledSize), (int)(inputMat.Rows/scaledSize)), 0, 0, Inter.Area);
                Emgu.CV.Util.VectorOfByte buf = new();
                CvInvoke.Imencode(".jpg", inputMat, buf);
                MemoryStream stream = new MemoryStream(buf.ToArray());
                pictureBox1.Image = Image.FromStream(stream);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var path = listView1.SelectedItems[0].Tag.ToString();
                if (path != null)
                {
                    Mat inputMat = CvInvoke.Imread(path);

                    var scaledX = inputMat.Cols / (double)pictureBox1.Width;
                    var scaledY = inputMat.Rows / (double)pictureBox1.Height;
                    var scaledSize = (scaledX > scaledY) ? scaledX : scaledY;

                    CvInvoke.Resize(inputMat, inputMat, new Size((int)(inputMat.Cols / scaledSize), (int)(inputMat.Rows / scaledSize)), 0, 0, Inter.Area);
                    Emgu.CV.Util.VectorOfByte buf = new();
                    CvInvoke.Imencode(".jpg", inputMat, buf);
                    MemoryStream stream = new MemoryStream(buf.ToArray());
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
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
