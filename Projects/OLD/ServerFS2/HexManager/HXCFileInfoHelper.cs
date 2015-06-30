using System.IO;
using System.Runtime.Serialization;
using FS2Api;

namespace HexManager
{
	public static class HXCFileInfoHelper
	{
		public static HexFileCollectionInfo Load(string fileName)
		{
			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(HexFileCollectionInfo));
				var hexFileCollectionInfo = (HexFileCollectionInfo)dataContractSerializer.ReadObject(fileStream);
				return hexFileCollectionInfo;
			}
			return null;
		}

		public static void Save(string fileName, HexFileCollectionInfo hexFileCollectionInfo)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(HexFileCollectionInfo));
			using (var fileStream = new FileStream(fileName, FileMode.Create))
			{
				dataContractSerializer.WriteObject(fileStream, hexFileCollectionInfo);
			}
		}
	}
}