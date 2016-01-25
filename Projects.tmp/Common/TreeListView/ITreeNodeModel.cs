using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TreeListView
{
	public interface ITreeNodeModel
	{
		bool IsSelected { get; set; }
		bool IsExpanded { get; set; }
		IEnumerable GetChildren();
		bool HasChild();
	}
}
