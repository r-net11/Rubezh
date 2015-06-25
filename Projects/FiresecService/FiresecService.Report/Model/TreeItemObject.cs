﻿using System.Collections.Generic;

namespace FiresecService.Report.Model
{
	public class TreeItemObject<T>
	{
		public TreeItemObject<T> Parent { get; set; }
		public List<TreeItemObject<T>> Children { get; set; }
	}
}
