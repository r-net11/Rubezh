using System;
using System.Linq;
using System.Threading;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKProcessor;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.Journal;
using System.Collections.Generic;

namespace GKSDK
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ZonesViewModel ZonesViewModel { get; private set; }
		public DirectionsViewModel DirectionsViewModel { get; private set; }
		public SKDViewModel SKDViewModel { get; private set; }
		public JournalsViewModel JournalsViewModel { get; private set; }

		public MainViewModel()
		{
			InitializeGK();

			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			DirectionsViewModel = new DirectionsViewModel();
			SKDViewModel = new SKDViewModel();
			JournalsViewModel = new JournalsViewModel();
		}

		void InitializeGK()
		{
			for (int i = 1; i <= 10; i++)
			{
				var message = ClientManager.Connect(ClientType.Other, ConnectionSettingsManager.ServerAddress, GlobalSettingsHelper.GlobalSettings.AdminLogin, GlobalSettingsHelper.GlobalSettings.AdminPassword);
				if (message == null)
					break;
				Thread.Sleep(5000);
				if (i == 10)
				{
					MessageBoxService.ShowError("Ошибка соединения с сервером: " + message);
					return;
				}
			}

			ServiceFactoryBase.Events = new EventAggregator();

			ClientManager.GetConfiguration("GKSDK/Configuration");
			GKDriversCreator.Create();
            GKManager.UpdateConfiguration();
            GKManager.CreateStates();
			DescriptorsManager.Create();
			InitializeStates();

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			SafeFiresecService.JournalItemsEvent -= OnNewJournalItems;
			SafeFiresecService.JournalItemsEvent += OnNewJournalItems;

			ClientManager.StartPoll();
		}

		void InitializeStates()
		{
			var gkStates = ClientManager.FiresecService.GKGetStates();
			CopyGKStates(gkStates);
		}

		void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				CopyGKStates(gkCallbackResult.GKStates);
			});
		}

		public void OnNewJournalItems(List<JournalItem> journalItems, bool isNew)
		{
			if (isNew)
			{
				ApplicationService.Invoke(() =>
				{
					JournalsViewModel.OnNewJournalItems(journalItems);
				});
			}
		}

		void CopyGKStates(GKStates gkStates)
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
		}
	}
}