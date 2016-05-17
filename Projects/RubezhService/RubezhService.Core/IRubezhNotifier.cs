using RubezhAPI.Models;
using System;

namespace RubezhService
{
	public interface IRubezhNotifier
	{
		void UILog(string message, bool isError = false);
		void BalloonShowFromServer(string text);
		void OnPoll(Guid clientUID);
		void AddClient(ClientCredentials clientCredentials);
		void RemoveClient(Guid uid);
		void AddServerTask(ServerTask serverTask);
		void RemoveServerTask(ServerTask serverTask);
		void SetLocalAddress(string address);
		void SetRemoteAddress(string address);
	}
}
