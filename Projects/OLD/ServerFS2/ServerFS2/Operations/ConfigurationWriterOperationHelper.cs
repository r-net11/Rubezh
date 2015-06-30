using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Service;

namespace ServerFS2
{
	public static class ConfigurationWriterOperationHelper
	{
		public static bool Write(Device device)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Формирование базы данных устройств"));
			var systemDatabaseCreator = new SystemDatabaseCreator();
			systemDatabaseCreator.Create(0x3000);
			Device parentPanel;
			if ((device.Driver.DriverType == DriverType.IndicationBlock) || (device.Driver.DriverType == DriverType.PDU) || (device.Driver.DriverType == DriverType.PDU_PT))
			{
				var nonPanelDatabase = systemDatabaseCreator.NonPanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == device.UID);
				if (nonPanelDatabase == null)
					throw new FS2Exception("Не найдена сформированная для устройства база данных");
				parentPanel = nonPanelDatabase.ParentPanel;
				var bytes = nonPanelDatabase.BytesDatabase.GetBytes();
				CallbackManager.AddProgress(new FS2ProgressInfo("Запись базы данных в прибор"));
				return SetConfigurationOperationHelper.WriteNonPanelDeviceConfiguration(parentPanel, bytes);
			}
			var panelDatabase = systemDatabaseCreator.PanelDatabases.FirstOrDefault(x => x.ParentPanel.UID == device.UID);
			if (panelDatabase == null)
				throw new FS2Exception("Не найдена сформированная для устройства база данных");
			parentPanel = panelDatabase.ParentPanel;
			var bytes1 = panelDatabase.RomDatabase.BytesDatabase.GetBytes();
			var bytes2 = panelDatabase.FlashDatabase.BytesDatabase.GetBytes();
			CallbackManager.AddProgress(new FS2ProgressInfo("Запись базы данных в прибор"));
			return SetConfigurationOperationHelper.WriteDeviceConfiguration(parentPanel, bytes2, bytes1);
		}
		public static List<Guid> WriteAll(List<Guid> deviceUIDs)
		{
			var failedDevices = new List<Guid>();
			CallbackManager.AddProgress(new FS2ProgressInfo("Формирование базы данных устройств"));
			var systemDatabaseCreator = new SystemDatabaseCreator();
			systemDatabaseCreator.Create(0x3000);

			foreach (var panelDatabase in systemDatabaseCreator.PanelDatabases)
			{
				var parentPanel = panelDatabase.ParentPanel;
				if (deviceUIDs == null || deviceUIDs.Contains(parentPanel.UID))
				{
					var bytes1 = panelDatabase.RomDatabase.BytesDatabase.GetBytes();
					var bytes2 = panelDatabase.FlashDatabase.BytesDatabase.GetBytes();
					CallbackManager.AddProgress(new FS2ProgressInfo("Запись базы данных в прибор " + panelDatabase.ParentPanel.PresentationAddressAndName));
					var result = SetConfigurationOperationHelper.WriteDeviceConfiguration(parentPanel, bytes2, bytes1);
					if (!result)
					{
						failedDevices.Add(parentPanel.UID);
					}
				}
			}

			return failedDevices;
		}
	}
}