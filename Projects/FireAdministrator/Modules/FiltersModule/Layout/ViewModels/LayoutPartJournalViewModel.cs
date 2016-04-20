using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using RubezhAPI.Models.Layouts;
using RubezhAPI.SKD;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Windows.Services.Layout;
using RubezhClient;
using RubezhAPI.Journal;

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
			var journalFilter = ClientManager.SystemConfiguration.JournalFilters.FirstOrDefault(item => item.UID == _properties.FilterUID);
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