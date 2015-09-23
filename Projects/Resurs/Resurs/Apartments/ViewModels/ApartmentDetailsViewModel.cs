using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ApartmentDetailsViewModel : SaveCancelDialogViewModel
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
			Description = apartment.Description;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		protected override bool Save()
		{
			Apartment.Name = Name;
			Apartment.Description = Description;
			return base.Save();
		}
	}
}