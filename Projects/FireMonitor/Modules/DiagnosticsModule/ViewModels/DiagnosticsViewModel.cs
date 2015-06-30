using System;
using System.Threading;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	[Serializable]
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			CheckHaspCommand = new RelayCommand(OnCheckHasp);
			TestCommand = new RelayCommand(OnTest);
			SKDDataCommand = new RelayCommand(OnSKDData);
			SKDDataAscCommand = new RelayCommand(OnSKDDataAsc);
			GenerateEmployeeDaysCommand = new RelayCommand(OnGenerateEmployeeDays);
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
				OnPropertyChanged(() => Text);
			}
		}

		public RelayCommand CheckHaspCommand { get; private set; }
		void OnCheckHasp()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					ApplicationService.Invoke(() =>
					{
					});
					Thread.Sleep(TimeSpan.FromMilliseconds(3000));
				}
			}));
			thread.Name = "Diagnostics";
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			FiresecManager.FiresecService.Test("");
			return;

			var thread = new Thread(new ThreadStart(() =>
			{
				throw new Exception("TestCommand");
			}));
			thread.Name = "Diagnostics";
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand SKDDataCommand { get; private set; }
		void OnSKDData()
		{
			FiresecManager.FiresecService.GenerateTestData(false);
		}
		
		public RelayCommand SKDDataAscCommand { get; private set; }
		void OnSKDDataAsc()
		{
			FiresecManager.FiresecService.GenerateTestData(true);
		}

		public RelayCommand GenerateEmployeeDaysCommand { get; private set; }
		void OnGenerateEmployeeDays()
		{
			FiresecManager.FiresecService.GenerateEmployeeDays();
		}
	}
}