
using System;
using FiresecAPI.SKD;
namespace SKDDriver
{
	public class ConfigurationSynchroniser
	{
	
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
			ParentUID = configItem.Parent.UID;
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
