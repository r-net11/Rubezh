using GKProcessor;
using Infrastructure.Common.Windows;
using RubezhAPI.Journal;
using RubezhAPI.License;
using System;
using System.Threading;

namespace FiresecService.Processor
{
	public static class FiresecLicenseProcessor
	{
		static AutoResetEvent waitHandler = new AutoResetEvent(false);

		static LicenseMode? PreviousLicenseMode { get { return LicenseManager.PreviousLicenseInfo == null ? null : (LicenseMode?)LicenseManager.PreviousLicenseInfo.LicenseMode; } }

		static FiresecLicenseProcessor()
		{
			if (!TryLoadLicense())
				SetDemonstration();
			LicenseManager.LicenseChanged += LicenseHelper_LicenseChanged;
		}

		static void LicenseHelper_LicenseChanged()
		{
			if (LicenseManager.CurrentLicenseInfo.LicenseMode != PreviousLicenseMode)
			{
				if (PreviousLicenseMode == LicenseMode.Demonstration)
					waitHandler.Set();

				if (LicenseManager.CurrentLicenseInfo.LicenseMode == LicenseMode.HasLicense)
					FiresecService.Service.FiresecService.AddJournalMessage(JournalEventNameType.Лицензия_обнаружена, null, null, null);
				else
					FiresecService.Service.FiresecService.AddJournalMessage(JournalEventNameType.Отсутствует_лицензия, null, null, null);
				DiagnosticsManager.Add("LicenseMode=" + LicenseManager.CurrentLicenseInfo.LicenseMode);
			}
			FiresecService.Service.FiresecService.NotifyConfigurationChanged();
		}
		static void SetDemonstration()
		{
			LicenseManager.CurrentLicenseInfo = new FiresecLicenseInfo()
			{
				LicenseMode = LicenseMode.Demonstration,
				RemoteClientsCount = 1,
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
			LicenseManager.CurrentLicenseInfo = new FiresecLicenseInfo()
			{
				LicenseMode = LicenseMode.NoLicense,
				RemoteClientsCount = 0,
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
				LicenseManager.CurrentLicenseInfo = licenseInfo;
			}
		}

		public static bool TryLoadLicense()
		{
			var licenseInfo = LicenseManager.TryLoad(AppDataFolderHelper.GetFile("FiresecService.license"));
			if (licenseInfo == null)
				return false;
			SetLicense(licenseInfo);
			return true;
		}
	}
}