namespace FiresecAPI.GK
{
	public class FormulaOperation
	{
		public FormulaOperationType FormulaOperationType { get; set; }
		public byte FirstOperand { get; set; }
		public ushort SecondOperand { get; set; }
		public GKBase GKBaseSecondOperand { get; set; }

		public string Comment { get; set; }
		public int StackLevel { get; set; }
	}
}