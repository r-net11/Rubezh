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
				Pim.LinkGKBases(EnterDevice);
				Pim.LinkGKBases(ExitDevice);
				Pim.LinkGKBases(EnterButton);
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
				FormulaBytes = Formula.GetBytes();
				return;
			}

			Formula.AddGetBit(GKStateBit.Fire1, ExitButton, DatabaseType);
			Formula.AddGetBit(GKStateBit.Attention, EnterDevice, DatabaseType);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddGetBit(GKStateBit.On, PimDoor, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim, DatabaseType);
			Formula.AddGetBit(GKStateBit.Fire1, EnterButton, DatabaseType);
			Formula.AddGetBit(GKStateBit.Attention, ExitDevice, DatabaseType);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddGetBit(GKStateBit.On, PimDoor, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Off, PimDoor, DatabaseType);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim, DatabaseType);
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
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
				Pim.LinkGKBases(EnterDevice);
				Pim.LinkGKBases(ExitDevice);
				Pim.LinkGKBases(EnterButton);
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
				FormulaBytes = Formula.GetBytes();
				return;
			}

			Formula.AddGetBit(GKStateBit.Fire1, ExitButton, DatabaseType);
			Formula.AddGetBit(GKStateBit.Attention, EnterDevice, DatabaseType);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddGetBit(GKStateBit.On, PimDoor, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Off, PimDoor, DatabaseType);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Pim, DatabaseType);
			Formula.AddGetBit(GKStateBit.Fire1, EnterButton, DatabaseType);
			Formula.AddGetBit(GKStateBit.Attention, ExitDevice, DatabaseType);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddGetBit(GKStateBit.On, PimDoor, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Pim, DatabaseType);
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}
	}
}