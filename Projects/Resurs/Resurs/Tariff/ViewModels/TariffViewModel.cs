using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class TariffViewModel : BaseViewModel
	{
		public TariffViewModel(Tariff tariff)
		{
			Tariff = tariff;
		}

		Tariff _tariff;
		public Tariff Tariff
		{
			get { return _tariff; }
			set
			{
				_tariff = value;
				OnPropertyChanged(() => Tariff);
			}
		}

		public void Update(Tariff tariff)
		{
			Tariff = tariff;
		}
	}
}