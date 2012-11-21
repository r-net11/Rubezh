using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Common;
using FiresecAPI;
using System.Collections.Generic;
using System.Threading;

namespace FiresecDomain
{
    public partial class NativeFiresecClient
    {
        public static bool IsOperationBuisy = false;
        public static DateTime OperationDateTime;

        OperationResult<T> SafeCall<T>(Func<T> func, string methodName)
        {
			var safeCallResult = (OperationResult<T>)_dispatcher.Invoke
			(
				new Func<OperationResult<T>>
				(
					() =>
					{
						return SafeLoopCall(func, methodName);
					}
				)
			);
			return safeCallResult;
        }

        OperationResult<T> SafeLoopCall<T>(Func<T> f, string methodName)
        {
            var resultData = new OperationResult<T>();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    IsOperationBuisy = true;
                    OperationDateTime = DateTime.Now;

                    var result = f();

                    IsOperationBuisy = false;

                    resultData.Result = result;
                    resultData.HasError = false;
                    resultData.Error = null;
                    return resultData;
                }
                catch (COMException e)
                {
					string exceptionText = "COMException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
                    if (!FiresecExceptionHelper.IsWellKnownComException(e.Message))
                    {
                        Logger.Error(exceptionText);
                    }
					Trace.WriteLine(exceptionText);
                    if (e.Message == "Пользователь не зарегистрировался на сервере")
                    {
                        Logger.Error("NativeFiresecClient.SafeLoopCall Пользователь не зарегистрировался на сервере");
                        DoConnect();
                    }
                    resultData.Result = default(T);
                    resultData.HasError = true;
                    resultData.Error = e.Message;
                    SocketServerHelper.StartIfNotRunning();
                }
                catch (System.Reflection.TargetParameterCountException e)
                {
                    if (!FiresecExceptionHelper.IsWellKnownTargetParameterCountException(e.Message))
                    {
                        string exceptionText = "TargetParameterCountException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
                        Logger.Error(exceptionText);
                    }
                    resultData.Result = default(T);
                    resultData.HasError = true;
                    resultData.Error = e.Message;
                    SocketServerHelper.StartIfNotRunning();
                }
                catch (System.Runtime.InteropServices.InvalidComObjectException e)
                {
                    string exceptionText = "InvalidComObjectException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
                    Logger.Error(exceptionText);
                    resultData.Result = default(T);
                    resultData.HasError = true;
                    resultData.Error = e.Message;
                    SocketServerHelper.StartIfNotRunning();
                }
                catch (Exception e)
                {
                    string exceptionText = "Исключение при вызове NativeFiresecClient.SafeLoopCall попытка " + i.ToString();
                    Logger.Error(e, exceptionText);
                    resultData.Result = default(T);
                    resultData.HasError = true;
                    resultData.Error = e.Message;
                    SocketServerHelper.StartIfNotRunning();
					Trace.WriteLine(e.Message);
                }
                finally
                {
                    IsOperationBuisy = false;
                }
            }
            return resultData;
        }
    }
}