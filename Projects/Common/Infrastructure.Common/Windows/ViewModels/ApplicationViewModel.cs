using System.Diagnostics;
using System.IO;
using System.Windows;
using Common;
using Infrastructure.Common.About.ViewModels;

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
		}

		private bool _allowHelp;
		public bool AllowHelp
		{
			get { return _allowHelp; }
			set
			{
				_allowHelp = value;
				OnPropertyChanged("AllowHelp");
			}
		}
		private bool _allowMinimize;
		public bool AllowMinimize
		{
			get { return _allowMinimize; }
			set
			{
				_allowMinimize = value;
				OnPropertyChanged("AllowMinimize");
			}
		}

		public void Minimize()
		{
			Surface.WindowState = WindowState.Minimized;
		}
		public void Maximize()
		{
			Surface.WindowState = Surface.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
		}
		public void Normalize()
		{
			Surface.WindowState = WindowState.Normal;
		}
		public virtual void ShowHelp()
		{
			var fileName = Infrastructure.Common.AppDataFolderHelper.GetFile("Manual.pdf");
			if (File.Exists(fileName))
				Process.Start(fileName);
		}
		public virtual void ShowAbout()
		{
			var aboutViewModel = new AboutViewModel();
			DialogService.ShowModalWindow(aboutViewModel);
		}

		private BaseViewModel _headerTop;
		public BaseViewModel HeaderTop
		{
			get { return _headerTop; }
			set
			{
				_headerTop = value;
				OnPropertyChanged("HeaderTop");
			}
		}

		private BaseViewModel _contentFotter;
		public BaseViewModel ContentFotter
		{
			get { return _contentFotter; }
			set
			{
				_contentFotter = value;
				OnPropertyChanged("ContentFotter");
			}
		}
	}
}