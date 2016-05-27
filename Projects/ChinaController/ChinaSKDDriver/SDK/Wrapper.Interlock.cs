using ChinaSKDDriverNativeApi;
using StrazhDeviceSDK.API;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		/// <summary>
		/// Получает настройку Interlock на контроллере
		/// </summary>
		/// <returns>Настройка Interlock</returns>
		public InterlockConfiguration GetInterlockConfiguration()
		{
			NativeWrapper.WRAP_InterlockCfg nativeCfg;
			NativeWrapper.WRAP_GetInterlockCfg(LoginID, out nativeCfg);

			return NativeInterlockCfgToInterlockConfiguration(nativeCfg);
		}

		/// <summary>
		/// Устанавливает настройку Interlock на контроллере
		/// </summary>
		/// <param name="cfg">Настройка Interlock</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибкой</returns>
		public bool SetInterlockConfiguration(InterlockConfiguration cfg)
		{
			NativeWrapper.WRAP_InterlockCfg nativeCfg = InterlockConfigurationToNativeInterlockCfg(cfg);
			return NativeWrapper.WRAP_SetInterlockCfg(LoginID, ref nativeCfg);
		}

		private InterlockConfiguration NativeInterlockCfgToInterlockConfiguration(NativeWrapper.WRAP_InterlockCfg nativeCfg)
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

		private NativeWrapper.WRAP_InterlockCfg InterlockConfigurationToNativeInterlockCfg(InterlockConfiguration cfg)
		{
			var nativeCfg = new NativeWrapper.WRAP_InterlockCfg();

			nativeCfg.nDoorsCount = cfg.DoorsCount;
			nativeCfg.bIsActivated = cfg.IsActivated;
			nativeCfg.CurrentInterlockMode = (NativeWrapper.WRAP_InterlockMode)cfg.CurrentInterlockMode;

			return nativeCfg;
		}
	}
}