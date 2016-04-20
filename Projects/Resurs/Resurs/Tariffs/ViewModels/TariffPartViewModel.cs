using Infrastructure.Common.Windows.Windows.ViewModels;
using ResursAPI;
using System;

namespace Resurs.ViewModels
{
	public class TariffPartViewModel : BaseViewModel
	{
		public TariffPart TariffPart { get; set; }
		public TariffPartViewModel(TariffPart tariffPart = null)
		{
			if (tariffPart == null)
			{
				TariffPart = new ResursAPI.TariffPart();
				Price = 0;
				StartTime = new TimeSpan(0, 0, 0);
				Discount = 0;
				Threshold = 0;
			}
			else
			{
				TariffPart = tariffPart;
				Price = tariffPart.Price;
				StartTime = tariffPart.StartTime;
				Discount = tariffPart.Discount;
				Threshold = tariffPart.Threshold;
			}
		}

		double _threshold;
		public double Threshold
		{
			get { return _threshold; }
			set 
			{ 
				_threshold = value;
				OnPropertyChanged(() => Threshold);
			}
		}

		double _price;
		public double Price
		{
			get { return _price; }
			set
			{
				_price = value;
				OnPropertyChanged(() => Price);
			}
		}

		double _discount;
		public double Discount
		{
			get { return _discount; }
			set
			{
				_discount = value;
				OnPropertyChanged(() => Discount);
			}
		}

		TimeSpan _startTime;
		public TimeSpan StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}
	}
}