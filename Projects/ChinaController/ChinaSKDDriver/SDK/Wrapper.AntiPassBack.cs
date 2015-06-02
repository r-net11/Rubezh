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
		public AntiPassBackConfiguration GetAntiPassBackConfiguration()
		{
			NativeWrapper.WRAP_AntiPassBackCfg nativeCfg;
			var result = NativeWrapper.WRAP_GetAntiPassBackCfg(LoginID, out nativeCfg);

			return NativeAntiPassBackCfgToAntiPassBackConfiguration(nativeCfg);
		}

		public void SetAntiPassBackConfiguration(AntiPassBackConfiguration cfg)
		{
			NativeWrapper.WRAP_AntiPassBackCfg nativeCfg = AntiPassBackConfigurationToNativeAntiPathBackCfg(cfg);
			NativeWrapper.WRAP_SetAntiPassBackCfg(LoginID, ref nativeCfg);
		}

		AntiPassBackConfiguration NativeAntiPassBackCfgToAntiPassBackConfiguration(NativeWrapper.WRAP_AntiPassBackCfg nativeCfg)
		{
			AntiPassBackConfiguration cfg = new AntiPassBackConfiguration();

			cfg.DoorsCount = nativeCfg.nDoorsCount;
			cfg.CanActivate = nativeCfg.bCanActivate;
			cfg.IsActivated = nativeCfg.bIsActivated;
			foreach (var mode in nativeCfg.AvailableAntiPassBackModes)
			{
				var modeAvailability = new AntiPassBackModeAvailability();
				modeAvailability.AntiPassBackMode = (AntiPassBackModes)mode.AntiPassBackMode;
				modeAvailability.IsAvailable = mode.bIsAvailable;
				cfg.AvailableAntiPassBackModes.Add(modeAvailability);
			}
			cfg.CurrentAntiPassBackMode = (AntiPassBackModes)nativeCfg.CurrentAntiPassBackMode;

			return cfg;
		}

		NativeWrapper.WRAP_AntiPassBackCfg AntiPassBackConfigurationToNativeAntiPathBackCfg(AntiPassBackConfiguration cfg)
		{
			var nativeCfg = new NativeWrapper.WRAP_AntiPassBackCfg();

			nativeCfg.nDoorsCount = cfg.DoorsCount;
			nativeCfg.bIsActivated = cfg.IsActivated;
			nativeCfg.CurrentAntiPassBackMode = (NativeWrapper.WRAP_AntiPassBackMode)cfg.CurrentAntiPassBackMode;

			return nativeCfg;
		}
	}
}