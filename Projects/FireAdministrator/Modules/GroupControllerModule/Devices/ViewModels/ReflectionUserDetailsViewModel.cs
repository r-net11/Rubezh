using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RubezhClient;
using FiresecAPI.GK;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using System.Text.RegularExpressions;


namespace GKModule.ViewModels
{
	class ReflectionUserDetailsViewModel : SaveCancelDialogViewModel
	{
		public MirrorUser MirrorUser { get; set; }
		private ObservableCollection<MirrorUserNewModel> _mirrorUsers;
		public ReflectionUserDetailsViewModel(ObservableCollection<MirrorUserNewModel> mirrorUsers, MirrorUser mirrorUser = null)
		{
			_mirrorUsers = new ObservableCollection<MirrorUserNewModel>(mirrorUsers);
			 GKCardTypes = new ObservableCollection<GKCardType>(Enum.GetValues(typeof(GKCardType)).OfType<GKCardType>());
			if (mirrorUser == null)
			{
				Title = "Создание доступа для пользователя";
				mirrorUser = new MirrorUser();
				DateEndAccess = mirrorUser.DateEndAccess;
			}
			else	
			{	
				Title = string.Format("Редактирование пользователя: {0}", mirrorUser.Name);
			}
			Name = mirrorUser.Name;
			Password = mirrorUser.Password;
			DateEndAccess = mirrorUser.DateEndAccess;
			SelectedGKCardType = mirrorUser.Type;
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
