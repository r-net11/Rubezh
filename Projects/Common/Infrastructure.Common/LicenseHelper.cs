using System.Linq;
using Defender;
using System;
using Common;

namespace Infrastructure.Common
{
	public static class LicenseHelper
	{
        static License _license;
        public static License License 
        {
            get { return _license; }
            set
            {
                if (TrySetValues(value))
                    _license = value;
            }
        }
        public static int NumberOfUsers { get; set; }
        public static bool FireAlarm { get; set; }
        public static bool SecurityAlarm { get; set; }
        public static bool Skd { get; set; }
        public static bool ControlScripts { get; set; }
        public static bool OrsServer { get; set; }

        public static bool TryLoad()
        {
            License = LicenseProcessor.ProcessLoad(AppDataFolderHelper.GetFile("FiresecService.license"), InitialKey.Generate());
            return License != null;
        }

        static bool TrySetValues(License license)
        {
            try
            {
                int numberOfUsers = 0;
                bool fireAlarm = false,
                    securityAlarm = false,
                    skd = false,
                    controlScripts = false,
                    orsServer = false;

                if (license != null)
                {
                    var parameter = license.Parameters.FirstOrDefault(x => x.Id == "NumberOfUsers");
                    if (parameter != null)
                        numberOfUsers = (int)parameter.Value;
                    parameter = license.Parameters.FirstOrDefault(x => x.Id == "FireAlarm");
                    if (parameter != null)
                        fireAlarm = (bool)parameter.Value;
                    parameter = license.Parameters.FirstOrDefault(x => x.Id == "SecurityAlarm");
                    if (parameter != null)
                        securityAlarm = (bool)parameter.Value;
                    parameter = license.Parameters.FirstOrDefault(x => x.Id == "Skd");
                    if (parameter != null)
                        skd = (bool)parameter.Value;
                    parameter = license.Parameters.FirstOrDefault(x => x.Id == "ControlScripts");
                    if (parameter != null)
                        controlScripts = (bool)parameter.Value;
                    parameter = license.Parameters.FirstOrDefault(x => x.Id == "OrsServer");
                    if (parameter != null)
                        orsServer = (bool)parameter.Value;
                }

                NumberOfUsers = numberOfUsers;
                FireAlarm = fireAlarm;
                SecurityAlarm = securityAlarm;
                Skd = skd;
                ControlScripts = controlScripts;
                OrsServer = orsServer;
            }
            catch (Exception e)
            {
                Logger.Error(e, "LicenseHelper.Ctrs");
                return false;
            }
            return true;
        }        
	}
}