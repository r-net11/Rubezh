using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace StrazhService.Monitor
{
	public class WindowsServiceStatusMonitor : IWindowsServiceStatusMonitor
	{
		private const string ServiceName = "StrazhService";

		private readonly ServiceController _serviceController;

		private CancellationTokenSource _cancellationTokenSource;

		public WindowsServiceStatusMonitor(ServiceController serviceController)
		{
			_serviceController = serviceController;
		}

		#region <Реализация IWindowsServiceStatusMonitor>

		public bool IsStarted { get; private set; }

		public ServiceControllerStatus Status { get; private set; }

		public event Action<ServiceControllerStatus> StatusChanged;

		public bool Start()
		{
			var localServices = ServiceController.GetServices();
			if (localServices.All(x => x.ServiceName != ServiceName))
			{
				Logger.Info(string.Format("Служба '{0}' не обнаружена", ServiceName));
				return IsStarted;
			}

			Logger.Info(string.Format("Служба '{0}' обнаружена", ServiceName));
			IsStarted = true;
			Logger.Info(string.Format("Начинаем слежение за службой '{0}'", ServiceName));
			_cancellationTokenSource = new CancellationTokenSource();
			Task.Factory.StartNew(() => ForegroundWork(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
			return IsStarted;
		}

		public void Stop()
		{
			if (_cancellationTokenSource == null)
				return;

			Logger.Info(string.Format("Останавливаем слежение за статусом службы '{0}'", ServiceName));
			_cancellationTokenSource.Cancel();
		}

		#endregion <Реализация IWindowsServiceStatusMonitor>

		private void ForegroundWork(CancellationToken cancellationToken)
		{
			Status = _serviceController.Status;
			Logger.Info(string.Format("Cтатус службы '{0}' изменился на '{1}'", ServiceName, Status));
			RaiseStatusChanged(Status);

			while (true)
			{
				_serviceController.Refresh();
				var newStatus = _serviceController.Status;
				if (newStatus != Status)
				{
					Status = newStatus;
					Logger.Info(string.Format("Cтатус службы '{0}' изменился на '{1}'", ServiceName, Status));
					RaiseStatusChanged(Status);
				}
				cancellationToken.ThrowIfCancellationRequested();
			}
		}

		private void RaiseStatusChanged(ServiceControllerStatus status)
		{
			var temp = StatusChanged;
			if (temp != null)
				temp(status);
		}
	}
}