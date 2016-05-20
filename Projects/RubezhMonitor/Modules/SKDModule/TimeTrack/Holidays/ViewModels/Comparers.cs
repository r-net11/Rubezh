using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class HolidayViewModelNameComparer : TreeNodeComparer<HolidayViewModel>
	{
		protected override int Compare(HolidayViewModel x, HolidayViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class HolidayViewModelDescriptionComparer : TreeNodeComparer<HolidayViewModel>
	{
		protected override int Compare(HolidayViewModel x, HolidayViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}
}