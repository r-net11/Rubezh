using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;

namespace GKProcessor
{
	public class DeviceDescriptor : BaseDescriptor
	{
		public DeviceDescriptor(GKDevice device, DatabaseType databaseType)
		{
			DatabaseType = databaseType;
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

			SetFormulaBytes();
			SetPropertiesBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if (DatabaseType == DatabaseType.Gk)
			{
				if (Device.Driver.HasLogic)
				{
					if (Device.DeviceLogic.ClausesGroup.Clauses.Count > 0)
					{
						Formula.AddClauseFormula(Device.DeviceLogic.ClausesGroup);
						AddMro2MFormula();
						if (Device.DeviceLogic.OffClausesGroup.Clauses.Count == 0)
						{
							Formula.AddStandardTurning(Device);
						}
						else
						{
							Formula.AddGetBit(GKStateBit.Norm, Device);
							Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
							Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
						}
					}
					if (Device.DeviceLogic.OffClausesGroup.Clauses.Count > 0)
					{
						Formula.AddClauseFormula(Device.DeviceLogic.OffClausesGroup);
						Formula.AddGetBit(GKStateBit.Norm, Device);
						Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
						Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
					}
				}

				if (Device.DriverType == GKDriverType.RSR2_GuardDetector && Device.GuardZone != null)
				{
					Formula.AddGetBit(GKStateBit.On, Device.GuardZone);
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
					Formula.AddGetBit(GKStateBit.Off, Device.GuardZone);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
				}
				if (Device.DriverType == GKDriverType.RSR2_CodeReader && Device.GuardZone != null)
				{
					Formula.AddGetBit(GKStateBit.On, Device.GuardZone);
					Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device);
					Formula.AddGetBit(GKStateBit.Off, Device.GuardZone);
					Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device);
				}
			}
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void AddMro2MFormula()
		{
			if (Device.DriverType == GKDriverType.MRO_2)
			{
				if (Device.DeviceLogic.ZoneLogicMROMessageType == ZoneLogicMROMessageType.Add)
				{
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.AddArgumentPutBit(31, Device);
				}
				var value = (int)Device.DeviceLogic.ZoneLogicMROMessageNo;
				if ((value & 1) == 1)
				{
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.AddArgumentPutBit(28, Device);
				}
				if ((value & 2) == 2)
				{
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.AddArgumentPutBit(29, Device);
				}
				if ((value & 4) == 4)
				{
					Formula.Add(FormulaOperationType.CONST, 0, 1);
					Formula.AddArgumentPutBit(30, Device);
				}
			}
		}

		void SetPropertiesBytes()
		{
			var binProperties = new List<BinProperty>();

			if (DatabaseType == DatabaseType.Gk && Device.Driver.IsDeviceOnShleif)
			{
				return;
			}
			foreach (var property in Device.Properties)
			{
				var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null && driverProperty.IsAUParameter)
				{
					if (driverProperty.CanNotEdit)
					{
						if (Device.DriverType == GKDriverType.MPT)
							property.Value = Device.IsChildMPTOrMRO() ? (ushort)(2 << 6) : (ushort)(1 << 6);
						if (Device.DriverType == GKDriverType.MRO_2)
							property.Value = Device.IsChildMPTOrMRO() ? (ushort)1 : (ushort)2;

						if (Device.DriverType == GKDriverType.RSR2_MVP)
						{
							if (driverProperty.Name == "Число АУ на АЛС3 МВП")
								property.Value = (ushort)Device.Children[0].AllChildren.Count(x => x.Driver.IsReal);
							if (driverProperty.Name == "Число АУ на АЛС4 МВП")
								property.Value = (ushort)Device.Children[1].AllChildren.Count(x => x.Driver.IsReal);
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
	}
}