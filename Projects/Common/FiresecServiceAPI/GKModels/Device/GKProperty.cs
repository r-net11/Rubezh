using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Свойство устройств ГК
	/// </summary>
	[DataContract]
	public class GKProperty
	{
		/// <summary>
		/// Название
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Значение
		/// </summary>
		[DataMember]
		public ushort Value { get; set; }

		/// <summary>
		/// Строковое значение
		/// </summary>
		[DataMember]
		public string StringValue { get; set; }

		public GKDriverProperty DriverProperty { get; set; }
	}
}