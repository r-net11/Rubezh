using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;

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

		#endregion

		#region Event handlers for form

		private void EventHandler_MainWinFormView_Load(object sender, EventArgs e)
		{
		}

		private void EventHandler_MainWinFormView_Shown(object sender, EventArgs e)
		{
		}

		private void EventHandler_MainWinFormView_FormClosing(object sender, FormClosingEventArgs e)
		{
			ShowInTaskbar = false;
			Visible = false;
			e.Cancel = true;
		}

		private void MainWinFormView_Activated(object sender, EventArgs e)
		{
		}

		#endregion

		#region Methods

		protected override void SetVisibleCore(bool value)
		{
			// этот код необходим что бы скрыть форму при запуске приложения
			// http://stackoverflow.com/questions/4556411/how-to-hide-a-window-in-start-in-c-sharp-desktop-application
			if (!IsHandleCreated && value)
			{
				value = false;
				CreateHandle();
			}
			base.SetVisibleCore(value);
		}

		#endregion
	}
}
