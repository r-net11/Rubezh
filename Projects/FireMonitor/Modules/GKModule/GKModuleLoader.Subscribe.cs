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
			DescriptorsManager.CreateDynamicObjectsInXManager();
			InitializeStates();
			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.Start();
			}
		}

		void GKAfterInitialize()
		{
			SafeFiresecService.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			SafeFiresecService.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			GKProcessorManager.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);

			GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);
		}

		void InitializeStates()
		{
			var gkStates = FiresecManager.FiresecService.GKGetStates();
			CopyGKStates(gkStates);
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
				CopyGKStates(gkCallbackResult.GKStates);
				ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
			});
		}

		void CopyGKStates(GKStates gkStates)
		{
			foreach (var remoteDeviceState in gkStates.DeviceStates)
			{
				var device = XManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
				if (device != null)
				{
					remoteDeviceState.CopyTo(device.State);
                    device.State.OnStateChanged();
				}
			}
			foreach (var remoteZoneState in gkStates.ZoneStates)
			{
				var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
				if (zone != null)
				{
					remoteZoneState.CopyTo(zone.State);
                    zone.State.OnStateChanged();
				}
			}
			foreach (var remoteDirectionState in gkStates.DirectionStates)
			{
				var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
				if (direction != null)
				{
					remoteDirectionState.CopyTo(direction.State);
                    direction.State.OnStateChanged();
				}
			}
			foreach (var remotePumpStationState in gkStates.PumpStationStates)
			{
				var pumpStation = XManager.PumpStations.FirstOrDefault(x => x.UID == remotePumpStationState.UID);
				if (pumpStation != null)
				{
					remotePumpStationState.CopyTo(pumpStation.State);
                    pumpStation.State.OnStateChanged();
				}
			}
			foreach (var delayState in gkStates.DelayStates)
			{
				var delay = XManager.Delays.FirstOrDefault(x => x.UID == delayState.UID);
				if (delay == null)
					delay = XManager.Delays.FirstOrDefault(x => x.PresentationName == delayState.PresentationName);
				if (delay != null)
				{
					delayState.CopyTo(delay.State);
                    delay.State.OnStateChanged();
				}
			}
			foreach (var remotePimState in gkStates.PimStates)
			{
				var pim = XManager.Pims.FirstOrDefault(x => x.UID == remotePimState.UID);
				if (pim == null)
					pim = XManager.Pims.FirstOrDefault(x => x.PresentationName == remotePimState.PresentationName);
				if (pim != null)
				{
					remotePimState.CopyTo(pim.State);
                    pim.State.OnStateChanged();
				}
			}
		}
	}
}