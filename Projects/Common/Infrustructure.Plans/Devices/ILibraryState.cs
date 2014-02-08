using System.Collections.Generic;

namespace Infrustructure.Plans.Devices
{
	public interface ILibraryState<TLibraryFrame, TStateType>
		where TLibraryFrame : ILibraryFrame
	{
		List<TLibraryFrame> Frames { get; set; }
		int Layer { get; set; }
		TStateType StateType { get; }
	}
}