using System;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class DirectionDescriptor : BaseDescriptor
	{
		GKDirection Direction { get; set; }

		public DirectionDescriptor(GKDirection direction)
			: base(direction)
		{
			DescriptorType = DescriptorType.Direction;
			Direction = direction;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x106);
			SetAddress((ushort)0);
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
			if (Direction.Logic.StopClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(Direction.Logic.StopClausesGroup);
				if (Direction.Logic.OnClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.DUP);
				Formula.AddPutBit(GKStateBit.Stop_InManual, Direction);
			}
			if (Direction.Logic.OnClausesGroup.Clauses.Count + Direction.Logic.OnClausesGroup.ClauseGroups.Count > 0)
			{
				if (Direction.Logic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.COM);
				Formula.AddClauseFormula(Direction.Logic.OnClausesGroup);
				if (Direction.Logic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Direction);
				if (Direction.Logic.UseOffCounterLogic)
				{
					Formula.AddClauseFormula(Direction.Logic.OnClausesGroup);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Direction);
				}
			}

			if (!Direction.Logic.UseOffCounterLogic && Direction.Logic.OffClausesGroup.Clauses.Count + Direction.Logic.OffClausesGroup.ClauseGroups.Count > 0)
			{
				Formula.AddClauseFormula(Direction.Logic.OffClausesGroup);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Direction);
			}
			Formula.Add(FormulaOperationType.END);
		}

		void SetPropertiesBytes()
		{
			Parameters = new List<byte>();
			var binProperties = new List<BinProperty>();
			binProperties.Add(new BinProperty()
			{
				No = 0,
				Value = Direction.Delay
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = Direction.Hold
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)Direction.DelayRegime
			});

			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}
}