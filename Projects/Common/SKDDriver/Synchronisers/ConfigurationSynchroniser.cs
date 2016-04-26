using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SKDDriver
{
	public static class ConfigurationSynchroniser
	{
		public static OperationResult Export(ConfigurationExportFilter filter)
		{
			if (!Directory.Exists(filter.Path))
				return new OperationResult(Resources.Language.ConfigurationSynchroniser.Export_Error);
			var devicesResult = new OperationResult();
			var doorsResult = new OperationResult();
			var zonesResult = new OperationResult();
			if (filter.IsExportDevices)
				devicesResult = Export<ExportDevice, SKDDevice>(SKDManager.Devices, "Devices.xml", filter.Path);
			if (filter.IsExportDoors)
				doorsResult = Export<ExportDoor, SKDDoor>(SKDManager.Doors, "Doors.xml", filter.Path);
			if (filter.IsExportZones)
			{
				zonesResult = Export<ExportZone, SKDZone>(SKDManager.Zones, "StrazhZones.xml", filter.Path);
			}
			return TranslatiorHelper.ConcatOperationResults(devicesResult, doorsResult, zonesResult);
		}

		private static OperationResult Export<TExportItem, TConfigItem>(List<TConfigItem> configItems, string fileName, string path)
			where TExportItem : IConfigExportItem<TConfigItem>, new()
		{
			try
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
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}
	}

	public class ExportZone : IConfigExportItem<SKDZone>
	{
		public Guid UID { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public int Number { get; set; }

		public void Initialize(SKDZone configItem)
		{
			UID = configItem.UID;
			Name = configItem.Name;
			Description = configItem.Description;
			Number = configItem.No;
		}
	}

	public class ExportDevice : IConfigExportItem<SKDDevice>
	{
		public Guid UID { get; set; }

		public string Name { get; set; }

		public string Address { get; set; }

		public Guid ParentUID { get; set; }

		public void Initialize(SKDDevice configItem)
		{
			UID = configItem.UID;
			Name = configItem.Name;
			Address = configItem.Address;
			ParentUID = configItem.Parent != null ? configItem.Parent.UID : Guid.Empty;
		}
	}

	public class ExportDoor : IConfigExportItem<SKDDoor>
	{
		public Guid UID { get; set; }

		public int Number { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public int Type { get; set; }

		public Guid InDeviceUID { get; set; }

		public Guid OutDeviceUID { get; set; }

		public void Initialize(SKDDoor configItem)
		{
			UID = configItem.UID;
			Number = configItem.No;
			Name = configItem.Name;
			Description = configItem.Description;
			Type = (int)configItem.DoorType;
			InDeviceUID = configItem.InDeviceUID;
			OutDeviceUID = configItem.OutDeviceUID;
		}
	}

	public interface IConfigExportItem<T>
	{
		void Initialize(T configItem);
	}
}