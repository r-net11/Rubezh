using System;
using System.Linq;
using System.Threading;
using Common;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKOPCServer.ViewModels;
using GKProcessor;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Windows;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Windows.BalloonTrayTip;
using RubezhAPI.License;
using RubezhAPI;

namespace GKOPCServer
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			try
			{
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml");

				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.Name = "GK OPC Main Window";
				WindowThread.Priority = ThreadPriority.Highest;
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
				MainViewStartedEvent.WaitOne();

				UILogger.Log("Соединение с сервером");
				for (int i = 1; i <= 10; i++)
				{
					var message = ClientManager.Connect(ClientType.OPC, ConnectionSettingsManager.ServerAddress, GlobalSettingsHelper.GlobalSettings.AdminLogin, GlobalSettingsHelper.GlobalSettings.AdminPassword);
					if (message == null)
						break;
					Thread.Sleep(5000);
					if (i == 10)
					{
						UILogger.Log("Ошибка соединения с сервером: " + message);
						return;
					}
				}

				if (InitializeGK())
					GKOPCManager.Start();
				UILogger.Log("Готово");
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске");
				Close();
			}
		}

		static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel, false, false);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
			}
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			ClientManager.Disconnect();
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}

			System.Environment.Exit(1);
		}

		static bool InitializeGK()
		{
			bool result = true;
			ServiceFactoryBase.Events = new EventAggregator();

			UILogger.Log("Загрузка лицензии");
			ClientManager.GetLicense();
			if (!LicenseManager.CurrentLicenseInfo.HasOpcServer)
			{
				BalloonHelper.ShowFromServer("Отсутствует лицензия модуля \"GLOBAL OPC Сервер\"");
				result = false;
			}

			UILogger.Log("Загрузка конфигурации с сервера");
			ClientManager.GetConfiguration("GKOPC/Configuration");

			UILogger.Log("Создание драйверов");
			GKDriversCreator.Create();

			UILogger.Log("Обновление конфигурации");
			GKManager.UpdateConfiguration();
			GKManager.CreateStates();
			DescriptorsManager.Create();
			UILogger.Log("Получение состояний объектов");
			InitializeStates();

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			ClientManager.StartPoll();
			return result;
		}

		static void InitializeStates()
		{
			var gkStates = ClientManager.FiresecService.GKGetStates();
			CopyGKStates(gkStates);
		}

		static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				CopyGKStates(gkCallbackResult.GKStates);
			});
		}

		static void CopyGKStates(GKStates gkStates)
		{
			foreach (var remoteDeviceState in gkStates.DeviceStates)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					remoteDeviceState.CopyTo(device.State);
					device.State.OnStateChanged();
				}
			}
			foreach (var remoteZoneState in gkStates.ZoneStates)
			{
				var zone = GKManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
				if (zone != null)
				{
					remoteZoneState.CopyTo(zone.State);
					zone.State.OnStateChanged();
				}
			}
			foreach (var remoteDirectionState in gkStates.DirectionStates)
			{
				var direction = GKManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
				if (direction != null)
				{
					remoteDirectionState.CopyTo(direction.State);
					direction.State.OnStateChanged();
				}
			}
			foreach (var remotePumpStationState in gkStates.PumpStationStates)
			{
				var pumpStation = GKManager.PumpStations.FirstOrDefault(x => x.UID == remotePumpStationState.UID);
				if (pumpStation != null)
				{
					remotePumpStationState.CopyTo(pumpStation.State);
					pumpStation.State.OnStateChanged();
				}
			}
			foreach (var delayState in gkStates.DelayStates)
			{
				var delay = GKManager.Delays.FirstOrDefault(x => x.UID == delayState.UID);
				if (delay == null)
					delay = GKManager.Delays.FirstOrDefault(x => x.PresentationName == delayState.PresentationName);
				if (delay != null)
				{
					delayState.CopyTo(delay.State);
					delay.State.OnStateChanged();

				}
			}
			foreach (var remotePimState in gkStates.PimStates)
			{
				var pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == remotePimState.UID);
				if (pim == null)
					pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.PresentationName == remotePimState.PresentationName);
				if (pim != null)
				{
					remotePimState.CopyTo(pim.State);
					pim.State.OnStateChanged();
				}
			}
			foreach (var mptState in gkStates.MPTStates)
			{
				var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == mptState.UID);
				if (mpt != null)
				{
					mptState.CopyTo(mpt.State);
					mpt.State.OnStateChanged();
				}
			}
			foreach (var guardZoneState in gkStates.GuardZoneStates)
			{
				var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == guardZoneState.UID);
				if (guardZone != null)
				{
					guardZoneState.CopyTo(guardZone.State);
					guardZone.State.OnStateChanged();
				}
			}
			foreach (var doorState in gkStates.DoorStates)
			{
				var door = GKManager.Doors.FirstOrDefault(x => x.UID == doorState.UID);
				if (door != null)
				{
					doorState.CopyTo(door.State);
					door.State.OnStateChanged();
				}
			}
		}
	}
}