using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeReason.Reports
{
	[Flags]
	public enum Hint
	{
		None,					// Default
		SkipCloneeRegular,		// Move regular Row/RowGroup instead of full clone process
		CustomClone,			// Use custom Clone mechanism instead of standart XamlWriter/XamlReader
	}
}
