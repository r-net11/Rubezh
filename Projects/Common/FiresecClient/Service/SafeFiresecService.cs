using System;
using System.ServiceModel;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class SafeFiresecService// : IFiresecService
	{
		FiresecServiceFactory FiresecServiceFactory;
		public IFiresecService FiresecService { get; set; }
		string _serverAddress;
		ClientCredentials _clientCredentials;
		bool IsDisconnecting = false;

		public SafeFiresecService(string serverAddress)
		{
			FiresecServiceFactory = new FiresecClient.FiresecServiceFactory();
			_serverAddress = serverAddress;
			FiresecService = FiresecServiceFactory.Create(serverAddress);

			StartOperationQueueThread();
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
			{
				StopOperationQueueThread();
			};
		}

		OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func, string methodName, bool reconnectOnException = true)
		{
			try
			{
				var result = func();
				OnConnectionAppeared();
				if (result != null)
					return result;
			}
			catch (Exception e)
			{
				LogException(e, methodName);
				OnConnectionLost();
				if (reconnectOnException)
				{
					if (Recover())
						return SafeOperationCall(func, methodName, false);
				}
			}
			var operationResult = new OperationResult<T>()
			{
				HasError = true,
				Error = "Ошибка при при вызове операции"
			};
			return operationResult;
		}

		T SafeOperationCall<T>(Func<T> func, string methodName, bool reconnectOnException = true)
		{
			try
			{
				var t = func();
				OnConnectionAppeared();
				return t;
			}
			catch (ActionNotSupportedException)
			{ }
			catch (Exception e)
			{
				LogException(e, methodName);
				OnConnectionLost();
				if (reconnectOnException)
				{
					if (Recover())
						return SafeOperationCall(func, methodName);
				}
			}
			return default(T);
		}

		void SafeOperationCall(Action action, string methodName, bool reconnectOnException = true)
		{
			try
			{
				action();
				OnConnectionAppeared();
			}
			catch (Exception e)
			{
				LogException(e, methodName);
				OnConnectionLost();
				if (reconnectOnException)
				{
					if (Recover())
						SafeOperationCall(action, methodName, false);
				}
			}
		}

		static void SafeOperationCall(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				Logger.Error(e, "SafeFiresecService.SafeOperationCall");
			}
		}

		void LogException(Exception e, string methodName)
		{
			if (e is CommunicationObjectFaultedException)
			{
				Logger.Error("FiresecClient.SafeOperationCall CommunicationObjectFaultedException " + e.Message + " " + methodName);
			}
			else if (e is ObjectDisposedException)
			{
				Logger.Error("FiresecClient.SafeOperationCall ObjectDisposedException " + e.Message + " " + methodName);
			}
			else if (e is CommunicationException)
			{
				Logger.Error("FiresecClient.SafeOperationCall CommunicationException " + e.Message + " " + methodName);
			}
			else if (e is TimeoutException)
			{
				Logger.Error("FiresecClient.SafeOperationCall TimeoutException " + e.Message + " " + methodName);
			}
			else
			{
				Logger.Error(e, "FiresecClient.SafeOperationCall " + e.Message + " " + methodName);
			}
		}

		public static event Action ConnectionLost;
		void OnConnectionLost()
		{
			if (isConnected == false)
				return;
			if (ConnectionLost != null)
				ConnectionLost();
			isConnected = false;
		}

		public static event Action ConnectionAppeared;
		void OnConnectionAppeared()
		{
			if (isConnected == true)
				return;

			if (ConnectionAppeared != null)
				ConnectionAppeared();

			isConnected = true;
		}

		bool Recover()
		{
			if (IsDisconnecting)
				return false;

			Logger.Error("SafeFiresecService.Recover");

			SuspendPoll = true;
			try
			{
				FiresecServiceFactory.Dispose();
				FiresecServiceFactory = new FiresecClient.FiresecServiceFactory();
				FiresecService = FiresecServiceFactory.Create(_serverAddress);
				FiresecService.Connect(FiresecServiceFactory.UID, _clientCredentials, false);
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				SuspendPoll = false;
			}
		}

		public OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew)
		{
			_clientCredentials = clientCredentials;
			return SafeOperationCall(() =>
			{
				try
				{
					return FiresecService.Connect(uid, clientCredentials, isNew);
				}
				//catch (EndpointNotFoundException) { }
				//catch (System.IO.PipeException) { }
				//catch (SecurityNegotiationException) { }
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове FiresecClient.Connect");
				}
				return new OperationResult<bool>()
				{
					Result = false,
					HasError = true,
					Error = "Не удается соединиться с сервером " + _serverAddress
				};
			}, "Connect");
		}

		public void Dispose()
		{
			IsDisconnecting = true;
			Disconnect(FiresecServiceFactory.UID);
			StopPoll();
			FiresecServiceFactory.Dispose();
		}
	}
}