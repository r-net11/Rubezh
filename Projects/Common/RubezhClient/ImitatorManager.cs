using System;
using Common;

namespace RubezhClient
{
	public class ImitatorManager
	{
		public static ImitatorService ImitatorService { get; internal set; }
		public static string Connect(string imitatorAddress)
		{
			try
			{
				string message = null;
				for (int i = 0; i < 3; i++)
				{
					ImitatorService = new ImitatorService(imitatorAddress);
					var operationResult = ImitatorService.TestImitator();
					if (!operationResult.HasError)
					{
						message = operationResult.Result;
						break;
					}
					message = operationResult.Error;
				}
				return message;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ImitatorManager.Connect");
				return e.Message;
			}
		}
	}
}
