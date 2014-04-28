using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class ScheduleViewModelNameComparer : TreeNodeComparer<ScheduleViewModel>
	{
		protected override int Compare(ScheduleViewModel x, ScheduleViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class ScheduleViewModelDescriptionComparer : TreeNodeComparer<ScheduleViewModel>
	{
		protected override int Compare(ScheduleViewModel x, ScheduleViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}
}