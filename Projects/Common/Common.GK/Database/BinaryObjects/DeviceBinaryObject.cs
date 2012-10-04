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
				InitializeStatesDirections();
				AddGkDeviceFormula();
			}
			else
			{
				Formula.Add(FormulaOperationType.END);
			}
			FormulaBytes = Formula.GetBytes();
		}

		void AddGkDeviceFormula()
		{
			if (Device.Driver.HasLogic)
			{
                if (Device.DeviceLogic.Clauses.Count > 0)
				{
                    AddClauseFormula(Device.DeviceLogic, XStateType.TurnOn);
                    AddClauseFormula(Device.DeviceLogic, XStateType.TurnOff);
				}
			}

			foreach (var statesDirection in StatesDirections)
			{
				var directionsCount = 0;
				foreach (var direction in statesDirection.Directions)
				{
					Formula.AddGetBitOff(XStateType.On, direction, DatabaseType);
					if (directionsCount > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					directionsCount++;
				}
				Formula.AddGetBit(XStateType.Norm, Device, DatabaseType);
				Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
				Formula.AddPutBit(statesDirection.StateType, Device, DatabaseType);			
			}

			Formula.Add(FormulaOperationType.END);
		}

		void AddClauseFormula(XDeviceLogic deviceLogic, XStateType stateType)
		{
			var clauseIndex = 0;
			foreach (var clause in deviceLogic.Clauses)
			{
				var baseObjects = new List<XBinaryBase>();
				foreach (var device in clause.Devices)
				{
					baseObjects.Add(device);
				}
				foreach (var zone in clause.Zones)
				{
					baseObjects.Add(zone);
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
								Formula.Add(FormulaOperationType.AND, comment: "Объединение объектов по И");
								break;

							case ClauseOperationType.AnyDevice:
							case ClauseOperationType.AnyZone:
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

			var statesDirection = StatesDirections.FirstOrDefault(x => x.StateType == stateType);
			if (statesDirection != null)
			{
				foreach (var direction in statesDirection.Directions)
				{
					Formula.AddGetBitOff(XStateType.On, direction, DatabaseType);
					Formula.Add(FormulaOperationType.OR);
				}
				StatesDirections.Remove(statesDirection);
			}

			if (stateType == XStateType.TurnOff)
			{
				Formula.Add(FormulaOperationType.COM, comment: "Условие Выключения");
			}
			Formula.AddGetBit(XStateType.Norm, Device, DatabaseType);
			Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			Formula.AddPutBit(stateType, Device, DatabaseType);
		}

		List<StatesDirection> StatesDirections;
		void InitializeStatesDirections()
		{
			StatesDirections = new List<StatesDirection>();
			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				foreach (var directionDevice in direction.Devices)
				{
					if (Device.UID == directionDevice.UID)
					{
						var statesDirection = StatesDirections.FirstOrDefault(x => x.StateType == XStateType.TurnOn);
						if (statesDirection == null)
						{
							statesDirection = new StatesDirection()
							{
								StateType = XStateType.TurnOn
							};
						}
						statesDirection.Directions.Add(direction);
						StatesDirections.Add(statesDirection);
					}
				}
			}
		}

		void SetPropertiesBytes()
		{
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
                            binProperty = new BinProperty()
                            {
                                No = no
                            };
                        binProperty.Value += (ushort)(value << driverProperty.Offset);
                        binProperties.Add(binProperty);
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

	class StatesDirection
	{
		public StatesDirection()
		{
			Directions = new List<XDirection>();
		}

		public XStateType StateType { get; set; }
		public List<XDirection> Directions { get; set; }
	}
}