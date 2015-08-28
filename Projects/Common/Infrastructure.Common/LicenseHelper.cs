using FiresecAPI;
using System;

namespace Infrastructure.Common
{
	public static class LicenseHelper
	{
		public static event Action<string, object, object> LicenceChanged;
		static LicenseMode _licenseMode;
		public static LicenseMode LicenseMode
		{
			get { return _licenseMode; }
			set 
			{
				if (_licenseMode != value)
				{
					var oldValue = _licenseMode;
					_licenseMode = value;
					if (LicenceChanged != null)
						LicenceChanged("LicenseMode", oldValue, value);
				}
			}
		}
			
		public static int NumberOfUsers { get; set; }
		public static bool FireAlarm { get; set; }
		public static bool SecurityAlarm { get; set; }
		public static bool Skd { get; set; }
		public static bool ControlScripts { get; set; }
		public static bool OrsServer { get; set; }
	}
}