namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class HeaderedWindowViewModel : WindowBaseViewModel
	{
		public HeaderedWindowViewModel()
		{
			Sizable = true;
		}

		private IHeaderViewModel _header;
		public IHeaderViewModel Header
		{
			get { return _header; }
			set
			{
				_header = value;
				OnPropertyChanged(() => Header);
			}
		}

		private bool _allowMaximize;
		public bool AllowMaximize
		{
			get { return _allowMaximize; }
			set
			{
				_allowMaximize = value;
				OnPropertyChanged("AllowMaximize");
			}
		}
	}
}