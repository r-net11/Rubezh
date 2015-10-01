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
		public Tariff Tariff;

		public TariffDetailsViewModel(Tariff tariff = null)
		{
			if (tariff == null)
			{
				tariff = new Tariff
					{
						Description = "",
						Devices = new List<Device>(),
						Name = "Новый тариф",
						TariffParts = new List<TariffPart>(),
					};
				Title = "Создание нового тарифа";
			}
			else
			{
				Title = "Редактирование тарифа";
			}

			Tariff = tariff;
			Name = tariff.Name;
			Description = tariff.Description;
			


		}
		public IEnumerable<TariffType> TariffType { get { return Enum.GetValues(typeof(TariffType)).Cast<TariffType>(); } }
		public TariffType SelectedTariffType { get; set; }
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
		private Guid _UID;

		public Guid UID
		{
			get { return _UID; }
			set { _UID = value; }
		}

		private int _tariffPartsNumber;
		public int TariffPartsNumber
		{
			get { return _tariffPartsNumber; }
			set { _tariffPartsNumber = value; }
		}
		protected override bool Save()
		{
			Tariff.Name = Name;
			Tariff.Description = Description;
			return base.Save();
		}
	}
}