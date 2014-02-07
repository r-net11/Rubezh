using System.Collections.Generic;

namespace Infrustructure.Plans.Devices
{
	public interface ILibraryState<TLibraryFrame, TStateType>
		where TLibraryFrame : ILibraryFrame
	{
		string Code { get; set; }
		List<TLibraryFrame> Frames { get; set; }
		int Layer { get; set; }
		TStateType StateType { get; }
	}
}