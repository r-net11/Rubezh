using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.GK;
using FiresecClient;
using JournalModule.Events;
using JournalModule.ViewModels;
using Infrastructure;
using Infrastructure.Events;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using System;

namespace JournalModule
{
	public static class AutoActivationWatcher
	{
		public static void Run()
		{
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournal);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournal);
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

					var gkStateClass = XManager.GetMinStateClass();
					var skdStateClass = SKDManager.GetMinStateClass();
					var globalStateClass = (XStateClass)Math.Min((int)gkStateClass, (int)skdStateClass);

					if (journalItem.StateClass <= globalStateClass ||
						(globalStateClass != XStateClass.Fire1 && globalStateClass != XStateClass.Fire2 && globalStateClass != XStateClass.Attention))
					{
						if (journalItemViewModel.ShowOnPlanCommand.CanExecute(null))
							journalItemViewModel.ShowOnPlanCommand.Execute();
					}
				}
			}
		}
	}
}