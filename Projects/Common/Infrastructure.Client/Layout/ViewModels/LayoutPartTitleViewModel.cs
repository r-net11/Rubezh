using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Client.Layout.ViewModels
{
	public class LayoutPartTitleViewModel : BaseViewModel
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
	}
}
