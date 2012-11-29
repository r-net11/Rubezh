using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Common;
using FiresecAPI;
using Infrastructure.Common;

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
            else
            {
                Logger.Error(e, "FiresecClient.SafeOperationCall " + e.Message + " " + methodName);
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

        bool isConnected = true;
        public bool SuspendPoll = false;

        public static event Action ConnectionLost;
        void OnConnectionLost()
        {
            if (isConnected == false)
                return;
            if (ConnectionLost != null)
                ConnectionLost();
            isConnected = false;
			FSAgentLoadHelper.Load();
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
            Logger.Error("SafeFiresecService.Recover");

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