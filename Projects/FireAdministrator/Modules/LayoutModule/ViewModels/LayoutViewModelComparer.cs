using Infrastructure.Common.TreeList;

namespace LayoutModule.ViewModels
{
	public class LayoutViewModelComparer : TreeNodeComparer<LayoutViewModel>
	{
		protected override int Compare(LayoutViewModel x, LayoutViewModel y)
		{
			if (x.IsFolder && y.IsLayout)
				return -1;
			if (x.IsLayout && y.IsFolder)
				return 1;
			return string.Compare(x.Caption, y.Caption);
		}
	}
}