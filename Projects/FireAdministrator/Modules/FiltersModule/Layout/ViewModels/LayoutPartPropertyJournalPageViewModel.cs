﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using Infrastructure.Common.Services.Layout;
using RubezhClient;
using FiresecAPI.Journal;

namespace FiltersModule.ViewModels
{
	public class LayoutPartPropertyJournalPageViewModel : LayoutPartPropertyPageViewModel
	{
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
				OnPropertyChanged(() => SelectedFilter);
			}
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
			if ((SelectedFilter == null && properties.ReferenceUID != Guid.Empty) || (SelectedFilter != null && properties.ReferenceUID != SelectedFilter.UID))
			{
				properties.ReferenceUID = SelectedFilter == null ? Guid.Empty : SelectedFilter.UID;
				_layoutPartJournalViewModel.UpdateLayoutPart(SelectedFilter);
				return true;
			}
			return false;
		}
	}
}