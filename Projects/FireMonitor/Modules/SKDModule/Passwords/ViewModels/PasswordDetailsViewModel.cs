using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class PasswordDetailsViewModel : SaveCancelDialogViewModel
	{
		public Password Password { get; private set; }
		public ShortPassword ShortPassword
		{
			get
			{
				return new ShortPassword
				{
					UID = Password.UID,
					Name = Password.Name,
					Description = Password.Description,
					OrganisationUID = Password.OrganisationUID
				};
			}
		}

		Guid OrganisationUID { get; set; }

		public PasswordDetailsViewModel(Guid orgnaisationUID, Guid? passwordUID = null)
		{
			OrganisationUID = orgnaisationUID;
			if (passwordUID == null)
			{
				Title = "Создание пароля";
				Password = new Password()
				{
					Name = "Новый пароль",
					OrganisationUID = OrganisationUID
				};
			}
			else
			{
				Password = PasswordHelper.GetDetails(passwordUID);
				Title = string.Format("Свойства пароля: {0}", Password.Name);
			}
			CopyProperties();
		}

		public void CopyProperties()
		{
			Name = Password.Name;
			Description = Password.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					OnPropertyChanged("Description");
				}
			}
		}


		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name);
		}

		protected override bool Save()
		{
			Password.Name = Name;
			Password.Description = Description;
			Password.OrganisationUID = OrganisationUID;
			return PasswordHelper.Save(Password);
		}
	}
}