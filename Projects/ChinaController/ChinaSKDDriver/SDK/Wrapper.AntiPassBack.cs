using ChinaSKDDriverNativeApi;
using StrazhDeviceSDK.API;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		/// <summary>
		/// Получает настройку AntiPassBack для контроллера
		/// </summary>
		/// <returns>Настройка AntiPassBack</returns>
		public AntiPassBackConfiguration GetAntiPassBackConfiguration()
		{
			NativeWrapper.WRAP_AntiPassBackCfg nativeCfg;
			var result = NativeWrapper.WRAP_GetAntiPassBackCfg(LoginID, out nativeCfg);

			return NativeAntiPassBackCfgToAntiPassBackConfiguration(nativeCfg);
		}

		/// <summary>
		/// Устанавливает настройку AntiPassBack для контроллера
		/// </summary>
		/// <param name="cfg">Настройка AntiPassBack</param>
		/// <returns>true - операция завершилась успешно,
		/// false - операция завершилась с ошибкой</returns>
		public bool SetAntiPassBackConfiguration(AntiPassBackConfiguration cfg)
		{
			NativeWrapper.WRAP_AntiPassBackCfg nativeCfg = AntiPassBackConfigurationToNativeAntiPathBackCfg(cfg);
			return NativeWrapper.WRAP_SetAntiPassBackCfg(LoginID, ref nativeCfg);
		}

		private AntiPassBackConfiguration NativeAntiPassBackCfgToAntiPassBackConfiguration(NativeWrapper.WRAP_AntiPassBackCfg nativeCfg)
		{
			AntiPassBackConfiguration cfg = new AntiPassBackConfiguration();

			cfg.DoorsCount = nativeCfg.nDoorsCount;
			cfg.CanActivate = nativeCfg.bCanActivate;
			cfg.IsActivated = nativeCfg.bIsActivated;
			foreach (var mode in nativeCfg.AvailableAntiPassBackModes)
			{
				var modeAvailability = new AntiPassBackModeAvailability();
				modeAvailability.AntiPassBackMode = (AntiPassBackMode)mode.AntiPassBackMode;
				modeAvailability.IsAvailable = mode.bIsAvailable;
				cfg.AvailableAntiPassBackModes.Add(modeAvailability);
			}
			cfg.CurrentAntiPassBackMode = (AntiPassBackMode)nativeCfg.CurrentAntiPassBackMode;

			return cfg;
		}

		private NativeWrapper.WRAP_AntiPassBackCfg AntiPassBackConfigurationToNativeAntiPathBackCfg(AntiPassBackConfiguration cfg)
		{
			var nativeCfg = new NativeWrapper.WRAP_AntiPassBackCfg();

			nativeCfg.nDoorsCount = cfg.DoorsCount;
			nativeCfg.bIsActivated = cfg.IsActivated;
			nativeCfg.CurrentAntiPassBackMode = (NativeWrapper.WRAP_AntiPassBackMode)cfg.CurrentAntiPassBackMode;

			return nativeCfg;
		}
	}
}