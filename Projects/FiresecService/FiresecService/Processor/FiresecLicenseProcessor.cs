using System.Net;
using System.Net.Sockets;
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
			FiresecService.Service.FiresecService.NotifyConfigurationChanged();
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

#if DEBUG
			var host = Dns.GetHostEntry(Dns.GetHostName());
			var ip = host.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
			if (ip != null && ip.ToString() == "172.16.5.27")
			{
				LicenseHelper.SetLicense(LicenseMode.HasLicense, 10, true, true, true, true, true);
				return true;
			}
#endif
			License = LicenseProcessor.ProcessLoad(AppDataFolderHelper.GetFile("FiresecService.license"), InitialKey);
			if (License == null || License.InitialKey != InitialKey)
				return false;

			var licenseWrapper = new FiresecLicenseWrapper(License);
			if (licenseWrapper.IsNull)
				return false;

			LicenseHelper.SetLicense(LicenseMode.HasLicense,
				licenseWrapper.RemoteWorkplacesCount,
				licenseWrapper.Fire,
				licenseWrapper.Security,
				licenseWrapper.Access,
				licenseWrapper.Video,
				licenseWrapper.OpcServer);
			return true;
		}
	}
}
