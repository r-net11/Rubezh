using System;

namespace CodeReason.Reports
{
	[Flags]
	public enum Hint
	{
		None,					// Default
		MoveRegular,			// Move regular Row/RowGroup instead of full clone process
		SimpleClone,			// Use custom Clone mechanism instead of standart XamlWriter/XamlReader
	}
}
