using System;
using System.Linq;
using System.Collections.Generic;
using GKProcessor;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans;
using GKModule.Reports;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using XFiresecAPI;
using Infrastructure.Common.Services;
using FiresecAPI;

namespace GKModule
{
	public partial class GKModuleLoader
	{
		void SubscribeGK()
		{
			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.Suspend();
			}
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DescriptorsManager.Create();
			foreach (var gkDatabase in DescriptorsManager.GkDatabases)
			{
				foreach (var delay in gkDatabase.Delays)
				{
					delay.State = new XState(delay);
				}
			}

			InitializeStates();

			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.Start();
			}

			SafeFiresecService.GKProgressCallbackEvent -= new Action<FiresecAPI.GKProgressCallback>(OnGKProgressCallbackEvent);
			SafeFiresecService.GKProgressCallbackEvent += new Action<FiresecAPI.GKProgressCallback>(OnGKProgressCallbackEvent);

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			GKProcessorManager.GKProgressCallbackEvent -= new Action<FiresecAPI.GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKProgressCallbackEvent += new Action<FiresecAPI.GKProgressCallback>(OnGKProgressCallbackEvent);

			GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);
		}

		void InitializeStates()
		{
			var gkStates = FiresecManager.FiresecService.GKGetStates();
			foreach (var remoteDeviceState in gkStates.DeviceStates)
			{
				var device = XManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					device.State.StateClasses = remoteDeviceState.StateClasses;
					device.State.StateClass = remoteDeviceState.StateClass;
					device.State.OnDelay = remoteDeviceState.OnDelay;
					device.State.HoldDelay = remoteDeviceState.HoldDelay;
					device.State.OffDelay = remoteDeviceState.OffDelay;
					device.State.MeasureParameter = remoteDeviceState.MeasureParameter;
				}
			}
			foreach (var remoteZoneState in gkStates.ZoneStates)
			{
				var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
				if (zone != null)
				{
					zone.State.StateClasses = remoteZoneState.StateClasses;
					zone.State.StateClass = remoteZoneState.StateClass;
					zone.State.OnDelay = remoteZoneState.OnDelay;
					zone.State.HoldDelay = remoteZoneState.HoldDelay;
					zone.State.OffDelay = remoteZoneState.OffDelay;
				}
			}
			foreach (var remoteDirectionState in gkStates.DirectionStates)
			{
				var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
				if (direction != null)
				{
					direction.State.StateClasses = remoteDirectionState.StateClasses;
					direction.State.StateClass = remoteDirectionState.StateClass;
					direction.State.OnDelay = remoteDirectionState.OnDelay;
					direction.State.HoldDelay = remoteDirectionState.HoldDelay;
					direction.State.OffDelay = remoteDirectionState.OffDelay;
				}
			}
		}

		void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			ApplicationService.Invoke(() =>
			{
				switch (gkProgressCallback.GKProgressCallbackType)
				{
					case GKProgressCallbackType.Start:
						LoadingService.Show(gkProgressCallback.Name, gkProgressCallback.Name, gkProgressCallback.Count, gkProgressCallback.CanCancel);
						return;

					case GKProgressCallbackType.Progress:
						LoadingService.DoStep(gkProgressCallback.Name);
						return;

					case GKProgressCallbackType.Stop:
						LoadingService.Close();
						return;
				}
			});
		}

		void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				if (gkCallbackResult.JournalItems.Count > 0)
				{
					ServiceFactory.Events.GetEvent<NewXJournalEvent>().Publish(gkCallbackResult.JournalItems);
				}
				foreach (var remoteDeviceState in gkCallbackResult.DeviceStates)
				{
					var device = XManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
					if (device != null)
					{
						device.State.AdditionalStates = remoteDeviceState.AdditionalStates;
						device.State.StateClasses = remoteDeviceState.StateClasses;
						device.State.StateClass = remoteDeviceState.StateClass;
						device.State.OnDelay = remoteDeviceState.OnDelay;
						device.State.HoldDelay = remoteDeviceState.HoldDelay;
						device.State.OffDelay = remoteDeviceState.OffDelay;
						device.State.MeasureParameter = remoteDeviceState.MeasureParameter;
						device.State.OnStateChanged();
					}
				}
				foreach (var remoteZoneState in gkCallbackResult.ZoneStates)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
					if (zone != null)
					{
						zone.State.StateClasses = remoteZoneState.StateClasses;
						zone.State.StateClass = remoteZoneState.StateClass;
						zone.State.OnDelay = remoteZoneState.OnDelay;
						zone.State.HoldDelay = remoteZoneState.HoldDelay;
						zone.State.OffDelay = remoteZoneState.OffDelay;
						zone.State.OnStateChanged();
					}
				}
				foreach (var remoteDirectionState in gkCallbackResult.DirectionStates)
				{
					var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
					if (direction != null)
					{
						direction.State.StateClasses = remoteDirectionState.StateClasses;
						direction.State.StateClass = remoteDirectionState.StateClass;
						direction.State.OnDelay = remoteDirectionState.OnDelay;
						direction.State.HoldDelay = remoteDirectionState.HoldDelay;
						direction.State.OffDelay = remoteDirectionState.OffDelay;
						direction.State.OnStateChanged();
					}
				}
				foreach (var remotePumpStationState in gkCallbackResult.PumpStationStates)
				{
					var pumpStation = XManager.PumpStations.FirstOrDefault(x => x.UID == remotePumpStationState.UID);
					if (pumpStation != null)
					{
						pumpStation.State.StateClasses = remotePumpStationState.StateClasses;
						pumpStation.State.StateClass = remotePumpStationState.StateClass;
						pumpStation.State.OnDelay = remotePumpStationState.OnDelay;
						pumpStation.State.HoldDelay = remotePumpStationState.HoldDelay;
						pumpStation.State.OffDelay = remotePumpStationState.OffDelay;
						pumpStation.State.OnStateChanged();
					}
				}
				ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
			});
		}
	}
}