using Infrastructure.Common.License;
using RubezhAPI.License;
using RubezhLicense;
using System;
using System.Linq;
using System.Windows.Forms;

namespace RubezhService.LicenseEditor
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

				int remoteClientsCount;
				if (!int.TryParse(args[2], out remoteClientsCount))
					return;

				var licenseInfo = new RubezhLicenseInfo()
				{
					RemoteClientsCount = remoteClientsCount,
					HasFirefighting = args.Any(x => x.Trim().ToLower() == "firefighting"),
					HasGuard = args.Any(x => x.Trim().ToLower() == "guard"),
					HasSKD = args.Any(x => x.Trim().ToLower() == "skd"),
					HasVideo = args.Any(x => x.Trim().ToLower() == "video"),
					HasOpcServer = args.Any(x => x.Trim().ToLower() == "opcserver")
				};

				LicenseManager.TrySave(args[0], licenseInfo, key);

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
