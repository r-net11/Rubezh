namespace FiresecService.LicenseEditor
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
			this.tableLayoutPanelCenter = new System.Windows.Forms.TableLayoutPanel();
			this.labelOpcServer = new System.Windows.Forms.Label();
			this.labelVideo = new System.Windows.Forms.Label();
			this.labelAccess = new System.Windows.Forms.Label();
			this.labelFire = new System.Windows.Forms.Label();
			this.checkBoxFire = new System.Windows.Forms.CheckBox();
			this.checkBoxSecurity = new System.Windows.Forms.CheckBox();
			this.checkBoxAccess = new System.Windows.Forms.CheckBox();
			this.checkBoxVideo = new System.Windows.Forms.CheckBox();
			this.checkBoxOpcServer = new System.Windows.Forms.CheckBox();
			this.labelRemoteWorkplacesCount = new System.Windows.Forms.Label();
			this.labelSecurity = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.numericUpDownRemoteWorkplacesCount = new System.Windows.Forms.NumericUpDown();
			this.panelTop = new System.Windows.Forms.Panel();
			this.textBoxKey = new System.Windows.Forms.TextBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.tableLayoutPanelCenter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRemoteWorkplacesCount)).BeginInit();
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
			// tableLayoutPanelCenter
			// 
			this.tableLayoutPanelCenter.AutoSize = true;
			this.tableLayoutPanelCenter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelCenter.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
			this.tableLayoutPanelCenter.ColumnCount = 2;
			this.tableLayoutPanelCenter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73F));
			this.tableLayoutPanelCenter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27F));
			this.tableLayoutPanelCenter.Controls.Add(this.labelOpcServer, 0, 6);
			this.tableLayoutPanelCenter.Controls.Add(this.labelVideo, 0, 5);
			this.tableLayoutPanelCenter.Controls.Add(this.labelAccess, 0, 4);
			this.tableLayoutPanelCenter.Controls.Add(this.labelFire, 0, 2);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxFire, 1, 2);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxSecurity, 1, 3);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxAccess, 1, 4);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxVideo, 1, 5);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxOpcServer, 1, 6);
			this.tableLayoutPanelCenter.Controls.Add(this.labelRemoteWorkplacesCount, 0, 1);
			this.tableLayoutPanelCenter.Controls.Add(this.labelSecurity, 0, 3);
			this.tableLayoutPanelCenter.Controls.Add(this.labelVersion, 1, 0);
			this.tableLayoutPanelCenter.Controls.Add(this.numericUpDownRemoteWorkplacesCount, 1, 1);
			this.tableLayoutPanelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelCenter.Location = new System.Drawing.Point(0, 34);
			this.tableLayoutPanelCenter.Name = "tableLayoutPanelCenter";
			this.tableLayoutPanelCenter.RowCount = 7;
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.Size = new System.Drawing.Size(498, 162);
			this.tableLayoutPanelCenter.TabIndex = 1;
			// 
			// labelOpcServer
			// 
			this.labelOpcServer.AutoSize = true;
			this.labelOpcServer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelOpcServer.Location = new System.Drawing.Point(5, 139);
			this.labelOpcServer.Name = "labelOpcServer";
			this.labelOpcServer.Size = new System.Drawing.Size(353, 21);
			this.labelOpcServer.TabIndex = 13;
			this.labelOpcServer.Text = "GLOBAL OPC сервер:";
			this.labelOpcServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelVideo
			// 
			this.labelVideo.AutoSize = true;
			this.labelVideo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelVideo.Location = new System.Drawing.Point(5, 117);
			this.labelVideo.Name = "labelVideo";
			this.labelVideo.Size = new System.Drawing.Size(353, 20);
			this.labelVideo.TabIndex = 12;
			this.labelVideo.Text = "GLOBAL Видео:";
			this.labelVideo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelAccess
			// 
			this.labelAccess.AutoSize = true;
			this.labelAccess.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelAccess.Location = new System.Drawing.Point(5, 95);
			this.labelAccess.Name = "labelAccess";
			this.labelAccess.Size = new System.Drawing.Size(353, 20);
			this.labelAccess.TabIndex = 11;
			this.labelAccess.Text = "GLOBAL Доступ:";
			this.labelAccess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFire
			// 
			this.labelFire.AutoSize = true;
			this.labelFire.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelFire.Location = new System.Drawing.Point(5, 51);
			this.labelFire.Name = "labelFire";
			this.labelFire.Size = new System.Drawing.Size(353, 20);
			this.labelFire.TabIndex = 9;
			this.labelFire.Text = "GLOBAL Пожаротушение:";
			this.labelFire.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBoxFire
			// 
			this.checkBoxFire.AutoSize = true;
			this.checkBoxFire.Location = new System.Drawing.Point(366, 54);
			this.checkBoxFire.Name = "checkBoxFire";
			this.checkBoxFire.Size = new System.Drawing.Size(15, 14);
			this.checkBoxFire.TabIndex = 3;
			this.checkBoxFire.UseVisualStyleBackColor = true;
			// 
			// checkBoxSecurity
			// 
			this.checkBoxSecurity.AutoSize = true;
			this.checkBoxSecurity.Location = new System.Drawing.Point(366, 76);
			this.checkBoxSecurity.Name = "checkBoxSecurity";
			this.checkBoxSecurity.Size = new System.Drawing.Size(15, 14);
			this.checkBoxSecurity.TabIndex = 4;
			this.checkBoxSecurity.UseVisualStyleBackColor = true;
			// 
			// checkBoxAccess
			// 
			this.checkBoxAccess.AutoSize = true;
			this.checkBoxAccess.Location = new System.Drawing.Point(366, 98);
			this.checkBoxAccess.Name = "checkBoxAccess";
			this.checkBoxAccess.Size = new System.Drawing.Size(15, 14);
			this.checkBoxAccess.TabIndex = 5;
			this.checkBoxAccess.UseVisualStyleBackColor = true;
			// 
			// checkBoxVideo
			// 
			this.checkBoxVideo.AutoSize = true;
			this.checkBoxVideo.Location = new System.Drawing.Point(366, 120);
			this.checkBoxVideo.Name = "checkBoxVideo";
			this.checkBoxVideo.Size = new System.Drawing.Size(15, 14);
			this.checkBoxVideo.TabIndex = 6;
			this.checkBoxVideo.UseVisualStyleBackColor = true;
			// 
			// checkBoxOpcServer
			// 
			this.checkBoxOpcServer.AutoSize = true;
			this.checkBoxOpcServer.Location = new System.Drawing.Point(366, 142);
			this.checkBoxOpcServer.Name = "checkBoxOpcServer";
			this.checkBoxOpcServer.Size = new System.Drawing.Size(15, 14);
			this.checkBoxOpcServer.TabIndex = 7;
			this.checkBoxOpcServer.UseVisualStyleBackColor = true;
			// 
			// labelRemoteWorkplacesCount
			// 
			this.labelRemoteWorkplacesCount.AutoSize = true;
			this.labelRemoteWorkplacesCount.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelRemoteWorkplacesCount.Location = new System.Drawing.Point(5, 23);
			this.labelRemoteWorkplacesCount.Name = "labelRemoteWorkplacesCount";
			this.labelRemoteWorkplacesCount.Size = new System.Drawing.Size(353, 26);
			this.labelRemoteWorkplacesCount.TabIndex = 8;
			this.labelRemoteWorkplacesCount.Text = "GLOBAL Удаленное рабочее место (количество):";
			this.labelRemoteWorkplacesCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSecurity
			// 
			this.labelSecurity.AutoSize = true;
			this.labelSecurity.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelSecurity.Location = new System.Drawing.Point(5, 73);
			this.labelSecurity.Name = "labelSecurity";
			this.labelSecurity.Size = new System.Drawing.Size(353, 20);
			this.labelSecurity.TabIndex = 10;
			this.labelSecurity.Text = "GLOBAL Охрана:";
			this.labelSecurity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelVersion
			// 
			this.labelVersion.AutoSize = true;
			this.labelVersion.Location = new System.Drawing.Point(366, 5);
			this.labelVersion.Margin = new System.Windows.Forms.Padding(3);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(0, 13);
			this.labelVersion.TabIndex = 15;
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// numericUpDownRemoteWorkplacesCount
			// 
			this.numericUpDownRemoteWorkplacesCount.Location = new System.Drawing.Point(366, 26);
			this.numericUpDownRemoteWorkplacesCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownRemoteWorkplacesCount.Name = "numericUpDownRemoteWorkplacesCount";
			this.numericUpDownRemoteWorkplacesCount.Size = new System.Drawing.Size(120, 20);
			this.numericUpDownRemoteWorkplacesCount.TabIndex = 14;
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
			this.panelBottom.Location = new System.Drawing.Point(0, 196);
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
			this.ClientSize = new System.Drawing.Size(498, 226);
			this.Controls.Add(this.tableLayoutPanelCenter);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "MainForm";
			this.Text = "Редактор лицензий Firesec Service";
			this.tableLayoutPanelCenter.ResumeLayout(false);
			this.tableLayoutPanelCenter.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRemoteWorkplacesCount)).EndInit();
			this.panelTop.ResumeLayout(false);
			this.panelTop.PerformLayout();
			this.panelBottom.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelKey;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCenter;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.CheckBox checkBoxFire;
        private System.Windows.Forms.CheckBox checkBoxSecurity;
        private System.Windows.Forms.CheckBox checkBoxAccess;
        private System.Windows.Forms.CheckBox checkBoxVideo;
        private System.Windows.Forms.CheckBox checkBoxOpcServer;
        private System.Windows.Forms.Label labelOpcServer;
        private System.Windows.Forms.Label labelVideo;
        private System.Windows.Forms.Label labelAccess;
        private System.Windows.Forms.Label labelFire;
        private System.Windows.Forms.Label labelRemoteWorkplacesCount;
        private System.Windows.Forms.NumericUpDown numericUpDownRemoteWorkplacesCount;
        private System.Windows.Forms.Label labelSecurity;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Label labelVersion;
    }
}

