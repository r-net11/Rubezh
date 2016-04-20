using System.Collections.ObjectModel;
using System.Windows.Input;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Ribbon
{
	public class RibbonMenuItemViewModel : BaseViewModel
	{
		public RibbonMenuItemViewModel(string text = null, string imageSource = null, string toolTip = null)
		{
			Text = text;
			ImageSource = imageSource;
			ToolTip = toolTip;
			IsEnabled = true;
			IsVisible = true;
		}
		public RibbonMenuItemViewModel(string text, ObservableCollection<RibbonMenuItemViewModel> items, string imageSource = null, string toolTip = null)
			: this(text, new RibbonMenuViewModel(items), imageSource, toolTip)
		{
		}
		public RibbonMenuItemViewModel(string text, BaseViewModel content, string imageSource = null, string toolTip = null)
			: this(text, imageSource, toolTip)
		{
			Content = content;
		}
		public RibbonMenuItemViewModel(string text, ICommand command, string imageSource = null, string toolTip = null)
			: this(text, command, null, imageSource, toolTip)
		{
		}
		public RibbonMenuItemViewModel(string text, ICommand command, object commandParameter, string imageSource = null, string toolTip = null)
			: this(text, imageSource, toolTip)
		{
			Command = command ?? DisabledRelayCommand.Instance;
			CommandParameter = commandParameter;
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		private string _imageSource;
		public string ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged(() => ImageSource);
			}
		}

		private string _toolTip;
		public string ToolTip
		{
			get { return _toolTip; }
			set
			{
				_toolTip = value;
				OnPropertyChanged(() => ToolTip);
			}
		}

		private ICommand _command;
		public ICommand Command
		{
			get { return _command; }
			set
			{
				_command = value;
				OnPropertyChanged(() => Command);
			}
		}

		private object _commandParameter;
		public object CommandParameter
		{
			get { return _commandParameter; }
			set
			{
				_commandParameter = value;
				OnPropertyChanged(() => CommandParameter);
			}
		}

		private BaseViewModel _content;
		public BaseViewModel Content
		{
			get { return _content; }
			set
			{
				_content = value;
				OnPropertyChanged(() => Content);
			}
		}

		private bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		private bool _isVisible;
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				OnPropertyChanged(() => IsVisible);
			}
		}

		private int _order;
		public int Order
		{
			get { return _order; }
			set
			{
				_order = value;
				OnPropertyChanged(() => Order);
			}
		}

		private bool _isNewGroup;
		public bool IsNewGroup
		{
			get { return _isNewGroup; }
			set
			{
				_isNewGroup = value;
				OnPropertyChanged(() => IsNewGroup);
			}
		}

		public RibbonMenuItemViewModel this[int index]
		{
			get
			{
				var menuViewModel = Content as RibbonMenuViewModel;
				if (index >= menuViewModel.Items.Count)
					return null;
				return menuViewModel == null ? null : menuViewModel.Items[index];
			}
		}
		public void Add(RibbonMenuItemViewModel ribbonMenuItemViewModel)
		{
			var menuViewModel = Content as RibbonMenuViewModel;
			if (menuViewModel != null)
				menuViewModel.Items.Add(ribbonMenuItemViewModel);
		}
	}
}
