using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;

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

			var setAlarmDevices = new List<XGuardZoneDevice>();
			var setGuardDevices = new List<XGuardZoneDevice>();
			var resetGuardDevices = new List<XGuardZoneDevice>();
			foreach (var guardZoneDevice in GuardZone.GuardZoneDevices)
			{
				switch (guardZoneDevice.Device.DriverType)
				{
					case XDriverType.RSR2_AM_1:
					case XDriverType.RSR2_GuardDetector:
						switch (guardZoneDevice.ActionType)
						{
							case XGuardZoneDeviceActionType.SetAlarm:
								setAlarmDevices.Add(guardZoneDevice);
								break;

							case XGuardZoneDeviceActionType.SetGuard:
								setGuardDevices.Add(guardZoneDevice);
								break;

							case XGuardZoneDeviceActionType.ResetGuard:
								resetGuardDevices.Add(guardZoneDevice);
								break;
						}
						break;

					case XDriverType.RSR2_CodeReader:
						if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeReaderEnterType != XCodeReaderEnterType.None)
						{
							var code = XManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUID);
							if (code != null)
							{
								setAlarmDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType != XCodeReaderEnterType.None)
						{
							var code = XManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUID);
							if (code != null)
							{
								setGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeReaderEnterType != XCodeReaderEnterType.None)
						{
							var code = XManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUID);
							if (code != null)
							{
								resetGuardDevices.Add(guardZoneDevice);
							}
						}
					    break;
				}
			}

			if (setAlarmDevices.Count > 0)
			{
				var count = 0;
				foreach (var guardDevice in setAlarmDevices)
				{
					if (guardDevice.Device.DriverType == XDriverType.RSR2_CodeReader)
					{
						var stateBit = CodeReaderEnterTypeToStateBit(guardDevice.CodeReaderSettings.AlarmSettings.CodeReaderEnterType);
						Formula.AddGetBit(stateBit, guardDevice.Device);
						Formula.Add(FormulaOperationType.BR, 1, 4);
						Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
						var code = XManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardDevice.CodeReaderSettings.AlarmSettings.CodeUID);
						Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
						Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetAlarmLevel, guardDevice.Device.GKDescriptorNo);
						Formula.Add(FormulaOperationType.OR);
					}
					else
					{
						Formula.AddGetBit(XStateBit.Fire1, guardDevice.Device);
					}
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

			if (setGuardDevices.Count > 0)
			{
				var count = 0;
				foreach (var guardDevice in setGuardDevices)
				{
					if (guardDevice.Device.DriverType == XDriverType.RSR2_CodeReader)
					{
						var stateBit = CodeReaderEnterTypeToStateBit(guardDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType);
						Formula.AddGetBit(stateBit, guardDevice.Device);
						Formula.Add(FormulaOperationType.BR, 1, 4);
						Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
						var code = XManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardDevice.CodeReaderSettings.SetGuardSettings.CodeUID);
						Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
						Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
						Formula.Add(FormulaOperationType.OR);
					}
					else
					{
						Formula.AddGetBit(XStateBit.Fire1, guardDevice.Device);
					}
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
				Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, GuardZone);
			}

			if (resetGuardDevices.Count > 0)
			{
				var count = 0;
				foreach (var guardDevice in resetGuardDevices)
				{
					if (guardDevice.Device.DriverType == XDriverType.RSR2_CodeReader)
					{
						var stateBit = CodeReaderEnterTypeToStateBit(guardDevice.CodeReaderSettings.ResetGuardSettings.CodeReaderEnterType);
						Formula.AddGetBit(stateBit, guardDevice.Device);
						Formula.Add(FormulaOperationType.BR, 1, 4);
						Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
						var code = XManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardDevice.CodeReaderSettings.ResetGuardSettings.CodeUID);
						Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
						Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.ResetGuardLevel, guardDevice.Device.GKDescriptorNo);
						Formula.Add(FormulaOperationType.OR);
					}
					else
					{
						Formula.AddGetBit(XStateBit.Fire1, guardDevice.Device);
					}
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
				Formula.AddPutBit(XStateBit.TurnOff_InAutomatic, GuardZone);
			}

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		XStateBit CodeReaderEnterTypeToStateBit(XCodeReaderEnterType codeReaderEnterType)
		{
			switch (codeReaderEnterType)
			{
				case XCodeReaderEnterType.CodeOnly:
					return XStateBit.Attention;

				case XCodeReaderEnterType.CodeAndOne:
					return XStateBit.Fire1;

				case XCodeReaderEnterType.CodeAndTwo:
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