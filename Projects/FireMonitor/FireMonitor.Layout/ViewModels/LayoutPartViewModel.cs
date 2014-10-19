using System;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows.ViewModels;
using Xceed.Wpf.AvalonDock;
using System.Collections.Generic;

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
				if (SelectedChanged != null)
					SelectedChanged(this, EventArgs.Empty);
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
				if (ActiveChanged != null)
					ActiveChanged(this, EventArgs.Empty);
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
	}
}