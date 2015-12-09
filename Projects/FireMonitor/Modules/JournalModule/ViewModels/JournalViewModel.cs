using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;
using JournalModule.Events;
using Infrastructure.Common.Services.Layout;
using System.Windows.Threading;
using System;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
	public class JournalViewModel : ViewPartViewModel, ILayoutPartContent
	{
		int _unreadCount;
		public bool IsShowButtons { get; private set; }
		public JournalFilter Filter { get; private set; }
		public bool IsLoading { get; private set; }

		public JournalViewModel(JournalFilter journalFilter = null)
		{
			_unreadCount = 0;
			Filter = journalFilter;
			if (Filter == null)
			{
				Filter = new JournalFilter();
				IsShowButtons = true;
			}
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			ShowFilterCommand = new RelayCommand(OnShowFilter);
		}

		public void Initialize()
		{
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Unsubscribe(OnNewJournalItems);
			ServiceFactory.Events.GetEvent<NewJournalItemsEvent>().Subscribe(OnNewJournalItems);
			ServiceFactory.Events.GetEvent<UpdateJournalItemsEvent>().Unsubscribe(OnUpdateJournalItems);
			ServiceFactory.Events.GetEvent<UpdateJournalItemsEvent>().Subscribe(OnUpdateJournalItems);
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
		}

		public void SetJournalItems()
		{
			var result = ClientManager.FiresecService.BeginGetJournal(Filter);
			if (result.HasError)
			{
				MessageBoxService.Show(result.Error);
				return;
			}
			IsLoading = true;
			OnPropertyChanged(() => IsLoading);
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

		bool CheckFilter(JournalItem journalItem)
		{
			if (Filter.JournalSubsystemTypes.Count > 0 && !Filter.JournalSubsystemTypes.Contains(journalItem.JournalSubsystemType))
				return false;
			if (Filter.JournalEventNameTypes.Count > 0 && !Filter.JournalEventNameTypes.Contains(journalItem.JournalEventNameType))
				return false;
			if (Filter.JournalEventDescriptionTypes.Count > 0 && !Filter.JournalEventDescriptionTypes.Contains(journalItem.JournalEventDescriptionType))
				return false;
			if (Filter.JournalObjectTypes.Count > 0 && !Filter.JournalObjectTypes.Contains(journalItem.JournalObjectType))
				return false;
			if (Filter.ObjectUIDs.Count > 0 && !Filter.ObjectUIDs.Contains(journalItem.ObjectUID))
				return false;
			return true;
		}

		void OnNewJournalItems(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (!CheckFilter(journalItem))
					continue;

				var journalItemViewModel = new JournalItemViewModel(journalItem);
				if (JournalItems.Count > 0)
					JournalItems.Insert(0, journalItemViewModel);
				else
					JournalItems.Add(journalItemViewModel);

				if (JournalItems.Count > Filter.LastItemsCount)
					JournalItems.RemoveAt(Filter.LastItemsCount);
			}

			if (SelectedJournal == null)
				SelectedJournal = JournalItems.FirstOrDefault();

			_unreadCount += journalItems.Count;
			UpdateUnread();
		}

		void OnUpdateJournalItems(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				if (!CheckFilter(journalItem))
					continue;

				var existingJournalItem = JournalItems.FirstOrDefault(x => x.JournalItem.UID == journalItem.UID);
				if (existingJournalItem != null)
				{
					existingJournalItem.JournalItem.VideoUID = journalItem.VideoUID;
					existingJournalItem.JournalItem.CameraUID = journalItem.CameraUID;
					existingJournalItem.OnPropertyChanged(() => existingJournalItem.ShowVideoCommand);
				}
			}
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

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var archiveFilterViewModel = new ArchiveFilterViewModel(Filter, isShowDateTime: false);
			if (DialogService.ShowModalWindow(archiveFilterViewModel))
			{
				Filter = archiveFilterViewModel.GetModel();
				SetJournalItems();
			}
		}

		public void OnGetJournal(List<JournalItem> list)
		{
			ApplicationService.BeginInvoke(() =>
			{
				JournalItems = new ObservableCollection<JournalItemViewModel>();
				foreach (var journalItem in list)
				{
					var journalItemViewModel = new JournalItemViewModel(journalItem);
					JournalItems.Add(journalItemViewModel);
				}
				SelectedJournal = JournalItems.FirstOrDefault();
				IsLoading = false;
				OnPropertyChanged(() => IsLoading);
			});
		}
	}
}