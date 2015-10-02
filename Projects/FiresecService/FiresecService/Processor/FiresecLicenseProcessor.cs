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

		static LicenseMode? PreviousLicenseMode { get { return FiresecLicenseManager.PreviousLicenseInfo == null ? null : (LicenseMode?)FiresecLicenseManager.PreviousLicenseInfo.LicenseMode; } }

		static FiresecLicenseProcessor()
		{
			if (!TryLoadLicense())
				SetDemonstration();
			FiresecLicenseManager.LicenseChanged += LicenseHelper_LicenseChanged;
		}
		static void LicenseHelper_LicenseChanged()
		{
			if (FiresecLicenseManager.CurrentLicenseInfo.LicenseMode != PreviousLicenseMode)
			{
				if (PreviousLicenseMode == LicenseMode.Demonstration)
					waitHandler.Set();

				if (FiresecLicenseManager.CurrentLicenseInfo.LicenseMode == LicenseMode.HasLicense)
					GKProcessorManager.AddGKMessage(JournalEventNameType.Лицензия_обнаружена, JournalEventDescriptionType.NULL, "", null, null);
				else
					GKProcessorManager.AddGKMessage(JournalEventNameType.Отсутствует_лицензия, JournalEventDescriptionType.NULL, "", null, null);
				DiagnosticsManager.Add("LicenseMode=" + FiresecLicenseManager.CurrentLicenseInfo.LicenseMode);
			}
			FiresecService.Service.FiresecService.NotifyConfigurationChanged();
		}
		static void SetDemonstration()
		{
			FiresecLicenseManager.CurrentLicenseInfo = new FiresecLicenseInfo()
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
			FiresecLicenseManager.CurrentLicenseInfo = new FiresecLicenseInfo()
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