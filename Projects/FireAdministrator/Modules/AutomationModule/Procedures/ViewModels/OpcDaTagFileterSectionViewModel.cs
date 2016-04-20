using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AutomationModule.ViewModels
{
	public class OpcDaTagFileterSectionViewModel : SaveCancelDialogViewModel
	{
		#region Constructors

		public OpcDaTagFileterSectionViewModel(Procedure procedure)
		{
			Title = "Выбор фильтра";
			Procedure = procedure;
			InitializeFilters();
			CreateFilterCommand = new RelayCommand(OnCreateFilter);

			ServiceFactory.Events.GetEvent<CreateOpcDaTagFilterEvent>().Subscribe(ListWasChanged);
		}

		#endregion

		#region Fields And Properties

		Procedure Procedure { get; set; }

		void InitializeFilters()
		{
			Filters = new ObservableCollection<OpcTagFilterViewModel>();
			foreach (var filter in ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters
				.FindAll(x => !Procedure.OpcDaTagFiltersUids.Contains(x.UID)))
			{
				var filterViewModel = new OpcTagFilterViewModel(filter);
				Filters.Add(filterViewModel);
			}
			OnPropertyChanged(() => Filters);
			SelectedFilter = Filters.FirstOrDefault();
		}

		public RelayCommand CreateFilterCommand { get; private set; }
		void OnCreateFilter()
		{
			var opcDaTagFilterCreationViewModel = new OpcDaTagFilterCreationViewModel();
			if (DialogService.ShowModalWindow(opcDaTagFilterCreationViewModel))
			{
				var filterViewModel = opcDaTagFilterCreationViewModel.OpcDaTagFilterResult;
				ClientManager.SystemConfiguration.AutomationConfiguration.OpcDaTagFilters.Add(filterViewModel.OpcDaTagFilter);
				ServiceFactory.SaveService.AutomationChanged = true;
				
				ServiceFactoryBase.Events.GetEvent<CreateOpcDaTagFilterEvent>().Publish(filterViewModel.OpcDaTagFilter.UID);
				InitializeFilters();
			}
		}

		public ObservableCollection<OpcTagFilterViewModel> Filters { get; private set; }

		OpcTagFilterViewModel _selectedFilter;
		public OpcTagFilterViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}

		#endregion

		#region Methods

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

		protected override bool CanSave()
		{
			return SelectedFilter != null;
		}

		#endregion
	}
}