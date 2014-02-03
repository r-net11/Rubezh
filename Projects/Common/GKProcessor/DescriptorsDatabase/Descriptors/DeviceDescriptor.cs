using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using XFiresecAPI;

namespace GKProcessor
{
	public class DeviceDescriptor : BaseDescriptor
	{
		public DeviceDescriptor(XDevice device, DatabaseType databaseType)
		{
			DatabaseType = databaseType;
			DescriptorType = DescriptorType.Device;
			Device = device;
			Build();
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
					if (Device.DeviceLogic.Clauses.Count > 0)
					{
						Formula.AddClauseFormula(Device.DeviceLogic.Clauses);
						AddMro2MFormula();
						if (Device.DeviceLogic.OffClauses == null || Device.DeviceLogic.OffClauses.Count == 0)
						{
							Formula.AddStandardTurning(Device);
						}
						else
						{
							Formula.AddGetBit(XStateBit.Norm, Device);
							Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
							Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, Device);
						}
					}
					if (Device.DeviceLogic.OffClauses.Count > 0)
					{
						Formula.AddClauseFormula(Device.DeviceLogic.OffClauses);
						Formula.AddGetBit(XStateBit.Norm, Device);
						Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
						Formula.AddPutBit(XStateBit.TurnOff_InAutomatic, Device);
					}
				}
			}
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void AddMro2MFormula()
		{
			if (Device.DriverType == XDriverType.MRO_2)
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
					if (driverProperty.IsMPTOrMRORegime)
					{
						if (Device.DriverType == XDriverType.MPT)
							property.Value = Device.IsChildMPTOrMRO() ? (ushort)(2 << 6) : (ushort)(1 << 6);
						if (Device.DriverType == XDriverType.MRO_2)
							property.Value = Device.IsChildMPTOrMRO() ? (ushort)1 : (ushort)2;
					}

					byte no = driverProperty.No;
					ushort value = property.Value;
					if (driverProperty.Mask > 0)
					{
						if (driverProperty.DriverPropertyType == XDriverPropertyTypeEnum.BoolType)
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
					if (Device.DriverType == XDriverType.RSR2_KAU && driverProperty.No == 1)
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