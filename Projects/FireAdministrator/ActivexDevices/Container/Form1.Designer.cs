﻿namespace Container
{
    partial class Form1
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
            this.activeX = new ActiveX.CActiveX();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // activeX
            // 
            this.activeX.BackColor = System.Drawing.SystemColors.HotTrack;
            this.activeX.DriverId = null;
            this.activeX.Location = new System.Drawing.Point(39, 21);
            this.activeX.Name = "activeX";
            this.activeX.Size = new System.Drawing.Size(225, 191);
            this.activeX.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(102, 233);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 26);
            this.button1.TabIndex = 1;
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 266);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.activeX);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.Integration.ElementHost activeX;
        private ActiveX.CActiveX activeX;
        private System.Windows.Forms.Button button1;



    }
}

