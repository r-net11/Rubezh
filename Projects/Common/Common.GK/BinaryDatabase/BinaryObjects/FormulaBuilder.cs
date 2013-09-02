using System;
using System.Collections.Generic;
using System.Text;
using FiresecAPI;
using XFiresecAPI;

namespace Common.GK
{
	public class FormulaBuilder
	{
		public List<FormulaOperation> FormulaOperations { get; protected set; }

		public FormulaBuilder()
		{
			FormulaOperations = new List<FormulaOperation>();
		}

		public void Add(FormulaOperationType formulaOperationType, byte firstOperand = 0, ushort secondOperand = 0, string comment = null)
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

		public void AddGetBitOff(XStateType stateType, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.GETBIT,
				(byte)stateType,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Проверка состояния " + stateType.ToDescription() + " " + XBinaryBaseToString(binaryBase));
			Add(FormulaOperationType.GETBIT,
				(byte)XStateType.Ignore,
				binaryBase.GetDatabaseNo(DatabaseType.Gk));
			Add(FormulaOperationType.COM);
			Add(FormulaOperationType.AND);
		}

		public void AddGetBit(XStateType stateType, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.GETBIT,
				(byte)stateType,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Проверка состояния " + stateType.ToDescription() + " " + XBinaryBaseToString(binaryBase));
		}

		public void AddPutBit(XStateType stateType, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.PUTBIT,
				(byte)stateType,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Запись состояния " + stateType.ToDescription() + " " + XBinaryBaseToString(binaryBase));
		}

		public void AddArgumentPutBit(byte bit, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.PUTBIT,
				(byte)bit,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Запись бита-параметра " + XBinaryBaseToString(binaryBase));
		}

		public void AddStandardTurning(XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.DUP);
			AddGetBit(XStateType.Norm, binaryBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(XStateType.TurnOn_InAutomatic, binaryBase);
			Add(FormulaOperationType.COM);
			AddGetBit(XStateType.Norm, binaryBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(XStateType.TurnOff_InAutomatic, binaryBase);
		}

		public void AddClauseFormula(XDeviceLogic deviceLogic)
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
					AddGetBitOff(clause.StateType, baseObject);

					if (objectIndex > 0)
					{
						switch (clause.ClauseOperationType)
						{
							case ClauseOperationType.AllDevices:
							case ClauseOperationType.AllZones:
							case ClauseOperationType.AllDirections:
								Add(FormulaOperationType.AND, comment: "Объединение объектов по И");
								break;

							case ClauseOperationType.AnyDevice:
							case ClauseOperationType.AnyZone:
							case ClauseOperationType.AnyDirection:
								Add(FormulaOperationType.OR, comment: "Объединение объектов по Или");
								break;
						}
					}
					objectIndex++;
				}

				if (clause.ClauseConditionType == ClauseConditionType.IfNot)
					Add(FormulaOperationType.COM, comment: "Условие Если НЕ");

				if (clauseIndex > 0)
				{
					switch (clause.ClauseJounOperationType)
					{
						case ClauseJounOperationType.And:
							Add(FormulaOperationType.AND, comment: "Объединение нескольких условий по И");
							break;

						case ClauseJounOperationType.Or:
							Add(FormulaOperationType.OR, comment: "Объединение нескольких условий по ИЛИ");
							break;
					}
				}
				clauseIndex++;
			}
		}

		string XBinaryBaseToString(XBinaryBase binaryBase)
		{
			return binaryBase.BinaryInfo.Type + " " + binaryBase.BinaryInfo.Name + " " + binaryBase.BinaryInfo.Address;
		}

		public List<byte> GetBytes()
		{
			var bytes = new List<byte>();
			foreach (var formulaOperation in FormulaOperations)
			{
				bytes.Add((byte)formulaOperation.FormulaOperationType);
				bytes.Add(formulaOperation.FirstOperand);
				bytes.AddRange(BitConverter.GetBytes(formulaOperation.SecondOperand));
			}
			return bytes;
		}

		public string GetStringFomula()
		{
			var stringBuilder = new StringBuilder();
			foreach (var formulaOperation in FormulaOperations)
			{
				stringBuilder.Append(formulaOperation.FormulaOperationType + "\t");
				stringBuilder.Append(formulaOperation.FirstOperand + "\t");
				stringBuilder.Append(formulaOperation.SecondOperand + "\t");
				stringBuilder.Append(formulaOperation.Comment);
				stringBuilder.AppendLine("");
			}
			return stringBuilder.ToString();
		}
	}
}