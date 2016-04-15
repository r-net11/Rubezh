using System;
using System.Windows.Threading;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.ViewModels
{
	public class RestartApplicationViewModel : SaveCancelDialogViewModel
	{
		public RestartApplicationViewModel()
		{
			Title = "Страж";
			TopMost = true;
			AllowClose = false;
			Sizable = false;
			SaveCaption = "Перезапуск";
			CancelCaption = "Отложить перезапуск";
			Total = 60;
			var timer = new DispatcherTimer();
			timer.Tick += (s, e) => Counter++;
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Start();
		}

		int _counter;
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