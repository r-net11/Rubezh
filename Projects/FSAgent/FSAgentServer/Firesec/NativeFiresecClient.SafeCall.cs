using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Common;
using FiresecAPI;
using System.Collections.Generic;
using System.Threading;

namespace FSAgentServer
{
	public partial class NativeFiresecClient
	{
		OperationResult<T> SafeCall<T>(Func<T> func, string methodName)
		{
			var resultData = new OperationResult<T>();
			for (int i = 0; i < 3; i++)
			{
				try
				{
					var result = func();

					resultData.Result = result;
					resultData.HasError = false;
					resultData.Error = null;
					return resultData;
				}
				catch (COMException e)
				{
					if (FiresecExceptionHelper.IsWellKnownNormalException(e.Message))
					{
						resultData.Result = default(T);
						resultData.HasError = true;
						resultData.Error = e.Message;
						return resultData;
					}
					string exceptionText = "COMException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
					if (!FiresecExceptionHelper.IsWellKnownComException(e.Message))
					{
						Logger.Error(exceptionText);
					}
					Trace.WriteLine(exceptionText);
					if (e.Message == "Пользователь не зарегистрировался на сервере")
					{
						Logger.Error("NativeFiresecClient.SafeLoopCall Пользователь не зарегистрировался на сервере");
						Connect();
					}
					resultData.Result = default(T);
					resultData.HasError = true;
					resultData.Error = e.Message;
					SocketServerHelper.Restart();
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
					SocketServerHelper.Restart();
				}
				catch (System.Runtime.InteropServices.InvalidComObjectException e)
				{
					string exceptionText = "InvalidComObjectException " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
					Logger.Error(exceptionText);
					resultData.Result = default(T);
					resultData.HasError = true;
					resultData.Error = e.Message;
					SocketServerHelper.Restart();
				}
				catch (Exception e)
				{
                    string exceptionText = "Exception " + e.Message + " при вызове " + methodName + " попытка " + i.ToString();
					Logger.Error(e, exceptionText);
					resultData.Result = default(T);
					resultData.HasError = true;
					resultData.Error = e.Message;
					SocketServerHelper.Restart();
					Trace.WriteLine(e.Message);
				}
			}
			return resultData;
		}
	}
}