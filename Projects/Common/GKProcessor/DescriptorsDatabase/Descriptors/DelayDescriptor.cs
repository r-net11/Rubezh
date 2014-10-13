using System;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class DelayDescriptor : BaseDescriptor
	{
		public DelayDescriptor(GKDelay delay)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.Delay;
			Delay = delay;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x101);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if (Delay.DeviceLogic.ClausesGroup.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(Delay.DeviceLogic.ClausesGroup);
				if (Delay.DeviceLogic.OffClausesGroup.Clauses.Count == 0)
				{
					Formula.AddStandardTurning(Delay);
				}
				else
				{
					Formula.AddGetBit(GKStateBit.Norm, Delay);
					Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Задержки");
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay);
				}
			}
			if (Delay.DeviceLogic.OffClausesGroup.Clauses.Count > 0)
			{
				Formula.AddClauseFormula(Delay.DeviceLogic.OffClausesGroup);
				Formula.AddGetBit(GKStateBit.Norm, Delay);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Задержки");
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Delay);
			}
			FormulaBytes = Formula.GetBytes();
		}

		void SetPropertiesBytes()
		{
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