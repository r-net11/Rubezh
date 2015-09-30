using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class TariffDetailsViewModel : SaveCancelDialogViewModel
	{
		public Tariff Tariff { get; private set; }

		public TariffDetailsViewModel(Tariff tariff = null)
		{
			if (tariff == null)
			{
				tariff = new Tariff();
				Title = "Создание абонента";
			}
			else
			{
				Title = "Редактирование абонента";
			}

			tariff = tariff;
			Name = tariff.Name;
			Description = tariff.Description;
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

		protected override bool Save()
		{
			Tariff.Name = Name;
			Tariff.Description = Description;
			return base.Save();
		}
	}
}