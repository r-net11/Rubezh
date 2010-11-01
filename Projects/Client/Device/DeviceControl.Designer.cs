namespace Client.UserControls
{
    partial class DeviceControl
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
            this.labelType = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.labelId = new System.Windows.Forms.Label();
            this.labelParameters = new System.Windows.Forms.Label();
            this.labelStates = new System.Windows.Forms.Label();
            this.labelCommands = new System.Windows.Forms.Label();
            this.labelEvents = new System.Windows.Forms.Label();
            this.listBoxCommands = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(148, 0);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(89, 13);
            this.labelType.TabIndex = 0;
            this.labelType.Text = "Тип Устройства";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(148, 20);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(92, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Имя Устройства";
            // 
            // labelId
            // 
            this.labelId.AutoSize = true;
            this.labelId.Location = new System.Drawing.Point(148, 39);
            this.labelId.Name = "labelId";
            this.labelId.Size = new System.Drawing.Size(150, 13);
            this.labelId.TabIndex = 2;
            this.labelId.Text = "Идентификатор Устройства";
            // 
            // labelParameters
            // 
            this.labelParameters.AutoSize = true;
            this.labelParameters.Location = new System.Drawing.Point(148, 62);
            this.labelParameters.Name = "labelParameters";
            this.labelParameters.Size = new System.Drawing.Size(66, 13);
            this.labelParameters.TabIndex = 3;
            this.labelParameters.Text = "Параметры";
            // 
            // labelStates
            // 
            this.labelStates.AutoSize = true;
            this.labelStates.Location = new System.Drawing.Point(148, 75);
            this.labelStates.Name = "labelStates";
            this.labelStates.Size = new System.Drawing.Size(61, 13);
            this.labelStates.TabIndex = 4;
            this.labelStates.Text = "Состояния";
            // 
            // labelCommands
            // 
            this.labelCommands.AutoSize = true;
            this.labelCommands.Location = new System.Drawing.Point(148, 88);
            this.labelCommands.Name = "labelCommands";
            this.labelCommands.Size = new System.Drawing.Size(54, 13);
            this.labelCommands.TabIndex = 5;
            this.labelCommands.Text = "Команды";
            // 
            // labelEvents
            // 
            this.labelEvents.AutoSize = true;
            this.labelEvents.Location = new System.Drawing.Point(148, 202);
            this.labelEvents.Name = "labelEvents";
            this.labelEvents.Size = new System.Drawing.Size(51, 13);
            this.labelEvents.TabIndex = 6;
            this.labelEvents.Text = "События";
            // 
            // listBoxCommands
            // 
            this.listBoxCommands.FormattingEnabled = true;
            this.listBoxCommands.Location = new System.Drawing.Point(151, 104);
            this.listBoxCommands.Name = "listBoxCommands";
            this.listBoxCommands.Size = new System.Drawing.Size(201, 95);
            this.listBoxCommands.TabIndex = 7;
            // 
            // DynamicDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBoxCommands);
            this.Controls.Add(this.labelEvents);
            this.Controls.Add(this.labelCommands);
            this.Controls.Add(this.labelStates);
            this.Controls.Add(this.labelParameters);
            this.Controls.Add(this.labelId);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.labelType);
            this.Name = "DynamicDevice";
            this.Size = new System.Drawing.Size(933, 471);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelId;
        private System.Windows.Forms.Label labelParameters;
        private System.Windows.Forms.Label labelStates;
        private System.Windows.Forms.Label labelCommands;
        private System.Windows.Forms.Label labelEvents;
        private System.Windows.Forms.ListBox listBoxCommands;

    }
}
