using RubezhAPI.GK;

namespace GKProcessor
{
	public class DoorPimDescriptorCrossing : PimDescriptor
	{
		public GKDevice LockControlDevice { get; private set; }
		public GKDevice LockControlDeviceExit { get; private set; }
		public GKDoor PimDoor { get; private set; }

		public DoorPimDescriptorCrossing(GKDoor pimDoor)
			: base(pimDoor.PimCrossing)
		{
			PimDoor = pimDoor;
			LockControlDevice = pimDoor.LockControlDevice;
			LockControlDeviceExit = pimDoor.LockControlDeviceExit;
			if (Pim != null)
			{
				if (LockControlDevice != null)
					Pim.LinkToDescriptor(LockControlDevice);
				if (LockControlDeviceExit != null)
					Pim.LinkToDescriptor(LockControlDeviceExit);
				Pim.LinkToDescriptor(PimDoor);
				Pim.LinkToDescriptor(Pim);
			}
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

			if (LockControlDevice == null || LockControlDeviceExit == null)
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}

			Formula.AddGetBit(GKStateBit.Fire1, LockControlDevice);
			Formula.AddGetBit(GKStateBit.Fire1, LockControlDeviceExit);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim);

			Formula.AddGetBit(GKStateBit.On, Pim);
			Formula.AddGetBit(GKStateBit.Fire1, LockControlDevice);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Fire2, Pim);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.Fire1, Pim);

			Formula.AddGetBit(GKStateBit.On, Pim);
			Formula.AddGetBit(GKStateBit.Fire1, LockControlDeviceExit);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Fire1, Pim);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.Fire2, Pim);

			Formula.AddGetBit(GKStateBit.Fire1, Pim);
			Formula.AddGetBit(GKStateBit.Fire1, LockControlDeviceExit);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Fire2, Pim);
			Formula.AddGetBit(GKStateBit.Fire1, LockControlDevice);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Pim);

			Formula.AddGetBit(GKStateBit.TurningOff, Pim);
			Formula.AddGetBit(GKStateBit.Off, PimDoor);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim);

			Formula.Add(FormulaOperationType.END);
		}
	}
}