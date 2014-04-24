using FiresecAPI;
using Infrastructure.Common;
using System.Collections.Generic;
using System.Linq;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.CheckBoxList;
using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure.Common.TreeList;
using System;

namespace SKDModule.ViewModels
{
	public class HRFilterViewModel : OrganisationFilterBaseViewModel<HRFilter>
	{
		public EmployeeFilterViewModel EmployeeFilterViewModel { get; private set; }
		public DepartmentsFilterViewModel DepartmentsFilterViewModel { get; private set; }
		public PositionsFilterViewModel PositionsFilterViewModel { get; private set; }

		public HRFilterViewModel(HRFilter filter)
			: base(filter)
		{
			ResetCommand = new RelayCommand(OnReset);
			InitializeFilter(filter);
		}

		void InitializeFilter(HRFilter filter)
		{
			EmployeeFilterViewModel = new EmployeeFilterViewModel(filter.EmployeeFilter);
			DepartmentsFilterViewModel = new DepartmentsFilterViewModel(filter.DepartmentFilter);
			PositionsFilterViewModel = new PositionsFilterViewModel(filter.PositionFilter);
		}

		protected override bool Save()
		{
			base.Save();
			Filter.EmployeeFilter = EmployeeFilterViewModel.Save();
			Filter.DepartmentFilter = DepartmentsFilterViewModel.Save();
			Filter.PositionFilter = PositionsFilterViewModel.Save();

			Filter.EmployeeFilter.DepartmentUIDs = Filter.DepartmentFilter.UIDs.ToList();
			Filter.EmployeeFilter.PositionUIDs = Filter.PositionFilter.UIDs.ToList();

			Filter.EmployeeFilter.OrganisationUIDs = Filter.OrganisationUIDs;
			Filter.DepartmentFilter.OrganisationUIDs = Filter.OrganisationUIDs;
			Filter.PositionFilter.OrganisationUIDs = Filter.OrganisationUIDs;

			return true;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Filter = new HRFilter();
			InitializeFilter(Filter);
		}
	}
}