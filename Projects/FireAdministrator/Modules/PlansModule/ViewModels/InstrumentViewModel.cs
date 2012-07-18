using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Windows.Input;
using Infrustructure.Plans.Designer;

namespace PlansModule.ViewModels
{
	public class InstrumentViewModel : BaseViewModel, IInstrument
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

		public ICommand Command { get; set; }
		public InstrumentAdorner Adorner { get; set; }
		public bool Autostart { get; set; }
	}
}
