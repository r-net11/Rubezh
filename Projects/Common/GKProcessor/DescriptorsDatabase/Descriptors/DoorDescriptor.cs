using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKProcessor
{
	public class DoorDescriptor : BaseDescriptor
	{
		GKDoor Door { get; set; }
		public DoorPimDescriptorEnter DoorPimDescriptorEnter { get; private set; }
		public DoorPimDescriptorExit DoorPimDescriptorExit { get; private set; }
		public DoorPimDescriptorCrossing DoorPimDescriptorCrossing { get; private set; }

		public DelayDescriptor LockDelayDescriptor { get; private set; }
		public DelayDescriptor LockDelayExitDescriptor { get; private set; }
		public DelayDescriptor ResetDelayDescriptor { get; private set; }

		public DoorDescriptor(GKDoor door)
			: base(door)
		{
			DescriptorType = DescriptorType.Door;
			Door = door;
			if (Door.DoorType == GKDoorType.Turnstile)
			{
				LockDelayDescriptor = new TurnstileDelayDescriptor(Door, Door.LockDelay, GKDoorDelayType.LockDelay);
				LockDelayExitDescriptor = new TurnstileDelayDescriptor(Door, Door.LockDelayExit, GKDoorDelayType.LockDelayExit);
				ResetDelayDescriptor = new TurnstileDelayDescriptor(Door, Door.ResetDelay, GKDoorDelayType.ResetDelay);
			}
			if (Door.DoorType == GKDoorType.AirlockBooth)
			{
				DoorPimDescriptorEnter = new DoorPimDescriptorEnter(Door);
				DoorPimDescriptorExit = new DoorPimDescriptorExit(Door);
			}
			if (Door.DoorType == GKDoorType.Barrier)
			{
				LockDelayDescriptor = new BarrierDelayDescriptor(Door, Door.LockDelay, GKDoorDelayType.LockDelay);
				LockDelayExitDescriptor = new BarrierDelayDescriptor(Door, Door.LockDelayExit, GKDoorDelayType.LockDelayExit);
			}
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(0x104);
			SetAddress(0);
			SetPropertiesBytes();
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();

			var hasOpenRegimeLogic = Door.OpenRegimeLogic.OnClausesGroup.GetObjects().Count > 0;
			var hasOpenExitRegimeLogic = Door.OpenExitRegimeLogic.OnClausesGroup.GetObjects().Count > 0 && Door.DoorType == GKDoorType.Turnstile;
			if (hasOpenRegimeLogic || hasOpenExitRegimeLogic)
			{
				if (hasOpenRegimeLogic)
					Formula.AddClauseFormula(Door.OpenRegimeLogic.OnClausesGroup);
				if (hasOpenExitRegimeLogic)
					Formula.AddClauseFormula(Door.OpenExitRegimeLogic.OnClausesGroup);
				if (hasOpenRegimeLogic && hasOpenExitRegimeLogic)
					Formula.Add(FormulaOperationType.OR);
				Formula.Add(FormulaOperationType.BR, 1, 7);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.SetRegime_Manual, Door);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
				Formula.AddPutBit(GKStateBit.TurnOn_InManual, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}

			var hasCloseRegimeLogic = Door.CloseRegimeLogic.OnClausesGroup.GetObjects().Count > 0;
			if (hasCloseRegimeLogic)
			{
				Formula.AddClauseFormula(Door.CloseRegimeLogic.OnClausesGroup);
				Formula.Add(FormulaOperationType.BR, 1, 7);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.SetRegime_Manual, Door);
				Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Door);
				Formula.AddPutBit(GKStateBit.TurnOffNow_InManual, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}

			var hasNormRegimeLogic = Door.NormRegimeLogic.OnClausesGroup.GetObjects().Count > 0;
			if (hasNormRegimeLogic)
			{
				Formula.AddClauseFormula(Door.NormRegimeLogic.OnClausesGroup);
				Formula.AddGetBit(GKStateBit.Norm, Door);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.Add(FormulaOperationType.BR, 1, 7);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.SetRegime_Automatic, Door);
				Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Door);
				Formula.AddPutBit(GKStateBit.TurnOffNow_InManual, Door);
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
		}

		void SetFormulaBarrier()
		{
			var lockControlDevice = Door.LockControlDevice;
			var lockControlDeviceExit = Door.LockControlDeviceExit;
			var enterDevice = Door.EnterDevice;
			var exitDevice = Door.ExitDevice;
			var enterZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Door.EnterZoneUID);
			var exitZone = GKManager.SKDZones.FirstOrDefault(x => x.UID == Door.ExitZoneUID);

			if (lockControlDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDevice);
				Formula.Add(FormulaOperationType.COM);
				Formula.AddGetBit(GKStateBit.Off, Door);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.Fire1, Door);

				Formula.AddGetBit(GKStateBit.Fire1, lockControlDevice);
				Formula.AddGetBit(GKStateBit.TurningOff, Door);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Door);
			}

			Formula.AddGetBit(GKStateBit.Attention, enterDevice);
			Formula.AddGetBit(GKStateBit.Off, Door);
			Formula.Add(FormulaOperationType.AND);
			if (!Door.AntipassbackOn || enterZone == null || exitZone == null)
			{
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.AddWithGKBase(FormulaOperationType.ACS, (byte)Door.EnterLevel, enterDevice);
			}
			else
			{
				Formula.Add(FormulaOperationType.BR, 1, 13);
				Formula.AddWithGKBase(FormulaOperationType.ACSP, (byte)Door.EnterLevel, enterDevice);
				Formula.Add(FormulaOperationType.BR, 1, 10);
				Formula.Add(FormulaOperationType.TSTP, 0, (byte)exitZone.No);
				Formula.Add(FormulaOperationType.BR, 1, 8);
				Formula.Add(FormulaOperationType.CONST, 0, 0);
				Formula.AddWithGKBase(FormulaOperationType.PUTMEMB, 0, Door);
				Formula.Add(FormulaOperationType.CONST, 0, 1);

				Formula.AddWithGKBase(FormulaOperationType.GETMEMB, 0, Door);
				Formula.Add(FormulaOperationType.BR, 1, 0);
				Formula.Add(FormulaOperationType.CONST, 0, (byte)enterZone.No);
				Formula.Add(FormulaOperationType.PUTP);
			}

			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
			Formula.Add(FormulaOperationType.EXIT);

			Formula.AddGetBit(GKStateBit.Attention, exitDevice);
			Formula.AddGetBit(GKStateBit.Off, Door);
			Formula.Add(FormulaOperationType.AND);
			if (!Door.AntipassbackOn || enterZone == null || exitZone == null)
			{
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.AddWithGKBase(FormulaOperationType.ACS, (byte)Door.EnterLevel, exitDevice);
			}
			else
			{
				Formula.Add(FormulaOperationType.BR, 1, 13);
				Formula.AddWithGKBase(FormulaOperationType.ACSP, (byte)Door.EnterLevel, exitDevice);
				Formula.Add(FormulaOperationType.BR, 1, 10);
				Formula.Add(FormulaOperationType.TSTP, 0, (byte)enterZone.No);
				Formula.Add(FormulaOperationType.BR, 1, 8);
				Formula.Add(FormulaOperationType.CONST, 0, 0);
				Formula.AddWithGKBase(FormulaOperationType.PUTMEMB, 0, Door);
				Formula.Add(FormulaOperationType.CONST, 0, 1);

				Formula.AddWithGKBase(FormulaOperationType.GETMEMB, 0, Door);
				Formula.Add(FormulaOperationType.BR, 1, 0);
				Formula.Add(FormulaOperationType.CONST, 0, (byte)exitZone.No);
				Formula.Add(FormulaOperationType.PUTP);
			}

			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
			Formula.Add(FormulaOperationType.EXIT);
		}

		void SetFormulaDoor()
		{
			if (Door.LockControlDevice != null)
			{
				Formula.AddGetBit(GKStateBit.On, Door);
				Formula.AddGetBit(GKStateBit.Fire1, Door.LockControlDevice);
				Formula.AddGetBit(GKStateBit.Fire2, Door.LockControlDevice);
				Formula.Add(FormulaOperationType.OR);
				Formula.Add(FormulaOperationType.AND);
				if (Door.AntipassbackOn)
				{
					Formula.Add(FormulaOperationType.BR, 1, 4);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.AddWithGKBase(FormulaOperationType.GETMEMB, 0, Door);
					Formula.Add(FormulaOperationType.PUTP);
				}
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Door);
				Formula.AddGetBit(GKStateBit.TurningOff, Door);
				Formula.AddGetBit(GKStateBit.Fire1, Door.LockControlDevice);
				Formula.AddGetBit(GKStateBit.Fire2, Door.LockControlDevice);
				Formula.Add(FormulaOperationType.OR);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Door);
				Formula.AddGetBit(GKStateBit.Off, Door);
				Formula.AddGetBit(GKStateBit.Fire1, Door.LockControlDevice);
				Formula.AddGetBit(GKStateBit.Fire2, Door.LockControlDevice);
				Formula.Add(FormulaOperationType.OR);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddGetBit(GKStateBit.Fire1, Door);
				Formula.Add(FormulaOperationType.OR);
				Formula.AddPutBit(GKStateBit.Fire1, Door);
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

			if (lockControlDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDevice);
				Formula.AddGetBit(GKStateBit.Off, Door.PimEnter);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddGetBit(GKStateBit.Fire1, Door);
				Formula.Add(FormulaOperationType.OR);
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.Fire1, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}

			if (lockControlDeviceExit != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDeviceExit);
				Formula.AddGetBit(GKStateBit.Off, Door.PimExit);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddGetBit(GKStateBit.Fire1, Door);
				Formula.Add(FormulaOperationType.OR);
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.Fire1, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}

			if (lockControlDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDevice);
				Formula.AddGetBit(GKStateBit.On, Door);
				Formula.Add(FormulaOperationType.AND);
				if (Door.AntipassbackOn && exitZone != null)
				{
					Formula.Add(FormulaOperationType.BR, 1, 7);
					Formula.AddWithGKBase(FormulaOperationType.GETMEMB, 0, Door);
					Formula.Add(FormulaOperationType.BR, 1, 0);
					Formula.Add(FormulaOperationType.CONST, 0, (byte)exitZone.No);
					Formula.Add(FormulaOperationType.PUTP);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
				}
				else
				{
					Formula.Add(FormulaOperationType.BR, 1, 3);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
				}
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Door);
				Formula.Add(FormulaOperationType.EXIT);
				Formula.AddGetBit(GKStateBit.TurningOff, Door);
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDevice);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDeviceExit);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}

			if (lockControlDeviceExit != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDeviceExit);
				Formula.AddGetBit(GKStateBit.On, Door);
				Formula.Add(FormulaOperationType.AND);
				if (Door.AntipassbackOn && enterZone != null)
				{
					Formula.Add(FormulaOperationType.BR, 1, 7);
					Formula.AddWithGKBase(FormulaOperationType.GETMEMB, 0, Door);
					Formula.Add(FormulaOperationType.BR, 1, 0);
					Formula.Add(FormulaOperationType.CONST, 0, (byte)enterZone.No);
					Formula.Add(FormulaOperationType.PUTP);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
				}
				else
				{
					Formula.Add(FormulaOperationType.BR, 1, 3);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
				}
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Door);
				Formula.Add(FormulaOperationType.EXIT);
				Formula.AddGetBit(GKStateBit.TurningOff, Door);
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDeviceExit);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddGetBit(GKStateBit.Fire1, lockControlDevice);
				Formula.Add(FormulaOperationType.COM);
				Formula.Add(FormulaOperationType.AND);
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}
			Formula.AddGetBit(GKStateBit.Attention, enterDevice);
			Formula.AddGetBit(GKStateBit.Off, Door);
			Formula.Add(FormulaOperationType.AND);
			if (!Door.AntipassbackOn || exitZone == null)
			{
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.AddWithGKBase(FormulaOperationType.ACS, (byte)Door.EnterLevel, enterDevice);
			}
			else
			{
				Formula.Add(FormulaOperationType.BR, 1, 9);
				Formula.AddWithGKBase(FormulaOperationType.ACSP, (byte)Door.EnterLevel, enterDevice);
				Formula.Add(FormulaOperationType.BR, 1, 6);
				Formula.Add(FormulaOperationType.TSTP, 0, (byte)exitZone.No);
				Formula.Add(FormulaOperationType.BR, 1, 4);
				Formula.Add(FormulaOperationType.CONST, 0, 0);
				Formula.AddWithGKBase(FormulaOperationType.PUTMEMB, 0, Door);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
			}
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
			Formula.Add(FormulaOperationType.EXIT);
			if (enterButton != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, enterButton);
				Formula.AddGetBit(GKStateBit.Off, Door);
				Formula.Add(FormulaOperationType.AND);
				Formula.Add(FormulaOperationType.BR, 1, 3);
				Formula.Add(FormulaOperationType.CONST, 0, 1);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}
			if (exitDevice != null)
			{
				Formula.AddGetBit(GKStateBit.Attention, exitDevice);
				Formula.AddGetBit(GKStateBit.Off, Door);
				Formula.Add(FormulaOperationType.AND);
				if (!Door.AntipassbackOn || enterZone == null)
				{
					Formula.Add(FormulaOperationType.BR, 1, 3);
					Formula.AddWithGKBase(FormulaOperationType.ACS, (byte)Door.EnterLevel, exitDevice);
				}
				else
				{
					Formula.Add(FormulaOperationType.BR, 1, 9);
					Formula.AddWithGKBase(FormulaOperationType.ACSP, (byte)Door.EnterLevel, exitDevice);
					Formula.Add(FormulaOperationType.BR, 1, 6);
					Formula.Add(FormulaOperationType.TSTP, 0, (byte)enterZone.No);
					Formula.Add(FormulaOperationType.BR, 1, 4);
					Formula.Add(FormulaOperationType.CONST, 0, 0);
					Formula.AddWithGKBase(FormulaOperationType.PUTMEMB, 0, Door);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
				}
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}
			if (exitButton != null)
			{
				Formula.AddGetBit(GKStateBit.Fire1, exitButton);
				Formula.AddGetBit(GKStateBit.Off, Door);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
			}
			if (Door.PimEnter != null)
			{
				Door.LinkToDescriptor(Door.PimEnter);
			}

			if (Door.PimExit != null)
			{
				Door.LinkToDescriptor(Door.PimExit);
			}
		}

		void TurnOnDoorBuilder(bool enterDevice)
		{
			var device = enterDevice ? Door.EnterDevice : Door.ExitDevice;
			if (device.Driver.IsCardReaderOrCodeReader)
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
				Formula.AddGetBit(GKStateBit.Attention, device);
				Formula.AddGetBit(GKStateBit.Off, Door);
				Formula.Add(FormulaOperationType.AND);
				Formula.Add(FormulaOperationType.BR, 1, (byte)operationCount);

				if (!Door.AntipassbackOn)
					Formula.AddWithGKBase(FormulaOperationType.ACS, (byte)Door.EnterLevel, device);
				if (Door.AntipassbackOn)
				{
					Formula.AddWithGKBase(FormulaOperationType.ACSP, (byte)Door.EnterLevel, device);
					Formula.Add(FormulaOperationType.BR, 1, (byte)(operationCount - 3));
					if (zone1 != null)
					{
						Formula.Add(FormulaOperationType.TSTP, 0, (byte)zone1.No);
						Formula.Add(FormulaOperationType.BR, 1, (byte)(operationCount - 5));
					}
					if (zone2 != null)
					{
						Formula.Add(FormulaOperationType.CONST, 0, (byte)zone2.No);
						Formula.AddWithGKBase(FormulaOperationType.PUTMEMB, 0, Door);
						Formula.Add(FormulaOperationType.CONST, 0, 1);
					}
				}
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
				Formula.Add(FormulaOperationType.EXIT);
			}
			else
			{
				Formula.AddGetBit(GKStateBit.Fire1, Door.ExitDevice);
				Formula.AddGetBit(GKStateBit.Off, Door);
				Formula.Add(FormulaOperationType.AND);
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Door);
			}
		}

		void SetPropertiesBytes()
		{
			Parameters = new List<byte>();
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