using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MuliclientAPI;
using System.ServiceModel;

namespace MultiClientRunner
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
	public class Muliclient : IMuliclient
	{
		List<CallbackItem> CallbackItems = new List<CallbackItem>();

		public void Connect(string clientId)
		{
			var callbackItem = new CallbackItem();
			callbackItem.ClientId = clientId;
			callbackItem.Callback = OperationContext.Current.GetCallbackChannel<IMuliclientCallback>();
			CallbackItems.Add(callbackItem);
		}

		public void SetStatus(string clientId, string status)
		{

		}

		public List<MulticlientData> GetMulticlientData()
		{
			return MulticlientServer.MulticlientDatas;
		}

		public void Activate(string oldClientId, string newClientId, WindowSize windowSize)
		{
			var newCallbackItem = CallbackItems.FirstOrDefault(x => x.ClientId == newClientId);
			if (newCallbackItem != null)
			{
				newCallbackItem.Callback.SetWindowSize(windowSize);
				newCallbackItem.Callback.Show();
			}

			//foreach (var callbackItem in CallbackItems)
			//{
			//    if (callbackItem.ClientId != oldClientId)
			//    {
			//        callbackItem.Callback.Hide();
			//    }
			//}
		}

		public void Show(string clientId)
		{
			var callbackItem = CallbackItems.FirstOrDefault(x => x.ClientId == clientId);
			if (callbackItem != null)
			{
				callbackItem.Callback.Show();
			}
		}

		public void Hide(string clientId)
		{
			var callbackItem = CallbackItems.FirstOrDefault(x => x.ClientId == clientId);
			if (callbackItem != null)
			{
				callbackItem.Callback.Hide();
			}
		}

		public WindowSize GetWindowSize(string clientId)
		{
			var callbackItem = CallbackItems.FirstOrDefault(x => x.ClientId == clientId);
			if (callbackItem != null)
			{
				return callbackItem.Callback.GetWindowSize();
			}
			return null;
		}

		public void SetWindowSize(string clientId, WindowSize windowSize)
		{
			var callbackItem = CallbackItems.FirstOrDefault(x => x.ClientId == clientId);
			if (callbackItem != null)
			{
				callbackItem.Callback.SetWindowSize(windowSize);
			}
		}
	}

	public class CallbackItem
	{
		public IMuliclientCallback Callback { get; set; }
		public string ClientId { get; set; }
	}
}