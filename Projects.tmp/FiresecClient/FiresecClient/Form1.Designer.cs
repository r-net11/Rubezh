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
			this.Test = new System.Windows.Forms.Button();
			this.StartPoll = new System.Windows.Forms.Button();
			this.Poll = new System.Windows.Forms.Button();
			this.Disconnect = new System.Windows.Forms.Button();
			this.Connect = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.Test2 = new System.Windows.Forms.Button();
			this.StartPoll2 = new System.Windows.Forms.Button();
			this.Poll2 = new System.Windows.Forms.Button();
			this.Disconnect2 = new System.Windows.Forms.Button();
			this.Connect2 = new System.Windows.Forms.Button();
			this.NewChannelFactory = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage0.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
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
			this.tabPage0.Controls.Add(this.NewChannelFactory);
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
			this.tableLayoutPanel1.Controls.Add(this.Test, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.StartPoll, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.Poll, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.Disconnect, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.Connect, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(609, 448);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// Test
			// 
			this.Test.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Test.Location = new System.Drawing.Point(3, 301);
			this.Test.Name = "Test";
			this.Test.Size = new System.Drawing.Size(298, 144);
			this.Test.TabIndex = 4;
			this.Test.Text = "Test";
			this.Test.UseVisualStyleBackColor = true;
			this.Test.Click += new System.EventHandler(this.Test_Click);
			// 
			// StartPoll
			// 
			this.StartPoll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StartPoll.Location = new System.Drawing.Point(307, 152);
			this.StartPoll.Name = "StartPoll";
			this.StartPoll.Size = new System.Drawing.Size(299, 143);
			this.StartPoll.TabIndex = 3;
			this.StartPoll.Text = "StartPoll";
			this.StartPoll.UseVisualStyleBackColor = true;
			this.StartPoll.Click += new System.EventHandler(this.StartPoll_Click);
			// 
			// Poll
			// 
			this.Poll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Poll.Location = new System.Drawing.Point(3, 152);
			this.Poll.Name = "Poll";
			this.Poll.Size = new System.Drawing.Size(298, 143);
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
			this.Disconnect.Size = new System.Drawing.Size(299, 143);
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
			this.Connect.Size = new System.Drawing.Size(298, 143);
			this.Connect.TabIndex = 0;
			this.Connect.Text = "Connect";
			this.Connect.UseVisualStyleBackColor = true;
			this.Connect.Click += new System.EventHandler(this.Connect_Click);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.tableLayoutPanel2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(615, 454);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.Test2, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.StartPoll2, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.Poll2, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.Disconnect2, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.Connect2, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(609, 448);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// Test2
			// 
			this.Test2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Test2.Location = new System.Drawing.Point(3, 301);
			this.Test2.Name = "Test2";
			this.Test2.Size = new System.Drawing.Size(298, 144);
			this.Test2.TabIndex = 4;
			this.Test2.Text = "Test2";
			this.Test2.UseVisualStyleBackColor = true;
			this.Test2.Click += new System.EventHandler(this.Test2_Click);
			// 
			// StartPoll2
			// 
			this.StartPoll2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.StartPoll2.Location = new System.Drawing.Point(307, 152);
			this.StartPoll2.Name = "StartPoll2";
			this.StartPoll2.Size = new System.Drawing.Size(299, 143);
			this.StartPoll2.TabIndex = 3;
			this.StartPoll2.Text = "StartPoll2";
			this.StartPoll2.UseVisualStyleBackColor = true;
			// 
			// Poll2
			// 
			this.Poll2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Poll2.Location = new System.Drawing.Point(3, 152);
			this.Poll2.Name = "Poll2";
			this.Poll2.Size = new System.Drawing.Size(298, 143);
			this.Poll2.TabIndex = 2;
			this.Poll2.Text = "Poll2";
			this.Poll2.UseVisualStyleBackColor = true;
			this.Poll2.Click += new System.EventHandler(this.Poll2_Click);
			// 
			// Disconnect2
			// 
			this.Disconnect2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Disconnect2.Location = new System.Drawing.Point(307, 3);
			this.Disconnect2.Name = "Disconnect2";
			this.Disconnect2.Size = new System.Drawing.Size(299, 143);
			this.Disconnect2.TabIndex = 1;
			this.Disconnect2.Text = "Disconnect2";
			this.Disconnect2.UseVisualStyleBackColor = true;
			this.Disconnect2.Click += new System.EventHandler(this.Disconnect2_Click);
			// 
			// Connect2
			// 
			this.Connect2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Connect2.Location = new System.Drawing.Point(3, 3);
			this.Connect2.Name = "Connect2";
			this.Connect2.Size = new System.Drawing.Size(298, 143);
			this.Connect2.TabIndex = 0;
			this.Connect2.Text = "Connect2";
			this.Connect2.UseVisualStyleBackColor = true;
			this.Connect2.Click += new System.EventHandler(this.Connect2_Click);
			// 
			// NewChannelFactory
			// 
			this.NewChannelFactory.Location = new System.Drawing.Point(8, 84);
			this.NewChannelFactory.Name = "NewChannelFactory";
			this.NewChannelFactory.Size = new System.Drawing.Size(169, 23);
			this.NewChannelFactory.TabIndex = 3;
			this.NewChannelFactory.Text = "NewChannelFactory";
			this.NewChannelFactory.UseVisualStyleBackColor = true;
			this.NewChannelFactory.Click += new System.EventHandler(this.NewChannelFactory_Click);
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
			this.tabPage2.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
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
		private System.Windows.Forms.Button Test;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button Test2;
		private System.Windows.Forms.Button StartPoll2;
		private System.Windows.Forms.Button Poll2;
		private System.Windows.Forms.Button Disconnect2;
		private System.Windows.Forms.Button Connect2;
		private System.Windows.Forms.Button NewChannelFactory;
	}
}

