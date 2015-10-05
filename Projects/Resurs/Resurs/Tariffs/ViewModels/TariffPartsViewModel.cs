using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	class TariffPartsViewModel : SaveCancelDialogViewModel
	{
		public TariffPartsViewModel(Tariff tariff)
		{
			Title = "Редактирование тарифных интервалов";
			TariffParts = new ObservableCollection<TariffPartViewModel>();
			foreach (var item in tariff.TariffParts)
			{
				TariffParts.Add(new TariffPartViewModel(item));
			}
			AddTariffPartCommand = new RelayCommand(OnAddTariffPartCommand, canAddTariffPart);
			Tariff = tariff;
		}
		public Tariff Tariff { get; set; }
		private bool canAddTariffPart(object obj)
		{
			if (TariffParts.Count < 8)
			{
				return true;
			}
			else return false;
		}
		public RelayCommand AddTariffPartCommand { get; set; }
		void OnAddTariffPartCommand()
		{
			TariffParts.Add(new TariffPartViewModel( new TariffPart(Tariff)));
		}
		private ObservableCollection<TariffPartViewModel> _tariffParts;
		public ObservableCollection<TariffPartViewModel> TariffParts
		{
			get { return _tariffParts; }
			set 
			{ 
				_tariffParts = value;
				OnPropertyChanged(() => TariffParts);
			}
		}
		
	}
}
