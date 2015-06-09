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

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		public BaseDescriptor KauBaseDescriptor { get; private set; }
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
			BaseDescriptor.Build();
			var count = BaseDescriptor.Formula.FormulaOperations.Count;
			//if (count > 1)
			{
				LogicDescriptor = BaseDescriptor;
			}

			if (KauBaseDescriptor != null)
			{
				KauBaseDescriptor.Build();
				count = KauBaseDescriptor.Formula.FormulaOperations.Count;
				if (count > 1)
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

					case FormulaOperationType.DUP:
						if (stack.Any())
						{
							var currentStackValue = stack.LastOrDefault();
							stack.Add(currentStackValue);
						}
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

					case FormulaOperationType.CONST:
						stack.Add(formulaOperation.FirstOperand);
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

					case FormulaOperationType.END:
						break;

					case FormulaOperationType.EXIT:
						break;
				}
			}

			foreach (var stateBitVale in stateBitVales)
			{
				//var stateBitViewModel = StateBits.FirstOrDefault(x => x.StateBit == stateBitVale.Key);
				//if (stateBitViewModel != null)
				{
					//if (stateBitViewModel.IsActive != stateBitVale.Value)
					if (stateBitVale.Value)
					{
						//stateBitViewModel.IsActive = stateBitVale.Value;
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
			}
		}
	}
}