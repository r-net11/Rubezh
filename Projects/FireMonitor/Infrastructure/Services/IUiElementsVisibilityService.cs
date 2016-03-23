using FiresecAPI.Models;

namespace Infrastructure
{
	public interface IUiElementsVisibilityService
	{
		/// <summary>
		/// Видимость элемента главного меню "СКД / Учет рабочего времени"
		/// </summary>
		bool IsMainMenuSkdUrvElementVisible { get; }
		
		/// <summary>
		/// Видимость элемента главного меню "Отчеты / Учет рабочего времени"
		/// </summary>
		bool IsMainMenuReportsUrvElementVisible { get; }

		/// <summary>
		/// Инициализирует сервис объектом лицензии
		/// </summary>
		/// <param name="licenseData"></param>
		void Initialize(ILicenseData licenseData);
	}
}