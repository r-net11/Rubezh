using System;
using System.Collections.Generic;

namespace RubezhAPI.Plans.Devices
{
	public interface ILibraryDevice<TLibraryState, TLibraryFrame, TStateType>
		where TLibraryFrame : ILibraryFrame
		where TLibraryState : ILibraryState<TLibraryFrame>
	{
		Guid DriverId { get; set; }
		List<TLibraryState> States { get; }
	}
}