using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Journal;
using System.Collections.Generic;
using System.Diagnostics;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		public BaseDescriptor LogicDescriptor { get; private set; }
		public List<FormulaOperation> FormulaOperations { get; protected set; }
		bool IsKauDecriptor;

		public void SetKauDescriptor(BaseDescriptor kauBaseDescriptor)
		{
			KauBaseDescriptor = kauBaseDescriptor;
			InitializeLogic();
			FormulaOperations = LogicDescriptor.Formula.FormulaOperations;
		}

		public void InitializeLogic()
		{
			GKBaseDescriptor.Build();
			LogicDescriptor = GKBaseDescriptor;

			if (KauBaseDescriptor != null)
			{
				KauBaseDescriptor.Build();
				if (KauBaseDescriptor.Formula.FormulaOperations.Count > 1)
				{
					LogicDescriptor = KauBaseDescriptor;
					IsKauDecriptor = true;
				}
			}
		}

		void RecalculateOutputLogic()
		{
			foreach (var gkBase in LogicDescriptor.GKBase.OutputGKBases)
			{
				var descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.DescriptorNo == gkBase.GKDescriptorNo);
				if (descriptorViewModel != null)
				{
					descriptorViewModel.RecalculateLogic();
				}
			}
		}

		void RecalculateLogic()
		{
			var stack = new List<int>();
			var stateBitVales = new Dictionary<GKStateBit, bool>();

			foreach (var formulaOperation in FormulaOperations)
			{
				var stateBit = (GKStateBit)formulaOperation.FirstOperand;
				var descriptorNo = formulaOperation.SecondOperand;
				DescriptorViewModel descriptorViewModel = null;
				if (IsKauDecriptor)
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKBase.KAUDescriptorNo == descriptorNo);
				else
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.DescriptorNo == descriptorNo);

				switch (formulaOperation.FormulaOperationType)
				{
					case FormulaOperationType.GETBIT:
						if (descriptorViewModel != null)
						{
							var stateBitViewModel = descriptorViewModel.StateBits.FirstOrDefault(x => x.StateBit == stateBit);
							if (stateBitViewModel != null)
							{
								var bitValue = stateBitViewModel.IsActive ? 1 : 0;
								stack.Add(bitValue);
							}
						}
						break;

					case FormulaOperationType.PUTBIT:
						if (stack.Any())
						{
							var currentStackValue = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);
							if (stateBitVales.ContainsKey(stateBit))
								stateBitVales[stateBit] = currentStackValue != 0;
							else
								stateBitVales.Add(stateBit, currentStackValue != 0);
						}
						break;

					case FormulaOperationType.CONST:
						stack.Add(formulaOperation.SecondOperand);
						break;

					case FormulaOperationType.GETBYTE:
						break;

					case FormulaOperationType.PUTBYTE:
						break;

					case FormulaOperationType.GETWORD:
						break;

					case FormulaOperationType.PUTWORD:
						break;

					case FormulaOperationType.ADD:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = currentStackValue1 + currentStackValue2;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.SUB:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = currentStackValue1 - currentStackValue2;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.MUL:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = currentStackValue1 * currentStackValue2;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.PUTP:
						break;

					case FormulaOperationType.OR:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue1 != 0 || currentStackValue2 != 0) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.AND:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue1 != 0 && currentStackValue2 != 0) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.XOR:
						break;

					case FormulaOperationType.COM:
						if (stack.Any())
						{
							var currentStackValue = stack.LastOrDefault();
							currentStackValue = currentStackValue != 0 ? 0 : 1;
							stack.RemoveAt(stack.Count - 1);
							stack.Add(currentStackValue);
						}
						break;

					case FormulaOperationType.NEG:
						break;

					case FormulaOperationType.EQ:
						break;

					case FormulaOperationType.NE:
						break;

					case FormulaOperationType.GT:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue2 > currentStackValue1) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.GE:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue2 >= currentStackValue1) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.LT:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue2 < currentStackValue1) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.LE:
						if (stack.Count > 1)
						{
							var currentStackValue1 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var currentStackValue2 = stack.LastOrDefault();
							stack.RemoveAt(stack.Count - 1);

							var newStackValue = (currentStackValue2 <= currentStackValue1) ? 1 : 0;
							stack.Add(newStackValue);
						}
						break;

					case FormulaOperationType.TSTP:
						break;

					case FormulaOperationType.CMPKOD:
						break;

					case FormulaOperationType.KOD:
						break;

					case FormulaOperationType.ACS:
						break;

					case FormulaOperationType.EXIT:
						break;

					case FormulaOperationType.BR:
						break;

					case FormulaOperationType.DUP:
						if (stack.Any())
						{
							var currentStackValue = stack.LastOrDefault();
							stack.Add(currentStackValue);
						}
						break;

					case FormulaOperationType.END:
						break;

					case FormulaOperationType.ACSP:
						break;

					case FormulaOperationType.PUTMEMB:
						break;

					case FormulaOperationType.GETMEMB:
						break;
				}

				Trace.WriteLine(formulaOperation.FormulaOperationType + "\t" + string.Join(" ", stack));
			}

			var hasZoneBitsChanged = false;

			foreach (var stateBitVale in stateBitVales)
			{
				if (stateBitVale.Key == GKStateBit.Attention || stateBitVale.Key == GKStateBit.Fire1 || stateBitVale.Key == GKStateBit.Fire2)
				{
					var stateBitViewModel = StateBits.FirstOrDefault(x => x.StateBit == stateBitVale.Key);
					if (stateBitViewModel != null)
					{
						if (stateBitViewModel.IsActive != stateBitVale.Value)
						{
							stateBitViewModel.IsActive = stateBitVale.Value;
							hasZoneBitsChanged = true;
						}
					}
				}

				if (stateBitVale.Value)
				{
					if (stateBitVale.Key == GKStateBit.TurnOn_InAutomatic)
					{
						if (Regime == Regime.Automatic)
						{
							OnTurnOn();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOnNow_InAutomatic)
					{
						if (Regime == Regime.Automatic)
						{
							OnTurnOnNow();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOff_InAutomatic)
					{
						if (Regime == Regime.Automatic)
						{
							OnTurnOff();
						}
					}
					if (stateBitVale.Key == GKStateBit.TurnOffNow_InAutomatic)
					{
						if (Regime == Regime.Automatic)
						{
							OnTurnOffNow();
						}
					}
				}
			}

			if (GKBase is GKZone && hasZoneBitsChanged)
			{
				if (stateBitVales.ContainsKey(GKStateBit.Attention) && stateBitVales[GKStateBit.Attention])
				{
					var journalItem = new ImitatorJournalItem(2, 4, 0, 0);
					AddJournalItem(journalItem);
				}
				else if (stateBitVales.ContainsKey(GKStateBit.Fire1) && stateBitVales[GKStateBit.Fire1])
				{
					var journalItem = new ImitatorJournalItem(2, 2, 0, 0);
					AddJournalItem(journalItem);
				}
				else if (stateBitVales.ContainsKey(GKStateBit.Fire2) && stateBitVales[GKStateBit.Fire2])
				{
					var journalItem = new ImitatorJournalItem(2, 3, 0, 0);
					AddJournalItem(journalItem);
				}
				else
				{
					var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
					AddJournalItem(journalItem);
				}
			}
		}
	}
}