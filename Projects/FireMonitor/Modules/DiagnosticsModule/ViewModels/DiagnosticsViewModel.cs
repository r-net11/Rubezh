using System;
using System.Linq;
using System.Threading;
using FiresecAPI.GK;
using FiresecClient;
using GKProcessor;
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
			TurnOnOffRMCommand = new RelayCommand(OnTurnOnOffRM);
			CheckHaspCommand = new RelayCommand(OnCheckHasp);
			TestCommand = new RelayCommand(OnTest);
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

		public RelayCommand TurnOnOffRMCommand { get; private set; }
		void OnTurnOnOffRM()
		{
			var rmDevice = XManager.Devices.FirstOrDefault(x => x.DriverType == XDriverType.RM_1 && x.ShleifNo == 3 && x.IntAddress == 1);
			var flag = false;

			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					if (IsThreadStoping)
						return;
					Thread.Sleep(TimeSpan.FromMilliseconds(3000));
					flag = !flag;

					ApplicationService.Invoke(() =>
					{
						if (flag)
							Watcher.SendControlCommand(rmDevice, XStateBit.TurnOn_InManual, "");
						else
							Watcher.SendControlCommand(rmDevice, XStateBit.TurnOff_InManual, "");
					});
				}
			}));
			thread.Name = "Diagnostics";
			thread.IsBackground = true;
			thread.Start();
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
						var hasLicense = LicenseHelper.CheckLicense(false);
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
			var thread = new Thread(new ThreadStart(() =>
			{
				throw new Exception("TestCommand");
			}));
			thread.Name = "Diagnostics";
			thread.IsBackground = true;
			thread.Start();
		}
	}
}