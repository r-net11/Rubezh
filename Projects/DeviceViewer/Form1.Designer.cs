namespace DeviceViewer
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
            this.PlaceInTreeTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.deviceTreeView = new System.Windows.Forms.TreeView();
            this.ZoneTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.DriverIdTextBox = new System.Windows.Forms.TextBox();
            this.MetadataDriverIdTextBox = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.AddressTextBox = new System.Windows.Forms.TextBox();
            this.ParentAddressTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // PlaceInTreeTextBox
            // 
            this.PlaceInTreeTextBox.Location = new System.Drawing.Point(493, 520);
            this.PlaceInTreeTextBox.Name = "PlaceInTreeTextBox";
            this.PlaceInTreeTextBox.Size = new System.Drawing.Size(223, 20);
            this.PlaceInTreeTextBox.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(422, 523);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "PlaceInTree";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(455, 497);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Zone";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column4,
            this.Column3});
            this.dataGridView1.Location = new System.Drawing.Point(391, 29);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(741, 455);
            this.dataGridView1.TabIndex = 14;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "id";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "name";
            this.Column2.Name = "Column2";
            this.Column2.Width = 400;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "class";
            this.Column4.Name = "Column4";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "IsActive";
            this.Column3.Name = "Column3";
            // 
            // deviceTreeView
            // 
            this.deviceTreeView.Location = new System.Drawing.Point(7, 8);
            this.deviceTreeView.Name = "deviceTreeView";
            this.deviceTreeView.Size = new System.Drawing.Size(378, 626);
            this.deviceTreeView.TabIndex = 13;
            this.deviceTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.deviceTreeView_NodeMouseClick);
            // 
            // ZoneTextBox
            // 
            this.ZoneTextBox.Location = new System.Drawing.Point(493, 494);
            this.ZoneTextBox.Name = "ZoneTextBox";
            this.ZoneTextBox.Size = new System.Drawing.Size(223, 20);
            this.ZoneTextBox.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(744, 492);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(144, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Get Device State";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(443, 549);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "DriverId";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(402, 575);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "MetadataDriverId";
            // 
            // DriverIdTextBox
            // 
            this.DriverIdTextBox.Location = new System.Drawing.Point(493, 546);
            this.DriverIdTextBox.Name = "DriverIdTextBox";
            this.DriverIdTextBox.Size = new System.Drawing.Size(223, 20);
            this.DriverIdTextBox.TabIndex = 21;
            // 
            // MetadataDriverIdTextBox
            // 
            this.MetadataDriverIdTextBox.Location = new System.Drawing.Point(493, 572);
            this.MetadataDriverIdTextBox.Name = "MetadataDriverIdTextBox";
            this.MetadataDriverIdTextBox.Size = new System.Drawing.Size(223, 20);
            this.MetadataDriverIdTextBox.TabIndex = 22;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(744, 520);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(349, 331);
            this.richTextBox1.TabIndex = 23;
            this.richTextBox1.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(421, 601);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Self Address";
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Location = new System.Drawing.Point(493, 598);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(223, 20);
            this.AddressTextBox.TabIndex = 25;
            // 
            // ParentAddressTextBox
            // 
            this.ParentAddressTextBox.Location = new System.Drawing.Point(493, 624);
            this.ParentAddressTextBox.Name = "ParentAddressTextBox";
            this.ParentAddressTextBox.Size = new System.Drawing.Size(223, 20);
            this.ParentAddressTextBox.TabIndex = 29;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(411, 627);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "ParentAddress";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1144, 863);
            this.Controls.Add(this.ParentAddressTextBox);
            this.Controls.Add(this.AddressTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.MetadataDriverIdTextBox);
            this.Controls.Add(this.DriverIdTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PlaceInTreeTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.deviceTreeView);
            this.Controls.Add(this.ZoneTextBox);
            this.Controls.Add(this.label6);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PlaceInTreeTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TreeView deviceTreeView;
        private System.Windows.Forms.TextBox ZoneTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox DriverIdTextBox;
        private System.Windows.Forms.TextBox MetadataDriverIdTextBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox AddressTextBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column3;
        private System.Windows.Forms.TextBox ParentAddressTextBox;
        private System.Windows.Forms.Label label6;
    }
}

