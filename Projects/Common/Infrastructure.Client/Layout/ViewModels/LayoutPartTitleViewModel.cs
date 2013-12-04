using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Services.Layout;
using FiresecAPI.Models.Layouts;

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

		public override ILayoutProperties GetProperties()
		{
			return null;
		}
		public override void SetProperties(ILayoutProperties properties)
		{
		}
	}
}
