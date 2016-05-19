using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class ScheduleShemeViewModelNameComparer : TreeNodeComparer<ScheduleSchemeViewModel>
	{
		protected override int Compare(ScheduleSchemeViewModel x, ScheduleSchemeViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class ScheduleShemeViewModelDescriptionComparer : TreeNodeComparer<ScheduleSchemeViewModel>
	{
		protected override int Compare(ScheduleSchemeViewModel x, ScheduleSchemeViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}

	public class ScheduleShemeViewModelTypeComparer : TreeNodeComparer<ScheduleSchemeViewModel>
	{
		protected override int Compare(ScheduleSchemeViewModel x, ScheduleSchemeViewModel y)
		{
			return string.Compare(x.Type, y.Type);
		}
	}
}