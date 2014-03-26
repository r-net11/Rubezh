namespace XFiresecAPI
{
	public interface INamedBase
	{
		int No { get; set; }
		string Name { get; set; }
		string Description { get; set; }
		string PresentationName { get; }
	}
}