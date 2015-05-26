using System.ComponentModel;

namespace PowerCalculator.Models
{
	public class Cable
	{
		public double Resistivity { get; set; }
		public double Length { get; set; }
        public double Resistance { get { return Resistivity * Length; } }
	}
}