using System;
using System.Collections.Generic;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class PositionEmployeeListViewModel : EmployeeListBaseViewModel<EmployeeListItemViewModel>
	{
		public PositionEmployeeListViewModel(PositionViewModel parent) : base(parent) { }

		protected override bool AddToParent(ShortEmployee employee)
		{
			return EmployeeHelper.SetPosition(employee, _parent.UID);
		}

		protected override bool RemoveFromParent(ShortEmployee employee)
		{
			return EmployeeHelper.SetPosition(employee, null);
		}

		public override bool CanEditPosition { get { return false; } }
		public override bool CanEditDepartment { get { return true; } }

		protected override EmployeeFilter Filter
		{
			get 
            { 
                return new EmployeeFilter 
                { 
                    PositionUIDs = new List<Guid> { _parent.UID }, 
                    OrganisationUIDs = new List<Guid> { _parent.OrganisationUID }, 
                    LogicalDeletationType = _isWithDeleted ? LogicalDeletationType.All : LogicalDeletationType.Active 
                }; 
            }
		}

		protected override EmployeeFilter EmptyFilter
		{
			get 
            { 
                return new EmployeeFilter 
                { 
                    OrganisationUIDs = new List<Guid> { _parent.OrganisationUID }, 
                    IsEmptyPosition = true
                }; 
            }
		}

		protected override Guid GetParentUID(Employee employee)
		{
			return employee.PositionUID;
		}
	}
}
