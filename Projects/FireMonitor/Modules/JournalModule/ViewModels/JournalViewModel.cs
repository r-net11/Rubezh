using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;
using JournalModule.Events;

namespace JournalModule.ViewModels
{
	public class JournalViewModel : ViewPartViewModel
	{
		public JournalFilter JournalFilter { get; private set; }

		public JournalViewModel(JournalFilter journalFilter = null)
		{
			JournalFilter = journalFilter;
			if (JournalFilter == null)
				JournalFilter = new JournalFilter();
			JournalItems = new ObservableCollection<JournalItemViewModel>();
		}

		public void Initialize()
		{
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournalItems);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournalItems);
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
		}

		public void SetJournalItems(List<JournalItem> journalItems)
		{
			journalItems.Reverse();
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			foreach (var journalItem in journalItems)
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				JournalItems.Add(journalItemViewModel);
			}
			SelectedJournal = JournalItems.FirstOrDefault();
		}

		ObservableCollection<JournalItemViewModel> _journalItems;
		public ObservableCollection<JournalItemViewModel> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged(() => JournalItems);
			}
		}

		JournalItemViewModel _selectedJournal;
		public JournalItemViewModel SelectedJournal
		{
			get { return _selectedJournal; }
			set
			{
				_selectedJournal = value;
				OnPropertyChanged(() => SelectedJournal);
			}
		}

		void OnNewJournalItems(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (JournalFilter.JournalSubsystemTypes.Count > 0 && !JournalFilter.JournalSubsystemTypes.Contains(journalItem.JournalSubsystemType))
					continue;
				if (JournalFilter.JournalEventNameTypes.Count > 0 && !JournalFilter.JournalEventNameTypes.Contains(journalItem.JournalEventNameType))
					continue;
				if (JournalFilter.JournalObjectTypes.Count > 0 && !JournalFilter.JournalObjectTypes.Contains(journalItem.JournalObjectType))
					continue;
				if (JournalFilter.ObjectUIDs.Count > 0 && !JournalFilter.ObjectUIDs.Contains(journalItem.ObjectUID))
					continue;

				var journalItemViewModel = new JournalItemViewModel(journalItem);
				if (JournalItems.Count > 0)
					JournalItems.Insert(0, journalItemViewModel);
				else
					JournalItems.Add(journalItemViewModel);

				if (JournalItems.Count > JournalFilter.LastItemsCount)
					JournalItems.RemoveAt(JournalFilter.LastItemsCount);

				if ((journalItem.JournalObjectType == JournalObjectType.GKZone || journalItem.JournalObjectType == JournalObjectType.GKDirection) &&
					(journalItem.StateClass == XStateClass.Fire1 || journalItem.StateClass == XStateClass.Fire2 || journalItem.StateClass == XStateClass.Attention))
				{
					if (FiresecManager.CheckPermission(PermissionType.Oper_NoAlarmConfirm) == false)
					{
						var confirmationViewModel = new ConfirmationViewModel(journalItem);
						DialogService.ShowWindow(confirmationViewModel);
					}
				}
			}

			if (SelectedJournal == null)
				SelectedJournal = JournalItems.FirstOrDefault();
		}

		public List<JournalColumnType> AdditionalColumns
		{
			get { return ClientSettings.ArchiveDefaultState.AdditionalJournalColumnTypes; }
		}

		bool additionalColumnsChanged;
		public bool AdditionalColumnsChanged
		{
			get { return additionalColumnsChanged; }
			set
			{
				additionalColumnsChanged = value;
				OnPropertyChanged(() => AdditionalColumnsChanged);
			}
		}

		void OnSettingsChanged(object o)
		{
			AdditionalColumnsChanged = !AdditionalColumnsChanged;
		}
	}
}