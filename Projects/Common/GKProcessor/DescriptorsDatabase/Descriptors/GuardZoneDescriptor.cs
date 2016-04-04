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
		public GuardZoneChangePimDescriptor GuardZoneChangePimDescriptor { get; private set; }
		readonly IEnumerable<GKDevice> SetGuardDevices;
		readonly IEnumerable<GKDevice> ResetGuardDevices;
		readonly IEnumerable<GKDevice> SetAlarmDevices;

		readonly IEnumerable<GKGuardZoneDevice> SetGuardPimDevices;
		readonly IEnumerable<GKGuardZoneDevice> ResetGuardPimDevices;
		readonly List<GKGuardZoneDevice> ChangeGuardPimDevices;
		readonly IEnumerable<GKGuardZoneDevice> SetAlarmPimDevices;

		public GuardZoneDescriptor(GKGuardZone zone)
			: base(zone)
		{
			DescriptorType = DescriptorType.GuardZone;
			GuardZone = zone;
			SetGuardDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.SetGuard, false).Select(x => x.Device);
			ResetGuardDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.ResetGuard, false).Select(x => x.Device);
			SetAlarmDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.SetAlarm, false).Select(x => x.Device);

			SetGuardPimDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.SetGuard, true);
			ResetGuardPimDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.ResetGuard, true);
			ChangeGuardPimDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.ChangeGuard, true).ToList();
			ChangeGuardPimDevices.AddRange(DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.ChangeGuard, false).ToList());
			SetAlarmPimDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.SetAlarm, true);

			if (GuardZone.GuardZoneDevices.FindAll(x => x.ActionType == GKGuardZoneDeviceActionType.ChangeGuard || x.Device.Driver.IsCardReaderOrCodeReader).Count > 0)
			{
				GuardZonePimDescriptor = new GuardZonePimDescriptor(GuardZone);
				GuardZoneChangePimDescriptor = new GuardZoneChangePimDescriptor(GuardZone);
				GuardZone.LinkToDescriptor(GuardZone.ChangePim);
			}
			else
				GuardZone.LinkToDescriptor(GuardZone);
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

			AddGuardDevicesLogic(SetAlarmDevices, GKStateBit.Fire1);
			AddGuardDevicesLogic(ResetGuardDevices, GKStateBit.TurnOff_InAutomatic);
			AddGuardDevicesLogic(SetGuardDevices, GKStateBit.TurnOn_InAutomatic);
			Formula.Add(FormulaOperationType.END);
		}

		void AddGuardDevicesLogic(IEnumerable<GKDevice> devices, GKStateBit commandStateBit)
		{
			var count = DescriptorHelper.AddFireOrFailureLogic(devices, Formula);
			switch (commandStateBit)
			{
				case GKStateBit.Fire1:
					if (count > 0 || SetAlarmPimDevices.Any())
					{
						if (SetAlarmPimDevices.Any())
							Formula.AddGetBit(GKStateBit.On, GuardZone.Pim);
						else
							Formula.AddGetBit(GKStateBit.Attention, GuardZone);

						if (count > 0)
							Formula.Add(FormulaOperationType.OR);

						if (SetAlarmPimDevices.Any())
						{
							Formula.AddGetBit(GKStateBit.Fire1, GuardZone.ChangePim);
							int count2 = DescriptorHelper.AddCodeReadersLogic(SetAlarmPimDevices, Formula, commandStateBit);
							if (count2 > 0)
								Formula.Add(FormulaOperationType.AND);
							if (count > 0)
							Formula.Add(FormulaOperationType.OR);
						}
						Formula.AddPutBit(GKStateBit.Attention, GuardZone);
					}
					break;
				case GKStateBit.TurnOn_InAutomatic:
				case GKStateBit.TurnOff_InAutomatic:
					var useChangePimLogic = ChangeGuardPimDevices.Count > 0
						|| (commandStateBit == GKStateBit.TurnOn_InAutomatic && SetGuardPimDevices.Any())
						|| (commandStateBit == GKStateBit.TurnOff_InAutomatic && ResetGuardPimDevices.Any());
					if (count > 0 || useChangePimLogic)
					{
						if (useChangePimLogic)
						{
							DescriptorHelper.AddChangeLogic(ChangeGuardPimDevices, Formula);
							int count2 = 0;
							if (commandStateBit == GKStateBit.TurnOn_InAutomatic)
							{
								count2 = DescriptorHelper.AddCodeReadersLogic(SetGuardPimDevices, Formula, commandStateBit);
								if (ChangeGuardPimDevices.Count > 0 && count2 > 0)
									Formula.Add(FormulaOperationType.OR);
								Formula.AddGetBit(GKStateBit.On, GuardZone.ChangePim);
							}
							else
							{
								count2 = DescriptorHelper.AddCodeReadersLogic(ResetGuardPimDevices, Formula, commandStateBit);
								if (ChangeGuardPimDevices.Count > 0 && count2 > 0)
									Formula.Add(FormulaOperationType.OR);
								Formula.AddGetBit(GKStateBit.Off, GuardZone.ChangePim);
							}
							Formula.Add(FormulaOperationType.AND);

							if (count > 0)
								Formula.Add(FormulaOperationType.OR);
						}
						if (commandStateBit == GKStateBit.TurnOn_InAutomatic)
							AddMissedLogic();
						Formula.Add(FormulaOperationType.BR, 1, 3);
						Formula.Add(FormulaOperationType.CONST, 0, 1);
						Formula.AddPutBit(commandStateBit, GuardZone);
						Formula.Add(FormulaOperationType.EXIT);
					}
					break;
			}
		}

		void AddMissedLogic()
		{
			foreach (var setAlarmDevice in SetAlarmDevices)
			{
				Formula.AddGetBit(GKStateBit.Fire1, setAlarmDevice);
				Formula.AddGetBit(GKStateBit.Fire2, setAlarmDevice);
				Formula.Add(FormulaOperationType.OR);
				Formula.AddGetBit(GKStateBit.Failure, setAlarmDevice);
				Formula.Add(FormulaOperationType.OR);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
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