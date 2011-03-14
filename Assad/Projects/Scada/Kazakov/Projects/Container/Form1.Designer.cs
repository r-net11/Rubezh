namespace Container
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
            this.control11 = new AControls.Control1();
            this.control31 = new AControls.Control3();
            this.control41 = new AControls.Control4();
            this.control21 = new AControls.Control2();
            this.SuspendLayout();
            // 
            // control11
            // 
            this.control11.Location = new System.Drawing.Point(56, 12);
            this.control11.Name = "control11";
            this.control11.Size = new System.Drawing.Size(265, 275);
            this.control11.State = 0;
            this.control11.TabIndex = 5;
            // 
            // control31
            // 
            this.control31.BackColor = System.Drawing.Color.Transparent;
            this.control31.Location = new System.Drawing.Point(0, 161);
            this.control31.Name = "control31";
            this.control31.Size = new System.Drawing.Size(83, 79);
            this.control31.State = 0;
            this.control31.TabIndex = 4;
            // 
            // control41
            // 
            this.control41.BackColor = System.Drawing.Color.Transparent;
            this.control41.Location = new System.Drawing.Point(98, 161);
            this.control41.Name = "control41";
            this.control41.Size = new System.Drawing.Size(80, 79);
            this.control41.State = 3;
            this.control41.TabIndex = 3;
            // 
            // control21
            // 
            this.control21.BackColor = System.Drawing.Color.Transparent;
            this.control21.Location = new System.Drawing.Point(208, 161);
            this.control21.Name = "control21";
            this.control21.Size = new System.Drawing.Size(83, 79);
            this.control21.State = 3;
            this.control21.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 266);
            this.Controls.Add(this.control11);
            this.Controls.Add(this.control31);
            this.Controls.Add(this.control41);
            this.Controls.Add(this.control21);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private AControls.Control2 control21;
        private AControls.Control4 control41;
        private AControls.Control3 control31;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private HostingWpfUserControlInWf.UserControl1 userControl11;
        private AControls.Control1 control11;



    }
}

