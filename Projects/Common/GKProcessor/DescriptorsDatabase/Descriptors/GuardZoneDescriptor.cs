using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using Common;

namespace GKProcessor
{
	public class GuardZoneDescriptor : BaseDescriptor
	{
		GkDatabase GkDatabase;
		public GKPim GuardZonePim { get; private set; }
		public PimDescriptor GuardZonePimDescriptor { get; private set; }

		public GuardZoneDescriptor(GkDatabase gkDatabase, GKGuardZone zone)
		{
			GkDatabase = gkDatabase;
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

			var setGuardDevices = new List<GKGuardZoneDevice>();
			var resetGuardDevices = new List<GKGuardZoneDevice>();
			var changeGuardDevices = new List<GKGuardZoneDevice>();
			var setAlarmDevices = new List<GKGuardZoneDevice>();
			foreach (var guardZoneDevice in GuardZone.GuardZoneDevices)
			{
				switch (guardZoneDevice.Device.DriverType)
				{
					case GKDriverType.RSR2_AM_1:
					case GKDriverType.RSR2_GuardDetector:
						switch (guardZoneDevice.ActionType)
						{
							case GKGuardZoneDeviceActionType.SetGuard:
								setGuardDevices.Add(guardZoneDevice);
								break;

							case GKGuardZoneDeviceActionType.ResetGuard:
								resetGuardDevices.Add(guardZoneDevice);
								break;

							case GKGuardZoneDeviceActionType.ChangeGuard:
								changeGuardDevices.Add(guardZoneDevice);
								break;

							case GKGuardZoneDeviceActionType.SetAlarm:
								setAlarmDevices.Add(guardZoneDevice);
								break;
						}
						break;

					case GKDriverType.RSR2_CodeReader:
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

						if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUID);
							if (code != null)
							{
								changeGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUID);
							if (code != null)
							{
								setAlarmDevices.Add(guardZoneDevice);
							}
						}
						break;
				}
			}

			AddGuardDevicesLogic(setAlarmDevices, GKStateBit.Fire1);
			AddGuardDevicesLogic(setGuardDevices, GKStateBit.TurnOn_InAutomatic);
			AddGuardDevicesLogic(resetGuardDevices, GKStateBit.TurnOff_InAutomatic);
			AddChangeDevicesLogic(changeGuardDevices);

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
						GKCodeReaderSettingsPart settingsPart = null;
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

						Formula.AddGetBit(stateBit, guardDevice.Device, DatabaseType.Gk);
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
						Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device, DatabaseType.Gk);
					}
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
				if (commandStateBit == GKStateBit.Fire1)
				{
					Formula.AddGetBit(GKStateBit.Fire1, GuardZone, DatabaseType.Gk);
					Formula.Add(FormulaOperationType.OR);
				}
				Formula.AddPutBit(commandStateBit, GuardZone, DatabaseType.Gk);
			}
		}

		void AddChangeDevicesLogic(List<GKGuardZoneDevice> guardZoneDevices)
		{
			if (guardZoneDevices.Count > 0)
			{
				GuardZonePim = new GKPim()
				{
					Name = GuardZone.PresentationName,
					IsAutoGenerated = true,
					GuardZoneUID = GuardZone.UID
				};
				GuardZonePim.UID = GuidHelper.CreateOn(GuardZone.UID, 0);
				GuardZonePimDescriptor = new PimDescriptor(GuardZonePim);

				var count = 0;
				foreach (var guardDevice in guardZoneDevices)
				{
					GKDeviceConfiguration.LinkGKBases(GuardZonePim, guardDevice.Device);

					if (guardDevice.Device.DriverType == GKDriverType.RSR2_CodeReader)
					{
						var settingsPart = guardDevice.CodeReaderSettings.ChangeGuardSettings;
						var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);
						var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == settingsPart.CodeUID);

						GuardZonePimDescriptor.Formula.AddGetBit(stateBit, guardDevice.Device, DatabaseType.Gk);
						switch (GuardZone.GuardZoneEnterMethod)
						{
							case GKGuardZoneEnterMethod.GlobalOnly:
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.BR, 1, 3);
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
								break;

							case GKGuardZoneEnterMethod.UserOnly:
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.BR, 1, 2);
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.AND);
								break;

							case GKGuardZoneEnterMethod.Both:
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.BR, 1, 5);
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.CMPKOD, 1, code.GKDescriptorNo);
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
								GuardZonePimDescriptor.Formula.Add(FormulaOperationType.OR);
								break;
						}
					}
					else
					{
						GuardZonePimDescriptor.Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device, DatabaseType.Gk);
					}
					if (count > 0)
					{
						GuardZonePimDescriptor.Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
				GuardZonePimDescriptor.Formula.Add(FormulaOperationType.DUP);
				GuardZonePimDescriptor.Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, GuardZone, DatabaseType.Gk);
				GuardZonePimDescriptor.Formula.Add(FormulaOperationType.COM);
				GuardZonePimDescriptor.Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, GuardZone, DatabaseType.Gk);
				GuardZonePimDescriptor.Formula.Add(FormulaOperationType.END);
				GuardZonePimDescriptor.FormulaBytes = GuardZonePimDescriptor.Formula.GetBytes();
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