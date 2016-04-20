using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using RubezhAPI.Models;
using Infrastructure.Common.TreeList;

namespace SecurityModule.ViewModels
{
	public class PermissionViewModel : TreeNodeViewModel<PermissionViewModel>
	{
		public PermissionType PermissionType { get; private set; }
		public string Name { get; private set; }
		public bool IsPermission { get; private set; }

		public PermissionViewModel(string name, List<PermissionViewModel> children = null)
		{
			Name = name;
			IsPermission = false;

			if (children != null)
			{
				foreach (var child in children)
				{
					AddChild(child);
				}
			}
		}

		public PermissionViewModel(PermissionType permissionType)
		{
			PermissionType = permissionType;
			Name = permissionType.ToDescription();
			IsPermission = true;
		}

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
				//if (!value)
				{
					if (Parent != null)
					{
						Parent.UpdateParent();
					}
				}
			}
		}

		public string Image { get { return IsPermission ? "/Controls;component/Images/AccessTemplate.png":"/Controls;component/Images/CFolder.png" ; } }

		public void UpdateParent()
		{
			_isChecked = Children.All(x => x.IsChecked);
			OnPropertyChanged(() => IsChecked);
			if (Parent != null)
			{
				Parent.UpdateParent();
			}
		}
	}
}