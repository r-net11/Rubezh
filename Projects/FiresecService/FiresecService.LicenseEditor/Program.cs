using FiresecLicense;
using System;
using System.Linq;
using System.Windows.Forms;

namespace FiresecService.LicenseEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 3)
            {
                var key = InitialKey.FromHexString(args[1]);
                if (key.BinaryValue == null)
                    return;

				int remoteWorkplacesCount;
				if (!int.TryParse(args[2], out remoteWorkplacesCount))
                    return;

			    var licenseInfo = new FiresecLicenseInfo()
				{
					RemoteWorkplacesCount = remoteWorkplacesCount,
					Fire = args.Any(x => x.Trim().ToLower() == "fire"),
					Security = args.Any(x => x.Trim().ToLower() == "security"),
					Access = args.Any(x => x.Trim().ToLower() == "access"),
					Video = args.Any(x => x.Trim().ToLower() == "video"),
					OpcServer = args.Any(x => x.Trim().ToLower() == "opcserver")
				};
	                
				FiresecLicenseManager.TrySave(args[0], licenseInfo, key);

                return;
            }

            if (args.Length == 1 && args[0].ToLower().Replace("/", "-").Replace(" ", "") == "-gui")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
