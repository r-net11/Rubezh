using System.ComponentModel;
using System.Runtime.Serialization;
using Localization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDInterlockConfiguration
	{
		/// <summary>
		/// Interlock активирован?
		/// </summary>
		[DataMember]
		public bool IsActivated { get; set; }

		/// <summary>
		/// Текущий режим Interlock
		/// </summary>
		[DataMember]
		public SKDInterlockMode CurrentInterlockMode;
	}

	public enum SKDInterlockMode
	{
		//[Description("Замок 1+2")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDInterlockConfiguration), "L1L2")]
		L1L2,

        //[Description("Замок 1+2+3")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDInterlockConfiguration), "L1L2L3")]
		L1L2L3,

        //[Description("Замок 1+2+3+4")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDInterlockConfiguration), "L1L2L3L4")]
		L1L2L3L4,

        //[Description("Замок 2+3+4")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDInterlockConfiguration), "L2L3L4")]
		L2L3L4,

        //[Description("Замок 1+3 и 2+4")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDInterlockConfiguration), "L1L3_L2L4")]
		L1L3_L2L4,

        //[Description("Замок 1+4 и 2+3")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDInterlockConfiguration), "L1L4_L2L3")]
		L1L4_L2L3,

        //[Description("Замок 3+4")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Device.SKDInterlockConfiguration), "L3L4")]
		L3L4
	}
}