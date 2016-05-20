using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LayoutModule.ViewModels
{
	public class LayoutUsersViewModel : BaseViewModel
	{
		Layout _layout;
		public LayoutUsersViewModel()
		{
			Update();
		}

		ObservableCollection<LayoutUserViewModel> _users;
		public ObservableCollection<LayoutUserViewModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged(() => Users);
			}
		}

		LayoutUserViewModel _selectedUser;
		public LayoutUserViewModel SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				OnPropertyChanged(() => SelectedUser);
			}
		}
		public void Update()
		{
			var users = new List<LayoutUserViewModel>();
			ClientManager.SecurityConfiguration.Users.ForEach(item => users.Add(new LayoutUserViewModel(item)));
			users.Sort(Comparison);
			Users = new ObservableCollection<LayoutUserViewModel>(users);
		}
		public void Update(Layout layout)
		{
			_layout = layout;
			if (_layout != null)
				for (int i = _layout.Users.Count - 1; i >= 0; i--)
					if (Users.Select(x => x.User.UID == _layout.Users[i]).Count() == 0)
						_layout.Users.RemoveAt(i);
			foreach (var layoutUserViewModel in Users)
				layoutUserViewModel.IsActive = _layout != null && _layout.Users.Contains(layoutUserViewModel.User.UID);
			SelectedUser = Users.FirstOrDefault();
		}

		int Comparison(LayoutUserViewModel x, LayoutUserViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
		public void Save()
		{
			if (_layout != null)
			{
				_layout.Users.Clear();
				foreach (var layoutUserViewModel in Users)
					if (layoutUserViewModel.IsActive)
						_layout.Users.Add(layoutUserViewModel.User.UID);
			}
		}
	}
}