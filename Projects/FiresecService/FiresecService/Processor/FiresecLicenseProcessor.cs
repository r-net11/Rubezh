using FiresecAPI.Journal;
using FiresecLicense;
using GKProcessor;
using Infrastructure.Common;
using System;
using System.Threading;

namespace FiresecService.Processor
{
	public static class FiresecLicenseProcessor
	{
		static AutoResetEvent waitHandler = new AutoResetEvent(false); 

		static LicenseMode oldLicenseMode;

		static FiresecLicenseProcessor()
		{
			if (!TryLoadLicense())
				SetDemonstration();
			FiresecLicenseManager.LicenseChanged += LicenseHelper_LicenseChanged;
		}
		static void LicenseHelper_LicenseChanged()
		{
			if (FiresecLicenseManager.CurrentLicenseInfo.LicenseMode != oldLicenseMode)
			{
				if (oldLicenseMode == LicenseMode.Demonstration)
					waitHandler.Set();

				if (FiresecLicenseManager.CurrentLicenseInfo.LicenseMode == LicenseMode.HasLicense)
					GKProcessorManager.AddGKMessage(JournalEventNameType.Лицензия_обнаружена, JournalEventDescriptionType.NULL, "", null, null);
				else
					GKProcessorManager.AddGKMessage(JournalEventNameType.Отсутствует_лицензия, JournalEventDescriptionType.NULL, "", null, null);
				DiagnosticsManager.Add("LicenseMode=" + FiresecLicenseManager.CurrentLicenseInfo.LicenseMode);
				oldLicenseMode = FiresecLicenseManager.CurrentLicenseInfo.LicenseMode;
			}
			FiresecService.Service.FiresecService.NotifyConfigurationChanged();
		}
		public static void SetDemonstration()
		{
			FiresecLicenseManager.CurrentLicenseInfo = new FiresecLicenseInfo()
			{
				LicenseMode = LicenseMode.Demonstration,
				RemoteWorkplacesCount = 1,
				Fire = true,
				Security = true,
				Access = true,
				Video = true,
				OpcServer = true
			};
			var awaiter = new Thread(() =>
			{
				if (!waitHandler.WaitOne(TimeSpan.FromSeconds(30)))
					SetNoLicense();
			}) { Name = "DemoIntervalAwaiter", IsBackground = true };
			awaiter.Start();
		}
		public static void SetNoLicense()
		{
			FiresecLicenseManager.CurrentLicenseInfo = new FiresecLicenseInfo()
			{
				LicenseMode = LicenseMode.NoLicense,
				RemoteWorkplacesCount = 0,
				Fire = false,
				Security = false,
				Access = false,
				Video = false,
				OpcServer = false
			};
		}

		public static void SetLicense(FiresecLicenseInfo licenseInfo)
		{
			if (licenseInfo == null)
				SetNoLicense();
			else
				FiresecLicenseManager.CurrentLicenseInfo = licenseInfo;
		}
		
		public static bool TryLoadLicense()
		{
			var licenseInfo = FiresecLicenseManager.TryLoad(AppDataFolderHelper.GetFile("FiresecService.license"));
			if (licenseInfo == null)
				return false;
			SetLicense(licenseInfo);
			return true;
		}
	}
}
