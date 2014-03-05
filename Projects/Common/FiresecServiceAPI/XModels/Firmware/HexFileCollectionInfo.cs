using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class HexFileCollectionInfo
	{
		public HexFileCollectionInfo()
		{
			HexFileInfos = new List<HEXFileInfo>();
			Name = "Обновление прошивки";
			MinorVersion = 1;
			MajorVersion = 1;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int MinorVersion { get; set; }

		[DataMember]
		public int MajorVersion { get; set; }

		[DataMember]
		public List<HEXFileInfo> HexFileInfos { get; set; }
	}
}