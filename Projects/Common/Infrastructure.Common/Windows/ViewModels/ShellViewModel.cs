using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Infrastructure.Common.Navigation;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class ShellViewModel : ApplicationViewModel
	{
		private double _splitterDistance;
		private GridLength _emptyGridColumn;
		public ShellViewModel(string name)
		{
			Name = name;
			_emptyGridColumn = new GridLength(0, GridUnitType.Pixel);
			_splitterDistance = RegistrySettingsHelper.GetDouble(Name + ".Shell.SplitterDistance");
			if (_splitterDistance == 0)
				_splitterDistance = 1;
			Width1 = new GridLength(_splitterDistance, GridUnitType.Star);
			LeftPanelVisible = true;
			AllowHelp = true;
			AllowMaximize = true;
			AllowMinimize = true;
			ContentFotter = null;
			MinWidth = 800;
			MinHeight = 600;
			ContentItems = new ObservableCollection<IViewPartViewModel>();
			MinimizeCommand = new RelayCommand<MinimizeTarget>(OnMinimize);
			TextVisibility = !RegistrySettingsHelper.GetBool(Name + ".Shell.TextVisibility");
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

		private bool textVisibility;
		public bool TextVisibility
		{
			get { return textVisibility; }
			set
			{
				if (TextVisibility != value)
					RegistrySettingsHelper.SetBool(Name + ".Shell.TextVisibility", !value);
				textVisibility = value;
				OnPropertyChanged("TextVisibility");
			}
		}

		public string Name { get; private set; }

		private List<NavigationItem> _navigationItems;
		public List<NavigationItem> NavigationItems
		{
			get { return _navigationItems; }
			set
			{
				_navigationItems = value;
				foreach (var navigationItem in _navigationItems)
				{
					navigationItem.Context = this;
					if (navigationItem.Childs != null)
						foreach (var navigationItemChild in navigationItem.Childs)
						{
							navigationItemChild.Context = this;
						}
				}
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

		private ObservableCollection<IViewPartViewModel> _contentItems;
		public ObservableCollection<IViewPartViewModel> ContentItems
		{
			get { return _contentItems; }
			set
			{
				_contentItems = value;
				OnPropertyChanged("ContentItems");
			}
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

		private RightContentViewModel _rightContent;
		public RightContentViewModel RightContent
		{
			get { return _rightContent; }
			set
			{
				_rightContent = value;
				OnPropertyChanged("RightContent");
				RightPanelVisible = RightContent != null;
				if (RightContent != null)
					RightContent.Shell = this;
			}
		}

		public bool IsRightPanelFocused { get; set; }
		public bool IsLeftPanelFocused { get; set; }

		private bool _isRightPanelEnabled;
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

		private bool _leftPanelVisible;
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
		private bool _rightPanelVisible;
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

		private GridLength _width1;
		public GridLength Width1
		{
			get { return _width1; }
			set
			{
				_width1 = value;
				OnPropertyChanged(() => Width1);
			}
		}
		private GridLength _width2;
		public GridLength Width2
		{
			get { return _width2; }
			set
			{
				_width2 = value;
				OnPropertyChanged(() => Width2);
			}
		}
		private GridLength _width3;
		public GridLength Width3
		{
			get { return _width3; }
			set
			{
				_width3 = value;
				OnPropertyChanged(() => Width3);
			}
		}

		private void UpdateWidth()
		{
			if (Width1 != _emptyGridColumn && Width3 != _emptyGridColumn)
				_splitterDistance = Width1.Value / Width3.Value;
			Width1 = LeftPanelVisible ? new GridLength(_splitterDistance, GridUnitType.Star) : _emptyGridColumn;
			Width2 = LeftPanelVisible && RightPanelVisible ? GridLength.Auto : _emptyGridColumn;
			Width3 = RightPanelVisible ? new GridLength(1, GridUnitType.Star) : _emptyGridColumn;
		}

		public override void OnClosed()
		{
			UpdateWidth();
			RegistrySettingsHelper.SetDouble(Name + ".Shell.SplitterDistance", _splitterDistance);
			base.OnClosed();
		}
	}
}