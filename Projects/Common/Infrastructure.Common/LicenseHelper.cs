using FiresecAPI;
using System;

namespace Infrastructure.Common
{
	public static class LicenseHelper
	{
		public static event Action LicenseChanged;
		public static LicenseMode LicenseMode { get; private set; }
		public static int NumberOfUsers { get; private set; }
		public static bool FireAlarm { get; private set; }
		public static bool SecurityAlarm { get; private set; }
		public static bool Skd { get; private set; }
		public static bool ControlScripts { get; private set; }
		public static bool OpcServer { get; private set; }

		public static bool SetLicense(LicenseMode licenseMode, int numberOfUsers, bool fireAlarm, bool securityAlarm, bool skd, bool controlScripts, bool opcServer)
		{
			bool isChanged =
				licenseMode != LicenseMode ||
				numberOfUsers != NumberOfUsers ||
				fireAlarm != FireAlarm ||
				securityAlarm != SecurityAlarm ||
				skd != Skd ||
				controlScripts != ControlScripts ||
				opcServer != OpcServer;

			if (isChanged)
			{
				LicenseMode = licenseMode;
				NumberOfUsers = numberOfUsers;
				FireAlarm = fireAlarm;
				SecurityAlarm = securityAlarm;
				Skd = skd;
				ControlScripts = controlScripts;
				OpcServer = opcServer;
				if (LicenseChanged != null)
					LicenseChanged();
			}

			return isChanged;
		}
	}
}