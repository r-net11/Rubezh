using System;
using System.Collections.Generic;
using FiresecAPI.GK;

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

		public void AddGetBitOff(GKStateBit stateBit, GKBase xBase)
		{
			Add(FormulaOperationType.GETBIT, (byte)stateBit, xBase.GKDescriptorNo);
			Add(FormulaOperationType.GETBIT, (byte)GKStateBit.Ignore, xBase.GKDescriptorNo);
			Add(FormulaOperationType.COM);
			Add(FormulaOperationType.AND);
		}

		public void AddGetBit(GKStateBit stateBit, GKBase xBase)
		{
			Add(FormulaOperationType.GETBIT, (byte)stateBit, xBase.GKDescriptorNo);
		}

		public void AddPutBit(GKStateBit stateBit, GKBase xBase)
		{
			Add(FormulaOperationType.PUTBIT, (byte)stateBit, xBase.GKDescriptorNo);
		}

		public void AddArgumentPutBit(byte bit, GKBase xBase)
		{
			Add(FormulaOperationType.PUTBIT, (byte)bit, xBase.GKDescriptorNo);
		}

		public void AddStandardTurning(GKBase xBase)
		{
			Add(FormulaOperationType.DUP);
			AddGetBit(GKStateBit.Norm, xBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(GKStateBit.TurnOn_InAutomatic, xBase);
			Add(FormulaOperationType.COM);
			AddGetBit(GKStateBit.Norm, xBase);
			Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный Устройства");
			AddPutBit(GKStateBit.TurnOff_InAutomatic, xBase);
		}

		public void AddClauseFormula(GKClauseGroup clauseGroup)
		{
			var clauseIndex = 0;
			foreach (var clause in clauseGroup.Clauses)
			{
				var xBases = new List<GKBase>();
				foreach (var device in clause.Devices)
				{
					xBases.Add(device);
				}
				foreach (var zone in clause.Zones)
				{
					xBases.Add(zone);
				}
				foreach (var guardZone in clause.GuardZones)
				{
					xBases.Add(guardZone);
				}
				foreach (var direction in clause.Directions)
				{
					xBases.Add(direction);
				}
				foreach (var mpt in clause.MPTs)
				{
					xBases.Add(mpt);
				}
				foreach (var delay in clause.Delays)
				{
					xBases.Add(delay);
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
							case ClauseOperationType.AllGuardZones:
							case ClauseOperationType.AllDirections:
							case ClauseOperationType.AllMPTs:
							case ClauseOperationType.AllDelays:
								Add(FormulaOperationType.AND, comment: "Объединение объектов по И");
								break;

							case ClauseOperationType.AnyDevice:
							case ClauseOperationType.AnyZone:
							case ClauseOperationType.AnyGuardZone:
							case ClauseOperationType.AnyDirection:
							case ClauseOperationType.AnyMPT:
							case ClauseOperationType.AnyDelay:
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
					switch (clauseGroup.ClauseJounOperationType)
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

			foreach (var group in clauseGroup.ClauseGroups)
			{
				AddClauseFormula(group);

				if (clauseIndex > 0)
				{
					switch (clauseGroup.ClauseJounOperationType)
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
					case FormulaOperationType.ACS:
						stackDepth += 1;
						break;

					case FormulaOperationType.KOD:
						stackDepth += 2;
						break;

					case FormulaOperationType.ADD:
					case FormulaOperationType.AND:
					case FormulaOperationType.EQ:
					case FormulaOperationType.NE:
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

					case FormulaOperationType.CMPKOD:
						stackDepth -= 2;
						break;

					case FormulaOperationType.COM:
					case FormulaOperationType.END:
					case FormulaOperationType.NEG:
					case FormulaOperationType.BR:
						stackDepth += 0;
						break;
				}
				formulaOperation.StackLevel = stackDepth;
			}
			return stackDepth != 0;
		}
	}
}