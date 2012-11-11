using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Security;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Windows.Threading;

namespace FiresecClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public partial class SafeFiresecService : IFiresecService
    {
        FiresecServiceFactory FiresecServiceFactory;
        public IFiresecService FiresecService { get; set; }
        string _serverAddress;
        ClientCredentials _clientCredentials;

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

        void LogException(Exception e, string methodName)
        {
            if (e is CommunicationObjectFaultedException)
            {
                Logger.Error("FiresecClient.SafeOperationCall CommunicationObjectFaultedException " + methodName);
            }
            else
            {
                Logger.Error(e, "FiresecClient.SafeOperationCall " + methodName);
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
                catch (EndpointNotFoundException) { }
                catch (System.IO.PipeException) { }
                catch (SecurityNegotiationException) { }
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
            }, "Connect");
        }

        public void Dispose()
        {
            Disconnect();
            StopPoll();
            FiresecServiceFactory.Dispose();
        }

        public IAsyncResult BeginPoll(int index, DateTime dateTime, AsyncCallback asyncCallback, object state)
        {
			return SafeOperationCall(() => { return FiresecService.BeginPoll(index, dateTime, asyncCallback, state); }, "BeginPoll");
        }
        public List<CallbackResult> EndPoll(IAsyncResult asyncResult)
        {
			return SafeOperationCall(() => { return FiresecService.EndPoll(asyncResult); }, "EndPoll");
        }
        public List<CallbackResult> ShortPoll()
        {
			return SafeOperationCall(() => { return FiresecService.ShortPoll(); }, "ShortPoll");
        }
    }
}