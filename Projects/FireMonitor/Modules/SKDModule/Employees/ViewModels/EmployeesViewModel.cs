using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : CartothequeTabItemBase<ShortEmployee, EmployeeFilter, EmployeeViewModel, EmployeeDetailsViewModel>
	{
		public PersonType PersonType { get; private set; }
		public List<ShortAdditionalColumnType> AdditionalColumnTypes { get; private set; }
		public HRViewModel HRViewModel { get; private set; }

		public EmployeesViewModel(HRViewModel hrViewModel):base()
		{
			HRViewModel = hrViewModel;
		}

		public override void Initialize(EmployeeFilter filter)
		{
			base.Initialize(filter);
			PersonType = filter.PersonType;
			InitializeAdditionalColumns();
		}	

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();
			var columnTypes = AdditionalColumnTypeHelper.GetByCurrentUser();
			if (columnTypes == null)
				return;
			columnTypes = columnTypes.Where(x => x.DataType == AdditionalColumnDataType.Text);
			AdditionalColumnTypes = columnTypes != null ? columnTypes.ToList() : new List<ShortAdditionalColumnType>();
			foreach (var additionalColumnType in AdditionalColumnTypes)
			{
				if (additionalColumnType.DataType == AdditionalColumnDataType.Text && additionalColumnType.IsInGrid)
					AdditionalColumnNames.Add(additionalColumnType.Name);
			}
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Publish(null);
		}

		protected override IEnumerable<ShortEmployee> GetModels(EmployeeFilter filter)
		{
			return EmployeeHelper.Get(filter);
		}

		protected override IEnumerable<ShortEmployee> GetModelsByOrganisation(Guid organisationUID)
		{
			return EmployeeHelper.GetShortByOrganisation(organisationUID);
		}

		protected override bool MarkDeleted(Guid uid)
		{
			return EmployeeHelper.MarkDeleted(uid);
		}

		public bool IsEmployeeSelected
		{
			get { return SelectedItem != null && !SelectedItem.IsOrganisation; }
		}

		protected override void UpdateSelected() 
		{
			OnPropertyChanged(() => IsEmployeeSelected);
			if (SelectedItem != null)
				SelectedItem.UpdatePhoto();
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }
	}
}