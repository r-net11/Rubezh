using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;

namespace Resurs.ViewModels
{
	public class PermissionViewModel : BaseViewModel
	{
		public PermissionType PermissionType { get; private set; }
		public string Name { get; private set; }

		public PermissionViewModel(PermissionType permissionType, bool flag)
		{
			PermissionType = permissionType;
			Name = permissionType.ToString();
			IsChecked = flag;
		}

		 bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}

	}
}