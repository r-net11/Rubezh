namespace MakeDeviceModel
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
            this.button1 = new System.Windows.Forms.Button();
            this.RTB_Memo = new System.Windows.Forms.RichTextBox();
            this.Edit_Version = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Edit_TextVersion = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(887, 511);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 21);
            this.button1.TabIndex = 0;
            this.button1.Text = "Создать файл";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // RTB_Memo
            // 
            this.RTB_Memo.Location = new System.Drawing.Point(27, 12);
            this.RTB_Memo.Name = "RTB_Memo";
            this.RTB_Memo.Size = new System.Drawing.Size(604, 536);
            this.RTB_Memo.TabIndex = 1;
            this.RTB_Memo.Text = "";
            // 
            // Edit_Version
            // 
            this.Edit_Version.Location = new System.Drawing.Point(839, 12);
            this.Edit_Version.MaxLength = 30;
            this.Edit_Version.Name = "Edit_Version";
            this.Edit_Version.Size = new System.Drawing.Size(100, 20);
            this.Edit_Version.TabIndex = 2;
            this.Edit_Version.Text = "version6";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(682, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Версия сборки:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(682, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Текст версии АПИ";
            // 
            // Edit_TextVersion
            // 
            this.Edit_TextVersion.Location = new System.Drawing.Point(839, 43);
            this.Edit_TextVersion.Name = "Edit_TextVersion";
            this.Edit_TextVersion.Size = new System.Drawing.Size(165, 20);
            this.Edit_TextVersion.TabIndex = 5;
            this.Edit_TextVersion.Text = "АПИ ОПС Рубеж. Версия 6";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 560);
            this.Controls.Add(this.Edit_TextVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Edit_Version);
            this.Controls.Add(this.RTB_Memo);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox RTB_Memo;
        private System.Windows.Forms.TextBox Edit_Version;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Edit_TextVersion;
    }
}

