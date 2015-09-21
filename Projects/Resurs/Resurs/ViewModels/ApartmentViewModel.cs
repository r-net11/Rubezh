using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
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

		public void Update(Apartment apartment)
		{
			Apartment = apartment;
		}
	}
}