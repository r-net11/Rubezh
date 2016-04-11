using System.Windows.Forms.Integration;

namespace RviClient.UI.VideoCell
{
	partial class PlayerPanel
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
			this.backgroundElementHost = new System.Windows.Forms.Integration.ElementHost();
			this.topPanel = new System.Windows.Forms.Panel();
			this.topElementHost = new System.Windows.Forms.Integration.ElementHost();
			this.bottomElementHost = new System.Windows.Forms.Integration.ElementHost();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.topPanel.SuspendLayout();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// backgroundElementHost
			// 
			this.backgroundElementHost.BackColor = System.Drawing.SystemColors.ControlLight;
			this.backgroundElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this.backgroundElementHost.Location = new System.Drawing.Point(0, 0);
			this.backgroundElementHost.Name = "backgroundElementHost";
			this.backgroundElementHost.Size = new System.Drawing.Size(428, 150);
			this.backgroundElementHost.TabIndex = 0;
			this.backgroundElementHost.Child = null;
			// 
			// topPanel
			// 
			this.topPanel.Controls.Add(this.topElementHost);
			this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.topPanel.Location = new System.Drawing.Point(0, 0);
			this.topPanel.Margin = new System.Windows.Forms.Padding(0);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(428, 25);
			this.topPanel.TabIndex = 1;
			this.topPanel.Visible = false;
			// 
			// topElementHost
			// 
			this.topElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this.topElementHost.Location = new System.Drawing.Point(0, 0);
			this.topElementHost.Margin = new System.Windows.Forms.Padding(0);
			this.topElementHost.Name = "topElementHost";
			this.topElementHost.Size = new System.Drawing.Size(428, 25);
			this.topElementHost.TabIndex = 1;
			this.topElementHost.Child = null;
			// 
			// bottomElementHost
			// 
			this.bottomElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this.bottomElementHost.Location = new System.Drawing.Point(0, 0);
			this.bottomElementHost.Margin = new System.Windows.Forms.Padding(0);
			this.bottomElementHost.Name = "bottomElementHost";
			this.bottomElementHost.Size = new System.Drawing.Size(428, 25);
			this.bottomElementHost.TabIndex = 0;
			this.bottomElementHost.Child = null;
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.bottomElementHost);
			this.bottomPanel.Location = new System.Drawing.Point(0, 125);
			this.bottomPanel.Margin = new System.Windows.Forms.Padding(0);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(428, 25);
			this.bottomPanel.TabIndex = 2;
			this.bottomPanel.Visible = false;
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(254)))), ((int)(((byte)(192)))));
			this.label1.Location = new System.Drawing.Point(181, 71);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 13);
			this.label1.TabIndex = 3;
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label1.Leave += new System.EventHandler(this.label1_Leave);
			this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label1_MouseMove);
			// 
			// PlayerPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.bottomPanel);
			this.Controls.Add(this.topPanel);
			this.Controls.Add(this.backgroundElementHost);
			this.Name = "PlayerPanel";
			this.Size = new System.Drawing.Size(428, 150);
			this.topPanel.ResumeLayout(false);
			this.bottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		// Плеер
		private ElementHost backgroundElementHost;
		
		// Верхняя панель
		private ElementHost topElementHost;
		private System.Windows.Forms.Panel topPanel;

		// Нижняя панель
		private ElementHost bottomElementHost;
		private System.Windows.Forms.Panel bottomPanel;

		private System.Windows.Forms.Label label1;
	}
}
