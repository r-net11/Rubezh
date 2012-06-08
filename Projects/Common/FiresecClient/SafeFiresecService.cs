using System;
using System.ServiceModel;
using System.Timers;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
	public partial class SafeFiresecService : IFiresecService
	{
		FiresecServiceFactory FiresecServiceFactory;
		public IFiresecService FiresecService { get; set; }
		string _serverAddress;
		ClientCredentials _clientCredentials;
		bool _isConnected = true;
		System.Timers.Timer _pingTimer;
		System.Timers.Timer _recoveryTimer;

		public SafeFiresecService(string serverAddress)
		{
			FiresecServiceFactory = new FiresecClient.FiresecServiceFactory();
			_serverAddress = serverAddress;
			FiresecService = FiresecServiceFactory.Create(serverAddress);

			_pingTimer = new System.Timers.Timer();
			_pingTimer.Interval = 1000;
			_pingTimer.Elapsed += new ElapsedEventHandler((source, e) => { Ping(); });

			_recoveryTimer = new System.Timers.Timer();
			_recoveryTimer.Interval = 10000;
			_recoveryTimer.Elapsed += new ElapsedEventHandler((source, e) => { Recover(); });
		}

		public static event Action ConnectionLost;
		void OnConnectionLost()
		{
			if (_isConnected == false)
				return;

			if (ConnectionLost != null)
				ConnectionLost();

			_isConnected = false;
		}

		bool Recover()
		{
			_recoveryTimer.Stop();
			_pingTimer.Stop();
			FiresecServiceFactory.Dispose();
			FiresecServiceFactory = new FiresecClient.FiresecServiceFactory();
			FiresecService = FiresecServiceFactory.Create(_serverAddress);
			try
			{
				FiresecService.Connect(_clientCredentials, false);
				_recoveryTimer.Stop();
				_pingTimer.Start();
				return true;
			}
			catch
			{
				_recoveryTimer.Start();
			}
			return false;
		}

		public static event Action ConnectionAppeared;
		void OnConnectionAppeared()
		{
			if (_isConnected == true)
				return;

			if (ConnectionAppeared != null)
				ConnectionAppeared();

			_isConnected = true;
		}

		public void StartPing()
		{
			_pingTimer.Start();
		}

		public void StopPing()
		{
			_pingTimer.Stop();
			_pingTimer.Dispose();
		}

		public string Ping()
		{
			try
			{
				var result = FiresecService.Ping();
				OnConnectionAppeared();
				return result;
			}
			catch
			{
				OnConnectionLost();
				Recover();
			}
			return null;
		}

		OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func, bool reconnectOnException = true)
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
				Logger.Error(e, "Исключение при вызове FiresecClient.SafeOperationCall<T>(Func<OperationResult<T>> func)");
				OnConnectionLost();
				if (reconnectOnException)
				{
					if (Recover())
						return SafeOperationCall(func, false);
				}
			}
			var operationResult = new OperationResult<T>()
			{
				HasError = true,
				Error = "Ошибка при при вызове операции на клиенте"
			};
			return operationResult;
		}

		T SafeOperationCall<T>(Func<T> func, bool reconnectOnException = true)
		{
			try
			{
				var t = func();
				OnConnectionAppeared();
				return t;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecClient.SafeOperationCall<T>(Func<T> func)");
				OnConnectionLost();
				if (reconnectOnException)
				{
					if (Recover())
						return SafeOperationCall(func, false);
				}
			}
			return default(T);
		}

		void SafeOperationCall(Action action, bool reconnectOnException = true)
		{
			try
			{
				action();
				OnConnectionAppeared();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecClient.SafeOperationCall(Action action)");
				OnConnectionLost();
				if (reconnectOnException)
				{
					if (Recover())
						SafeOperationCall(action, false);
				}
			}
		}

		public OperationResult<bool> Connect(ClientCredentials clientCredentials, bool isNew)
		{
			_clientCredentials = clientCredentials;
			return SafeOperationCall(() =>
			{
				try
				{
					return FiresecService.Connect(clientCredentials, isNew);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове FiresecClient.Connect");
				}
				return new OperationResult<bool>()
				{
					Result = false,
					HasError = true,
					Error = "Не удается соединиться с сервером"
				};
			});
		}

		public void Dispose()
		{
			Logger.Info("SafeFiresecService.Dispose");
			StopPing();
			Disconnect();
			FiresecServiceFactory.Dispose();
		}
	}
}