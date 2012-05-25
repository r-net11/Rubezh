using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class WindowBaseViewModel : BaseViewModel
	{
		public WindowBaseViewModel()
		{
			MinHeight = 0;
			MinWidth = 0;
			MaxHeight = double.PositiveInfinity;
			MaxWidth = double.PositiveInfinity;
			CloseOnEscape = false;
		}

		public Window Surface { get; internal set; }

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

		public bool CloseOnEscape { get; set; }

		public virtual bool OnClosing(bool isCanceled)
		{
			return isCanceled;
		}
		public virtual void OnClosed()
		{
		}
		public void Close()
		{
			Close(true);
		}
		public void Close(bool result)
		{
			if (Surface != null)
			{
				if (ComponentDispatcher.IsThreadModal)
					Surface.DialogResult = result;
				Surface.Close();
			}
		}
	}
}
