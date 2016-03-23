using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFiltersViewModel: MenuViewPartViewModel, IDisposable
	{
		#region Constructors
		
		public OpcDaTagFiltersViewModel()
		{
			Menu = new OpcDaTagFiltersMenuViewModel(this);

			AddOpcTagFilterCommand = new RelayCommand(OnAddOpcTagFilter);
			DeleteOpcTagFilterCommand = new RelayCommand(OnDeleteOpcTagFilter, CanDeleteOpcTagFilter);
			EditOpcTagFilterCommand = new RelayCommand(OnEditOpcTagFilter, CanEditOpcTagFilter);

			var filters = ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.Select(filter => new OpcTagFilterViewModel(filter));

			Filters = new ObservableCollection<OpcTagFilterViewModel>(filters);
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

				var procedures = ClientManager.SystemConfiguration.AutomationConfiguration.Procedures
					.Where(procedure => procedure.OpcDaTagFiltersUids
						.Any(filter => filter == _selectedFilter.OpcDaTagFilter.UID));
						//.Select(p => new ProcedureDetailsViewModel(p));
				Procedures = procedures.ToArray();

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

		public void Dispose() {}
		
		#endregion

		#region Commands

		public RelayCommand AddOpcTagFilterCommand { get; private set; }
		void OnAddOpcTagFilter()
		{
			var opcDaTagFilterCreationViewModel = new OpcDaTagFilterCreationViewModel();
			if (DialogService.ShowModalWindow(opcDaTagFilterCreationViewModel))
			{
				var filterViewModel = opcDaTagFilterCreationViewModel.OpcDaTagFilterResult;
				ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters.Add(filterViewModel.OpcDaTagFilter);
				Filters.Add(new OpcTagFilterViewModel(filterViewModel.OpcDaTagFilter));
				ServiceFactory.SaveService.AutomationChanged = true;
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

				Filters.Remove(SelectedFilter);
				SelectedFilter = Filters.FirstOrDefault();

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