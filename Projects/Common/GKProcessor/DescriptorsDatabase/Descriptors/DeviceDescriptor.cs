using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI;

namespace GKProcessor
{
	public class DeviceDescriptor : BaseDescriptor
	{
		GKDevice Device { get; set; }

		public DeviceDescriptor(GKDevice device)
			: base(device)
		{
			DescriptorType = DescriptorType.Device;
			Device = device;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(Device.Driver.DriverTypeNo);

			var address = 0;
			if (Device.Driver.IsDeviceOnShleif)
				address = (Device.ShleifNo - 1) * 256 + Device.IntAddress;
			SetAddress((ushort)address);
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

			var mirrorParents = Device.GetMirrorParents();
			Formula.AddMirrorLogic(Device, mirrorParents);

			if (CreateMPTLogic())
				return;

			if (Device.Driver.HasLogic)
			{
				var hasOn1 = Device.Logic.OnClausesGroup.IsNotEmpty();
				var hasOn2 = Device.Logic.On2ClausesGroup.IsNotEmpty();
				if (hasOn1 && ! hasOn2)
				{
					Formula.AddClauseFormula(Device.Logic.OnClausesGroup);
					Formula.Add(FormulaOperationType.CONST, 0, 0);
					Formula.Add(FormulaOperationType.CONST, 0, 0);
					Formula.Add(FormulaOperationType.CONST, 0, 0);
					Formula.Add(FormulaOperationType.CONST, 0, 0);
					Formula.Add(FormulaOperationType.CONST, 0, 0);
					Formula.AddPutBit(31, Device);
					Formula.AddPutBit(30, Device);
					Formula.AddPutBit(29, Device);
					Formula.AddPutBit(28, Device);
					Formula.AddPutBit(27, Device);
					if (Device.Logic.UseOffCounterLogic)
						Formula.AddStandardTurning(Device);
					else
						Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
				}
				if (!hasOn1 && hasOn2)
				{
					Formula.AddClauseFormula(Device.Logic.On2ClausesGroup);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.AddPutBit(31, Device);
					Formula.AddPutBit(30, Device);
					Formula.AddPutBit(29, Device);
					Formula.AddPutBit(28, Device);
					Formula.AddPutBit(27, Device);
					if (Device.Logic.UseOffCounterLogic)
						Formula.AddStandardTurning(Device);
					else
						Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
				}
				if (hasOn1 && hasOn2)
				{
					Formula.AddClauseFormula(Device.Logic.OnClausesGroup);
					Formula.AddClauseFormula(Device.Logic.On2ClausesGroup);
					Formula.Add(FormulaOperationType.DUP);
					Formula.Add(FormulaOperationType.DUP);
					Formula.Add(FormulaOperationType.DUP);
					Formula.Add(FormulaOperationType.DUP);
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(31, Device);
					Formula.AddPutBit(30, Device);
					Formula.AddPutBit(29, Device);
					Formula.AddPutBit(28, Device);
					Formula.AddPutBit(27, Device);
					Formula.Add(FormulaOperationType.OR);
					if (Device.Logic.UseOffCounterLogic)
						Formula.AddStandardTurning(Device);
					else
						Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
				}
				
				if (!Device.Logic.UseOffCounterLogic && Device.Logic.OffClausesGroup.IsNotEmpty())
				{
					Formula.AddClauseFormula(Device.Logic.OffClausesGroup);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
				}
				if (Device.Logic.OnNowClausesGroup.IsNotEmpty())
				{
					Formula.AddClauseFormula(Device.Logic.OnNowClausesGroup);
					Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Device);
				}
				if (Device.Logic.OffNowClausesGroup.IsNotEmpty())
				{
					Formula.AddClauseFormula(Device.Logic.OffNowClausesGroup);
					Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Device);
				}
				if (Device.Logic.StopClausesGroup.IsNotEmpty())
				{
					Formula.AddClauseFormula(Device.Logic.StopClausesGroup);
					Formula.AddPutBit(GKStateBit.Stop_InManual, Device);
				}

				SetICLogic();
			}

			if ((Device.DriverType == GKDriverType.RSR2_GuardDetector || Device.DriverType == GKDriverType.RSR2_GuardDetectorSound || Device.DriverType == GKDriverType.RSR2_HandGuardDetector)
				&& Device.GuardZones != null && Device.GuardZones.Count > 0)
			{
				Formula.AddGetBit(GKStateBit.On, Device.GuardZones.FirstOrDefault());
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
				Formula.AddGetBit(GKStateBit.Off, Device.GuardZones.FirstOrDefault());
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
			}

