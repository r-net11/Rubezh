using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Binary;

namespace ServerFS2.ConfigurationWriter
{
	public class ZoneTable : TableBase
	{
		public Zone Zone { get; set; }

		public override Guid UID
		{
			get { return Zone.UID; }
		}

		public BinaryZone BinaryZone;
		public List<EffectorDeviceTable> EffectorDeviceTables = new List<EffectorDeviceTable>();

		public ZoneTable(FlashDatabase flashDatabase, BinaryZone binaryZone)
			: base(flashDatabase, "Зона " + binaryZone.Zone.PresentationName)
		{
			binaryZone.TableBase = this;
			BinaryZone = binaryZone;
			Zone = binaryZone.Zone;
		}

		public override void Create()
		{
			for (int i = 0; i < 4; i++)
			{
				BytesDatabase.AddByte(0, "Внутренние параметры", true);
			}
			BytesDatabase.AddShort(BinaryZone.LocalNo, "Номер");
			BytesDatabase.AddString(Zone.Name, "Описание");
			var lengtByteDescription = BytesDatabase.AddShort(0, "Длина записи");
			BytesDatabase.AddByte(10, "Длина нижеследующих параметров");
			var zoneConfig = 0;
			if (Zone.ZoneType == ZoneType.Guard)
				zoneConfig = 1;
			BytesDatabase.AddByte(zoneConfig, "Конфиг");
			int zoneAttributes = 0;
			if (Zone.ZoneType == ZoneType.Guard)
			{
				if (Zone.GuardZoneType == GuardZoneType.Ordinary)
					zoneAttributes += 1;
				if (Zone.GuardZoneType == GuardZoneType.Delay)
					zoneAttributes += 8;
				if (Zone.GuardZoneType == GuardZoneType.CanNotReset)
					zoneAttributes += 16;
				if (Zone.Skipped)
					zoneAttributes += 2;
			}
			var hasMPT = false;
			foreach (var device in Zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT)
					hasMPT = true;
			}
			if (hasMPT)
				zoneAttributes += 4;
			BytesDatabase.AddByte(zoneAttributes, "Атрибуты");
			var detectorCount = Zone.DetectorCount;
			if (Zone.ZoneType == ZoneType.Guard)
				detectorCount = 0;
			BytesDatabase.AddByte(detectorCount, "Количество датчиков для формирования Пожара");
			BytesDatabase.AddByte(0, "Количество потерянных ИП", true);
			BytesDatabase.AddShort(Zone.No, "Глобальный номер");

			var diectionNo = 0;
			foreach (var direction in PanelDatabase.BinaryPanel.TempDirections)
			{
				if (direction.ZoneUIDs.Contains(Zone.UID))
					diectionNo = direction.Id;
			}

			BytesDatabase.AddShort(Zone.Delay, "Время задержки");
			BytesDatabase.AddByte(diectionNo, "Номер направления");
			BytesDatabase.AddByte(Zone.AutoSet, "Время автоперевзятия");

			BytesDatabase.AddShort(GetDevicesInLogic().Count, "Общее количество связанных с зоной ИУ");
			InitializeMPT();
			InitializeLocalIUDevices();
			InitializeRemoteIUDevices();
			InitializeRemoteIUPanelsAnd();
			InitializeAllDevices();
			BytesDatabase.SetShort(lengtByteDescription, BytesDatabase.ByteDescriptions.Count);
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
			var devicesOnShleifs = DevicesOnShleifHelper.GetLocalForZone(ParentPanel, Zone);
			foreach (var devicesOnShleif in devicesOnShleifs)
			{
				var referenceBytesDatabase = new BytesDatabase("Локальные устройства шлейфа " + devicesOnShleif.ShleifNo.ToString() + " для зоны " + Zone.PresentationName);
				ReferenceBytesDatabase.Add(referenceBytesDatabase);

				foreach (var device in devicesOnShleif.Devices)
				{
					TableBase table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == device.UID);
					referenceBytesDatabase.AddReferenceToTable(table, "Ссылка на локальное устройство " + device.PresentationAddressAndName);
				}
				var byteDescription = referenceBytesDatabase.ByteDescriptions.FirstOrDefault();
				BytesDatabase.AddByte(devicesOnShleif.Devices.Count, "Количество связанных локальных ИУ шлейфа " + devicesOnShleif.ShleifNo.ToString());
				BytesDatabase.AddReference(byteDescription, "Указатель на размещение абсолютного адреса размещения первого в списке связанного локального ИУ шлейфа  " + devicesOnShleif.ShleifNo.ToString());
			}
		}

		List<BytesDatabase> ReferenceBytesDatabase = new List<BytesDatabase>();

		void InitializeRemoteIUDevices()
		{
			var devices = DevicesOnShleifHelper.GetRemoteForZone(ParentPanel, Zone);
			devices = devices.OrderBy(x => x.IntAddress).ToList();
			BytesDatabase.AddByte(devices.Count, "Количество связанных внешних ИУ");

			var referenceBytesDatabase = new BytesDatabase("Внешние устройства" + " для зоны " + Zone.PresentationName);
			ReferenceBytesDatabase.Add(referenceBytesDatabase);

			foreach (var device in devices)
			{
				TableBase table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == device.UID);
				referenceBytesDatabase.AddReferenceToTable(table, "Ссылка на внешнее устройство " + device.DottedPresentationAddressAndName);
			}

			var byteDescription = referenceBytesDatabase.ByteDescriptions.FirstOrDefault();
			BytesDatabase.AddReference(byteDescription, "Указатель на размещение абсолютного адреса размещения первого в списке связанного внешнего ИУ");
		}

		void InitializeRemoteIUPanelsAnd()
		{
			var remotePanels = DevicesOnShleifHelper.GetRemotePanelsForZone(ParentPanel, Zone);
			BytesDatabase.AddByte(remotePanels.Count, "Количество внешних приборов, ИУ которого могут управлятся нашими ИП по логике межприборное И");
			foreach (var device in remotePanels)
			{
				BytesDatabase.AddByte(device.IntAddress, "Адрес прибора, ИУ которого могут управляться нашими ИП, и которому надо сообщить при изменении состояния зоны");
			}
		}

		void InitializeAllDevices()
		{
			foreach (var referenceBytesDatabase in ReferenceBytesDatabase)
			{
				BytesDatabase.Add(referenceBytesDatabase);
			}
		}

		List<Device> GetDevicesInLogic()
		{
			var result = new List<Device>();
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID == ParentPanel.UID)
				{
					result.Add(device);
				}
			}

			foreach (var device in Zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT)
				{
					result.Add(device);
				}
			}
			return result;
		}
	}
}