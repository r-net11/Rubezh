using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FiresecService.Views
{
	public partial class MainWinFormView : Form, IMainView
	{
		#region Constructors
		public MainWinFormView()
		{
			InitializeComponent();
		}

		#endregion

		#region Fields And Properties

		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}

		public string LastLog
		{
			get { return _toolStripStatusLabelLastLog.Text; }
			set 
			{ 
				_toolStripStatusLabelLastLog.Text = value;
			}
		}

		public void ShowBalloonTip(int timeOut, string title, string text, ToolTipIcon icon)
		{
			_notifyIcon.ShowBalloonTip(timeOut, title, text, icon);
		}

		#endregion

		#region Event handlers for form

		private void EventHandler_MainWinFormView_Load(object sender, EventArgs e)
		{
		}

		private void EventHandler_MainWinFormView_Shown(object sender, EventArgs e)
		{
			Form form = (Form)sender;
			form.ShowInTaskbar = false;
			form.Visible = false;
		}

		private void EventHandler_MainWinFormView_FormClosing(object sender, FormClosingEventArgs e)
		{
			ShowInTaskbar = false;
			Visible = false;
			e.Cancel = true;
		}

		#endregion


		#region Event handlers for context menu

		private void EventHandler_toolStripMenuItemShowForm_Click(object sender, EventArgs e)
		{
			if (Visible == false)
			{
				ShowInTaskbar = true;
				Visible = true;
			}
		}

		private void EventHandler_toolStripMenuItemExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		#endregion
	}
}
