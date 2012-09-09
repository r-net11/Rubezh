using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using FiresecAPI;

namespace AlarmModule
{
	public class Alarm
	{
		public AlarmType AlarmType { get; set; }
		public StateType StateType { get; set; }
		public Guid DeviceUID { get; set; }
		public int? ZoneNo { get; set; }
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

			var zone = FiresecManager.Zones.FirstOrDefault(x => x.No == ZoneNo);

			FiresecManager.FiresecService.AddJournalRecord(new JournalRecord()
			{
				SystemTime = DateTime.Now,
				DeviceTime = DateTime.Now,
				ZoneName = zone.No.ToString(),
				Description = "Состояние \"" + StateName + "\" подтверждено оператором",
				StateType = StateType.Info
			});
		}

		public bool CanRemoveFromIgnoreList()
		{
			return ((StateType == StateType.Off) &&
			(FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_RemoveFromIgnoreList)));
		}

		public void RemoveFromIgnoreList()
		{
			var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == DeviceUID);
			if (FiresecManager.CanDisable(deviceState) && deviceState.IsDisabled)
                FiresecManager.FiresecDriver.RemoveFromIgnoreList(new List<Guid>() { deviceState.Device.UID });
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
				if (device.ZoneNo == ZoneNo)
				{
					var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
					if (deviceState.States.Any(x => x.DriverState.StateType == StateType.Fire))
					{
						DeviceUID = deviceState.UID;
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
				parentDeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.ParentPanel.UID);
			else
				parentDeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.Parent.UID);

			var resetItem = new ResetItem();

			switch (AlarmType)
			{
				case AlarmType.Fire:
				case AlarmType.Attention:
				case AlarmType.Info:
				case AlarmType.Failure:
					resetItem.DeviceUID = parentDeviceState.UID;
					foreach (var state in parentDeviceState.States)
					{
						if (state.DriverState.StateType == EnumsConverter.AlarmTypeToStateType(AlarmType) && state.DriverState.IsManualReset)
							resetItem.StateNames.Add(state.DriverState.Name);
					}
					break;

				case AlarmType.Service:
					resetItem.DeviceUID = device.UID;
					var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
					foreach (var state in deviceState.States)
					{
						if (state.DriverState.IsManualReset)
							resetItem.StateNames.Add(state.DriverState.Name);
					}
					break;

				case AlarmType.Auto:
				case AlarmType.Off:
					break;
			}

			if (resetItem.StateNames.Count > 0)
				return resetItem;
			return null;
		}
	}
}