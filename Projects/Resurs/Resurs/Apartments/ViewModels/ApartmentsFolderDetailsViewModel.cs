using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ApartmentsFolderDetailsViewModel : SaveCancelDialogViewModel
	{
		Apartment _apartment;
		public Apartment Apartment
		{
			get { return _apartment; }
			set
			{
				_apartment = value;
				if (_apartment != null)
				{
					Name = _apartment.Name;
					Description = _apartment.Description;
				}
			}
		}

		public ApartmentsFolderDetailsViewModel(Apartment apartment, bool isNew = false, bool isReadOnly = false)
		{
			Title = isNew ? "Создание группы абонентов" : "Редактирование группы абонентов";
			Apartment = apartment;
			IsReadOnly = isReadOnly;
		}

		bool _isReadOnly;
		public bool IsReadOnly
		{
			get { return _isReadOnly; }
			set
			{
				_isReadOnly = value;
				OnPropertyChanged(() => IsReadOnly);
			}
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

			DBCash.SaveApartment(Apartment);

			return base.Save();
		}
	}
}