
namespace Infrastructure.Common.Windows.Windows.ViewModels
{
	class ApplicationHeaderViewModel : BaseViewModel, IHeaderViewModel
	{
		public ApplicationHeaderViewModel(ApplicationViewModel content)
		{
			Content = content;
			ShowIconAndTitle = true;
		}

		bool _showIconAndTitle;
		public bool ShowIconAndTitle
		{
			get { return _showIconAndTitle; }
			set
			{
				_showIconAndTitle = value;
				OnPropertyChanged(() => ShowIconAndTitle);
			}
		}

		#region ICaptionedHeaderViewModel Members

		HeaderedWindowViewModel _content;
		public HeaderedWindowViewModel Content
		{
			get { return _content; }
			set
			{
				_content = value;
				OnPropertyChanged("Content");
			}
		}

		#endregion
	}
}