using System.Collections.Generic;
using System.Text;

namespace FiresecAPI.GK
{
	public class FormulaOperation
	{
		public FormulaOperation()
		{
			StackLevels = new List<int>();
		}
		public FormulaOperationType FormulaOperationType { get; set; }
		public byte FirstOperand { get; set; }
		public ushort SecondOperand { get; set; }
		public GKBase GKBaseSecondOperand { get; set; }

		public string Comment { get; set; }
		public List<int> StackLevels { get; set; }

		public string StackLevelsString
		{
			get
			{
				var stackLevelsString = "";
				if (StackLevels != null && StackLevels.Count > 0)
				{
					foreach (var stackLevel in StackLevels)
					{
						if (stackLevel == 9999)
							stackLevelsString += "   ";
						else
						{
							if (stackLevel >= 0)
								stackLevelsString += " " + stackLevel;
							else
								stackLevelsString += stackLevel;
						}
					}
				}
				return stackLevelsString;
			}
		}
	}
}