using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FS2Api
{
	[DataContract]
	public class HexFileCollectionInfo
	{
		public HexFileCollectionInfo()
		{
			FileInfos = new List<HEXFileInfo>();
			DriverType = DriverType.Rubezh_2AM;
			Name = "Обновление прошивки";
			MinorVersion = 1;
			MajorVersion = 1;
		}

		[DataMember]
		public DriverType DriverType { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int MinorVersion { get; set; }

		[DataMember]
		public int MajorVersion { get; set; }

		[DataMember]
		public string AutorName { get; set; }

		[DataMember]
		public List<HEXFileInfo> FileInfos { get; set; }
	}
}