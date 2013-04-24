using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecAPI.Models.Binary;

namespace ClientFS2.ConfigurationWriter
{
    public class ZoneTable : TableBase
    {
        public Zone Zone { get; set; }

        public override Guid UID
        {
            get { return Zone.UID; }
        }

        BinaryZone BinaryZone;
        public List<EffectorDeviceTable> EffectorDeviceTables = new List<EffectorDeviceTable>();

        public ZoneTable(PanelDatabase2 panelDatabase2, BinaryZone binaryZone)
            : base(panelDatabase2, binaryZone.Zone.PresentationName)
        {
            binaryZone.TableBase = this;
            BinaryZone = binaryZone;
            Zone = binaryZone.Zone;
        }

        public override void Create()
        {
            for (int i = 0; i < 4; i++)
            {
                BytesDatabase.AddByte((byte)0, "Внутренние параметры", true);
            }
            BytesDatabase.AddShort((short)BinaryZone.LocalNo, "Номер");
            BytesDatabase.AddString(Zone.Name, "Описание");
            var lengtByteDescription = BytesDatabase.AddShort((short)0, "Длина записи");
            BytesDatabase.AddByte((byte)10, "Длина нижеследующих параметров");
            var zoneConfig = 0;
            if (Zone.ZoneType == ZoneType.Guard)
                zoneConfig = 1;
            BytesDatabase.AddByte((byte)zoneConfig, "Конфиг");
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
            BytesDatabase.AddByte((byte)zoneAttributes, "Атрибуты");
            BytesDatabase.AddByte((byte)Zone.DetectorCount, "Количество датчиков для формирования Пожара");
            BytesDatabase.AddByte((byte)0, "Количество потерянных ИП", true);
            BytesDatabase.AddShort((short)Zone.No, "Глобальный номер");

            var diectionNo = 0;

            BytesDatabase.AddShort((short)(Zone.AutoSet * 10), "Время задержки");
            BytesDatabase.AddByte((byte)diectionNo, "Номер направления");
            BytesDatabase.AddByte((byte)Zone.Delay, "Время автоперевзятия");

			BytesDatabase.AddShort((short)GetDevicesInLogic().Count, "Общее количество связанных с зоной ИУ");
            InitializeMPT();
            InitializeLocalIUDevices();
            //InitializeRemoteIUDevices();
            InitializeRemoteIUPanelsOr();
            InitializeRemoteIUPanelsAnd();
            InitializeAllDevices();
            BytesDatabase.SetShort(lengtByteDescription, (short)(BytesDatabase.ByteDescriptions.Count));
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
                var referenceTable = new TableBase(PanelDatabase, "Локальные устройства шлейфа " + devicesOnShleif.ShleifNo.ToString() + " для зоны " + Zone.PresentationName);
                foreach (var device in devicesOnShleif.Devices)
                {
                    TableBase table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == device.UID);
                    referenceTable.BytesDatabase.AddReferenceToTable(table, "Ссылка на устройство " + device.PresentationAddressAndName);
                }
                if (referenceTable.BytesDatabase.ByteDescriptions.Count > 0)
                {
                    ReferenceTables.Add(referenceTable);
                }
                var byteDescriptions = referenceTable.BytesDatabase.ByteDescriptions.FirstOrDefault();
                BytesDatabase.AddByte((byte)devicesOnShleif.Devices.Count, "Количество связанных локальных ИУ шлейфа " + devicesOnShleif.ShleifNo.ToString());
                BytesDatabase.AddReference(byteDescriptions, "Указатель на размещение абсолютного адреса размещения первого в списек связанного локального ИУ шлейфа  " + devicesOnShleif.ShleifNo.ToString());
            }
        }

		//void InitializeRemoteIUDevices()
		//{
		//    var devicesOnShleifs = DevicesOnShleifHelper.GetRemoteForZone(ParentPanel, Zone);
		//    foreach (var devicesOnShleif in devicesOnShleifs)
		//    {
		//        var referenceTable = new TableBase(PanelDatabase, "Внешние устройства шлейфа " + devicesOnShleif.ShleifNo.ToString() + " для зоны " + Zone.PresentationName);
		//        foreach (var device in devicesOnShleif.Devices)
		//        {
		//            TableBase table = EffectorDeviceTables.FirstOrDefault(x => x.UID == device.UID);
		//            referenceTable.BytesDatabase.AddReferenceToTable(table, "Ссылка на устройство " + device.PresentationAddressAndName);
		//        }
		//        if (referenceTable.BytesDatabase.ByteDescriptions.Count > 0)
		//        {
		//            ReferenceTables.Add(referenceTable);
		//        }
		//        var byteDescription = referenceTable.BytesDatabase.ByteDescriptions.FirstOrDefault();
		//        BytesDatabase.AddByte((byte)devicesOnShleif.Devices.Count, "Количество связанных внешних ИУ шлейфа " + devicesOnShleif.ShleifNo.ToString());
		//        BytesDatabase.AddReference(byteDescription, "Указатель на размещение абсолютного адреса размещения первого в списек связанного внешнего ИУ шлейфа  " + devicesOnShleif.ShleifNo.ToString());
		//    }
		//}

        void InitializeRemoteIUPanelsOr()
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
            BytesDatabase.AddByte((byte)remotePanels.Count, "Количество внешних приборов, ИУ которого могут управлятся нашими ИП по логике межприборное ИЛИ");
            ByteDescription byteDescription = null;
            BytesDatabase.AddReference(byteDescription, "Указатель на размещение абсолютного адреса размещения первого в списек связанного внешнего ИУ по логике межприборное ИЛИ");
        }

        void InitializeRemoteIUPanelsAnd()
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

		List<Device> GetDevicesInLogic()
		{
			var result = Zone.DevicesInZoneLogic;
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