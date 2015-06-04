﻿using System;
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
			if ((DatabaseType == DatabaseType.Gk && GKBase.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !GKBase.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				FormulaBytes = Formula.GetBytes();
				return;
			}

			if (Delay.Logic.OnClausesGroup.Clauses.Count + Delay.Logic.OnClausesGroup.ClauseGroups.Count > 0)
			{
				Formula.AddClauseFormula(Delay.Logic.OnClausesGroup, DatabaseType);
				if (!Delay.Logic.UseOffCounterLogic)
				{
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Delay, DatabaseType);
				}
				else
				{
					Formula.AddStandardTurning(Delay, DatabaseType);
				}
			}
			if (!Delay.Logic.UseOffCounterLogic && Delay.Logic.OffClausesGroup.Clauses.Count + Delay.Logic.OffClausesGroup.ClauseGroups.Count > 0)
			{
				Formula.AddClauseFormula(Delay.Logic.OffClausesGroup, DatabaseType);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Delay, DatabaseType);
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