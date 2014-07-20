using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models.Layouts;
using FiresecAPI.SKD;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;
using FiresecClient;
using FiresecAPI.Journal;

namespace FiltersModule.ViewModels
{
	public class LayoutPartJournalViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartJournalProperties _properties;

		public LayoutPartJournalViewModel(LayoutPartJournalProperties properties)
		{
			Title = "Журнал событий";
			IconSource = LayoutPartDescription.IconPath + "BLevels.png";
			_properties = properties ?? new LayoutPartJournalProperties();
			var journalFilter = FiresecManager.SystemConfiguration.JournalFilters.FirstOrDefault(item => item.UID == _properties.FilterUID);
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