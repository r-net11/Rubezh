using System.Windows.Input;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace Infrastructure.Common.Ribbon
{
	public class RibbonMenuItemViewModel : BaseViewModel
	{
		public RibbonMenuItemViewModel()
		{
		}
		public RibbonMenuItemViewModel(string text, BaseViewModel content, string imageSource = null, string toolTip = null)
		{
			Text = text;
			ImageSource = imageSource;
			Content = content;
			ToolTip = toolTip;
		}
		public RibbonMenuItemViewModel(string text, ObservableCollection<RibbonMenuItemViewModel> items, string imageSource = null, string toolTip = null)
		{
			Text = text;
			ImageSource = imageSource;
			Content = new RibbonMenuViewModel(items);
			ToolTip = toolTip;
		}
		public RibbonMenuItemViewModel(string text, ICommand command, string imageSource = null, string toolTip = null)
			: this(text, command, null, imageSource, toolTip)
		{
		}
		public RibbonMenuItemViewModel(string text, ICommand command, object commandParameter, string imageSource = null, string toolTip = null)
		{
			Text = text;
			Command = command;
			CommandParameter = commandParameter;
			ImageSource = imageSource;
			ToolTip = toolTip;
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
	}

}
