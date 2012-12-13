using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MuliclientAPI;
using System.ServiceModel;

namespace MuliclientAPI
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class Muliclient : IMuliclient
	{
		List<IMuliclientCallback> Callbacks = new List<IMuliclientCallback>();

		public void Connect(int clientId)
		{
			var callback = OperationContext.Current.GetCallbackChannel<IMuliclientCallback>();
			Callbacks.Add(callback);
		}

		public void Loaded(int clientId)
		{
		}

		public void Closed(int clientId)
		{
		}

		public void ShowAll()
		{
			foreach (var callback in Callbacks)
			{
				callback.Show();
			}
		}

		public void HideAll()
		{
			foreach (var callback in Callbacks)
			{
				callback.Hide();
			}
		}
	}
}