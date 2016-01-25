using System;
using System.Collections.Generic;
using RubezhAPI.GK;

namespace GKProcessor
{
	public class PumpStationDescriptor : BaseDescriptor
	{
		GKPumpStation PumpStation { get; set; }
		GKPim GlobalPim { get; set; }

		public PumpStationDescriptor(GKPim globalPim, GKPumpStation pumpStation)
			: base(pumpStation)
		{
			GlobalPim = globalPim;
			DescriptorType = DescriptorType.PumpStation;
			PumpStation = pumpStation;
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

			PumpStation.LinkToDescriptor(GlobalPim);
			Formula.AddGetBit(GKStateBit.On, GlobalPim);
			Formula.Add(FormulaOperationType.BR, 2, 1);
			Formula.Add(FormulaOperationType.EXIT);

			var mirrorParents = PumpStation.GetMirrorParents();
			Formula.AddMirrorLogic(PumpStation, mirrorParents);

			var hasAutomaticOffLogic = PumpStation.AutomaticOffLogic.OnClausesGroup.Clauses.Count + PumpStation.AutomaticOffLogic.OnClausesGroup.ClauseGroups.Count > 0;
			var hasStartLogic = PumpStation.StartLogic.OnClausesGroup.Clauses.Count + PumpStation.StartLogic.OnClausesGroup.ClauseGroups.Count > 0;
			var hasStopLogic = PumpStation.StopLogic.OnClausesGroup.Clauses.Count + PumpStation.StopLogic.OnClausesGroup.ClauseGroups.Count > 0;
			if (hasAutomaticOffLogic)
			{
				Formula.AddClauseFormula(PumpStation.AutomaticOffLogic.OnClausesGroup);
				Formula.AddPutBit(GKStateBit.SetRegime_Manual, PumpStation);
			}

			if (hasStartLogic)
			{
				Formula.AddClauseFormula(PumpStation.StartLogic.OnClausesGroup);

				if (hasStopLogic)
				{
					Formula.AddClauseFormula(PumpStation.StopLogic.OnClausesGroup);
					Formula.Add(FormulaOperationType.COM);
					Formula.Add(FormulaOperationType.AND);
				}

				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, PumpStation);
			}
			if (hasStopLogic)
			{
				Formula.AddClauseFormula(PumpStation.StopLogic.OnClausesGroup);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, PumpStation);
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
				Value = PumpStation.Delay
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = PumpStation.Hold
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)PumpStation.DelayRegime
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