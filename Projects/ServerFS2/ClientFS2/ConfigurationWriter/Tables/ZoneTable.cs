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

        public BinaryZone BinaryZone;
        public List<EffectorDeviceTable> EffectorDeviceTables = new List<EffectorDeviceTable>();

        public ZoneTable(PanelDatabase2 panelDatabase2, BinaryZone binaryZone)
            : base(panelDatabase2, "Зона " + binaryZone.Zone.PresentationName)
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
            foreach (var direction in PanelDatabase.BinaryPanel.TempDirections)
            {
                if (direction.ZoneUIDs.Contains(Zone.UID))
                    diectionNo = direction.Id;
            }

            BytesDatabase.AddShort((short)(Zone.AutoSet * 10), "Время задержки");
            BytesDatabase.AddByte((byte)diectionNo, "Номер направления");
            BytesDatabase.AddByte((byte)Zone.Delay, "Время автоперевзятия");

			BytesDatabase.AddShort((short)GetDevicesInLogic().Count, "Общее количество связанных с зоной ИУ");
            InitializeMPT();
            InitializeLocalIUDevices();
            InitializeRemoteIUDevices();
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
				var referenceBytesDatabase = new BytesDatabase("Локальные устройства шлейфа " + devicesOnShleif.ShleifNo.ToString() + " для зоны " + Zone.PresentationName);
				ReferenceBytesDatabase.Add(referenceBytesDatabase);

				foreach (var device in devicesOnShleif.Devices)
				{
					TableBase table = PanelDatabase.Tables.FirstOrDefault(x => x.UID == device.UID);
					referenceBytesDatabase.AddReferenceToTable(table, "Ссылка на локальное устройство " + device.PresentationAddressAndName);
				}
				var byteDescription = referenceBytesDatabase.ByteDescriptions.FirstOrDefault();
				BytesDatabase.AddByte((byte)devicesOnShleif.Devices.Count, "Количество связанных локальных ИУ шлейфа " + devicesOnShleif.ShleifNo.ToString());
				BytesDatabase.AddReference(byteDescription, "Указатель на размещение абсолютного адреса размещения первого в списке связанного локального ИУ шлейфа  " + devicesOnShleif.ShleifNo.ToString());
			}
		}

		List<BytesDatabase> ReferenceBytesDatabase = new List<BytesDatabase>();

		void InitializeRemoteIUDevices()
		{
			var devices = DevicesOnShleifHelper.GetRemoteForZone(ParentPanel, Zone);
			devices = devices.OrderBy(x => x.IntAddress).ToList();
			BytesDatabase.AddByte((byte)devices.Count, "Количество связанных внешних ИУ");

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
            BytesDatabase.AddByte((byte)remotePanels.Count, "Количество внешних приборов, ИУ которого могут управлятся нашими ИП по логике межприборное И");
            foreach (var device in remotePanels)
            {
                BytesDatabase.AddByte((byte)device.IntAddress, "Адрес прибора, ИУ которого могут управляться нашими ИП, и которому надо сообщить при изменении состояния зоны");
            }
        }

        void InitializeAllDevices()
        {
			foreach (var referenceBytesDatabase in ReferenceBytesDatabase)
			{
				BytesDatabase.Add(referenceBytesDatabase);
			}
        }

		List<Device> GetDevicesInZoneLogic()
		{
			//var parentPanel = Zone.DevicesInZone.FirstOrDefault().ParentPanel;
			var result = new List<Device>();
			foreach (var device in Zone.DevicesInZoneLogic)
			{
				if (device.ParentPanel.UID == ParentPanel.UID)
				{
					result.Add(device);
				}
			}
			return result;
		}

		List<Device> GetDevicesInLogic()
		{
			var result = GetDevicesInZoneLogic();
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