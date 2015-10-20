using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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
			DBCash.Tariffs.ForEach(x => Tariffs.Add(new TariffViewModel(x)));
			SelectedTariff = Tariffs.FirstOrDefault();
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
				OnPropertyChanged(() => HasSelectedTariff);
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
			return DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditTariff);
		}
		void OnAdd()
		{
			var tariffDetailsViewModel = new TariffDetailsViewModel();
			if (DialogService.ShowModalWindow(tariffDetailsViewModel))
			{
				var tariffViewModel = new TariffViewModel(tariffDetailsViewModel.Tariff);
				Tariffs.Add(tariffViewModel);
				SelectedTariff = tariffViewModel;
				DBCash.CreateTariff(tariffViewModel.Tariff);
				DBCash.AddJournalForUser(JournalType.AddTariff, SelectedTariff.Tariff);
			}
		}

		public RelayCommand EditCommand { get; private set; }
		bool CanEdit()
		{
			return SelectedTariff != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditTariff);
		}
		void OnEdit()
		{
			var tariffDetailsViewModel = new TariffDetailsViewModel(SelectedTariff.Tariff);
			if (DialogService.ShowModalWindow(tariffDetailsViewModel))
			{
				var tariffViewModel = new TariffViewModel(tariffDetailsViewModel.Tariff);
				SelectedTariff.Tariff = tariffDetailsViewModel.Tariff;
				DBCash.UpdateTariff(tariffDetailsViewModel.Tariff);
				DBCash.AddJournalForUser(JournalType.EditTariff, SelectedTariff.Tariff);
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		bool CanRemove()
		{
			return SelectedTariff != null && DBCash.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditTariff);
		}
		void OnRemove()
		{
			var index = Tariffs.IndexOf(SelectedTariff);
			DBCash.DeleteTariff(SelectedTariff.Tariff);
			DBCash.AddJournalForUser(JournalType.DeleteTariff, SelectedTariff.Tariff);
			Tariffs.Remove(SelectedTariff);
			if (Tariffs.FirstOrDefault() == null)
				SelectedTariff = null;
			else if (Tariffs.ElementAtOrDefault(index) != null)
			{
				SelectedTariff = Tariffs.ElementAt(index);
			}
			else 
			{
				SelectedTariff = Tariffs.ElementAt(index - 1);
			}
		}

		public void Select(Guid tariffUID)
		{
			if (tariffUID != Guid.Empty && IsVisible)
			{
				var _tariffViewModel = Tariffs.FirstOrDefault(x => x.Tariff.UID == tariffUID);
				if (_tariffViewModel != null)
				{
					Bootstrapper.MainViewModel.SelectedTabIndex = 6;
					SelectedTariff = _tariffViewModel;
				}
			}
		}
		public bool IsVisible
		{
			get { return DBCash.CheckPermission(PermissionType.ViewTariff); }
		}
	}
}