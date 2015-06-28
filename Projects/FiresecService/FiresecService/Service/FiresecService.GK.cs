using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecClient;
using GKProcessor;
using SKDDriver;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		string UserName
		{
			get
			{
				if (CurrentClientCredentials != null)
					return CurrentClientCredentials.FriendlyUserName;
				return "<Нет>";
			}
		}

		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			ChinaSKDDriver.Processor.CancelProgress(progressCallbackUID, userName);
			GKProcessorManager.CancelGKProgress(progressCallbackUID, userName);
		}


		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes)
		{
			return OperationResult<bool>.FromError("Не найден компонент в конфигурации");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID)
		{
			return OperationResult<List<GKProperty>>.FromError("Не найден компонент в конфигурации");
		}


		//public void GKOpenSKDZone(Guid zoneUID) //TODO: Change to SKD
		//{
		//	var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == zoneUID);
		//	if (zone != null)
		//	{
		//		GKProcessorManager.GKOpenSKDZone(zone);
		//	}
		//}

		//public void GKCloseSKDZone(Guid zoneUID)
		//{
		//	var zone = GKManager.SKDZones.FirstOrDefault(x => x.UID == zoneUID);
		//	if (zone != null)
		//	{
		//		GKProcessorManager.GKCloseSKDZone(zone);
		//	}
		//}

		#region Users
		//public OperationResult<List<GKUser>> GKGetUsers(Guid gkDeviceUID)
		//{
		//	var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
		//	if (gkControllerDevice != null)
		//	{
		//		try
		//		{
		//			return GKSKDHelper.GetAllUsers(gkControllerDevice);
		//		}
		//		catch (Exception e)
		//		{
		//			return OperationResult<List<GKUser>>.FromError(e.Message);
		//		}
		//	}
		//	return OperationResult<List<GKUser>>.FromError("Не найден ГК в конфигурации");
		//}
		#endregion
	}
}