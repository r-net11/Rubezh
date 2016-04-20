using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Common;
using Infrastructure.Common.Windows;

namespace Infrastructure.Common
{
	public static class SingleLaunchHelper
	{
		public static bool KillRunningProcess(bool force = false)
		{
			string processName = Process.GetCurrentProcess().ProcessName;
			bool result = true;
			Process[] processes = null;

			try
			{
				processes = Process.GetProcessesByName(processName);

				if (processes.Count() > 1)
				{
					if (!force)
						result = MessageBoxService.ShowQuestion("Другой экземпляр программы уже запущен. Завершить?");

					if (result)
					{
						foreach (var process in processes)
						{
							if (process.Id == Process.GetCurrentProcess().Id)
								continue;

							if ((process != null) && (process.HasExited == false))
							{
								process.CloseMainWindow();
								//process.WaitForExit();
								process.Kill();
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SingleLaunchHelper.KillRunningProcess");
			}

			return result;
		}
	}

	public static class MutexHelper
	{
		static Mutex Mutex { get; set; }

		public static bool IsNew(string mutexName)
		{
			bool isNew;
			Mutex = new Mutex(true, mutexName, out isNew);
			return isNew;
		}

		public static void KeepAlive()
		{
			if (Mutex != null)
				GC.KeepAlive(Mutex);
		}
	}

	public class DoubleLaunchLocker : IDisposable
	{
		public string SignalId { get; private set; }
		public string WaitId { get; private set; }
		const int TIMEOUT = 3000;

		EventWaitHandle _signalHandler;
		EventWaitHandle _waitHandler;
		bool _force = false;
		Action ShuttingDown;


		public DoubleLaunchLocker(string signalId, string waitId, bool force = false, Action shuttingDown = null)
		{
			SignalId = signalId;
			WaitId = waitId;
			_force = force;
			ShuttingDown = shuttingDown;

			bool isNew;
			_signalHandler = new EventWaitHandle(false, EventResetMode.AutoReset, signalId, out isNew);
			if (!isNew)
			{
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				bool terminate = false;
				if (!force && !RequestConfirmation())
					terminate = true;
				if (!terminate)
				{
					_waitHandler = new EventWaitHandle(false, EventResetMode.AutoReset, waitId);
					_signalHandler.Dispose();
					_signalHandler = new EventWaitHandle(false, EventResetMode.ManualReset, signalId, out isNew);
					if (!isNew)
					{
						_signalHandler.Set();
						terminate = !_waitHandler.WaitOne(TIMEOUT);
					}
				}
				if (terminate)
				{
					TryShutdown();
					ForceShutdown();
				}
				else
					Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
			}
			ThreadPool.QueueUserWorkItem(WaitingHandler, waitId);
		}
		private bool RequestConfirmation()
		{
			return MessageBoxService.ShowConfirmation("Другой экземпляр программы уже запущен. Завершить?");
		}
		private void WaitingHandler(object startInfo)
		{
			try
			{
				_signalHandler.WaitOne();
				TryShutdown();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
				ForceShutdown();
			}
			finally
			{
				_waitHandler = new EventWaitHandle(false, EventResetMode.AutoReset, (string)startInfo);
				_waitHandler.Set();
				ForceShutdown();
			}
		}
		private void TryShutdown()
		{
			if (ShuttingDown != null)
				ShuttingDown();

			if (!_force && Application.Current != null)
			{
				Application.Current.Dispatcher.InvokeShutdown();
				ApplicationService.DoEvents();
			}
		}
		private void ForceShutdown()
		{
			if (Application.Current == null || !Application.Current.Dispatcher.HasShutdownFinished)
				Environment.Exit(0);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_signalHandler != null)
			{
				_signalHandler.Close();
				_signalHandler.Dispose();
			}
			if (_waitHandler != null)
			{
				_waitHandler.Close();
				_waitHandler.Dispose();
			}
		}

		#endregion
	}

	public class SingleLaunchActivator : IDisposable
	{
		public string SignalId { get; private set; }
		public string WaitId { get; private set; }

		EventWaitHandle _signalHandler;
		Action ShuttingDown;
		Action Activation;


		public SingleLaunchActivator(string signalId, string waitId, Action shuttingDown = null, Action activation = null)
		{
			SignalId = signalId;
			WaitId = waitId;
			ShuttingDown = shuttingDown;
			Activation = activation;

			bool isNew;
			_signalHandler = new EventWaitHandle(false, EventResetMode.AutoReset, signalId, out isNew);
			if (!isNew)
			{
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				_signalHandler.Set();
				TryShutdown();
				ForceShutdown();
			}
			ThreadPool.QueueUserWorkItem(WaitingHandler, waitId);
		}
		private void WaitingHandler(object startInfo)
		{
			while (true)
			{
				_signalHandler.WaitOne();
				if (Activation != null)
					Activation();
				_signalHandler = new EventWaitHandle(false, EventResetMode.AutoReset, SignalId);
			}
		}
		private void TryShutdown()
		{
			if (ShuttingDown != null)
				ShuttingDown();

			if (Application.Current != null)
			{
				Application.Current.Dispatcher.InvokeShutdown();
				ApplicationService.DoEvents();
			}
		}
		private void ForceShutdown()
		{
			if (Application.Current == null || !Application.Current.Dispatcher.HasShutdownFinished)
				Environment.Exit(0);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_signalHandler != null)
			{
				_signalHandler.Close();
				_signalHandler.Dispose();
			}
		}

		#endregion
	}
}