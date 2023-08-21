
namespace PhotoManager
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtPath = new System.Windows.Forms.TextBox();
            btnBrowse = new System.Windows.Forms.Button();
            treeFiles = new System.Windows.Forms.TreeView();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            treeOut = new System.Windows.Forms.TreeView();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            buttonAdd = new System.Windows.Forms.Button();
            buttonOut = new System.Windows.Forms.Button();
            textOutPath = new System.Windows.Forms.TextBox();
            btnDuplicate = new System.Windows.Forms.Button();
            btnPrune = new System.Windows.Forms.Button();
            btnBuild = new System.Windows.Forms.Button();
            btnTaken = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // txtPath
            // 
            txtPath.Location = new System.Drawing.Point(15, 17);
            txtPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            txtPath.Name = "txtPath";
            txtPath.Size = new System.Drawing.Size(425, 27);
            txtPath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(446, 15);
            btnBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(86, 31);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Browse";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // treeFiles
            // 
            treeFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            treeFiles.Location = new System.Drawing.Point(3, 4);
            treeFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            treeFiles.Name = "treeFiles";
            treeFiles.Size = new System.Drawing.Size(519, 611);
            treeFiles.TabIndex = 5;
            treeFiles.AfterSelect += treeFiles_AfterSelect;
            treeFiles.NodeMouseDoubleClick += treeFiles_NodeMouseDoubleClick;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(treeOut, 0, 0);
            tableLayoutPanel1.Controls.Add(treeFiles, 0, 0);
            tableLayoutPanel1.Location = new System.Drawing.Point(15, 157);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 635F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1050, 619);
            tableLayoutPanel1.TabIndex = 7;
            // 
            // treeOut
            // 
            treeOut.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            treeOut.Location = new System.Drawing.Point(528, 4);
            treeOut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            treeOut.Name = "treeOut";
            treeOut.Size = new System.Drawing.Size(519, 611);
            treeOut.TabIndex = 6;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBar1.Location = new System.Drawing.Point(0, 785);
            progressBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(1080, 31);
            progressBar1.TabIndex = 8;
            // 
            // buttonAdd
            // 
            buttonAdd.Location = new System.Drawing.Point(18, 118);
            buttonAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new System.Drawing.Size(86, 31);
            buttonAdd.TabIndex = 9;
            buttonAdd.Text = "Add";
            buttonAdd.UseVisualStyleBackColor = true;
            buttonAdd.Click += buttonAdd_Click;
            // 
            // buttonOut
            // 
            buttonOut.Location = new System.Drawing.Point(974, 17);
            buttonOut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonOut.Name = "buttonOut";
            buttonOut.Size = new System.Drawing.Size(86, 31);
            buttonOut.TabIndex = 11;
            buttonOut.Text = "Browse";
            buttonOut.UseVisualStyleBackColor = true;
            // 
            // textOutPath
            // 
            textOutPath.Location = new System.Drawing.Point(543, 19);
            textOutPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            textOutPath.Name = "textOutPath";
            textOutPath.Size = new System.Drawing.Size(425, 27);
            textOutPath.TabIndex = 10;
            // 
            // btnDuplicate
            // 
            btnDuplicate.Location = new System.Drawing.Point(142, 119);
            btnDuplicate.Name = "btnDuplicate";
            btnDuplicate.Size = new System.Drawing.Size(94, 29);
            btnDuplicate.TabIndex = 12;
            btnDuplicate.Text = "Mark";
            btnDuplicate.UseVisualStyleBackColor = true;
            btnDuplicate.Click += btnDuplicate_Click;
            // 
            // btnPrune
            // 
            btnPrune.Location = new System.Drawing.Point(261, 119);
            btnPrune.Name = "btnPrune";
            btnPrune.Size = new System.Drawing.Size(94, 29);
            btnPrune.TabIndex = 13;
            btnPrune.Text = "Prune";
            btnPrune.UseVisualStyleBackColor = true;
            btnPrune.Click += btnPrune_Click;
            // 
            // btnBuild
            // 
            btnBuild.Location = new System.Drawing.Point(494, 119);
            btnBuild.Name = "btnBuild";
            btnBuild.Size = new System.Drawing.Size(94, 29);
            btnBuild.TabIndex = 14;
            btnBuild.Text = "Build";
            btnBuild.UseVisualStyleBackColor = true;
            btnBuild.Click += btnBuild_Click;
            // 
            // btnTaken
            // 
            btnTaken.Location = new System.Drawing.Point(373, 119);
            btnTaken.Name = "btnTaken";
            btnTaken.Size = new System.Drawing.Size(94, 29);
            btnTaken.TabIndex = 15;
            btnTaken.Text = "Taken";
            btnTaken.UseVisualStyleBackColor = true;
            btnTaken.Click += btnTaken_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1080, 819);
            Controls.Add(btnTaken);
            Controls.Add(btnBuild);
            Controls.Add(btnPrune);
            Controls.Add(btnDuplicate);
            Controls.Add(buttonOut);
            Controls.Add(textOutPath);
            Controls.Add(buttonAdd);
            Controls.Add(progressBar1);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(btnBrowse);
            Controls.Add(txtPath);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TreeView treeFiles;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonOut;
        private System.Windows.Forms.TextBox textOutPath;
        private System.Windows.Forms.Button btnDuplicate;
        private System.Windows.Forms.Button btnPrune;
        private System.Windows.Forms.TreeView treeOut;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.Button btnTaken;
    }
}

