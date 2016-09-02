namespace ReportSystem.Api.Interfaces
{
	public interface IPaperKindSetting
	{
		string Name { get; }
		int Width { get; }
		int Height { get; }
		bool IsSystem { get; }
	}
}
