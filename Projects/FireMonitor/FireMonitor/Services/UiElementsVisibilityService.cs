using FiresecAPI.Models;
using Infrastructure;

namespace FireMonitor
{
	public class UiElementsVisibilityService : IUiElementsVisibilityService
	{
		private ILicenseData _licenseData;

		#region <Реализация интерфейса IUiElementsVisibilityService>

		/// <summary>
		/// Видимость элемента главного меню "СКД / Учет рабочего времени"
		/// </summary>
		public bool IsMainMenuSkdUrvElementVisible { get; private set; }

		/// <summary>
		/// Видимость элемента главного меню "Отчеты / Учет рабочего времени"
		/// </summary>
		public bool IsMainMenuReportsUrvElementVisible { get; private set; }

		/// <summary>
		/// Инициализирует сервис объектом лицензии
		/// </summary>
		/// <param name="licenseData"></param>
		public void Initialize(ILicenseData licenseData)
		{
			_licenseData = licenseData;
			UpdateElementsVisibility();
		}

		private void UpdateElementsVisibility()
		{
			IsMainMenuSkdUrvElementVisible = true;
			IsMainMenuReportsUrvElementVisible = true;

			if (_licenseData.IsUnlimitedUsers &&
			    _licenseData.IsEnabledURV)
				return;

			if (_licenseData.IsUnlimitedUsers &&
			    !_licenseData.IsEnabledURV)
			{
				IsMainMenuSkdUrvElementVisible = false;
				IsMainMenuReportsUrvElementVisible = false;
			}


		}

		#endregion </Реализация интерфейса IUiElementsVisibilityService>
	}
}