namespace Infrastructure.Common.Windows.ViewModels
{
	internal class ApplicationHeaderViewModel : BaseViewModel, IHeaderViewModel
	{
		public ApplicationHeaderViewModel(ApplicationViewModel content)
		{
			Content = content;
			ShowIconAndTitle = true;
		}

		private bool _showIconAndTitle;

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

		private HeaderedWindowViewModel _content;

		public HeaderedWindowViewModel Content
		{
			get { return _content; }
			set
			{
				_content = value;
				OnPropertyChanged("Content");
			}
		}

		#endregion ICaptionedHeaderViewModel Members
	}
}