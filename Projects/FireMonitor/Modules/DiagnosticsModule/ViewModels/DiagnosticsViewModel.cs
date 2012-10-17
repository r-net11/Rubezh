using Firesec.Imitator;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System;
using FiresecAPI;
using System.Diagnostics;
using System.Text;
using FiresecAPI.Models;
using System.Threading;
using System.Collections.Generic;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			Test1Command = new RelayCommand(OnTest1);
			Test2Command = new RelayCommand(OnTest2);
			Test3Command = new RelayCommand(OnTest3);
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

		public RelayCommand Test1Command { get; private set; }
		void OnTest1()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				var random = new Random(1);
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(100));
					var zoneIndex = random.Next(0, FiresecManager.Zones.Count - 1);
					var zone = FiresecManager.Zones[zoneIndex];
					var devices = new List<Device>();
					foreach (var device in zone.DevicesInZone)
					{
						if (device.Driver.CanDisable)
						{
							devices.Add(device);
						}
					}
					FiresecManager.FiresecDriver.RemoveFromIgnoreList(devices);
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test2Command { get; private set; }
		void OnTest2()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				var random = new Random(1);
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
					var zoneIndex = random.Next(0, FiresecManager.Zones.Count - 1);
					var zone = FiresecManager.Zones[zoneIndex];
					var devices = new List<Device>();
					foreach (var device in zone.DevicesInZone)
					{
						if (device.Driver.CanDisable)
						{
							devices.Add(device);
						}
					}
					FiresecManager.FiresecDriver.AddToIgnoreList(devices);
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test3Command { get; private set; }
		void OnTest3()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(100));
					FiresecManager.ResetAllStates();
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}
	}
}