using System;
using System.Linq;
using System.Collections.Generic;
using XFiresecAPI;

namespace GKProcessor
{
	public class PumpStationDescriptor : BaseDescriptor
	{
		public PumpStationDescriptor(XPumpStation pumpStation)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.PumpStation;
			PumpStation = pumpStation;
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

			if (PumpStation.StartLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(PumpStation.StartLogic);
			}
			Formula.AddGetBit(XStateBit.Norm, PumpStation);
			Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный НС");
			Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, PumpStation);

			if (PumpStation.ForbidLogic.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(PumpStation.ForbidLogic);
			}
			Formula.AddPutBit(XStateBit.SetRegime_Manual, PumpStation);

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