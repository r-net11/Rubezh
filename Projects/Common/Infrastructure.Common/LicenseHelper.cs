using FiresecAPI;
using System;

namespace Infrastructure.Common
{
	public static class LicenseHelper
	{
		public static event Action LicenseChanged;
		public static LicenseMode LicenseMode { get; private set; }
		public static int RemoteWorkplacesCount { get; private set; }
		public static bool Fire { get; private set; }
		public static bool Security { get; private set; }
		public static bool Access { get; private set; }
		public static bool Video { get; private set; }
		public static bool OpcServer { get; private set; }

		public static bool SetLicense(LicenseMode licenseMode, int remoteWorkplacesCount, bool fire, bool security, bool access, bool video, bool opcServer)
		{
			bool isChanged =
				licenseMode != LicenseMode ||
				remoteWorkplacesCount != RemoteWorkplacesCount ||
				fire != Fire ||
				security != Security ||
				access != Access ||
				video != Video ||
				opcServer != OpcServer;

			if (isChanged)
			{
				LicenseMode = licenseMode;
				RemoteWorkplacesCount = remoteWorkplacesCount;
				Fire = fire;
				Security = security;
				Access = access;
				Video = video;
				OpcServer = opcServer;
				if (LicenseChanged != null)
					LicenseChanged();
			}

			return isChanged;
		}
	}
}