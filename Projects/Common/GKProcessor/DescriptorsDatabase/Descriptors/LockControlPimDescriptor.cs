using FiresecAPI.GK;

namespace GKProcessor
{
	public class LockControlPimDescriptor : PimDescriptor
	{
		GKDoor Door;

		public LockControlPimDescriptor(GKPim pim, DatabaseType dataBaseType, GKDoor door)
			: base(pim, dataBaseType)
		{
			Door = door;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x107);
			SetAddress((ushort)0);
			SetFormulaBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType);
			Formula.AddGetBit(GKStateBit.Fire1, Door.LockControlDevice, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.Failure, Door.LockControlDevice, DatabaseType);
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}
	}
}