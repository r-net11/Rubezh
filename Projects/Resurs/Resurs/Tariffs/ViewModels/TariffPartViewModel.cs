using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;

namespace Resurs.ViewModels
{
	public class TariffPartViewModel : BaseViewModel
	{
		public TariffPart TariffPart { get; set; }
		public TariffPartViewModel()
		{
			TariffPart = new ResursAPI.TariffPart();
			Price = 0;
			StartTime = new TimeSpan(0,0,0);
			EndTime = new TimeSpan(0,0,0);
			
			Discount = 0;
			Threshold = 0;
		}

		public TariffPartViewModel(TariffPart tariffPart)
		{
			TariffPart = tariffPart;
			Price = tariffPart.Price;
			StartTime = tariffPart.StartTime;
			EndTime = tariffPart.EndTime;
			Discount = tariffPart.Discount;
			Threshold = tariffPart.Threshold;
		}

		private double _threshold;

		public double Threshold
		{
			get { return _threshold; }
			set { _threshold = value; }
		}

		private double _price;

		public double Price
		{
			get { return _price; }
			set 
			{ 
				_price = value;
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

		private TimeSpan _startTime;

		public TimeSpan StartTime
		{
			get { return _startTime; }
			set 
			{ 
				_startTime = value;
				OnPropertyChanged(() => StartTime);
			}
		}
		private TimeSpan _endTime;

		public TimeSpan EndTime
		{
			get { return _endTime; }
			set { _endTime = value;
			OnPropertyChanged(() => EndTime);
			}
		}
	}
}
