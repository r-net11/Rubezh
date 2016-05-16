using System;
using System.Collections.Generic;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class DelayDescriptor : BaseDescriptor
	{
		protected GKDelay Delay { get; set; }

		public DelayDescriptor(GKDelay delay)
			: base(delay)
		{
			DescriptorType = DescriptorType.Delay;
			Delay = delay;
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

			var mirrorParents = Delay.GetMirrorParents();
			Formula.AddMirrorLogic(Delay, mirrorParents);

			if (Delay.Logic.StopClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(Delay.Logic.StopClausesGroup);
				if (Delay.Logic.OnClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.DUP);
				Formula.AddPutBit(GKStateBit.Stop_InManual, Delay);
			}
			if (Delay.Logic.OnClausesGroup.Clauses.Count + Delay.Logic.OnClausesGroup.ClauseGroups.Count > 0)
			{
				if (Delay.Logic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.COM);
				Formula.AddClauseFormula(Delay.Logic.OnClausesGroup);
				if (Delay.Logic.StopClausesGroup.GetObjects().Count > 0)
					Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay);
				if (Delay.Logic.UseOffCounterLogic)
				{
					Formula.AddClauseFormula(Delay.Logic.OnClausesGroup);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Delay);
				}
			}

			if (!Delay.Logic.UseOffCounterLogic && Delay.Logic.OffClausesGroup.GetObjects().Count > 0)
			{
				Formula.AddClauseFormula(Delay.Logic.OffClausesGroup);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Delay);
			}
			Formula.Add(FormulaOperationType.END);
		}

		protected void SetPropertiesBytes()
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