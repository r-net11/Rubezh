using Infrastructure.Common.TreeList;
using XFiresecAPI;
using SKDModule.Intervals.ScheduleShemes.ViewModels;

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
}