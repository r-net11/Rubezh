using FiresecAPI.GK;
using FiresecAPI.Models;
using System.Collections.Generic;
using System;

namespace GKProcessor
{
	public class GuardZoneDescriptor : BaseDescriptor
	{
		public GuardZoneDescriptor(XGuardZone zone)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.GuardZone;
			GuardZone = zone;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x108);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();

			List<XDevice> onDevices = new List<XDevice>();
			List<XDevice> offDevices = new List<XDevice>();
			List<XDevice> alarmDevices = new List<XDevice>();
			foreach (var guardZoneDevice in GuardZone.GuardZoneDevices)
			{
				switch(guardZoneDevice.ActionType)
				{
					case XGuardZoneDeviceActionType.SetGuard:
						onDevices.Add(guardZoneDevice.Device);
						break;

					case XGuardZoneDeviceActionType.ResetGuard:
						offDevices.Add(guardZoneDevice.Device);
						break;

					case XGuardZoneDeviceActionType.SetAlarm:
						alarmDevices.Add(guardZoneDevice.Device);
						break;
				}
			}

			if (onDevices.Count > 0)
			{
				var count = 0;
				foreach (var device in onDevices)
				{
					Formula.AddGetBit(GetDeviceStateBit(device), device);
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
				Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, GuardZone);
			}

			if (offDevices.Count > 0)
			{
				var count = 0;
				foreach (var device in offDevices)
				{
					Formula.AddGetBit(GetDeviceStateBit(device), device);
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
				Formula.AddPutBit(XStateBit.TurnOff_InAutomatic, GuardZone);
			}

			if (alarmDevices.Count > 0)
			{
				var count = 0;
				foreach (var device in alarmDevices)
				{
					Formula.AddGetBit(GetDeviceStateBit(device), device);
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}

				Formula.AddGetBit(XStateBit.Fire1, GuardZone);
				Formula.Add(FormulaOperationType.OR);
				Formula.AddPutBit(XStateBit.Fire1, GuardZone);
			}

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		XStateBit GetDeviceStateBit(XDevice device)
		{
			switch (device.DriverType)
			{
				case XDriverType.RSR2_AM_1:
				case XDriverType.RSR2_GuardDetector:
					return XStateBit.Fire1;

				case XDriverType.RSR2_HandDetector:
					return XStateBit.Fire2;
			}
			return XStateBit.Fire1;
		}

		void SetPropertiesBytes()
		{
			var binProperties = new List<BinProperty>();
			binProperties.Add(new BinProperty()
			{
				No = 0,
				Value = (ushort)GuardZone.Delay
			});

			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}
}