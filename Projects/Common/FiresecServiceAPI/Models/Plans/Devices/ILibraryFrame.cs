namespace StrazhAPI.Plans.Devices
{
	public interface ILibraryFrame
	{
		int Id { get; set; }

		int Duration { get; set; }

		string Image { get; set; }
	}
}