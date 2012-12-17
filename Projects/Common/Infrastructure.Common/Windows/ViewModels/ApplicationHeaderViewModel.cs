namespace Infrastructure.Common.Windows.ViewModels
{
	class ApplicationHeaderViewModel : BaseViewModel, IHeaderViewModel
	{
		public ApplicationHeaderViewModel(ApplicationViewModel content)
		{
			Content = content;
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

		#endregion
	}
}