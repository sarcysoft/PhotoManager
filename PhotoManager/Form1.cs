﻿using System;
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
            char[] trimChars = new char[]  {'\\'};

            listPhotos = new List<string> { };
            dictPhotos = new Dictionary<string, PhotoDetails> { };

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

                nodes[0].Nodes.Add(hash, details.filename);
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
    }

    public class PhotoDetails
    {
        public string filename;
        public string path;
        public DateTime dateTime;
        public long size;
    }
}
