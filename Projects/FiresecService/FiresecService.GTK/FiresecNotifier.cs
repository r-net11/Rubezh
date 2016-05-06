using Infrastructure.Common.BalloonTrayTip;
using System;
using FiresecService.View;
using FiresecService.Views;

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
			MainView.Current.OnPoll(clientUID);
		}

		public void AddClient(RubezhAPI.Models.ClientCredentials clientCredentials)
		{
			MainView.Current.AddClient(clientCredentials);
		}

		public void RemoveClient(Guid uid)
		{
			MainView.Current.RemoveClient(uid);
		}

		public void AddServerTask(ServerTask serverTask)
		{
			//MainPresenter.Current.AddTask(serverTask);
		}

		public void RemoveServerTask(ServerTask serverTask)
		{
			//MainPresenter.Current.RemoveTask(serverTask);
		}

		public void SetLocalAddress(string address)
		{
			MainView.Current.SetLocalAddress(address);
		}

		public void SetRemoteAddress(string address)
		{
			MainView.Current.SetRemoteAddress(address);
		}
	}
}
