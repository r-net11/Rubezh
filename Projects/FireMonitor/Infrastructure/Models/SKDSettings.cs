using System;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
	[DataContract]
	public class SKDSettings
	{
		[DataMember]
		public Guid CardCreatorReaderUID { get; set; }
	}
}