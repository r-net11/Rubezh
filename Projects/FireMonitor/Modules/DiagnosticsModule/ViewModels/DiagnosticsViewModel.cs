using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System;
using System.Diagnostics;
using FiresecAPI.Models;
using System.Threading;
using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Events;
using System.Windows;
using Infrastructure.Common.BalloonTrayTip;
using Firesec;
using FiresecAPI;
using Infrustructure.Plans.Events;
using Infrastructure.Common.Windows;



namespace DiagnosticsModule.ViewModels
{
	[Serializable]
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			Test1Command = new RelayCommand(OnTest1);
			Test2Command = new RelayCommand(OnTest2);
			Test3Command = new RelayCommand(OnTest3);
			Test4Command = new RelayCommand(OnTest4);
			Test5Command = new RelayCommand(OnTest5);
			Test6Command = new RelayCommand(OnTest6);
			Test7Command = new RelayCommand(OnTest7);
			Test8Command = new RelayCommand(OnTest8);
			Test9Command = new RelayCommand(OnTest9);
			Test10Command = new RelayCommand(OnTest10);
			Test11Command = new RelayCommand(OnTest11);
			TestBalloonCommand = new RelayCommand(OnTestBalloon);
			ServiceFactory.Events.GetEvent<WarningItemEvent>().Subscribe(OnWarningTest);
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

		public RelayCommand Test8Command { get; private set; }
		void OnTest8()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					if (IsThreadStoping)
						break;
					Thread.Sleep(TimeSpan.FromSeconds(1));

					var index = random.Next(0, 10);
					switch (index)
					{
						case 0:
							RemoveFromIgnoreList();
							break;

						case 1:
							AddToIgnoreList();
							break;

						case 2:
							FiresecManager.ResetAllStates();
							;
							break;

						case 3:
							//ControlDevice();
							break;

						case 4:
							ChangeGuardZone();
							break;

						case 5:
							ChangeGuardZone();
							break;

						case 6:
							Application.Current.Dispatcher.Invoke(new Action(Navigate));
							break;
					}
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test1Command { get; private set; }
		void OnTest1()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
					RemoveFromIgnoreList();
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
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
					AddToIgnoreList();
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		Random random = new Random(1);

		public RelayCommand Test3Command { get; private set; }
		void OnTest3()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
					FiresecManager.ResetAllStates();
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test4Command { get; private set; }
		void OnTest4()
		{
            MessageBoxService.Show("ApplicationService.ApplicationWindow.Left = " + ApplicationService.ApplicationWindow.Left.ToString());
			//throw new Exception("Unknown exception");
		}

		public RelayCommand Test5Command { get; private set; }
		void OnTest5()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
					ControlDevice();
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		bool IsGuardZoneInverse;

		public RelayCommand Test6Command { get; private set; }
		void OnTest6()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
					ChangeGuardZone();
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test7Command { get; private set; }
		void OnTest7()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(5000));

					Application.Current.Dispatcher.Invoke(new Action(Navigate));
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		void RemoveFromIgnoreList()
		{
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

		void AddToIgnoreList()
		{
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

		void ControlDevice()
		{
			var deviceControls = new List<DeviceControl>();
			foreach (var device in FiresecManager.Devices)
			{
				foreach (var property in device.Driver.Properties)
				{
					if (property.IsControl)
					{
						var deviceControl = new DeviceControl()
						{
							Device = device,
							DriverProperty = property
						};
						deviceControls.Add(deviceControl);
					}
				}
			}

			var deviceControlIndex = random.Next(0, deviceControls.Count - 1);
			var randomDeviceControl = deviceControls[deviceControlIndex];
			FiresecManager.FiresecDriver.ExecuteCommand(randomDeviceControl.Device, randomDeviceControl.DriverProperty.Name);
		}

		void ChangeGuardZone()
		{
			var guardZones = new List<Zone>();
			foreach (var zone in FiresecManager.Zones)
			{
				if (zone.ZoneType == ZoneType.Guard)
				{
					guardZones.Add(zone);
				}
			}

			var zoneIndex = random.Next(0, guardZones.Count - 1);
			var guardZone = guardZones[zoneIndex];

			IsGuardZoneInverse = !IsGuardZoneInverse;
			if (IsGuardZoneInverse)
				FiresecManager.SetZoneGuard(guardZone);
			else
				FiresecManager.UnSetZoneGuard(guardZone);
		}

		void Navigate()
		{
			var eventIndex = random.Next(0, 10);
			switch (eventIndex)
			{
				case 0:
					ServiceFactory.Events.GetEvent<ShowAlarmsEvent>().Publish(null);
					break;
				case 1:
					//ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(null);
					break;
				case 2:
					ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
					break;
				case 3:
					ServiceFactory.Events.GetEvent<ShowDiagnosticsEvent>().Publish(null);
					break;
				case 4:
					ServiceFactory.Events.GetEvent<ShowGKEvent>().Publish(null);
					break;
				case 5:
					ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
					break;
				case 6:
					ServiceFactory.Events.GetEvent<ShowNothingEvent>().Publish(null);
					break;
				case 7:
					if (FiresecManager.PlansConfiguration.Plans.Count > 0)
					{
						ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
					}
					break;
				case 8:
					ServiceFactory.Events.GetEvent<ShowReportsEvent>().Publish(null);
					break;
				case 9:
					ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Guid.Empty);
					break;
			}
		}

		public RelayCommand Test9Command { get; private set; }
		void OnTest9()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				int count = 0;
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(10));
					FiresecManager.FiresecDriver.AddUserMessage("Test Message " + count++.ToString());
					if (count % 1000 == 0)
					{
						Trace.WriteLine("Count = " + count.ToString());
					}
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test10Command { get; private set; }
		void OnTest10()
		{
		}

		public RelayCommand Test11Command { get; private set; }
		void OnTest11()
		{
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementDevices)
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementRectangleZones)
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementPolygonZones)
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementRectangles)
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementEllipses)
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementTextBlocks)
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementPolygons)
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				foreach (var element in plan.ElementPolylines)
					ServiceFactory.Events.GetEvent<NavigateToPlanElementEvent>().Publish(new NavigateToPlanElementEventArgs(plan.UID, element.UID));
		}

		void OnWarningTest(object obj)
		{
			Random rnd = new Random();
            BalloonHelper.Show("Предупреждение", rnd.Next(100).ToString());
		}
		
        public RelayCommand TestBalloonCommand { get; private set; }
		void OnTestBalloon()
		{
			Infrastructure.Common.BalloonTrayTip.BalloonHelper.Show("Hi, there", "Hi, there");
			var thread = new Thread(OnTestBalloonRun);
			thread.Start();
		}

		void OnTestBalloonRun()
		{
			System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			{
				Infrastructure.Common.BalloonTrayTip.BalloonHelper.Show("Hello", "Hello");
			}));
		}
	}

	internal class DeviceControl
	{
		public Device Device { get; set; }
		public DriverProperty DriverProperty { get; set; }
	}
}