using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FS2Api;
using FiresecAPI;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void AddToIgnoreList(List<Device> devices)
		{
			if (IsFS2Enabled)
			{
				var deviceUIDs = new List<Guid>();
				foreach (var device in devices)
				{
					deviceUIDs.Add(device.UID);
				}
				FS2ClientContract.AddToIgnoreList(deviceUIDs);
			}
			else
			{
				FiresecDriver.AddToIgnoreList(devices);
			}
		}

		public static void RemoveFromIgnoreList(List<Device> devices)
		{
			if (IsFS2Enabled)
			{
				var deviceUIDs = new List<Guid>();
				foreach (var device in devices)
				{
					deviceUIDs.Add(device.UID);
				}
				FS2ClientContract.RemoveFromIgnoreList(deviceUIDs);
			}
			else
			{
				FiresecDriver.RemoveFromIgnoreList(devices);
			}
		}

		public static void ResetStates(List<ResetItem> resetItems)
		{
			if (IsFS2Enabled)
			{
				var paneleResetItems = new List<PanelResetItem>();
				foreach (var resetItem in resetItems)
				{
					var parentPanel = resetItem.DeviceState.Device;
					var panelResetItem = paneleResetItems.FirstOrDefault(x => x.PanelUID == parentPanel.UID);
					if (panelResetItem == null)
					{
						panelResetItem = new PanelResetItem()
						{
							PanelUID = parentPanel.UID
						};
						paneleResetItems.Add(panelResetItem);
					}
					foreach (var deviceDriverState in resetItem.States)
					{
						panelResetItem.Ids.Add(deviceDriverState.DriverState.Code);
					}
				}
				FS2ClientContract.ResetStates(paneleResetItems);
			}
			else
			{
				FiresecDriver.ResetStates(resetItems);
			}
		}

		public static void ExecuteCommand(Device device, string methodName)
		{
			if (IsFS2Enabled)
			{
				FS2ClientContract.ExecuteCommand(device.UID, methodName);
			}
			else
			{
				FiresecDriver.ExecuteCommand(device, methodName);
			}
		}

		public static OperationResult<DateTime> GetArchiveStartDate()
		{
			if (IsFS2Enabled)
			{
				return FiresecManager.FS2ClientContract.GetArchiveStartDate();
			}
			else
			{
				return FiresecManager.FiresecService.GetArchiveStartDate();
			}
		}

		public static void BeginGetFilteredArchive(ArchiveFilter archiveFilter)
		{
			if (IsFS2Enabled)
			{
				FiresecManager.FS2ClientContract.BeginGetFilteredArchive(archiveFilter);
			}
			else
			{
				FiresecManager.FiresecService.BeginGetFilteredArchive(archiveFilter);
			}
		}

		public static OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
		{
			if (IsFS2Enabled)
			{
				return FiresecManager.FS2ClientContract.GetDistinctDescriptions();
			}
			else
			{
				return FiresecManager.FiresecService.GetDistinctDescriptions();
			}
		}
	}
}