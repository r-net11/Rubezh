using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using System.ServiceModel;
using FiresecAPI.Models;
using System.Threading;
using Common;
using FiresecAPI;

namespace FSAgentServer
{
    [ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = true,
    InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class FSAgentContract : IFSAgentContract
    {
        public string GetStatus()
        {
            return null;
        }

        public List<FSAgentCallbac> Poll(Guid clientUID)
        {
            ClientsManager.Add(clientUID);

            var clientInfo = ClientsManager.ClientInfos.FirstOrDefault(x => x.UID == clientUID);
            if (clientInfo != null)
            {
                var result = CallbackManager.Get(clientInfo.CallbackIndex);
                clientInfo.CallbackIndex = CallbackManager.LastIndex;
                return result;
            }
            return new List<FSAgentCallbac>();
        }

		public void AddToIgnoreList(List<string> devicePaths)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.FiresecSerializedClient.AddToIgnoreList(devicePaths);
			});
		}
		public void RemoveFromIgnoreList(List<string> devicePaths)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.FiresecSerializedClient.RemoveFromIgnoreList(devicePaths);
			});
		}
		public void ResetStates(string states)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.FiresecSerializedClient.ResetStates(states);
			});
		}
		public void SetZoneGuard(string placeInTree, string localZoneNo)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.FiresecSerializedClient.SetZoneGuard(placeInTree, localZoneNo);
			});
		}
		public void UnSetZoneGuard(string placeInTree, string localZoneNo)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.FiresecSerializedClient.UnSetZoneGuard(placeInTree, localZoneNo);
			});
		}
		public void AddUserMessage(string message)
		{
			WatcherManager.Current.AddTask(() =>
			{
				WatcherManager.Current.FiresecSerializedClient.AddUserMessage(message);
			});
		}

        public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
        {
            return (OperationResult<string>)WatcherManager.Current.Invoke(new Func<object>(() =>
                {
                    return WatcherManager.Current.FiresecSerializedClient.NativeFiresecClient.DeviceGetInformation(coreConfig, devicePath);
                }));
        }
    }
}