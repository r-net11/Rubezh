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
					delay.DelayState = new XDelayState();
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
					device.DeviceState.StateClasses = remoteDeviceState.StateClasses;
					device.DeviceState.StateClass = remoteDeviceState.StateClass;
					device.DeviceState.OnDelay = remoteDeviceState.OnDelay;
					device.DeviceState.HoldDelay = remoteDeviceState.HoldDelay;
					device.DeviceState.OffDelay = remoteDeviceState.OffDelay;
				}
			}
			foreach (var remoteZoneState in gkStates.ZoneStates)
			{
				var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
				if (zone != null)
				{
					zone.ZoneState.StateClasses = remoteZoneState.StateClasses;
					zone.ZoneState.StateClass = remoteZoneState.StateClass;
					zone.ZoneState.OnDelay = remoteZoneState.OnDelay;
					zone.ZoneState.HoldDelay = remoteZoneState.HoldDelay;
					zone.ZoneState.OffDelay = remoteZoneState.OffDelay;
				}
			}
			foreach (var remoteDirectionState in gkStates.DirectionStates)
			{
				var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
				if (direction != null)
				{
					direction.DirectionState.StateClasses = remoteDirectionState.StateClasses;
					direction.DirectionState.StateClass = remoteDirectionState.StateClass;
					direction.DirectionState.OnDelay = remoteDirectionState.OnDelay;
					direction.DirectionState.HoldDelay = remoteDirectionState.HoldDelay;
					direction.DirectionState.OffDelay = remoteDirectionState.OffDelay;
				}
			}
		}

		void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			ApplicationService.Invoke(() =>
			{
				switch(gkProgressCallback.GKProgressCallbackType)
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
				if (gkCallbackResult.DeviceStates.Count > 0)
				{
					foreach (var remoteDeviceState in gkCallbackResult.DeviceStates)
					{
						var device = XManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
						if (device != null)
						{
							device.DeviceState.StateClasses = remoteDeviceState.StateClasses;
							device.DeviceState.StateClass = remoteDeviceState.StateClass;
							device.DeviceState.OnDelay = remoteDeviceState.OnDelay;
							device.DeviceState.HoldDelay = remoteDeviceState.HoldDelay;
							device.DeviceState.OffDelay = remoteDeviceState.OffDelay;
							device.DeviceState.OnStateChanged();
						}
					}
				}
				if (gkCallbackResult.ZoneStates.Count > 0)
				{
					foreach (var remoteZoneState in gkCallbackResult.ZoneStates)
					{
						var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
						if (zone != null)
						{
							zone.ZoneState.StateClasses = remoteZoneState.StateClasses;
							zone.ZoneState.StateClass = remoteZoneState.StateClass;
							zone.ZoneState.OnDelay = remoteZoneState.OnDelay;
							zone.ZoneState.HoldDelay = remoteZoneState.HoldDelay;
							zone.ZoneState.OffDelay = remoteZoneState.OffDelay;
							zone.ZoneState.OnStateChanged();
						}
					}
				}
				if (gkCallbackResult.DirectionStates.Count > 0)
				{
					foreach (var remoteDirectionState in gkCallbackResult.DirectionStates)
					{
						var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
						if (direction != null)
						{
							direction.DirectionState.StateClasses = remoteDirectionState.StateClasses;
							direction.DirectionState.StateClass = remoteDirectionState.StateClass;
							direction.DirectionState.OnDelay = remoteDirectionState.OnDelay;
							direction.DirectionState.HoldDelay = remoteDirectionState.HoldDelay;
							direction.DirectionState.OffDelay = remoteDirectionState.OffDelay;
							direction.DirectionState.OnStateChanged();
						}
					}
				}
				ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
			});
		}
	}
}