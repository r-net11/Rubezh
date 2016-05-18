using RubezhAPI.GK;

namespace GKProcessor
{
	public class TurnstileDelayDescriptor : DelayDescriptor
	{
		GKDoor Door { get; set; }
		GKDoorDelayType DoorDelayType { get; set; }

		public TurnstileDelayDescriptor(GKDoor door, GKDelay delay, GKDoorDelayType doorDelayType)
			: base(delay)
		{
			Door = door;
			DoorDelayType = doorDelayType;

			switch (doorDelayType)
			{
				case GKDoorDelayType.LockDelay:
				case GKDoorDelayType.LockDelayExit:
					delay.LinkToDescriptor(door);
					break;
				case GKDoorDelayType.ResetDelay:
					delay.LinkLogic(door.OpenRegimeLogic.OnClausesGroup);
					delay.LinkLogic(door.OpenExitRegimeLogic.OnClausesGroup);
					delay.LinkLogic(door.NormRegimeLogic.OnClausesGroup);
					break;
			}
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

#region Условие всегда открыто на вход (выход) и норма для турникета

			var hasOpenRegimeLogic = Door.OpenRegimeLogic.OnClausesGroup.GetObjects().Count > 0;
			var hasOpenExitRegimeLogic = Door.OpenExitRegimeLogic.OnClausesGroup.GetObjects().Count > 0;
			var hasNormRegimeLogic = Door.NormRegimeLogic.OnClausesGroup.GetObjects().Count > 0;

			if (hasOpenRegimeLogic && (DoorDelayType == GKDoorDelayType.LockDelay || DoorDelayType == GKDoorDelayType.ResetDelay))
			{
				Formula.AddClauseFormula(Door.OpenRegimeLogic.OnClausesGroup);
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay);
				Formula.Add(FormulaOperationType.EXIT);
			}

			if (hasOpenExitRegimeLogic && (DoorDelayType == GKDoorDelayType.LockDelayExit || DoorDelayType == GKDoorDelayType.ResetDelay))
			{
				Formula.AddClauseFormula(Door.OpenExitRegimeLogic.OnClausesGroup);
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay);
				Formula.Add(FormulaOperationType.EXIT);
			}

			if (hasNormRegimeLogic && DoorDelayType == GKDoorDelayType.ResetDelay)
			{
				Formula.AddClauseFormula(Door.NormRegimeLogic.OnClausesGroup);
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay);
				Formula.Add(FormulaOperationType.EXIT);
			}
#endregion
			if (DoorDelayType == GKDoorDelayType.LockDelay || DoorDelayType == GKDoorDelayType.LockDelayExit)
			{
				var device = DoorDelayType == GKDoorDelayType.LockDelay ? Door.EnterDevice : Door.ExitDevice;

				if (device != null)
				{
					Formula.AddGetBit(GKStateBit.Attention, device);
					Formula.Add(FormulaOperationType.BR, 1, 6);
				}

				Formula.AddGetBit(GKStateBit.On, Door);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay);
				Formula.AddGetBit(GKStateBit.TurningOff, Door);
				Formula.AddGetBit(GKStateBit.Off, Door);
				Formula.Add(FormulaOperationType.OR);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Delay);
			}
			Formula.Add(FormulaOperationType.END);
		}
	}
}