using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class ZoneTable : TableBase
	{
		public Zone Zone { get; set; }

		public override Guid UID
		{
			get { return Zone.UID; }
		}

		public ZoneTable(PanelDatabase panelDatabase, Zone zone)
			: base(panelDatabase)
		{
			Zone = zone;
		}

		public override void  Create()
{
			BytesDatabase.AddShort((short)Zone.No, "Номер");
			BytesDatabase.AddString(Zone.Name, "Описание");
			var lengtByteDescription = BytesDatabase.AddShort((short)0, "Длина записи");
			BytesDatabase.AddShort((short)10, "Длина нижеследующих параметров");
			var zoneConfig = 0;
			if(Zone.ZoneType == ZoneType.Guard)
				zoneConfig = 1;
			BytesDatabase.AddByte((byte)zoneConfig, "Конфиг");
			int zoneAttributes = 0;
			if (Zone.GuardZoneType == GuardZoneType.Ordinary)
				zoneAttributes += 1;
			if (Zone.GuardZoneType == GuardZoneType.Delay)
				zoneAttributes += 8;
			if (Zone.GuardZoneType == GuardZoneType.CanNotReset)
				zoneAttributes += 16;
			if(Zone.Skipped)
				zoneAttributes += 2;
			var hasMPT = false;
			foreach (var device in Zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT)
					hasMPT = true;
			}
			if (hasMPT)
				zoneAttributes += 4;
			BytesDatabase.AddByte((byte)zoneAttributes, "Атрибуты");
			BytesDatabase.AddByte((byte)Zone.DetectorCount, "Количество датчиков для формирования Пожара");
			BytesDatabase.AddByte((byte)0, "Количество потерянных ИП");
			BytesDatabase.AddShort((short)Zone.No, "Глобальный номер");

			var autosetTime = 0;
			if(!string.IsNullOrEmpty(Zone.AutoSet))
			{
				autosetTime = Int32.Parse(Zone.AutoSet) * 10;
			}
			var delayTime = 0;
			if (!string.IsNullOrEmpty(Zone.Delay))
			{
				delayTime = Int32.Parse(Zone.Delay);
			}
			var diectionNo = 0;

			BytesDatabase.AddShort((short)delayTime, "Время задержки");
			BytesDatabase.AddByte((byte)diectionNo, "Номер направления");
			BytesDatabase.AddByte((byte)autosetTime, "Время автоперевзятия");

			BytesDatabase.AddShort((short)Zone.DevicesInZoneLogic.Count, "Общее количество связанных с зоной ИУ");
			InitializeMPT();
			InitializeLocalIUDevices();
			InitializeRemoteIUDevices();
			InitializeRemoteIUPanels();
			InitializeAllDevices();
			BytesDatabase.SetShort(lengtByteDescription, (short)BytesDatabase.ByteDescriptions.Count);
			BytesDatabase.SetGroupName(Zone.PresentationName);
		}

		void InitializeMPT()
		{
			Device mptDevice = null;
			foreach (var device in Zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT && device.Parent.Driver.DriverType != DriverType.MPT)
					mptDevice = device;
			}
			TableBase table = null;
			if (mptDevice != null)
			{
				table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == mptDevice.UID);
			}
			BytesDatabase.AddReferenceToTable(table, "Указатель на ведущее МПТ в зоне");
		}

		void InitializeLocalIUDevices()
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= ParentPanel.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID == ParentPanel.UID)
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
					var table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == device.UID);
					referenceBytesDatabase.AddReferenceToTable(table, "Ссылка на устройство " + device.PresentationAddressAndName);
				}
				if (referenceBytesDatabase.ByteDescriptions.Count > 0)
					ReferenceBytesDatabase.Add(referenceBytesDatabase);
				var byteDescriptions = referenceBytesDatabase.ByteDescriptions.FirstOrDefault();
				BytesDatabase.AddByte((byte)devicesOnShleif.Devices.Count, "Количество связанных локальных ИУ шлейфа " + devicesOnShleif.ShleifNo.ToString());
				BytesDatabase.AddReference(byteDescriptions, "Указатель на размещение абсолютного адреса размещения первого в списек связанного локального ИУ шлейфа  " + devicesOnShleif.ShleifNo.ToString());
			}
		}

		void InitializeRemoteIUDevices()
		{
			var devicesOnShleifs = new List<DevicesOnShleif>();
			for (int i = 1; i <= ParentPanel.Driver.ShleifCount; i++)
			{
				var devicesOnShleif = new DevicesOnShleif()
				{
					ShleifNo = i
				};
				devicesOnShleifs.Add(devicesOnShleif);
			}
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID != ParentPanel.UID)
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
					var table = PanelDatabase.Tables.FirstOrDefault(x=>x.UID == device.UID);
					referenceBytesDatabase.AddReferenceToTable(table, "Ссылка на устройство " + device.PresentationAddressAndName);
				}
				if (referenceBytesDatabase.ByteDescriptions.Count > 0)
					ReferenceBytesDatabase.Add(referenceBytesDatabase);
				var byteDescriptions = referenceBytesDatabase.ByteDescriptions.FirstOrDefault();
				BytesDatabase.AddByte((byte)devicesOnShleif.Devices.Count, "Количество связанных внешних ИУ шлейфа " + devicesOnShleif.ShleifNo.ToString());
				BytesDatabase.AddReference(byteDescriptions, "Указатель на размещение абсолютного адреса размещения первого в списек связанного внешнего ИУ шлейфа  " + devicesOnShleif.ShleifNo.ToString());
			}
		}

		void InitializeRemoteIUPanels()
		{
			var remotePanels = new List<Device>();
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID != ParentPanel.UID)
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
				if (device.ParentPanel.UID == ParentPanel.UID)
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
				var table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == device.UID);
				BytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения связанного с зоной ИУ");
			}
			foreach (var device in remoteDevices)
			{
				var table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == device.UID);
				BytesDatabase.AddReferenceToTable(table, "Абсолютный адрес размещения связанного с зоной внешнего ИУ");
			}
		}
	}
}