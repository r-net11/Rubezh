using System;
using System.Collections.Generic;
using XFiresecAPI;
using FiresecAPI.Models;

namespace Common.GK
{
	public class ZoneBinaryObject : BinaryObjectBase
	{
		public ZoneBinaryObject(XZone zone, DatabaseType databaseType)
		{
			DatabaseType = databaseType;
			ObjectType = ObjectType.Zone;
			Zone = zone;
			Build();
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((short)0x100);
			SetAddress((short)0);
			Parameters = new List<byte>();
			SetFormulaBytes();
			InitializeAllBytes();
		}

		void SetFormulaBytes()
		{
			FormulaOperations = new List<FormulaOperation>();
			//AddNoneFireFormula(XStateType.Attention);
			//AddNoneFireFormula(XStateType.Test);
			//AddNoneFireFormula(XStateType.Failure);
			//AddFireFormula(XStateType.Fire1);
			//AddFireFormula(XStateType.Fire2);
			AddNewFormula();

			Formula = new List<byte>();
			foreach (var formulaOperation in FormulaOperations)
			{
				Formula.Add((byte)formulaOperation.FormulaOperationType);
				Formula.Add(formulaOperation.FirstOperand);
				Formula.AddRange(BitConverter.GetBytes(formulaOperation.SecondOperand));
			}
		}

		void AddNewFormula()
		{
			for (int i = 0; i < Zone.Devices.Count; i++)
			{
				var device = Zone.Devices[i];

				AddFormulaOperation(FormulaOperationType.GETBIT,
					(byte)XStateType.Fire1,
					device.GetDatabaseNo(DatabaseType),
					"Проверка состояния " + XStateType.Fire1.ToDescription() + " устройства " + device.ShortPresentationAddressAndDriver);
				AddFormulaOperation(FormulaOperationType.GETBIT,
					(byte)XStateType.Off,
					device.GetDatabaseNo(DatabaseType));
				AddFormulaOperation(FormulaOperationType.NEG);
				AddFormulaOperation(FormulaOperationType.AND);

				if (i > 0)
				{
					AddFormulaOperation(FormulaOperationType.ADD);
				}
			}

			AddFormulaOperation(FormulaOperationType.DUP);
			AddFormulaOperation(FormulaOperationType.CONST, 0, 1);
			AddFormulaOperation(FormulaOperationType.GE);
			AddFormulaOperation(FormulaOperationType.PUTBIT,
				(byte)XStateType.Attention,
				Zone.GetDatabaseNo(DatabaseType),
				"Запись состояния " + XStateType.Attention.ToDescription() + " зоны");

			AddFormulaOperation(FormulaOperationType.DUP);
			AddFormulaOperation(FormulaOperationType.CONST, 0, Zone.Fire1Count);
			AddFormulaOperation(FormulaOperationType.GE);
			AddFormulaOperation(FormulaOperationType.GETBIT,
				(byte)XStateType.Fire1,
				Zone.GetDatabaseNo(DatabaseType),
				"Проверка состояния " + XStateType.Fire1.ToDescription() + " зоны");
			AddFormulaOperation(FormulaOperationType.OR);
			AddFormulaOperation(FormulaOperationType.PUTBIT,
				(byte)XStateType.Fire1,
				Zone.GetDatabaseNo(DatabaseType),
				"Запись состояния " + XStateType.Fire1.ToDescription() + " зоны");

			AddFormulaOperation(FormulaOperationType.DUP);
			AddFormulaOperation(FormulaOperationType.CONST, 0, Zone.Fire2Count);
			AddFormulaOperation(FormulaOperationType.GE);
			AddFormulaOperation(FormulaOperationType.GETBIT,
				(byte)XStateType.Fire2,
				Zone.GetDatabaseNo(DatabaseType),
				"Проверка состояния " + XStateType.Fire2.ToDescription() + " зоны");
			AddFormulaOperation(FormulaOperationType.OR);
			AddFormulaOperation(FormulaOperationType.PUTBIT,
				(byte)XStateType.Fire2,
				Zone.GetDatabaseNo(DatabaseType),
				"Запись состояния " + XStateType.Fire2.ToDescription() + " зоны");
		}

		void AddNoneFireFormula(XStateType stateType)
		{
			for (int i = 0; i < Zone.Devices.Count; i++)
			{
				var device = Zone.Devices[i];

				AddFormulaOperation(FormulaOperationType.GETBIT,
					(byte)stateType,
					device.GetDatabaseNo(DatabaseType),
					"Проверка состояния " + stateType.ToDescription() + " устройства " + device.ShortPresentationAddressAndDriver);

				if (i > 0)
				{
					AddFormulaOperation(FormulaOperationType.ADD);
				}

				AddFormulaOperation(FormulaOperationType.CONST, 0, Zone.Fire1Count);
				AddFormulaOperation(FormulaOperationType.GE);
			}

			AddFormulaOperation(FormulaOperationType.PUTBIT,
				(byte)stateType,
				Zone.GetDatabaseNo(DatabaseType),
				"Запись состояния " + stateType.ToDescription() + " зоны");
		}

		void AddFireFormula(XStateType stateType)
		{
			AddFormulaOperation(FormulaOperationType.GETBIT,
				(byte)stateType,
				Zone.GetDatabaseNo(DatabaseType),
				"Проверка состояния " + stateType.ToDescription() + " зоны");

			for (int i = 0; i < Zone.Devices.Count; i++)
			{
				var device = Zone.Devices[i];

				if ((device.Driver.DriverType != XDriverType.HandDetector) && (device.Driver.DriverType != XDriverType.RadioHandDetector))
					continue;

				AddFormulaOperation(FormulaOperationType.GETBIT,
					(byte)stateType,
					device.GetDatabaseNo(DatabaseType),
					"Проверка состояния " + stateType.ToDescription() + " ручника " + device.ShortPresentationAddressAndDriver);

				AddFormulaOperation(FormulaOperationType.OR);
			}

			for (int i = 0; i < Zone.Devices.Count; i++)
			{
				var device = Zone.Devices[i];

				if ((device.Driver.DriverType == XDriverType.HandDetector) || (device.Driver.DriverType == XDriverType.RadioHandDetector))
					continue;

				AddFormulaOperation(FormulaOperationType.GETBIT,
					(byte)stateType,
					device.GetDatabaseNo(DatabaseType),
					"Проверка состояния " + stateType.ToDescription() + " устройства " + device.ShortPresentationAddressAndDriver);

				AddFormulaOperation(FormulaOperationType.ADD);
			}

			AddFormulaOperation(FormulaOperationType.CONST, 0, Zone.Fire1Count);
			AddFormulaOperation(FormulaOperationType.GE);
			AddFormulaOperation(FormulaOperationType.OR);

			AddFormulaOperation(FormulaOperationType.PUTBIT,
				(byte)stateType,
				Zone.GetDatabaseNo(DatabaseType),
				"Запись состояния " + stateType.ToDescription() + " зоны");
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
	}
}