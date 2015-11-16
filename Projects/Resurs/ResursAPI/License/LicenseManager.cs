using RubezhLicense;
using System;

namespace ResursAPI.License
{
	public static class LicenseManager
	{

		public static event Action LicenseChanged;

		static InitialKey _initialKey;
		public static InitialKey InitialKey
		{
			get
			{
				if (_initialKey == null)
					_initialKey = InitialKey.Generate();
				return _initialKey;
			}
		}

		static ResursLicenseInfo _currentLicenseInfo;
		public static ResursLicenseInfo CurrentLicenseInfo
		{
			get { return _currentLicenseInfo; }
			set
			{
				if (!object.Equals(_currentLicenseInfo, value))
				{
					_currentLicenseInfo = value;
					if (LicenseChanged != null)
						LicenseChanged();
				}
			}
		}

		static RubezhLicenseManager<ResursLicenseInfo> _manager = new RubezhLicenseManager<ResursLicenseInfo>();

		public static ResursLicenseInfo TryLoad(string path)
		{
			return _manager.TryLoad(path, InitialKey);
		}

		public static ResursLicenseInfo TryLoad(string path, InitialKey key)
		{
			return _manager.TryLoad(path, key);
		}

		public static bool TrySave(string path, ResursLicenseInfo licenseInfo)
		{
			return _manager.TrySave(path, licenseInfo, InitialKey);
		}

		public static bool TrySave(string path, ResursLicenseInfo licenseInfo, InitialKey key)
		{
			return _manager.TrySave(path, licenseInfo, key);
		}

		public static bool CheckLicense(string path)
		{
			return _manager.TryLoad(path, InitialKey) != null;
		}
	}
}
