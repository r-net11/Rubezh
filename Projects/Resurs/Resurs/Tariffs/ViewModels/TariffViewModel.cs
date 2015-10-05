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
		public TariffViewModel (Tariff tariff)
		{
			Description = tariff.Description;
			Name = tariff.Name;
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
			}
		}
		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}
		private TariffType _tariffType;

		public TariffType TariffType
		{
			get { return _tariffType; }
			set { _tariffType = value;
			OnPropertyChanged(() => TariffType);
			}
		}
		public ushort TariffParts
		{
			get { return (ushort)Tariff.TariffParts.Count; }
		}
	}
}