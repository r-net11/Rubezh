using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;

namespace Resurs.ViewModels
{
	public class PermissionViewModel : BaseViewModel
	{
		public PermissionType PermissionType { get; private set; }
		public bool IsEnabled {get; set;}
		public PermissionViewModel(PermissionType permissionType)
		{
			PermissionType = permissionType;
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