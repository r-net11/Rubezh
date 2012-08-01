using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace Commom.GK
{
	public class DeviceBinaryObject : BinaryObjectBase
	{
		public DeviceBinaryObject(XDevice device, DatabaseType databaseType)
		{
			DatabaseType = databaseType;
			Device = device;

			DeviceType = BytesHelper.ShortToBytes(device.Driver.DriverTypeNo);

			short address = 0;
			if (device.Driver.IsDeviceOnShleif)
				address = (short)((device.ShleifNo - 1) * 256 + device.IntAddress);
			SetAddress(address);

			SetObjectOutDependencesBytes();
			SetFormulaBytes();
			SetPropertiesBytes();
			InitializeAllBytes();
			InitializeIsGK();
		}

		void InitializeIsGK()
		{
			var kauDevice = Device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
			var gkDevice = Device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);

			foreach (var stateLogic in Device.DeviceLogic.StateLogics)
			{
				foreach (var clause in stateLogic.Clauses)
				{
					foreach (var deviceUID in clause.Devices)
					{
						var clauseDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
						var cluseKauDevice = clauseDevice.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
						var clauseGkDevice = clauseDevice.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK);
						if (kauDevice.UID != cluseKauDevice.UID)
						{
							Device.GkDatabaseParent = gkDevice;
						}
						if (gkDevice.UID != clauseGkDevice.UID)
						{
							throw (new Exception("Устройства находятся в отношении логики разных ГК"));
						}
					}

					foreach (var zoneNo in clause.Zones)
					{
						var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
					}
				}
			}
		}

		void SetObjectOutDependencesBytes()
		{
			//var inputObjects = new List<short>();

			//foreach (var stateLogic in Device.DeviceLogic.StateLogics)
			//{
			//    foreach (var clause in stateLogic.Clauses)
			//    {
			//        foreach (var deviceUID in clause.Devices)
			//        {
			//            var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			//            inputObjects.Add(device.GetDatabaseNo(DatabaseType));
			//        }
			//        foreach (var zoneNo in clause.Zones)
			//        {
			//            var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneNo);
			//            inputObjects.Add(zone.GetDatabaseNo(DatabaseType));
			//        }
			//    }
			//}

			//InputDependenses = new List<byte>();
			//foreach (var inputObject in inputObjects)
			//{
			//    InputDependenses.AddRange(BitConverter.GetBytes(inputObject));
			//}
		}

		void SetFormulaBytes()
		{
			Formula = new List<byte>();
			FormulaOperations = new List<FormulaOperation>();

			if (Device.Driver.HasLogic)
			{
				foreach (var stateLogic in Device.DeviceLogic.StateLogics)
				{
					for (int clauseIndex = 0; clauseIndex < stateLogic.Clauses.Count; clauseIndex++)
					{
						var clause = stateLogic.Clauses[clauseIndex];
						if (clause.ClauseOperandType == ClauseOperandType.Zone)
						{
							MessageBoxService.Show("Логика срабатывания по состоянию зон пока не реализована");
							continue;
						}

						if (clause.Devices.Count == 1)
						{
							var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[0]);
							GetBit(device, (byte)clause.StateType);

							AddFormulaOperation(FormulaOperationType.GETBIT,
								(byte)clause.StateType,
								device.GetDatabaseNo(DatabaseType),
								"Проверка состояния одного объекта");
						}
						else
						{
							var firstDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[0]);
							GetBit(firstDevice, (byte)clause.StateType);

							for (int i = 1; i < clause.Devices.Count; i++)
							{
								var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == clause.Devices[i]);

								AddFormulaOperation(FormulaOperationType.GETBIT,
									(byte)clause.StateType,
									device.GetDatabaseNo(DatabaseType));

								var formulaOperationType = FormulaOperationType.AND;
								switch (clause.ClauseOperationType)
								{
									case ClauseOperationType.All:
										formulaOperationType = FormulaOperationType.AND;
										break;

									case ClauseOperationType.One:
										formulaOperationType = FormulaOperationType.OR;
										break;
								}

								AddFormulaOperation(formulaOperationType,
									comment: "Проверка состояния очередного объекта объекта");
							}
						}

						if (clauseIndex + 1 < stateLogic.Clauses.Count)
						{
							var formulaOperationType = FormulaOperationType.AND;
							switch (clause.ClauseJounOperationType)
							{
								case ClauseJounOperationType.And:
									formulaOperationType = FormulaOperationType.AND;
									break;

								case ClauseJounOperationType.Or:
									formulaOperationType = FormulaOperationType.OR;
									break;
							}
							AddFormulaOperation(formulaOperationType,
								comment: "Объединение нескольких условий");
						}
					}

					AddFormulaOperation(FormulaOperationType.PUTBIT,
						(byte)stateLogic.StateType,
						Device.GetDatabaseNo(DatabaseType),
						"Запись бита глобального словосостояния");
				}
			}

			AddFormulaOperation(FormulaOperationType.END,
				comment: "Завершающий оператор");

			foreach (var formulaOperation in FormulaOperations)
			{
				Formula.Add((byte)formulaOperation.FormulaOperationType);
				Formula.Add(formulaOperation.FirstOperand);
				Formula.AddRange(BitConverter.GetBytes(formulaOperation.SecondOperand));
			}
		}

		void AddFormulaOperation(FormulaOperationType formulaOperationType, byte firstOperand = 0, short secondOperand = 0, string comment = null)
		{
			var formulaOperation = new FormulaOperation()
			{
				FormulaOperationType = formulaOperationType,
				FirstOperand = firstOperand,
				SecondOperand = secondOperand,
				Comment = comment
			};
			FormulaOperations.Add(formulaOperation);
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

		void GetBit(XDevice device, byte bitNo)
		{
			if ((bitNo == 1) || (bitNo == 2) || (bitNo == 3))
			{
				if (device.Driver.UseOffBitInLogic)
				{
					AddFormulaOperation(FormulaOperationType.GETBIT,
						(byte)6,
						device.GetDatabaseNo(DatabaseType),
						"Проверка бита обхода");

					AddFormulaOperation(FormulaOperationType.COM);
				}
			}

			AddFormulaOperation(FormulaOperationType.GETBIT,
				bitNo,
				device.GetDatabaseNo(DatabaseType));
		}
	}

	class BinProperty
	{
		public byte No { get; set; }
		public short Value { get; set; }
	}
}