using RubezhAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public class UsersViewModel : DialogViewModel
	{
		public UsersViewModel()
		{
			Title = "Пользователи прибора";

			using(var dbService = new DbService())
			{
				var users = dbService.ImitatorUserTraslator.Get();
				Users = new ObservableCollection<ImitatorUser>(users);
			}
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