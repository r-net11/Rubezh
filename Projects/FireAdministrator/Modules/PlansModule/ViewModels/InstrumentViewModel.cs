using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class InstrumentViewModel : BaseViewModel
	{
		private string _imageSource;
		public string ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged("ImageSource");
			}
		}

		private string _toolTip;
		public string ToolTip
		{
			get { return _toolTip; }
			set
			{
				_toolTip = value;
				OnPropertyChanged("ToolTip");
			}
		}
	}
}
