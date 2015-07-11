using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;
using System;
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

		public static void AddUser(List<byte> bytes)
		{
			var gkNo = BytesHelper.SubstructShort(bytes, 0);
			var cardType = bytes[2];
			var name = BytesHelper.BytesToStringDescription(bytes, 4);
			var cardNo = BytesHelper.SubstructInt(bytes, 36);
			var level = bytes[40];
			var schedule = bytes[41];
			var seconds = BytesHelper.SubstructInt(bytes, 44);

			var descriptorNos = new List<int>();
			for (int i = 0; i <= 68; i++)
			{
				var descriptorNo = BytesHelper.SubstructInt(bytes, 48 + i * 2);
				descriptorNos.Add(descriptorNo);
			}

			var sheduleNos = new List<int>();
			for (int i = 0; i <= 68; i++)
			{
				var sheduleNo = BytesHelper.SubstructInt(bytes, 184 + i * 2);
				sheduleNos.Add(sheduleNo);
			}

			var packNo = BytesHelper.SubstructShort(bytes, 255);
		}

		public static void EditUser(List<byte> bytes)
		{

		}
	}
}