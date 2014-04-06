using System;
using System.Drawing;
using System.Windows.Forms;

namespace Infrastructure.Common.Video.RVI_VSS
{
	partial class WinFormsPlayer
	{
		private System.ComponentModel.IContainer components = null;
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
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(6, 9);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(this.Width/30, this.Height/100);
			this.label.TabIndex = 0;
			this.label.ForeColor = Color.White;

			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label);
			this.Name = "WinFormsPlayer";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private Label label;
	}
}