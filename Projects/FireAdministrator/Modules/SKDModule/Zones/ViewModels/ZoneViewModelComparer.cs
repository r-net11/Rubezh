using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class ZoneViewModelNameComparer : TreeNodeComparer<ZoneViewModel>
	{
		protected override int Compare(ZoneViewModel x, ZoneViewModel y)
		{
			return string.Compare(x.Zone.Name, y.Zone.Name);
		}
	}
	public class ZoneViewModelDescriptionComparer : TreeNodeComparer<ZoneViewModel>
	{
		protected override int Compare(ZoneViewModel x, ZoneViewModel y)
		{
			return string.Compare(x.Description, y.Description);
		}
	}
}