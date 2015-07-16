using FiresecAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GKImitator.ViewModels
{
	public class UsersViewModel : DialogViewModel
	{
		public UsersViewModel()
		{
			Title = "Пользователи прибора";
			Users = new ObservableCollection<ImitatorUser>(DBHelper.ImitatorSerializedCollection.ImitatorUsers);
		}

		public ObservableCollection<ImitatorUser> Users { get; private set; }

		ImitatorUser _selectedUser;
		public ImitatorUser SelectedUser
		{
			get { return _selectedUser; }
			set
			{
				_selectedUser = value;
				OnPropertyChanged(() => SelectedUser);
			}
		}
	}
}