using FiresecService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.License;
using RubezhAPI.License;
using System;
using System.IO;

namespace FiresecService
{
	static class LicensePresenter
	{
		public static void Initialize()
		{
			InitialKey = LicenseManager.InitialKey.ToString();
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			LicenseManager.LicenseChanged += FiresecLicenseManager_LicenseChanged;
			SetTitle();
		}

		static void SetTitle()
		{
			Console.Title = LicenseInfo.LicenseMode == LicenseMode.Demonstration ?
				"Сервер приложений Глобал [Демонстрационный режим]" :
				"Сервер приложений Глобал";
		}

		static public string InitialKey { get; private set; }
		static public FiresecLicenseInfo LicenseInfo { get; set; }

		static public string LoadLicense(string path)
		{
			if (!LicenseManager.CheckLicense(path))
				return "Некорректный файл лицензии";

			try
			{
				File.Copy(path, AppDataFolderHelper.GetFile("FiresecService.license"), true);
			}
			catch (Exception e)
			{
				return "Ошибка копирования файла лицензии.\n" + e.Message;
			}

			return FiresecLicenseProcessor.TryLoadLicense() ? "" : "Ошибка загрузки лицензии";
		}

		static void FiresecLicenseManager_LicenseChanged()
		{
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			SetTitle();
			PageController.OnPageChanged(Page.License);
		}
	}
}
