using System;

namespace StrazhService.Monitor
{
	public interface IServiceStateHolder
	{
		ServiceState State { get; set; }

		event Action<ServiceState> ServiceStateChanged;
	}
}