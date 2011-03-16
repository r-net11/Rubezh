namespace RubezhAX
{
    partial class UCRubezh
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
            this.DeviceNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DeviceNameLabel
            // 
            this.DeviceNameLabel.AutoSize = true;
            this.DeviceNameLabel.Location = new System.Drawing.Point(15, 13);
            this.DeviceNameLabel.Name = "DeviceNameLabel";
            this.DeviceNameLabel.Size = new System.Drawing.Size(0, 13);
            this.DeviceNameLabel.TabIndex = 0;
            // 
            // UCRubezh
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lime;
            this.Controls.Add(this.DeviceNameLabel);
            this.Name = "UCRubezh";
            this.Load += new System.EventHandler(this.UCRubezh_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label DeviceNameLabel;

    }
}
