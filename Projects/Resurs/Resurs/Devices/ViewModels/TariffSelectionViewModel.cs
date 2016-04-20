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
	public class TariffSelectionViewModel : SaveCancelDialogViewModel
	{
		public TariffSelectionViewModel(DeviceDetailsViewModel parentViewModel)
		{
			Tariffs = new ObservableCollection<Tariff>(DbCache.Tariffs.Where(x => 
				x.TariffType == parentViewModel.TariffType && 
				x.TariffParts.Count <= parentViewModel.Device.MaxTatiffParts));
			_emptyTariff = new Tariff { Name = "Открепить от тарифа" };
			Tariffs.Insert(0, _emptyTariff);
			if(parentViewModel.Tariff != null)
				SelectedTariff = Tariffs.FirstOrDefault(x => x.UID == parentViewModel.Tariff.UID);
		}

		Tariff _emptyTariff;

		Tariff _selectedTariff;
		public Tariff SelectedTariff
		{
			get { return _selectedTariff; }
			set
			{
				_selectedTariff = value;
				OnPropertyChanged(() => SelectedTariff);
			}
		}

		public Tariff Tariff 
		{ 
			get 
			{ 
				if (SelectedTariff.UID == _emptyTariff.UID) 
					return null; 
				return SelectedTariff; 
			} 
		}

		public ObservableCollection<Tariff> Tariffs { get; private set; }
	}
}
