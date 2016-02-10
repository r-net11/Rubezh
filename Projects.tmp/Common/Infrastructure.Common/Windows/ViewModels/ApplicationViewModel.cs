using System.Windows;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class ApplicationViewModel : HeaderedWindowViewModel
	{
		public ApplicationViewModel()
		{
			Header = new ApplicationHeaderViewModel(this);
			CloseOnEscape = false;
			AllowHelp = false;
			AllowMaximize = false;
			AllowMinimize = false;
			AllowLogoIcon = true;

			ApplicationMinimizeCommand = new RelayCommand(Minimize, () => AllowMinimize);
			ApplicationMaximizeCommand = new RelayCommand(Maximize, () => AllowMaximize);
			ApplicationNormalizeCommand = new RelayCommand(Normalize);
			ApplicationCloseCommand = new RelayCommand(() => Close(false), () => AllowClose);
			ApplicationHelpCommand = new RelayCommand(ShowHelp, () => AllowHelp);
			ApplicationAboutCommand = new RelayCommand(ShowAbout, () => AllowHelp);
		}

		protected internal override void SetSurface(Window surface)
		{
			
		}

		private bool _allowHelp;
		public bool AllowHelp
		{
			get { return _allowHelp; }
			set
			{
				_allowHelp = value;
				OnPropertyChanged(() => AllowHelp);
			}
		}
		private bool _allowMinimize;
		public bool AllowMinimize
		{
			get { return _allowMinimize; }
			set
			{
				_allowMinimize = value;
				OnPropertyChanged(() => AllowMinimize);
			}
		}
		private bool _allowLogoIcon;
		public bool AllowLogoIcon
		{
			get { return _allowLogoIcon; }
			set
			{
				_allowLogoIcon = value;
				OnPropertyChanged(() => AllowLogoIcon);
			}
		}

		public RelayCommand ApplicationCloseCommand { get; private set; }
		public RelayCommand ApplicationMinimizeCommand { get; private set; }
		private void Minimize()
		{
			
		}
		public RelayCommand ApplicationMaximizeCommand { get; private set; }
		private void Maximize()
		{
			
			OnPropertyChanged(() => IsMaximized);
		}
		public RelayCommand ApplicationNormalizeCommand { get; private set; }
		private void Normalize()
		{
			
			OnPropertyChanged(() => IsMaximized);
		}
		public RelayCommand ApplicationHelpCommand { get; private set; }
		protected virtual void ShowHelp()
		{
			ManualPdfHelper.Show();
		}
		public RelayCommand ApplicationAboutCommand { get; private set; }
		protected virtual void ShowAbout()
		{
			
		}

		public bool IsMaximized;

		private BaseViewModel _headerTop;
		public BaseViewModel HeaderTop
		{
			get { return _headerTop; }
			set
			{
				_headerTop = value;
				OnPropertyChanged(() => HeaderTop);
			}
		}

		private BaseViewModel _headerMenu;
		public BaseViewModel HeaderMenu
		{
			get { return _headerMenu; }
			set
			{
				_headerMenu = value;
				OnPropertyChanged(() => HeaderMenu);
			}
		}

		private BaseViewModel _contentFotter;
		public BaseViewModel ContentFotter
		{
			get { return _contentFotter; }
			set
			{
				_contentFotter = value;
				OnPropertyChanged(() => ContentFotter);
			}
		}

		public virtual void Run()
		{
		}
	}
}