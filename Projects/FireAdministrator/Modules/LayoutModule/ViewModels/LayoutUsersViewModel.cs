using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using Infrastructure.Events;

namespace LayoutModule.ViewModels
{
	public class LayoutUsersViewModel : BaseViewModel
	{
		private Dictionary<Guid, LayoutUserViewModel> _map;
		private Layout _layout;
		private bool _locked;
		public LayoutUsersViewModel()
		{
			ServiceFactory.Events.GetEvent<AddUserEvent>().Unsubscribe(OnAddUser);
			ServiceFactory.Events.GetEvent<AddUserEvent>().Subscribe(OnAddUser);
			ServiceFactory.Events.GetEvent<DeleteUserEvent>().Unsubscribe(OnDeleteUser);
			ServiceFactory.Events.GetEvent<DeleteUserEvent>().Subscribe(OnDeleteUser);
			Update();
			_locked = false;
		}

		private ObservableCollection<LayoutUserViewModel> _users;
		public ObservableCollection<LayoutUserViewModel> Users
		{
			get { return _users; }
			set
			{
				_users = value;
				OnPropertyChanged(() => Users);
			}
		}

		private LayoutUserViewModel _selectedUser;
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
			_locked = true;
			var roles = new Dictionary<Guid, UserRole>();
			_map = new Dictionary<Guid, LayoutUserViewModel>();
			ClientManager.SecurityConfiguration.UserRoles.ForEach(item => roles.Add(item.UID, item));
			ClientManager.SecurityConfiguration.Users.ForEach(item => _map.Add(item.UID, new LayoutUserViewModel(item, IsActiveChanged)));
			RewriteUsers(_map);
			_locked = false;
		}
		public void Update(Layout layout)
		{
			_locked = true;
			_layout = layout;
			if (_layout != null)
				for (int i = _layout.Users.Count - 1; i >= 0; i--)
					if (!_map.ContainsKey(_layout.Users[i]))
						_layout.Users.RemoveAt(i);
			foreach (var layoutUserViewModel in Users)
				layoutUserViewModel.IsActive = _layout != null && _layout.Users.Contains(layoutUserViewModel.User.UID);
			SelectedUser = Users.FirstOrDefault();
			_locked = false;
		}
		private void IsActiveChanged(LayoutUserViewModel layoutUserViewModel)
		{
			if (_layout != null && !_locked)
			{
				//if (layoutUserViewModel.IsActive && !_layout.Users.Contains(layoutUserViewModel.User.UID))
				//	_layout.Users.Add(layoutUserViewModel.User.UID);
				//else if (!layoutUserViewModel.IsActive && _layout.Users.Contains(layoutUserViewModel.User.UID))
				//	_layout.Users.Remove(layoutUserViewModel.User.UID);
				//ServiceFactory.SaveService.LayoutsChanged = true;
			}
		}
		private int Comparison(LayoutUserViewModel x, LayoutUserViewModel y)
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
		void OnAddUser(User user)
		{
			_map.Add(user.UID, new LayoutUserViewModel(user, IsActiveChanged));
			RewriteUsers(_map);
		}
		void OnDeleteUser(User user)
		{
			_map.Remove(user.UID);
			RewriteUsers(_map);
		}
		void RewriteUsers(Dictionary<Guid, LayoutUserViewModel> map)
		{
			var list = map.Values.ToList();
			list.Sort(Comparison);
			Users = new ObservableCollection<LayoutUserViewModel>(list);
		}
	}
}