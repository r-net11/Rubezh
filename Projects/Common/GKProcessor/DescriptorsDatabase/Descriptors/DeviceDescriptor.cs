using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

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
				if (!Device.Logic.UseOffCounterLogic && Device.Logic.OffClausesGroup.Clauses.Count + Device.Logic.OffClausesGroup.ClauseGroups.Count > 0)
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

			if ((Device.DriverType == GKDriverType.RSR2_CodeReader || Device.DriverType == GKDriverType.RSR2_CardReader || Device.DriverType == GKDriverType.RSR2_GuardDetector || Device.DriverType == GKDriverType.RSR2_GuardDetectorSound) && Device.GuardZones != null && Device.GuardZones.Count > 0)
			{
				Formula.AddGetBit(GKStateBit.On, Device.GuardZones.FirstOrDefault());
				Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
				Formula.AddGetBit(GKStateBit.Off, Device.GuardZones.FirstOrDefault());
				Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
			}
			if (Device.Door != null && Device.Door.DoorType == GKDoorType.Barrier)
			{
				if (Device.Door.LockDeviceUID == Device.UID)
				{
					Formula.AddGetBit(GKStateBit.On, Device.Door);
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
				}

				if (Device.Door.LockDeviceExitUID == Device.UID)
				{
					Formula.AddGetBit(GKStateBit.TurningOff, Device.Door);
					Formula.AddGetBit(GKStateBit.Off, Device.Door);
					Formula.Add(FormulaOperationType.OR);
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
				}
			}
			else
			{
				if (Device.Door != null && Device.Door.LockDeviceUID == Device.UID)
				{
					var exitDevice = Device.Door.ExitDevice;
					var enterButton = Device.Door.EnterButton;
					if (exitDevice != null && enterButton != null)
					{
						Formula.AddGetBit(GKStateBit.Attention, exitDevice);
						Formula.Add(FormulaOperationType.BR, 2, 8);
						Formula.AddGetBit(GKStateBit.Fire1, enterButton);
						Formula.Add(FormulaOperationType.BR, 2, 6);
					}

					Formula.AddGetBit(GKStateBit.On, Device.Door);
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
					Formula.AddGetBit(GKStateBit.TurningOff, Device.Door);
					Formula.AddGetBit(GKStateBit.Off, Device.Door);
					Formula.Add(FormulaOperationType.OR);
					if (Device.Door.LockControlDevice != null)
					{
						Formula.AddGetBit(GKStateBit.Fire1, Device.Door.LockControlDevice);
						Formula.Add(FormulaOperationType.OR);
					}
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
				}

				if (Device.Door != null && Device.Door.LockDeviceExitUID == Device.UID)
				{
					var enterDevice = Device.Door.EnterDevice;
					var exitButton = Device.Door.ExitButton;
					if (enterDevice != null && exitButton != null)
					{
						Formula.AddGetBit(GKStateBit.Attention, enterDevice);
						Formula.Add(FormulaOperationType.BR, 2, 8);
						Formula.AddGetBit(GKStateBit.Fire1, exitButton);
						Formula.Add(FormulaOperationType.BR, 2, 6);
					}

					Formula.AddGetBit(GKStateBit.On, Device.Door);
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
					Formula.AddGetBit(GKStateBit.TurningOff, Device.Door);
					Formula.AddGetBit(GKStateBit.Off, Device.Door);
					Formula.Add(FormulaOperationType.OR);
					if (Device.Door.LockControlDeviceExit != null)
					{
						Formula.AddGetBit(GKStateBit.Fire1, Device.Door.LockControlDeviceExit);
						Formula.Add(FormulaOperationType.OR);
					}
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
				}
			}
			Formula.Add(FormulaOperationType.END);
		}

		void SetPropertiesBytes()
		{
			Parameters = new List<byte>();
			var binProperties = new List<BinProperty>();

			if (DatabaseType == DatabaseType.Gk && Device.Driver.IsDeviceOnShleif)
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
					Device.LinkGKBases(deviceMpt);
					return true;
				}
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard || mptDevice.MPTDeviceType == GKMPTDeviceType.ExitBoard || mptDevice.MPTDeviceType == GKMPTDeviceType.Speaker)
				{ 
					CreateOnDevices(deviceMpt);
					Device.LinkGKBases(deviceMpt);
					return true;
				}
				if (mptDevice.MPTDeviceType == GKMPTDeviceType.Bomb)
				{
					CreateBombDevices(deviceMpt);
					Device.LinkGKBases(deviceMpt);
					return true;
				}
			}
			return false;
		}

		void CreateAutomaticOffBoards(GKMPT mpt)
		{
			Formula = new FormulaBuilder();
			Formula.AddGetBit(GKStateBit.Norm, mpt);
			Formula.Add(FormulaOperationType.DUP);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
			Formula.Add(FormulaOperationType.COM);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
			Formula.Add(FormulaOperationType.END);
		}

		void CreateOnDevices(GKMPT mpt)
		{
			Formula = new FormulaBuilder();
			Formula.AddGetBit(GKStateBit.TurningOn, mpt);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
			Formula.AddGetBit(GKStateBit.Off, mpt);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
			Formula.Add(FormulaOperationType.END);
		}

		void CreateBombDevices(GKMPT mpt)
		{
			Formula = new FormulaBuilder();
			Formula.AddGetBit(GKStateBit.On, mpt);
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
			Formula.AddGetBit(GKStateBit.Off, mpt);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
			Formula.Add(FormulaOperationType.END);
		}
	}
}