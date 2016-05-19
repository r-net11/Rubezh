using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.License;
using RubezhAPI.Journal;
using RubezhAPI.License;
using System;
using System.Threading;

namespace RubezhService.Processor
{
	public static class RubezhLicenseProcessor
	{
		static AutoResetEvent waitHandler = new AutoResetEvent(false);

		static LicenseMode? PreviousLicenseMode { get { return LicenseManager.PreviousLicenseInfo == null ? null : (LicenseMode?)LicenseManager.PreviousLicenseInfo.LicenseMode; } }

		static RubezhLicenseProcessor()
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
					RubezhService.Service.RubezhService.AddJournalMessage(JournalEventNameType.Лицензия_обнаружена, null, null, null);
				else
					RubezhService.Service.RubezhService.AddJournalMessage(JournalEventNameType.Отсутствует_лицензия, null, null, null);
				DiagnosticsManager.Add("LicenseMode=" + LicenseManager.CurrentLicenseInfo.LicenseMode);
			}
			RubezhService.Service.RubezhService.NotifyConfigurationChanged();
		}
		static void SetDemonstration()
		{
			LicenseManager.CurrentLicenseInfo = new RubezhLicenseInfo()
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
			LicenseManager.CurrentLicenseInfo = new RubezhLicenseInfo()
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

		public static void SetLicense(RubezhLicenseInfo licenseInfo)
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
			var licenseInfo = LicenseManager.TryLoad(AppDataFolderHelper.GetFile("RubezhService.license"));
			if (licenseInfo == null)
				return false;
			SetLicense(licenseInfo);
			return true;
		}
	}
}