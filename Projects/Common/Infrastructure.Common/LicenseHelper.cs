using System.Linq;
using Defender;

namespace Infrastructure.Common
{
	public static class LicenseHelper
	{
        public static License License { get; private set; }
        public static int NumberOfUsers { get; private set; }
        public static bool FireAlarm { get; private set; }
        public static bool SecurityAlarm { get; private set; }
        public static bool Skd { get; private set; }
        public static bool ControlScripts { get; private set; }
        public static bool OrsServer { get; private set; }

        static LicenseHelper()
        {
            License = LicenseProcessor.ProcessLoad(AppDataFolderHelper.GetFile("FiresecService.license"), InitialKey.Generate());
            if (License != null)
            {
                var parameter = License.Parameters.FirstOrDefault(x => x.Id == "NumberOfUsers");
                if (parameter != null)
                    NumberOfUsers = (int)parameter.Value;
                parameter = License.Parameters.FirstOrDefault(x => x.Id == "FireAlarm");
                if (parameter != null)
                    FireAlarm = (bool)parameter.Value;
                parameter = License.Parameters.FirstOrDefault(x => x.Id == "SecurityAlarm");
                if (parameter != null)
                    SecurityAlarm = (bool)parameter.Value;
                parameter = License.Parameters.FirstOrDefault(x => x.Id == "Skd");
                if (parameter != null)
                    Skd = (bool)parameter.Value;
                parameter = License.Parameters.FirstOrDefault(x => x.Id == "ControlScripts");
                if (parameter != null)
                    ControlScripts = (bool)parameter.Value;
                parameter = License.Parameters.FirstOrDefault(x => x.Id == "OrsServer");
                if (parameter != null)
                    OrsServer = (bool)parameter.Value;
            }
        }
	}
}