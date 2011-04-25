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
            this.control31 = new AControls.Control1();
            this.SuspendLayout();
            // 
            // control31
            // 
            this.control31.BackColor = System.Drawing.Color.Transparent;
            this.control31.Location = new System.Drawing.Point(44, 12);
            this.control31.Name = "control31";
            this.control31.Size = new System.Drawing.Size(147, 147);
            this.control31.State = 0;
            this.control31.TabIndex = 0;
            this.control31.Load += new System.EventHandler(this.control31_Load);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.control31);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private AControls.Control1 control31;

    }
}

