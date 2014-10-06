using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using JournalModule.ViewModels;

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

					var gkStateClass = GKManager.GetMinStateClass();
					var skdStateClass = SKDManager.GetMinStateClass();
					var globalStateClass = (XStateClass)Math.Min((int)gkStateClass, (int)skdStateClass);

					if (journalItemViewModel.StateClass <= globalStateClass ||
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