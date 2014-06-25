using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using SKDModule.Events;
using SKDModule.ViewModels;
using JournalItem = FiresecAPI.SKD.JournalItem;

namespace SKDModule
{
	public static class AutoActivationWatcher
	{
		public static void Run()
		{
			ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewSKDJournalEvent>().Subscribe(OnNewJournal);
		}

		public static void OnNewJournal(List<JournalItem> journalItems)
		{
			if (ClientSettings.AutoActivationSettings.IsAutoActivation)
			{
				if ((Application.Current.MainWindow != null) && (!Application.Current.MainWindow.IsActive))
				{
					Application.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
					Application.Current.MainWindow.Activate();
				}
			}
			if (ClientSettings.AutoActivationSettings.IsPlansAutoActivation)
			{
				foreach (var journalItem in journalItems)
				{
					var journalItemViewModel = new JournalItemViewModel(journalItem);

					var globalStateClass = SKDManager.GetMinStateClass();

					if (journalItem.State <= globalStateClass ||
						(globalStateClass != XStateClass.Fire1 && globalStateClass != XStateClass.Fire2 && globalStateClass != XStateClass.Attention))
					{
						switch (journalItem.ObjectType)
						{
							case ObjectType.Контроллер_СКД:
							case ObjectType.Считыватель_СКД:
								var device = SKDManager.Devices.FirstOrDefault(x => x.UID == journalItem.ObjectUID);
								if (device != null)
								{
									var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementSKDDevices.Any(y => y.DeviceUID == device.UID); });
									if (existsOnPlan)
									{
										ServiceFactory.Events.GetEvent<ShowSKDDeviceOnPlanEvent>().Publish(device);
									}
								}
								break;
						}
					}
				}
			}
		}
	}
}