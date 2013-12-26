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
		void AddJournalItem(JournalItem journalItem);

		[OperationContract]
		void CancelGKProgress();

		[OperationContract]
		OperationResult<bool> GKWriteConfiguration(Guid deviceUID, bool writeFileToGK);

		[OperationContract]
		OperationResult<XDeviceConfiguration> GKReadConfiguration(Guid deviceUID);

		[OperationContract]
        OperationResult<bool> GKUpdateFirmware(Guid deviceUID, string fileName);

        [OperationContract]
        OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<XDevice> devices);
        
		[OperationContract]
		bool GKSyncronyseTime(Guid deviceUID);

		[OperationContract]
		string GKGetDeviceInfo(Guid deviceUID);

		[OperationContract]
		OperationResult<int> GKGetJournalItemsCount(Guid deviceUID);

		[OperationContract]
		OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no);

		[OperationContract]
		OperationResult<bool> GKSetSingleParameter(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> GKGetSingleParameter(Guid deviceUID);

		[OperationContract]
		GKStates GKGetStates();

		[OperationContract]
		void GKExecuteDeviceCommand(Guid deviceUID, XStateBit stateBit);

		[OperationContract]
		void GKReset(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKResetFire1(Guid zoneUID);

		[OperationContract]
		void GKResetFire2(Guid zoneUID);

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

		[OperationContract]
		void GKStartMeasureMonitoring(Guid deviceUID);

		[OperationContract]
		void GKStopMeasureMonitoring(Guid deviceUID);
	}
}