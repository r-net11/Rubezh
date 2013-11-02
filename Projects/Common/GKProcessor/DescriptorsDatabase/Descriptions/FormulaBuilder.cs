using System;
using System.Collections.Generic;
using FiresecAPI;
using XFiresecAPI;

namespace GKProcessor
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

		public void AddGetBitOff(XStateBit stateBit, XBase xBase)
		{
			Add(FormulaOperationType.GETBIT,
				(byte)stateBit,
				xBase.GKDescriptorNo,
				"Проверка состояния " + stateBit.ToDescription() + " " + xBase.DescriptorInfo);
			Add(FormulaOperationType.GETBIT,
				(byte)XStateBit.Ignore,
				xBase.GKDescriptorNo);
			Add(FormulaOperationType.COM);
			Add(FormulaOperationType.AND);
		}

		public void AddGetBit(XStateBit stateBit, XBase xBase)
		{
			Add(FormulaOperationType.GETBIT,
				(byte)stateBit,
				xBase.GKDescriptorNo,
				"Проверка состояния " + stateBit.ToDescription() + " " + xBase.DescriptorInfo);
		}

		public void AddPutBit(XStateBit stateBit, XBase xBase)
		{
			Add(FormulaOperationType.PUTBIT,
				(byte)stateBit,
				xBase.GKDescriptorNo,
				"Запись состояния " + stateBit.ToDescription() + " " + xBase.DescriptorInfo);
		}

		public void AddArgumentPutBit(byte bit, XBase xBase)
		{
			Add(FormulaOperationType.PUTBIT,
				(byte)bit,
				xBase.GKDescriptorNo,
				"Запись бита-параметра " + xBase.DescriptorInfo);
		}

		public void AddStandardTurning(XBase xBase)
		{
			Add(FormulaOperationType.DUP);
			AddGetBit(XStateBit.Norm, xBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(XStateBit.TurnOn_InAutomatic, xBase);
			Add(FormulaOperationType.COM);
			AddGetBit(XStateBit.Norm, xBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(XStateBit.TurnOff_InAutomatic, xBase);
		}

		public void AddClauseFormula(XDeviceLogic deviceLogic)
		{
			var clauseIndex = 0;
			foreach (var clause in deviceLogic.Clauses)
			{
				var xBases = new List<XBase>();
				foreach (var zone in clause.Zones)
				{
					xBases.Add(zone);
				}
				foreach (var device in clause.Devices)
				{
					xBases.Add(device);
				}
				foreach (var direction in clause.Directions)
				{
					xBases.Add(direction);
				}

				var objectIndex = 0;
				foreach (var xBase in xBases)
				{
					AddGetBitOff(clause.StateType, xBase);

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

		public bool CalculateStackLevels()
		{
			var stackDepth = 0;
			foreach (var formulaOperation in FormulaOperations)
			{
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
				formulaOperation.StackLevel = stackDepth;
			}
			return stackDepth > 0;
		}
	}
}