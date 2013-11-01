using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Threading;
using Common.GK;
using Common;

namespace GKProcessor
{
	public partial class Watcher
	{
		DateTime LastLicenseCheckTime;
		DateTime StartTime = DateTime.Now;
		bool HasLicense = true;

		bool CheckLicense()
		{
			try
			{
				if ((DateTime.Now - StartTime).TotalSeconds > 6 && (DateTime.Now - LastLicenseCheckTime).TotalSeconds > 6)
				{
					var hasLicense = LicenseHelper.CheckLicense(false);
					if (hasLicense != HasLicense)
					{
						HasLicense = hasLicense;
						foreach (var descriptor in GkDatabase.Descriptors)
						{
							var baseState = descriptor.XBase.GetXBaseState();
							baseState.IsNoLicense = !hasLicense;
						}

						if (hasLicense)
							GKDBHelper.AddMessage("Отсутствует лицензия", "");
						else
							GKDBHelper.AddMessage("Лицензия обнаружена", "");

						DiagnosticsManager.Add("hasLicense=" + hasLicense);
					}

					if (hasLicense)
					{
						LastLicenseCheckTime = DateTime.Now;
						return true;
					}
					else
					{
						Thread.Sleep(TimeSpan.FromSeconds(10));
						return false;
					}
				}
				else
				{
					return true;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalWatcher.OnRunThread CheckLicense");
				return false;
			}
		}
	}
}