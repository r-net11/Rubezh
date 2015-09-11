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

		public void Resolve(CommonDatabase commonDatabase)
		{
			foreach (var formulaOperation in FormulaOperations)
			{
				if (formulaOperation.GKBaseSecondOperand != null)
				{
					var descriptor = commonDatabase.Descriptors.FirstOrDefault(x => x.GKBase == formulaOperation.GKBaseSecondOperand);
					if (descriptor != null)
					{
						if (commonDatabase.DatabaseType == DatabaseType.Gk)
							formulaOperation.SecondOperand = descriptor.GKBase.GKDescriptorNo;
						else
							formulaOperation.SecondOperand = descriptor.GKBase.KAUDescriptorNo;
					}
					//formulaOperation.SecondOperand = commonDatabase.DatabaseType == DatabaseType.Gk ? formulaOperation.GKBaseSecondOperand.GKDescriptorNo : formulaOperation.GKBaseSecondOperand.KAUDescriptorNo;
				}
			}
			if (FormulaOperations.Count == 0 || FormulaOperations.LastOrDefault().FormulaOperationType != FormulaOperationType.END)
			{
				Add(FormulaOperationType.END);
			}
		}

		public FormulaOperation Add(FormulaOperationType formulaOperationType, byte firstOperand = 0, ushort secondOperand = 0, string comment = null)
		{
			var formulaOperation = new FormulaOperation()
			{
				FormulaOperationType = formulaOperationType,
				FirstOperand = firstOperand,
				SecondOperand = secondOperand,
				Comment = comment
			};
			FormulaOperations.Add(formulaOperation);
			return formulaOperation;
		}

		public FormulaOperation AddWithGKBase(FormulaOperationType formulaOperationType, byte firstOperand, GKBase gkBase)
		{
			var formulaOperation = new FormulaOperation()
			{
				FormulaOperationType = formulaOperationType,
				FirstOperand = firstOperand,
				GKBaseSecondOperand = gkBase,
			};
			FormulaOperations.Add(formulaOperation);
			return formulaOperation;
		}

		public void AddGetBitOff(GKStateBit stateBit, GKBase gkBase)
		{
			AddWithGKBase(FormulaOperationType.GETBIT, (byte)stateBit, gkBase);
		}

		public void AddGetBit(GKStateBit stateBit, GKBase gkBase)
		{
			AddWithGKBase(FormulaOperationType.GETBIT, (byte)stateBit, gkBase);
		}

		public void AddPutBit(GKStateBit stateBit, GKBase gkBase)
		{
			AddWithGKBase(FormulaOperationType.PUTBIT, (byte)stateBit, gkBase);
		}

		public void AddArgumentPutBit(byte bit, GKBase gkBase)
		{
			AddWithGKBase(FormulaOperationType.PUTBIT, (byte)bit, gkBase);
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

		public bool HasStackOverflow()
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

		public bool CheckStackOverflow()
		{
			var branches = new List<FormulaBranch>();
			branches.Add(new FormulaBranch());

			for (int i = 0; i < FormulaOperations.Count; i++)
			{
				var formulaOperation = FormulaOperations[i];
				var newBranches = new List<FormulaBranch>();

				foreach (var branch in branches.Where(x => x.CurrentFormulaNo == i && !x.IsCompleted))
				{
					branch.CurrentFormulaNo++;

					switch (formulaOperation.FormulaOperationType)
					{
						case FormulaOperationType.CONST:
						case FormulaOperationType.DUP:
						case FormulaOperationType.GETBIT:
						case FormulaOperationType.GETBYTE:
						case FormulaOperationType.GETWORD:
						case FormulaOperationType.ACS:
						case FormulaOperationType.TSTP:
							branch.StackValues.Add(null);
							break;

						case FormulaOperationType.KOD:
						case FormulaOperationType.GETMEMB:
							branch.StackValues.Add(null);
							branch.StackValues.Add(null);
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
							branch.RemoveLast();
							break;

						case FormulaOperationType.PUTP:
						case FormulaOperationType.PUTMEMB:
							branch.RemoveLast();
							branch.RemoveLast();
							break;

						case FormulaOperationType.COM:
						case FormulaOperationType.NEG:
							break;

						case FormulaOperationType.END:
						case FormulaOperationType.EXIT:
							branch.IsCompleted = true;
							break;

						case FormulaOperationType.ACSP:
							var newBranch = branch.Clone();
							newBranches.Add(newBranch);

							branch.StackValues.Add(0);
							newBranch.StackValues.Add(null);
							newBranch.StackValues.Add(0);
							break;

						case FormulaOperationType.BR:
							if (formulaOperation.FirstOperand == 0)
							{
								branch.CurrentFormulaNo += formulaOperation.SecondOperand;
							}
							else
							{
								var lastStackValue = branch.StackValues.LastOrDefault();
								if (lastStackValue.HasValue)
								{
									if ((formulaOperation.FirstOperand == 1 && lastStackValue.Value == 0) || (formulaOperation.FirstOperand == 2 && lastStackValue.Value != 0))
									{
										branch.RemoveLast();
										branch.CurrentFormulaNo += formulaOperation.SecondOperand;
									}
								}
								else
								{
									branch.RemoveLast();
									newBranch = branch.Clone();
									newBranches.Add(newBranch);
									newBranch.CurrentFormulaNo += formulaOperation.SecondOperand;
								}
							}
							break;
					}
				}

				branches.AddRange(newBranches);
			}
			return !branches.Any(x => x.StackValues.Count != 0 || x.IsError);
		}

		public class FormulaBranch
		{
			public FormulaBranch()
			{
				StackValues = new List<int?>();
				CurrentFormulaNo = 0;
				IsCompleted = false;
			}

			public List<int?> StackValues { get; set; }

			public int CurrentFormulaNo { get; set; }

			public bool IsCompleted { get; set; }

			public bool IsError { get; set; }

			public void RemoveLast()
			{
				if (StackValues.Count > 0)
				{
					StackValues.RemoveAt(StackValues.Count - 1);
				}
				else
				{
					IsError = true;
					IsCompleted = true;
				}
			}

			public FormulaBranch Clone()
			{
				return new FormulaBranch()
				{
					StackValues = StackValues.ToList(),
					CurrentFormulaNo = CurrentFormulaNo,
					IsCompleted = IsCompleted
				};
			}
		}
	}
}