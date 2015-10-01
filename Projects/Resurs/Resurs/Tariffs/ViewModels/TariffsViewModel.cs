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
			Initialize();
		}

		private void Initialize()
		{
			Tariffs = new ObservableCollection<TariffViewModel>();
			
		}
		public bool HasSelectedTariff { get { return SelectedTariff != null; } }

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
		private ObservableCollection<TariffViewModel> _tariffs;

		public ObservableCollection<TariffViewModel> Tariffs
		{
			get { return _tariffs; }
			set 
			{ 
				_tariffs = value;
				OnPropertyChanged(() => Tariffs);
			}
		}


		public RelayCommand AddCommand { get; private set; }
		bool CanAdd()
		{
			return true;
		}
		void OnAdd()
		{
			var tariffDetailsViewModel = new TariffDetailsViewModel(null);
			if(DialogService.ShowModalWindow(tariffDetailsViewModel))
			{
				var tariffViewModel = new TariffViewModel(tariffDetailsViewModel.Tariff);
				Tariffs.Add(tariffViewModel);
				SelectedTariff = tariffViewModel;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		bool CanEdit()
		{
			return SelectedTariff != null;
		}
		void OnEdit()
		{
			var tariffDetailsViewModel = new TariffDetailsViewModel(SelectedTariff.Tariff);
			if (DialogService.ShowModalWindow(tariffDetailsViewModel))
			{
				var tariffViewModel = new TariffViewModel(tariffDetailsViewModel.Tariff);
				SelectedTariff.Update(tariffDetailsViewModel.Tariff);
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		bool CanRemove()
		{
			return SelectedTariff != null;
		}
		void OnRemove()
		{
			
		}

	}
}