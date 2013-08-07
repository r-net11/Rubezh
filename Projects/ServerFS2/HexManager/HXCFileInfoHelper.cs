using System.IO;
using System.Runtime.Serialization;
using HexManager.Models;

namespace HexManager
{
	public static class HXCFileInfoHelper
	{
		public static HXCFileInfo Load(string fileName)
		{
			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(HXCFileInfo));
				var hxcFileInfo = (HXCFileInfo)dataContractSerializer.ReadObject(fileStream);
				return hxcFileInfo;
			}
			return null;
		}

		public static void Save(string fileName, HXCFileInfo hxcFileInfo)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(HXCFileInfo));
			using (var fileStream = new FileStream(fileName, FileMode.Create))
			{
				dataContractSerializer.WriteObject(fileStream, hxcFileInfo);
			}
		}
	}
}