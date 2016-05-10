using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDAntiPassBackConfiguration
	{
		/// <summary>
		/// Anti-pass Back активирован?
		/// </summary>
		[DataMember]
		public bool IsActivated { get; set; }

		/// <summary>
		/// Текущий режим Anti-pass Back
		/// </summary>
		[DataMember]
		public SKDAntiPassBackMode CurrentAntiPassBackMode;
	}

	public enum SKDAntiPassBackMode
	{
		//[Description("Считыватель 1 - Вход, Считыватель 2 - Выход")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDAntiPassBackConfiguration), "R1In_R2Out")]
		R1In_R2Out = 0,

        //[Description("Считыватель 1 или 3- Вход, Считыватель 2 или 4 - Выход")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDAntiPassBackConfiguration), "R1R3In_R2R4Out")]
		R1R3In_R2R4Out = 1,

        //[Description("Считыватель 3 - Вход, Считыватель 4 - Выход")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDAntiPassBackConfiguration), "R3In_R4Out")]
		R3In_R4Out = 2
	}
}