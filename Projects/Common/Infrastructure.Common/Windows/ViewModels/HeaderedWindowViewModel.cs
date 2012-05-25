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
			PropertyChanged += new PropertyChangedEventHandler(CaptionedViewModel_PropertyChanged);
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

		private IHeaderViewModel _header;
		public IHeaderViewModel Header
		{
			get { return _header; }
			set
			{
				_header = value;
				if (_header != null)
					_header.Title = Title;
				OnPropertyChanged("Header");
			}
		}

		private void CaptionedViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Title" && _header != null)
				_header.Title = Title;
		}
	}
}
