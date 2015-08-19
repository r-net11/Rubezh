using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

		public void Resolve(DatabaseType dataBaseType)
		{
			foreach (var formulaOperation in FormulaOperations)
			{
				if (formulaOperation.GKBaseSecondOperand != null)
				{
					formulaOperation.SecondOperand = dataBaseType == DatabaseType.Gk ? formulaOperation.GKBaseSecondOperand.GKDescriptorNo : formulaOperation.GKBaseSecondOperand.KAUDescriptorNo;
				}
			}
			if (FormulaOperations.Count == 0 || FormulaOperations.LastOrDefault().FormulaOperationType != FormulaOperationType.END)
			{
				Add(FormulaOperationType.END);
			}
		}

		public FormulaOperation Add(FormulaOperationType formulaOperationType, byte firstOperand = 0, ushort secondOperand = 0, GKBase gkBase = null, string comment = null)
		{
			var formulaOperation = new FormulaOperation()
			{
				FormulaOperationType = formulaOperationType,
				FirstOperand = firstOperand,
				SecondOperand = secondOperand,
				GKBaseSecondOperand = gkBase,
				Comment = comment
			};
			FormulaOperations.Add(formulaOperation);
			return formulaOperation;
		}

		public void AddGetBitOff(GKStateBit stateBit, GKBase gkBase)
		{
			Add(FormulaOperationType.GETBIT, (byte)stateBit, 0, gkBase);
		}

		public void AddGetBit(GKStateBit stateBit, GKBase gkBase)
		{
			Add(FormulaOperationType.GETBIT, (byte)stateBit, 0, gkBase);
		}

		public void AddPutBit(GKStateBit stateBit, GKBase gkBase)
		{
			Add(FormulaOperationType.PUTBIT, (byte)stateBit, 0, gkBase);
		}

		public void AddArgumentPutBit(byte bit, GKBase gkBase)
		{
			Add(FormulaOperationType.PUTBIT, (byte)bit, 0, gkBase);
		}

		public void AddStandardTurning(GKBase gkBase)
		{
			Add(FormulaOperationType.DUP);
			AddPutBit(GKStateBit.TurnOn_InAutomatic, gkBase);
			Add(FormulaOperationType.COM);
			AddPutBit(GKStateBit.TurnOff_InAutomatic, gkBase);
		}

		public void AddClauseFormula(GKClauseGroup clauseGroup)
		{
			var clauseIndex = 0;
			foreach (var clause in clauseGroup.Clauses)
			{
				var gkBases = new List<GKBase>();
				foreach (var device in clause.Devices)
				{
					gkBases.Add(device);
				}
				foreach (var zone in clause.Zones)
				{
					gkBases.Add(zone);
				}
				foreach (var guardZone in clause.GuardZones)
				{
					gkBases.Add(guardZone);
				}
				foreach (var direction in clause.Directions)
				{
					gkBases.Add(direction);
				}
				foreach (var mpt in clause.MPTs)
				{
					gkBases.Add(mpt);
				}
				foreach (var delay in clause.Delays)
				{
					gkBases.Add(delay);
				}
				foreach (var door in clause.Doors)
				{
					gkBases.Add(door);
				}
				foreach (var pumpStation in clause.PumpStations)
				{
					gkBases.Add(pumpStation);
				}

				var objectIndex = 0;
				foreach (var gkBase in gkBases)
				{
					AddGetBitOff(clause.StateType, gkBase);

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
							case ClauseOperationType.AllDoors:
								Add(FormulaOperationType.AND, comment: "Объединение объектов по И");
								break;

							case ClauseOperationType.AnyDevice:
							case ClauseOperationType.AnyZone:
							case ClauseOperationType.AnyGuardZone:
							case ClauseOperationType.AnyDirection:
							case ClauseOperationType.AnyMPT:
							case ClauseOperationType.AnyDelay:
							case ClauseOperationType.AnyDoor:
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

		public int CalculateStackLevels(List<FormulaOperation> formulaOperations)
		{
			var stackDepth = 0;
			foreach (var formulaOperation in formulaOperations)
			{
				switch (formulaOperation.FormulaOperationType)
				{
					case FormulaOperationType.CONST:
					case FormulaOperationType.DUP:
					case FormulaOperationType.GETBIT:
					case FormulaOperationType.GETBYTE:
					case FormulaOperationType.GETWORD:
					case FormulaOperationType.ACS:
					case FormulaOperationType.TSTP:
						stackDepth += 1;
						break;

					case FormulaOperationType.KOD:
					case FormulaOperationType.ACSP:
					case FormulaOperationType.GETMEMB:
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
					case FormulaOperationType.CMPKOD:
						stackDepth -= 1;
						break;

					case FormulaOperationType.BR:
						if (formulaOperation.FirstOperand == 0)
							stackDepth += 0;
						else
							stackDepth -= 1;
						break;

					case FormulaOperationType.PUTP:
					case FormulaOperationType.PUTMEMB:
						stackDepth -= 2;
						break;

					case FormulaOperationType.COM:
					case FormulaOperationType.END:
					case FormulaOperationType.NEG:
					case FormulaOperationType.EXIT:
						stackDepth += 0;
						break;
				}
				formulaOperation.StackLevels.Add(stackDepth);
			}
			return stackDepth;
		}

		public bool CalculateStackLevels()
		{
			var stackDepths = new List<int>();
			FormulaOperations.ForEach(x => x.StackLevels = new List<int>());
			var formulaOperations = new List<FormulaOperation>(FormulaOperations);
			var unconditionalBrOperations = formulaOperations.FindAll(x => x.FormulaOperationType == FormulaOperationType.BR && x.FirstOperand == 0);
			foreach (var unconditionalBrOperation in unconditionalBrOperations)
			{
				var missFormulaOperations = formulaOperations.FindAll(x => (formulaOperations.IndexOf(x) - formulaOperations.IndexOf(unconditionalBrOperation) <= unconditionalBrOperation.SecondOperand) &&
					(formulaOperations.IndexOf(x) - formulaOperations.IndexOf(unconditionalBrOperation) > 0));
				missFormulaOperations.ForEach(x => x.StackLevels.Add(9999));
				formulaOperations.RemoveAll(x => (formulaOperations.IndexOf(x) - formulaOperations.IndexOf(unconditionalBrOperation) <= unconditionalBrOperation.SecondOperand) &&
					(formulaOperations.IndexOf(x) - formulaOperations.IndexOf(unconditionalBrOperation) > 0));
			}
			stackDepths.Add(CalculateStackLevels(formulaOperations));
			foreach (var formulaOperation in FormulaOperations)
			{
				if (formulaOperation.FormulaOperationType == FormulaOperationType.BR && formulaOperation.FirstOperand != 0)
				{
					formulaOperations = new List<FormulaOperation>(FormulaOperations);
					var missFormulaOperations = formulaOperations.FindAll(x => (formulaOperations.IndexOf(x) - formulaOperations.IndexOf(formulaOperation) <= formulaOperation.SecondOperand) &&
						(formulaOperations.IndexOf(x) - formulaOperations.IndexOf(formulaOperation) > 0));
					missFormulaOperations.ForEach(x => x.StackLevels.Add(9999));
					formulaOperations.RemoveAll(x => (formulaOperations.IndexOf(x) - formulaOperations.IndexOf(formulaOperation) <= formulaOperation.SecondOperand) &&
						(formulaOperations.IndexOf(x) - formulaOperations.IndexOf(formulaOperation) > 0));
					unconditionalBrOperations = formulaOperations.FindAll(x => x.FormulaOperationType == FormulaOperationType.BR && x.FirstOperand == 0);
					foreach (var unconditionalBrOperation in unconditionalBrOperations)
					{
						missFormulaOperations = formulaOperations.FindAll(x => (formulaOperations.IndexOf(x) - formulaOperations.IndexOf(unconditionalBrOperation) <= unconditionalBrOperation.SecondOperand) &&
							(formulaOperations.IndexOf(x) - formulaOperations.IndexOf(unconditionalBrOperation) > 0));
						missFormulaOperations.ForEach(x => x.StackLevels.Add(9999));
						formulaOperations.RemoveAll(x => (formulaOperations.IndexOf(x) - formulaOperations.IndexOf(unconditionalBrOperation) <= unconditionalBrOperation.SecondOperand) &&
							(formulaOperations.IndexOf(x) - formulaOperations.IndexOf(unconditionalBrOperation) > 0));
					}
					stackDepths.Add(CalculateStackLevels(formulaOperations));
				}
			}

			return stackDepths.Any(x => x != 0);
		}
	}
}