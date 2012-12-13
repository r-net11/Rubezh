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
        List<CallbackItem> CallbackItems = new List<CallbackItem>();

        public void Connect(string clientId)
		{
            var callbackItem = new CallbackItem();
            callbackItem.ClientId = clientId;
            callbackItem.Callback = OperationContext.Current.GetCallbackChannel<IMuliclientCallback>();
            CallbackItems.Add(callbackItem);
		}

        public void Loaded(string clientId)
		{
		}

        public void Closed(string clientId)
		{
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