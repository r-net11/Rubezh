namespace PowerCalculator.Models
{
	public class CableRepositoryItem
	{
		public CableRepositoryItem()
		{
			Lenght = 1;
		}

		public CableType CableType { get; set; }
		public int Lenght { get; set; }
	}
}