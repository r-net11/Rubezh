using System;
using System.Threading;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public static class GKLicenseProcessor
	{
		static Thread Thread;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
		static bool CurrentHasLicense = true;
		public static bool HasLicense { get; private set; }

		static GKLicenseProcessor()
		{
			HasLicense = true;
		}

		public static void Start()
		{
			if (Thread == null)
			{
				Thread = new Thread(OnRun);
				Thread.Name = "GK LicenseProcessor";
				Thread.IsBackground = true;
				Thread.Start();
			}
		}

		public static void Stop()
		{
			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(1));
				}
			}
		}

		static void OnRun()
		{
			if (!GlobalSettingsHelper.GlobalSettings.UseHasp)
				return;

			AutoResetEvent = new AutoResetEvent(false);
			if (AutoResetEvent.WaitOne(TimeSpan.FromMinutes(10)))
			{
				return;
			}

			while (true)
			{
				try
				{
					if (AutoResetEvent.WaitOne(TimeSpan.FromMinutes(10)))
					{
						break;
					}

					ApplicationService.Invoke(() =>
					{
						CurrentHasLicense = LicenseHelper.CheckLicense(false);
					});
					if (CurrentHasLicense != HasLicense)
					{
						HasLicense = CurrentHasLicense;

						if (CurrentHasLicense)

							GKProcessorManager.AddGKMessage(JournalEventNameType.Лицензия_обнаружена, "", null, null);
						else
							GKProcessorManager.AddGKMessage(JournalEventNameType.Отсутствует_лицензия, "", null, null);

						DiagnosticsManager.Add("hasLicense=" + CurrentHasLicense);
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "GKLicenseProcessor.OnRun");
				}
			}
		}
	}
}