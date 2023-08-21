using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using Emgu.CV.ImgHash;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Data;

namespace PhotoManager
{
    public partial class Form1 : Form
    {
        private static List<string> listPhotos;
        private static ConcurrentQueue<string> queuePhotos;
        private static TreeNode baseNode;
        private static bool complete;
        static private int total;
        static private int processed;
        static private Mutex mutex = new Mutex(false);
        private static string basePath;

        //private Dictionary<string, PhotoDetails> dictPhotos;
        public ImageList iconList { get; set; }


        public Form1()
        {
            InitializeComponent();
            iconList = new();
            _ = NpgsqlConnection.GlobalTypeMapper.UseNodaTime();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = folderBrowser.SelectedPath;
                basePath = txtPath.Text;
                // ScanFolders();
            }
        }

        private static string GetHash(FileInfo fi)
        {
#if true
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

        static private void CheckDetails()
        {
            while (!complete)
            {
                Parallel.For(0, queuePhotos.Count, new ParallelOptions() { MaxDegreeOfParallelism = 20 }, (i) =>
                {
                    string photo;
                    queuePhotos.TryDequeue(out photo);

                    DbDetails details = new DbDetails();
                    details.fullPath = photo;
                    details.size = 0;
                    string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                        Host,
                        User,
                        DBname,
                        Port,
                        Password);

                    using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                    {

                        conn.Open();
                        string existsQuery = "SELECT count(*) FROM photos WHERE full_path=@full_path";
                        using (NpgsqlCommand existCmd = new NpgsqlCommand(existsQuery, conn))
                        {
                            existCmd.Parameters.AddWithValue("@full_path", photo);
                            int count = Convert.ToInt32(existCmd.ExecuteScalar());
                            if (count == 0)
                            {

                                FileInfo fi = new FileInfo(photo);

                                details.hash = GetHash(fi);
                                details.size = fi.Length;

                            }

                            if (details.size > 0)
                            {
                                //duplicateCmd.Parameters.AddWithValue("@hash", photo.hash);
                                int duplicate = -1;// Convert.ToInt32(duplicateCmd.ExecuteScalar());

                                string insertQuery = "INSERT INTO photos(full_path, hash, size, duplicate) VALUES(@full_path, @hash, @size, @duplicate) ON CONFLICT DO NOTHING";
                                using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@full_path", details.fullPath);
                                    insertCmd.Parameters.AddWithValue("@hash", details.hash);
                                    insertCmd.Parameters.AddWithValue("@size", details.size);
                                    insertCmd.Parameters.AddWithValue("@duplicate", duplicate);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                            mutex.WaitOne();
                            processed++;
                            mutex.ReleaseMutex();
                        }
                    }
                });
            }
        }

        private void ScanFolders()
        {
            char[] trimChars = new char[] { '\\' };

            listPhotos = new List<string> { };

            //dictPhotos = new Dictionary<string, PhotoDetails> { };

            Recurse(txtPath.Text);

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

                TreeNode newNode = new TreeNode($"{fi.Length}{fi.FullName.GetHashCode()}");
                newNode.Text = details.filename;
                //newNode.ImageIndex = imageIndex;

                nodes[0].Nodes.Add(newNode);

                lastPath = details.path;
            }

            treeFiles.EndUpdate();
        }

