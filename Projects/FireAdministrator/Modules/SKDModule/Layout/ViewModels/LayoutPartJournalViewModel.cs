using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Services.Layout;
using FiresecAPI.Models.Layouts;
using Infrastructure.Client.Layout.ViewModels;
using FiresecClient;
using FiresecAPI.Models;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class LayoutPartJournalViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartSKDJournalProperties _properties;

		public LayoutPartJournalViewModel(LayoutPartSKDJournalProperties properties)
		{
			Title = "Журнал событий";
			IconSource = LayoutPartDescription.IconPath + "BLevels.png";
			_properties = properties ?? new LayoutPartSKDJournalProperties();
			var journalFilter = SKDManager.SKDConfiguration.SKDSystemConfiguration.JournalFilters.FirstOrDefault(item => item.UID == _properties.FilterUID);
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

		public void UpdateLayoutPart(SKDJournalFilter journalFilter)
		{
			FilterTitle = journalFilter == null ? "" : journalFilter.Name;
		}
	}
}