﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace Infrastructure.Common.Windows.ViewModels
{
	public abstract class WindowBaseViewModel : BaseViewModel
	{
		public event EventHandler Closed;
		public event CancelEventHandler Closing;

		public WindowBaseViewModel()
		{
			CloseOnEscape = false;
			Sizable = false;
			TopMost = false;
			HideInTaskbar = false;
			AllowClose = true;
			ResizeMode = System.Windows.ResizeMode.CanResize;
		}

		public Window Surface { get; private set; }
		internal protected virtual void SetSurface(Window surface)
		{
			Surface = surface;
		}

		private string _icon;
		public string Icon
		{
			get { return _icon; }
			set
			{
				_icon = value;
				OnPropertyChanged(() => Icon);
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
		private bool _topMost;
		public bool TopMost
		{
			get { return _topMost; }
			set
			{
				_topMost = value;
				OnPropertyChanged("TopMost");
			}
		}

		private bool _sizable;
		public bool Sizable
		{
			get { return _sizable; }
			set
			{
				_sizable = value;
				OnPropertyChanged("Sizable");
				ResizeMode = Sizable ? ResizeMode.CanResize : ResizeMode.CanMinimize;
				OnPropertyChanged("ResizeMode");
			}
		}

		private bool _allowClose;
		public bool AllowClose
		{
			get { return _allowClose; }
			set
			{
				_allowClose = value;
				OnPropertyChanged("AllowClose");
			}
		}

		public ResizeMode ResizeMode { get; private set; }

		public bool CloseOnEscape { get; set; }
		public bool HideInTaskbar { get; set; }
		public bool CustomPosition { get; set; }
		public double X { get; set; }
		public double Y { get; set; }

		public virtual void OnLoad()
		{
			Surface.Owner = DialogService.GetActiveWindow();
			Surface.ShowInTaskbar = Surface.Owner == null;
			if (CustomPosition)
			{
				Surface.WindowStartupLocation = WindowStartupLocation.Manual;
				Surface.Left = X;
				Surface.Top = Y;
			}
			else
			{
				Surface.WindowStartupLocation = Surface.Owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
			}
			
		}

		public virtual bool OnClosing(bool isCanceled)
		{
			return !AllowClose || isCanceled;
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
				AllowClose = true;
				if (ComponentDispatcher.IsThreadModal && Surface.IsModal())
					Surface.DialogResult = result;
				Surface.Close();
			}
		}

		internal void InternalClosing(CancelEventArgs e)
		{
			if (Closing != null)
				Closing(this, e);
			e.Cancel = OnClosing(e.Cancel);
		}
		internal void InternalClosed()
		{
			if (Closed != null)
				Closed(this, EventArgs.Empty);
			OnClosed();
			if (Surface.Owner != null)
				Surface.Owner.Activate();
		}

		public virtual void Loaded()
		{
		}
		public virtual void Unloaded()
		{
		}
	}
}