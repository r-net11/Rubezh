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
			}
		}
		private bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (value && Parent != null && Parent.Container != null)
				{
					DockingManager.IsFocusManagerEnabled = false;
					Parent.Container.IsActive = true;
					DockingManager.IsFocusManagerEnabled = true;
				}
				_isActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}
	}
}