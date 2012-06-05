using System;
using System.ServiceModel;
using System.Timers;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Diagnostics;

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
			_pingTimer.Elapsed += new ElapsedEventHandler(OnPingTimer);

			_recoveryTimer = new System.Timers.Timer();
			_recoveryTimer.Interval = 10000;
			_recoveryTimer.Elapsed += new ElapsedEventHandler((source, e) => { OnRecover(); });
		}

		public static event Action ConnectionLost;
		void OnConnectionLost()
		{
			if (_isConnected == false)
				return;

			if (ConnectionLost != null)
				ConnectionLost();

			_isConnected = false;

			OnRecover();
		}

		void OnRecover()
		{
			Trace.WriteLine("OnRecover");
			_recoveryTimer.Stop();
			_pingTimer.Stop();
			FiresecServiceFactory.Dispose();
			FiresecServiceFactory = new FiresecClient.FiresecServiceFactory();
			FiresecService = FiresecServiceFactory.Create(_serverAddress);
			try
			{
				FiresecService.Connect(_clientCredentials, false);
				_recoveryTimer.Stop();
			}
			catch
			{
				_recoveryTimer.Start();
			}
			_pingTimer.Start();
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

		private void OnPingTimer(object source, ElapsedEventArgs e)
		{
			Ping();
		}

		public string Ping()
		{
			try
			{
				var result = FiresecService.Ping();
				OnConnectionAppeared();
				return result;
			}
			catch (CommunicationObjectFaultedException)
			{
				OnConnectionLost();
			}
			catch (InvalidOperationException)
			{
				OnConnectionLost();
			}
			catch (CommunicationException)
			{
				OnConnectionLost();
			}
			catch (Exception)
			{
				OnConnectionLost();
			}
			return null;
		}

		OperationResult<T> SafeOperationCall<T>(Func<OperationResult<T>> func)
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
			}
			var operationResult = new OperationResult<T>()
			{
				HasError = true,
				Error = "Ошибка при при вызове операции на клиенте"
			};
			return operationResult;
		}

		T SafeOperationCall<T>(Func<T> func)
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
			}
			return default(T);
		}

		void SafeOperationCall(Action action)
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
			StopPing();
			Disconnect();
			FiresecServiceFactory.Dispose();
		}
	}
}