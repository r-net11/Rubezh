namespace FiresecService.Views
{
	partial class MainView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
			this._statusStripMain = new System.Windows.Forms.StatusStrip();
			this._toolStripStatusLabelLastLog = new System.Windows.Forms.ToolStripStatusLabel();
			this._tabControlMain = new System.Windows.Forms.TabControl();
			this._statusStripMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// _statusStripMain
			// 
			this._statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			this._statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this._toolStripStatusLabelLastLog});
			this._statusStripMain.Location = new System.Drawing.Point(0, 646);
			this._statusStripMain.Name = "_statusStripMain";
			this._statusStripMain.Size = new System.Drawing.Size(834, 25);
			this._statusStripMain.TabIndex = 1;
			this._statusStripMain.Text = "statusStrip1";
			// 
			// _toolStripStatusLabelLastLog
			// 
			this._toolStripStatusLabelLastLog.Name = "_toolStripStatusLabelLastLog";
			this._toolStripStatusLabelLastLog.Size = new System.Drawing.Size(60, 20);
			this._toolStripStatusLabelLastLog.Text = "LastLog";
			// 
			// _tabControlMain
			// 
			this._tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControlMain.Location = new System.Drawing.Point(0, 0);
			this._tabControlMain.Name = "_tabControlMain";
			this._tabControlMain.SelectedIndex = 0;
			this._tabControlMain.Size = new System.Drawing.Size(834, 646);
			this._tabControlMain.TabIndex = 2;
			// 
			// MainView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(834, 671);
			this.Controls.Add(this._tabControlMain);
			this.Controls.Add(this._statusStripMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(850, 650);
			this.Name = "MainView";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MainWinFormView";
			this._statusStripMain.ResumeLayout(false);
			this._statusStripMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip _statusStripMain;
		private System.Windows.Forms.TabControl _tabControlMain;
		private System.Windows.Forms.ToolStripStatusLabel _toolStripStatusLabelLastLog;
	}
}