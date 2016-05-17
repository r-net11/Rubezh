using System;
using System.Threading;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;

namespace DiagnosticsModule.ViewModels
{
	[Serializable]
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			TestCommand = new RelayCommand(OnTest);
			SKDDataCommand = new RelayCommand(OnSKDData);
			SKDDataAscCommand = new RelayCommand(OnSKDDataAsc);
			GenerateEmployeeDaysCommand = new RelayCommand(OnGenerateEmployeeDays);
			JournalCommand = new RelayCommand(OnJournal);
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

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			ClientManager.RubezhService.Test("");
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
			ClientManager.RubezhService.GenerateTestData(false);
		}

        public RelayCommand JournalCommand { get; private set; }
        void OnJournal()
        {
			ClientManager.RubezhService.GenerateJournal();
        }
		
		public RelayCommand SKDDataAscCommand { get; private set; }
		void OnSKDDataAsc()
		{
			ClientManager.RubezhService.GenerateTestData(true);
		}

		public RelayCommand GenerateEmployeeDaysCommand { get; private set; }
		void OnGenerateEmployeeDays()
		{
			ClientManager.RubezhService.GenerateEmployeeDays();
		}
	}
}