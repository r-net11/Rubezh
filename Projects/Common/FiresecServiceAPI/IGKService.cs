using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using System.ServiceModel;

namespace FiresecAPI
{
	[ServiceContract]
	public interface IGKService
	{
		[OperationContract]
		void GKWriteConfiguration(XDevice device, bool writeFileToGK);

		[OperationContract]
		OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device);

		[OperationContract]
		void GKUpdateFirmware(XDevice device, string fileName);

		[OperationContract]
		bool GKSyncronyseTime(XDevice device);

		[OperationContract]
		string GKGetDeviceInfo(XDevice device);

		[OperationContract]
		OperationResult<int> GKGetJournalItemsCount(XDevice device);

		[OperationContract]
		OperationResult<JournalItem> GKReadJournalItem(XDevice device, int no);

		[OperationContract]
		OperationResult<bool> GKSetSingleParameter(XDevice device);

		[OperationContract]
		OperationResult<bool> GKGetSingleParameter(XDevice device);

		[OperationContract]
		void GKExecuteDeviceCommand(XDevice device, XStateBit stateType);

		[OperationContract]
		void GKReset(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKResetFire1(Guid zoneUid);

		[OperationContract]
		void GKResetFire2(Guid zoneUid);

		[OperationContract]
		void GKSetAutomaticRegime(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKSetManualRegime(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKSetIgnoreRegime(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKTurnOn(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKTurnOnNow(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKTurnOff(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffNow(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKStop(Guid uid, XBaseObjectType objectType);
	}
}