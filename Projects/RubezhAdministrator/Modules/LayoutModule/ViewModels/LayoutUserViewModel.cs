using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;

namespace LayoutModule.ViewModels
{
	public class LayoutUserViewModel : BaseViewModel
	{
		public User User { get; private set; }
		public LayoutUserViewModel(User user)
		{
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

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				_isActive = value;
				OnPropertyChanged(() => IsActive);
			}
		}
		public void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Login);
		}
	}
}