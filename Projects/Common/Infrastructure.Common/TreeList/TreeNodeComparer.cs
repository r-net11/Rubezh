using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;

namespace Infrastructure.Common.TreeList
{
	public class TreeNodeComparer<T> : IItemComparer
		where T : TreeNodeViewModel<T>
	{
		protected virtual int Compare(T x, T y)
		{
			return 0;
		}

		#region IItemComparer Members

		int IItemComparer.Compare(TreeNodeViewModel x, TreeNodeViewModel y)
		{
			return Compare((T)x, (T)y);
		}

		#endregion
	}
}
