using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class GuardZoneDescriptor : BaseDescriptor
	{
		public GuardZoneDescriptor(GKGuardZone zone)
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

			var setAlarmDevices = new List<GKGuardZoneDevice>();
			var setGuardDevices = new List<GKGuardZoneDevice>();
			var resetGuardDevices = new List<GKGuardZoneDevice>();
			foreach (var guardZoneDevice in GuardZone.GuardZoneDevices)
			{
				switch (guardZoneDevice.Device.DriverType)
				{
					case GKDriverType.RSR2_AM_1:
					case GKDriverType.RSR2_GuardDetector:
						switch (guardZoneDevice.ActionType)
						{
							case GKGuardZoneDeviceActionType.SetAlarm:
								setAlarmDevices.Add(guardZoneDevice);
								break;

							case GKGuardZoneDeviceActionType.SetGuard:
								setGuardDevices.Add(guardZoneDevice);
								break;

							case GKGuardZoneDeviceActionType.ResetGuard:
								resetGuardDevices.Add(guardZoneDevice);
								break;
						}
						break;

					case GKDriverType.RSR2_CodeReader:
						if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUID);
							if (code != null)
							{
								setAlarmDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUID);
							if (code != null)
							{
								setGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUID);
							if (code != null)
							{
								resetGuardDevices.Add(guardZoneDevice);
							}
						}
						break;
				}
			}

			AddGuardDevicesLogic(setAlarmDevices, GKStateBit.Fire1);
			AddGuardDevicesLogic(setGuardDevices, GKStateBit.TurnOn_InAutomatic);
			AddGuardDevicesLogic(resetGuardDevices, GKStateBit.TurnOff_InAutomatic);

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void AddGuardDevicesLogic(List<GKGuardZoneDevice> guardZoneDevices, GKStateBit commandStateBit)
		{
			if (guardZoneDevices.Count > 0)
			{
				var count = 0;
				foreach (var guardDevice in guardZoneDevices)
				{
					if (guardDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
					{
						XCodeReaderSettingsPart settingsPart = null;
						switch(commandStateBit)
						{
							case GKStateBit.TurnOn_InAutomatic:
								settingsPart = guardDevice.CodeReaderSettings.SetGuardSettings;
								break;

							case GKStateBit.TurnOff_InAutomatic:
								settingsPart = guardDevice.CodeReaderSettings.ResetGuardSettings;
								break;

							case GKStateBit.Fire1:
								settingsPart = guardDevice.CodeReaderSettings.AlarmSettings;
								break;
						}
						var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);
						var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == settingsPart.CodeUID);

						Formula.AddGetBit(stateBit, guardDevice.Device);
						switch (GuardZone.GuardZoneEnterMethod)
						{
							case GKGuardZoneEnterMethod.GlobalOnly:
								Formula.Add(FormulaOperationType.BR, 1, 3);
								Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
								break;

							case GKGuardZoneEnterMethod.UserOnly:
								Formula.Add(FormulaOperationType.BR, 1, 2);
								Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.AND);
								break;

							case GKGuardZoneEnterMethod.Both:
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
						Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device);
					}
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
				if (commandStateBit == GKStateBit.Fire1)
				{
					Formula.AddGetBit(GKStateBit.Fire1, GuardZone);
					Formula.Add(FormulaOperationType.OR);
				}
				Formula.AddPutBit(commandStateBit, GuardZone);
			}
		}

		GKStateBit CodeReaderEnterTypeToStateBit(GKCodeReaderEnterType codeReaderEnterType)
		{
			switch (codeReaderEnterType)
			{
				case GKCodeReaderEnterType.CodeOnly:
					return GKStateBit.Attention;

				case GKCodeReaderEnterType.CodeAndOne:
					return GKStateBit.Fire1;

				case GKCodeReaderEnterType.CodeAndTwo:
					return GKStateBit.Fire2;
			}
			return GKStateBit.Fire1;
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