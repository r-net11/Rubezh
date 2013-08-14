using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FS2Api
{
	[DataContract]
	public class HEXFileInfo
	{
		public HEXFileInfo()
		{
			Lines = new List<string>();
		}

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public HexMemoryType HexMemoryType { get; set; }

		[DataMember]
		public List<string> Lines { get; set; }
	}
}