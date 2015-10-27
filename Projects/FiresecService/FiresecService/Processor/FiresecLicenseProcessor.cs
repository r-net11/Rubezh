using RubezhAPI.Journal;
using RubezhLicense;
using GKProcessor;
using Infrastructure.Common;
using System;
using System.Threading;
using RubezhAPI.License;

namespace FiresecService.Processor
{
	public static class FiresecLicenseProcessor
	{
		static AutoResetEvent waitHandler = new AutoResetEvent(false);

		static LicenseMode? PreviousLicenseMode { get { return LicenseManager.FiresecLicenseManager.PreviousLicenseInfo == null ? null : (LicenseMode?)LicenseManager.FiresecLicenseManager.PreviousLicenseInfo.LicenseMode; } }

		static FiresecLicenseProcessor()
		{
			if (!TryLoadLicense())
				SetDemonstration();
			LicenseManager.FiresecLicenseManager.LicenseChanged += LicenseHelper_LicenseChanged;
		}

		static void LicenseHelper_LicenseChanged()
		{
			if (LicenseManager.FiresecLicenseManager.CurrentLicenseInfo.LicenseMode != PreviousLicenseMode)
			{
				if (PreviousLicenseMode == LicenseMode.Demonstration)
					waitHandler.Set();

				if (LicenseManager.FiresecLicenseManager.CurrentLicenseInfo.LicenseMode == LicenseMode.HasLicense)
					FiresecService.Service.FiresecService.InsertJournalMessage(JournalEventNameType.Лицензия_обнаружена, null, JournalEventDescriptionType.NULL);
				else
					FiresecService.Service.FiresecService.InsertJournalMessage(JournalEventNameType.Отсутствует_лицензия, null, JournalEventDescriptionType.NULL);
				DiagnosticsManager.Add("LicenseMode=" + LicenseManager.FiresecLicenseManager.CurrentLicenseInfo.LicenseMode);
			}
			FiresecService.Service.FiresecService.NotifyConfigurationChanged();
		}
		static void SetDemonstration()
		{
			LicenseManager.FiresecLicenseManager.CurrentLicenseInfo = new FiresecLicenseInfo()
			{
				LicenseMode = LicenseMode.Demonstration,
				RemoteWorkplacesCount = 1,
				HasFirefighting = true,
				HasGuard = true,
				HasSKD = true,
				HasVideo = true,
				HasOpcServer = true
			};
			var awaiter = new Thread(() =>
			{
				if (!waitHandler.WaitOne(TimeSpan.FromHours(2)))
					SetNoLicense();
			}) { Name = "DemoIntervalAwaiter", IsBackground = true };
			awaiter.Start();
		}
		static void SetNoLicense()
		{
			LicenseManager.FiresecLicenseManager.CurrentLicenseInfo = new FiresecLicenseInfo()
			{
				LicenseMode = LicenseMode.NoLicense,
				RemoteWorkplacesCount = 0,
				HasFirefighting = false,
				HasGuard = false,
				HasSKD = false,
				HasVideo = false,
				HasOpcServer = false
			};
		}

		public static void SetLicense(FiresecLicenseInfo licenseInfo)
		{
			if (licenseInfo == null)
				SetNoLicense();
			else
			{
				licenseInfo.LicenseMode = LicenseMode.HasLicense;
				LicenseManager.FiresecLicenseManager.CurrentLicenseInfo = licenseInfo;
			}
		}

		public static bool TryLoadLicense()
		{
			var licenseInfo = LicenseManager.FiresecLicenseManager.TryLoad(AppDataFolderHelper.GetFile("FiresecService.license"));
			if (licenseInfo == null)
				return false;
			SetLicense(licenseInfo);
			return true;
		}
	}
}