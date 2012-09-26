using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Collections.Generic;

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
                LogException(e);
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
                Error = "Ошибка при при вызове операции"
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
                LogException(e);
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
                LogException(e);
                OnConnectionLost();
                if (reconnectOnException)
                {
                    if (Recover())
                        SafeOperationCall(action, false);
                }
            }
        }

        void LogException(Exception e)
        {
            if (e is CommunicationObjectFaultedException)
            {
                Logger.Error("Исключение при вызове FiresecClient.SafeOperationCall CommunicationObjectFaultedException");
            }
            else
            {
                Logger.Error(e, "Исключение при вызове FiresecClient.SafeOperationCall");
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
            });
        }

        public void Dispose()
        {
            Disconnect();
            StopPoll();
            FiresecServiceFactory.Dispose();
        }

        public IAsyncResult BeginPoll(int index, DateTime dateTime, AsyncCallback asyncCallback, object state)
        {
            try
            {
                return FiresecService.BeginPoll(index, dateTime, asyncCallback, state);
            }
            catch
            {
                return null;
            }
        }
        public List<CallbackResult> EndPoll(IAsyncResult asyncResult)
        {
            try
            {
                return FiresecService.EndPoll(asyncResult);
            }
            catch
            {
                return null;
            }
        }
    }
}