using ReportSystem.Api.Interfaces;

namespace StrazhAPI.Printing
{
	/// <summary>
	/// Объект, описывающий структуру хранения настроек для разновидности формата листа
	/// </summary>
	public class UserKindSetting : IPaperKindSetting
	{
		public string Name { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public UserKindSetting()
		{
			Name = "Пользовательский размер листа";
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
