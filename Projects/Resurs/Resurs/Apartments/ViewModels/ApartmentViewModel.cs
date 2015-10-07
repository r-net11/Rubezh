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

		public bool IsContentLoaded { get; set; }

		public void Update(Apartment apartment)
		{
			Apartment = apartment;
		}

		public void Update()
		{
			if (Apartment != null && !IsContentLoaded)
			{
				Apartment = DBCash.GetApartment(Apartment.UID, true);
				IsContentLoaded = true;
			}
		}
	}
}