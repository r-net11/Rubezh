using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using FiresecAPI.Models;
using FiresecAPI;

namespace FSAgentAPI
{
	[ServiceContract]
	public interface IFSAgentContract
	{
		[OperationContract]
		string GetStatus();

		[OperationContract]
        List<FSAgentCallbac> Poll(Guid clientUID);

		[OperationContract]
		void AddToIgnoreList(List<string> devicePaths);

		[OperationContract]
		void RemoveFromIgnoreList(List<string> devicePaths);

		[OperationContract]
		void ResetStates(string states);

		[OperationContract]
		void SetZoneGuard(string placeInTree, string localZoneNo);

		[OperationContract]
		void UnSetZoneGuard(string placeInTree, string localZoneNo);

		[OperationContract]
		void AddUserMessage(string message);

        [OperationContract]
        OperationResult<string> DeviceGetInformation(string coreConfig, string devicePath);
	}
}