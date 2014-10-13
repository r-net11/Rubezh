using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
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

		/// <summary>
		/// Название
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Минарная версия
		/// </summary>
		[DataMember]
		public int MinorVersion { get; set; }

		/// <summary>
		/// Мажорная версия
		/// </summary>
		[DataMember]
		public int MajorVersion { get; set; }

		/// <summary>
		/// Список данных для обновления контроллеров
		/// </summary>
		[DataMember]
		public List<HEXFileInfo> HexFileInfos { get; set; }
	}
}