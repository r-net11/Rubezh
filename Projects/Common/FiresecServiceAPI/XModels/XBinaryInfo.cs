namespace XFiresecAPI
{
	public class XBinaryInfo
	{
		public string Type { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }

		public override string ToString()
		{
			return Type + " " + Name + " " + Address;
		}
	}
}