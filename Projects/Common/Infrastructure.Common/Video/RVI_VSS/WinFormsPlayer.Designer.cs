using System;
using System.Windows.Forms;

namespace Infrastructure.Common.Video.RVI_VSS
{
	partial class WinFormsPlayer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private ContextMenuStrip contextMenuStrip1;
		private ToolStripMenuItem toolStripMenuItem1;
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
			this.components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Text = "WinFormsPlayer";
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Text = "&Архив";
			this.toolStripMenuItem1.Click += ToolStripMenuItem1OnClick;
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1 });
			this.ContextMenuStrip = this.contextMenuStrip1;
			this.contextMenuStrip1.ResumeLayout(false);
			this.PerformLayout();
		}



		#endregion
	}
}