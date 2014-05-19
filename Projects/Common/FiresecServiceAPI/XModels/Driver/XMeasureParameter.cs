namespace FiresecAPI.GK
{
	public class XMeasureParameter
	{
		public byte No { get; set; }
		public string Name { get; set; }
		public bool IsDelay { get; set; }
		public bool IsHighByte { get; set; }
		public bool IsLowByte { get; set; }
		public string InternalName { get; set; }
		public double? Multiplier { get; set; }
	}
}