namespace ActiveX
{
    partial class ActiveX
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost = new System.Windows.Forms.Integration.ElementHost();
            this.currentDeviceView1 = new CurrentDeviceModule.Views.CurrentDeviceView();
            this.SuspendLayout();
            // 
            // elementHost
            // 
            this.elementHost.Location = new System.Drawing.Point(0, 0);
            this.elementHost.Name = "elementHost";
            this.elementHost.Size = new System.Drawing.Size(174, 175);
            this.elementHost.TabIndex = 0;
            this.elementHost.Text = "elementHost1";
            this.elementHost.Child = this.currentDeviceView1;
            // 
            // ActiveXDeviceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.elementHost);
            this.Name = "ActiveX";
            this.Size = new System.Drawing.Size(181, 181);
            this.Load += new System.EventHandler(this.ActiveXDeviceControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost;
        private CurrentDeviceModule.Views.CurrentDeviceView currentDeviceView1;
    }
}
