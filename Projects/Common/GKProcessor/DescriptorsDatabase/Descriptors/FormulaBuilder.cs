using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RubezhAPI.GK;
using System.Text;

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
						formulaOperation.SecondOperand = (ushort)descriptor.No;
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

		public void AddGetWord(bool isHiword, GKBase gkBase)
		{
			AddWithGKBase(FormulaOperationType.GETWORD, (byte)(isHiword ? 1 : 0), gkBase);
		}

		public void AddPutBit(GKStateBit stateBit, GKBase gkBase)
		{
			AddWithGKBase(FormulaOperationType.PUTBIT, (byte)stateBit, gkBase);
		}

		public void AddPutBit(byte stateBit, GKBase gkBase)
		{
			AddWithGKBase(FormulaOperationType.PUTBIT, stateBit, gkBase);
		}

		public void AddPutWord(bool isHiword, GKBase gkBase)
		{
			AddWithGKBase(FormulaOperationType.PUTWORD, (byte)(isHiword ? 1 : 0), gkBase);
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

		public void AddMirrorLogic(GKBase gkBase, List<GKDevice> mirrorParents)
		{
			if (mirrorParents != null && mirrorParents.Count > 0)
			{
				int count = 0;
				int mirrorParentsCount = mirrorParents.Count;
				foreach (var mirrorParent in mirrorParents)
				{
					if (count > 0)
						Add(FormulaOperationType.BR, 1, 0);
					count++;
					AddGetWord(true, mirrorParent);
					Add(FormulaOperationType.CONST, 0, 0xF800);
					Add(FormulaOperationType.AND);
					Add(FormulaOperationType.DUP);
					Add(FormulaOperationType.CONST, 0, 0);
					Add(FormulaOperationType.EQ);
					Add(FormulaOperationType.BR, 1, (ushort)((mirrorParentsCount - count) * 8));
				}

				if (gkBase is GKDevice || gkBase is GKDirection || gkBase is GKDelay || gkBase is GKPumpStation || gkBase is GKMPT)
				{
					Add(FormulaOperationType.DUP);
					Add(FormulaOperationType.CONST, 0, 0x800);
					Add(FormulaOperationType.EQ);
					AddPutBit(GKStateBit.SetRegime_Automatic, gkBase);
				}

				if (gkBase is GKDevice || gkBase is GKDirection || gkBase is GKDelay || gkBase is GKPumpStation || gkBase is GKMPT)
				{
					Add(FormulaOperationType.DUP);
					Add(FormulaOperationType.CONST, 0, 0x1000);
					Add(FormulaOperationType.EQ);
					AddPutBit(GKStateBit.SetRegime_Manual, gkBase);
				}

				if (gkBase is GKGuardZone)
				{
					Add(FormulaOperationType.DUP);
					Add(FormulaOperationType.CONST, 0, 0x2000);
					Add(FormulaOperationType.EQ);
					AddPutBit(GKStateBit.TurnOn_InAutomatic, gkBase);
				}

				if (gkBase is GKGuardZone)
				{
					Add(FormulaOperationType.CONST, 0, 0x2800);
					Add(FormulaOperationType.EQ);
					AddPutBit(GKStateBit.TurnOff_InAutomatic, gkBase);
				}

				if (gkBase is GKZone)
				{
					Add(FormulaOperationType.CONST, 0, 0x4000);
					Add(FormulaOperationType.EQ);
					AddPutBit(GKStateBit.Reset, gkBase);
				}

				if (gkBase is GKDevice || gkBase is GKDirection || gkBase is GKDelay || gkBase is GKPumpStation || gkBase is GKMPT)
				{
					Add(FormulaOperationType.DUP);
					Add(FormulaOperationType.CONST, 0, 0x4800);
					Add(FormulaOperationType.EQ);
					AddPutBit(GKStateBit.TurnOn_InManual, gkBase);
				}

				if (gkBase is GKDevice || gkBase is GKDirection || gkBase is GKDelay || gkBase is GKPumpStation || gkBase is GKMPT)
				{
					Add(FormulaOperationType.CONST, 0, 0x5000);
					Add(FormulaOperationType.EQ);
					AddPutBit(GKStateBit.TurnOff_InManual, gkBase);
				}
			}
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
							case ClauseOperationType.AllPumpStations:
								Add(FormulaOperationType.AND, comment: "Объединение объектов по И");
								break;

							case ClauseOperationType.AnyDevice:
							case ClauseOperationType.AnyZone:
							case ClauseOperationType.AnyGuardZone:
							case ClauseOperationType.AnyDirection:
							case ClauseOperationType.AnyMPT:
							case ClauseOperationType.AnyDelay:
							case ClauseOperationType.AnyDoor:
							case ClauseOperationType.AnyPumpStation:
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

		public bool CheckStackOverflow()
		{
			return !GetBranches().Any(x => x.StackValues.Count != 0 || x.IsError);
		}

		public List<FormulaBranch> GetBranches()
		{
			var branches = new List<FormulaBranch>();
			branches.Add(new FormulaBranch());

			for (int i = 0; i < FormulaOperations.Count; i++)
			{
				var formulaOperation = FormulaOperations[i];
				var newBranches = new List<FormulaBranch>();

				//var stackValuesHashes = new HashSet<string>();
				//foreach (var branch in branches.Where(x => x.CurrentFormulaNo == i && !x.IsCompleted))
				//{
				//	if (!stackValuesHashes.Add(branch.StackValuesHash))
				//		branch.IsCompleted = true;
				//}

				foreach (var branch in branches.Where(x => x.CurrentFormulaNo == i && !x.IsCompleted))
				{
					switch (formulaOperation.FormulaOperationType)
					{
						case FormulaOperationType.CONST:
						case FormulaOperationType.DUP:
						case FormulaOperationType.GETBIT:
						case FormulaOperationType.GETBYTE:
						case FormulaOperationType.ACS:
						case FormulaOperationType.GETWORD:
							branch.StackValues.Add(null);
							branch.CalculateStackDepth();
							branch.CurrentFormulaNo += 1;
							break;

						case FormulaOperationType.KOD:
						case FormulaOperationType.GETMEMB:
							branch.StackValues.Add(null);
							branch.StackValues.Add(null);
							branch.CalculateStackDepth();
							branch.CurrentFormulaNo += 1;
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
						case FormulaOperationType.SUB:
						case FormulaOperationType.XOR:
						case FormulaOperationType.CMPKOD:
						case FormulaOperationType.PUTWORD:
							branch.RemoveLast();
							branch.CalculateStackDepth();
							branch.CurrentFormulaNo += 1;
							break;
						
						case FormulaOperationType.PUTP:
						case FormulaOperationType.PUTMEMB:
							branch.RemoveLast();
							branch.RemoveLast();
							branch.CalculateStackDepth();
							branch.CurrentFormulaNo += 1;
							break;

						case FormulaOperationType.COM:
						case FormulaOperationType.NEG:
							branch.CalculateStackDepth();
							branch.CurrentFormulaNo += 1;
							break;

						case FormulaOperationType.END:
						case FormulaOperationType.EXIT:
							branch.IsCompleted = true;
							branch.CalculateStackDepth();
							break;

						case FormulaOperationType.ACSP:
							var newBranch = branch.Clone();
							newBranches.Add(newBranch);

							branch.StackValues.Add(0);
							newBranch.StackValues.Add(null);
							newBranch.StackValues.Add(1);

							branch.CalculateStackDepth();
							branch.CurrentFormulaNo += 1;
							newBranch.CalculateStackDepth();
							newBranch.CurrentFormulaNo += 1;
							break;

						case FormulaOperationType.TSTP:
							newBranch = branch.Clone();
							newBranches.Add(newBranch);

							branch.RemoveLast();
							branch.StackValues.Add(0);
							newBranch.RemoveLast();
							newBranch.StackValues.Add(null);
							newBranch.StackValues.Add(1);

							branch.CalculateStackDepth();
							branch.CurrentFormulaNo += 1;
							newBranch.CalculateStackDepth();
							newBranch.CurrentFormulaNo += 1;
							break;

						case FormulaOperationType.BR:
							if (formulaOperation.FirstOperand == 0)
							{
								branch.CalculateStackDepth();
								branch.CurrentFormulaNo += 1;
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
										branch.CalculateStackDepth();
										branch.CurrentFormulaNo += 1;
										branch.CurrentFormulaNo += formulaOperation.SecondOperand;
									}
									else
									{
										branch.RemoveLast();
										branch.CalculateStackDepth();
										branch.CurrentFormulaNo += 1;
									}
								}
								else
								{
									branch.RemoveLast();
									newBranch = branch.Clone();

									branch.CalculateStackDepth();
									branch.CurrentFormulaNo += 1;

									newBranches.Add(newBranch);
									newBranch.CalculateStackDepth();
									newBranch.CurrentFormulaNo += 1;
									newBranch.CurrentFormulaNo += formulaOperation.SecondOperand;
								}
							}

							break;
					}
				}

				branches.AddRange(newBranches);
			}
			return branches;
		}

		public class FormulaBranch
		{
			public FormulaBranch()
			{
				StackValues = new List<int?>();
				CurrentFormulaNo = 0;
				IsCompleted = false;
				StackDepthHistory = new List<Tuple<int, int>>();
			}

			public List<int?> StackValues { get; set; }

			public int CurrentFormulaNo { get; set; }

			public bool IsCompleted { get; set; }

			public bool IsError { get; set; }

			public List<Tuple<int, int>> StackDepthHistory { get; set; }

			public string StackValuesHash
			{
				get
				{
					var stringBuilder = new StringBuilder();
					foreach (var stackValue in StackValues)
					{
						if (stackValue.HasValue)
							stringBuilder.Append(stackValue.Value + " ");
						else
							stringBuilder.Append("null ");
					}
					return stringBuilder.ToString();
				}
			}

			public void CalculateStackDepth()
			{
				StackDepthHistory.Add(new Tuple<int, int>(CurrentFormulaNo, StackValues.Count));
			}

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