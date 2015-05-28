using System.Xml.Serialization;
namespace PowerCalculator.Models
{
	public class CableRepositoryItem
	{
		public CableRepositoryItem()
		{
			Length = 1;
		}

		public double Resistivity { get; set; }
		public double Length { get; set; }

		/*[XmlIgnore]
		public double PercentsOfTotalLenght { get; set; }

		[XmlIgnore]
		public int DevicesCount { get; set; }*/
	}
}