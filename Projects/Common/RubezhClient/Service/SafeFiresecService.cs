using Common;
using RubezhAPI;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.ServiceModel;
using System.Windows.Threading;

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
			FiresecServiceFactory = new RubezhClient.FiresecServiceFactory(serverAddress);
			_serverAddress = serverAddress;

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
				ConnectionAppeared();
				if (result != null)
					return result;
			}
			catch (Exception e)
			{
				LogException(e, methodName);
				ConnectionLost();
			}
			return OperationResult<T>.FromError("Ошибка при при вызове операции");
		}

		T SafeOperationCall<T>(Func<T> func, string methodName)
		{
			try
			{
				var t = func();
				ConnectionAppeared();
				return t;
			}
			catch (ActionNotSupportedException)
			{ }
			catch (Exception e)
			{
				LogException(e, methodName);
				ConnectionLost();
			}
			return default(T);
		}

		bool SafeOperationCall(Action action, string methodName)
		{
			try
			{
				action();
				ConnectionAppeared();
				return true;
			}
			catch (Exception e)
			{
				LogException(e, methodName);
				ConnectionLost();
				return false;
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

		public static event Action OnConnectionLost;
		void ConnectionLost()
		{
			if (isConnected == false)
				return;
			if (OnConnectionLost != null)
				OnConnectionLost();
			isConnected = false;
		}

		public static event Action OnConnectionAppeared;
		void ConnectionAppeared()
		{
			if (isConnected == true)
				return;

			if (OnConnectionAppeared != null)
				OnConnectionAppeared();

			isConnected = true;
		}

		public OperationResult<bool> Connect(ClientCredentials clientCredentials)
		{
			_clientCredentials = clientCredentials;
			return SafeOperationCall(() =>
			{
				try
				{
					var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
					using (firesecService as IDisposable)
						return firesecService.Connect(clientCredentials);
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

		public bool LayoutChanged(Guid clientUID, Guid layoutUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.LayoutChanged(clientUID, layoutUID);
			}, "LayoutChanged");
		}
		public OperationResult<FiresecLicenseInfo> GetLicenseInfo()
		{
			return SafeOperationCall(() =>
			{
				try
				{
					var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
					using (firesecService as IDisposable)
						return firesecService.GetLicenseInfo(FiresecServiceFactory.UID);
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