namespace PowerCalculator.Models
{
	public class CableRepositoryItem
	{
		public CableRepositoryItem()
		{
			Lenght = 1;
		}

		public Cable CableType { get; set; }
		public int Lenght { get; set; }
	}
}