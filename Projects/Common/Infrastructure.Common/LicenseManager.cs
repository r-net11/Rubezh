using RubezhAPI.License;
using RubezhLicense;
using System;

namespace Infrastructure.Common.License
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

		static RubezhLicenseInfo _currentLicenseInfo = new RubezhLicenseInfo();
		public static RubezhLicenseInfo CurrentLicenseInfo
		{
			get { return _currentLicenseInfo; }
			set
			{
				if (!object.Equals(_currentLicenseInfo, value))
				{
					PreviousLicenseInfo = _currentLicenseInfo;
					_currentLicenseInfo = value;
					if (LicenseChanged != null)
						LicenseChanged();
				}
			}
		}

		public static RubezhLicenseInfo PreviousLicenseInfo { get; private set; }

		static RubezhLicenseManager<RubezhLicenseInfo> _manager = new RubezhLicenseManager<RubezhLicenseInfo>();

		public static RubezhLicenseInfo TryLoad(string path)
		{
			return _manager.TryLoad(path, InitialKey);
		}

		public static RubezhLicenseInfo TryLoad(string path, InitialKey key)
		{
			return _manager.TryLoad(path, key);
		}

		public static bool TrySave(string path, RubezhLicenseInfo licenseInfo)
		{
			return _manager.TrySave(path, licenseInfo, InitialKey);
		}

		public static bool TrySave(string path, RubezhLicenseInfo licenseInfo, InitialKey key)
		{
			return _manager.TrySave(path, licenseInfo, key);
		}

		public static bool CheckLicense(string path)
		{
			return _manager.TryLoad(path, InitialKey) != null;
		}
	}
}
