using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HexManager.Models
{
	[DataContract]
	public class HXCFileInfo
	{
		public HXCFileInfo()
		{
			FileInfos = new List<HEXFileInfo>();
		}

		[DataMember]
		public List<HEXFileInfo> FileInfos { get; set; }
	}
}