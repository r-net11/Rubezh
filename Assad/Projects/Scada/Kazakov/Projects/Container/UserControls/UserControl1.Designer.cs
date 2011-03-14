namespace AControls
{
    partial class Control1
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

                this.components = new System.ComponentModel.Container();
                this.panel = new System.Windows.Forms.Panel();
                this.timer1 = new System.Windows.Forms.Timer(this.components);
                this.SuspendLayout();
                // 
                // panel
                // 
                this.panel.Location = new System.Drawing.Point(0, 0);
                this.panel.Name = "panel";
                this.panel.Size = new System.Drawing.Size(200, 100);
                this.panel.TabIndex = 0;
                this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
                // 
                // timer1
                // 
                this.timer1.Interval = 200;
                this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
                // 
                // Control1
                // 
                this.Controls.Add(this.panel);
                this.Name = "Control1";
                this.Size = new System.Drawing.Size(400, 258);
                this.Load += new System.EventHandler(this.Control1_Load);
                this.ResumeLayout(false);
        }

        #endregion

        private HostingWpfUserControlInWf.UserControl1 userControl11;



    }
}
