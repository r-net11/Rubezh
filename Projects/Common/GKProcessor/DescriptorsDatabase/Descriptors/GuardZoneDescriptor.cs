using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class GuardZoneDescriptor : BaseDescriptor
	{
		GKGuardZone GuardZone { get; set; }
		public GuardZonePimDescriptor GuardZonePimDescriptor { get; private set; }
		readonly List<Tuple<GKDevice, GKCodeReaderSettingsPart>> SetGuardDevices;
		readonly List<Tuple<GKDevice, GKCodeReaderSettingsPart>> ResetGuardDevices;
		readonly List<Tuple<GKDevice, GKCodeReaderSettingsPart>> ChangeGuardDevices;
		readonly List<Tuple<GKDevice, GKCodeReaderSettingsPart>> SetAlarmDevices;

		public GuardZoneDescriptor(GKGuardZone zone)
			: base(zone)
		{
			DescriptorType = DescriptorType.GuardZone;
			GuardZone = zone;

			SetGuardDevices = new List<Tuple<GKDevice, GKCodeReaderSettingsPart>>();
			ResetGuardDevices = new List<Tuple<GKDevice, GKCodeReaderSettingsPart>>();
			ChangeGuardDevices = new List<Tuple<GKDevice, GKCodeReaderSettingsPart>>();
			SetAlarmDevices = new List<Tuple<GKDevice, GKCodeReaderSettingsPart>>();
			foreach (var guardZoneDevice in GuardZone.GuardZoneDevices)
			{
				switch (guardZoneDevice.Device.DriverType)
				{
					case GKDriverType.RSR2_MAP4:
					case GKDriverType.RSR2_AM_1:
					case GKDriverType.RSR2_GuardDetector:
					case GKDriverType.RSR2_HandGuardDetector:
					case GKDriverType.RSR2_GuardDetectorSound:
						switch (guardZoneDevice.ActionType)
						{
							case GKGuardZoneDeviceActionType.SetGuard:
								SetGuardDevices.Add(new Tuple<GKDevice, GKCodeReaderSettingsPart>(guardZoneDevice.Device, guardZoneDevice.CodeReaderSettings.SetGuardSettings));
								break;

							case GKGuardZoneDeviceActionType.ResetGuard:
								ResetGuardDevices.Add(new Tuple<GKDevice, GKCodeReaderSettingsPart>(guardZoneDevice.Device, guardZoneDevice.CodeReaderSettings.ResetGuardSettings));
								break;

							case GKGuardZoneDeviceActionType.ChangeGuard:
								ChangeGuardDevices.Add(new Tuple<GKDevice, GKCodeReaderSettingsPart>(guardZoneDevice.Device, guardZoneDevice.CodeReaderSettings.ChangeGuardSettings));
								break;

							case GKGuardZoneDeviceActionType.SetAlarm:
								SetAlarmDevices.Add(new Tuple<GKDevice, GKCodeReaderSettingsPart>(guardZoneDevice.Device, guardZoneDevice.CodeReaderSettings.AlarmSettings));
								break;
						}
						break;

					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_CardReader:
					case GKDriverType.RSR2_CodeCardReader:
						if (guardZoneDevice.CodeReaderSettings.SetGuardSettings.CanBeUsed)
							SetGuardDevices.Add(new Tuple<GKDevice, GKCodeReaderSettingsPart>(guardZoneDevice.Device, guardZoneDevice.CodeReaderSettings.SetGuardSettings));

						if (guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CanBeUsed)
							ResetGuardDevices.Add(new Tuple<GKDevice, GKCodeReaderSettingsPart>(guardZoneDevice.Device, guardZoneDevice.CodeReaderSettings.ResetGuardSettings));

						if (guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CanBeUsed)
							ChangeGuardDevices.Add(new Tuple<GKDevice, GKCodeReaderSettingsPart>(guardZoneDevice.Device, guardZoneDevice.CodeReaderSettings.ChangeGuardSettings));

						if (guardZoneDevice.CodeReaderSettings.AlarmSettings.CanBeUsed)
							SetAlarmDevices.Add(new Tuple<GKDevice, GKCodeReaderSettingsPart>(guardZoneDevice.Device, guardZoneDevice.CodeReaderSettings.AlarmSettings));
						break;
				}
			}
			if (ChangeGuardDevices.Count > 0)
				GuardZonePimDescriptor = new GuardZonePimDescriptor(GuardZone);
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

			var mirrorParents = GuardZone.GetMirrorParents();
			Formula.AddMirrorLogic(GuardZone, mirrorParents);

			if (ChangeGuardDevices.Count == 0)
				GuardZone.LinkToDescriptor(GuardZone);
			AddGuardDevicesLogic(SetAlarmDevices, GKStateBit.Fire1);
			AddGuardDevicesLogic(SetGuardDevices, GKStateBit.TurnOn_InAutomatic);
			AddGuardDevicesLogic(ResetGuardDevices, GKStateBit.TurnOff_InAutomatic);
			AddMissedLogic(SetAlarmDevices);
			Formula.Add(FormulaOperationType.END);
		}

		public static int AddSettings(List<Tuple<GKDevice, GKCodeReaderSettingsPart>> deviceAndSettings, FormulaBuilder Formula, GKStateBit commandStateBit)
		{
			var count = 0;

			foreach (var guardDevice in deviceAndSettings.Where(x => !x.Item1.Driver.IsCardReaderOrCodeReader))
			{
				Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Item1);
				if (count > 0)
				{
					Formula.Add(FormulaOperationType.OR);
				}

				if (commandStateBit == GKStateBit.Fire1)
				{
					Formula.AddGetBit(GKStateBit.Fire2, guardDevice.Item1);
					Formula.Add(FormulaOperationType.OR);
					Formula.AddGetBit(GKStateBit.Failure, guardDevice.Item1);
					Formula.Add(FormulaOperationType.OR);
				}
				count++;
			}

			foreach (var guardDevice in deviceAndSettings.Where(x => x.Item1.Driver.IsCardReaderOrCodeReader))
			{
				FormulaHelper.AddCodeReaderLogic(Formula, guardDevice.Item2, guardDevice.Item1);
				if (count > 0)
				{
					Formula.Add(FormulaOperationType.OR);
				}
				count++;
			}

			return count;
		}
		void AddGuardDevicesLogic(List<Tuple<GKDevice, GKCodeReaderSettingsPart>> deviceAndSettings, GKStateBit commandStateBit)
		{
			var count = AddSettings(deviceAndSettings, Formula, commandStateBit);
			switch (commandStateBit)
			{
				case GKStateBit.Fire1:
					if (count > 0)
					{
						if (ChangeGuardDevices.Count > 0)
							Formula.AddGetBit(GKStateBit.On, GuardZone.Pim);
						else
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
							AddSettings(ChangeGuardDevices, Formula, GKStateBit.No);
							if (commandStateBit == GKStateBit.TurnOn_InAutomatic)
							{
								Formula.AddGetBit(GKStateBit.Off, GuardZone);
								Formula.Add(FormulaOperationType.AND);
							}
							if (commandStateBit == GKStateBit.TurnOff_InAutomatic)
							{
								Formula.AddGetBit(GKStateBit.On, GuardZone);
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
								if (!setAlarmDevice.Item1.Driver.IsCardReaderOrCodeReader)
								{
									Formula.AddGetBit(GKStateBit.Fire1, setAlarmDevice.Item1);
									Formula.AddGetBit(GKStateBit.Failure, setAlarmDevice.Item1);
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

		void AddMissedLogic(List<Tuple<GKDevice, GKCodeReaderSettingsPart>> deviceAndSettings)
		{
			if (deviceAndSettings.Count == 0)
				return;
			var count = 0;
			Formula.AddGetBit(GKStateBit.TurningOn, GuardZone);
			Formula.Add(FormulaOperationType.BR, 2, 1);
			Formula.Add(FormulaOperationType.EXIT);
			foreach (var guardDevice in deviceAndSettings)
			{
				if (!guardDevice.Item1.Driver.IsCardReaderOrCodeReader)
				{
					Formula.AddGetBit(GKStateBit.Fire1, guardDevice.Item1);
					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					Formula.AddGetBit(GKStateBit.Failure, guardDevice.Item1);
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