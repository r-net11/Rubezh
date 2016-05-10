using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Versioning;
using LocalizationConveters;

namespace StrazhDeviceSDK.API
{
	public class AntiPassBackConfiguration
	{
		public AntiPassBackConfiguration()
		{
			AvailableAntiPassBackModes = new List<AntiPassBackModeAvailability>();
		}

		/// <summary>
		/// Количество дверей на контроллере
		/// </summary>
		public int DoorsCount { get; set; }

		/// <summary>
		/// Возможность активации Anti-pass Back
		/// </summary>
		public bool CanActivate { get; set; }

		/// <summary>
		/// Anti-pass Back активирован?
		/// </summary>
		public bool IsActivated { get; set; }

		/// <summary>
		/// Доступность режимов Anti-pass Back
		/// </summary>
		public List<AntiPassBackModeAvailability> AvailableAntiPassBackModes;

		/// <summary>
		/// Текущий режим Anti-pass Back
		/// </summary>
		public AntiPassBackMode CurrentAntiPassBackMode;
	}

	/// <summary>
	/// Режимы Anti-path Back
	/// </summary>
	public enum AntiPassBackMode
	{
		//[Description("Считыватель 1 - Вход, Считыватель 2 - Выход")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.AntiPassBackConfiguration), "R1In_R2Out")]
		R1In_R2Out = 0,

        //[Description("Считыватель 1 или 3- Вход, Считыватель 2 или 4 - Выход")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.AntiPassBackConfiguration), "R1R3In_R2R4Out")]
		R1R3In_R2R4Out = 1,

        //[Description("Считыватель 3 - Вход, Считыватель 4 - Выход")]
        [LocalizedDescription(typeof(ChinaSKDDriver.Resources.Language.API.AntiPassBackConfiguration), "R3In_R4Out")]
		R3In_R4Out = 2
	}

	/// <summary>
	/// Описывает возможность установки указанного режима Anti-path Back на контроллере
	/// </summary>
	public class AntiPassBackModeAvailability
	{
		/// <summary>
		/// Режим Anti-pass Back
		/// </summary>
		public AntiPassBackMode AntiPassBackMode { get; set; }

		/// <summary>
		/// Доступность режима Anti-pass Back
		/// </summary>
		public bool IsAvailable { get; set; }
	}
}