			if (Device.DriverType == GKDriverType.RSR2_MAP4 && Device.Zones.Count > 0)
			{
				int count = 0;
				foreach (var zone in Device.Zones)
				{
					Formula.AddGetBit(GKStateBit.Fire1, zone);
					if (count > 0)
						Formula.Add(FormulaOperationType.OR);
					Formula.AddGetBit(GKStateBit.Fire2, zone);
					Formula.Add(FormulaOperationType.OR);
					Formula.AddGetBit(GKStateBit.Attention, zone);
					Formula.Add(FormulaOperationType.OR);
					count++;
					Device.LinkToDescriptor(zone);
				}
				Formula.AddPutBit(GKStateBit.Reset, Device);
			}

			if (Device.Door != null && (Device.Door.LockDeviceUID == Device.UID || Device.Door.LockDeviceExitUID == Device.UID))
			{
				switch (Device.Door.DoorType)
				{
					case GKDoorType.AirlockBooth:
					case GKDoorType.Turnstile:
						var device = Device.Door.LockDeviceUID == Device.UID ? Device.Door.ExitDevice : Device.Door.EnterDevice;
						var button = Device.Door.LockDeviceUID == Device.UID ? Device.Door.EnterButton : Device.Door.ExitButton;
						if (device != null)
						{
							Formula.AddGetBit(GKStateBit.Attention, device);
							if (button != null)
							{
								Formula.Add(FormulaOperationType.BR, 2, 8);
								Formula.AddGetBit(GKStateBit.Fire1, button);
							}
							Formula.Add(FormulaOperationType.BR, 2, 6);
						}

						Formula.AddGetBit(GKStateBit.On, Device.Door);
						Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
						Formula.AddGetBit(GKStateBit.TurningOff, Device.Door);
						Formula.AddGetBit(GKStateBit.Off, Device.Door);
						Formula.Add(FormulaOperationType.OR);
						Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
						break;

					case GKDoorType.Barrier:
						if (Device.Door.LockDeviceUID == Device.UID)
						{
							Formula.AddGetBit(GKStateBit.On, Device.Door);
							Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
							Formula.AddGetBit(GKStateBit.Off, Device.Door);
							Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
						}
						else
						{
							Formula.AddGetBit(GKStateBit.Fire1, Device.Door.LockControlDevice);
							Formula.AddGetBit(GKStateBit.Fire1, Device.Door.LockControlDeviceExit);
							Formula.Add(FormulaOperationType.OR);
							Formula.Add(FormulaOperationType.COM);
							Formula.AddGetBit(GKStateBit.Off, Device.Door);
							Formula.Add(FormulaOperationType.AND);
							Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
							Formula.AddGetBit(GKStateBit.On, Device.Door);
							Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
						}
						break;

					default:
						Formula.AddGetBit(GKStateBit.On, Device.Door);
						Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
						Formula.AddGetBit(GKStateBit.TurningOff, Device.Door);
						Formula.AddGetBit(GKStateBit.Off, Device.Door);
						Formula.Add(FormulaOperationType.OR);
						Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
						break;
				}
				Formula.Add(FormulaOperationType.END);
			}
		}

		void SetICLogic()
		{
			byte blink1RedByte = 16;
			byte blink3RedByte = 11;
			byte offRedByte = 25;
			var hasBlink1Red = Device.Logic.RedIndicatorLogic.Blink1ClausesGroup.IsNotEmpty();
			var hasBlink3Red = Device.Logic.RedIndicatorLogic.Blink3ClausesGroup.IsNotEmpty();
			if (hasBlink1Red && !hasBlink3Red)
			{
				Formula.AddClauseFormula(Device.Logic.RedIndicatorLogic.Blink1ClausesGroup);
				if (Device.Logic.RedIndicatorLogic.UseOffCounterLogic)
				{
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blink1RedByte, Device);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(offRedByte, Device);
				}
				else
					Formula.AddPutBit(blink1RedByte, Device);
			}
			if (!hasBlink1Red && hasBlink3Red)
			{
				Formula.AddClauseFormula(Device.Logic.RedIndicatorLogic.Blink3ClausesGroup);
				if (Device.Logic.RedIndicatorLogic.UseOffCounterLogic)
				{
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blink3RedByte, Device);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(offRedByte, Device);
				}
				else
					Formula.AddPutBit(blink3RedByte, Device);
			}
			if (hasBlink1Red && hasBlink3Red)
			{
				if (Device.Logic.RedIndicatorLogic.UseOffCounterLogic)
				{
					Formula.AddClauseFormula(Device.Logic.RedIndicatorLogic.Blink1ClausesGroup);
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blink1RedByte, Device);
					Formula.AddClauseFormula(Device.Logic.RedIndicatorLogic.Blink3ClausesGroup);
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blink3RedByte, Device);
					Formula.Add(FormulaOperationType.OR);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(offRedByte, Device);
				}
				else
				{
					Formula.AddClauseFormula(Device.Logic.RedIndicatorLogic.Blink1ClausesGroup);
					Formula.AddPutBit(blink1RedByte, Device);
					Formula.AddClauseFormula(Device.Logic.RedIndicatorLogic.Blink3ClausesGroup);
					Formula.AddPutBit(blink3RedByte, Device);
				}
			}
			if (!Device.Logic.RedIndicatorLogic.UseOffCounterLogic && Device.Logic.RedIndicatorLogic.Blink1ClausesGroup.IsNotEmpty())
			{
				Formula.AddClauseFormula(Device.Logic.RedIndicatorLogic.OffClausesGroup);
				Formula.AddPutBit(offRedByte, Device);
			}

			byte blink1GreenByte = 17;
			byte blink3GreenByte = 13;
			byte offGreenByte = 23;
			var hasBlink1Green = Device.Logic.GreenIndicatorLogic.Blink1ClausesGroup.IsNotEmpty();
			var hasBlink3Green = Device.Logic.GreenIndicatorLogic.Blink3ClausesGroup.IsNotEmpty();
			if (hasBlink1Green && !hasBlink3Green)
			{
				Formula.AddClauseFormula(Device.Logic.GreenIndicatorLogic.Blink1ClausesGroup);
				if (Device.Logic.GreenIndicatorLogic.UseOffCounterLogic)
				{
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blink1GreenByte, Device);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(offGreenByte, Device);
				}
				else
					Formula.AddPutBit(blink1GreenByte, Device);
			}
			if (!hasBlink1Green && hasBlink3Green)
			{
				Formula.AddClauseFormula(Device.Logic.GreenIndicatorLogic.Blink3ClausesGroup);
				if (Device.Logic.GreenIndicatorLogic.UseOffCounterLogic)
				{
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blink3GreenByte, Device);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(offGreenByte, Device);
				}
				else
					Formula.AddPutBit(blink3GreenByte, Device);
			}
			if (hasBlink1Green && hasBlink3Green)
			{
				if (Device.Logic.GreenIndicatorLogic.UseOffCounterLogic)
				{
					Formula.AddClauseFormula(Device.Logic.GreenIndicatorLogic.Blink1ClausesGroup);
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blink1GreenByte, Device);
					Formula.AddClauseFormula(Device.Logic.GreenIndicatorLogic.Blink3ClausesGroup);
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blink3GreenByte, Device);
					Formula.Add(FormulaOperationType.OR);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(offGreenByte, Device);
				}
				else
				{
					Formula.AddClauseFormula(Device.Logic.GreenIndicatorLogic.Blink1ClausesGroup);
					Formula.AddPutBit(blink1GreenByte, Device);
					Formula.AddClauseFormula(Device.Logic.GreenIndicatorLogic.Blink3ClausesGroup);
					Formula.AddPutBit(blink3GreenByte, Device);
				}
			}
			if (!Device.Logic.GreenIndicatorLogic.UseOffCounterLogic && Device.Logic.GreenIndicatorLogic.Blink1ClausesGroup.IsNotEmpty())
			{
				Formula.AddClauseFormula(Device.Logic.GreenIndicatorLogic.OffClausesGroup);
				Formula.AddPutBit(offGreenByte, Device);
			}

			byte blinkYellowByte = 12;
			byte offYellowByte = 15;
			if (Device.Logic.YellowIndicatorLogic.Blink1ClausesGroup.IsNotEmpty())
			{
				Formula.AddClauseFormula(Device.Logic.YellowIndicatorLogic.Blink1ClausesGroup);
				if (Device.Logic.YellowIndicatorLogic.UseOffCounterLogic)
				{
					Formula.Add(FormulaOperationType.DUP);
					Formula.AddPutBit(blinkYellowByte, Device);
					Formula.Add(FormulaOperationType.COM);
					Formula.AddPutBit(offYellowByte, Device);
				}
				else
					Formula.AddPutBit(blinkYellowByte, Device);
			}
			if (!Device.Logic.YellowIndicatorLogic.UseOffCounterLogic && Device.Logic.YellowIndicatorLogic.OffClausesGroup.IsNotEmpty())
			{
				Formula.AddClauseFormula(Device.Logic.YellowIndicatorLogic.OffClausesGroup);
				Formula.AddPutBit(offYellowByte, Device);
			}
		}

		void SetPropertiesBytes()
		{
			Parameters = new List<byte>();
			var binProperties = new List<BinProperty>();

			if (DatabaseType == DatabaseType.Gk && Device.Driver.IsDeviceOnShleif)
			{
				return;
			}

			if (DatabaseType == DatabaseType.Gk && (Device.DriverType == GKDriverType.GKMirror || (Device.Parent!=null && Device.Parent.DriverType == GKDriverType.GKMirror)))
			{
				return;
			}

			Device.Properties = (from i in Device.Driver.Properties join o in Device.Properties on i.Name equals o.Name select o).ToList();

			foreach (var property in Device.Properties)
			{
				var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null && driverProperty.IsAUParameter)
				{
					if (driverProperty.CanNotEdit)
					{
						if (Device.DriverType == GKDriverType.RSR2_MVP)
						{
							if (driverProperty.Name == "Число АУ на АЛС3 МВП")
								property.Value = (ushort)Device.Children[0].AllChildren.Count(x => x.Driver.IsReal && !x.Driver.IsGroupDevice);
							if (driverProperty.Name == "Число АУ на АЛС4 МВП")
								property.Value = (ushort)Device.Children[1].AllChildren.Count(x => x.Driver.IsReal && !x.Driver.IsGroupDevice);
						}

						if (Device.DriverType == GKDriverType.RSR2_KDKR)
						{
							if (driverProperty.Name == "Число АУ на АЛС3 КД")
								property.Value = (ushort)Device.Children[13].AllChildren.Count(x => x.Driver.IsReal && !x.Driver.IsGroupDevice);
							if (driverProperty.Name == "Число АУ на АЛС4 КД")
								property.Value = (ushort)Device.Children[14].AllChildren.Count(x => x.Driver.IsReal && !x.Driver.IsGroupDevice);
						}
					}

					byte no = driverProperty.No;
					ushort value = property.Value;
					if (driverProperty.Mask > 0)
					{
						if (driverProperty.DriverPropertyType == GKDriverPropertyTypeEnum.BoolType)
						{
							if (value > 0)
								value = (ushort)driverProperty.Mask;
						}
						else
						{
							value = (ushort)(value & driverProperty.Mask);
						}
					}
					if (driverProperty.IsHieghByte)
						value = (ushort)(value * 256);
					if (Device.DriverType == GKDriverType.RSR2_KAU && driverProperty.No == 1)
					{
						value = (ushort)(256 + value % 256);
					}

					var binProperty = binProperties.FirstOrDefault(x => x.No == no);
					if (binProperty == null)
					{
						binProperty = new BinProperty()
						{
							No = no
						};
						binProperties.Add(binProperty);
					}
					binProperty.Value += value;
				}
			}

			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}

		public bool CreateMPTLogic()
		{
			var deviceMpts = GKManager.MPTs.FindAll(x => x.MPTDevices.Exists(y => y.Device == Device));
			foreach (var deviceMpt in deviceMpts)
			{
				var mptDevice = deviceMpt.MPTDevices.FirstOrDefault(x => x.Device == Device);
				if (mptDevice == null)
					return false;
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard)
				{
					CreateAutomaticOffBoards(deviceMpt);
					Device.LinkToDescriptor(deviceMpt);
					return true;
				}
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard || mptDevice.MPTDeviceType == GKMPTDeviceType.ExitBoard || mptDevice.MPTDeviceType == GKMPTDeviceType.Speaker)
				{ 
					CreateOnDevices(deviceMpt);
					Device.LinkToDescriptor(deviceMpt);
					return true;
				}
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.Bomb)
				{
					CreateBombDevices(deviceMpt);
					Device.LinkToDescriptor(deviceMpt);
					return true;
				}
			}
			return false;
		}

		void CreateAutomaticOffBoards(GKMPT mpt)
		{
			Formula.AddGetBit(GKStateBit.Norm, mpt);
			Formula.Add(FormulaOperationType.DUP);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
			Formula.Add(FormulaOperationType.COM);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
			Formula.Add(FormulaOperationType.END);
		}

		void CreateOnDevices(GKMPT mpt)
		{
			Formula.AddGetBit(GKStateBit.TurningOn, mpt);
			Formula.AddGetBit(GKStateBit.On, mpt);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
			Formula.AddGetBit(GKStateBit.Off, mpt);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
			Formula.Add(FormulaOperationType.END);
		}

		void CreateBombDevices(GKMPT mpt)
		{
			Formula.AddGetBit(GKStateBit.On, mpt);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
			Formula.AddGetBit(GKStateBit.Off, mpt);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
			Formula.Add(FormulaOperationType.END);
		}
	}
}