        private static void Recurse(string path)
        {

            try
            {
                EnumerationOptions enumerationOptions = new EnumerationOptions();
                enumerationOptions.RecurseSubdirectories = true;

                foreach (var f in Directory.GetFiles(path, "*.jpg", enumerationOptions))
                {
                    total++;
                    queuePhotos.Enqueue(f);
                }

                foreach (var f in Directory.GetFiles(path, "*.jpeg", enumerationOptions))
                {
                    total++;
                    queuePhotos.Enqueue(f);
                }

                foreach (var f in Directory.GetFiles(path, "*.png", enumerationOptions))
                {
                    total++;
                    queuePhotos.Enqueue(f);
                }
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

                    //listView1.Items.Add(item);
                }
            }
            // If the file is not found, handle the exception and inform the user.
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("File not found.");
            }
        }

        private static void ParseSubTree(TreeNode startNode)
        {

            if (startNode.Text.Contains(".j") || startNode.Text.Contains(".p"))
            {

                listPhotos.Add(startNode.FullPath);
                total++;
                queuePhotos.Enqueue(startNode.FullPath);
            }
            else
            {
                foreach (TreeNode node in startNode.Nodes)
                {
                    ParseSubTree(node);
                }
            }
        }
        private static void ParseFolders(string path)
        {


        }

        private static string Host = "172.16.0.2";
        private static string User = "bryce";
        private static string DBname = "photomgr";
        private static string Password = "pass1234";
        private static string Port = "5432";

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
                Host,
                User,
                DBname,
                Port,
                Password);


            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {

                conn.Open();

                // perform database operations

                // create database and table
                string createTableQuery = "CREATE TABLE IF NOT EXISTS photos (id SERIAL PRIMARY KEY, full_path TEXT, hash TEXT, size INT, duplicate INT, UNIQUE(full_path))";

                using (var command = new NpgsqlCommand(createTableQuery, conn))
                {
                    command.ExecuteNonQuery();
                }

                // close connection
            }
            string duplicateQuery = "SELECT count(*) FROM photos WHERE hash=@hash";


            listPhotos = new() { };
            queuePhotos = new() { };

            Thread t1 = new Thread(new ThreadStart(ParseThread));
            Thread t2 = new Thread(new ThreadStart(DbThread));

            complete = false;
            total = 0;
            processed = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = 0;

            t1.Start();
            t2.Start();

            while (!complete)
            {
                Thread.Sleep(100);

                if (total > 0)
                {
                    progressBar1.Maximum = total;
                }

                progressBar1.Value = processed;

                complete = (total > 0) && (total == processed);
            }

            t1.Join();
            t2.Join();

            progressBar1.Value = 0;
        }

        private void treeFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            baseNode = e.Node;
        }

        static void ParseThread()
        {
            //ParseSubTree(baseNode);
            Recurse(basePath);
        }
        static void DbThread()
        {
            CheckDetails();
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
    Host,
    User,
    DBname,
    Port,
    Password);


            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {

                conn.Open();

                // perform database operations

                // create database and table
                string resetQuery = "UPDATE photos SET duplicate = 0";

                using (var command = new NpgsqlCommand(resetQuery, conn))
                {
                    command.ExecuteNonQuery();
                }

                List<string> hashList = new List<string>();

                string duplicateQuery = "SELECT hash, size, COUNT(*) FROM photos GROUP BY hash, size HAVING COUNT(*) > 1";
                using (var command = new NpgsqlCommand(duplicateQuery, conn))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            hashList.Add(reader.GetString(0));
                        }
                    }
                }

                string update1Query = "UPDATE photos SET duplicate = 2 WHERE hash = @hash";
                string update2Query = "UPDATE photos SET duplicate = 1 WHERE hash = @hash AND id IN (SELECT id FROM photos WHERE hash = @hash LIMIT 1)";
                foreach (var hash in hashList)
                {
                    using (var command = new NpgsqlCommand(update1Query, conn))
                    {
                        command.Parameters.AddWithValue("@hash", hash);
                        command.ExecuteNonQuery();
                    }

                    using (var command = new NpgsqlCommand(update2Query, conn))
                    {
                        command.Parameters.AddWithValue("@hash", hash);
                        command.ExecuteNonQuery();
                    }
                }

                // close connection
            }
        }

        private void btnPrune_Click(object sender, EventArgs e)
        {
            string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
Host,
User,
DBname,
Port,
Password);


            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {

                conn.Open();

                // perform database operations

                string update1Query = "DELETE FROM photos WHERE duplicate = 2 ";
                using (var command = new NpgsqlCommand(update1Query, conn))
                {
                    command.ExecuteNonQuery();
                }

                // close connection
            }
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            int minSize = 100 * 1024;

            string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
