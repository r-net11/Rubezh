﻿using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace GKProcessor
{
	public class DoorDescriptor : BaseDescriptor
	{
		GKDoor Door { get; set; }

		public DoorDescriptor(GKDoor door)
			: base(door, DatabaseType.Gk)
		{
			DescriptorType = DescriptorType.Door;
			Door = door;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x104);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if (Door.EnterDevice != null || Door.ExitDevice != null)
			{
				if (Door.EnterDevice != null)
				{
					Formula.AddGetBit(GKStateBit.Attention, Door.EnterDevice, DatabaseType.Gk);
					Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
					Formula.Add(FormulaOperationType.AND);
					Formula.Add(FormulaOperationType.BR, 1, 3);
					Formula.Add(FormulaOperationType.ACS, (byte)Door.EnterLevel, (ushort)Door.EnterDevice.GKDescriptorNo);
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);
					Formula.Add(FormulaOperationType.EXIT);
				}
				if (Door.ExitDevice != null)
				{
					if (Door.ExitDevice.DriverType == GKDriverType.RSR2_CodeReader || Door.ExitDevice.DriverType == GKDriverType.RSR2_CardReader)
					{
						Formula.AddGetBit(GKStateBit.Attention, Door.ExitDevice, DatabaseType.Gk);
						Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
						Formula.Add(FormulaOperationType.AND);
						Formula.Add(FormulaOperationType.BR, 1, 3);
						Formula.Add(FormulaOperationType.ACS, (byte)Door.EnterLevel, (ushort)Door.ExitDevice.GKDescriptorNo);
						Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);
						Formula.Add(FormulaOperationType.EXIT);
					}
					else
					{
						Formula.AddGetBit(GKStateBit.Fire1, Door.ExitDevice, DatabaseType.Gk);
						Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
						Formula.Add(FormulaOperationType.AND);
						Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);
					}
				}
			}

			if (Door.LockControlDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
				Formula.AddGetBit(GKStateBit.Fire1, Door.LockControlDevice, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddGetBit(GKStateBit.Fire1, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.OR);
				Formula.AddPutBit(GKStateBit.Fire1, Door, DatabaseType.Gk);
			}

			var hasOpenRegimeLogic = Door.OpenRegimeLogic.OnClausesGroup.Clauses.Count + Door.OpenRegimeLogic.OnClausesGroup.ClauseGroups.Count > 0;
			if (hasOpenRegimeLogic)
			{
				Formula.AddClauseFormula(Door.OpenRegimeLogic.OnClausesGroup, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.BR, 1, 7);
				Formula.AddGetBit(GKStateBit.On, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.DUP);
				Formula.Add(FormulaOperationType.DUP);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);
				Formula.AddPutBit(GKStateBit.TurnOn_InManual, Door, DatabaseType.Gk);
				Formula.AddPutBit(GKStateBit.SetRegime_Manual, Door, DatabaseType.Gk);
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
				Value = (ushort)(Door.Delay)
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = (ushort)(Door.Hold)
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