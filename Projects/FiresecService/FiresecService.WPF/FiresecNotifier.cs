using FiresecService.Models;
using Infrastructure.Common.BalloonTrayTip;
using System;

namespace FiresecService
{
	public class FiresecNotifier : IFiresecNotifier
	{
		public void UILog(string message, bool isError = false)
		{
			UILogger.Log(message, isError);
		}

		public void BalloonShowFromServer(string text)
		{
			BalloonHelper.ShowFromServer(text);
		}

		public void OnPoll(Guid clientUID)
		{
			MainViewModel.Current.OnPoll(clientUID);
		}

		public void AddClient(RubezhAPI.Models.ClientCredentials clientCredentials)
		{
			MainViewModel.Current.AddClient(clientCredentials);
		}

		public void RemoveClient(Guid uid)
		{
			MainViewModel.Current.RemoveClient(uid);
		}

		public void AddServerTask(ServerTask serverTask)
		{
			MainViewModel.Current.ServerTasksViewModel.Add(serverTask);
		}

		public void RemoveServerTask(ServerTask serverTask)
		{
			MainViewModel.Current.ServerTasksViewModel.Remove(serverTask);
		}

		public void SetLocalAddress(string address)
		{
			MainViewModel.SetLocalAddress(address);
		}

		public void SetRemoteAddress(string address)
		{
			MainViewModel.SetRemoteAddress(address);
		}
	}
}
