
using RubezhAPI.Models;
using System;

namespace RubezhService
{
	public class RubezhNotifier : IRubezhNotifier
	{
		public void UILog(string message, bool isError = false)
		{
			LogPresenter.AddLog(message, isError);
		}

		public void BalloonShowFromServer(string text) { }

		public void OnPoll(Guid clientUID)
		{
			PollingPresenter.AddOrUpdate(clientUID);
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			ConnectionsPresenter.AddConnection(clientCredentials);
		}

		public void RemoveClient(Guid uid)
		{
			ConnectionsPresenter.RemoveConnection(uid);
		}

		public void AddServerTask(ServerTask serverTask)
		{
			OperationsPresenter.AddServerTask(serverTask);
		}

		public void RemoveServerTask(ServerTask serverTask)
		{
			OperationsPresenter.RemoveServerTask(serverTask);
		}

		public void SetLocalAddress(string address)
		{
			StatusPresenter.SetLocalAddress(address);
		}

		public void SetRemoteAddress(string address)
		{
			StatusPresenter.SetRemoteAddress(address);
		}
	}
}
