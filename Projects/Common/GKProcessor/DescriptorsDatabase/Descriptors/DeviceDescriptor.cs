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
				if (Device.Logic.OnClausesGroup.Clauses.Count + Device.Logic.OnClausesGroup.ClauseGroups.Count > 0)
				{
					Formula.AddClauseFormula(Device.Logic.OnClausesGroup);
					if (Device.Logic.UseOffCounterLogic)
					{
						Formula.AddStandardTurning(Device);
					}
					else
					{
						Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
					}
				}
				if (!Device.Logic.UseOffCounterLogic && Device.Logic.OffClausesGroup.GetObjects().Count > 0)
				{
					Formula.AddClauseFormula(Device.Logic.OffClausesGroup);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
				}
				if (Device.Logic.OnNowClausesGroup.Clauses.Count + Device.Logic.OnNowClausesGroup.ClauseGroups.Count > 0)
				{
					Formula.AddClauseFormula(Device.Logic.OnNowClausesGroup);
					Formula.AddPutBit(GKStateBit.TurnOnNow_InAutomatic, Device);
				}
				if (Device.Logic.OffNowClausesGroup.Clauses.Count + Device.Logic.OffNowClausesGroup.ClauseGroups.Count > 0)
				{
					Formula.AddClauseFormula(Device.Logic.OffNowClausesGroup);
					Formula.AddPutBit(GKStateBit.TurnOffNow_InAutomatic, Device);
				}
				if (Device.Logic.StopClausesGroup.Clauses.Count + Device.Logic.StopClausesGroup.ClauseGroups.Count > 0)
				{
					Formula.AddClauseFormula(Device.Logic.StopClausesGroup);
					Formula.AddPutBit(GKStateBit.Stop_InManual, Device);
				}
			}

			if ((Device.Driver.IsCardReaderOrCodeReader || Device.DriverType == GKDriverType.RSR2_GuardDetector || Device.DriverType == GKDriverType.RSR2_GuardDetectorSound)
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