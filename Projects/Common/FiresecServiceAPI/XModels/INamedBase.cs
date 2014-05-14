namespace FiresecAPI.GK
{
	public interface INamedBase
	{
		ushort No { get; set; }
		string Name { get; set; }
		string Description { get; set; }
		string PresentationName { get; }
	}
}