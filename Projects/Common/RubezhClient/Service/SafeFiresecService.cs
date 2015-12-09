using System;
using System.ServiceModel;
using System.Windows.Threading;
using Common;
using RubezhAPI;
using RubezhAPI.Models;
using Infrastructure.Common;
using RubezhAPI.License;

namespace RubezhClient
{
	public partial class SafeFiresecService : ISafeFiresecService
	{
		FiresecServiceFactory FiresecServiceFactory;
        public IFiresecService FiresecService { get; set; }
		string _serverAddress;
		ClientCredentials _clientCredentials;
		bool IsDisconnecting = false;

		public SafeFiresecService(string serverAddress)
		{
			FiresecServiceFactory = new RubezhClient.FiresecServiceFactory();
			_serverAddress = serverAddress;
			FiresecService = FiresecServiceFactory.Create(serverAddress);

			StartOperationQueueThread();
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
			{
				StopOperationQueueThread();
			};
		}

		OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func, string methodName)
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
                if (!Recover())
                {
                    FiresecServiceFactory.Dispose();
                }
			}
			return OperationResult<T>.FromError("Ошибка при при вызове операции");
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
				if (reconnectOnException && !Recover())
				{
                    FiresecServiceFactory.Dispose();
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
				if (reconnectOnException && !Recover())
				{
                    FiresecServiceFactory.Dispose();
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
				Logger.Error("RubezhClient.SafeOperationCall CommunicationObjectFaultedException " + e.Message + " " + methodName);
			}
			else if (e is ObjectDisposedException)
			{
				Logger.Error("RubezhClient.SafeOperationCall ObjectDisposedException " + e.Message + " " + methodName);
			}
			else if (e is CommunicationException)
			{
				Logger.Error("RubezhClient.SafeOperationCall CommunicationException " + e.Message + " " + methodName);
			}
			else if (e is TimeoutException)
			{
				Logger.Error("RubezhClient.SafeOperationCall TimeoutException " + e.Message + " " + methodName);
			}
			else
			{
				Logger.Error(e, "RubezhClient.SafeOperationCall " + e.Message + " " + methodName);
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

        public static event Action<string> ReconnectionErrorEvent;
		bool Recover()
		{
			if (IsDisconnecting)
				return false;

			Logger.Error("SafeFiresecService.Recover");

			SuspendPoll = true;
			try
			{
				FiresecServiceFactory.Dispose();
				FiresecServiceFactory = new RubezhClient.FiresecServiceFactory();
				FiresecService = FiresecServiceFactory.Create(_serverAddress);
                OperationResult<bool> operationResult = null;
				TimeoutOperation.Execute(() => operationResult = FiresecService.Connect(FiresecServiceFactory.UID, _clientCredentials, false), TimeSpan.FromSeconds(30));
				if (operationResult != null && operationResult.HasError && ReconnectionErrorEvent != null)
                    ReconnectionErrorEvent(operationResult.Error);
				return operationResult.Result;
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
					Logger.Error("Исключение при вызове RubezhClient.Connect " + e.GetType().Name.ToString());
				}
				return OperationResult<bool>.FromError("Не удается соединиться с сервером " + _serverAddress, false);
			}, "Connect");
		}

        public OperationResult<FiresecLicenseInfo> GetLicenseInfo()
        {
            return SafeOperationCall(() =>
            {
                try
                {
                    return FiresecService.GetLicenseInfo();
                }
                catch (Exception e)
                {
					Logger.Error("Исключение при вызове RubezhClient.GetLicenseInfo " + e.GetType().Name.ToString());
                }
                return OperationResult<FiresecLicenseInfo>.FromError("Не удается получить лицензию от " + _serverAddress);
            }, "GetLicenseInfo");
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