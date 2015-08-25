using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

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

		public GuardZoneDescriptor(GKGuardZone zone)
			: base(zone)
		{
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
							if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs.Count > 0 || guardZoneDevice.CodeReaderSettings.SetGuardSettings.AccessLevel > 0)
							{
								SetGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs.Count > 0 || guardZoneDevice.CodeReaderSettings.ResetGuardSettings.AccessLevel > 0)
							{
								ResetGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs.Count > 0 || guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.AccessLevel > 0)
							{
								ChangeGuardDevices.Add(guardZoneDevice);
							}
						}

						if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeReaderEnterType != GKCodeReaderEnterType.None)
						{
							if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs.Count > 0 || guardZoneDevice.CodeReaderSettings.AlarmSettings.AccessLevel > 0)
							{
								SetAlarmDevices.Add(guardZoneDevice);
							}
						}
						break;
				}
			}

			if (ChangeGuardDevices.Count > 0)
			{
				GuardZonePimDescriptor = new GuardZonePimDescriptor(GuardZone, ChangeGuardDevices);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x108);
			SetAddress(0);
			SetPropertiesBytes();
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
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
		}

		void AddGuardDevicesLogic(List<GKGuardZoneDevice> guardZoneDevices, GKStateBit commandStateBit)
		{
			var count = 0;

			foreach (var guardDevice in guardZoneDevices.Where(x => x.Device.DriverType != GKDriverType.RSR2_CodeReader && x.Device.DriverType != GKDriverType.RSR2_CardReader))
			{
				Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device);
				if (count > 0)
				{
					Formula.Add(FormulaOperationType.OR);
				}

				if (commandStateBit == GKStateBit.Fire1)
				{
					Formula.AddGetBit(GKStateBit.Fire2, guardDevice.Device);
					Formula.Add(FormulaOperationType.OR);
					Formula.AddGetBit(GKStateBit.Failure, guardDevice.Device);
					Formula.Add(FormulaOperationType.OR);
				}
				count++;
			}

			foreach (var guardDevice in guardZoneDevices.Where(x => x.Device.DriverType == GKDriverType.RSR2_CodeReader || x.Device.DriverType == GKDriverType.RSR2_CardReader))
			{
				GKCodeReaderSettingsPart settingsPart = null;
				switch (commandStateBit)
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

				FormulaHelper.AddCodeReaderLogic(Formula, settingsPart, guardDevice.Device);
				if (count > 0)
				{
					Formula.Add(FormulaOperationType.OR);
				}
				count++;
			}

			switch (commandStateBit)
			{
				case GKStateBit.Fire1:
					if (count > 0)
					{
						Formula.AddGetBit(GKStateBit.Attention, GuardZone);
						Formula.Add(FormulaOperationType.OR);
						Formula.AddPutBit(GKStateBit.Attention, GuardZone);
					}
					break;
				case GKStateBit.TurnOn_InAutomatic:
				case GKStateBit.TurnOff_InAutomatic:
					if (count > 0 || ChangeGuardDevices.Count > 0)
					{
						if (ChangeGuardDevices.Count > 0)
						{
							GuardZonePimDescriptor.SetGuardZoneChangeLogic(GuardZone, ChangeGuardDevices, Formula);
							if (commandStateBit == GKStateBit.TurnOn_InAutomatic)
							{
								Formula.AddGetBit(GKStateBit.On, GuardZone.Pim);
								Formula.Add(FormulaOperationType.AND);
							}
							if (commandStateBit == GKStateBit.TurnOff_InAutomatic)
							{
								Formula.AddGetBit(GKStateBit.Off, GuardZone.Pim);
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
								if (setAlarmDevice.Device.DriverType != GKDriverType.RSR2_CodeReader && setAlarmDevice.Device.DriverType != GKDriverType.RSR2_CardReader)
								{
									Formula.AddGetBit(GKStateBit.Fire1, setAlarmDevice.Device);
									Formula.AddGetBit(GKStateBit.Failure, setAlarmDevice.Device);
									Formula.Add(FormulaOperationType.OR);
									Formula.Add(FormulaOperationType.COM);
									Formula.Add(FormulaOperationType.AND);
								}
							}
						}
						Formula.Add(FormulaOperationType.BR, 1, 3);
						Formula.Add(FormulaOperationType.CONST, 0, 1);
						Formula.AddPutBit(commandStateBit, GuardZone);
						Formula.Add(FormulaOperationType.EXIT);
					}
					break;
			}
		}

		void AddMissedLogic(List<GKGuardZoneDevice> guardZoneDevices)
		{
			if (guardZoneDevices.Count == 0)
				return;
			var count = 0;
			Formula.AddGetBit(GKStateBit.TurningOn, GuardZone);
			Formula.Add(FormulaOperationType.BR, 2, 1);
			Formula.Add(FormulaOperationType.EXIT);
			foreach (var guardDevice in guardZoneDevices)
			{
				if (guardDevice.Device.DriverType != GKDriverType.RSR2_CodeReader && guardDevice.Device.DriverType != GKDriverType.RSR2_CardReader)
				{
					Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Device);
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					Formula.AddGetBit(GKStateBit.Failure, guardDevice.Device);
					Formula.Add(FormulaOperationType.OR);
					count++;
				}
			}
			if (count > 0)
			{
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, GuardZone);
			}
		}

		void SetPropertiesBytes()
		{
			Parameters = new List<byte>();
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