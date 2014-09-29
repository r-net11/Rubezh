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

			AddGuardDevicesLogic(setAlarmDevices, XStateBit.Fire1);
			AddGuardDevicesLogic(setGuardDevices, XStateBit.TurnOn_InAutomatic);
			AddGuardDevicesLogic(resetGuardDevices, XStateBit.TurnOff_InAutomatic);

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void AddGuardDevicesLogic(List<XGuardZoneDevice> guardZoneDevices, XStateBit commandStateBit)
		{
			if (guardZoneDevices.Count > 0)
			{
				var count = 0;
				foreach (var guardDevice in guardZoneDevices)
				{
					if (guardDevice.Device.DriverType == XDriverType.RSR2_CodeReader)
					{
						XCodeReaderSettingsPart settingsPart = null;
						switch(commandStateBit)
						{
							case XStateBit.TurnOn_InAutomatic:
								settingsPart = guardDevice.CodeReaderSettings.SetGuardSettings;
								break;

							case XStateBit.TurnOff_InAutomatic:
								settingsPart = guardDevice.CodeReaderSettings.ResetGuardSettings;
								break;

							case XStateBit.Fire1:
								settingsPart = guardDevice.CodeReaderSettings.AlarmSettings;
								break;
						}
						var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);
						var code = XManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == settingsPart.CodeUID);

						Formula.AddGetBit(stateBit, guardDevice.Device);
						switch (GuardZone.GuardZoneEnterMethod)
						{
							case XGuardZoneEnterMethod.GlobalOnly:
								Formula.Add(FormulaOperationType.BR, 1, 3);
								Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
								break;

							case XGuardZoneEnterMethod.UserOnly:
								Formula.Add(FormulaOperationType.BR, 1, 2);
								Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.AND);
								break;

							case XGuardZoneEnterMethod.Both:
								Formula.Add(FormulaOperationType.BR, 1, 5);
								Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
								Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.OR);
								break;
						}
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
				if (commandStateBit == XStateBit.Fire1)
				{
					Formula.AddGetBit(XStateBit.Fire1, GuardZone);
					Formula.Add(FormulaOperationType.OR);
				}
				Formula.AddPutBit(commandStateBit, GuardZone);
			}
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