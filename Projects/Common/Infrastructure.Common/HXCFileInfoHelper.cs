using System.IO;
using System.Xml.Serialization;
using FiresecAPI.GK;

namespace Infrastructure.Common
{
	public static class HXCFileInfoHelper
	{
		public static HexFileCollectionInfo Load(string fileName)
		{
			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var xmlSerializer = new XmlSerializer(typeof(HexFileCollectionInfo));
				var hexFileCollectionInfo = (HexFileCollectionInfo)xmlSerializer.Deserialize(fileStream);
				return hexFileCollectionInfo;
			}
		}

		public static void Save(string fileName, HexFileCollectionInfo hexFileCollectionInfo)
		{
			var xmlSerializer = new XmlSerializer(typeof(HexFileCollectionInfo));
			using (var fileStream = new FileStream(fileName, FileMode.Create))
			{
				xmlSerializer.Serialize(fileStream, hexFileCollectionInfo);
			}
		}
	}
}