using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using System.Diagnostics;

namespace Firesec
{
    public partial class NativeFiresecClient
    {
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
			CheckForRead();
			return safeCallResult;
        }

        public static int OperationNo = 0;
        public static bool IsOperationBuisy = false;
        public static DateTime OperationDateTime;

        OperationResult<T> SafeLoopCall<T>(Func<T> f, string methodName)
        {
            var resultData = new OperationResult<T>();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    OperationNo++;
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
                    if (!FiresecExceptionHelper.IsWellKnownComException(e.Message))
                    {
                        string exceptionText = "COMException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
                        Logger.Error(exceptionText);
                    }
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
                    //if (!FiresecExceptionHelper.IsWellKnownInvalidComObjectException(e.Message))
                    //{
                    string exceptionText = "InvalidComObjectException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
                    Logger.Error(exceptionText);
                    //}
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