﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;

namespace AlarmModule
{
	public class Alarm
	{
		public AlarmType AlarmType { get; set; }
		public StateType StateType { get; set; }
		public Guid DeviceUID { get; set; }
        public Guid ZoneUID { get; set; }
		public string StateName { get; set; }
		public bool IsDeleting { get; set; }

		public AlarmEntityType AlarmEntityType
		{
			get
			{
				switch(AlarmType)
				{
					case AlarmType.Fire:
					case AlarmType.Guard:
						return AlarmEntityType.Zone;
				}
				return AlarmEntityType.Device;
			}
		}

		public bool IsConfirmed { get; private set; }

		public void Confirm()
		{
			IsConfirmed = true;

            var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == ZoneUID);

			var journalRecords = new List<JournalRecord>();
			journalRecords.Add(
				new JournalRecord()
					{
						SystemTime = DateTime.Now,
						DeviceTime = DateTime.Now,
						ZoneName = zone.No.ToString(),
						Description = "Состояние \"" + StateName + "\" подтверждено оператором",
						StateType = StateType.Info
					}
				);
			FiresecManager.FiresecService.AddJournalRecords(journalRecords);
		}

		public bool CanRemoveFromIgnoreList()
		{
			return ((StateType == StateType.Off) &&
			(FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_RemoveFromIgnoreList)));
		}

		public void RemoveFromIgnoreList()
		{
			var deviceState = FiresecManager.Devices.FirstOrDefault(x => x.UID == DeviceUID).DeviceState;
			if (FiresecManager.CanDisable(deviceState) && deviceState.IsDisabled)
				FiresecManager.FiresecDriver.RemoveFromIgnoreList(new List<Device>() { deviceState.Device });
		}

		public bool CanReset()
		{
			return GetResetItem() != null;
		}

		public void Reset()
		{
			var resetItems = new List<ResetItem>();
			resetItems.Add(GetResetItem());
            FiresecManager.FiresecDriver.ResetStates(resetItems);
		}

		public ResetItem GetResetItem()
		{
			switch (AlarmEntityType)
			{
				case AlarmEntityType.Device:
					return GetDeviceResetItem();

				case AlarmEntityType.Zone:
					return GetZoneResetItem();

				default:
					return null;
			}
		}

		ResetItem GetZoneResetItem()
		{
			foreach (var device in FiresecManager.Devices)
			{
                if (device.ZoneUID == ZoneUID)
				{
					if (device.DeviceState.States.Any(x => x.DriverState.StateType == StateType.Fire))
					{
						DeviceUID = device.UID;
						return GetDeviceResetItem();
					}
				}
			}

			return null;
		}

		ResetItem GetDeviceResetItem()
		{
			var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == DeviceUID);
			DeviceState parentDeviceState;
			if (device.ParentPanel != null)
				parentDeviceState = FiresecManager.Devices.FirstOrDefault(x => x.UID == device.ParentPanel.UID).DeviceState;
			else
				parentDeviceState = FiresecManager.Devices.FirstOrDefault(x => x.UID == device.Parent.UID).DeviceState;

			var resetItem = new ResetItem();

			switch (AlarmType)
			{
				case AlarmType.Fire:
				case AlarmType.Attention:
				case AlarmType.Info:
				case AlarmType.Failure:
					resetItem.DeviceState = parentDeviceState;
					foreach (var state in parentDeviceState.States)
					{
						if (state.DriverState.StateType == EnumsConverter.AlarmTypeToStateType(AlarmType) && state.DriverState.IsManualReset)
							resetItem.States.Add(state);
					}
					break;

				case AlarmType.Service:
					resetItem.DeviceState = device.DeviceState;
					var deviceState = FiresecManager.Devices.FirstOrDefault(x => x.UID == device.UID).DeviceState;
					foreach (var state in deviceState.States)
					{
						if (state.DriverState.IsManualReset)
							resetItem.States.Add(state);
					}
					break;

				case AlarmType.Auto:
				case AlarmType.Off:
					break;
			}

			if (resetItem.States.Count > 0)
				return resetItem;
			return null;
		}
	}
}