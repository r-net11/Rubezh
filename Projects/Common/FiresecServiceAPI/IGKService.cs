using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace FiresecAPI
{
	public interface IGKService
	{
		void GKWriteConfiguration(XDevice device);
		OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device);
		void GKUpdateFirmware(XDevice device, string fileName);
		bool GKSyncronyseTime(XDevice device);
		string GKGetDeviceInfo(XDevice device);
		OperationResult<int> GKGetJournalItemsCount(XDevice device);
		OperationResult<JournalItem> GKReadJournalItem(XDevice device, int no);
		OperationResult<bool> GKSetSingleParameter(XDevice device);
		OperationResult<bool> GKGetSingleParameter(XDevice device);

		void GKExecuteDeviceCommand(XDevice device, XStateBit stateType);
		void GKReset(Guid uid, XBaseObjectType objectType);
		void GKResetFire1(Guid zoneUid);
		void GKResetFire2(Guid zoneUid);
		void GKSetAutomaticRegime(Guid uid, XBaseObjectType objectType);
		void GKSetManualRegime(Guid uid, XBaseObjectType objectType);
		void GKSetIgnoreRegime(Guid uid, XBaseObjectType objectType);
		void GKTurnOn(Guid uid, XBaseObjectType objectType);
		void GKTurnOnNow(Guid uid, XBaseObjectType objectType);
		void GKTurnOff(Guid uid, XBaseObjectType objectType);
		void GKTurnOffNow(Guid uid, XBaseObjectType objectType);
		void GKStop(Guid uid, XBaseObjectType objectType);
	}
}