using System;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class DirectionDescriptor : BaseDescriptor
	{
		GKDirection Direction { get; set; }

		public DirectionDescriptor(GKDirection direction, DatabaseType dataBaseType)
			: base(direction, dataBaseType)
		{
			DescriptorType = DescriptorType.Direction;
			Direction = direction;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x106);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
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

			if (Direction.Logic.OnClausesGroup.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(Direction.Logic.OnClausesGroup, DatabaseType);
				if (!Direction.Logic.UseOffCounterLogic)
				{
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Direction, DatabaseType);
				}
				else
				{
					Formula.AddStandardTurning(Direction, DatabaseType);
				}
			}
			if (!Direction.Logic.UseOffCounterLogic && Direction.Logic.OffClausesGroup.Clauses.Count + Direction.Logic.OffClausesGroup.ClauseGroups.Count > 0)
			{
				Formula.AddClauseFormula(Direction.Logic.OffClausesGroup, DatabaseType);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Direction, DatabaseType);
			}
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void SetPropertiesBytes()
		{
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