/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;
using System.Windows.Media;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[Serializable]
	public abstract class LayoutElement : DependencyObject, ILayoutElement, IComparable<LayoutElement>, IComparable
	{
		internal LayoutElement()
		{
		}

		#region Parent

		[NonSerialized]
		private ILayoutContainer _parent = null;
		[NonSerialized]
		private ILayoutRoot _root = null;
		[XmlIgnore]
		public ILayoutContainer Parent
		{
			get { return _parent; }
			set
			{
				if (_parent != value)
				{
					ILayoutContainer oldValue = _parent;
					ILayoutRoot oldRoot = _root;
					RaisePropertyChanging("Parent");
					OnParentChanging(oldValue, value);
					_parent = value;
					OnParentChanged(oldValue, value);

					_root = Root;
					if (oldRoot != _root)
						OnRootChanged(oldRoot, _root);

					RaisePropertyChanged("Parent");

					var root = Root as LayoutRoot;
					if (root != null)
						root.FireLayoutUpdated();
				}
			}
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle execute code before to the Parent property changes.
		/// </summary>
		protected virtual void OnParentChanging(ILayoutContainer oldValue, ILayoutContainer newValue)
		{
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the Parent property.
		/// </summary>
		protected virtual void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
		{

		}


		protected virtual void OnRootChanged(ILayoutRoot oldRoot, ILayoutRoot newRoot)
		{
			if (oldRoot != null)
				((LayoutRoot)oldRoot).OnLayoutElementRemoved(this);
			if (newRoot != null)
				((LayoutRoot)newRoot).OnLayoutElementAdded(this);
		}


		#endregion

		[field: NonSerialized]
		[field: XmlIgnore]
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
		}

		[field: NonSerialized]
		[field: XmlIgnore]
		public event PropertyChangingEventHandler PropertyChanging;

		protected virtual void RaisePropertyChanging(string propertyName)
		{
			if (PropertyChanging != null)
				PropertyChanging(this, new System.ComponentModel.PropertyChangingEventArgs(propertyName));
		}

		public ILayoutRoot Root
		{
			get
			{
				var parent = Parent;

				while (parent != null && (!(parent is ILayoutRoot)))
				{
					parent = parent.Parent;
				}

				return parent as ILayoutRoot;
			}
		}

		#region Title

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(LayoutContent), new UIPropertyMetadata(null, OnTitlePropertyChanged, CoerceTitleValue));

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		private static object CoerceTitleValue(DependencyObject obj, object value)
		{
			var lc = (LayoutElement)obj;
			if (((string)value) != lc.Title)
			{
				lc.RaisePropertyChanging(LayoutElement.TitleProperty.Name);
			}
			return value;
		}

		private static void OnTitlePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((LayoutElement)obj).RaisePropertyChanged(LayoutElement.TitleProperty.Name);
		}

		#endregion //Title

		#region IsSelected

		private bool _isSelected = false;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					bool oldValue = _isSelected;
					RaisePropertyChanging("IsSelected");
					_isSelected = value;
					var parentSelector = (Parent as ILayoutContentSelector);
					if (parentSelector != null)
						parentSelector.SelectedContentIndex = _isSelected ? parentSelector.IndexOf(this) : -1;
					OnIsSelectedChanged(oldValue, value);
					RaisePropertyChanged("IsSelected");
				}
			}
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the IsSelected property.
		/// </summary>
		protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
		{
			if (IsSelectedChanged != null)
				IsSelectedChanged(this, EventArgs.Empty);
		}

		public event EventHandler IsSelectedChanged;

		#endregion

		#region IsActive

		[field: NonSerialized]
		private bool _isActive = false;
		[XmlIgnore]
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					RaisePropertyChanging("IsActive");
					bool oldValue = _isActive;

					_isActive = value;

					var root = Root;
					if (root != null && _isActive && this is LayoutContent)
						root.ActiveContent = (LayoutContent)this;

					if (_isActive)
						IsSelected = true;

					OnIsActiveChanged(oldValue, value);
					RaisePropertyChanged("IsActive");
				}
			}
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the IsActive property.
		/// </summary>
		protected virtual void OnIsActiveChanged(bool oldValue, bool newValue)
		{
			if (newValue)
				LastActivationTimeStamp = DateTime.Now;

			if (IsActiveChanged != null)
				IsActiveChanged(this, EventArgs.Empty);
		}

		public event EventHandler IsActiveChanged;

		#endregion

		#region ToolTip

		private object _toolTip = null;
		public object ToolTip
		{
			get { return _toolTip; }
			set
			{
				if (_toolTip != value)
				{
					_toolTip = value;
					RaisePropertyChanged("ToolTip");
				}
			}
		}

		#endregion

		#region IconSource

		private ImageSource _iconSource = null;
		public ImageSource IconSource
		{
			get { return _iconSource; }
			set
			{
				if (_iconSource != value)
				{
					_iconSource = value;
					RaisePropertyChanged("IconSource");
				}
			}
		}

		#endregion

		#region LastActivationTimeStamp

		private DateTime? _lastActivationTimeStamp = null;
		public DateTime? LastActivationTimeStamp
		{
			get { return _lastActivationTimeStamp; }
			set
			{
				if (_lastActivationTimeStamp != value)
				{
					_lastActivationTimeStamp = value;
					RaisePropertyChanged("LastActivationTimeStamp");
				}
			}
		}

		#endregion

		#region Margin

		private int _margin = 0;
		public int Margin
		{
			get { return _margin; }
			set
			{
				if (_margin != value)
				{
					_margin = value;
					RaisePropertyChanged("Margin");
				}
			}
		}

		#endregion

#if DEBUG
		public virtual void ConsoleDump(int tab)
		{
			System.Diagnostics.Debug.Write(new String(' ', tab * 4));
			System.Diagnostics.Debug.WriteLine(this.ToString());
		}
#endif

		#region IComparable<LayoutElement> Members

		public int CompareTo(LayoutElement other)
		{
			return string.Compare(Title, other.Title);
		}

		#endregion

		#region IComparable Members

		int IComparable.CompareTo(object obj)
		{
			return CompareTo((LayoutElement)obj);
		}

		#endregion
	}
}
