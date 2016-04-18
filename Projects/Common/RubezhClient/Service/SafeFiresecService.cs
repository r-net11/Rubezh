using Common;
using Infrastructure.Common.Windows;
using OpcClientSdk;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.License;
using RubezhAPI.Models;
using System;
using System.ServiceModel;
using System.Threading;
using System.Windows.Threading;

namespace RubezhClient
{
	public partial class SafeFiresecService : ISafeFiresecService
	{
		FiresecServiceFactory FiresecServiceFactory;
		string _serverAddress;
		ClientCredentials _clientCredentials;
		bool IsDisconnecting = false;

		public SafeFiresecService(string serverAddress)
		{
			IsConnected = true;
			FiresecServiceFactory = new RubezhClient.FiresecServiceFactory(serverAddress);
			_serverAddress = serverAddress;

			StartOperationQueueThread();
			Dispatcher.CurrentDispatcher.ShutdownStarted += (s, e) =>
			{
				StopOperationQueueThread();
			};
		}

		public Guid UID { get { return FiresecServiceFactory.UID; } }

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
				if (e is TimeoutException)
					ShowTimeoutException(methodName);
			}
			return OperationResult<T>.FromError("Ошибка при вызове операции");
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
				if (e is TimeoutException)
					ShowTimeoutException(methodName);
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
				if (e is TimeoutException)
					ShowTimeoutException(methodName);
				return false;
			}
		}

		void ShowTimeoutException(string methodName)
		{
			new Thread(() =>
				ApplicationService.Invoke(() =>
					MessageBoxService.ShowError("Превышено время ожидания выполнения операции " + methodName))
					).Start();
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
			if (IsConnected == false)
				return;
			if (OnConnectionLost != null)
				OnConnectionLost();
			IsConnected = false;
		}

		public static event Action OnConnectionAppeared;
		void ConnectionAppeared()
		{
			if (IsConnected == true)
				return;

			if (OnConnectionAppeared != null)
				OnConnectionAppeared();

			IsConnected = true;
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

		public OperationResult<OpcDaServer[]> GetOpcDaServers(Guid clientUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetOpcDaServers(clientUID);
			}, "GetOpcDaServerNames");
		}

		public OperationResult<OpcServerStatus> GetOpcDaServerStatus(Guid clientUID, OpcDaServer server)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetOpcDaServerStatus(clientUID, server);
			}, "GetOpcDaServerStatus");
		}

		public OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(Guid clientUID, OpcDaServer server)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetOpcDaServerGroupAndTags(clientUID, server);
			}, "GetOpcDaServerGroupAndTags");
		}

		public OperationResult<OpcDaTagValue[]> ReadOpcDaServerTags(Guid clientUID, OpcDaServer server)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.ReadOpcDaServerTags(clientUID, server);
			}, "ReadOpcDaServerTags");
		}

		public OperationResult<bool> WriteOpcDaServerTag(Guid clientUID, Guid tagId, object value)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.WriteOpcDaTag(clientUID, tagId, value);
			}, "WriteOpcDaServerTag");
		}

		public OperationResult<bool> WriteOpcDaServerTag(Guid tagId, object value)
		{
			return WriteOpcDaServerTag(FiresecServiceFactory.UID, tagId, value);
		}
	}
}