using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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
		[Description("Замок 1+2")]
		L1L2,

		[Description("Замок 1+2+3")]
		L1L2L3,

		[Description("Замок 1+2+3+4")]
		L1L2L3L4,

		[Description("Замок 2+3+4")]
		L2L3L4,

		[Description("Замок 1+3 и 2+4")]
		L1L3_L2L4,

		[Description("Замок 1+4 и 2+3")]
		L1L4_L2L3,

		[Description("Замок 3+4")]
		L3L4
	}
}