using RubezhAPI.GK;

namespace GKProcessor
{
	public class GuardZonePimDescriptor : PimDescriptor
	{

		public GKGuardZone PimGuardZone { get; private set; }

		public GuardZonePimDescriptor(GKGuardZone pimGuardZone)
			: base(pimGuardZone.Pim)
		{
			PimGuardZone = pimGuardZone;
			Pim.LinkToDescriptor(PimGuardZone);
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

			Formula.AddGetBit(GKStateBit.Attention, PimGuardZone);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Pim);
			Formula.AddGetBit(GKStateBit.Fire1, PimGuardZone);
			Formula.AddGetBit(GKStateBit.Fire2, PimGuardZone);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim);
			Formula.AddGetBit(GKStateBit.Attention, PimGuardZone);
			Formula.AddGetBit(GKStateBit.Fire1, PimGuardZone);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddGetBit(GKStateBit.Fire2, PimGuardZone);
			Formula.Add(FormulaOperationType.OR);
			Formula.Add(FormulaOperationType.COM);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim);
			Formula.Add(FormulaOperationType.END);
		}
	}
}