Host,
User,
DBname,
Port,
Password);
            char[] trimChars = new char[] { '/' };
            treeFiles.Nodes.Clear();

            treeFiles.BeginUpdate();
            treeFiles.Nodes.Add("/", "/");

            string lastPath = "";
            TreeNode[] nodes = null;

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {

                conn.Open();

                // perform database operations

                string hashQuery = "SELECT hash, size, taken FROM details WHERE taken IS NOT NULL";
                using (var command = new NpgsqlCommand(hashQuery, conn))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            progressBar1.Value = 0;
                            //progressBar1.Maximum = hashList.Count;

                            //foreach (var hash in hashList)
                            {
                                progressBar1.Value++;

                                DateTime taken = new DateTime(reader.GetInt64(2)); 

                                string outPath = taken.ToString("/yyyy/MM/dd/");
                                string outFile = taken.ToString("yyyy_MM_dd_HHmmss") + "_001.jpg";

                                if (outPath != lastPath)
                                {
                                    nodes = treeFiles.Nodes.Find("/" + outPath.TrimEnd(trimChars), true);
                                    if (nodes.Length <= 0)
                                    {
                                        string tempPathL = "/";
                                        string tempPathR = outPath.Substring(tempPathL.Length).Trim(trimChars) + "/";

                                        do
                                        {
                                            string tempPath = tempPathR.Substring(0, tempPathR.IndexOf("/"));

                                            TreeNode[] pathNodes = treeFiles.Nodes.Find(tempPathL + "/" + tempPath, true);
                                            if (pathNodes.Length <= 0)
                                            {
                                                pathNodes = treeFiles.Nodes.Find(tempPathL, true);
                                                if (pathNodes.Length > 0)
                                                {
                                                    tempPathR = tempPathR.Substring(tempPathR.IndexOf("/")).TrimStart(trimChars);
                                                    tempPathL = tempPathL + "/" + tempPath;
                                                    pathNodes[0].Nodes.Add(tempPathL, tempPath);
                                                }
                                            }
                                            else
                                            {
                                                tempPathR = tempPathR.Substring(tempPathR.IndexOf("/")).TrimStart(trimChars);
                                                tempPathL = tempPathL + "/" + tempPath;
                                            }

                                            nodes = treeFiles.Nodes.Find("/" + outPath.TrimEnd(trimChars), true);
                                        }
                                        while (nodes.Length <= 0);
                                    }
                                }

                                var fullPath = $"{outPath}/{outFile}";

                                TreeNode newNode = new TreeNode($"{reader.GetString(0)}");
                                newNode.Text = outFile;
                                //newNode.ImageIndex = imageIndex;

                                nodes[0].Nodes.Add(newNode);

                                lastPath = outPath;
                            }
                        }
                    }
                }
                // close connection
            }

            treeFiles.Sort();
            treeFiles.EndUpdate();
        }

        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        private void btnTaken_Click(object sender, EventArgs e)
        {
            string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer",
Host,
User,
DBname,
Port,
Password);

            List<string> hashList = new List<string>();

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {

                conn.Open();

                // perform database operations

                string hashQuery = "SELECT hash FROM details WHERE taken IS NULL";
                using (var command = new NpgsqlCommand(hashQuery, conn))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            hashList.Add(reader.GetString(0));
                        }
                    }
                    // close connection
                }

                progressBar1.Value = 0;
                progressBar1.Maximum = hashList.Count;

                foreach (var hash in hashList)
                {
                    progressBar1.Value++;

                    string path = "";
                    string pathQuery = "SELECT full_path FROM photos WHERE hash = @hash LIMIT 1";
                    using (var command = new NpgsqlCommand(pathQuery, conn))
                    {
                        command.Parameters.AddWithValue("@hash", hash);

                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                path = reader.GetString(0);
                            }
                        }
                        // close connection
                    }

                    DateTime dateTaken;
                    try
                    {
                        dateTaken = GetDateTakenFromImage(path);
                    }
                    catch
                    {
                        continue;
                    }

                    string updateQuery = "UPDATE details SET taken = @taken WHERE hash = @hash";
                    using (var command = new NpgsqlCommand(updateQuery, conn))
                    {
                        command.Parameters.AddWithValue("@taken", dateTaken.ToBinary());
                        command.Parameters.AddWithValue("@hash", hash);
                        command.ExecuteNonQuery();
                    }
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
    public class DbDetails
    {
        public string fullPath;
        public string hash;
        public long size;
    }
}
