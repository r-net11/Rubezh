using System.Collections.Generic;
using Common;
using FiresecClient;
using StrazhAPI.Models;

namespace StrazhService.Monitor
{
	public class ClientsManager
	{
		public static IEnumerable<ClientCredentials> GetServerClients()
		{
			var operationResult = FiresecManager.FiresecService.GetClients();
			if (operationResult.HasError)
				Logger.Warn(string.Format("Ошибка получения списка Клиентов Сервера: {0}", operationResult.Error));
			return operationResult.Result;
		}
	}
}