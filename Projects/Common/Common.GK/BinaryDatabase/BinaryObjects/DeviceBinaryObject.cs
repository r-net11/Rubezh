using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using XFiresecAPI;

namespace Common.GK
{
	public class DeviceBinaryObject : BinaryObjectBase
	{
		public DeviceBinaryObject(XDevice device, DatabaseType databaseType)
		{
			DatabaseType = databaseType;
			ObjectType = ObjectType.Device;
			Device = device;
			Build();
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes(Device.Driver.DriverTypeNo);

			ushort address = 0;
			if (Device.Driver.IsDeviceOnShleif)
				address = (ushort)((Device.ShleifNoNew - 1) * 256 + Device.IntAddress);
			SetAddress(address);

			SetFormulaBytes();
			SetPropertiesBytes();
			InitializeAllBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			if (DatabaseType == DatabaseType.Gk)
			{
				if (Device.Driver.HasLogic && Device.DeviceLogic.Clauses.Count > 0)
				{
					AddClauseFormula(Device.DeviceLogic);
				}
			}
			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void AddClauseFormula(XDeviceLogic deviceLogic)
		{
			Formula.AddClauseFormula(deviceLogic);
			AddMro2MFormula();
			Formula.AddStandardTurning(Device);
		}

		void AddMro2MFormula()
		{
			if (Device.Driver.DriverType == XDriverType.MRO_2)
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
				Parameters = new List<byte>();
				return;
			}
			foreach (var property in Device.Properties)
			{
				var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null && driverProperty.IsAUParameter)
				{
					if (driverProperty.IsMPTOrMRORegime)
					{
						if (Device.Driver.DriverType == XDriverType.MPT)
							property.Value = Device.IsChildMPTOrMRO() ? (ushort)2 : (ushort)1;
						if (Device.Driver.DriverType == XDriverType.MRO_2)
							property.Value = Device.IsChildMPTOrMRO() ? (ushort)1 : (ushort)2;
					}

					byte no = driverProperty.No;
					ushort value = property.Value;
					if (driverProperty.Offset > 0)
						value = (ushort)(value << driverProperty.Offset);
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
					if (driverProperty.Multiplier != 0)
						value = (ushort)(value * driverProperty.Multiplier);
					if (Device.Driver.DriverType == XDriverType.RSR2_KAU && driverProperty.No == 1)
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

			Parameters = new List<byte>();
			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}

	class BinProperty
	{
		public byte No { get; set; }
		public ushort Value { get; set; }
	}
}