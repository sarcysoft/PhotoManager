﻿
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
            this.pictureSource = new System.Windows.Forms.PictureBox();
            this.pictureDest = new System.Windows.Forms.PictureBox();
            this.pictureTarget = new System.Windows.Forms.PictureBox();
            this.pictureBestFit = new System.Windows.Forms.PictureBox();
            this.numSegmentSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBestFit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSegmentSize)).BeginInit();
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
            this.pictureBestFit.Click += new System.EventHandler(this.pictureBestFit_Click);
            // 
            // numSegmentSize
            // 
            this.numSegmentSize.Location = new System.Drawing.Point(95, 333);
            this.numSegmentSize.Maximum = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.numSegmentSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSegmentSize.Name = "numSegmentSize";
            this.numSegmentSize.Size = new System.Drawing.Size(61, 23);
            this.numSegmentSize.TabIndex = 4;
            this.numSegmentSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numSegmentSize.ValueChanged += new System.EventHandler(this.numSegmentSize_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 335);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Segment Size";
            // 
            // CompositeMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 425);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numSegmentSize);
            this.Controls.Add(this.pictureBestFit);
            this.Controls.Add(this.pictureTarget);
            this.Controls.Add(this.pictureDest);
            this.Controls.Add(this.pictureSource);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(920, 464);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(920, 464);
            this.Name = "CompositeMaker";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "CompositeMaker";
            this.Load += new System.EventHandler(this.CompositeMaker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBestFit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSegmentSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureSource;
        private System.Windows.Forms.PictureBox pictureDest;
        private System.Windows.Forms.PictureBox pictureTarget;
        private System.Windows.Forms.PictureBox pictureBestFit;
        private System.Windows.Forms.NumericUpDown numSegmentSize;
        private System.Windows.Forms.Label label1;
    }
}