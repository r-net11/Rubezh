using System;
using System.Linq;
using System.Text;
using System.Windows;
using DiagnosticsModule.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Firesec.Imitator;
using Firesec;
using System.Threading;
using System.Diagnostics;
using Common.GK;
using DevicesModule.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			ShowDriversCommand = new RelayCommand(OnShowDrivers);
			ShowXDriversCommand = new RelayCommand(OnShowXDrivers);
			ShowTreeCommand = new RelayCommand(OnShowTree);
			JournalTestCommand = new RelayCommand(OnJournalTest);
			Test1Command = new RelayCommand(OnTest1);
			Test2Command = new RelayCommand(OnTest2);
			Test3Command = new RelayCommand(OnTest3);
			Test4Command = new RelayCommand(OnTest4);
		}

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

		public RelayCommand ShowDriversCommand { get; private set; }
		void OnShowDrivers()
		{
			var driversView = new DriversView();
			driversView.ShowDialog();
		}

		public RelayCommand ShowXDriversCommand { get; private set; }
		void OnShowXDrivers()
		{
			var driversView = new XDriversView();
			driversView.ShowDialog();
		}

		public RelayCommand JournalTestCommand { get; private set; }
		void OnJournalTest()
		{
			var JournalTestViewModel = new JournalTestViewModel();
			DialogService.ShowModalWindow(JournalTestViewModel);
		}

		public RelayCommand ShowTreeCommand { get; private set; }
		void OnShowTree()
		{
			var devicesTreeViewModel = new DevicesTreeViewModel();
			DialogService.ShowModalWindow(devicesTreeViewModel);
		}

		public RelayCommand Test1Command { get; private set; }
		void OnTest1()
		{
			while (true)
			{
				WriteAllDeviceConfigurationHelper.Run(false);
				Trace.WriteLine("WriteAllDeviceConfigurationHelper Done");
			}
		}

		public RelayCommand Test2Command { get; private set; }
		void OnTest2()
		{
		}

		public RelayCommand Test3Command { get; private set; }
		void OnTest3()
		{
		}

		public RelayCommand Test4Command { get; private set; }
		void OnTest4()
		{
		}
	}
}