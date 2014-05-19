using System;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
	[DataContract]
	public class SKDSettings
	{
		public SKDSettings()
		{

		}

		[DataMember]
		public Guid CardCreatorReaderUID { get; set; }
	}
}