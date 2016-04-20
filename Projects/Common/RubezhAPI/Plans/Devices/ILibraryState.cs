using RubezhAPI.GK;
using System.Collections.Generic;

namespace RubezhAPI.Plans.Devices
{
	public interface ILibraryState<TLibraryFrame>
		where TLibraryFrame : ILibraryFrame
	{
		List<TLibraryFrame> Frames { get; set; }
		int Layer { get; set; }
		XStateClass StateType { get; }
	}
}