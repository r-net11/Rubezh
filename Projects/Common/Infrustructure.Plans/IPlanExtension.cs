using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrustructure.Plans
{
	public interface IPlanExtension
	{
		int Index { get; }
		string Alias { get; }
		string Title { get; }
		object TabPage { get; }
	}
}
