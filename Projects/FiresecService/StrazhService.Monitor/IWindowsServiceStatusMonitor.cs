using System;
using System.ServiceProcess;

namespace StrazhService.Monitor
{
	public interface IWindowsServiceStatusMonitor
	{
		bool IsStarted { get; }
		ServiceControllerStatus Status { get; }
		event Action<ServiceControllerStatus> StatusChanged;
		bool Start();
		void Stop();
	}
}