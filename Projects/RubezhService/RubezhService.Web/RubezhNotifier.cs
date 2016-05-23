
using RubezhAPI.Models;
using RubezhService.Models;
using System;

namespace RubezhService
{
	public class RubezhNotifier : IRubezhNotifier
	{
		public void UILog(string message, bool isError = false)
		{
			LogModel.AddLog(message, isError);
		}

		public void BalloonShowFromServer(string text) { }

		public void OnPoll(Guid clientUID)
		{
			PollingModel.AddOrUpdate(clientUID);
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			ConnectionsModel.AddConnection(clientCredentials);
		}

		public void RemoveClient(Guid uid)
		{
			ConnectionsModel.RemoveConnection(uid);
		}

		public void AddServerTask(ServerTask serverTask)
		{
			OperationsModel.AddServerTask(serverTask);
		}

		public void RemoveServerTask(ServerTask serverTask)
		{
			OperationsModel.RemoveServerTask(serverTask);
		}

		public void SetLocalAddress(string address)
		{
			StatusModel.SetLocalAddress(address);
		}

		public void SetRemoteAddress(string address)
		{
			StatusModel.SetRemoteAddress(address);
		}
	}
}
