using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class HeaderedWindowViewModel : WindowBaseViewModel
	{
		public HeaderedWindowViewModel()
		{
			Sizable = true;
		}

		private IHeaderViewModel _header;
		public IHeaderViewModel Header
		{
			get { return _header; }
			set
			{
				_header = value;
				OnPropertyChanged("Header");
			}
		}
	}
}
