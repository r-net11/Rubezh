using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

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
			DeviceType = BytesHelper.ShortToBytes(0x104);
			SetAddress(0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();

			var hasOpenRegimeLogic = Door.OpenRegimeLogic.OnClausesGroup.Clauses.Count + Door.OpenRegimeLogic.OnClausesGroup.ClauseGroups.Count > 0;
			if (hasOpenRegimeLogic)
			{
				Formula.AddClauseFormula(Door.OpenRegimeLogic.OnClausesGroup, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.BR, 1, 7);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);
				Formula.AddPutBit(GKStateBit.TurnOn_InManual, Door, DatabaseType.Gk);
				Formula.AddPutBit(GKStateBit.SetRegime_Manual, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.EXIT);
			}

			var hasCloseRegimeLogic = Door.CloseRegimeLogic.OnClausesGroup.Clauses.Count + Door.CloseRegimeLogic.OnClausesGroup.ClauseGroups.Count > 0;
			if (hasCloseRegimeLogic)
			{
				Formula.AddClauseFormula(Door.CloseRegimeLogic.OnClausesGroup, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.BR, 1, 7);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Door, DatabaseType.Gk);
				Formula.AddPutBit(GKStateBit.TurnOff_InManual, Door, DatabaseType.Gk);
				Formula.AddPutBit(GKStateBit.SetRegime_Manual, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.EXIT);
			}

			var hasNormRegimeLogic = Door.NormRegimeLogic.OnClausesGroup.Clauses.Count + Door.NormRegimeLogic.OnClausesGroup.ClauseGroups.Count > 0;
			if (hasNormRegimeLogic)
			{
				Formula.AddClauseFormula(Door.NormRegimeLogic.OnClausesGroup, DatabaseType.Gk);
				Formula.AddGetBit(GKStateBit.Norm, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.Add(FormulaOperationType.BR, 1, 7);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Door, DatabaseType.Gk);
				Formula.AddPutBit(GKStateBit.TurnOff_InManual, Door, DatabaseType.Gk);
				Formula.AddPutBit(GKStateBit.SetRegime_Automatic, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.EXIT);
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

			if (Door.EnterDevice != null || Door.ExitDevice != null)
			{
				if (Door.EnterDevice != null)
					TurnOnDoorBuilder(true);
				if (Door.ExitDevice != null)
					TurnOnDoorBuilder(false);
			}

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void TurnOnDoorBuilder(bool enterDevice)
		{
			var device = enterDevice ? Door.EnterDevice : Door.ExitDevice;
			if (device.DriverType == GKDriverType.RSR2_CodeReader || device.DriverType == GKDriverType.RSR2_CardReader)
			{
				int operationCount = 6;
				var zone1 = GKManager.SKDZones.FirstOrDefault(x => x.UID == (enterDevice ? Door.ExitZoneUID : Door.EnterZoneUID));
				var zone2 = GKManager.SKDZones.FirstOrDefault(x => x.UID == (enterDevice ? Door.EnterZoneUID : Door.ExitZoneUID));

				if (zone1 != null)
					operationCount++;
				if (zone2 != null)
					operationCount += 2;
				Formula.AddGetBit(GKStateBit.Attention, device, DatabaseType.Gk);
				Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.AND);
				Formula.Add(FormulaOperationType.BR, 1, (byte) operationCount);

				if (!GlobalSettingsHelper.GlobalSettings.AntipassbackOn)
					Formula.Add(FormulaOperationType.ACS, (byte) Door.EnterLevel, device.GKDescriptorNo);
				if (GlobalSettingsHelper.GlobalSettings.AntipassbackOn)
				{
					Formula.Add(FormulaOperationType.ACSP, (byte) Door.EnterLevel, device.GKDescriptorNo);
					Formula.Add(FormulaOperationType.BR, 1, (byte) (operationCount - 2));
					if (zone1 != null)
					{
						Formula.Add(FormulaOperationType.TSTP, 0, (byte) zone1.No);
						Formula.Add(FormulaOperationType.BR, 1, (byte)(operationCount - 4));
					}
					if (zone2 != null)
					{
						Formula.Add(FormulaOperationType.CONST, 0, (byte) zone2.No);
						Formula.Add(FormulaOperationType.PUTP);
						Formula.Add(FormulaOperationType.CONST, 0, 1);
					}
				}
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

		void SetPropertiesBytes()
		{
			var binProperties = new List<BinProperty>();
			binProperties.Add(new BinProperty
			{
				No = 0,
				Value = (ushort)(Door.Delay)
			});
			binProperties.Add(new BinProperty
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