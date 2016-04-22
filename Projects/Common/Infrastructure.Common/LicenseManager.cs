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

		static FiresecLicenseInfo _currentLicenseInfo = new FiresecLicenseInfo();
		public static FiresecLicenseInfo CurrentLicenseInfo
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

		public static FiresecLicenseInfo PreviousLicenseInfo { get; private set; }

		static RubezhLicenseManager<FiresecLicenseInfo> _manager = new RubezhLicenseManager<FiresecLicenseInfo>();

		public static FiresecLicenseInfo TryLoad(string path)
		{
			return _manager.TryLoad(path, InitialKey);
		}

		public static FiresecLicenseInfo TryLoad(string path, InitialKey key)
		{
			return _manager.TryLoad(path, key);
		}

		public static bool TrySave(string path, FiresecLicenseInfo licenseInfo)
		{
			return _manager.TrySave(path, licenseInfo, InitialKey);
		}

		public static bool TrySave(string path, FiresecLicenseInfo licenseInfo, InitialKey key)
		{
			return _manager.TrySave(path, licenseInfo, key);
		}

		public static bool CheckLicense(string path)
		{
			return _manager.TryLoad(path, InitialKey) != null;
		}
	}
}
