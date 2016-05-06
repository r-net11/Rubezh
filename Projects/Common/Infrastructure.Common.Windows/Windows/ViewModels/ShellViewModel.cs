using Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Ribbon;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class ShellViewModel : ApplicationViewModel
	{
		double _splitterDistance;
		GridLength _emptyGridColumn;
		public ShellViewModel(ClientType clientType)
		{
			ClientType = clientType;
			_emptyGridColumn = new GridLength(0, GridUnitType.Pixel);
			_splitterDistance = RegistrySettingsHelper.GetDouble(ClientType + ".Shell.SplitterDistance");
			if (_splitterDistance == 0)
				_splitterDistance = 1;
			Width1 = new GridLength(_splitterDistance, GridUnitType.Star);
			LeftPanelVisible = true;
			AllowHelp = true;
			AllowMaximize = true;
			AllowMinimize = true;
			ContentFotter = null;
			ContentItems = new ObservableCollection<IViewPartViewModel>();
			MinimizeCommand = new RelayCommand<MinimizeTarget>(OnMinimize);
			TextVisibility = !RegistrySettingsHelper.GetBool(ClientType + ".Shell.TextVisibility");
			RibbonVisible = false;
			ToolbarVisible = true;
		}

		string logoSource;
		public string LogoSource
		{
			get { return logoSource; }
			set
			{
				logoSource = value;
				OnPropertyChanged(() => LogoSource);
			}
		}

		public RelayCommand<MinimizeTarget> MinimizeCommand { get; private set; }
		void OnMinimize(MinimizeTarget target)
		{
			switch (target)
			{
				case MinimizeTarget.NavigationText:
					TextVisibility = !TextVisibility;
					break;
				case MinimizeTarget.LeftPanel:
					LeftPanelVisible = !RightPanelVisible;
					RightPanelVisible = true;
					break;
				case MinimizeTarget.RightPanel:
					RightPanelVisible = !LeftPanelVisible;
					LeftPanelVisible = true;
					break;
			}
		}

		bool textVisibility;
		public bool TextVisibility
		{
			get { return textVisibility; }
			set
			{
				if (TextVisibility != value)
					RegistrySettingsHelper.SetBool(ClientType + ".Shell.TextVisibility", !value);
				textVisibility = value;
				OnPropertyChanged(() => TextVisibility);
			}
		}

		public ClientType ClientType { get; private set; }
		public Layout Layout { get; protected set; }

		ReadOnlyCollection<NavigationItem> _navigationItems;
		public ReadOnlyCollection<NavigationItem> NavigationItems
		{
			get { return _navigationItems; }
			set
			{
				_navigationItems = value;
				UpdateNavigationItemContext(value);
				OnPropertyChanged(() => NavigationItems);
			}
		}

		double _minWidth;
		public double MinWidth
		{
			get { return _minWidth; }
			set
			{
				_minWidth = value;
				OnPropertyChanged(() => MinWidth);
			}
		}

		double _minHeight;
		public double MinHeight
		{
			get { return _minHeight; }
			set
			{
				_minHeight = value;
				OnPropertyChanged(() => MinHeight);
			}
		}

		double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		ObservableCollection<IViewPartViewModel> _contentItems;
		public ObservableCollection<IViewPartViewModel> ContentItems
		{
			get { return _contentItems; }
			set
			{
				_contentItems = value;
				OnPropertyChanged(() => ContentItems);
			}
		}

		bool _toolbarVisible;
		public bool ToolbarVisible
		{
			get { return _toolbarVisible; }
			set
			{
				_toolbarVisible = value;
				OnPropertyChanged(() => ToolbarVisible);
			}
		}

		BaseViewModel _toolbar;
		public BaseViewModel Toolbar
		{
			get { return _toolbar; }
			set
			{
				_toolbar = value;
				OnPropertyChanged(() => Toolbar);
			}
		}

		RightContentViewModel _rightContent;
		public RightContentViewModel RightContent
		{
			get { return _rightContent; }
			set
			{
				_rightContent = value;
				OnPropertyChanged(() => RightContent);
				RightPanelVisible = RightContent != null;
				if (RightContent != null)
					RightContent.Shell = this;
			}
		}

		public bool IsRightPanelFocused { get; set; }
		public bool IsLeftPanelFocused { get; set; }

		bool _isRightPanelEnabled;
		public bool IsRightPanelEnabled
		{
			get { return _isRightPanelEnabled; }
			set
			{
				_isRightPanelEnabled = RightContent == null ? false : value;
				OnPropertyChanged(() => IsRightPanelEnabled);
				if (!IsRightPanelEnabled)
					RightPanelVisible = false;
			}
		}

		bool _leftPanelVisible;
		public bool LeftPanelVisible
		{
			get { return _leftPanelVisible; }
			set
			{
				if (LeftPanelVisible == value)
					return;
				_leftPanelVisible = value;
				OnPropertyChanged(() => LeftPanelVisible);
				UpdateWidth();
			}
		}

		bool _rightPanelVisible;
		public bool RightPanelVisible
		{
			get { return _rightPanelVisible; }
			set
			{
				if (RightPanelVisible == value || (value && !IsRightPanelEnabled))
					return;
				_rightPanelVisible = RightContent == null ? false : value;
				OnPropertyChanged(() => RightPanelVisible);
				UpdateWidth();
				if (RightContent != null)
				{
					if (RightPanelVisible)
						RightContent.Content.Show();
					else
						RightContent.Content.Hide();
				}
			}
		}

		bool _ribbonVisible;
		public bool RibbonVisible
		{
			get { return _ribbonVisible; }
			set
			{
				_ribbonVisible = value;
				OnPropertyChanged(() => RibbonVisible);
			}
		}

		RibbonMenuViewModel _ribbonContent;
		public RibbonMenuViewModel RibbonContent
		{
			get { return _ribbonContent; }
			set
			{
				_ribbonContent = value;
				OnPropertyChanged(() => RibbonContent);
			}
		}

		GridLength _width1;
		public GridLength Width1
		{
			get { return _width1; }
			set
			{
				_width1 = value;
				OnPropertyChanged(() => Width1);
			}
		}

		GridLength _width2;
		public GridLength Width2
		{
			get { return _width2; }
			set
			{
				_width2 = value;
				OnPropertyChanged(() => Width2);
			}
		}

		GridLength _width3;
		public GridLength Width3
		{
			get { return _width3; }
			set
			{
				_width3 = value;
				OnPropertyChanged(() => Width3);
			}
		}

		void UpdateWidth()
		{
			if (Width1 != _emptyGridColumn && Width3 != _emptyGridColumn)
				_splitterDistance = Width1.Value / Width3.Value;
			Width1 = LeftPanelVisible ? new GridLength(_splitterDistance, GridUnitType.Star) : _emptyGridColumn;
			Width2 = LeftPanelVisible && RightPanelVisible ? GridLength.Auto : _emptyGridColumn;
			Width3 = RightPanelVisible ? new GridLength(1, GridUnitType.Star) : _emptyGridColumn;
		}

		public override bool OnClosing(bool isCanceled)
		{
			return base.OnClosing(isCanceled);
		}
		public override void OnClosed()
		{
			ApplicationService.Layout.Close();
			UpdateWidth();
			RegistrySettingsHelper.SetDouble(ClientType + ".Shell.SplitterDistance", _splitterDistance);
			base.OnClosed();
		}

		void UpdateNavigationItemContext(IEnumerable<NavigationItem> items)
		{
			if (items != null)
				items.ForEach(item =>
				{
					item.Context = this;
					UpdateNavigationItemContext(item.Childs);
				});
		}
	}
}