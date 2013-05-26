
namespace Infrastructure.Common.Windows.ViewModels
{
	public class RightContentViewModel : BaseViewModel
	{
		public RightContentViewModel()
		{
		}

		private BaseViewModel _menu;
		public BaseViewModel Menu
		{
			get { return _menu; }
			set
			{
				_menu = value;
				OnPropertyChanged(() => Menu);
			}
		}
		private ViewPartViewModel _content;
		public ViewPartViewModel Content
		{
			get { return _content; }
			set
			{
				_content = value;
				OnPropertyChanged(() => Content);
			}
		}
			
		private ShellViewModel _shell;
		public ShellViewModel Shell
		{
			get { return _shell; }
			set
			{
				_shell = value;
				OnPropertyChanged(() => Shell);
			}
		}
		
	}
}
