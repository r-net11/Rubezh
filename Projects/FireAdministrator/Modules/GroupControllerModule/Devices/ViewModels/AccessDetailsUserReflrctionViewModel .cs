using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.GK;
using Infrastructure.Common;
using System.Collections.ObjectModel;


namespace GKModule.ViewModels
{
	class AccessDetailsUserReflrctionViewModel : SaveCancelDialogViewModel
	{
		public MirrorUser MirrorUser { get; set; }
		public AccessDetailsUserReflrctionViewModel(MirrorUser mirrorUser = null)
		{
			if (mirrorUser == null)
			{
				Title = "Создание доступа для пользователя";
				mirrorUser = new MirrorUser();	
			}
			else	
			{
				Name = mirrorUser.Name;
				Password = mirrorUser.Password;
				DateEndAccess = mirrorUser.DateEndAccess;
				SelectedGKCardType = mirrorUser.Type;	
				Title = string.Format("Редактирование пользователя: {0}", mirrorUser.Name);
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
			MirrorUser = new MirrorUser();
			MirrorUser.Name = Name;
			MirrorUser.Password = Password;
			MirrorUser.DateEndAccess = DateEndAccess;
			MirrorUser.Type = SelectedGKCardType;
			return base.Save();
		}
	}
}
