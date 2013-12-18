using System;
using System.Linq;
using System.Collections.Generic;
using XFiresecAPI;
using Common;

namespace GKProcessor
{
	public class PumpStationDescriptor : BaseDescriptor
	{
		public XDelay MainDelay { get; private set; }

		public PumpStationDescriptor(GkDatabase gkDatabase, XPumpStation pumpStation)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.PumpStation;
			PumpStation = pumpStation;

			MainDelay = new XDelay();
			MainDelay.UID = GuidHelper.CreateOn(PumpStation.UID);
			gkDatabase.AddDelay(MainDelay);

			Build();
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

			if (PumpStation.AutomaticOffLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(PumpStation.AutomaticOffLogic);
				Formula.AddPutBit(XStateBit.SetRegime_Manual, PumpStation);
			}

			if (PumpStation.StartLogic.Clauses.Count > 0)
				Formula.AddClauseFormula(PumpStation.StartLogic);
			if (PumpStation.StopLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(PumpStation.StopLogic);
				Formula.Add(FormulaOperationType.DUP);
				Formula.AddGetBit(XStateBit.Norm, PumpStation);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный НС");

				//Formula.AddGetBit(XStateBit.On, MainDelay);
				//Formula.AddGetBit(XStateBit.On, PumpStation);
				//Formula.Add(FormulaOperationType.AND);
				//Formula.Add(FormulaOperationType.OR);

				Formula.AddPutBit(XStateBit.TurnOff_InAutomatic, PumpStation);

				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
			}
			else
			{
				//Formula.AddGetBit(XStateBit.On, MainDelay);
				//Formula.AddGetBit(XStateBit.On, PumpStation);
				//Formula.Add(FormulaOperationType.AND);
				//Formula.AddPutBit(XStateBit.TurnOff_InAutomatic, PumpStation);
			}

			Formula.AddGetBit(XStateBit.Norm, PumpStation);
			Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный НС");
			Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, PumpStation);

            Formula.Add(FormulaOperationType.END);
            FormulaBytes = Formula.GetBytes();
        }

		void SetPropertiesBytes()
		{
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
				Value = PumpStation.Regime
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