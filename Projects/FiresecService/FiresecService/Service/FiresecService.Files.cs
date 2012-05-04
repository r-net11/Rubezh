using System.Collections.Generic;
using System.IO;
using Common;
using FiresecService.Configuration;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public List<string> GetFileNamesList(string directory)
		{
			//lock (this)
			{
				return HashHelper.GetFileNamesList(directory);
			}
		}

		public Dictionary<string, string> GetDirectoryHash(string directory)
		{
			//lock (this)
			{
				return HashHelper.GetDirectoryHash(directory);
			}
		}

		public Stream GetFile(string directoryNameAndFileName)
		{
			//lock (this)
			{
				var filePath = ConfigurationFileManager.ConfigurationDirectory(directoryNameAndFileName);
				return new FileStream(filePath, FileMode.Open, FileAccess.Read);
			}
		}
	}
}