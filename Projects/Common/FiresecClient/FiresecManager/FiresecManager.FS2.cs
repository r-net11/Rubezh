using System;
using System.IO;
using Common;
using FS2Client;
using Infrastructure.Common;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static bool IsFS2Enabled
		{
			get
			{
#if DEBUG
				return File.Exists("C:/FS2.txt");
#endif
				return false;
			}
		}

		public static FS2ClientContract FS2ClientContract { get; private set; }

		public static void InitializeFS2()
		{
            try
            {
				FS2ClientContract = new FS2ClientContract(AppSettingsManager.FS2ServerAddress);
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.InitializeFiresecDriver");
            }
		}
	}
}