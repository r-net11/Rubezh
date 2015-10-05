using Infrastructure.Common.Windows.ViewModels;
using Resurs.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	class TariffPartViewModel : BaseViewModel
	{
		public TariffPartViewModel(TariffPart tariffPart)
		{

		}

		private double _price;

		public double Price
		{
			get { return Price; }
			set 
			{ 
				Price = value;
				OnPropertyChanged(() => Price);
			}
		}
		private double _discount;

		public double Discount
		{
			get { return _discount; }
			set 
			{ 
				_discount = value;
				OnPropertyChanged(() => Discount);
			}
		}
		private TimeSpan _partDuration;

		public TimeSpan PartDuration
		{
			get { return _partDuration; }
			set 
			{ 
				_partDuration = value;
				OnPropertyChanged(() => PartDuration);
			}
		}



	}
}
