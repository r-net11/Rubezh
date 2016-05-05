using System;
using StrazhAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;
using Xceed.Wpf.AvalonDock;
using System.Collections.Generic;
using StrazhAPI.Automation;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Common;

namespace FireMonitor.Layout.ViewModels
{
	public class LayoutPartViewModel : BaseViewModel, ILayoutPartContainer
	{
		public ILayoutPartPresenter LayoutPartPresenter { get; private set; }
		public LayoutPart LayoutPart { get; private set; }

		public LayoutPartViewModel(LayoutPart layoutPart, ILayoutPartPresenter layoutPartPresenter, ILayoutPartContent parent = null)
		{
			LayoutPart = layoutPart;
			LayoutPartPresenter = layoutPartPresenter;
			Content = LayoutPartPresenter.CreateContent(LayoutPart.Properties);
			IconSource = LayoutPartPresenter.IconSource;
			Title = LayoutPart.Title ?? LayoutPartPresenter.Name;
			if (Content is ILayoutPartContent)
				((ILayoutPartContent)Content).SetLayoutPartContainer(this);
			Parent = parent;
		}

		public Guid UID
		{
			get { return LayoutPart.UID; }
		}
		public BaseViewModel Content { get; private set; }
		public ILayoutPartContent Parent { get; private set; }

		public event EventHandler SelectedChanged;
		public event EventHandler ActiveChanged;

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged(() => Title);
			}
		}
		private string _iconSource;
		public string IconSource
		{
			get { return _iconSource; }
			set
			{
				_iconSource = value;
				OnPropertyChanged(() => IconSource);
			}
		}
		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged(() => IsSelected);
				FireSelectedChanged();
			}
		}
		private bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
				FireActiveChanged();
			}
		}
		public bool IsVisibleLayout
		{
			get
			{
				if (!IsSelected)
					return false;
				if (Parent != null && Parent.Container != null)
					return Parent.Container.IsVisibleLayout;
				return true;
			}
		}

		private int _margin;
		public int Margin
		{
			get { return _margin; }
			set
			{
				_margin = value;
				OnPropertyChanged(() => Margin);
			}
		}
		private int _borderThickness;
		public int BorderThickness
		{
			get { return _borderThickness; }
			set
			{
				_borderThickness = value;
				OnPropertyChanged(() => BorderThickness);
			}
		}
		private Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
			}
		}
		private Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
		}

		public void Activate()
		{
			if (Parent != null && Parent.Container != null)
			{
				Parent.Container.IsSelected = true;
				Parent.Container.IsActive = true;
			}
			IsSelected = true;
			IsActive = true;
		}

		public void FireSelectedChanged()
		{
			if (SelectedChanged != null)
				SelectedChanged(this, EventArgs.Empty);
		}
		public void FireActiveChanged()
		{
			if (ActiveChanged != null)
				ActiveChanged(this, EventArgs.Empty);
		}

		public object GetProperty(LayoutPartPropertyName property)
		{
			switch (property)
			{
				case LayoutPartPropertyName.Title:
					return Title;
				case LayoutPartPropertyName.BackgroundColor:
					return BackgroundColor;
				case LayoutPartPropertyName.BorderColor:
					return BorderColor;
				case LayoutPartPropertyName.BorderThickness:
					return BorderThickness;
				case LayoutPartPropertyName.Margin:
					return Margin;
				default:
					return Content is ILayoutPartControl ? ((ILayoutPartControl)Content).GetProperty(property) : null;
			}
		}
		public void SetProperty(LayoutPartPropertyName property, object value)
		{
			switch (property)
			{
				case LayoutPartPropertyName.Title:
					Title = value.ToString();
					break;
				case LayoutPartPropertyName.BackgroundColor:
					BackgroundColor = Utils.Cast<Color>(value);
					break;
				case LayoutPartPropertyName.BorderColor:
					BorderColor = Utils.Cast<Color>(value);
					break;
				case LayoutPartPropertyName.BorderThickness:
					BorderThickness = Utils.Cast<int>(value);
					break;
				case LayoutPartPropertyName.Margin:
					Margin = Utils.Cast<int>(value);
					break;
				default:
					if (Content is ILayoutPartControl)
						((ILayoutPartControl)Content).SetProperty(property, value);
					break;
			}
		}
	}
}