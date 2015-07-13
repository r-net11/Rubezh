using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.GK;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using System.Text.RegularExpressions;


namespace GKModule.ViewModels
{
	class AccessDetailsUserReflrctionViewModel : SaveCancelDialogViewModel
	{
		public MirrorUser MirrorUser { get; set; }
		private List <MirrorUser> _mirrorUsers;
		public AccessDetailsUserReflrctionViewModel(List <MirrorUser> mirrorUsers ,MirrorUser mirrorUser = null)
		{
			_mirrorUsers = new List<MirrorUser>(mirrorUsers);
			if (mirrorUser == null)
			{
				Title = "Создание доступа для пользователя";
				mirrorUser = new MirrorUser() { DateEndAccess = DateTime.Now.AddYears(1)};
				DateEndAccess = mirrorUser.DateEndAccess;
			}
			else	
			{
				Name = mirrorUser.Name;
				Password = mirrorUser.Password;
				DateEndAccess = mirrorUser.DateEndAccess;
				SelectedGKCardType = mirrorUser.Type;	
				Title = string.Format("Редактирование пользователя: {0}", mirrorUser.Name);
				_mirrorUsers.RemoveAll(x => x.Name == mirrorUser.Name && x.Password == x.Password);
			}

			GKCardTypes = new ObservableCollection<GKCardType>(Enum.GetValues(typeof(GKCardType)).OfType<GKCardType>());
			
		}
		public ObservableCollection<GKCardType> GKCardTypes { get; private set; }

		private string name_;
		public string Name
		{
			 get{return name_;}
			 set
			{
				name_ = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string password_;
		public string Password
		{
 			get	{return password_;}

			set
			{
				password_ = value;
				OnPropertyChanged(() => Password);
			}
		}
		private DateTime dateEndAccess_;
		public DateTime DateEndAccess
		{
 			get { return dateEndAccess_;}
			
			set
			{
				dateEndAccess_ = value;
				OnPropertyChanged(() => DateEndAccess);
			}
		
		}

		GKCardType _selectedGKCardType;
		public GKCardType SelectedGKCardType
		{
			get { return _selectedGKCardType; }

			set
			{
				_selectedGKCardType = value;
				OnPropertyChanged(() => SelectedGKCardType);
			}
		}

		protected override bool Save()
		{
			
			if (_mirrorUsers.Any(x => x.Name == Name) )
			{
				MessageBoxService.Show("Данный Логин уже существует");
				return false;
			}

			if (_mirrorUsers.Any(x => x.Password == Password) )
			{
				MessageBoxService.Show("Данный Пароль уже существует");
				return false;
			}

			if (string.IsNullOrWhiteSpace(Name))
			{
				MessageBoxService.Show("Поле Имя должно быть заполнено");
				return false;
			}

			if (string.IsNullOrWhiteSpace(Password))
			{
				MessageBoxService.Show("Поле Пароль должно быть заполнено");
				return false;
			}

			MirrorUser = new MirrorUser();
			MirrorUser.Name = Name;
			MirrorUser.Password = Password;
			MirrorUser.DateEndAccess = DateEndAccess;
			MirrorUser.Type = SelectedGKCardType;
			return base.Save();
		}
	}
}
