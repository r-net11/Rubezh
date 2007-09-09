using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Events;

namespace DiagnosticsModule.ViewModels
{
	[Serializable]
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			Test1Command = new RelayCommand(OnTest1);
		}

		public void StopThreads()
		{
			IsThreadStoping = true;
		}

		bool IsThreadStoping = false;

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public RelayCommand Test1Command { get; private set; }
		void OnTest1()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}
	}
}