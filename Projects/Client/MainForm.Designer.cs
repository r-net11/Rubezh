namespace Client
{
    partial class MainForm
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
            this.startButton = new System.Windows.Forms.Button();
            this.panelDevice = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.deviceViewerTextBox = new System.Windows.Forms.TextBox();
            this.parentAddressTextBox = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.deviceTreeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 12);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(192, 23);
            this.startButton.TabIndex = 32;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // panelDevice
            // 
            this.panelDevice.AutoScroll = true;
            this.panelDevice.BackColor = System.Drawing.SystemColors.Window;
            this.panelDevice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDevice.Location = new System.Drawing.Point(678, 28);
            this.panelDevice.Name = "panelDevice";
            this.panelDevice.Size = new System.Drawing.Size(400, 463);
            this.panelDevice.TabIndex = 38;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 70);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(192, 23);
            this.button2.TabIndex = 40;
            this.button2.Text = "Show Logs";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(734, 501);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Address";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(700, 527);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 45;
            this.label4.Text = "Parent Address";
            // 
            // deviceViewerTextBox
            // 
            this.deviceViewerTextBox.Location = new System.Drawing.Point(785, 498);
            this.deviceViewerTextBox.Name = "deviceViewerTextBox";
            this.deviceViewerTextBox.Size = new System.Drawing.Size(100, 20);
            this.deviceViewerTextBox.TabIndex = 46;
            // 
            // parentAddressTextBox
            // 
            this.parentAddressTextBox.Location = new System.Drawing.Point(785, 524);
            this.parentAddressTextBox.Name = "parentAddressTextBox";
            this.parentAddressTextBox.Size = new System.Drawing.Size(100, 20);
            this.parentAddressTextBox.TabIndex = 47;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 41);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(192, 23);
            this.button3.TabIndex = 48;
            this.button3.Text = "Show Device Viewer";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.deviceViewerButton_Click);
            // 
            // deviceTreeView
            // 
            this.deviceTreeView.Location = new System.Drawing.Point(344, 28);
            this.deviceTreeView.Name = "deviceTreeView";
            this.deviceTreeView.Size = new System.Drawing.Size(328, 463);
            this.deviceTreeView.TabIndex = 49;
            this.deviceTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.deviceTreeView_NodeMouseClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1081, 586);
            this.Controls.Add(this.deviceTreeView);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.parentAddressTextBox);
            this.Controls.Add(this.deviceViewerTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panelDevice);
            this.Controls.Add(this.startButton);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "АПИ РУБЕЖ ТЕСТ";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Panel panelDevice;
        public System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox deviceViewerTextBox;
        private System.Windows.Forms.TextBox parentAddressTextBox;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TreeView deviceTreeView;
    }
}

