using System.Collections.Generic;
using System.Linq;
using Common;
using StrazhAPI;
using StrazhAPI.Models;
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

		private bool _isReadOnly;
		public bool IsReadOnly {
			get { return _isReadOnly; }
			set
			{
				if (_isReadOnly == value)
					return;
				_isReadOnly = value;
				OnPropertyChanged(() => IsReadOnly);
				SetReadOnlyForChildren(_isReadOnly);
			}
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

		public void UpdateParent()
		{
			_isChecked = Children.All(x => x.IsChecked);
			OnPropertyChanged(() => IsChecked);
			if (Parent != null)
			{
				Parent.UpdateParent();
			}
		}

		private void SetReadOnlyForChildren(bool isReadOnly)
		{
			Nodes.ForEach(child => ((PermissionViewModel)child).IsReadOnly = isReadOnly);
		}
	}
}