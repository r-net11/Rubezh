using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutPartViewModel : BaseViewModel
	{
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