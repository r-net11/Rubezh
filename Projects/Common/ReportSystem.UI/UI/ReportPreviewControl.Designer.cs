namespace ReportSystem.UI
{
	partial class ReportPreviewControl
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
			this.documentViewer1 = new DevExpress.XtraPrinting.Preview.DocumentViewer();
			this.SuspendLayout();
			//
			// documentViewer1
			//
			this.documentViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.documentViewer1.IsMetric = false;
			this.documentViewer1.Location = new System.Drawing.Point(0, 0);
			this.documentViewer1.Name = "documentViewer1";
			this.documentViewer1.Size = new System.Drawing.Size(679, 336);
			this.documentViewer1.TabIndex = 0;
			//
			// ReportPreviewControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.documentViewer1);
			this.Name = "ReportPreviewControl";
			this.Size = new System.Drawing.Size(679, 336);
			this.ResumeLayout(false);

		}

		#endregion

		private DevExpress.XtraPrinting.Preview.DocumentViewer documentViewer1;
	}
}
