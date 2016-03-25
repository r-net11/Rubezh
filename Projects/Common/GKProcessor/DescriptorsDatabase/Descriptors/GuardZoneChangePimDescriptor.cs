using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class GuardZoneChangePimDescriptor : PimDescriptor
	{
		public GKGuardZone GuardZone { get; private set; }
		readonly IEnumerable<GKGuardZoneDevice> SetGuardDevices;
		readonly IEnumerable<GKGuardZoneDevice> ResetGuardDevices;
		readonly List<GKGuardZoneDevice> ChangeGuardDevices;
		readonly IEnumerable<GKGuardZoneDevice> SetAlarmDevices;

		public GuardZoneChangePimDescriptor(GKGuardZone guardZone)
			: base(guardZone.ChangePim)
		{
			GuardZone = guardZone;
			SetGuardDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.SetGuard, true);
			ResetGuardDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.ResetGuard, true);
			ChangeGuardDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.ChangeGuard, true).ToList();
			ChangeGuardDevices.AddRange(DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.ChangeGuard, false).ToList());
			SetAlarmDevices = DescriptorHelper.GetGuardZoneDevices(GuardZone, GKGuardZoneDeviceActionType.SetAlarm, true);
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x107);
			SetAddress(0);
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}

			Formula.AddGetBit(GKStateBit.Test, GuardZone.ChangePim);
			Formula.Add(FormulaOperationType.COM);
			Formula.AddPutBit(GKStateBit.Test, GuardZone.ChangePim);
			// Фиктивная логика для того, чтобы состояние охранной зоны всегда пересчитывалось, даже когда состояние ПИМ не изменилось
			//Formula.AddGetBit(GKStateBit.Fire1, Pim);
			//Formula.Add(FormulaOperationType.DUP);
			//Formula.AddPutBit(GKStateBit.Reset, Pim);
			//Formula.Add(FormulaOperationType.COM);
			//Formula.AddPutBit(GKStateBit.Fire1, Pim);

			AddGuardDevicesLogic(SetAlarmDevices, GKStateBit.Fire1);
			AddGuardDevicesLogic(SetGuardDevices, GKStateBit.TurnOnNow_InAutomatic);
			AddGuardDevicesLogic(ResetGuardDevices, GKStateBit.TurnOffNow_InAutomatic);

			Formula.Add(FormulaOperationType.END);
		}

		void AddGuardDevicesLogic(IEnumerable<GKGuardZoneDevice> guardDevices, GKStateBit commandStateBit)
		{
			int count = DescriptorHelper.AddCodeReadersLogic(guardDevices, Formula, commandStateBit);
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
						Formula.AddPutBit(GKStateBit.Fire1, GuardZone.ChangePim);
					}
					break;
				case GKStateBit.TurnOnNow_InAutomatic:
				case GKStateBit.TurnOffNow_InAutomatic:
					if (count > 0 || ChangeGuardDevices.Count > 0)
					{
						if (ChangeGuardDevices.Count > 0)
						{
							if (ChangeGuardDevices.Count > 0)
							{
								DescriptorHelper.AddChangeLogic(ChangeGuardDevices, Formula);
								if (commandStateBit == GKStateBit.TurnOnNow_InAutomatic)
								{
									Formula.AddGetBit(GKStateBit.Off, GuardZone);
									Formula.Add(FormulaOperationType.AND);
								}
								if (commandStateBit == GKStateBit.TurnOffNow_InAutomatic)
								{
									Formula.AddGetBit(GKStateBit.On, GuardZone);
									Formula.Add(FormulaOperationType.AND);
								}
							}
							if (count > 0 && ChangeGuardDevices.Count > 0)
							{
								Formula.Add(FormulaOperationType.OR);
							}
						}

						Formula.Add(FormulaOperationType.BR, 1, 3);
						Formula.Add(FormulaOperationType.CONST, 0, 1);
						Formula.AddPutBit(commandStateBit, GuardZone.ChangePim);
						Formula.Add(FormulaOperationType.EXIT);
					}
					break;
			}
		}
	}
}