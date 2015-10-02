using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public partial class ApartmentDetailsViewModel : SaveCancelDialogViewModel
	{
		public Apartment Apartment { get; private set; }

		public ApartmentDetailsViewModel(Apartment apartment = null)
		{
			if(apartment == null)
			{
				apartment = new Apartment();
				Title = "Создание абонента";
			}
			else
			{
				Title = "Редактирование абонента";
			}

			Apartment = apartment;
			Name = apartment.Name;
			Address = apartment.Address;
			Description = apartment.Description;
			Phone = apartment.Phone;
			Email = apartment.Email;
			Login = apartment.Login;
			Password = apartment.Password;
			IsSendEmail = apartment.IsSendEmail;
		}
				
		protected override bool Save()
		{
			Apartment.Name = Name;
			Apartment.Address = Address;
			Apartment.Description = Description;
			Apartment.Phone = Phone;
			Apartment.Email = Email;
			Apartment.Login = Login;
			Apartment.Password = Password;
			Apartment.IsSendEmail = IsSendEmail;
			return base.Save();
		}
	}
}