using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiagnosticsModule.ViewModels
{
	public class RibbonMenuItem : BaseViewModel
	{
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
				
	}
}
