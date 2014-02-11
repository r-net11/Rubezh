using System;
using Common;
using DiagnosticsModule.Views;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace DiagnosticsModule.ViewModels
{
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
		private void OnTest1()
		{
			FiresecManager.FiresecService.Test("");
		}
	}
}