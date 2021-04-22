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

    public class PhotoDetails
    {
        public string filename;
        public string path;
        public DateTime dateTime;
        public long size;
    }

    public partial class Form1 : Form
    {
        private List<string> listPhotos;

        private Dictionary<string, PhotoDetails> dictPhotos;

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

        private void btnScan_Click(object sender, EventArgs e)
        {
            listPhotos = new List<string> { };
            dictPhotos = new Dictionary<string, PhotoDetails> { };

            Recurse(txtPath.Text);
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

                lstFiles.Items.Clear();

                int duplicates = 0;

                foreach(var photo in listPhotos)
                {
                    FileInfo fi = new FileInfo(photo);

                    PhotoDetails details = new PhotoDetails();
                    details.filename = fi.Name;
                    details.dateTime = fi.CreationTime;
                    details.path = fi.DirectoryName;
                    details.size = fi.Length;

                    string hash = $"{details.filename}{details.dateTime.ToString()}{details.size}";

                    if (dictPhotos.ContainsKey(hash))
                    {
                        duplicates++;
                    }
                    else
                    {
                        ListViewItem item = new ListViewItem();
                        string[] data = { fi.DirectoryName, fi.CreationTime.ToString(), fi.Length.ToString() };
                        lstFiles.Items.Add(fi.Name).SubItems.AddRange(data);
                    }
                }

                txtStatus.Text = $"{listPhotos.Count} files found. {duplicates} duplicates, {listPhotos.Count - duplicates} unique.";
            }
            catch (Exception e)
            {

            }
        }
    }
}
