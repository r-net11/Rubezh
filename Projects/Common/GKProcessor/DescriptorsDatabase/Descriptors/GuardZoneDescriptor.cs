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
		GKGuardZone GuardZone { get; set; }
		public GuardZonePimDescriptor GuardZonePimDescriptor { get; private set; }
		List<GKGuardZoneDevice> SetGuardDevices;
		List<GKGuardZoneDevice> ResetGuardDevices;
		List<GKGuardZoneDevice> ChangeGuardDevices;
		List<GKGuardZoneDevice> SetAlarmDevices;

		public GuardZoneDescriptor(GKGuardZone zone, DatabaseType databaseType)
			: base(zone, databaseType)
		{
			DatabaseType = databaseType;
			DescriptorType = DescriptorType.GuardZone;
			GuardZone = zone;

			SetGuardDevices = new List<GKGuardZoneDevice>();
			ResetGuardDevices = new List<GKGuardZoneDevice>();
			ChangeGuardDevices = new List<GKGuardZoneDevice>();
			SetAlarmDevices = new List<GKGuardZoneDevice>();
			foreach (var guardZoneDevice in GuardZone.GuardZoneDevices)
			{
				switch (guardZoneDevice.Device.DriverType)
				{
					case GKDriverType.RSR2_MAP4:
					case GKDriverType.RSR2_AM_1:
					case GKDriverType.RSR2_GuardDetector:
						switch (guardZoneDevice.ActionType)
						{
							case GKGuardZoneDeviceActionType.SetGuard:
								SetGuardDevices.Add(guardZoneDevice);
								break;

							case GKGuardZoneDeviceActionType.ResetGuard:
								ResetGuardDevices.Add(guardZoneDevice);
								break;

							case GKGuardZoneDeviceActionType.ChangeGuard:
								ChangeGuardDevices.Add(guardZoneDevice);
								break;

							case GKGuardZoneDeviceActionType.SetAlarm:
								SetAlarmDevices.Add(guardZoneDevice);
								break;
						}
						break;

					case GKDriverType.RSR2_CodeReader:
						if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs.Count > 1)
							{
								SetGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs.Count > 1)
							{
								ResetGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs.Count > 1)
							{
								ChangeGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs.Count > 1)
							{
								SetAlarmDevices.Add(guardZoneDevice);
							}
						}
						break;
				}
			}

			if (ChangeGuardDevices.Count > 0)
			{
				GuardZonePimDescriptor = new GuardZonePimDescriptor(GuardZone, ChangeGuardDevices, DatabaseType);
			}
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
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				FormulaBytes = Formula.GetBytes();
				return;
			}

			AddGuardDevicesLogic(SetAlarmDevices, GKStateBit.Fire1);
			AddGuardDevicesLogic(SetGuardDevices, GKStateBit.TurnOn_InAutomatic);
			AddGuardDevicesLogic(ResetGuardDevices, GKStateBit.TurnOff_InAutomatic);
			AddChangeDevicesLogic(ChangeGuardDevices);

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

						Formula.AddGetBit(stateBit, guardDevice.Device, DatabaseType);
						switch (GuardZone.GuardZoneEnterMethod)
						{
							case GKGuardZoneEnterMethod.GlobalOnly:
								Formula.Add(FormulaOperationType.BR, 1, (byte)(2 + settingsPart.CodeUIDs.Count*2));
								Formula.Add(FormulaOperationType.KOD, 0, DatabaseType == DatabaseType.Gk ? guardDevice.Device.GKDescriptorNo : guardDevice.Device.KAUDescriptorNo);
								foreach (var codeUID in settingsPart.CodeUIDs)
								{
									var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
									Formula.Add(FormulaOperationType.CMPKOD, 1, DatabaseType == DatabaseType.Gk ? code.GKDescriptorNo : code.KAUDescriptorNo);
									Formula.Add(FormulaOperationType.OR);
								}
								break;

							case GKGuardZoneEnterMethod.UserOnly:
								Formula.Add(FormulaOperationType.BR, 1, 2);
								Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.AND);
								break;

							case GKGuardZoneEnterMethod.Both:
								Formula.Add(FormulaOperationType.BR, 1, (byte)(4 + settingsPart.CodeUIDs.Count * 2));
								Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
								foreach (var codeUID in settingsPart.CodeUIDs)
								{
									var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
									Formula.Add(FormulaOperationType.CMPKOD, 1, DatabaseType == DatabaseType.Gk ? code.GKDescriptorNo : code.KAUDescriptorNo);
									Formula.Add(FormulaOperationType.OR);
								}
								Formula.Add(FormulaOperationType.ACS, (byte)GuardZone.SetGuardLevel, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.OR);
								break;
						}
					}
					else
					{
						Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device, DatabaseType);
					}
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
				if (commandStateBit == GKStateBit.Fire1)
				{
					Formula.AddGetBit(GKStateBit.Fire1, GuardZone, DatabaseType);
					Formula.Add(FormulaOperationType.OR);
				}
				Formula.AddPutBit(commandStateBit, GuardZone, DatabaseType);
			}
		}

		void AddChangeDevicesLogic(List<GKGuardZoneDevice> guardZoneDevices)
		{
			if (GuardZone.Pim != null && guardZoneDevices.Count > 0)
			{
				GuardZone.LinkGKBases(GuardZone.Pim);
				Formula.AddGetBit(GKStateBit.On, GuardZone.Pim, DatabaseType);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, GuardZone, DatabaseType);
				Formula.AddGetBit(GKStateBit.Off, GuardZone.Pim, DatabaseType);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, GuardZone, DatabaseType);
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
				Value = (ushort)GuardZone.SetDelay
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = (ushort)GuardZone.ResetDelay
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)GuardZone.AlarmDelay
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