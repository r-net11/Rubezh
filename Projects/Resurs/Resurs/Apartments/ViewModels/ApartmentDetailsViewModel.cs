using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;

namespace Resurs.ViewModels
{
	public partial class ApartmentDetailsViewModel : SaveCancelDialogViewModel
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
					Address = _apartment.Address;
					Description = _apartment.Description;
					Phone = _apartment.Phone;
					Email = _apartment.Email;
					Login = _apartment.Login;
					Password = _apartment.Password;
					IsSendEmail = _apartment.IsSendEmail;
				}
			}
		}

		public ApartmentDetailsViewModel(Apartment apartment, bool isNew = false, bool isReadOnly = false)
		{
			Title = isNew ? "Создание абонента" : "Редактирование абонента";
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

			DBCash.SaveApartment(Apartment);

			return base.Save();
		}
	}
}