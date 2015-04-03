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
					case GKDriverType.RSR2_GuardDetectorSound:
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
					case GKDriverType.RSR2_CardReader:
						if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs.Count > 0 || GuardZone.GuardZoneEnterMethod != GKGuardZoneEnterMethod.GlobalOnly)
							{
								SetGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs.Count > 0 || GuardZone.GuardZoneEnterMethod != GKGuardZoneEnterMethod.GlobalOnly)
							{
								ResetGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs.Count > 0)
							{
								ChangeGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs.Count > 0 || GuardZone.GuardZoneEnterMethod != GKGuardZoneEnterMethod.GlobalOnly)
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
			AddMissedLogic(SetAlarmDevices);
			if (GuardZone.Pim != null && ChangeGuardDevices.Count > 0)
			{
				GuardZone.LinkGKBases(GuardZone.Pim);
			}
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void AddGuardDevicesLogic(List<GKGuardZoneDevice> guardZoneDevices, GKStateBit commandStateBit)
		{
			var count = 0;
			foreach (var guardDevice in guardZoneDevices)
			{
				if (guardDevice.Device.DriverType == GKDriverType.RSR2_CodeReader || guardDevice.Device.DriverType == GKDriverType.RSR2_CardReader)
				{
					GKCodeReaderSettingsPart settingsPart = null;
					var level = 0;
					switch (commandStateBit)
					{
						case GKStateBit.TurnOn_InAutomatic:
							settingsPart = guardDevice.CodeReaderSettings.SetGuardSettings;
							level = GuardZone.SetGuardLevel;
							break;

						case GKStateBit.TurnOff_InAutomatic:
							settingsPart = guardDevice.CodeReaderSettings.ResetGuardSettings;
							level = GuardZone.ResetGuardLevel;
							break;

						case GKStateBit.Fire1:
							settingsPart = guardDevice.CodeReaderSettings.AlarmSettings;
							level = -1;
							break;
					}

					var stateBit = CodeReaderEnterTypeToStateBit(settingsPart.CodeReaderEnterType);
					Formula.AddGetBit(stateBit, guardDevice.Device, DatabaseType);
					var gotoFormulaOperation = Formula.Add(FormulaOperationType.BR, 1, 100);
					var formulaNo = Formula.FormulaOperations.Count;

					switch (GuardZone.GuardZoneEnterMethod)
					{
						case GKGuardZoneEnterMethod.GlobalOnly:
							var codeIndex = 0;
							foreach (var codeUID in settingsPart.CodeUIDs)
							{
								var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
								Formula.Add(FormulaOperationType.KOD, 0, DatabaseType == DatabaseType.Gk ? guardDevice.Device.GKDescriptorNo : guardDevice.Device.KAUDescriptorNo);
								Formula.Add(FormulaOperationType.CMPKOD, 1, DatabaseType == DatabaseType.Gk ? code.GKDescriptorNo : code.KAUDescriptorNo);
								if (codeIndex > 0)
								{
									Formula.Add(FormulaOperationType.OR);
								}
								codeIndex++;
							}
							break;

						case GKGuardZoneEnterMethod.UserOnly:
							if (level >= 0)
							{
								Formula.Add(FormulaOperationType.ACS, (byte)level, guardDevice.Device.GKDescriptorNo);
							}
							break;

						case GKGuardZoneEnterMethod.Both:
							codeIndex = 0;
							foreach (var codeUID in settingsPart.CodeUIDs)
							{
								var code = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == codeUID);
								Formula.Add(FormulaOperationType.KOD, 0, guardDevice.Device.GKDescriptorNo);
								Formula.Add(FormulaOperationType.CMPKOD, 1, DatabaseType == DatabaseType.Gk ? code.GKDescriptorNo : code.KAUDescriptorNo);
								if (codeIndex > 0)
								{
									Formula.Add(FormulaOperationType.OR);
								}
								codeIndex++;
							}
							if (level >= 0)
							{
								Formula.Add(FormulaOperationType.ACS, (byte)level, guardDevice.Device.GKDescriptorNo);
								if (codeIndex > 0)
								{
									Formula.Add(FormulaOperationType.OR);
								}
							}
							break;
					}

					gotoFormulaOperation.SecondOperand = (ushort)(Formula.FormulaOperations.Count - formulaNo);
				}
				else
				{
					Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device, DatabaseType);
				}
				if (commandStateBit == GKStateBit.Fire1)
				{
					//if (count > 0)
					//{
					//    Formula.Add(FormulaOperationType.OR);
					//}
					//count++;
					Formula.AddGetBit(GKStateBit.Failure, guardDevice.Device, DatabaseType);
					Formula.Add(FormulaOperationType.OR);
				}
				if (count > 0)
				{
					Formula.Add(FormulaOperationType.OR);
				}
				count++;
			}

			switch(commandStateBit)
			{
				case GKStateBit.Fire1:
					if (count > 0)
					{
						Formula.AddGetBit(GKStateBit.Attention, GuardZone, DatabaseType);
						Formula.Add(FormulaOperationType.OR);
						Formula.AddPutBit(GKStateBit.Attention, GuardZone, DatabaseType);
					}
					break;
				case GKStateBit.TurnOn_InAutomatic:
				case GKStateBit.TurnOff_InAutomatic:
					if (count > 0 || ChangeGuardDevices.Count > 0)
					{
						if (ChangeGuardDevices.Count > 0)
						{
							GuardZonePimDescriptor.SetGuardZoneChangeLogic(GuardZone, ChangeGuardDevices, Formula, DatabaseType);
							if (commandStateBit == GKStateBit.TurnOn_InAutomatic)
							{
								Formula.AddGetBit(GKStateBit.On, GuardZone.Pim, DatabaseType);
								Formula.Add(FormulaOperationType.AND);
							}
							if (commandStateBit == GKStateBit.TurnOff_InAutomatic)
							{
								Formula.AddGetBit(GKStateBit.Off, GuardZone.Pim, DatabaseType);
								Formula.Add(FormulaOperationType.AND);
							}
							if (count > 0 && ChangeGuardDevices.Count > 0)
							{
								Formula.Add(FormulaOperationType.OR);
							}
						}
						if (commandStateBit == GKStateBit.TurnOn_InAutomatic)
						{
							foreach (var setAlarmDevice in SetAlarmDevices)
							{
								if (setAlarmDevice.Device.DriverType != GKDriverType.RSR2_CardReader)
								{
									Formula.AddGetBit(GKStateBit.Fire1, setAlarmDevice.Device, DatabaseType);
									Formula.AddGetBit(GKStateBit.Failure, setAlarmDevice.Device, DatabaseType);
									Formula.Add(FormulaOperationType.OR);
									Formula.Add(FormulaOperationType.COM);
									Formula.Add(FormulaOperationType.AND);
								}
							}
						}
						Formula.AddPutBit(commandStateBit, GuardZone, DatabaseType);
					}
					break;
			}
		}

		private void AddMissedLogic(List<GKGuardZoneDevice> guardZoneDevices)
		{
			var count = 0;
			if (guardZoneDevices.Count == 0)
				return;
			Formula.AddGetBit(GKStateBit.TurningOn, GuardZone, DatabaseType);
			Formula.Add(FormulaOperationType.BR, 2, 1);
			Formula.Add(FormulaOperationType.EXIT);
			foreach (var guardDevice in guardZoneDevices)
			{
				if (guardDevice.Device.DriverType != GKDriverType.RSR2_CardReader)
				{
					Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device, DatabaseType);
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					Formula.AddGetBit(GKStateBit.Failure, guardDevice.Device, DatabaseType);
					Formula.Add(FormulaOperationType.OR);
					count++;
				}
			}
			if (count > 0)
			{
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