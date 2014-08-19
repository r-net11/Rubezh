using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class DayIntervalViewModelNameComparer : TreeNodeComparer<DayIntervalViewModel>
	{
		protected override int Compare(DayIntervalViewModel x, DayIntervalViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class DayIntervalViewModelDescriptionComparer : TreeNodeComparer<DayIntervalViewModel>
	{
		protected override int Compare(DayIntervalViewModel x, DayIntervalViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}
}