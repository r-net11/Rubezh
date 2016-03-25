using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
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

		public override string Header
		{
			get { return "Фильтр журнала"; }
		}
		public override void CopyProperties()
		{
			var properties = (LayoutPartReferenceProperties)_layoutPartJournalViewModel.Properties;
			SelectedFilter = ClientManager.SystemConfiguration.JournalFilters.FirstOrDefault(item => item.UID == properties.ReferenceUID);
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartReferenceProperties)_layoutPartJournalViewModel.Properties;
			var hasChanges = SelectedFilter != null && properties.ReferenceUID != SelectedFilter.UID || oldLastItemsCount != LastItemsCount;
			if ((SelectedFilter == null && properties.ReferenceUID != Guid.Empty) || hasChanges)
			{
				properties.ReferenceUID = SelectedFilter == null ? Guid.Empty : SelectedFilter.UID;
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