using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System;
using System.Diagnostics;
using FiresecAPI.Models;
using System.Threading;
using System.Collections.Generic;
using Firesec;
using Infrastructure;
using Infrastructure.Events;
using System.Windows;
using Infrastructure.Common.BalloonTrayTip;


namespace DiagnosticsModule.ViewModels
{
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
            WarningTestCommand = new RelayCommand(OnWarningTest);
            NotificationTestCommand = new RelayCommand(OnNotificationTest);
            ConflagrationTestCommand = new RelayCommand(OnConflagrationTest);
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
					if (NativeFiresecClient.TasksCount > 10)
						continue;
					Thread.Sleep(TimeSpan.FromSeconds(1));

					var index = random.Next(0, 10);
					switch(index)
					{
						case 0:
							RemoveFromIgnoreList();
							break;

						case 1:
							AddToIgnoreList();
							break;

						case 2:
							FiresecManager.ResetAllStates();;
							break;

						case 3:
							ControlDevice();
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
					if (NativeFiresecClient.TasksCount > 10)
						continue;
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
					if (NativeFiresecClient.TasksCount > 10)
						continue;
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
                    if (NativeFiresecClient.TasksCount > 10)
                        continue;
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
            throw new Exception("Unknown exception");
        }

        public RelayCommand Test5Command { get; private set; }
        void OnTest5()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (NativeFiresecClient.TasksCount > 10)
                        continue;
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
                    if (NativeFiresecClient.TasksCount > 10)
                        continue;
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
					ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(null);
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
					ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
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
                    if (NativeFiresecClient.TasksCount > 10)
                        continue;
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

        public RelayCommand WarningTestCommand { get; private set; }
        void OnWarningTest()
        {
            BalloonHelper.ShowWarning("Предупреждение", "Это текст предупреждения. Он предупреждает.");
        }
        public RelayCommand NotificationTestCommand { get; private set; }
        void OnNotificationTest()
        {
            BalloonHelper.ShowNotification("Уведомление", "Уведомления текст это. Уведомляет он.");
        }
        public RelayCommand ConflagrationTestCommand { get; private set; }
        void OnConflagrationTest()
        {
            BalloonHelper.ShowConflagration("ПОЖАР", "АААААААААААААААААААААААААААААААА!!!!!!!!!!!");
        }
    }

    internal class DeviceControl
    {
        public Device Device { get; set; }
        public DriverProperty DriverProperty { get; set; }
    }
}