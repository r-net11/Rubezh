using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;

namespace Resurs.ViewModels
{
	public class TariffViewModel : BaseViewModel
	{
		public TariffViewModel (Tariff tariff)
		{
			Tariff = tariff;
			TariffType = Tariff.TariffType;
		}

		Tariff _tariff;
		public Tariff Tariff
		{
			get { return _tariff; }
			set
			{
				_tariff = value;
				OnPropertyChanged(() => Tariff);
				OnPropertyChanged(() => Name);
				OnPropertyChanged(() => Description);
				OnPropertyChanged(() => TariffType);
				OnPropertyChanged(() => IsDiscount);
			}
		}
		public string Name
		{
			get { return Tariff.Name; }
			set
			{
				Tariff.Name = value;
			}
		}
		private bool _isDiscount;

		public bool IsDiscount
		{
			get { return Tariff.IsDiscount; }
			set { Tariff.IsDiscount = value; }
		}

		public string Description
		{
			get { return Tariff.Description; }
			set
			{
				Tariff.Description = value;
			}
		}

		public TariffType TariffType
		{
			get { return Tariff.TariffType; }
			set 
			{
				Tariff.TariffType = value;
			}
		}
		public byte TariffParts
		{
			get { return (byte)Tariff.TariffParts.Count; }
		}
	}
}