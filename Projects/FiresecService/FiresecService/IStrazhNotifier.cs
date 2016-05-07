using StrazhAPI.Models;
using System;

namespace StrazhService
{
	public interface IStrazhNotifier
	{
		void UILog(string message);
		void BalloonShowFromServer(string text);
		void AddClient(ClientCredentials clientCredentials);
		void RemoveClient(Guid uid);
		void EditClient(Guid uid, string login);
	}
}
