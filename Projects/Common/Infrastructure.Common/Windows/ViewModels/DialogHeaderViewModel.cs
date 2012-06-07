
namespace Infrastructure.Common.Windows.ViewModels
{
	class DialogHeaderViewModel : BaseViewModel, IHeaderViewModel
	{
		public DialogHeaderViewModel(DialogViewModel content)
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