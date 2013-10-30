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

		public void AddGetBitOff(XStateBit stateType, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.GETBIT,
				(byte)stateType,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Проверка состояния " + stateType.ToDescription() + " " + XBinaryBaseToString(binaryBase));
			Add(FormulaOperationType.GETBIT,
				(byte)XStateBit.Ignore,
				binaryBase.GetDatabaseNo(DatabaseType.Gk));
			Add(FormulaOperationType.COM);
			Add(FormulaOperationType.AND);
		}

		public void AddGetBit(XStateBit stateType, XBinaryBase binaryBase)
		{
			Add(FormulaOperationType.GETBIT,
				(byte)stateType,
				binaryBase.GetDatabaseNo(DatabaseType.Gk),
				"Проверка состояния " + stateType.ToDescription() + " " + XBinaryBaseToString(binaryBase));
		}

		public void AddPutBit(XStateBit stateType, XBinaryBase binaryBase)
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
			AddGetBit(XStateBit.Norm, binaryBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(XStateBit.TurnOn_InAutomatic, binaryBase);
			Add(FormulaOperationType.COM);
			AddGetBit(XStateBit.Norm, binaryBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(XStateBit.TurnOff_InAutomatic, binaryBase);
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
			var stackDepth = 0;
			var stringBuilder = new StringBuilder();
			foreach (var formulaOperation in FormulaOperations)
			{
				stringBuilder.Append(formulaOperation.FormulaOperationType + "\t");
				stringBuilder.Append(formulaOperation.FirstOperand + "\t");
				stringBuilder.Append(formulaOperation.SecondOperand + "\t");
				stringBuilder.Append(formulaOperation.Comment + "\t");

				switch (formulaOperation.FormulaOperationType)
				{
					case FormulaOperationType.CONST:
					case FormulaOperationType.DUP:
					case FormulaOperationType.GETBIT:
					case FormulaOperationType.GETBYTE:
					case FormulaOperationType.GETWORD:
						stackDepth += 1;
						break;

					case FormulaOperationType.ADD:
					case FormulaOperationType.AND:
					case FormulaOperationType.EQ:
					case FormulaOperationType.GE:
					case FormulaOperationType.GT:
					case FormulaOperationType.LE:
					case FormulaOperationType.LT:
					case FormulaOperationType.MUL:
					case FormulaOperationType.OR:
					case FormulaOperationType.PUTBIT:
					case FormulaOperationType.PUTBYTE:
					case FormulaOperationType.PUTWORD:
					case FormulaOperationType.SUB:
					case FormulaOperationType.XOR:
						stackDepth -= 1;
						break;

					case FormulaOperationType.COM:
					case FormulaOperationType.END:
					case FormulaOperationType.NE:
					case FormulaOperationType.NEG:
						stackDepth += 0;
						break;
				}
				stringBuilder.Append(stackDepth + "\t");

				stringBuilder.AppendLine("");
			}
			return stringBuilder.ToString();
		}
	}
}