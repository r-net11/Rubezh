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
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.textBoxKey = new System.Windows.Forms.TextBox();
            this.checkBoxFireAlarm = new System.Windows.Forms.CheckBox();
            this.checkBoxSecurityAlarm = new System.Windows.Forms.CheckBox();
            this.checkBoxSkd = new System.Windows.Forms.CheckBox();
            this.checkBoxControlScripts = new System.Windows.Forms.CheckBox();
            this.checkBoxOpcServer = new System.Windows.Forms.CheckBox();
            this.labelNumberOfUsers = new System.Windows.Forms.Label();
            this.labelFireAlarm = new System.Windows.Forms.Label();
            this.labelSecurityAlarm = new System.Windows.Forms.Label();
            this.labelSkd = new System.Windows.Forms.Label();
            this.labelControlScripts = new System.Windows.Forms.Label();
            this.labelOpcServer = new System.Windows.Forms.Label();
            this.numericUpDownNumberOfUsers = new System.Windows.Forms.NumericUpDown();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tableLayoutPanelCenter.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumberOfUsers)).BeginInit();
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
            this.tableLayoutPanelCenter.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanelCenter.ColumnCount = 2;
            this.tableLayoutPanelCenter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73F));
            this.tableLayoutPanelCenter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.tableLayoutPanelCenter.Controls.Add(this.labelOpcServer, 0, 5);
            this.tableLayoutPanelCenter.Controls.Add(this.labelControlScripts, 0, 4);
            this.tableLayoutPanelCenter.Controls.Add(this.labelSkd, 0, 3);
            this.tableLayoutPanelCenter.Controls.Add(this.labelFireAlarm, 0, 1);
            this.tableLayoutPanelCenter.Controls.Add(this.checkBoxFireAlarm, 1, 1);
            this.tableLayoutPanelCenter.Controls.Add(this.checkBoxSecurityAlarm, 1, 2);
            this.tableLayoutPanelCenter.Controls.Add(this.checkBoxSkd, 1, 3);
            this.tableLayoutPanelCenter.Controls.Add(this.checkBoxControlScripts, 1, 4);
            this.tableLayoutPanelCenter.Controls.Add(this.checkBoxOpcServer, 1, 5);
            this.tableLayoutPanelCenter.Controls.Add(this.labelNumberOfUsers, 0, 0);
            this.tableLayoutPanelCenter.Controls.Add(this.numericUpDownNumberOfUsers, 1, 0);
            this.tableLayoutPanelCenter.Controls.Add(this.labelSecurityAlarm, 0, 2);
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
            this.tableLayoutPanelCenter.Size = new System.Drawing.Size(498, 142);
            this.tableLayoutPanelCenter.TabIndex = 1;
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
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonSave);
            this.panelBottom.Controls.Add(this.buttonLoad);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 176);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(498, 30);
            this.panelBottom.TabIndex = 3;
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
            // checkBoxFireAlarm
            // 
            this.checkBoxFireAlarm.AutoSize = true;
            this.checkBoxFireAlarm.Location = new System.Drawing.Point(366, 33);
            this.checkBoxFireAlarm.Name = "checkBoxFireAlarm";
            this.checkBoxFireAlarm.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFireAlarm.TabIndex = 3;
            this.checkBoxFireAlarm.UseVisualStyleBackColor = true;
            // 
            // checkBoxSecurityAlarm
            // 
            this.checkBoxSecurityAlarm.AutoSize = true;
            this.checkBoxSecurityAlarm.Location = new System.Drawing.Point(366, 55);
            this.checkBoxSecurityAlarm.Name = "checkBoxSecurityAlarm";
            this.checkBoxSecurityAlarm.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSecurityAlarm.TabIndex = 4;
            this.checkBoxSecurityAlarm.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkd
            // 
            this.checkBoxSkd.AutoSize = true;
            this.checkBoxSkd.Location = new System.Drawing.Point(366, 77);
            this.checkBoxSkd.Name = "checkBoxSkd";
            this.checkBoxSkd.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSkd.TabIndex = 5;
            this.checkBoxSkd.UseVisualStyleBackColor = true;
            // 
            // checkBoxControlScripts
            // 
            this.checkBoxControlScripts.AutoSize = true;
            this.checkBoxControlScripts.Location = new System.Drawing.Point(366, 99);
            this.checkBoxControlScripts.Name = "checkBoxControlScripts";
            this.checkBoxControlScripts.Size = new System.Drawing.Size(15, 14);
            this.checkBoxControlScripts.TabIndex = 6;
            this.checkBoxControlScripts.UseVisualStyleBackColor = true;
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
            // labelNumberOfUsers
            // 
            this.labelNumberOfUsers.AutoSize = true;
            this.labelNumberOfUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNumberOfUsers.Location = new System.Drawing.Point(5, 2);
            this.labelNumberOfUsers.Name = "labelNumberOfUsers";
            this.labelNumberOfUsers.Size = new System.Drawing.Size(353, 26);
            this.labelNumberOfUsers.TabIndex = 8;
            this.labelNumberOfUsers.Text = "Количество пользователей:";
            this.labelNumberOfUsers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelFireAlarm
            // 
            this.labelFireAlarm.AutoSize = true;
            this.labelFireAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFireAlarm.Location = new System.Drawing.Point(5, 30);
            this.labelFireAlarm.Name = "labelFireAlarm";
            this.labelFireAlarm.Size = new System.Drawing.Size(353, 20);
            this.labelFireAlarm.TabIndex = 9;
            this.labelFireAlarm.Text = "Пожарная сигнализация и пожаротушение:";
            this.labelFireAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSecurityAlarm
            // 
            this.labelSecurityAlarm.AutoSize = true;
            this.labelSecurityAlarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSecurityAlarm.Location = new System.Drawing.Point(5, 52);
            this.labelSecurityAlarm.Name = "labelSecurityAlarm";
            this.labelSecurityAlarm.Size = new System.Drawing.Size(353, 20);
            this.labelSecurityAlarm.TabIndex = 10;
            this.labelSecurityAlarm.Text = "Охранная сигнализация:";
            this.labelSecurityAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSkd
            // 
            this.labelSkd.AutoSize = true;
            this.labelSkd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSkd.Location = new System.Drawing.Point(5, 74);
            this.labelSkd.Name = "labelSkd";
            this.labelSkd.Size = new System.Drawing.Size(353, 20);
            this.labelSkd.TabIndex = 11;
            this.labelSkd.Text = "СКД:";
            this.labelSkd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelControlScripts
            // 
            this.labelControlScripts.AutoSize = true;
            this.labelControlScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControlScripts.Location = new System.Drawing.Point(5, 96);
            this.labelControlScripts.Name = "labelControlScripts";
            this.labelControlScripts.Size = new System.Drawing.Size(353, 20);
            this.labelControlScripts.TabIndex = 12;
            this.labelControlScripts.Text = "Сценарии управления:";
            this.labelControlScripts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelOpcServer
            // 
            this.labelOpcServer.AutoSize = true;
            this.labelOpcServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelOpcServer.Location = new System.Drawing.Point(5, 118);
            this.labelOpcServer.Name = "labelOpcServer";
            this.labelOpcServer.Size = new System.Drawing.Size(353, 22);
            this.labelOpcServer.TabIndex = 13;
            this.labelOpcServer.Text = "ОРС-Сервер:";
            this.labelOpcServer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownNumberOfUsers
            // 
            this.numericUpDownNumberOfUsers.Location = new System.Drawing.Point(366, 5);
            this.numericUpDownNumberOfUsers.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownNumberOfUsers.Name = "numericUpDownNumberOfUsers";
            this.numericUpDownNumberOfUsers.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownNumberOfUsers.TabIndex = 14;
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
            this.ClientSize = new System.Drawing.Size(498, 206);
            this.Controls.Add(this.tableLayoutPanelCenter);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Name = "MainForm";
            this.Text = "Редактор лицензий Firesec Service";
            this.tableLayoutPanelCenter.ResumeLayout(false);
            this.tableLayoutPanelCenter.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumberOfUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelKey;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCenter;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox textBoxKey;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.CheckBox checkBoxFireAlarm;
        private System.Windows.Forms.CheckBox checkBoxSecurityAlarm;
        private System.Windows.Forms.CheckBox checkBoxSkd;
        private System.Windows.Forms.CheckBox checkBoxControlScripts;
        private System.Windows.Forms.CheckBox checkBoxOpcServer;
        private System.Windows.Forms.Label labelOpcServer;
        private System.Windows.Forms.Label labelControlScripts;
        private System.Windows.Forms.Label labelSkd;
        private System.Windows.Forms.Label labelFireAlarm;
        private System.Windows.Forms.Label labelNumberOfUsers;
        private System.Windows.Forms.NumericUpDown numericUpDownNumberOfUsers;
        private System.Windows.Forms.Label labelSecurityAlarm;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

