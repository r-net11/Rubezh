using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class TariffDetailsViewModel : SaveCancelDialogViewModel
	{
		public Tariff Tariff;
		public TariffDetailsViewModel(Tariff tariff = null)
		{
			EditTariffPartCommand = new RelayCommand(OnEditTariffParts);
			TariffType = new ObservableCollection<TariffType>(Enum.GetValues(typeof(TariffType)).Cast<TariffType>());
			
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
		}
		public RelayCommand EditDevicesCommand { get; set; }
		void OnEditDevicesCommand()
		{

		}
		public RelayCommand EditTariffPartCommand { get; set; }
		void OnEditTariffParts()
		{
			var tariffPartViewModel = new TariffPartsViewModel(Tariff);
			if (DialogService.ShowModalWindow(tariffPartViewModel))
			{
				//var tariffViewModel = new TariffViewModel(tariffDetailsViewModel.Tariff);
				//DBCash.UpdateTariff(tariffViewModel.Tariff);
			}
		}

		public ObservableCollection<TariffType> TariffType { get; set; }
		public TariffType SelectedTariffType 
		{ 
			get 
			{
				return Tariff.TariffType;
			} 
			set
			{
				Tariff.TariffType = value; 
				OnPropertyChanged(() => SelectedTariffType); 
			}
		}

		public string Name
		{
			get { return Tariff.Name; }
			set
			{
				Tariff.Name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public string Description
		{
			get { return Tariff.Description; }
			set
			{
				Tariff.Description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public byte TariffParts
		{
			get { return (byte)Tariff.TariffParts.Count; }
		}
		protected override bool Save()
		{
			Tariff.Name = Name;
			Tariff.Description = Description;
			Tariff.TariffType = SelectedTariffType;
			for (byte i = 0; i < TariffParts; i++)
			{
				Tariff.TariffParts.Add(new TariffPart(Tariff));
			}
			return base.Save();

		}
	}
}