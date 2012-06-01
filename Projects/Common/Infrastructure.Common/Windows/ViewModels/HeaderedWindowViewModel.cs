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
			Height = 600;
			Width = 800;
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
