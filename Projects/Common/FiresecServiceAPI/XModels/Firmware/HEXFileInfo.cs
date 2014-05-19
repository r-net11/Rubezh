using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class HEXFileInfo
	{
		public HEXFileInfo()
		{
			Lines = new List<string>();
		}

		[DataMember]
		public XDriverType DriverType { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public List<string> Lines { get; set; }
	}
}