using System.Collections.Generic;
using System.Linq;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Windows.Services.Layout;

namespace Infrastructure.Client.Layout.ViewModels
{
	public class LayoutPartTitleViewModel : BaseLayoutPartViewModel
	{
		string _iconSource;
		public string IconSource
		{
			get { return _iconSource; }
			set
			{
				_iconSource = value;
				OnPropertyChanged(() => IconSource);
			}
		}
		string _title;
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