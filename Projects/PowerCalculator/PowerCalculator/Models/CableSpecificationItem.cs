
namespace PowerCalculator.Models
{
	public class CableSpecificationItem
	{
		public CableSpecificationItem()
		{
			Resistivity = 1;
			Length = 1;
		}

        public CableType CableType { get; set; }
		public double Resistivity { get; set; }
		public double Length { get; set; }
	}
}