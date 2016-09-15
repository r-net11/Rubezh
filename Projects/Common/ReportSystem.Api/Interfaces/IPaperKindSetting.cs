namespace ReportSystem.Api.Interfaces
{
	public interface IPaperKindSetting
	{
		string Name { get; }
		/// <summary>
		/// Ширина в миллиметрах
		/// </summary>
		int Width { get; }
		/// <summary>
		/// Высота в миллиметрах
		/// </summary>
		int Height { get; }
	}
}
