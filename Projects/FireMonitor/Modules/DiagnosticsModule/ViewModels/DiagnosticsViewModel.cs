using System;
using System.Linq;
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
using XFiresecAPI;
using GKProcessor;

namespace DiagnosticsModule.ViewModels
{
	[Serializable]
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			TurnOnOffRMCommand = new RelayCommand(OnTurnOnOffRM);
			CheckHaspCommand = new RelayCommand(OnCheckHasp);
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
							Watcher.SendControlCommand(rmDevice, XStateBit.TurnOn_InManual);
						else
							Watcher.SendControlCommand(rmDevice, XStateBit.TurnOff_InManual);
					});
				}
			}));
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
						Trace.WriteLine("CheckLicense " + hasLicense);
					});
					Thread.Sleep(TimeSpan.FromMilliseconds(3000));
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}
	}
}