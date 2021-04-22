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
    public partial class Form1 : Form
    {
        private List<string> listPhotos;

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

                foreach(var photo in listPhotos)
                {
                    FileInfo fi = new FileInfo(photo);

                    ListViewItem item = new ListViewItem();
                    string[] details = { fi.DirectoryName, fi.CreationTime.ToString(), fi.Length.ToString() };
                    lstFiles.Items.Add(fi.Name).SubItems.AddRange(details);
                }

                txtStatus.Text = $"{listPhotos.Count} files found.";
            }
            catch (Exception e)
            {

            }
        }
    }
}
