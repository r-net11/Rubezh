using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Processor;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class TariffsViewModel : BaseViewModel
	{
		public TariffsViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
		}

		TariffViewModel _selectedTariff;
		public TariffViewModel SelectedTariff
		{
			get { return _selectedTariff; }
			set
			{
				_selectedTariff = value;
				OnPropertyChanged(() => SelectedTariff);
			}
		}
		public ObservableCollection<TariffViewModel> AllTariffs;

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var tariffDetailsViewModel = new TariffDetailsViewModel(null);
			if (DialogService.ShowModalWindow(tariffDetailsViewModel))
			{
				var tariffViewModel = new TariffViewModel(tariffDetailsViewModel.Tariff);
				AllTariffs.Add(tariffViewModel);
				SelectedTariff = tariffViewModel;
			}
		}
		bool CanAdd()
		{
			return true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var tariffDetailsViewModel = new TariffDetailsViewModel(SelectedTariff.Tariff);
			if (DialogService.ShowModalWindow(tariffDetailsViewModel))
			{
				var tariffViewModel = new TariffViewModel(tariffDetailsViewModel.Tariff);
				SelectedTariff.Update(tariffDetailsViewModel.Tariff);
			}
		}
		bool CanEdit()
		{
			return SelectedTariff != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			
		}
		bool CanRemove()
		{
			return SelectedTariff != null;
		}

	}
}