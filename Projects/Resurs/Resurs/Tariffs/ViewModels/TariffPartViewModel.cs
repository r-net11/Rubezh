using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;

namespace Resurs.ViewModels
{
	public class TariffPartViewModel : BaseViewModel
	{
		TariffPart _tariffPart;
		public TariffPart TariffPart { 
			get { return _tariffPart; }
			set { _tariffPart = value; }
		}
		public TariffPartViewModel(TariffPart tariffPart)
		{
			TariffPart = tariffPart;
			Price = 0;
			BeginTime = new DateTime();
			EndTime = new DateTime();
			
			Discount = 0;
			Threshold = 0;
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

		private DateTime _beginTime;

		public DateTime BeginTime
		{
			get { return _beginTime; }
			set { _beginTime = value; }
		}
		private DateTime _endTime;

		public DateTime EndTime
		{
			get { return _endTime; }
			set { _endTime = value; }
		}

		private TimeSpan _partDuration;

		public TimeSpan PartDuration
		{
			get 
			{
				if (BeginTime.CompareTo(EndTime) <= 0)
				{
					return BeginTime.Subtract(EndTime);
				}
				else throw new Exception("Указано начальное время позже конечного.");
			}
		}
	}
}
