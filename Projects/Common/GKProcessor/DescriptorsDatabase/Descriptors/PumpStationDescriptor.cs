using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class PumpStationDescriptor : BaseDescriptor
	{
		GKPumpStation PumpStation { get; set; }

		public PumpStationDescriptor(CommonDatabase database, GKPumpStation pumpStation, DatabaseType dataBaseType)
			: base(pumpStation, dataBaseType)
		{
			DescriptorType = DescriptorType.PumpStation;
			PumpStation = pumpStation;
			database.AddDelay(pumpStation.MainDelay);
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

			var hasAutomaticOffLogic = PumpStation.AutomaticOffLogic.OnClausesGroup.Clauses.Count + PumpStation.AutomaticOffLogic.OnClausesGroup.ClauseGroups.Count > 0;
			var hasStartLogic = PumpStation.StartLogic.OnClausesGroup.Clauses.Count + PumpStation.StartLogic.OnClausesGroup.ClauseGroups.Count > 0;
			var hasStopLogic = PumpStation.StopLogic.OnClausesGroup.Clauses.Count + PumpStation.StopLogic.OnClausesGroup.ClauseGroups.Count > 0;
			if (hasAutomaticOffLogic)
			{
				Formula.Add(FormulaOperationType.DUP);
				Formula.AddClauseFormula(PumpStation.AutomaticOffLogic.OnClausesGroup, DatabaseType);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.SetRegime_Manual, PumpStation, DatabaseType);
			}

			if (hasStartLogic)
			{
				Formula.AddClauseFormula(PumpStation.StartLogic.OnClausesGroup, DatabaseType);
				//Formula.AddGetBit(GKStateBit.On, MainDelay);
				//Formula.Add(FormulaOperationType.AND);

				if (hasStopLogic)
				{
					Formula.AddClauseFormula(PumpStation.StopLogic.OnClausesGroup, DatabaseType);
					Formula.Add(FormulaOperationType.COM);
					Formula.Add(FormulaOperationType.AND);
				}

				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, PumpStation, DatabaseType);
			}
			if (hasStopLogic)
			{
				Formula.AddClauseFormula(PumpStation.StopLogic.OnClausesGroup, DatabaseType);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, PumpStation, DatabaseType);
			}

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
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