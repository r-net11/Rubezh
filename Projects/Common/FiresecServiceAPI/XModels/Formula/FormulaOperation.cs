namespace XFiresecAPI
{
    public class FormulaOperation
    {
        public FormulaOperationType FormulaOperationType { get; set; }
        public byte FirstOperand { get; set; }
        public short SecondOperand { get; set; }
        public string Comment { get; set; }
    }
}