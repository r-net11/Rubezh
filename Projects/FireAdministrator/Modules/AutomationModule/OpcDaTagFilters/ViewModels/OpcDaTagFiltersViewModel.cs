using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFiltersViewModel: MenuViewPartViewModel, IDisposable
	{
		#region Constructors
		
		public OpcDaTagFiltersViewModel()
		{
			Menu = new OpcDaTagFiltersMenuViewModel(this);

			CreateOpcTagFilterCommand = new RelayCommand(OnCreateOpcTagFilter);
			DeleteOpcTagFilterCommand = new RelayCommand(OnDeleteOpcTagFilter, CanDeleteOpcTagFilter);
			EditOpcTagFilterCommand = new RelayCommand(OnEditOpcTagFilter, CanEditOpcTagFilter);

			RegisterShortcuts();

			var filters = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.Select(filter => new OpcTagFilterViewModel(filter));

			Filters = new ObservableCollection<OpcTagFilterViewModel>(filters);

			ServiceFactoryBase.Events.GetEvent<CreateOpcDaTagFilterEvent>().Subscribe(ListWasChanged);
		}

		#endregion

		#region Fields And Properties

		public ObservableCollection<OpcTagFilterViewModel> Filters { get; private set; }
		
		OpcTagFilterViewModel _selectedFilter;
		public OpcTagFilterViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;

				if (_selectedFilter != null)
				{
					var procedures = ClientManager.SystemConfiguration.AutomationConfiguration.Procedures
						.Where(procedure => procedure.OpcDaTagFiltersUids
							.Any(filter => filter == _selectedFilter.OpcDaTagFilter.UID));
					Procedures = procedures.ToArray();
				}
				else
				{
					Procedures = null;
				}

				OnPropertyChanged(() => SelectedFilter);
			}
		}

		Procedure[] _Procedures;
		public Procedure[] Procedures
		{
			get { return _Procedures; }
			private set
			{
				_Procedures = value;
				OnPropertyChanged(() => Procedures);
			}
		}

		#endregion

		#region Methods

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), CreateOpcTagFilterCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditOpcTagFilterCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteOpcTagFilterCommand);
		}

		public void Dispose() {}

		public void ListWasChanged(Guid filterUID)
		{
			var newFilter = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.FirstOrDefault(filter => filter.UID == filterUID);
			
			if (newFilter != null)
			{
				if (!Filters.Any(vm => vm.OpcDaTagFilter.UID == newFilter.UID))
				{
					Filters.Add(new OpcTagFilterViewModel(newFilter));
				}
			}
		}

		public override void OnShow()
		{
			// Обновляем все модели
			var filters = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.Select(filter => new OpcTagFilterViewModel(filter));
			if (Filters != null)
			{
				Filters.Clear();
				var list = new ObservableCollection<OpcTagFilterViewModel>(filters);
				foreach (var item in list)
				{
					Filters.Add(item);
				}
			}
			else
			{
				Filters = new ObservableCollection<OpcTagFilterViewModel>(filters);
			}
			SelectedFilter = Filters.FirstOrDefault();

			base.OnShow();
		}

		#endregion

		#region Commands

		public RelayCommand CreateOpcTagFilterCommand { get; private set; }
		void OnCreateOpcTagFilter()
		{
			var opcDaTagFilterCreationViewModel = new OpcDaTagFilterCreationViewModel();
			if (DialogService.ShowModalWindow(opcDaTagFilterCreationViewModel))
			{
				var filterViewModel = opcDaTagFilterCreationViewModel.OpcDaTagFilterResult;
				ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters.Add(filterViewModel.OpcDaTagFilter);
				var newFilter = new OpcTagFilterViewModel(filterViewModel.OpcDaTagFilter);
				Filters.Add(newFilter);
				SelectedFilter = newFilter;
				ServiceFactory.SaveService.AutomationChanged = true;
				ServiceFactoryBase.Events.GetEvent<CreateOpcDaTagFilterEvent>().Publish(filterViewModel.OpcDaTagFilter.UID);
			}
		}

		public RelayCommand DeleteOpcTagFilterCommand { get; private set; }
		void OnDeleteOpcTagFilter()
		{
			if (MessageBoxService.ShowConfirmation("Вы действительно хотите удалить фильтр?", "Подтверждение на удаление"))
			{
				// Удаляем фильтр из связанных с ним процедур
				foreach (var procedure in Procedures)
				{
					procedure.OpcDaTagFiltersUids.Remove(SelectedFilter.OpcDaTagFilter.UID);
				}
				// Удалаяем сам фильтр из конфигурации
				ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
					.Remove(SelectedFilter.OpcDaTagFilter);

				ServiceFactory.Events.GetEvent<DeleteOpcDaTagFilterEvent>().Publish(SelectedFilter.OpcDaTagFilter.UID);

				var removedFilter = SelectedFilter;

				var index = Filters.IndexOf(SelectedFilter);

				if (index - 1 >= 0)
					SelectedFilter = Filters.ElementAt(index - 1);
				else if (index + 1 < Filters.Count)
					SelectedFilter = Filters.ElementAt(index + 1);
				else
					SelectedFilter = null;

				Filters.Remove(removedFilter);

				ServiceFactory.SaveService.AutomationChanged = true;

			}
		}
		bool CanDeleteOpcTagFilter()
		{
			return SelectedFilter != null;
		}

		public RelayCommand EditOpcTagFilterCommand { get; private set; }
		void OnEditOpcTagFilter()
		{
			var opcDaTagFilterEditingViewModel = new OpcDaTagFilterEditingViewModel(SelectedFilter);

			if (DialogService.ShowModalWindow(opcDaTagFilterEditingViewModel))
			{
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
		bool CanEditOpcTagFilter()
		{
			return SelectedFilter != null;
		}

		#endregion
	}
}