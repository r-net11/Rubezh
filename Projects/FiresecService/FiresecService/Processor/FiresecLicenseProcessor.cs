using Common;
using Defender;
using FiresecAPI;
using FiresecAPI.Journal;
using GKProcessor;
using Infrastructure.Common;
using System;
using System.Linq;
using System.Threading;

namespace FiresecService.Processor
{
	public static class FiresecLicenseProcessor
	{
		public static InitialKey InitialKey { get; set; }
		public static License License { get; set; }

		static AutoResetEvent waitHandler = new AutoResetEvent(false); 

		static LicenseMode oldLicenseMode;

		static FiresecLicenseProcessor()
		{
			InitialKey = InitialKey.Generate();
			if (!TryLoadLicense())
				SetDemonstration();
			LicenseHelper.LicenseChanged += LicenseHelper_LicenseChanged;
		}
		static void LicenseHelper_LicenseChanged()
		{
			if (LicenseHelper.LicenseMode != oldLicenseMode)
			{
				if (oldLicenseMode == LicenseMode.Demonstration)
					waitHandler.Set();

				if (LicenseHelper.LicenseMode == LicenseMode.HasLicense)
					GKProcessorManager.AddGKMessage(JournalEventNameType.Лицензия_обнаружена, JournalEventDescriptionType.NULL, "", null, null);
				else
					GKProcessorManager.AddGKMessage(JournalEventNameType.Отсутствует_лицензия, JournalEventDescriptionType.NULL, "", null, null);
				DiagnosticsManager.Add("LicenseMode=" + LicenseHelper.LicenseMode);
				oldLicenseMode = LicenseHelper.LicenseMode;
			}
			FiresecService.Service.FiresecService.NotifyLicenseChanged();
		}
		public static void SetDemonstration()
		{
			LicenseHelper.SetLicense(LicenseMode.Demonstration, 1, true, true, true, true, true);
			var awaiter = new Thread(() =>
			{
				if (!waitHandler.WaitOne(TimeSpan.FromMinutes(2)))
					SetNoLicense();
			}) { Name = "DemoIntervalAwaiter", IsBackground = true };
			awaiter.Start();
		}
		public static void SetNoLicense()
		{
			LicenseHelper.SetLicense(LicenseMode.NoLicense, 0, false, false, false, false, false);
		}
		public static bool CheckLicense(string path)
		{
			var license = LicenseProcessor.ProcessLoad(path, InitialKey);
			return license != null && license.InitialKey == InitialKey;
		}
		public static bool TryLoadLicense()
		{
			License = LicenseProcessor.ProcessLoad(AppDataFolderHelper.GetFile("FiresecService.license"), InitialKey);
			if (License == null || License.InitialKey != InitialKey)
				return false;

			int numberOfUsers = 0;
			bool fireAlarm = false,
				securityAlarm = false,
				skd = false,
				controlScripts = false,
				opcServer = false;
			try
			{
				var parameter = License.Parameters.FirstOrDefault(x => x.Id == "NumberOfUsers");
				if (parameter != null)
					numberOfUsers = (int)parameter.Value;
				parameter = License.Parameters.FirstOrDefault(x => x.Id == "FireAlarm");
				if (parameter != null)
					fireAlarm = (bool)parameter.Value;
				parameter = License.Parameters.FirstOrDefault(x => x.Id == "SecurityAlarm");
				if (parameter != null)
					securityAlarm = (bool)parameter.Value;
				parameter = License.Parameters.FirstOrDefault(x => x.Id == "Skd");
				if (parameter != null)
					skd = (bool)parameter.Value;
				parameter = License.Parameters.FirstOrDefault(x => x.Id == "ControlScripts");
				if (parameter != null)
					controlScripts = (bool)parameter.Value;
				parameter = License.Parameters.FirstOrDefault(x => x.Id == "OrsServer");
				if (parameter != null)
					opcServer = (bool)parameter.Value;
			}
			catch (Exception e)
			{
				Logger.Error(e, "LicenseHelper.Ctrs");
				return false;
			}
			LicenseHelper.SetLicense(LicenseMode.HasLicense, numberOfUsers, fireAlarm, securityAlarm, skd, controlScripts, opcServer);
			return true;
		}
	}
}
