using System;
using System.ServiceModel;
using System.Threading;
using Common;
using FiresecAPI;

namespace FSAgentClient
{
	public partial class FSAgent
	{
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
				Error = "Ошибка при при вызове операции FSAgent." + methodName
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

		void LogException(Exception e, string methodName)
		{
			if (IsDisconnecting)
				return;

			if (e is CommunicationObjectFaultedException)
			{
				Logger.Error("FSAgent.SafeOperationCall CommunicationObjectFaultedException " + e.Message + " " + methodName);
			}
			else if (e is ObjectDisposedException)
			{
				Logger.Error("FSAgent.SafeOperationCall ObjectDisposedException " + e.Message + " " + methodName);
			}
			else if (e is CommunicationException)
			{
				Logger.Error("FSAgent.SafeOperationCall CommunicationException " + e.Message + " " + methodName);
			}
			else if (e is TimeoutException)
			{
				Logger.Error("FSAgent.SafeOperationCall TimeoutException " + e.Message + " " + methodName);
			}
			else
			{
				Logger.Error(e, "FSAgent.SafeOperationCall " + e.Message + " " + methodName);
			}
		}

		bool isConnected = true;

		public static event Action ConnectionLost;
		void OnConnectionLost()
		{
			if (IsDisconnecting)
				return;

			if (isConnected)
			{
				if (ConnectionLost != null)
					ConnectionLost();
				isConnected = false;
			}
		}

		public static event Action ConnectionAppeared;
		void OnConnectionAppeared()
		{
			if (!isConnected)
			{
				if (ConnectionAppeared != null)
					ConnectionAppeared();
				isConnected = true;
			}
		}

		bool Recover()
		{
			if (IsDisconnecting)
				return false;

			Logger.Error("FSAgent.Recover");
			Thread.Sleep(TimeSpan.FromSeconds(1));

			SuspendPoll = true;
			try
			{
				FSAgentFactory.Dispose();
				FSAgentFactory = new FSAgentFactory();
				FSAgentContract = FSAgentFactory.Create(_serverAddress);
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
	}
}