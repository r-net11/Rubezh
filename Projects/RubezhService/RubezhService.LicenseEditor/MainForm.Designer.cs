namespace RubezhService.LicenseEditor
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
			this.labelSKD = new System.Windows.Forms.Label();
			this.labelFirefighting = new System.Windows.Forms.Label();
			this.checkBoxFirefighting = new System.Windows.Forms.CheckBox();
			this.checkBoxGuard = new System.Windows.Forms.CheckBox();
			this.checkBoxSKD = new System.Windows.Forms.CheckBox();
			this.checkBoxVideo = new System.Windows.Forms.CheckBox();
			this.checkBoxOpcServer = new System.Windows.Forms.CheckBox();
			this.labelRemoteWorkplacesCount = new System.Windows.Forms.Label();
			this.labelGuard = new System.Windows.Forms.Label();
			this.numericUpDownRemoteClientsCount = new System.Windows.Forms.NumericUpDown();
			this.panelTop = new System.Windows.Forms.Panel();
			this.textBoxKey = new System.Windows.Forms.TextBox();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.tableLayoutPanelCenter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRemoteClientsCount)).BeginInit();
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
			this.tableLayoutPanelCenter.Controls.Add(this.labelOpcServer, 0, 5);
			this.tableLayoutPanelCenter.Controls.Add(this.labelVideo, 0, 4);
			this.tableLayoutPanelCenter.Controls.Add(this.labelSKD, 0, 3);
			this.tableLayoutPanelCenter.Controls.Add(this.labelFirefighting, 0, 1);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxFirefighting, 1, 1);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxGuard, 1, 2);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxSKD, 1, 3);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxVideo, 1, 4);
			this.tableLayoutPanelCenter.Controls.Add(this.checkBoxOpcServer, 1, 5);
			this.tableLayoutPanelCenter.Controls.Add(this.labelRemoteWorkplacesCount, 0, 0);
			this.tableLayoutPanelCenter.Controls.Add(this.labelGuard, 0, 2);
			this.tableLayoutPanelCenter.Controls.Add(this.numericUpDownRemoteClientsCount, 1, 0);
			this.tableLayoutPanelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelCenter.Location = new System.Drawing.Point(0, 34);
			this.tableLayoutPanelCenter.Name = "tableLayoutPanelCenter";
			this.tableLayoutPanelCenter.RowCount = 6;
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCenter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanelCenter.Size = new System.Drawing.Size(498, 144);
			this.tableLayoutPanelCenter.TabIndex = 1;
			// 
			// labelOpcServer
			// 
			this.labelOpcServer.AutoSize = true;
			this.labelOpcServer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelOpcServer.Location = new System.Drawing.Point(5, 118);
			this.labelOpcServer.Name = "labelOpcServer";
			this.labelOpcServer.Size = new System.Drawing.Size(353, 24);
			this.labelOpcServer.TabIndex = 13;
			this.labelOpcServer.Text = "GLOBAL OPC сервер:";
			this.labelOpcServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelVideo
			// 
			this.labelVideo.AutoSize = true;
			this.labelVideo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelVideo.Location = new System.Drawing.Point(5, 96);
			this.labelVideo.Name = "labelVideo";
			this.labelVideo.Size = new System.Drawing.Size(353, 20);
			this.labelVideo.TabIndex = 12;
			this.labelVideo.Text = "GLOBAL Видео:";
			this.labelVideo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSKD
			// 
			this.labelSKD.AutoSize = true;
			this.labelSKD.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelSKD.Location = new System.Drawing.Point(5, 74);
			this.labelSKD.Name = "labelSKD";
			this.labelSKD.Size = new System.Drawing.Size(353, 20);
			this.labelSKD.TabIndex = 11;
			this.labelSKD.Text = "GLOBAL Доступ:";
			this.labelSKD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFirefighting
			// 
			this.labelFirefighting.AutoSize = true;
			this.labelFirefighting.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelFirefighting.Location = new System.Drawing.Point(5, 30);
			this.labelFirefighting.Name = "labelFirefighting";
			this.labelFirefighting.Size = new System.Drawing.Size(353, 20);
			this.labelFirefighting.TabIndex = 9;
			this.labelFirefighting.Text = "GLOBAL Пожаротушение:";
			this.labelFirefighting.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBoxFirefighting
			// 
			this.checkBoxFirefighting.AutoSize = true;
			this.checkBoxFirefighting.Location = new System.Drawing.Point(366, 33);
			this.checkBoxFirefighting.Name = "checkBoxFirefighting";
			this.checkBoxFirefighting.Size = new System.Drawing.Size(15, 14);
			this.checkBoxFirefighting.TabIndex = 3;
			this.checkBoxFirefighting.UseVisualStyleBackColor = true;
			// 
			// checkBoxGuard
			// 
			this.checkBoxGuard.AutoSize = true;
			this.checkBoxGuard.Location = new System.Drawing.Point(366, 55);
			this.checkBoxGuard.Name = "checkBoxGuard";
			this.checkBoxGuard.Size = new System.Drawing.Size(15, 14);
			this.checkBoxGuard.TabIndex = 4;
			this.checkBoxGuard.UseVisualStyleBackColor = true;
			// 
			// checkBoxSKD
			// 
			this.checkBoxSKD.AutoSize = true;
			this.checkBoxSKD.Location = new System.Drawing.Point(366, 77);
			this.checkBoxSKD.Name = "checkBoxSKD";
			this.checkBoxSKD.Size = new System.Drawing.Size(15, 14);
			this.checkBoxSKD.TabIndex = 5;
			this.checkBoxSKD.UseVisualStyleBackColor = true;
			// 
			// checkBoxVideo
			// 
			this.checkBoxVideo.AutoSize = true;
			this.checkBoxVideo.Location = new System.Drawing.Point(366, 99);
			this.checkBoxVideo.Name = "checkBoxVideo";
			this.checkBoxVideo.Size = new System.Drawing.Size(15, 14);
			this.checkBoxVideo.TabIndex = 6;
			this.checkBoxVideo.UseVisualStyleBackColor = true;
			// 
			// checkBoxOpcServer
			// 
			this.checkBoxOpcServer.AutoSize = true;
			this.checkBoxOpcServer.Location = new System.Drawing.Point(366, 121);
			this.checkBoxOpcServer.Name = "checkBoxOpcServer";
			this.checkBoxOpcServer.Size = new System.Drawing.Size(15, 14);
			this.checkBoxOpcServer.TabIndex = 7;
			this.checkBoxOpcServer.UseVisualStyleBackColor = true;
			// 
			// labelRemoteWorkplacesCount
			// 
			this.labelRemoteWorkplacesCount.AutoSize = true;
			this.labelRemoteWorkplacesCount.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelRemoteWorkplacesCount.Location = new System.Drawing.Point(5, 2);
			this.labelRemoteWorkplacesCount.Name = "labelRemoteWorkplacesCount";
			this.labelRemoteWorkplacesCount.Size = new System.Drawing.Size(353, 26);
			this.labelRemoteWorkplacesCount.TabIndex = 8;
			this.labelRemoteWorkplacesCount.Text = "GLOBAL Удаленное рабочее место (количество):";
			this.labelRemoteWorkplacesCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelGuard
			// 
			this.labelGuard.AutoSize = true;
			this.labelGuard.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelGuard.Location = new System.Drawing.Point(5, 52);
			this.labelGuard.Name = "labelGuard";
			this.labelGuard.Size = new System.Drawing.Size(353, 20);
			this.labelGuard.TabIndex = 10;
			this.labelGuard.Text = "GLOBAL Охрана:";
			this.labelGuard.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownRemoteClientsCount
			// 
			this.numericUpDownRemoteClientsCount.Location = new System.Drawing.Point(366, 5);
			this.numericUpDownRemoteClientsCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDownRemoteClientsCount.Name = "numericUpDownRemoteClientsCount";
			this.numericUpDownRemoteClientsCount.Size = new System.Drawing.Size(120, 20);
			this.numericUpDownRemoteClientsCount.TabIndex = 14;
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
			this.panelBottom.Location = new System.Drawing.Point(0, 178);
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
			this.ClientSize = new System.Drawing.Size(498, 208);
			this.Controls.Add(this.tableLayoutPanelCenter);
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.panelTop);
			this.Name = "MainForm";
			this.Text = "Редактор лицензий Rubezh Service";
			this.tableLayoutPanelCenter.ResumeLayout(false);
			this.tableLayoutPanelCenter.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRemoteClientsCount)).EndInit();
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
        private System.Windows.Forms.CheckBox checkBoxFirefighting;
        private System.Windows.Forms.CheckBox checkBoxGuard;
        private System.Windows.Forms.CheckBox checkBoxSKD;
        private System.Windows.Forms.CheckBox checkBoxVideo;
        private System.Windows.Forms.CheckBox checkBoxOpcServer;
        private System.Windows.Forms.Label labelOpcServer;
        private System.Windows.Forms.Label labelVideo;
        private System.Windows.Forms.Label labelSKD;
        private System.Windows.Forms.Label labelFirefighting;
        private System.Windows.Forms.Label labelRemoteWorkplacesCount;
        private System.Windows.Forms.NumericUpDown numericUpDownRemoteClientsCount;
        private System.Windows.Forms.Label labelGuard;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

