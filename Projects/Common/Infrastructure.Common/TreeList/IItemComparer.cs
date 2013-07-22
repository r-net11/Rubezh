using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.TreeList;

namespace Infrastructure.Common.TreeList
{
	public interface IItemComparer
	{
		int Compare(TreeNodeViewModel x, TreeNodeViewModel y);
	}
}
