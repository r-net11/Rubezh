using RubezhAPI.GK;

namespace GKProcessor
{
	public class BarrierDelayDescriptor : DelayDescriptor
	{
		GKDoor Door { get; set; }
		GKDoorDelayType DoorDelayType { get; set; }

		public BarrierDelayDescriptor(GKDoor door, GKDelay delay, GKDoorDelayType doorDelayType)
			: base(delay)
		{
			Door = door;
			DoorDelayType = doorDelayType;
			delay.LinkToDescriptor(door);
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x101);
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


			if (DoorDelayType == GKDoorDelayType.LockDelay)
			{
				Formula.AddGetBit(GKStateBit.On, Door);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay);
			}

			if (DoorDelayType == GKDoorDelayType.LockDelayExit)
			{
				Formula.AddGetBit(GKStateBit.TurningOff, Door);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay);
			}
		}
	}
}