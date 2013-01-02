using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace SecurityModule.ViewModels
{
	public class PermissionViewModel : BaseViewModel
	{
		public PermissionViewModel(PermissionType permissionType)
		{
			Name = permissionType.ToString();
			Desciption = permissionType.ToDescription();
		}

		public PermissionViewModel(string name)
		{
			Name = name;
			PermissionType permissionType;
			var result = Enum.TryParse<PermissionType>(name, out permissionType);
			if (result)
			{
				Desciption = permissionType.ToDescription();
			}
		}

		public string Name { get; private set; }
		public string Desciption { get; private set; }

		bool _isEnable;
		public bool IsEnable
		{
			get { return _isEnable; }
			set
			{
				_isEnable = value;
				OnPropertyChanged("IsEnable");
			}
		}
	}
}