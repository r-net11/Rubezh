using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class NamedIntervalViewModelNameComparer : TreeNodeComparer<NamedIntervalViewModel>
	{
		protected override int Compare(NamedIntervalViewModel x, NamedIntervalViewModel y)
		{
			return string.Compare(x.Name, y.Name);
		}
	}

	public class NamedIntervalViewModelDescriptionComparer : TreeNodeComparer<NamedIntervalViewModel>
	{
		protected override int Compare(NamedIntervalViewModel x, NamedIntervalViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}
}