using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
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
				address = (ushort)((Device.ShleifNo - 1) * 256 + Device.IntAddress);
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
                if (Device.Driver.HasLogic)
                {
                    if (Device.DeviceLogic.Clauses.Count > 0)
                    {
                        AddClauseFormula(Device.DeviceLogic);
                    }
                }
			}
            Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void AddClauseFormula(XDeviceLogic deviceLogic)
		{
			var clauseIndex = 0;
			foreach (var clause in deviceLogic.Clauses)
			{
				var baseObjects = new List<XBinaryBase>();
                foreach (var zone in clause.Zones)
                {
                    baseObjects.Add(zone);
                }
				foreach (var device in clause.Devices)
				{
					baseObjects.Add(device);
				}
                foreach (var direction in clause.Directions)
                {
                    baseObjects.Add(direction);
                }

				var objectIndex = 0;
				foreach (var baseObject in baseObjects)
				{
					Formula.AddGetBitOff(clause.StateType, baseObject, DatabaseType);

					if (objectIndex > 0)
					{
						switch (clause.ClauseOperationType)
						{
							case ClauseOperationType.AllDevices:
							case ClauseOperationType.AllZones:
                            case ClauseOperationType.AllDirections:
								Formula.Add(FormulaOperationType.AND, comment: "Объединение объектов по И");
								break;

							case ClauseOperationType.AnyDevice:
							case ClauseOperationType.AnyZone:
                            case ClauseOperationType.AnyDirection:
								Formula.Add(FormulaOperationType.OR, comment: "Объединение объектов по Или");
								break;
						}
					}
					objectIndex++;
				}

				if (clause.ClauseConditionType == ClauseConditionType.IfNot)
					Formula.Add(FormulaOperationType.COM, comment: "Условие Если НЕ");

				if (clauseIndex > 0)
				{
					switch (clause.ClauseJounOperationType)
					{
						case ClauseJounOperationType.And:
							Formula.Add(FormulaOperationType.AND, comment: "Объединение нескольких условий по И");
							break;

						case ClauseJounOperationType.Or:
							Formula.Add(FormulaOperationType.OR, comment: "Объединение нескольких условий по ИЛИ");
							break;
					}
				}
				clauseIndex++;
			}

			Formula.Add(FormulaOperationType.DUP);
			Formula.Add(FormulaOperationType.DUP);
			Formula.AddGetBit(XStateType.Norm, Device, DatabaseType);
			Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			Formula.AddGetBit(XStateType.Save, Device, DatabaseType);
			Formula.Add(FormulaOperationType.COM);
			Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом 31");
			Formula.AddPutBit(XStateType.TurnOn, Device, DatabaseType);
			Formula.Add(FormulaOperationType.COM);
			Formula.AddGetBit(XStateType.Norm, Device, DatabaseType);
			Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			Formula.AddPutBit(XStateType.TurnOff, Device, DatabaseType);
			Formula.AddPutBit(XStateType.Save, Device, DatabaseType);
		}

		void SetPropertiesBytes()
		{
			if (DatabaseType == DatabaseType.Gk)
			{
				if (Device.Parent == null || Device.Parent.Driver.DriverType != XDriverType.GK || Device.Driver.DriverType == XDriverType.KAU)
				{
					Parameters = new List<byte>();
					return;
				}
			}

			var binProperties = new List<BinProperty>();

            foreach (var property in Device.Properties)
            {
                var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
                if (driverProperty != null)
                {
					if (driverProperty.IsAUParameter)
                    {
                        byte no = driverProperty.No;
                        ushort value = property.Value;

                        var binProperty = binProperties.FirstOrDefault(x => x.No == no);
                        if (binProperty == null)
                        {
                            binProperty = new BinProperty()
                            {
                                No = no
                            };
                            binProperties.Add(binProperty);
                        }
                        binProperty.Value += (ushort)(value << driverProperty.Offset);
                    }
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