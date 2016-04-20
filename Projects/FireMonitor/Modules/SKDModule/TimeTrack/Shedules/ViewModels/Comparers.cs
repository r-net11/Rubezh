using Infrastructure.Common.Windows.TreeList;

namespace SKDModule.ViewModels
{
	public class ScheduleViewModelNameComparer : TreeNodeComparer<ScheduleViewModel>
	{
		protected override int Compare(ScheduleViewModel x, ScheduleViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class ScheduleViewModelSchemeComparer : TreeNodeComparer<ScheduleViewModel>
	{
		protected override int Compare(ScheduleViewModel x, ScheduleViewModel y)
		{
			return string.Compare(x.Scheme, y.Scheme);
		}
	}
}