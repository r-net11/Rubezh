using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Шаблон параметров устройств
	/// </summary>
	[DataContract]
	public class GKParameterTemplate : ModelBase
	{
		public GKParameterTemplate()
		{
			DeviceParameterTemplates = new List<GKDeviceParameterTemplate>();
		}

		/// <summary>
		/// Шаблоны параметров устройств
		/// </summary>
		[DataMember]
		public List<GKDeviceParameterTemplate> DeviceParameterTemplates { get; set; }
	}
}