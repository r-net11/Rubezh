
namespace PowerCalculator.Models
{
	public class Cable
	{
		public Cable()
		{
			Resistivity = 1;
			Length = 1;
		}

		public double Resistivity { get; set; }
		public double Length { get; set; }
		public double Resistance { get { return Resistivity * Length; } }
	}
}