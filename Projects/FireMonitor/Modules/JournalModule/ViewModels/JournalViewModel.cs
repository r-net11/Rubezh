using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;
using JournalModule.Events;
using Infrastructure.Common.Services.Layout;
using System.Windows.Threading;
using System;

namespace JournalModule.ViewModels
{
	public class JournalViewModel : ViewPartViewModel, ILayoutPartContent
	{
		private int _unreadCount;
		public JournalFilter JournalFilter { get; private set; }

		public JournalViewModel(JournalFilter journalFilter = null)
		{
			_unreadCount = 0;
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
				if (JournalFilter.JournalEventDescriptionTypes.Count > 0 && !JournalFilter.JournalEventDescriptionTypes.Contains(journalItem.JournalEventDescriptionType))
					continue;
				if (JournalFilter.JournalObjectTypes.Count > 0 && !JournalFilter.JournalObjectTypes.Contains(journalItem.JournalObjectType))
					continue;
				if (JournalFilter.ObjectUIDs.Count > 0 && !JournalFilter.ObjectUIDs.Contains(journalItem.ObjectUID))
					continue;

				var existingJournalItem = JournalItems.FirstOrDefault(x => x.JournalItem.UID == journalItem.UID);
				if (existingJournalItem != null)
				{
					existingJournalItem.JournalItem.VideoUID = journalItem.VideoUID;
					existingJournalItem.JournalItem.CameraUID = journalItem.CameraUID;
					existingJournalItem.OnPropertyChanged(() => existingJournalItem.ShowVideoCommand);
					continue;
				}

				var journalItemViewModel = new JournalItemViewModel(journalItem);
				if (JournalItems.Count > 0)
					JournalItems.Insert(0, journalItemViewModel);
				else
					JournalItems.Add(journalItemViewModel);

				if (JournalItems.Count > JournalFilter.LastItemsCount)
					JournalItems.RemoveAt(JournalFilter.LastItemsCount);

				if (FiresecManager.CheckPermission(PermissionType.Oper_NoAlarmConfirm) == false)
				{
					if (((journalItemViewModel.StateClass == XStateClass.Fire1 || journalItemViewModel.StateClass == XStateClass.Fire2 || journalItemViewModel.StateClass == XStateClass.Attention)) ||
						(journalItemViewModel.StateClass == XStateClass.Fire1))
					{
						var confirmationViewModel = new ConfirmationViewModel(journalItem);
						DialogService.ShowWindow(confirmationViewModel);
					}
				}
			}

			if (SelectedJournal == null)
				SelectedJournal = JournalItems.FirstOrDefault();

			_unreadCount += journalItems.Count;
			UpdateUnread();
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

		private DispatcherOperation _updateUnreadOperation = null;
		private void UpdateUnread()
		{
			if (_updateUnreadOperation == null && Container != null)
				_updateUnreadOperation = ApplicationService.BeginInvoke(new Action(() =>
				{
					_updateUnreadOperation = null;
					if (Container.IsVisibleLayout)
						_unreadCount = 0;
					var title = GetDefaultTitle();
					Container.Title = _unreadCount == 0 ? title : string.Format("{0} {1}", title, _unreadCount);
				}));
		}

		#region ILayoutPartContent Members

		public ILayoutPartContainer Container { get; private set; }

		public void SetLayoutPartContainer(ILayoutPartContainer container)
		{
			Container = container;
			if (Container != null)
				Container.SelectedChanged += (s, e) => UpdateUnread();
		}

		#endregion

		private string GetDefaultTitle()
		{
			return Container.LayoutPart.Title ?? Container.LayoutPartPresenter.Name;
		}
	}
}