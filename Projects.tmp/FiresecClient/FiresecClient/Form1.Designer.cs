namespace FiresecClient
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage0 = new System.Windows.Forms.TabPage();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.StartPoll = new System.Windows.Forms.Button();
			this.Poll = new System.Windows.Forms.Button();
			this.Disconnect = new System.Windows.Forms.Button();
			this.Connect = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabControl1.SuspendLayout();
			this.tabPage0.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage0);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(623, 480);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage0
			// 
			this.tabPage0.Controls.Add(this.textBox3);
			this.tabPage0.Controls.Add(this.textBox2);
			this.tabPage0.Controls.Add(this.textBox1);
			this.tabPage0.Location = new System.Drawing.Point(4, 22);
			this.tabPage0.Name = "tabPage0";
			this.tabPage0.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage0.Size = new System.Drawing.Size(615, 454);
			this.tabPage0.TabIndex = 2;
			this.tabPage0.Text = "tabPage0";
			this.tabPage0.UseVisualStyleBackColor = true;
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(8, 58);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(169, 20);
			this.textBox3.TabIndex = 2;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(8, 32);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(169, 20);
			this.textBox2.TabIndex = 1;
			this.textBox2.Text = "adm";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(8, 6);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(169, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "net.tcp://192.168.21.70:9988/FiresecService/";
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.tableLayoutPanel1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(615, 454);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.StartPoll, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.Poll, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.Disconnect, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.Connect, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(609, 448);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// StartPoll
			// 
			this.StartPoll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StartPoll.Location = new System.Drawing.Point(307, 227);
			this.StartPoll.Name = "StartPoll";
			this.StartPoll.Size = new System.Drawing.Size(299, 218);
			this.StartPoll.TabIndex = 3;
			this.StartPoll.Text = "StartPoll";
			this.StartPoll.UseVisualStyleBackColor = true;
			this.StartPoll.Click += new System.EventHandler(this.StartPoll_Click);
			// 
			// Poll
			// 
			this.Poll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Poll.Location = new System.Drawing.Point(3, 227);
			this.Poll.Name = "Poll";
			this.Poll.Size = new System.Drawing.Size(298, 218);
			this.Poll.TabIndex = 2;
			this.Poll.Text = "Poll";
			this.Poll.UseVisualStyleBackColor = true;
			this.Poll.Click += new System.EventHandler(this.Poll_Click);
			// 
			// Disconnect
			// 
			this.Disconnect.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Disconnect.Location = new System.Drawing.Point(307, 3);
			this.Disconnect.Name = "Disconnect";
			this.Disconnect.Size = new System.Drawing.Size(299, 218);
			this.Disconnect.TabIndex = 1;
			this.Disconnect.Text = "Disconnect";
			this.Disconnect.UseVisualStyleBackColor = true;
			this.Disconnect.Click += new System.EventHandler(this.Disconnect_Click);
			// 
			// Connect
			// 
			this.Connect.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Connect.Location = new System.Drawing.Point(3, 3);
			this.Connect.Name = "Connect";
			this.Connect.Size = new System.Drawing.Size(298, 218);
			this.Connect.TabIndex = 0;
			this.Connect.Text = "Connect";
			this.Connect.UseVisualStyleBackColor = true;
			this.Connect.Click += new System.EventHandler(this.Connect_Click);
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(615, 454);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(623, 480);
			this.Controls.Add(this.tabControl1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.tabControl1.ResumeLayout(false);
			this.tabPage0.ResumeLayout(false);
			this.tabPage0.PerformLayout();
			this.tabPage1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button Poll;
		private System.Windows.Forms.Button Disconnect;
		private System.Windows.Forms.Button Connect;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage0;
		private System.Windows.Forms.Button StartPoll;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox2;
	}
}

