using RubezhAPI.GK;

namespace GKProcessor
{
	public class DoorPimDescriptorEnter : PimDescriptor
	{
		public GKDevice EnterDevice { get; private set; }
		public GKDevice ExitDevice { get; private set; }
		public GKDevice EnterButton { get; private set; }
		public GKDevice ExitButton { get; private set; }
		public GKDoor PimDoor { get; private set; }

		public DoorPimDescriptorEnter(GKDoor pimDoor)
			: base(pimDoor.PimEnter)
		{
			PimDoor = pimDoor;
			EnterDevice = pimDoor.EnterDevice;
			ExitDevice = pimDoor.ExitDevice;
			EnterButton = pimDoor.EnterButton;
			ExitButton = pimDoor.ExitButton;
			if (Pim != null)
			{
				if (EnterDevice != null)
					Pim.LinkToDescriptor(EnterDevice);
				if (ExitDevice != null)
					Pim.LinkToDescriptor(ExitDevice);
				if (EnterButton != null)
					Pim.LinkToDescriptor(EnterButton);
				if (ExitButton != null)
					Pim.LinkToDescriptor(ExitButton);
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
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) ||
			    (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}
			if (ExitButton != null)
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
}