
namespace PowerCalculator.Models
{
	public class CableSpecificationItem
	{
		public CableSpecificationItem()
		{
			Resistivity = 1;
			Length = 1;
		}

		public double Resistivity { get; set; }
		public double Length { get; set; }
	}
}