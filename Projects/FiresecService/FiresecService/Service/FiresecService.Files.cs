using System.Collections.Generic;
using System.IO;
using Common;
using FiresecService.Configuration;
using Infrastructure.Common;

namespace FiresecService.Service
{
    public partial class FiresecService
    {
        public List<string> GetFileNamesList(string directory)
        {
            return HashHelper.GetFileNamesList(directory);
        }

        public Dictionary<string, string> GetDirectoryHash(string directory)
        {
            return HashHelper.GetDirectoryHash(directory);
        }

        public Stream GetFile(string directoryNameAndFileName)
        {
            var filePath = ConfigurationFileManager.ConfigurationDirectory(directoryNameAndFileName);
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public Stream GetConfig()
        {
            var filePath = ConfigurationFileManager.ConfigurationDirectory("config.fscp");
            ConfigurationFileManager.ActualizeConfiguration();
            ConfigActualizeHelper.Actualize(filePath);
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public void SetConfig(Stream stream)
        {
            var filePath = ConfigurationFileManager.ConfigurationDirectory("config.fscp");
            using (Stream file = File.OpenWrite(filePath))
            {
                CopyStream(stream, file);
            }
        }

        void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8 * 1024];
            int length;
            while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, length);
            }
        }
    }
}