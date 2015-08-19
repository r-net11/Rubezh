using FiresecAPI.GK;

namespace GKProcessor
{
	public class DoorPimDescriptorEnter : PimDescriptor
	{
		public GKDevice EnterDevice { get; private set; }
		public GKDevice ExitDevice { get; private set; }
		public GKDevice EnterButton { get; private set; }
		public GKDevice ExitButton { get; private set; }
		public GKDoor PimDoor { get; private set; }

		public DoorPimDescriptorEnter(GKDoor pimDoor, DatabaseType databaseType) : base(pimDoor.PimEnter, databaseType)
		{
			PimDoor = pimDoor;
			EnterDevice = pimDoor.EnterDevice;
			ExitDevice = pimDoor.ExitDevice;
			EnterButton = pimDoor.EnterButton;
			ExitButton = pimDoor.ExitButton;
			if (Pim != null)
			{
				if (EnterDevice!= null)
				Pim.LinkGKBases(EnterDevice);
				if (ExitDevice!=null)
				Pim.LinkGKBases(ExitDevice);
				if (EnterButton!=null)
				Pim.LinkGKBases(EnterButton);
				if (ExitButton!=null)
				Pim.LinkGKBases(ExitButton);
				Pim.LinkGKBases(PimDoor);
				Pim.LinkGKBases(Pim);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x107);
			SetAddress(0);
			SetFormulaBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}
			if (ExitButton!=null)
			Formula.AddGetBit(GKStateBit.Fire1, ExitButton);
			if (EnterDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Attention, EnterDevice);
				Formula.Add(FormulaOperationType.OR);
			}
			Formula.AddGetBit(GKStateBit.On, PimDoor);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim);
			if (EnterButton != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, EnterButton);
			}
			if (ExitDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Attention, ExitDevice);
				Formula.Add(FormulaOperationType.OR);
			}
			Formula.AddGetBit(GKStateBit.On, PimDoor);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Off, PimDoor);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim);
			Formula.Add(FormulaOperationType.END);
		}
	}

	public class DoorPimDescriptorExit : PimDescriptor
	{
		public GKDevice EnterDevice { get; private set; }
		public GKDevice ExitDevice { get; private set; }
		public GKDevice EnterButton { get; private set; }
		public GKDevice ExitButton { get; private set; }
		public GKDoor PimDoor { get; private set; }

		public DoorPimDescriptorExit(GKDoor pimDoor, DatabaseType databaseType) : base(pimDoor.PimExit, databaseType)
		{
			PimDoor = pimDoor;
			EnterDevice = pimDoor.EnterDevice;
			ExitDevice = pimDoor.ExitDevice;
			EnterButton = pimDoor.EnterButton;
			ExitButton = pimDoor.ExitButton;
			if (Pim != null)
			{
				if (EnterDevice!=null)
				Pim.LinkGKBases(EnterDevice);
				if (ExitDevice!=null)
				Pim.LinkGKBases(ExitDevice);
				if (EnterButton!=null)
				Pim.LinkGKBases(EnterButton);
				if (ExitButton!=null)
				Pim.LinkGKBases(ExitButton);
				Pim.LinkGKBases(PimDoor);
				Pim.LinkGKBases(Pim);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x107);
			SetAddress(0);
			SetFormulaBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}
			if (ExitButton != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, ExitButton);
			}
			if (EnterDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Attention, EnterDevice);
				Formula.Add(FormulaOperationType.OR);
			}
			Formula.AddGetBit(GKStateBit.On, PimDoor);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Off, PimDoor);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim);
			if (EnterButton != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, EnterButton);
			}
			if (EnterButton != null)
			{
				Formula.AddGetBit(GKStateBit.Attention, EnterButton);
				Formula.Add(FormulaOperationType.OR);
			}
			Formula.AddGetBit(GKStateBit.On, PimDoor);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim);
			Formula.Add(FormulaOperationType.END);
		}
	}
}