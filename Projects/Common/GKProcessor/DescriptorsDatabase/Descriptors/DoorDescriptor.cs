using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public class DoorDescriptor : BaseDescriptor
	{
		GKDoor Door { get; set; }
		public DoorPimDescriptorEnter DoorPimDescriptorEnter { get; private set; }
		public DoorPimDescriptorExit DoorPimDescriptorExit { get; private set; }

		public DoorDescriptor(GKDoor door) : base(door, DatabaseType.Gk)
		{
			DescriptorType = DescriptorType.Door;
			Door = door;
			if (Door.DoorType == GKDoorType.AirlockBooth)
			{
				DoorPimDescriptorEnter = new DoorPimDescriptorEnter(Door, DatabaseType);
				DoorPimDescriptorExit = new DoorPimDescriptorExit(Door, DatabaseType);
			}
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

			switch (Door.DoorType)
			{
				case GKDoorType.OneWay:
				case GKDoorType.TwoWay:
				case GKDoorType.Turnstile:
					SetFormulaDoor();
					break;
				case GKDoorType.Barrier:
					SetFormulaBarrier();
					break;
				case GKDoorType.AirlockBooth:
					SetFormulaAirlockBooth();
					break;
			}

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void SetFormulaBarrier()
		{
			if (Door.EnterDevice != null || Door.ExitDevice != null)
			{
				if (Door.EnterDevice != null)
					TurnOnDoorBuilder(true);
				if (Door.ExitDevice != null)
					TurnOnDoorBuilder(false);
			}
		}

		void SetFormulaDoor()
		{
			if (Door.LockControlDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
				Formula.AddGetBit(GKStateBit.Fire1, Door.LockControlDevice, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddGetBit(GKStateBit.Fire1, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.OR);
				Formula.AddPutBit(GKStateBit.Fire1, Door, DatabaseType.Gk);
				if (Door.AntipassbackOn)
				{
					Formula.AddGetBit(GKStateBit.On, Door, DatabaseType.Gk);
					Formula.AddGetBit(GKStateBit.Fire1, Door.LockControlDevice, DatabaseType.Gk);
					Formula.Add(FormulaOperationType.AND);
					Formula.Add(FormulaOperationType.BR, 1, 2);
					Formula.Add(FormulaOperationType.GETMEMB, 0, (byte)Door.GKDescriptorNo);
					Formula.Add(FormulaOperationType.PUTP);
				}
			}

			if (Door.EnterDevice != null || Door.ExitDevice != null)
			{
				if (Door.EnterDevice != null)
					TurnOnDoorBuilder(true);
				if (Door.ExitDevice != null)
					TurnOnDoorBuilder(false);
			}
		}

		void SetFormulaAirlockBooth()
		{
			var lockControlDevice = Door.LockControlDevice;
			var lockControlDeviceExit = Door.LockControlDeviceExit;
			var enterDevice = Door.EnterDevice;
			var exitDevice = Door.ExitDevice;
			var enterButton = Door.EnterButton;
			var exitButton = Door.ExitButton;
			var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Door.EnterZoneUID);
			var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Door.ExitZoneUID);

			Formula.AddGetBit(GKStateBit.Fire1, lockControlDevice, DatabaseType.Gk);
			Formula.AddGetBit(GKStateBit.Fire1, Door.PimEnter, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Fire1, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.Fire1, Door, DatabaseType.Gk);

			Formula.AddGetBit(GKStateBit.Fire1, lockControlDeviceExit, DatabaseType.Gk);
			Formula.AddGetBit(GKStateBit.Fire2, Door.PimExit, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddGetBit(GKStateBit.Fire1, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.Fire1, Door, DatabaseType.Gk);

			Formula.AddGetBit(GKStateBit.Fire1, lockControlDevice, DatabaseType.Gk);
			Formula.AddGetBit(GKStateBit.On, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.AND);
			Formula.Add(FormulaOperationType.BR, 1, 5);
			Formula.Add(FormulaOperationType.GETMEMB, 0, (byte)Door.GKDescriptorNo);
			Formula.Add(FormulaOperationType.BR, 1, 0);
			Formula.Add(FormulaOperationType.CONST, 0, (byte)exitZone.No);
			Formula.Add(FormulaOperationType.PUTP);
			Formula.Add(FormulaOperationType.EXIT);

			Formula.AddGetBit(GKStateBit.Fire1, lockControlDeviceExit, DatabaseType.Gk);
			Formula.AddGetBit(GKStateBit.On, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.AND);
			Formula.Add(FormulaOperationType.BR, 1, 5);
			Formula.Add(FormulaOperationType.GETMEMB, 0, (byte)Door.GKDescriptorNo);
			Formula.Add(FormulaOperationType.BR, 1, 0);
			Formula.Add(FormulaOperationType.CONST, 0, (byte)enterZone.No);
			Formula.Add(FormulaOperationType.PUTP);
			Formula.Add(FormulaOperationType.EXIT);

			Formula.AddGetBit(GKStateBit.Attention, enterDevice, DatabaseType.Gk);
			Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.AND);
			Formula.Add(FormulaOperationType.BR, 1, 9);
			Formula.Add(FormulaOperationType.ACSP, (byte)Door.EnterLevel, enterDevice.GKDescriptorNo);
			Formula.Add(FormulaOperationType.BR, 1, 6);
			Formula.Add(FormulaOperationType.TSTP, 0, (byte)exitZone.No);
			Formula.Add(FormulaOperationType.BR, 1, 4);
			Formula.Add(FormulaOperationType.CONST, 0, 0);
			Formula.Add(FormulaOperationType.PUTMEMB, 0, (byte)Door.GKDescriptorNo);
			Formula.Add(FormulaOperationType.CONST, 0, 1);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.EXIT);

			Formula.AddGetBit(GKStateBit.Fire1, enterButton, DatabaseType.Gk);
			Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.AND);
			Formula.Add(FormulaOperationType.BR, 1, 3);
			Formula.Add(FormulaOperationType.CONST, 0, 1);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.EXIT);

			Formula.AddGetBit(GKStateBit.Attention, exitDevice, DatabaseType.Gk);
			Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.AND);
			Formula.Add(FormulaOperationType.BR, 1, 9);
			Formula.Add(FormulaOperationType.ACSP, (byte)Door.EnterLevel, exitDevice.GKDescriptorNo);
			Formula.Add(FormulaOperationType.BR, 1, 6);
			Formula.Add(FormulaOperationType.TSTP, 0, (byte)enterZone.No);
			Formula.Add(FormulaOperationType.BR, 1, 4);
			Formula.Add(FormulaOperationType.CONST, 0, 0);
			Formula.Add(FormulaOperationType.PUTMEMB, 0, (byte)Door.GKDescriptorNo);
			Formula.Add(FormulaOperationType.CONST, 0, 1);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.EXIT);

			Formula.AddGetBit(GKStateBit.Fire1, exitButton, DatabaseType.Gk);
			Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door, DatabaseType.Gk);

			if (Door.PimEnter != null)
			{
				Door.LinkGKBases(Door.PimEnter);
			}

			if (Door.PimExit != null)
			{
				Door.LinkGKBases(Door.PimExit);
			}
		}

		void TurnOnDoorBuilder(bool enterDevice)
		{
			var device = enterDevice ? Door.EnterDevice : Door.ExitDevice;
			if (device.DriverType == GKDriverType.RSR2_CodeReader || device.DriverType == GKDriverType.RSR2_CardReader)
			{
				int operationCount = 4;
				var zone1 = GKManager.SKDZones.FirstOrDefault(x => x.UID == (enterDevice ? Door.ExitZoneUID : Door.EnterZoneUID));
				var zone2 = GKManager.SKDZones.FirstOrDefault(x => x.UID == (enterDevice ? Door.EnterZoneUID : Door.ExitZoneUID));

				if (zone1 != null)
					operationCount += 2;
				if (zone2 != null)
					operationCount += 3;
				if (!Door.AntipassbackOn)
					operationCount = 3;
				Formula.AddGetBit(GKStateBit.Attention, device, DatabaseType.Gk);
				Formula.AddGetBit(GKStateBit.Off, Door, DatabaseType.Gk);
				Formula.Add(FormulaOperationType.AND);
				Formula.Add(FormulaOperationType.BR, 1, (byte) operationCount);

				if (!Door.AntipassbackOn)
					Formula.Add(FormulaOperationType.ACS, (byte) Door.EnterLevel, device.GKDescriptorNo);
				if (Door.AntipassbackOn)
				{
					Formula.Add(FormulaOperationType.ACSP, (byte) Door.EnterLevel, device.GKDescriptorNo);
					Formula.Add(FormulaOperationType.BR, 1, (byte) (operationCount - 3));
					if (zone1 != null)
					{
						Formula.Add(FormulaOperationType.TSTP, 0, (byte) zone1.No);
						Formula.Add(FormulaOperationType.BR, 1, (byte)(operationCount - 5));
					}
					if (zone2 != null)
					{
						Formula.Add(FormulaOperationType.CONST, 0, (byte) zone2.No);
						Formula.Add(FormulaOperationType.PUTMEMB, 0, (byte)Door.GKDescriptorNo);
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