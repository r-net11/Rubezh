using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class CaptionedViewModel : WindowBaseViewModel
	{
		public CaptionedViewModel()
		{
			MinHeight = 600;
			MinWidth = 800;
			Sizable = true;
		}

		private bool _sizable;
		public bool Sizable
		{
			get { return _sizable; }
			set
			{
				_sizable = value;
				OnPropertyChanged("Sizable");
			}
		}

		private ICaptionedHeaderViewModel _header;
		public ICaptionedHeaderViewModel Header
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
