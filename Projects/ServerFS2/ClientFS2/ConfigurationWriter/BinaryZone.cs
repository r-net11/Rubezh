using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class BinaryZone : TableBase
	{
		public Zone Zone { get; set; }

		public BinaryZone(Device panelDevice, Zone zone)
			: base(panelDevice)
		{
			Zone = zone;

			BytesDatabase.AddShort((short)zone.No, "Номер");
			BytesDatabase.AddString(zone.Name, "Описание");
			var lengtByteDescription = BytesDatabase.AddShort((short)0, "Длина записи");
			BytesDatabase.AddShort((short)10, "Длина нижеследующих параметров");
			var zoneConfig = 0;
			if(zone.ZoneType == ZoneType.Guard)
				zoneConfig = 1;
			BytesDatabase.AddByte((byte)zoneConfig, "Конфиг");
			int zoneAttributes = 0;
			if (zone.GuardZoneType == GuardZoneType.Ordinary)
				zoneAttributes += 1;
			if (zone.GuardZoneType == GuardZoneType.Delay)
				zoneAttributes += 8;
			if (zone.GuardZoneType == GuardZoneType.CanNotReset)
				zoneAttributes += 16;
			if(zone.Skipped)
				zoneAttributes += 2;
			var hasMPT = false;
			foreach (var device in zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT)
					hasMPT = true;
			}
			if (hasMPT)
				zoneAttributes += 4;
			BytesDatabase.AddByte((byte)zoneAttributes, "Атрибуты");
			BytesDatabase.AddByte((byte)zone.DetectorCount, "Количество датчиков для формирования Пожара");
			BytesDatabase.AddByte((byte)0, "Количество потерянных ИП");
			BytesDatabase.AddShort((short)zone.No, "Глобальный номер");

			var autosetTime = 0;
			if(!string.IsNullOrEmpty(zone.AutoSet))
			{
				autosetTime = Int32.Parse(zone.AutoSet) * 10;
			}
			var delayTime = 0;
			if (!string.IsNullOrEmpty(zone.Delay))
			{
				delayTime = Int32.Parse(zone.Delay);
			}
			var diectionNo = 0;

			BytesDatabase.AddByte((byte)delayTime, "Время задержки");
			BytesDatabase.AddByte((byte)diectionNo, "Номер направления");
			BytesDatabase.AddByte((byte)autosetTime, "Время автоперевзятия");

			InitializeMPT();
			InitializeLocalIUDevices();
			InitializeRemoteIUDevices();
			InitializeRemoteIUPanels();
			InitializeAllDevices();
			BytesDatabase.SetShort(lengtByteDescription, (short)BytesDatabase.ByteDescriptions.Count);
			BytesDatabase.SetGroupName(zone.PresentationName);
		}

		void InitializeMPT()
		{
			Device mptDevice = null;
			foreach (var device in Zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT && device.Parent.Driver.DriverType != DriverType.MPT)
					mptDevice = device;
			}
			if (mptDevice != null)
			{

			}
		}

		void InitializeLocalIUDevices()
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= PanelDevice.Driver.ShleifCount; i++ )
				{
					var devicesOnShleif = new DevicesOnShleif()
					{
						ShleifNo = i
					};
					devicesOnShleifs.Add(devicesOnShleif);
				}
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID == PanelDevice.UID)
				{
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x=>x.ShleifNo == device.ShleifNo);
					devicesOnShleif.Devices.Add(device);
				}
			}
			foreach (var devicesOnShleif in devicesOnShleifs)
			{
				BytesDatabase.AddByte((byte)devicesOnShleif.Devices.Count, "Количество связанных ИУ шлейфа " + devicesOnShleif.ShleifNo.ToString());
				BytesDatabase.AddReference(null, "Указатель на размещение абсолютного адреса размещения первого в списек связанного ИУ шлейфа  " + devicesOnShleif.ShleifNo.ToString());
			}
		}

		void InitializeRemoteIUDevices()
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= PanelDevice.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID != PanelDevice.UID)
				{
					var devicesOnShleif = devicesOnShleifs.FirstOrDefault(x => x.ShleifNo == device.ShleifNo);
					devicesOnShleif.Devices.Add(device);
				}
			}
			foreach (var devicesOnShleif in devicesOnShleifs)
			{
				var referenceBytesDatabase = new BytesDatabase();
				foreach (var device in devicesOnShleif.Devices)
				{
					referenceBytesDatabase.AddReferenceToDevice(device);
				}
				if (referenceBytesDatabase.ByteDescriptions.Count > 0)
					ReferenceBytesDatabase.Add(referenceBytesDatabase);
				var byteDescriptions = referenceBytesDatabase.ByteDescriptions.FirstOrDefault();
				byteDescriptions = null;
				BytesDatabase.AddByte((byte)devicesOnShleif.Devices.Count, "Количество связанных внешних ИУ шлейфа " + devicesOnShleif.ShleifNo.ToString());
				BytesDatabase.AddReference(byteDescriptions, "Указатель на размещение абсолютного адреса размещения первого в списек связанного внешнего ИУ шлейфа  " + devicesOnShleif.ShleifNo.ToString());
			}
		}

		void InitializeRemoteIUPanels()
		{
			var remotePanels = new List<Device>();
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID != PanelDevice.UID)
				{
					if (!remotePanels.Any(x => x.UID == device.ParentPanel.UID))
					{
						remotePanels.Add(device.ParentPanel);
					}
				}
			}
			BytesDatabase.AddByte((byte)remotePanels.Count, "Количество внешних приборов, ИУ которого могут управлятся нашими ИП по логике межприборное И");
			foreach (var device in remotePanels)
			{
				BytesDatabase.AddByte((byte)device.IntAddress, "Адрес прибора, ИУ которого могут управляться нашими ИП, и которому надо сообщить при изменении состояния зоны");
			}
		}

		void InitializeAllDevices()
		{
			var localDevices = new List<Device>();
			var remoteDevices = new List<Device>();
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID == PanelDevice.UID)
				{
					localDevices.Add(device);
				}
				else
				{
					remoteDevices.Add(device);
				}
			}
			localDevices = (from Device device in localDevices orderby device.ShleifNo select device).ToList();
			remoteDevices = (from Device device in remoteDevices orderby device.ShleifNo select device).ToList();
			foreach (var device in localDevices)
			{
				BytesDatabase.AddReference(null, "Абсолютный адрес размещения связанного с зоной ИУ");
			}
			foreach (var device in remoteDevices)
			{
				BytesDatabase.AddReference(null, "Абсолютный адрес размещения связанного с зоной внешнего ИУ");
			}
		}
	}
}