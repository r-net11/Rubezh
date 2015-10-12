using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Шаблон параметров устройства
	/// </summary>
	[DataContract]
	public class GKDeviceParameterTemplate
	{
		/// <summary>
		/// Устройство ГК
		/// </summary>
		[DataMember]
		public GKDevice GKDevice { get; set; }
	}
}