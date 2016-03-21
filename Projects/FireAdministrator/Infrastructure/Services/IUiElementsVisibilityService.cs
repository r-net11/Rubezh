using FiresecAPI.Models;

namespace Infrastructure
{
	public interface IUiElementsVisibilityService
	{
		/// <summary>
		/// Видимость элемента "Главное меню \ Видео"
		/// </summary>
		bool IsMainMenuVideoElementVisible { get; }

		/// <summary>
		/// Видимость элемента "Главное меню \ Автоматизация"
		/// </summary>
		bool IsMainMenuAutomationElementVisible { get; }

		void Initialize(ILicenseData licenseData);
	}
}