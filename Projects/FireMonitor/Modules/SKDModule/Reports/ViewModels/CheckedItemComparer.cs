using Infrastructure.Common.CheckBoxList;
using Infrastructure.Common.TreeList;

namespace SKDModule.Reports.ViewModels
{
	public class CheckedItemComparer<T> : IItemComparer
	{
		protected virtual int Compare(T x, T y)
		{
			return 0;
		}

		public int Compare(TreeNodeViewModel x, TreeNodeViewModel y)
		{
			return 0;
		}
	}
}
