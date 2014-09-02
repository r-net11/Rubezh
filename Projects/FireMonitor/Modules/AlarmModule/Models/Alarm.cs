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
		public Device Device { get; set; }
		public Zone Zone { get; set; }
		public string StateName { get; set; }

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

		public bool CanRemoveFromIgnoreList()
		{
			//return ((StateType == StateType.Off) && (FiresecManager.CheckPermission(PermissionType.Oper_RemoveFromIgnoreList)));
			return (StateType == StateType.Off);
		}

		public void RemoveFromIgnoreList()
		{
			if (Device != null)
			{
				if (FiresecManager.CanDisable(Device.DeviceState) && Device.DeviceState.IsDisabled)
					FiresecManager.RemoveFromIgnoreList(new List<Device>() { Device });
			}
		}

		public bool CanReset()
		{
			return GetResetItem() != null;
		}

		public void Reset()
		{
			var resetItems = new List<ResetItem>();
			resetItems.Add(GetResetItem());
			FiresecManager.ResetStates(resetItems);
		}

		public ResetItem GetResetItem()
		{
			switch (AlarmEntityType)
			{
				case AlarmEntityType.Device:
					return GetDeviceResetItem(Device);

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
				if (device.Zone != null && device.Zone == Zone)
				{
					if (device.DeviceState.ThreadSafeStates.Any(x => x.DriverState.StateType == StateType.Fire))
					{
						return GetDeviceResetItem(device);
					}
				}
			}

			return null;
		}

		ResetItem GetDeviceResetItem(Device device)
		{
			if (device == null)
				return null;

			DeviceState parentDeviceState;
			if (device.ParentPanel != null)
				parentDeviceState = device.ParentPanel.DeviceState;
			else
				parentDeviceState = device.Parent.DeviceState;

			var resetItem = new ResetItem();

			switch (AlarmType)
			{
				case AlarmType.Guard:
				case AlarmType.Fire:
				case AlarmType.Attention:
				case AlarmType.Info:
				case AlarmType.Failure:
					resetItem.DeviceState = parentDeviceState;
					foreach (var state in parentDeviceState.ThreadSafeStates)
					{
						if (state.DriverState.StateType == EnumsConverter.AlarmTypeToStateType(AlarmType) && state.DriverState.IsManualReset)
							resetItem.States.Add(state);
					}
					break;

				case AlarmType.Service:
					resetItem.DeviceState = device.DeviceState;
					foreach (var state in device.DeviceState.ThreadSafeStates)
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

		public Alarm Clone()
		{
			var alarm = new Alarm();
			alarm.StateType = StateType;
			alarm.Device = Device;
			alarm.Zone = Zone;
			return alarm;
		}

		public bool IsEqualTo(Alarm alarm)
		{
			if (alarm.AlarmType != AlarmType)
				return false;
			if (alarm.Device != null && alarm.Device != Device)
				return false;
			if (alarm.Zone != null && alarm.Zone != Zone)
				return false;
			return true;
		}
	}
}