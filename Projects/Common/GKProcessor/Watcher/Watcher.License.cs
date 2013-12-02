using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Threading;
using Common;
using Infrastructure.Common.Windows;

namespace GKProcessor
{
	public partial class Watcher
	{
		DateTime LastLicenseCheckTime;
		DateTime StartTime = DateTime.Now;
		bool CurrentHasLicense = true;
		bool HasLicense = true;

		bool CheckLicense()
		{
			return true;
			//#if DEBUG
			//            return true;
			//#endif
			try
			{
				if ((DateTime.Now - StartTime).TotalSeconds < 6)
					return true;
				var checkInterval = CurrentHasLicense ? 6 : 1;
				if ((DateTime.Now - LastLicenseCheckTime).TotalSeconds > checkInterval)
				{
					ApplicationService.Invoke(() =>
						{
							CurrentHasLicense = LicenseHelper.CheckLicense(false);
						});
					if (CurrentHasLicense != HasLicense)
					{
						HasLicense = CurrentHasLicense;
						foreach (var descriptor in GkDatabase.Descriptors)
						{
							var baseState = descriptor.XBase.GetXBaseState();
							baseState.IsNoLicense = !CurrentHasLicense;
						}

						if (CurrentHasLicense)
							AddMessage("Отсутствует лицензия", "");
						else
							AddMessage("Лицензия обнаружена", "");

						DiagnosticsManager.Add("hasLicense=" + CurrentHasLicense);
					}

					if (CurrentHasLicense)
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
					return CurrentHasLicense;
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