using System;
using StrazhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutUserViewModel : BaseViewModel
	{
		private Action<LayoutUserViewModel> _isActiveChanged;
		public User User { get; private set; }

		public LayoutUserViewModel(User user, Action<LayoutUserViewModel> isActiveChanged)
		{
			_isActiveChanged = isActiveChanged;
			User = user;
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
		}
	}
}