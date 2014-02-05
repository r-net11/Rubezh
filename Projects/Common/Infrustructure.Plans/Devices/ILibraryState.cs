using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrustructure.Plans.Devices
{
	public interface ILibraryState<TLibraryFrame>
		where TLibraryFrame : ILibraryFrame
	{
		string Code { get; set; }
		List<TLibraryFrame> Frames { get; set; }
		int Layer { get; set; }
	}
}