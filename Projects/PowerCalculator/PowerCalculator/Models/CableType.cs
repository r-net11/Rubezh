using System.ComponentModel;

namespace PowerCalculator.Models
{
	public class CableType
	{
        public string Name { get; set; }
        public double Resistivity { get; set; }

        public override string ToString()
        {
            return Name;
        }
	}
}