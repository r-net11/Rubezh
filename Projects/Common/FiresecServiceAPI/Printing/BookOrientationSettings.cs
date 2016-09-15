using ReportSystem.Api.Interfaces;

namespace StrazhAPI.Printing
{
	public class BookOrientationSettings : IPaperKindSetting
	{
		public string Name { get; private set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public BookOrientationSettings()
		{
			Name = "A4 Книжная ориентация";
			Width = 210;
			Height = 297;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
