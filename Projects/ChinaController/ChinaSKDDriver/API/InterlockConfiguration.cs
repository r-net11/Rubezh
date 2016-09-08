using System.Collections.Generic;
using System.ComponentModel;
using Localization.Converters;
using Localization.StrazhDeviceSDK.Common;

namespace StrazhDeviceSDK.API
{
	public class InterlockConfiguration
	{
		public InterlockConfiguration()
		{
			AvailableInterlockModes = new List<InterlockModeAvailability>();
		}

		/// <summary>
		/// Количество дверей на контроллере
		/// </summary>
		public int DoorsCount { get; set; }

		/// <summary>
		/// Возможность активации Interlock
		/// </summary>
		public bool CanActivate { get; set; }

		/// <summary>
		/// Interlock активирован?
		/// </summary>
		public bool IsActivated { get; set; }

		/// <summary>
		/// Доступность режимов Interlock
		/// </summary>
		public List<InterlockModeAvailability> AvailableInterlockModes;

		/// <summary>
		/// Текущий режим Interlock
		/// </summary>
		public InterlockMode CurrentInterlockMode;
	}

	/// <summary>
	/// Режимы Anti-path Back
	/// </summary>
	public enum InterlockMode
	{
		//[Description("Замок 1+2")]
		[LocalizedDescription(typeof(CommonResources), "Lock12")]
		L1L2,

		//[Description("Замок 1+2+3")]
		[LocalizedDescription(typeof(CommonResources), "Lock123")]
		L1L2L3,

		//[Description("Замок 1+2+3+4")]
		[LocalizedDescription(typeof(CommonResources), "Lock1234")]
		
		L1L2L3L4,

		//[Description("Замок 2+3+4")]
		[LocalizedDescription(typeof(CommonResources), "Lock234")]
		L2L3L4,

		//[Description("Замок 1+3 и 2+4")]
		[LocalizedDescription(typeof(CommonResources), "Lock1324")]
		L1L3_L2L4,

		//[Description("Замок 1+4 и 2+3")]
		[LocalizedDescription(typeof(CommonResources), "Lock1423")]
		L1L4_L2L3,

		//[Description("Замок 3+4")]
		[LocalizedDescription(typeof(CommonResources), "Lock34")]
		L3L4
	}

	/// <summary>
	/// Описывает возможность установки указанного режима Interlock на контроллере
	/// </summary>
	public class InterlockModeAvailability
	{
		/// <summary>
		/// Режим Interlock
		/// </summary>
		public InterlockMode InterlockMode { get; set; }

		/// <summary>
		/// Доступность режима Interlock
		/// </summary>
		public bool IsAvailable { get; set; }
	}
}