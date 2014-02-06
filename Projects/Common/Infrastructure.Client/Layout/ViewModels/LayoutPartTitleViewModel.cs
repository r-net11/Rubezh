using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;

namespace Infrastructure.Client.Layout.ViewModels
{
	public class LayoutPartTitleViewModel : BaseLayoutPartViewModel
	{
		private string _iconSource;
		public string IconSource
		{
			get { return _iconSource; }
			set
			{
				_iconSource = value;
				OnPropertyChanged(() => IconSource);
			}
		}
		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged(() => Title);
			}
		}

		public override ILayoutProperties Properties
		{
			get { return null; }
		}
		public override IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages
		{
			get { return Enumerable.Empty<LayoutPartPropertyPageViewModel>(); }
		}
	}
}
