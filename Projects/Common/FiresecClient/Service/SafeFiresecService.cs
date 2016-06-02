using System;
using System.ServiceModel;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Enums;
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
					Logger.Error("Исключение при вызове FiresecClient.Connect " + e.GetType().Name.ToString());
				}
				return OperationResult<bool>.FromError("Не удается соединиться с сервером " + _serverAddress, false);
			}, "Connect");
		}

		public void Dispose()
		{
			IsDisconnecting = true;
			Disconnect(FiresecServiceFactory.UID);
			StopPoll();
			FiresecServiceFactory.Dispose();
		}

		/// <summary>
		/// Проверяет доступность СУБД MS SQL Server
		/// </summary>
		/// <param name="ipAddress">IP-адрес сервера СУБД</param>
		/// <param name="ipPort">IP-порт сервера СУБД</param>
		/// <param name="instanceName">Название именованной установки сервера СУБД</param>
		/// <param name="useIntegratedSecurity">Метод аутентификации</param>
		/// <param name="userID">Логин (только для SQL Server аутентификации)</param>
		/// <param name="userPwd">Пароль (только для SQL Server аутентификации)</param>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		public OperationResult<bool> CheckSqlServerConnection(string ipAddress, int ipPort, string instanceName,
			bool useIntegratedSecurity, string userID, string userPwd)
		{
			return SafeOperationCall(() => { return FiresecService.CheckSqlServerConnection(ipAddress, ipPort, instanceName, useIntegratedSecurity, userID, userPwd); }, "CheckSqlServerConnection");
		}

		#region Licensing
		//public OperationResult<bool> CheckLicenseExising()
		//{
		//	return SafeOperationCall(() => FiresecService.CheckLicenseExising(), "CheckLicenseExising");
		//}

		//public OperationResult<bool> CanConnect()
		//{
		//	return SafeOperationCall(() => FiresecService.CanConnect(), "CanConnect");
		//}

		//public OperationResult<bool> CanLoadModule(ModuleType type)
		//{
		//	return SafeOperationCall(() => FiresecService.CanLoadModule(type), "CanLoadModule");
		//}

		/// <summary>
		/// Получает данные лицензии с Сервера
		/// </summary>
		/// <returns>Объект OperationResult с результатом выполнения операции</returns>
		//public OperationResult<LicenseData> GetLicenseData()
		//{
		//	return SafeOperationCall(() => FiresecService.GetLicenseData(), "GetLicenseData");
		//}

		#endregion

	}
}