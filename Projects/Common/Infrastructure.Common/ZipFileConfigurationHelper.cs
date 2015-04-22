using System.IO;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using Ionic.Zip;

namespace Infrastructure.Common
{
	public static class ZipFileConfigurationHelper
	{
		public static string Error { get; private set; }

		public static GKDeviceConfiguration UnZipFromStream(MemoryStream memoryStream)
		{
			var zipFile = ZipFile.Read(memoryStream);
			var dataMemory = new MemoryStream();
			var firstOrDefault = zipFile.FirstOrDefault(x => x.FileName == "GKDeviceConfiguration.xml");
			if (firstOrDefault != null)
				firstOrDefault.Extract(dataMemory);
			dataMemory.Position = 0;
			try
			{
				return ZipSerializeHelper.DeSerialize<GKDeviceConfiguration>(dataMemory);
			}
			catch
			{
				Error = "Не удалось десериализовать xml конфигурацию";
				return null;
			}
			finally
			{
				memoryStream.Close();
				zipFile.Dispose();
				dataMemory.Close();
			}
		}
	}
}