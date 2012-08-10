using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
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

			short address = 0;
			if (Device.Driver.IsDeviceOnShleif)
				address = (short)((Device.ShleifNo - 1) * 256 + Device.IntAddress);
			SetAddress(address);

			SetFormulaBytes();
			SetPropertiesBytes();
			InitializeAllBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();

			if (Device.Driver.HasLogic)
			{
				foreach (var stateLogic in Device.DeviceLogic.StateLogics)
				{
					var clauseIndex = 0;
					foreach (var clause in stateLogic.Clauses)
					{
						var baseObjects = new List<XBinaryBase>();
						foreach (var device in clause.XDevices)
						{
							baseObjects.Add(device);
						}
						foreach (var zone in clause.XZones)
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
									case ClauseOperationType.All:
										Formula.Add(FormulaOperationType.AND, comment: "Объединение объектов по И");
										break;

									case ClauseOperationType.One:
										Formula.Add(FormulaOperationType.OR, comment: "Объединение объектов по Или");
										break;
								}
							}
							objectIndex++;
						}

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

					Formula.AddPutBit(stateLogic.StateType, Device, DatabaseType);
				}
			}

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		void SetPropertiesBytes()
		{
			var binProperties = new List<BinProperty>();

			foreach (var property in Device.Properties)
			{
				var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty.IsInternalDeviceParameter)
				{
					byte no = driverProperty.No;
					short value = property.Value;

					var binProperty = binProperties.FirstOrDefault(x => x.No == no);
					if (binProperty == null)
						binProperty = new BinProperty()
						{
							No = no
						};
					binProperty.Value += (short)(value << driverProperty.Offset);
					binProperties.Add(binProperty);
				}
			}
			foreach (var property in Device.Driver.DriverTypeMappedProperties)
			{
				var binProperty = new BinProperty()
				{
					No = property.No,
					Value = property.Value
				};
				binProperties.Add(binProperty);
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
		public short Value { get; set; }
	}
}