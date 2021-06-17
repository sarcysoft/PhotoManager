
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
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.labelMult = new System.Windows.Forms.Label();
            this.labelScale = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBestFit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureSource
            // 
            this.pictureSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureSource.Location = new System.Drawing.Point(13, 13);
            this.pictureSource.Name = "pictureSource";
            this.pictureSource.Size = new System.Drawing.Size(400, 300);
            this.pictureSource.TabIndex = 0;
            this.pictureSource.TabStop = false;
            this.pictureSource.Click += new System.EventHandler(this.pictureSource_Click);
            // 
            // pictureDest
            // 
            this.pictureDest.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureDest.Location = new System.Drawing.Point(489, 13);
            this.pictureDest.Name = "pictureDest";
            this.pictureDest.Size = new System.Drawing.Size(400, 300);
            this.pictureDest.TabIndex = 1;
            this.pictureDest.TabStop = false;
            this.pictureDest.DoubleClick += new System.EventHandler(this.pictureDest_DoubleClick);
            // 
            // pictureTarget
            // 
            this.pictureTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureTarget.Location = new System.Drawing.Point(419, 78);
            this.pictureTarget.Name = "pictureTarget";
            this.pictureTarget.Size = new System.Drawing.Size(64, 64);
            this.pictureTarget.TabIndex = 2;
            this.pictureTarget.TabStop = false;
            // 
            // pictureBestFit
            // 
            this.pictureBestFit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBestFit.Location = new System.Drawing.Point(419, 187);
            this.pictureBestFit.Name = "pictureBestFit";
            this.pictureBestFit.Size = new System.Drawing.Size(64, 64);
            this.pictureBestFit.TabIndex = 3;
            this.pictureBestFit.TabStop = false;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(211, 334);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 336);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "1/";
            // 
            // numScale
            // 
            this.numScale.Location = new System.Drawing.Point(28, 334);
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
            this.labelSize.AutoSize = true;
            this.labelSize.Location = new System.Drawing.Point(86, 336);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(69, 15);
            this.labelSize.TabIndex = 7;
            this.labelSize.Text = "[### x ###]";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(489, 334);
            this.trackBar1.Maximum = 16;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(297, 45);
            this.trackBar1.TabIndex = 8;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // labelZoom
            // 
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(792, 338);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(19, 15);
            this.labelZoom.TabIndex = 9;
            this.labelZoom.Text = "x1";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(2, 436);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(900, 23);
            this.progressBar1.TabIndex = 11;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(814, 338);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 12;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblCover
            // 
            this.lblCover.AutoSize = true;
            this.lblCover.Location = new System.Drawing.Point(13, 375);
            this.lblCover.Name = "lblCover";
            this.lblCover.Size = new System.Drawing.Size(38, 15);
            this.lblCover.TabIndex = 13;
            this.lblCover.Text = "label2";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(814, 367);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(489, 366);
            this.trackBar2.Maximum = 4;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(297, 45);
            this.trackBar2.TabIndex = 15;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(489, 395);
            this.trackBar3.Maximum = 8;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(297, 45);
            this.trackBar3.TabIndex = 16;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // labelMult
            // 
            this.labelMult.AutoSize = true;
            this.labelMult.Location = new System.Drawing.Point(792, 395);
            this.labelMult.Name = "labelMult";
            this.labelMult.Size = new System.Drawing.Size(19, 15);
            this.labelMult.TabIndex = 17;
            this.labelMult.Text = "x1";
            // 
            // labelScale
            // 
            this.labelScale.AutoSize = true;
            this.labelScale.Location = new System.Drawing.Point(792, 371);
            this.labelScale.Name = "labelScale";
            this.labelScale.Size = new System.Drawing.Size(19, 15);
            this.labelScale.TabIndex = 18;
            this.labelScale.Text = "x1";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(13, 406);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(38, 15);
            this.labelStatus.TabIndex = 19;
            this.labelStatus.Text = "label2";
            // 
            // CompositeMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 461);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelScale);
            this.Controls.Add(this.labelMult);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.trackBar2);
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
            this.MaximumSize = new System.Drawing.Size(920, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(920, 500);
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
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
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
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.Label labelMult;
        private System.Windows.Forms.Label labelScale;
        private System.Windows.Forms.Label labelStatus;
    }
}