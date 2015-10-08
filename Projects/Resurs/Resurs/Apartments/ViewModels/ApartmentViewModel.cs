using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ApartmentViewModel : TreeNodeViewModel<ApartmentViewModel>
	{
		public ApartmentViewModel(Apartment apartment)
		{
			Apartment = apartment;
		}

		Apartment _apartment;
		public Apartment Apartment
		{
			get { return _apartment; }
			set
			{
				_apartment = value;
				OnPropertyChanged(() => Apartment);
			}
		}

		public string ImageSource 
		{ 
			get
			{
				return _apartment != null && _apartment.IsFolder ? "/Controls;component/Images/CFolder.png" : "/Controls;component/Images/AccessTemplate.png";
			}
		}

		ApartmentDetailsViewModel _apartmentDetails;
		public ApartmentDetailsViewModel ApartmentDetails
		{
			get
			{
				if (_apartmentDetails == null && Apartment != null && !Apartment.IsFolder)
				{
					var apartment = DBCash.GetApartment(Apartment.UID, true);
					if (apartment == null)
						return null;
					_apartmentDetails = new ApartmentDetailsViewModel(apartment, false, true);
				}
				return _apartmentDetails;
			}
		}

		ApartmentsFolderDetailsViewModel _apartmentsFolderDetails;
		public ApartmentsFolderDetailsViewModel ApartmentsFolderDetails
		{
			get
			{
				if (_apartmentsFolderDetails == null && Apartment != null && Apartment.IsFolder)
				{
					var apartment = DBCash.GetApartment(Apartment.UID, true);
					if (apartment == null)
						return null;
					_apartmentsFolderDetails = new ApartmentsFolderDetailsViewModel(apartment, false, true);
				}
				return _apartmentsFolderDetails;
			}
		}

		public void Update(Apartment apartment)
		{
			Apartment = apartment;
			if (_apartmentDetails != null)
				_apartmentDetails.Apartment = apartment;
			if (_apartmentsFolderDetails != null)
				_apartmentsFolderDetails.Apartment = apartment;
		}
	}
}