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

		static FiresecLicenseProcessor()
		{
			InitialKey = InitialKey.Generate();
			LicenseHelper.LicenceChanged += LicenseHelper_LicenceChanged;
			if (!TryLoadLicense())
				SetDemonstration();
		}
		static void LicenseHelper_LicenceChanged(string propertyName, object oldValue, object newValue)
		{
			switch (propertyName)
			{
				case "LicenseMode":
					var oldLicenseModeValue = (LicenseMode)oldValue;
					var newLicenseModeValue = (LicenseMode)newValue;
					if (newLicenseModeValue == LicenseMode.Demonstration)
					{
						var awaiter = new Thread(() =>
						{
							if (!waitHandler.WaitOne(TimeSpan.FromHours(2)))
								SetNoLicense();
						}) { Name = "Demo Interval Awaiter", IsBackground = true };
						awaiter.Start();
					}
					if (oldLicenseModeValue == LicenseMode.Demonstration)
					{
						waitHandler.Set();
					}
					if (newLicenseModeValue == LicenseMode.HasLicense)
						GKProcessorManager.AddGKMessage(JournalEventNameType.Лицензия_обнаружена, JournalEventDescriptionType.NULL, "", null, null);
					else
						GKProcessorManager.AddGKMessage(JournalEventNameType.Отсутствует_лицензия, JournalEventDescriptionType.NULL, "", null, null);
					DiagnosticsManager.Add("LicenseMode=" + newLicenseModeValue);
					break;
			}
		}
		public static void SetDemonstration()
		{
			LicenseHelper.NumberOfUsers = 1;
			LicenseHelper.ControlScripts =
				LicenseHelper.FireAlarm =
				LicenseHelper.OrsServer =
				LicenseHelper.SecurityAlarm =
				LicenseHelper.Skd = true;
			LicenseHelper.LicenseMode = LicenseMode.Demonstration;
		}
		public static void SetNoLicense()
		{
			LicenseHelper.NumberOfUsers = 0;
			LicenseHelper.ControlScripts =
				LicenseHelper.FireAlarm =
				LicenseHelper.OrsServer =
				LicenseHelper.SecurityAlarm =
				LicenseHelper.Skd = false;
			LicenseHelper.LicenseMode = LicenseMode.NoLicense;
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
				orsServer = false;
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
					orsServer = (bool)parameter.Value;
			}
			catch (Exception e)
			{
				Logger.Error(e, "LicenseHelper.Ctrs");
				return false;
			}
			LicenseHelper.NumberOfUsers = numberOfUsers;
			LicenseHelper.FireAlarm = fireAlarm;
			LicenseHelper.SecurityAlarm = securityAlarm;
			LicenseHelper.Skd = skd;
			LicenseHelper.ControlScripts = controlScripts;
			LicenseHelper.OrsServer = orsServer;
			LicenseHelper.LicenseMode = LicenseMode.HasLicense;
			return true;
		}
	}
}
