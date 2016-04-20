using System;
using RubezhAPI;
using Infrastructure.Common.Windows.TreeList;
using SKDModule.PassCardDesigner.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeeViewModelLastNameComparer : TreeNodeComparer<EmployeeViewModel>
	{
		protected override int Compare(EmployeeViewModel x, EmployeeViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class EmployeeViewModelDepartmentComparer : TreeNodeComparer<EmployeeViewModel>
	{
		protected override int Compare(EmployeeViewModel x, EmployeeViewModel y)
		{
			return string.Compare(x.DepartmentName, y.DepartmentName);
		}
	}

	public class EmployeeViewModelPositionComparer : TreeNodeComparer<EmployeeViewModel>
	{
		protected override int Compare(EmployeeViewModel x, EmployeeViewModel y)
		{
			return string.Compare(x.PositionName, y.PositionName);
		}
	}

	public class EmployeeViewModelDescriptionComparer : TreeNodeComparer<EmployeeViewModel>
	{
		protected override int Compare(EmployeeViewModel x, EmployeeViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}

	public class PositionViewModelNameComparer : TreeNodeComparer<PositionViewModel>
	{
		protected override int Compare(PositionViewModel x, PositionViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class PositionViewModelDescriptionComparer : TreeNodeComparer<PositionViewModel>
	{
		protected override int Compare(PositionViewModel x, PositionViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}

	public class DepartmentViewModelNameComparer : TreeNodeComparer<DepartmentViewModel>
	{
		protected override int Compare(DepartmentViewModel x, DepartmentViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class DepartmentViewModelPhoneComparer : TreeNodeComparer<DepartmentViewModel>
	{
		protected override int Compare(DepartmentViewModel x, DepartmentViewModel y)
		{
			return string.Compare(x.Phone, y.Phone);
		}
	}

	public class DepartmentViewModelDescriptionComparer : TreeNodeComparer<DepartmentViewModel>
	{
		protected override int Compare(DepartmentViewModel x, DepartmentViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}

	public class AdditionalColumnTypeViewModelNameComparer : TreeNodeComparer<AdditionalColumnTypeViewModel>
	{
		protected override int Compare(AdditionalColumnTypeViewModel x, AdditionalColumnTypeViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class AdditionalColumnTypeViewModelDescriptionComparer : TreeNodeComparer<AdditionalColumnTypeViewModel>
	{
		protected override int Compare(AdditionalColumnTypeViewModel x, AdditionalColumnTypeViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}

	public class AdditionalColumnTypeViewModelDataTypeComparer : TreeNodeComparer<AdditionalColumnTypeViewModel>
	{
		protected override int Compare(AdditionalColumnTypeViewModel x, AdditionalColumnTypeViewModel y)
		{
			return string.Compare(x.DataType, y.DataType);
		}
	}

	public class PassCardTemplateViewModelNameComparer : TreeNodeComparer<PassCardTemplateViewModel>
	{
		protected override int Compare(PassCardTemplateViewModel x, PassCardTemplateViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class PassCardTemplateViewModelDescriptionComparer : TreeNodeComparer<PassCardTemplateViewModel>
	{
		protected override int Compare(PassCardTemplateViewModel x, PassCardTemplateViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}

	public class CardViewModelNumberComparer : TreeNodeComparer<CardViewModel>
	{
		protected override int Compare(CardViewModel x, CardViewModel y)
		{
			return string.Compare(x.Number, y.Number);
		}
	}

	public class CardViewModelTypeComparer : TreeNodeComparer<CardViewModel>
	{
		protected override int Compare(CardViewModel x, CardViewModel y)
		{
			return string.Compare(x.CardType, y.CardType);
		}
	}

	public class CardViewModelEmployeeNameComparer : TreeNodeComparer<CardViewModel>
	{
		protected override int Compare(CardViewModel x, CardViewModel y)
		{
			return string.Compare(x.EmployeeName, y.EmployeeName);
		}
	}

	public class CardViewModelStopReasonComparer : TreeNodeComparer<CardViewModel>
	{
		protected override int Compare(CardViewModel x, CardViewModel y)
		{
			return string.Compare(x.StopReason, y.StopReason);
		}
	}

	public class HolidayViewModelDateComparer : TreeNodeComparer<HolidayViewModel>
	{
		protected override int Compare(HolidayViewModel x, HolidayViewModel y)
		{
			if (x.IsOrganisation || y.IsOrganisation)
				return 0;
			return DateTime.Compare(x.Model.Date, y.Model.Date);
		}
	}

	public class HolidayViewModelReductionTimeComparer : TreeNodeComparer<HolidayViewModel>
	{
		protected override int Compare(HolidayViewModel x, HolidayViewModel y)
		{
			return string.Compare(x.ReductionTime, y.ReductionTime);
		}
	}

	public class HolidayViewModelTypeComparer : TreeNodeComparer<HolidayViewModel>
	{
		protected override int Compare(HolidayViewModel x, HolidayViewModel y)
		{
			if (x.IsOrganisation || y.IsOrganisation)
				return 0;
			return string.Compare(x.Model.Type.ToDescription(), y.Model.Type.ToDescription());
		}
	}

	public class HolidayViewModelTransitionDateComparer : TreeNodeComparer<HolidayViewModel>
	{
		protected override int Compare(HolidayViewModel x, HolidayViewModel y)
		{
			return string.Compare(x.TransitionDate, y.TransitionDate);
		}
	}
}