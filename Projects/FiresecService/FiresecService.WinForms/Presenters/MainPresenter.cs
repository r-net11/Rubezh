using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecService.Views;
using FiresecService.ViewModels;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace FiresecService.Presenters
{
	public class MainPresenter
	{
		public MainPresenter(IMainView view)
		{
			View = view;
			_dispatcher = Dispatcher.CurrentDispatcher;
			Logs = new ObservableCollection<LogViewModel>();
			LastLog = String.Empty;

			Current = this;
		}

		#region Fields And Properties

		public static MainPresenter Current { get; set; }

		Dispatcher _dispatcher;

		public IMainView View { get; private set; }

		public ObservableCollection<LogViewModel> Logs { get; private set; }

		public string LastLog
		{
			get { return View.LastLog; }
			set { View.LastLog = value; }
		}

		#endregion

		#region Methods
		
		public void AddLog(string message, bool isError)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				LastLog = message;
				var logViewModel = new LogViewModel(message, isError);
				Logs.Add(logViewModel);
				if (Logs.Count > 1000)
					Logs.RemoveAt(0);
			}));
		}


		#endregion
	}
}