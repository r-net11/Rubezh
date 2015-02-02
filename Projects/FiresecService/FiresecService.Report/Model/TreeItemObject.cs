using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecService.Report.Model
{
	public class TreeItemObject<T>
	{
		public TreeItemObject<T> Parent { get; set; }
		public List<TreeItemObject<T>> Children { get; set; }
	}
}
