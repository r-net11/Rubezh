namespace RubezhAPI.GK
{
	public class GKMeasureParameter
	{
		public byte No { get; set; }
		public string Name { get; set; }
		public bool IsDelay { get; set; }
		public string InternalName { get; set; }
		public double? Multiplier { get; set; }
		public bool HasNegativeValue { get; set; }
		public bool IsNotVisible { get; set; }
	}
}