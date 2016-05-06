using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class Attachment : SKDModelBase
	{
		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public byte[] Data { get; set; }
	}
}
