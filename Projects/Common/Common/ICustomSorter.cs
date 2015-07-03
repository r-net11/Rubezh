using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Common
{
	public interface ICustomSorter : IComparer
	{
		ListSortDirection SortDirection { get; set; }
	}
}
