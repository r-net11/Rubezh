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
	public class Presenter : ApplicationPresenter
	{
		public Presenter(IMainView view)
		{
			View = view;
			Logs = new ObservableCollection<LogViewModel>();
			LastLog = String.Empty;
			View.Title = "Сервер приложений Глобал";
			View.TabChanged += EventHandler_View_TabChanged;
			Current = this;
		}

		#region Fields And Properties

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
			FormDispatcher.BeginInvoke((Action)(() =>
			{
				LastLog = message;
				var logViewModel = new LogViewModel(message, isError);
				Logs.Add(logViewModel);
				if (Logs.Count > 1000)
					Logs.RemoveAt(0);
			}));
		}

		#endregion

		#region Event handlers for View

		void EventHandler_View_TabChanged(object sender, EventArgs e)
		{
			IMainView view = (IMainView)sender;

			if (view.SelectedTabView is ITabPageConnectionsView)
			{ }
			else if (view.SelectedTabView is ITabPageGKView)
			{ }
			else if (view.SelectedTabView is ITabPageLicenceView)
			{ }
			else if (view.SelectedTabView is ITabPageLogView)
			{ }
			else if (view.SelectedTabView is ITabPageOperationsView)
			{ }
			else if (view.SelectedTabView is ITabPagePollingView)
			{ }
			else if (view.SelectedTabView is ITabPageStatusView)
			{ }
		}

		#endregion
	}
}