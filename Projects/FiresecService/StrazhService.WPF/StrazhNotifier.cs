using Infrastructure.Common.BalloonTrayTip;
using System;
using StrazhService.ViewModels;

namespace StrazhService.Starter
{
	public class StrazhNotifier : IStrazhNotifier
	{
		public void UILog(string message)
		{
			UILogger.Log(message);
		}

		public void BalloonShowFromServer(string text)
		{
			BalloonHelper.ShowFromServer(text);
		}

		public void AddClient(StrazhAPI.Models.ClientCredentials clientCredentials)
		{
			MainViewModel.Current.AddClient(clientCredentials);
		}

		public void RemoveClient(Guid uid)
		{
			MainViewModel.Current.RemoveClient(uid);
		}

		public void EditClient(Guid uid, string login)
		{
			MainViewModel.Current.EditClient(uid, login);
		}
	}
}
