using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecService.Report.Model
{
	public class DeletableObjectInfo<T> : ObjectInfo
	{
		public string Name { get; set; }
		public bool IsDeleted { get; set; }
		public T Item { get; set; }
	}
}
