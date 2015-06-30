
namespace PowerCalculator.Models
{
	public class Cable
	{
		public Cable()
		{
			Length = 1;
		}

        public CableType CableType { get; set; }
		public double Resistivity { get; set; }
		public double Length { get; set; }
		public double Resistance { get { return Resistivity * Length; } }
	}
}