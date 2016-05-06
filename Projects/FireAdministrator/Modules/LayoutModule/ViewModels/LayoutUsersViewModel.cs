using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Models;
using StrazhAPI.Models.Layouts;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutUsersViewModel : BaseViewModel
	{
		private Dictionary<Guid, LayoutUserViewModel> _map;
		private Layout _layout;
		private bool _locked;
		public LayoutUsersViewModel()
		{
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
			FiresecManager.SecurityConfiguration.UserRoles.ForEach(item => roles.Add(item.UID, item));
			FiresecManager.SecurityConfiguration.Users.ForEach(item => _map.Add(item.UID, new LayoutUserViewModel(item, IsActiveChanged)));
			var list = _map.Values.ToList();
			list.Sort(Comparison);
			Users = new ObservableCollection<LayoutUserViewModel>(list);
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
	}
}