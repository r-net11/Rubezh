using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DescriptorLogicItem : BaseViewModel
	{
		public FormulaOperation FormulaOperation { get; private set; }

		public DescriptorLogicItem(FormulaOperation formulaOperation)
		{
			FormulaOperation = formulaOperation;
		}

		public string FirstOperand
		{
			get
			{
				switch(FormulaOperation.FormulaOperationType)
				{
					case FormulaOperationType.ADD:
					case FormulaOperationType.AND:
					case FormulaOperationType.COM:
					case FormulaOperationType.DUP:
					case FormulaOperationType.END:
					case FormulaOperationType.EQ:
					case FormulaOperationType.GE:
					case FormulaOperationType.GT:
					case FormulaOperationType.LE:
					case FormulaOperationType.LT:
					case FormulaOperationType.MUL:
					case FormulaOperationType.NE:
					case FormulaOperationType.NEG:
					case FormulaOperationType.OR:
					case FormulaOperationType.SUB:
					case FormulaOperationType.XOR:
						return "";
				}
				return FormulaOperation.FirstOperand.ToString();
			}
		}
	}
}