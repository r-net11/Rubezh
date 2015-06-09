using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public InterlockConfiguration GetInterlockConfiguration()
		{
			NativeWrapper.WRAP_InterlockCfg nativeCfg;
			var result = NativeWrapper.WRAP_GetInterlockCfg(LoginID, out nativeCfg);

			return NativeInterlockCfgToInterlockConfiguration(nativeCfg);
		}

		public bool SetInterlockConfiguration(InterlockConfiguration cfg)
		{
			NativeWrapper.WRAP_InterlockCfg nativeCfg = InterlockConfigurationToNativeAntiPathBackCfg(cfg);
			return NativeWrapper.WRAP_SetInterlockCfg(LoginID, ref nativeCfg);
		}

		InterlockConfiguration NativeInterlockCfgToInterlockConfiguration(NativeWrapper.WRAP_InterlockCfg nativeCfg)
		{
			InterlockConfiguration cfg = new InterlockConfiguration();

			cfg.DoorsCount = nativeCfg.nDoorsCount;
			cfg.CanActivate = nativeCfg.bCanActivate;
			cfg.IsActivated = nativeCfg.bIsActivated;
			foreach (var mode in nativeCfg.AvailableInterlockModes)
			{
				var modeAvailability = new InterlockModeAvailability();
				modeAvailability.InterlockMode = (InterlockMode)mode.InterlockMode;
				modeAvailability.IsAvailable = mode.bIsAvailable;
				cfg.AvailableInterlockModes.Add(modeAvailability);
			}
			cfg.CurrentInterlockMode = (InterlockMode)nativeCfg.CurrentInterlockMode;

			return cfg;
		}

		NativeWrapper.WRAP_InterlockCfg InterlockConfigurationToNativeAntiPathBackCfg(InterlockConfiguration cfg)
		{
			var nativeCfg = new NativeWrapper.WRAP_InterlockCfg();

			nativeCfg.nDoorsCount = cfg.DoorsCount;
			nativeCfg.bIsActivated = cfg.IsActivated;
			nativeCfg.CurrentInterlockMode = (NativeWrapper.WRAP_InterlockMode)cfg.CurrentInterlockMode;

			return nativeCfg;
		}
	}
}