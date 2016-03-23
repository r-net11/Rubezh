using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class GuardZoneChangePimDescriptor : PimDescriptor
	{
		public GKGuardZone PimGuardZone { get; private set; }
		IEnumerable<GKDevice> ChangeGuardDevice { get; set; }

		public GuardZoneChangePimDescriptor(GKGuardZone pimGuardZone)
			: base(pimGuardZone.ChangePim)
		{
			PimGuardZone = pimGuardZone;
			ChangeGuardDevice = pimGuardZone.GuardZoneDevices.FindAll(x => x.ActionType == GKGuardZoneDeviceActionType.ChangeGuard).Select(x=> x.Device);
			ChangeGuardDevice.ForEach(x => Pim.LinkToDescriptor(x));
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

			int count = 0;
			foreach (var device in ChangeGuardDevice)
			{
				Formula.AddGetBit(GKStateBit.Fire1, device);
				if (count > 0)
				{
					Formula.Add(FormulaOperationType.OR);
				}
				count++;
			}
			Formula.Add(FormulaOperationType.BR, 2, 1);
			Formula.Add(FormulaOperationType.EXIT);

			Formula.AddGetBit(GKStateBit.Off, PimGuardZone);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim);

			Formula.AddGetBit(GKStateBit.On, PimGuardZone);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim);

			// Фиктивная логика для того, чтобы состояние охранной зоны всегда пересчитывалось, даже когда состояние ПИМ не изменилось
			Formula.AddGetBit(GKStateBit.Fire1, Pim);
			Formula.Add(FormulaOperationType.DUP);
			Formula.AddPutBit(GKStateBit.Reset, Pim);
			Formula.Add(FormulaOperationType.COM);
			Formula.AddPutBit(GKStateBit.Fire1, Pim);
			Formula.Add(FormulaOperationType.END);
		}
	}
}