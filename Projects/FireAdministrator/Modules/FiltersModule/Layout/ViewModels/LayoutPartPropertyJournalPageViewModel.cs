using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Windows.Services.Layout;
using RubezhClient;
using RubezhAPI.Journal;
using Infrastructure;

namespace FiltersModule.ViewModels
{
	public class LayoutPartPropertyJournalPageViewModel : LayoutPartPropertyPageViewModel
	{
		int oldLastItemsCount;
		private LayoutPartJournalViewModel _layoutPartJournalViewModel;

		public LayoutPartPropertyJournalPageViewModel(LayoutPartJournalViewModel layoutPartFilterViewModel)
		{
			_layoutPartJournalViewModel = layoutPartFilterViewModel;
			Filters = new ObservableCollection<JournalFilter>(ClientManager.SystemConfiguration.JournalFilters);
		}

		ObservableCollection<JournalFilter> _filters;
		public ObservableCollection<JournalFilter> Filters
		{
			get { return _filters; }
			set
			{
				_filters = value;
				OnPropertyChanged(() => Filters);
			}
		}

		JournalFilter _selectedFilter;
		public JournalFilter SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				LastItemsCount = SelectedFilter != null ? SelectedFilter.LastItemsCount : JournalFilter.DefaultLastItemsCount;
				oldLastItemsCount = LastItemsCount;
				OnPropertyChanged(() => SelectedFilter);
				OnPropertyChanged(() => IsLastItemsCountEnabled);
			}
		}
		int _lastItemsCount;
		public int LastItemsCount
		{
			get { return _lastItemsCount; }
			set
			{
				_lastItemsCount = value;
				OnPropertyChanged(() => LastItemsCount);
			}
		}
		public bool IsLastItemsCountEnabled
		{
			get { return SelectedFilter != null; }
		}

		bool _oldIsShowBottomPanel;
		bool _isShowBottomPanel;
		public bool IsShowBottomPanel
		{
			get { return _isShowBottomPanel; }
			set
			{
				_isShowBottomPanel = value;
				OnPropertyChanged(() => IsShowBottomPanel);
			}
		}
		
		public override string Header
		{
			get { return "Фильтр журнала"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartJournalProperties)_layoutPartJournalViewModel.Properties;
			SelectedFilter = ClientManager.SystemConfiguration.JournalFilters.FirstOrDefault(item => item.UID == properties.FilterUID);
			IsShowBottomPanel = properties.IsVisibleBottomPanel;
			_oldIsShowBottomPanel = properties.IsVisibleBottomPanel;
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartJournalProperties)_layoutPartJournalViewModel.Properties;
			var hasChanges = SelectedFilter != null && properties.FilterUID != SelectedFilter.UID || oldLastItemsCount != LastItemsCount || _oldIsShowBottomPanel != IsShowBottomPanel;
			if ((SelectedFilter == null && properties.FilterUID != Guid.Empty) || hasChanges)
			{
				properties.FilterUID = SelectedFilter == null ? Guid.Empty : SelectedFilter.UID;
				properties.IsVisibleBottomPanel = IsShowBottomPanel;
				if (SelectedFilter != null)
					SelectedFilter.LastItemsCount = LastItemsCount;
				_layoutPartJournalViewModel.UpdateLayoutPart(SelectedFilter);
				ServiceFactory.SaveService.FilterChanged = true;
				return true;
			}
			return false;
		}
	}
}