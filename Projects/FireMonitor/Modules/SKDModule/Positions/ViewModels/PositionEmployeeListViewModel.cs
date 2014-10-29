using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;

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
			return EmployeeHelper.SetPosition(employee, Guid.Empty);
		}

		public override bool CanEditPosition { get { return false; } }
		public override bool CanEditDepartment { get { return true; } }

		protected override EmployeeFilter Filter
		{
			get { return new EmployeeFilter { PositionUIDs = new List<Guid> { _parent.UID } }; }
		}

		protected override EmployeeFilter EmptyFilter
		{
			get { return new EmployeeFilter { PositionUIDs = new List<Guid> { Guid.Empty } }; }
		}

		protected override Guid GetParentUID(Employee employee)
		{
			return employee.Position != null ? employee.Position.UID : Guid.Empty;
		}
	}
}
