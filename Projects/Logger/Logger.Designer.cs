namespace Logger
{
    partial class Logger
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
            this.outgoinglistBox = new System.Windows.Forms.ListBox();
            this.incomminglistBox = new System.Windows.Forms.ListBox();
            this.tabLoggerControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.RTB_outgoing = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.RTB_incomming = new System.Windows.Forms.RichTextBox();
            this.tabOption = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.inLogCount = new System.Windows.Forms.Label();
            this.outNameLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.outLogCount = new System.Windows.Forms.Label();
            this.inNameLabel = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabLoggerControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabOption.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // outgoinglistBox
            // 
            this.outgoinglistBox.FormattingEnabled = true;
            this.outgoinglistBox.HorizontalScrollbar = true;
            this.outgoinglistBox.ItemHeight = 16;
            this.outgoinglistBox.Location = new System.Drawing.Point(0, 1);
            this.outgoinglistBox.Name = "outgoinglistBox";
            this.outgoinglistBox.Size = new System.Drawing.Size(489, 468);
            this.outgoinglistBox.TabIndex = 12;
            this.outgoinglistBox.SelectedIndexChanged += new System.EventHandler(this.outgoinglistBox_SelectedIndexChanged);
            // 
            // incomminglistBox
            // 
            this.incomminglistBox.FormattingEnabled = true;
            this.incomminglistBox.HorizontalScrollbar = true;
            this.incomminglistBox.ItemHeight = 16;
            this.incomminglistBox.Location = new System.Drawing.Point(3, 6);
            this.incomminglistBox.Name = "incomminglistBox";
            this.incomminglistBox.Size = new System.Drawing.Size(482, 452);
            this.incomminglistBox.TabIndex = 13;
            this.incomminglistBox.SelectedIndexChanged += new System.EventHandler(this.incomminglistBox_SelectedIndexChanged);
            // 
            // tabLoggerControl
            // 
            this.tabLoggerControl.Controls.Add(this.tabPage1);
            this.tabLoggerControl.Controls.Add(this.tabPage2);
            this.tabLoggerControl.Controls.Add(this.tabOption);
            this.tabLoggerControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabLoggerControl.Location = new System.Drawing.Point(12, 28);
            this.tabLoggerControl.Name = "tabLoggerControl";
            this.tabLoggerControl.SelectedIndex = 0;
            this.tabLoggerControl.Size = new System.Drawing.Size(983, 498);
            this.tabLoggerControl.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.RTB_outgoing);
            this.tabPage1.Controls.Add(this.outgoinglistBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(975, 469);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Выдаваемая в АССАД информация";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // RTB_outgoing
            // 
            this.RTB_outgoing.BackColor = System.Drawing.SystemColors.Info;
            this.RTB_outgoing.ForeColor = System.Drawing.SystemColors.WindowText;
            this.RTB_outgoing.Location = new System.Drawing.Point(495, 3);
            this.RTB_outgoing.Name = "RTB_outgoing";
            this.RTB_outgoing.ReadOnly = true;
            this.RTB_outgoing.Size = new System.Drawing.Size(480, 466);
            this.RTB_outgoing.TabIndex = 13;
            this.RTB_outgoing.Text = "";
            this.RTB_outgoing.WordWrap = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.RTB_incomming);
            this.tabPage2.Controls.Add(this.incomminglistBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(975, 469);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Принимаемая из АССАДа информация";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // RTB_incomming
            // 
            this.RTB_incomming.BackColor = System.Drawing.SystemColors.Info;
            this.RTB_incomming.ForeColor = System.Drawing.SystemColors.WindowText;
            this.RTB_incomming.Location = new System.Drawing.Point(491, 6);
            this.RTB_incomming.Name = "RTB_incomming";
            this.RTB_incomming.Size = new System.Drawing.Size(478, 452);
            this.RTB_incomming.TabIndex = 14;
            this.RTB_incomming.Text = "";
            // 
            // tabOption
            // 
            this.tabOption.Controls.Add(this.groupBox3);
            this.tabOption.Controls.Add(this.groupBox2);
            this.tabOption.Controls.Add(this.groupBox1);
            this.tabOption.Location = new System.Drawing.Point(4, 25);
            this.tabOption.Name = "tabOption";
            this.tabOption.Padding = new System.Windows.Forms.Padding(3);
            this.tabOption.Size = new System.Drawing.Size(975, 469);
            this.tabOption.TabIndex = 2;
            this.tabOption.Text = "Опции/Статистика";
            this.tabOption.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Location = new System.Drawing.Point(587, 128);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(372, 100);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Опции:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(16, 21);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(312, 20);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Сохранение при выходе из программы";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Location = new System.Drawing.Point(587, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(372, 116);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Управление логами";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(150, 53);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(210, 23);
            this.button5.TabIndex = 7;
            this.button5.Text = "Считать лог из файла..";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(150, 24);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(210, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Записать лог в файл...";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(16, 24);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(115, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Записать лог ";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 53);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Считать лог  ";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.outNameLabel);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.inNameLabel);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 92);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Количество записей в коллекциях";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.inLogCount);
            this.panel1.Location = new System.Drawing.Point(146, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(138, 25);
            this.panel1.TabIndex = 2;
            // 
            // inLogCount
            // 
            this.inLogCount.AutoSize = true;
            this.inLogCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.inLogCount.Location = new System.Drawing.Point(3, 0);
            this.inLogCount.Name = "inLogCount";
            this.inLogCount.Size = new System.Drawing.Size(0, 18);
            this.inLogCount.TabIndex = 0;
            // 
            // outNameLabel
            // 
            this.outNameLabel.AutoSize = true;
            this.outNameLabel.Location = new System.Drawing.Point(16, 49);
            this.outNameLabel.Name = "outNameLabel";
            this.outNameLabel.Size = new System.Drawing.Size(124, 16);
            this.outNameLabel.TabIndex = 3;
            this.outNameLabel.Text = "outLogCollection";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.outLogCount);
            this.panel2.Location = new System.Drawing.Point(146, 49);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(138, 25);
            this.panel2.TabIndex = 3;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // outLogCount
            // 
            this.outLogCount.AutoSize = true;
            this.outLogCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.outLogCount.Location = new System.Drawing.Point(2, 0);
            this.outLogCount.Name = "outLogCount";
            this.outLogCount.Size = new System.Drawing.Size(0, 18);
            this.outLogCount.TabIndex = 0;
            // 
            // inNameLabel
            // 
            this.inNameLabel.AutoSize = true;
            this.inNameLabel.Location = new System.Drawing.Point(16, 27);
            this.inNameLabel.Name = "inNameLabel";
            this.inNameLabel.Size = new System.Drawing.Size(115, 16);
            this.inNameLabel.TabIndex = 0;
            this.inNameLabel.Text = "inLogCollection";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "XML-файлы|*.xml|Все файлы|*.*";
            this.saveFileDialog1.OverwritePrompt = false;
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "XML-файлы|*.xml|Все файлы|*.*";
            // 
            // Logger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 567);
            this.Controls.Add(this.tabLoggerControl);
            this.Name = "Logger";
            this.Text = "Logger";
            this.Deactivate += new System.EventHandler(this.Logger_Deactivate);
            this.Load += new System.EventHandler(this.Logger_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Logger_FormClosing);
            this.tabLoggerControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabOption.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox outgoinglistBox;
        private System.Windows.Forms.ListBox incomminglistBox;
        private System.Windows.Forms.TabControl tabLoggerControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabOption;
        private System.Windows.Forms.Label inNameLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label outNameLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label inLogCount;
        private System.Windows.Forms.Label outLogCount;
        private System.Windows.Forms.RichTextBox RTB_incomming;
        private System.Windows.Forms.RichTextBox RTB_outgoing;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}