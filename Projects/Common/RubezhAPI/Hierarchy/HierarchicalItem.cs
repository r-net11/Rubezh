using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RubezhAPI.Hierarchy
{
	public class HierarchicalItem<T> where T : ModelBase
	{
		public HierarchicalItem()
		{
			Children = new List<HierarchicalItem<T>>();
		}

		public List<HierarchicalItem<T>> Children { get; set; }

		[XmlIgnore]
		public HierarchicalItem<T> Parent { get; set; }

		public T Item { get; set; }
	}
}