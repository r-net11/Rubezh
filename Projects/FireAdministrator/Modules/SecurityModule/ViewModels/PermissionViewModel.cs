using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.TreeList;

namespace SecurityModule.ViewModels
{
	public class PermissionViewModel : TreeNodeViewModel<PermissionViewModel>
	{
		public PermissionType PermissionType { get; private set; }

		public PermissionViewModel(PermissionType permissionType, List<PermissionViewModel> children = null)
		{
			PermissionType = permissionType;
			Name = permissionType.ToDescription();

			if (children != null)
			{
				foreach (var child in children)
				{
					AddChild(child);
				}
			}
		}

		public string Name { get; private set; }

		public bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				foreach (var child in Children)
				{
					child.IsChecked = value;
				}
				if (!value)
				{
					if (Parent != null)
					{
						Parent.UnsetParent();
					}
				}
			}
		}

		public void UnsetParent()
		{
			_isChecked = false;
			OnPropertyChanged("IsChecked");
			if (Parent != null)
			{
				Parent.UnsetParent();
			}
		}
	}
}