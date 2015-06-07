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
			foreach (var formulaOperation in FormulaOperations)
			{
				if (formulaOperation.FormulaOperationType == FormulaOperationType.GETBIT)
				{
					var stateBit = (GKStateBit)formulaOperation.FirstOperand;
					var descriptorNo = formulaOperation.SecondOperand;
					DescriptorViewModel descriptorViewModel = null;
					if(IsKauDecriptor)
						descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKBase.KAUDescriptorNo == descriptorNo);
					else
						descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.DescriptorNo == descriptorNo);
					if (descriptorViewModel != null)
					{
						var stateBitViewModel = descriptorViewModel.StateBits.FirstOrDefault(x => x.StateBit == stateBit);
						if (stateBitViewModel != null)
						{
							var bitValue = stateBitViewModel.IsActive ? 1 : 0;
							stack.Add(bitValue);
						}
					}
				}

				if (formulaOperation.FormulaOperationType == FormulaOperationType.PUTBIT)
				{

				}

				if (formulaOperation.FormulaOperationType == FormulaOperationType.END)
				{
					break;
				}
			}
		}
	}
}