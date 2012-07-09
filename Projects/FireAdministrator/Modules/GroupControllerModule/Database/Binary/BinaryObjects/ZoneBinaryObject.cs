using System.Collections.Generic;
using XFiresecAPI;

namespace GKModule.Database
{
	public class ZoneBinaryObject : BinaryObjectBase
	{
		public ZoneBinaryObject(XZone zone, DatabaseType databaseType)
		{
			DatabaseType = databaseType;
			Zone = zone;

			DeviceType = BytesHelper.ShortToBytes((short)0x100);
			SetAddress((short)0);

			InputDependenses = new List<byte>();
			OutputDependenses = new List<byte>();
			Parameters = new List<byte>();

			SetFormulaBytes();

			InitializeAllBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new List<byte>();
			FormulaOperations = new List<FormulaOperation>();

			var count = Zone.DetectorCount;

			AppPlainFormula(XStateType.Attention);
			AppPlainFormula(XStateType.Test);
			AppPlainFormula(XStateType.Failure);
			AppFireFormula(XStateType.Fire1);
			AppFireFormula(XStateType.Fire2);
		}

		void AppPlainFormula(XStateType stateType)
		{
			for (int i = 1; i < Zone.Devices.Count; i++)
			{
				var device = Zone.Devices[i];

				AddFormulaOperation(FormulaOperationType.GETBIT,
					(byte)stateType,
					device.GetDatabaseNo(DatabaseType),
					"Проверка состояния очередного объекта");

				if ((Zone.Devices.Count > 0) || (i < Zone.Devices.Count - 1))
				{
					AddFormulaOperation(FormulaOperationType.OR);
				}
			}

			AddFormulaOperation(FormulaOperationType.PUTBIT,
				(byte)stateType,
				Zone.GetDatabaseNo(DatabaseType),
				"Запись бита глобального словосостояния");
		}

		void AppFireFormula(XStateType stateType)
		{
			AddFormulaOperation(FormulaOperationType.GETBIT,
				(byte)stateType,
				Zone.GetDatabaseNo(DatabaseType),
				"Проверка состояния самой зоны");

			AddFormulaOperation(FormulaOperationType.OR);

			for (int i = 1; i < Zone.Devices.Count; i++)
			{
				var device = Zone.Devices[i];

				if ((device.Driver.DriverType != XDriverType.HandDetector) || (device.Driver.DriverType != XDriverType.RadioHandDetector))
					continue;

				AddFormulaOperation(FormulaOperationType.GETBIT,
					(byte)stateType,
					device.GetDatabaseNo(DatabaseType),
					"Проверка состояния ручника");

				if ((Zone.Devices.Count > 0) || (i < Zone.Devices.Count - 1))
				{
					AddFormulaOperation(FormulaOperationType.OR);
				}
			}

			AddFormulaOperation(FormulaOperationType.OR);

			for (int i = 1; i < Zone.Devices.Count; i++)
			{
				var device = Zone.Devices[i];

				if ((device.Driver.DriverType == XDriverType.HandDetector) || (device.Driver.DriverType == XDriverType.RadioHandDetector))
					continue;

				AddFormulaOperation(FormulaOperationType.GETBIT,
					(byte)stateType,
					device.GetDatabaseNo(DatabaseType),
					"Проверка состояния очередного объекта");

				if ((Zone.Devices.Count > 0) || (i < Zone.Devices.Count - 1))
				{
					AddFormulaOperation(FormulaOperationType.ADD);
				}
			}

			AddFormulaOperation(FormulaOperationType.CONST, 0, Zone.DetectorCount);
			AddFormulaOperation(FormulaOperationType.GE);

			AddFormulaOperation(FormulaOperationType.OR);

			AddFormulaOperation(FormulaOperationType.PUTBIT,
				(byte)stateType,
				Zone.GetDatabaseNo(DatabaseType),
				"Запись бита глобального словосостояния");
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