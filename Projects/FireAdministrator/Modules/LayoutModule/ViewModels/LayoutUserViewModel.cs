using System;
using FiresecAPI.Models;
using Infrastructure.Common.TreeList;

namespace LayoutModule.ViewModels
{
	public class LayoutUserViewModel : TreeNodeViewModel<LayoutUserViewModel>
	{
		private Action<LayoutUserViewModel> _isActiveChanged;
		public User User { get; private set; }

		public LayoutUserViewModel(User user, string roleName, Action<LayoutUserViewModel> isActiveChanged)
		{
			_isActiveChanged = isActiveChanged;
			User = user;
			RoleName = roleName;
			Update();
		}

		public string Name
		{
			get { return User.Name; }
		}
		public string Login
		{
			get { return User.Login; }
		}
		public string RoleName { get; private set; }

		private bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
				if (_isActiveChanged != null)
					_isActiveChanged(this);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Login);
			OnPropertyChanged(() => RoleName);
		}
	}
}