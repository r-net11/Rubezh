using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class WindowBaseViewModel : BaseViewModel
	{
		public WindowBaseViewModel()
		{
		}

		private string _icon;
		public string Icon
		{
			get { return _icon; }
			set
			{
				_icon = value;
				OnPropertyChanged("Icon");
			}
		}
		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}
		private WindowStartupLocation _startupLocation;
		public WindowStartupLocation StartupLocation
		{
			get { return _startupLocation; }
			set
			{
				_startupLocation = value;
				OnPropertyChanged("StartupLocation");
			}
		}
		private double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged("Height");
			}
		}
		private double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged("Width");
			}
		}
		private double _minWidth;
		public double MinWidth
		{
			get { return _minWidth; }
			set
			{
				_minWidth = value;
				OnPropertyChanged("MinWidth");
			}
		}
		private double _minHeight;
		public double MinHeight
		{
			get { return _minHeight; }
			set
			{
				_minHeight = value;
				OnPropertyChanged("MinHeight");
			}
		}
		private double _maxHeight;
		public double MaxHeight
		{
			get { return _maxHeight; }
			set
			{
				_maxHeight = value;
				OnPropertyChanged("MaxHeight");
			}
		}
		private double _maxWidth;
		public double MaxWidth
		{
			get { return _maxWidth; }
			set
			{
				_maxWidth = value;
				OnPropertyChanged("MaxWidth");
			}
		}
	}
}
