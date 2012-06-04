using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Navigation;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class ShellViewModel : ApplicationViewModel
	{
		public ShellViewModel()
		{
			AllowHelp = true;
			AllowMaximize = true;
			AllowMinimize = true;
			ContentFotter = null;
			ContentHeader = null;
			MainContent = null;
			MinWidth = 800;
			MinHeight = 600;
		}

		private BaseViewModel _toolbar;
		public BaseViewModel Toolbar
		{
			get { return _toolbar; }
			set
			{
				_toolbar = value;
				OnPropertyChanged("Toolbar");
			}
		}
		private BaseViewModel _contentHeader;
		public BaseViewModel ContentHeader
		{
			get { return _contentHeader; }
			set
			{
				_contentHeader = value;
				OnPropertyChanged("ContentHeader");
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
		private BaseViewModel _mainContent;
		public BaseViewModel MainContent
		{
			get { return _mainContent; }
			set
			{
				_mainContent = value;
				OnPropertyChanged("MainContent");
			}
		}
		private List<NavigationItem> _navigationItems;
		public List<NavigationItem> NavigationItems
		{
			get { return _navigationItems; }
			set
			{
				_navigationItems = value;
				OnPropertyChanged("NavigationItems1");
			}
		}

		private double _minWidth;
		public double MinWidth
		{
			get { return _minWidth; }
			set
			{
				_minWidth = value;
				OnPropertyChanged("MinWidth");
			}
		}
		private double _minHeight;
		public double MinHeight
		{
			get { return _minHeight; }
			set
			{
				_minHeight = value;
				OnPropertyChanged("MinHeight");
			}
		}
		private double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged("Height");
			}
		}
		private double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged("Width");
			}
		}
	}
}
