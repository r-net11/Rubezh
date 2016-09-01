using System;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD.PassCardLibrary
{
	[DataContract]
	public class AdditionalColumnDTO
	{
		[DataMember]
		public Guid EmployeeUID { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string TextValue { get; set; }
		[DataMember]
		public byte[] GraphicValue { get; set; }
	}
}
