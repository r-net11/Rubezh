using System.Collections.Generic;
using FiresecAPI.GK;

namespace Infrustructure.Plans.Devices
{
	public interface ILibraryState<TLibraryFrame>
		where TLibraryFrame : ILibraryFrame
	{
		List<TLibraryFrame> Frames { get; set; }
		int Layer { get; set; }
		XStateClass StateType { get; }
	}
}