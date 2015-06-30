namespace Infrastructure.Common.Windows.ViewModels
{
	public class DialogViewModel : HeaderedWindowViewModel
	{
		public DialogViewModel()
		{
			Header = new DialogHeaderViewModel(this);
			CloseOnEscape = true;
			AllowMaximize = false;
		}

		private BaseViewModel _headerCommandViewModel;
		public BaseViewModel HeaderCommandViewModel
		{
			get { return _headerCommandViewModel; }
			set
			{
				_headerCommandViewModel = value;
				OnPropertyChanged(() => HeaderCommandViewModel);
			}
		}
		
	}
}