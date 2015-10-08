using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class FilterApartmentViewModel
	{
		public Apartment Apartment { get; set; }
		public FilterApartmentViewModel(Apartment apartment )
		{ 
			
		}

		bool _isCheked;
		public  bool IsCheked
		{
			get { return _isCheked;}
			set 
			{
				_isCheked = value;
				
			}

		}
	}
}
