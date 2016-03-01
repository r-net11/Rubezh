using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using RubezhDAL.DataClasses;

namespace RubezhDAL
{
	public static class ConfigurationSynchroniser
	{
		public static OperationResult<bool> Export(ConfigurationExportFilter filter)
		{
			if (!Directory.Exists(filter.Path))
				return OperationResult<bool>.FromError("Папка не существует");
			var devicesResult = new OperationResult<bool>();
			var doorsResult = new OperationResult<bool>();
			var zonesResult = new OperationResult<bool>();
			var gkZonesResult = new OperationResult<bool>();
			if (filter.IsExportDevices)
				devicesResult = Export<ExportDevice, GKDevice>(GKManager.Devices, "Devices.xml", filter.Path);
			if(filter.IsExportDoors)
				doorsResult = Export<ExportDoor, GKDoor>(GKManager.Doors, "Doors.xml", filter.Path);
			if (filter.IsExportZones)
			{
				gkZonesResult = Export<ExportZone, GKSKDZone>(GKManager.SKDZones, "GKZones.xml", filter.Path);
			}
			return DbServiceHelper.ConcatOperationResults(devicesResult, doorsResult, zonesResult);					
		}

		static OperationResult<bool> Export<TExportItem, TConfigItem>(List<TConfigItem> configItems, string fileName, string path)
			where TExportItem : IConfigExportItem<TConfigItem>, new()
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var exportItems = new List<TExportItem>();
				foreach (var item in configItems)
				{
					var exportItem = new TExportItem();
					exportItem.Initialize(item);
					exportItems.Add(exportItem);
				}
				var serializer = new XmlSerializer(typeof(List<TExportItem>));
				using (var fileStream = File.Open(fileName, FileMode.Create))
				{
					serializer.Serialize(fileStream, exportItems);
				}
				File.Move(fileName, Path.Combine(path, fileName));
				return true;
			});
		}
	}

	public class ExportZone : IConfigExportItem<GKSKDZone>
	{
		public Guid UID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Number { get; set; }

		public void Initialize(GKSKDZone configItem)
		{
			UID = configItem.UID;
			Name = configItem.Name;
			Description = configItem.Description;
			Number = configItem.No;
		}
	}


	public class ExportDevice : IConfigExportItem<GKDevice>
	{
		public Guid UID { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public Guid ParentUID { get; set; }

		public void Initialize(GKDevice configItem)
		{
			UID = configItem.UID;
			Name = configItem.Name;
			Address = configItem.Address;
			ParentUID = configItem.Parent != null ? configItem.Parent.UID : Guid.Empty;
		}
	}

	public class ExportDoor : IConfigExportItem<GKDoor>
	{
		public Guid UID { get; set; }
		public int Number { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Type { get; set; }
		public Guid InDeviceUID { get; set; }
		public Guid OutDeviceUID { get; set; }

		public void Initialize(GKDoor configItem)
		{
			UID = configItem.UID;
			Number = configItem.No;
			Name = configItem.Name;
			Description = configItem.Description;
			Type = (int)configItem.DoorType;
			InDeviceUID = configItem.EnterDeviceUID;
			OutDeviceUID = configItem.ExitDeviceUID;
		}
	}

	public interface IConfigExportItem<T>
	{
		void Initialize(T configItem);
	}
}
