namespace Resurs.LicenseEditor
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
			this.labelKey = new System.Windows.Forms.Label();
			this.labelDevicesCount = new System.Windows.Forms.Label();
			this.numericUpDownDevicesCount = new System.Windows.Forms.NumericUpDown();
			this.panelTop = new System.Windows.Forms.Panel();
			this.textBoxKey = new System.Windows.Forms.TextBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDevicesCount)).BeginInit();
			this.panelTop.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelKey
			// 
			this.labelKey.AutoSize = true;
			this.labelKey.Location = new System.Drawing.Point(12, 9);
			this.labelKey.Name = "labelKey";
			this.labelKey.Size = new System.Drawing.Size(36, 13);
			this.labelKey.TabIndex = 0;
			this.labelKey.Text = "Ключ:";
			// 
			// labelDevicesCount
			// 
			this.labelDevicesCount.AutoSize = true;
			this.labelDevicesCount.Location = new System.Drawing.Point(12, 39);
			this.labelDevicesCount.Name = "labelDevicesCount";
			this.labelDevicesCount.Size = new System.Drawing.Size(123, 13);
			this.labelDevicesCount.TabIndex = 8;
			this.labelDevicesCount.Text = "Количество устройств:";
			this.labelDevicesCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownDevicesCount
			// 
			this.numericUpDownDevicesCount.Location = new System.Drawing.Point(141, 37);
			this.numericUpDownDevicesCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownDevicesCount.Name = "numericUpDownDevicesCount";
			this.numericUpDownDevicesCount.Size = new System.Drawing.Size(345, 20);
			this.numericUpDownDevicesCount.TabIndex = 14;
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.textBoxKey);
			this.panelTop.Controls.Add(this.labelKey);
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(498, 34);
			this.panelTop.TabIndex = 2;
			// 
			// textBoxKey
			// 
			this.textBoxKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxKey.Location = new System.Drawing.Point(51, 6);
			this.textBoxKey.Name = "textBoxKey";
			this.textBoxKey.Size = new System.Drawing.Size(435, 20);
			this.textBoxKey.TabIndex = 1;
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.buttonSave);
			this.panelBottom.Controls.Add(this.buttonLoad);
			this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelBottom.Location = new System.Drawing.Point(0, 60);
			this.panelBottom.Name = "panelBottom";
			this.panelBottom.Size = new System.Drawing.Size(498, 30);
			this.panelBottom.TabIndex = 3;
			// 
			// buttonSave
			// 
			this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSave.Location = new System.Drawing.Point(384, 4);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(110, 23);
			this.buttonSave.TabIndex = 2;
			this.buttonSave.Text = "Сохранить";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// buttonLoad
			// 
			this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonLoad.Location = new System.Drawing.Point(268, 4);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(110, 23);
			this.buttonLoad.TabIndex = 1;
			this.buttonLoad.Text = "Загрузить";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "license";
			this.openFileDialog.Filter = "Файл лицензии (*.license)|*.license";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "license";
			this.saveFileDialog.Filter = "Файл лицензии (*.license)|*.license";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(498, 90);
			this.Controls.Add(this.labelDevicesCount);
			this.Controls.Add(this.numericUpDownDevicesCount);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "MainForm";
			this.Text = "Редактор лицензий Firesec Service";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownDevicesCount)).EndInit();
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label labelKey;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox textBoxKey;
		private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label labelDevicesCount;
		private System.Windows.Forms.NumericUpDown numericUpDownDevicesCount;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

