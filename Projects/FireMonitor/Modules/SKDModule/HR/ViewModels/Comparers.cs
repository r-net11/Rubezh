using System;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.SKD;
using Infrastructure.Common.TreeList;
using SKDModule.PassCardDesigner.ViewModels;
using SKDModule.Reports.ViewModels;
using DocumentType = SKDModule.Model.DocumentType;

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

	public class PassCardTemplateViewModelNameComparer : TreeNodeComparer<TemplateViewModel>
	{
		protected override int Compare(TemplateViewModel x, TemplateViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class PassCardTemplateViewModelDescriptionComparer : TreeNodeComparer<TemplateViewModel>
	{
		protected override int Compare(TemplateViewModel x, TemplateViewModel y)
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

	public class DepartmentSelectionItemViewModelNameComparer : TreeNodeComparer<DepartmentSelectionItemViewModel>
	{
		protected override int Compare(DepartmentSelectionItemViewModel x, DepartmentSelectionItemViewModel y)
		{
			return String.Compare(x.Department.Name, y.Department.Name);
		}
	}

	#region DocumentComparers
	public class DocumentNameViewModelComparer : TreeNodeComparer<DocumentType>
	{
		protected override int Compare(DocumentType x, DocumentType y)
		{
			if (x.IsOrganisation || y.IsOrganisation) return 0;
			return string.Compare(x.Name, y.Name);
		}
	}

	public class DocumentTypeViewModelComparer : TreeNodeComparer<DocumentType>
	{
		protected override int Compare(DocumentType x, DocumentType y)
		{
			if (x.IsOrganisation || y.IsOrganisation) return 0;
			return string.Compare(x.TimeTrackDocumentType.DocumentType.ToString(), y.TimeTrackDocumentType.DocumentType.ToString());
		}
	}

	public class DocumentCodeViewModelComparer : TreeNodeComparer<DocumentType>
	{
		protected override int Compare(DocumentType x, DocumentType y)
		{
			if (x.IsOrganisation || y.IsOrganisation) return 0;
			return x.TimeTrackDocumentType.Code.CompareTo(y.TimeTrackDocumentType.Code);
		}
	}

	public class DocumentShortNameViewModelComparer : TreeNodeComparer<DocumentType>
	{
		protected override int Compare(DocumentType x, DocumentType y)
		{
			if (x.IsOrganisation || y.IsOrganisation) return 0;
			return string.Compare(x.TimeTrackDocumentType.ShortName, y.TimeTrackDocumentType.ShortName);
		}
	}

	public class DepartmentFilterComparer : TreeNodeComparer<DepartmentFilterItemViewModel>
	{
		protected override int Compare(DepartmentFilterItemViewModel x, DepartmentFilterItemViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Name, y.Name);
		}
	}
	public class DepartmentFilterDescriptionComparer : TreeNodeComparer<DepartmentFilterItemViewModel>
	{
		protected override int Compare(DepartmentFilterItemViewModel x, DepartmentFilterItemViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Description, y.Description);
		}
	}

	public class EmployeesFilterItemNameViewModelComparer : TreeNodeComparer<EmployeesFilterItemViewModel>
	{
		protected override int Compare(EmployeesFilterItemViewModel x, EmployeesFilterItemViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Name, y.Name);
		}
	}

	public class CommonScheduleViewModelComparer : CheckedItemComparer<CheckedItemViewModel<CommonScheduleViewModel>>
	{
		protected override int Compare(CheckedItemViewModel<CommonScheduleViewModel> x, CheckedItemViewModel<CommonScheduleViewModel> y)
		{
			if (x == null || y == null || x.Item == null || y.Item == null) return 0;
			return string.Compare(x.Item.Name, y.Item.Name);
		}
	}

	public class ScheduleSchemePageComparer : TreeNodeComparer<TreeNodeItemViewModel>
	{
		protected override int Compare(TreeNodeItemViewModel x, TreeNodeItemViewModel y)
		{
			if (x == null || y == null || x.Item == null || y.Item == null) return 0;

			var valueX = x.Item.GetType().GetProperty("Name").GetValue(x.Item, null);
			var valueY = y.Item.GetType().GetProperty("Name").GetValue(y.Item, null);

			return string.Compare(valueX.ToString(), valueY.ToString());
		}
	}

	public class PositionFilterItemViewModelNameComparer : TreeNodeComparer<PositionFilterItemViewModel>
	{
		protected override int Compare(PositionFilterItemViewModel x, PositionFilterItemViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Name, y.Name);
		}
	}

	public class PositionFilterItemViewModelDescriptionComparer : TreeNodeComparer<PositionFilterItemViewModel>
	{
		protected override int Compare(PositionFilterItemViewModel x, PositionFilterItemViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Description, y.Description);
		}
	}

	public class DoorPageViewModelNameComparer : CheckedItemComparer<CheckedItemViewModel<SKDDoor>>
	{
		protected override int Compare(CheckedItemViewModel<SKDDoor> x, CheckedItemViewModel<SKDDoor> y)
		{
			if (x == null || x.Item == null || y == null || y.Item == null) return 0;
			return string.Compare(x.Item.Name, y.Item.Name);
		}
	}

	public class DoorPageViewModelDescriptionComparer : CheckedItemComparer<CheckedItemViewModel<SKDDoor>>
	{
		protected override int Compare(CheckedItemViewModel<SKDDoor> x, CheckedItemViewModel<SKDDoor> y)
		{
			if (x == null || x.Item == null || y == null || y.Item == null) return 0;
			return string.Compare(x.Item.Description, y.Item.Description);
		}
	}

	#endregion

	public class AccessTemplateFilterComparer : TreeNodeComparer<AccessTemplateFilterItemViewModel>
	{
		protected override int Compare(AccessTemplateFilterItemViewModel x, AccessTemplateFilterItemViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Name, y.Name);
		}
	}
	public class AccessTemplateFilterDescriptionComparer : TreeNodeComparer<AccessTemplateFilterItemViewModel>
	{
		protected override int Compare(AccessTemplateFilterItemViewModel x, AccessTemplateFilterItemViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Description, y.Description);
		}
	}

	#region <Компараторы для EmployeeCardForApplyAccessTemplateToUserGroupViewModel>

	public class EmployeeCardForApplyAccessTemplateToUserGroupViewModelNameComparer : TreeNodeComparer<EmployeeCardForApplyAccessTemplateToUserGroupViewModel>
	{
		protected override int Compare(EmployeeCardForApplyAccessTemplateToUserGroupViewModel x, EmployeeCardForApplyAccessTemplateToUserGroupViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Name, y.Name);
		}
	}

	public class EmployeeCardForApplyAccessTemplateToUserGroupViewModelDepartmentComparer : TreeNodeComparer<EmployeeCardForApplyAccessTemplateToUserGroupViewModel>
	{
		protected override int Compare(EmployeeCardForApplyAccessTemplateToUserGroupViewModel x, EmployeeCardForApplyAccessTemplateToUserGroupViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.Department, y.Department);
		}
	}

	public class EmployeeCardForApplyAccessTemplateToUserGroupViewModelPositionOrDescriptionComparer : TreeNodeComparer<EmployeeCardForApplyAccessTemplateToUserGroupViewModel>
	{
		protected override int Compare(EmployeeCardForApplyAccessTemplateToUserGroupViewModel x, EmployeeCardForApplyAccessTemplateToUserGroupViewModel y)
		{
			if (x == null || y == null) return 0;
			return string.Compare(x.PositionOrDescription, y.PositionOrDescription);
		}
	}

	#endregion </Компараторы для EmployeeCardForApplyAccessTemplateToUserGroupViewModel>
}