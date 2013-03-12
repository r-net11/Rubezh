using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		private double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}
	}
}
