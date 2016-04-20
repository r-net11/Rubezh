using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
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
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);
			Tariffs = new ObservableCollection<TariffViewModel>();
			DbCache.Tariffs.ForEach(x => Tariffs.Add(new TariffViewModel(x)));
			SelectedTariff = Tariffs.FirstOrDefault();
		}
		public bool HasSelectedTariff { get { return SelectedTariff != null; } }

		public ObservableCollection<TariffViewModel> Tariffs { get; private set; }
		
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
		
		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var tariffDetailsViewModel = new TariffDetailsViewModel();
			if (DialogService.ShowModalWindow(tariffDetailsViewModel))
			{
				DbCache.CreateTariff(tariffDetailsViewModel.Tariff);
				DbCache.AddJournalForUser(JournalType.AddTariff, tariffDetailsViewModel.Tariff);
				var tariffViewModel = new TariffViewModel(tariffDetailsViewModel.Tariff);
				Tariffs.Add(tariffViewModel);
				SelectedTariff = tariffViewModel;
			}
		}
		public RelayCommand EditCommand { get; private set; }
		bool CanEdit()
		{
			return SelectedTariff != null && DbCache.CurrentUser.UserPermissions.Any(x => x.PermissionType == PermissionType.EditTariff);
		}
		void OnEdit()
		{
			var tariffDetailsViewModel = new TariffDetailsViewModel(SelectedTariff.Tariff);
			if (DialogService.ShowModalWindow(tariffDetailsViewModel))
			{
				DbCache.UpdateTariff(tariffDetailsViewModel.Tariff);
				DbCache.AddJournalForUser(JournalType.EditTariff, SelectedTariff.Tariff);
				SelectedTariff.Tariff = tariffDetailsViewModel.Tariff;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var index = Tariffs.IndexOf(SelectedTariff);
			DbCache.DeleteTariff(SelectedTariff.Tariff);
			DbCache.AddJournalForUser(JournalType.DeleteTariff, SelectedTariff.Tariff);
			Tariffs.Remove(SelectedTariff);
			if (Tariffs.Count == 0)
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
				var tariffViewModel = Tariffs.FirstOrDefault(x => x.Tariff.UID == tariffUID);
				if (tariffViewModel != null)
				{
					Bootstrapper.MainViewModel.SelectedTabIndex = 2;
					SelectedTariff = tariffViewModel;
				}
			}
		}
		public bool IsVisible
		{
			get { return DbCache.CheckPermission(PermissionType.ViewTariff); }
		}
	}
}