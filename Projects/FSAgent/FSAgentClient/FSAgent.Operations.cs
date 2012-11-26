using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentAPI;
using FiresecAPI.Models;
using FiresecAPI;

namespace FSAgentClient
{
    public partial class FSAgent
    {
        public string GetStatus()
        {
            return SafeOperationCall(() => { return FSAgentContract.GetStatus(); }, "GetStatus");
        }

        public List<FSAgentCallbac> Poll(Guid clientUID)
        {
            return SafeOperationCall(() => { return FSAgentContract.Poll(clientUID); }, "GetChangeResult");
        }

		public void AddToIgnoreList(List<string> devicePaths)
		{
			SafeOperationCall(() => { FSAgentContract.AddToIgnoreList(devicePaths); }, "AddToIgnoreList");
		}

		public void RemoveFromIgnoreList(List<string> devicePaths)
		{
			SafeOperationCall(() => { FSAgentContract.RemoveFromIgnoreList(devicePaths); }, "RemoveFromIgnoreList");
		}

		public void ResetStates(string states)
		{
			SafeOperationCall(() => { FSAgentContract.ResetStates(states); }, "ResetStates");
		}

		public void SetZoneGuard(string placeInTree, string localZoneNo)
		{
			SafeOperationCall(() => { FSAgentContract.SetZoneGuard(placeInTree, localZoneNo); }, "SetZoneGuard");
		}

		public void UnSetZoneGuard(string placeInTree, string localZoneNo)
		{
			SafeOperationCall(() => { FSAgentContract.UnSetZoneGuard(placeInTree, localZoneNo); }, "UnSetZoneGuard");
		}

		public void AddUserMessage(string message)
		{
			SafeOperationCall(() => { FSAgentContract.AddUserMessage(message); }, "AddUserMessage");
		}

        public OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath)
        {
            return SafeOperationCall(() => { return FSAgentContract.DeviceGetInformation(coreConfig, devicePath); }, "DeviceGetInformation");
        }
    }
}