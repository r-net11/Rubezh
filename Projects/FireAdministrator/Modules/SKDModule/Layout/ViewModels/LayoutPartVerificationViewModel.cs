using System.Collections.Generic;
using FiresecAPI.Models.Layouts;
using Infrastructure.Client.Layout.ViewModels;
using Infrastructure.Common.Services.Layout;

namespace SKDModule.ViewModels
{
	public class LayoutPartVerificationViewModel : LayoutPartTitleViewModel
	{
		private LayoutPartSKDVerificationProperties _properties;

		public LayoutPartVerificationViewModel(LayoutPartSKDVerificationProperties properties)
		{
			Title = "Верификация";
			IconSource = LayoutPartDescription.IconPath + "BLevels.png";
			_properties = properties ?? new LayoutPartSKDVerificationProperties();
			//var journalFilter = SKDManager.SKDConfiguration.SKDSystemConfiguration.JournalFilters.FirstOrDefault(item => item.UID == _properties.FilterUID);
			//UpdateLayoutPart(journalFilter);
		}

		public override ILayoutProperties Properties
		{
			get { return _properties; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get
			{
				yield return new LayoutPartPropertyVerificationPageViewModel(this);
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

		public void UpdateLayoutPart()
		{
			FilterTitle = "";
		}
	}
}