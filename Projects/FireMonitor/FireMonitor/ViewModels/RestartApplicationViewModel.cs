using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Windows;
using Infrastructure.Common.Windows;
using System.Windows.Threading;

namespace FireMonitor.ViewModels
{
	public class RestartApplicationViewModel : SaveCancelDialogViewModel
	{
		public RestartApplicationViewModel()
		{
			Title = "Firesec";
			TopMost = true;
			AllowClose = false;
			Sizable = false;
			SaveCaption = "Да";
			CancelCaption = "Нет";
			Total = 60;
			var timer = new DispatcherTimer();
			timer.Tick += (s, e) => Counter++;
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Start();
#if DEBUG
			Total = 10;
#endif
		}

		private int _counter;
		public int Counter
		{
			get { return _counter; }
			set
			{
				_counter = value;
				OnPropertyChanged(() => Counter);
				if (Counter == Total)
					SaveCommand.Execute();
			}
		}
		public int Total { get; private set; }
	}
}
