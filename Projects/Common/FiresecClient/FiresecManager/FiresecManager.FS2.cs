using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Client;
using Common;
using Infrastructure.Common;
using System.IO;

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