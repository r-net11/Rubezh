using GKProcessor;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System;
using System.Linq;

namespace GKModule
{
	public partial class GKModuleLoader
	{
		void SubscribeGK()
		{
			GKManager.UpdateConfiguration();
			GKManager.CreateStates();
			DescriptorsManager.Create();
			InitializeStates();
		}

		void GKAfterInitialize()
		{
			SafeFiresecService.GKProgressCallbackEvent -= new Action<GKProgressCallback>(OnGKProgressCallbackEvent);
			SafeFiresecService.GKProgressCallbackEvent += new Action<GKProgressCallback>(OnGKProgressCallbackEvent);

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			SafeFiresecService.GKPropertyChangedEvent -= new Action<GKPropertyChangedCallback>(OnGKPropertyChanged);
			SafeFiresecService.GKPropertyChangedEvent += new Action<GKPropertyChangedCallback>(OnGKPropertyChanged);

			GKProcessorManager.GKProgressCallbackEvent -= new Action<GKProgressCallback, Guid?>(OnGKProgressCallbackEvent);
			GKProcessorManager.GKProgressCallbackEvent += new Action<GKProgressCallback, Guid?>(OnGKProgressCallbackEvent);

			GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
		}

		void InitializeStates()
		{
			var gkStates = ClientManager.FiresecService.GKGetStates();
			CopyGKStates(gkStates);
		}

		void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			OnGKProgressCallbackEvent(gkProgressCallback, null);
		}
		void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback, Guid? callbackUID)
		{
			ApplicationService.Invoke(() =>
			{
				switch (gkProgressCallback.GKProgressCallbackType)
				{
					case GKProgressCallbackType.Start:
						if (gkProgressCallback.GKProgressClientType == GKProgressClientType.Monitor)
						{
							LoadingService.Show(gkProgressCallback.Title, gkProgressCallback.Text, gkProgressCallback.StepCount, gkProgressCallback.CanCancel);
						}
						return;

					case GKProgressCallbackType.Progress:
						if (gkProgressCallback.GKProgressClientType == GKProgressClientType.Monitor)
						{
							LoadingService.DoStep(gkProgressCallback.Text, gkProgressCallback.Title, gkProgressCallback.StepCount, gkProgressCallback.CurrentStep, gkProgressCallback.CanCancel);
							if (LoadingService.IsCanceled)
								ClientManager.FiresecService.CancelGKProgress(gkProgressCallback.UID, ClientManager.CurrentUser.Name);
						}
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
				CopyGKStates(gkCallbackResult.GKStates);
				ServiceFactoryBase.Events.GetEvent<GKObjectsStateChangedEvent>().Publish(null);
			});
		}

		void OnGKPropertyChanged(GKPropertyChangedCallback gkPropertyChangedCallback)
		{
			if (gkPropertyChangedCallback != null)
			{
				ApplicationService.Invoke(() =>
				{
					var device = GKManager.Devices.FirstOrDefault(x => x.UID == gkPropertyChangedCallback.ObjectUID);
					if (device != null)
					{
						device.Properties = gkPropertyChangedCallback.DeviceProperties;
						ServiceFactoryBase.Events.GetEvent<GKObjectsPropertyChangedEvent>().Publish(null);
					}
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
			foreach (var remoteMPTState in gkStates.MPTStates)
			{
				var mpt = GKManager.MPTs.FirstOrDefault(x => x.UID == remoteMPTState.UID);
				if (mpt != null)
				{
					remoteMPTState.CopyTo(mpt.State);
					mpt.State.OnStateChanged();
				}
			}
			foreach (var delayState in gkStates.DelayStates)
			{
				var delay = GKManager.Delays.FirstOrDefault(x => x.UID == delayState.UID);
				if (delay != null)
				{
					delayState.CopyTo(delay.State);
					delay.State.OnStateChanged();
				}
				else
				{
					delay = GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.UID == delayState.UID);
					if (delay == null)
						delay = GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.PresentationName == delayState.PresentationName);
					if (delay != null)
					{
						delayState.CopyTo(delay.State);
						delay.State.OnStateChanged();
					}
				}
			}
			foreach (var remotePimState in gkStates.PimStates)
			{
				var pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == remotePimState.UID);
				if (pim == null)
					pim = GKManager.AutoGeneratedPims.FirstOrDefault(x => x.PresentationName == remotePimState.PresentationName);
				if (pim == null)
					pim = GKManager.GlobalPims.FirstOrDefault(x => x.DeviceUid == remotePimState.ReferenceUid);
				if (pim != null)
				{
					remotePimState.CopyTo(pim.State);
					pim.State.OnStateChanged();
				}
			}
			foreach (var remoteGuardZoneState in gkStates.GuardZoneStates)
			{
				var guardZone = GKManager.GuardZones.FirstOrDefault(x => x.UID == remoteGuardZoneState.UID);
				if (guardZone != null)
				{
					remoteGuardZoneState.CopyTo(guardZone.State);
					guardZone.State.OnStateChanged();
				}
			}
			foreach (var remoteDoorState in gkStates.DoorStates)
			{
				var door = GKManager.Doors.FirstOrDefault(x => x.UID == remoteDoorState.UID);
				if (door != null)
				{
					remoteDoorState.CopyTo(door.State);
					door.State.OnStateChanged();
				}
			}
			foreach (var remoteSKDZoneState in gkStates.SKDZoneStates)
			{
				var skdZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == remoteSKDZoneState.UID);
				if (skdZone != null)
				{
					remoteSKDZoneState.CopyTo(skdZone.State);
					skdZone.State.OnStateChanged();
				}
			}
			foreach (var deviceMeasureParameter in gkStates.DeviceMeasureParameters)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceMeasureParameter.DeviceUID);
				if (device != null)
				{
					device.State.XMeasureParameterValues = deviceMeasureParameter.MeasureParameterValues;
					device.State.OnMeasureParametersChanged();
				}
			}
		}
	}
}