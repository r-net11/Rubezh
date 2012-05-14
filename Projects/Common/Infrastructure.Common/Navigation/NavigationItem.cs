using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;

namespace Infrastructure.Common.Navigation
{
	public class NavigationItem : BaseViewModel
	{
		public string Title { get; private set; }
		public string Icon { get; private set; }
		public ReadOnlyCollection<NavigationItem> Childs { get; private set; }

		public NavigationItem(string title, string icon = null, IList<NavigationItem> childs = null, PermissionType? permission = null)
		{
			Title = title;
			Icon = icon;
			Childs = new ReadOnlyCollection<NavigationItem>(childs ?? new List<NavigationItem>());
			PermissionPredicate = null;
			IsVisible = true;
		}
		public virtual void Execute()
		{
		}

		public Predicate<NavigationItem> PermissionPredicate { get; set; }
		public bool CheckPermission()
		{
			return PermissionPredicate == null || PermissionPredicate(this);
		}

		private bool _isVisible;
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				OnPropertyChanged("IsVisible");
			}
		}
		private bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				_isExpanded = value;
				OnPropertyChanged("IsExpanded");
			}
		}
		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				OnPropertyChanged("IsSelected");
			}
		}
		private PermissionType? _permission = null;
		public PermissionType? Permission
		{
			get { return _permission; }
			set
			{
				_permission = value;
				OnPropertyChanged("Permission");
			}
		}
	}

}
