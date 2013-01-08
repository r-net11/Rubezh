namespace Infrastructure.Common.Windows.ViewModels
{
	class DialogHeaderViewModel : BaseViewModel, IHeaderViewModel
	{
		public DialogHeaderViewModel(DialogViewModel content)
		{
			Content = content;
		}

		#region IHeaderViewModel Members

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

		public bool ShowIconAndTitle { get; set; }

		#endregion
	}
}