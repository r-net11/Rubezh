using System.Collections;
using System.ComponentModel;

namespace Common
{
	public interface ICustomSorter : IComparer
	{
		ListSortDirection SortDirection { get; set; }
	}
}