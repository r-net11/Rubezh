﻿using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using RubezhAPI.Models.Layouts;
using RubezhAPI.SKD;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using RubezhClient;
using RubezhAPI.Journal;

namespace FiltersModule.ViewModels
{
	public class LayoutPartJournalViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartReferenceProperties _properties;

		public LayoutPartJournalViewModel(LayoutPartReferenceProperties properties)
		{
			Title = "Журнал событий";
			IconSource = LayoutPartDescription.IconPath + "BLevels.png";
			_properties = properties ?? new LayoutPartReferenceProperties();
			var journalFilter = ClientManager.SystemConfiguration.JournalFilters.FirstOrDefault(item => item.UID == _properties.ReferenceUID);
			UpdateLayoutPart(journalFilter);
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyJournalPageViewModel(this);
			}
		}

		string _filterTitle;
		public string FilterTitle
		{
			get { return _filterTitle; }
			set
			{
				_filterTitle = value;
				OnPropertyChanged(() => FilterTitle);
			}
		}

		public void UpdateLayoutPart(JournalFilter journalFilter)
		{
			FilterTitle = journalFilter == null ? "" : journalFilter.Name;
		}
	}
}