using RubezhService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.License;
using RubezhAPI.License;
using System;
using System.IO;

namespace RubezhService
{
	static class LicensePresenter
	{
		public static void Initialize()
		{
			InitialKey = LicenseManager.InitialKey.ToString();
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			LicenseManager.LicenseChanged += RubezhLicenseManager_LicenseChanged;
			SetTitle();
		}

		static void SetTitle()
		{
			Console.Title = LicenseInfo.LicenseMode == LicenseMode.Demonstration ?
				"Сервер приложений Глобал [Демонстрационный режим]" :
				"Сервер приложений Глобал";
		}

		static public string InitialKey { get; private set; }
		static public RubezhLicenseInfo LicenseInfo { get; set; }

		static public string LoadLicense(string path)
		{
			if (!LicenseManager.CheckLicense(path))
				return "Некорректный файл лицензии";

			try
			{
				File.Copy(path, AppDataFolderHelper.GetFile("RubezhService.license"), true);
			}
			catch (Exception e)
			{
				return "Ошибка копирования файла лицензии.\n" + e.Message;
			}

			return RubezhLicenseProcessor.TryLoadLicense() ? "" : "Ошибка загрузки лицензии";
		}

		static void RubezhLicenseManager_LicenseChanged()
		{
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			SetTitle();
			PageController.OnPageChanged(Page.License);
		}
	}
}
