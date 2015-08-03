using System;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class DelayDescriptor : BaseDescriptor
	{
		GKDelay Delay { get; set; }

		public DelayDescriptor(GKDelay delay, DatabaseType dataBaseType)
			: base(delay, dataBaseType)
		{
			DescriptorType = DescriptorType.Delay;
			Delay = delay;
			CreateFormula();
		}
		
		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x101);
			SetAddress(0);
			SetPropertiesBytes();

			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula = new FormulaBuilder();
				Formula.Add(FormulaOperationType.END);
			}
			Formula.Resolve(DatabaseType);
			FormulaBytes = Formula.GetBytes();
		}

		void CreateFormula()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				FormulaBytes = Formula.GetBytes();
				return;
			}

			if (Delay.Logic.StopClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(Delay.Logic.StopClausesGroup, DatabaseType);
				if (Delay.Logic.OnClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.DUP);
				Formula.AddPutBit(GKStateBit.Stop_InManual, Delay, DatabaseType);
			}
			if (Delay.Logic.OnClausesGroup.Clauses.Count + Delay.Logic.OnClausesGroup.ClauseGroups.Count > 0)
			{
				if (Delay.Logic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.COM);
				Formula.AddClauseFormula(Delay.Logic.OnClausesGroup, DatabaseType);
				if (Delay.Logic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay, DatabaseType);
				if (Delay.Logic.UseOffCounterLogic)
				{
					Formula.AddClauseFormula(Delay.Logic.OnClausesGroup, DatabaseType);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Delay, DatabaseType);
				}
			}

			if (Delay.Logic.OffClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(Delay.Logic.OffClausesGroup, DatabaseType);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Delay, DatabaseType);
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
				Value = Delay.DelayTime
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = Delay.Hold
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)Delay.DelayRegime
			});

			Parameters = new List<byte>();
			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}
}