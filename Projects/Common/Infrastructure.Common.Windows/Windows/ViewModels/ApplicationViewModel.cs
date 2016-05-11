using Infrastructure.Common.About.ViewModels;
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
			base.SetSurface(surface);
			surface.StateChanged += (s, e) => OnPropertyChanged(() => IsMaximized);
		}

		bool _allowHelp;
		public bool AllowHelp
		{
			get { return _allowHelp; }
			set
			{
				_allowHelp = value;
				OnPropertyChanged(() => AllowHelp);
			}
		}

		bool _allowMinimize;
		public bool AllowMinimize
		{
			get { return _allowMinimize; }
			set
			{
				_allowMinimize = value;
				OnPropertyChanged(() => AllowMinimize);
			}
		}

		bool _allowLogoIcon;
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

		void Minimize()
		{
			Surface.WindowState = WindowState.Minimized;
		}
		public RelayCommand ApplicationMaximizeCommand { get; private set; }

		void Maximize()
		{
			Surface.WindowState = WindowState.Maximized;
			OnPropertyChanged(() => IsMaximized);
		}
		public RelayCommand ApplicationNormalizeCommand { get; private set; }

		void Normalize()
		{
			Surface.WindowState = WindowState.Normal;
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
			var aboutViewModel = new AboutViewModel();
			DialogService.ShowModalWindow(aboutViewModel);
		}

		public bool IsMaximized
		{
			get { return Surface != null && Surface.WindowState == WindowState.Normal; }
		}

		BaseViewModel _headerTop;
		public BaseViewModel HeaderTop
		{
			get { return _headerTop; }
			set
			{
				_headerTop = value;
				OnPropertyChanged(() => HeaderTop);
			}
		}

		BaseViewModel _headerMenu;
		public BaseViewModel HeaderMenu
		{
			get { return _headerMenu; }
			set
			{
				_headerMenu = value;
				OnPropertyChanged(() => HeaderMenu);
			}
		}

		BaseViewModel _contentFotter;
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