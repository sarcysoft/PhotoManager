
namespace PhotoManager
{
    partial class CompositeMaker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureSource = new System.Windows.Forms.PictureBox();
            this.pictureDest = new System.Windows.Forms.PictureBox();
            this.pictureTarget = new System.Windows.Forms.PictureBox();
            this.pictureBestFit = new System.Windows.Forms.PictureBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numScale = new System.Windows.Forms.NumericUpDown();
            this.labelSize = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.labelZoom = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblCover = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.labelMult = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelOutSize = new System.Windows.Forms.Label();
            this.pictureBestColour = new System.Windows.Forms.PictureBox();
            this.trackThreshold = new System.Windows.Forms.TrackBar();
            this.numericSearchSize = new System.Windows.Forms.NumericUpDown();
            this.numericMaxSearch = new System.Windows.Forms.NumericUpDown();
            this.checkJpg = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBestFit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBestColour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSearchSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureSource
            // 
            this.pictureSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureSource.Location = new System.Drawing.Point(13, 13);
            this.pictureSource.Name = "pictureSource";
            this.pictureSource.Size = new System.Drawing.Size(400, 400);
            this.pictureSource.TabIndex = 0;
            this.pictureSource.TabStop = false;
            this.pictureSource.Click += new System.EventHandler(this.pictureSource_Click);
            // 
            // pictureDest
            // 
            this.pictureDest.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureDest.Location = new System.Drawing.Point(489, 13);
            this.pictureDest.Name = "pictureDest";
            this.pictureDest.Size = new System.Drawing.Size(400, 400);
            this.pictureDest.TabIndex = 1;
            this.pictureDest.TabStop = false;
            this.pictureDest.DoubleClick += new System.EventHandler(this.pictureDest_DoubleClick);
            // 
            // pictureTarget
            // 
            this.pictureTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureTarget.Location = new System.Drawing.Point(419, 92);
            this.pictureTarget.Name = "pictureTarget";
            this.pictureTarget.Size = new System.Drawing.Size(64, 64);
            this.pictureTarget.TabIndex = 2;
            this.pictureTarget.TabStop = false;
            // 
            // pictureBestFit
            // 
            this.pictureBestFit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBestFit.Location = new System.Drawing.Point(419, 264);
            this.pictureBestFit.Name = "pictureBestFit";
            this.pictureBestFit.Size = new System.Drawing.Size(64, 64);
            this.pictureBestFit.TabIndex = 3;
            this.pictureBestFit.TabStop = false;
            // 
            // btnCreate
            // 
            this.btnCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCreate.Location = new System.Drawing.Point(413, 445);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 431);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "1/";
            // 
            // numScale
            // 
            this.numScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numScale.Location = new System.Drawing.Point(28, 429);
            this.numScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numScale.Name = "numScale";
            this.numScale.Size = new System.Drawing.Size(42, 23);
            this.numScale.TabIndex = 6;
            this.numScale.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numScale.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // labelSize
            // 
            this.labelSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSize.AutoSize = true;
            this.labelSize.Location = new System.Drawing.Point(86, 431);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(69, 15);
            this.labelSize.TabIndex = 7;
            this.labelSize.Text = "[### x ###]";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.trackBar1.Location = new System.Drawing.Point(489, 422);
            this.trackBar1.Maximum = 16;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(297, 45);
            this.trackBar1.TabIndex = 8;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // labelZoom
            // 
            this.labelZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(781, 423);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(19, 15);
            this.labelZoom.TabIndex = 9;
            this.labelZoom.Text = "x1";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(2, 536);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(900, 23);
            this.progressBar1.TabIndex = 11;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdate.Location = new System.Drawing.Point(814, 445);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 12;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblCover
            // 
            this.lblCover.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCover.AutoSize = true;
            this.lblCover.Location = new System.Drawing.Point(13, 482);
            this.lblCover.Name = "lblCover";
            this.lblCover.Size = new System.Drawing.Size(38, 15);
            this.lblCover.TabIndex = 13;
            this.lblCover.Text = "label2";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(814, 474);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // trackBar3
            // 
            this.trackBar3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.trackBar3.Location = new System.Drawing.Point(489, 470);
            this.trackBar3.Maximum = 2;
            this.trackBar3.Minimum = 1;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(297, 45);
            this.trackBar3.TabIndex = 16;
            this.trackBar3.Value = 1;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // labelMult
            // 
            this.labelMult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMult.AutoSize = true;
            this.labelMult.Location = new System.Drawing.Point(781, 471);
            this.labelMult.Name = "labelMult";
            this.labelMult.Size = new System.Drawing.Size(19, 15);
            this.labelMult.TabIndex = 17;
            this.labelMult.Text = "x1";
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(13, 513);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(38, 15);
            this.labelStatus.TabIndex = 19;
            this.labelStatus.Text = "label2";
            // 
            // labelOutSize
            // 
            this.labelOutSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelOutSize.AutoSize = true;
            this.labelOutSize.Location = new System.Drawing.Point(604, 500);
            this.labelOutSize.Name = "labelOutSize";
            this.labelOutSize.Size = new System.Drawing.Size(69, 15);
            this.labelOutSize.TabIndex = 20;
            this.labelOutSize.Text = "[### x ###]";
            // 
            // pictureBestColour
            // 
            this.pictureBestColour.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBestColour.Location = new System.Drawing.Point(419, 334);
            this.pictureBestColour.Name = "pictureBestColour";
            this.pictureBestColour.Size = new System.Drawing.Size(64, 64);
            this.pictureBestColour.TabIndex = 21;
            this.pictureBestColour.TabStop = false;
            // 
            // trackThreshold
            // 
            this.trackThreshold.Location = new System.Drawing.Point(180, 431);
            this.trackThreshold.Maximum = 25;
            this.trackThreshold.Minimum = 1;
            this.trackThreshold.Name = "trackThreshold";
            this.trackThreshold.Size = new System.Drawing.Size(222, 45);
            this.trackThreshold.TabIndex = 22;
            this.trackThreshold.Value = 12;
            this.trackThreshold.Scroll += new System.EventHandler(this.trackThreshold_Scroll);
            // 
            // numericSearchSize
            // 
            this.numericSearchSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericSearchSize.Location = new System.Drawing.Point(28, 456);
            this.numericSearchSize.Name = "numericSearchSize";
            this.numericSearchSize.Size = new System.Drawing.Size(42, 23);
            this.numericSearchSize.TabIndex = 23;
            this.numericSearchSize.ValueChanged += new System.EventHandler(this.numericSearchSize_ValueChanged);
            // 
            // numericMaxSearch
            // 
            this.numericMaxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericMaxSearch.Location = new System.Drawing.Point(86, 456);
            this.numericMaxSearch.Name = "numericMaxSearch";
            this.numericMaxSearch.Size = new System.Drawing.Size(42, 23);
            this.numericMaxSearch.TabIndex = 24;
            this.numericMaxSearch.ValueChanged += new System.EventHandler(this.numericMaxSearch_ValueChanged);
            // 
            // checkJpg
            // 
            this.checkJpg.AutoSize = true;
            this.checkJpg.Location = new System.Drawing.Point(489, 508);
            this.checkJpg.Name = "checkJpg";
            this.checkJpg.Size = new System.Drawing.Size(81, 19);
            this.checkJpg.TabIndex = 25;
            this.checkJpg.Text = "JPEG Limit";
            this.checkJpg.UseVisualStyleBackColor = true;
            this.checkJpg.CheckedChanged += new System.EventHandler(this.checkJpg_CheckedChanged);
            // 
            // CompositeMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 561);
            this.Controls.Add(this.checkJpg);
            this.Controls.Add(this.numericMaxSearch);
            this.Controls.Add(this.numericSearchSize);
            this.Controls.Add(this.trackThreshold);
            this.Controls.Add(this.pictureBestColour);
            this.Controls.Add(this.labelOutSize);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelMult);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblCover);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.labelZoom);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.labelSize);
            this.Controls.Add(this.numScale);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.pictureBestFit);
            this.Controls.Add(this.pictureTarget);
            this.Controls.Add(this.pictureDest);
            this.Controls.Add(this.pictureSource);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(920, 600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(920, 600);
            this.Name = "CompositeMaker";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "CompositeMaker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CompositeMaker_FormClosing);
            this.Load += new System.EventHandler(this.CompositeMaker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBestFit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBestColour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericSearchSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaxSearch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureSource;
        private System.Windows.Forms.PictureBox pictureDest;
        private System.Windows.Forms.PictureBox pictureTarget;
        private System.Windows.Forms.PictureBox pictureBestFit;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numScale;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label labelZoom;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblCover;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.Label labelMult;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelOutSize;
        private System.Windows.Forms.PictureBox pictureBestColour;
        private System.Windows.Forms.TrackBar trackThreshold;
        private System.Windows.Forms.NumericUpDown numericSearchSize;
        private System.Windows.Forms.NumericUpDown numericMaxSearch;
        private System.Windows.Forms.CheckBox checkJpg;
    }
}