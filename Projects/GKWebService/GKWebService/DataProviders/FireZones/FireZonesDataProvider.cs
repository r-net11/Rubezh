using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System.Linq;
using GKWebService.Models.FireZone;
using GKWebService.Utils;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace GKWebService.DataProviders.FireZones
{
    public class FireZonesDataProvider
    {
        /// <summary>
        /// Инстанс провайдера данных
        /// </summary>
        public static FireZonesDataProvider Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                return _instance = new FireZonesDataProvider();
            }
        }

        /// <summary>
        /// Метод, предоставляющий данные 
        /// для вкладки "Пожарные зоны"
        /// </summary>
        /// <returns>Список зон с соответствующими устройствами</returns>
        public List<FireZone> GetFireZones()
        {
            //Формируем список зон со всеми необходимыми данными
            List<FireZone> list = new List<FireZone>();

            //Проходимся по списку всех пожарных зон
            foreach (var currentZone in GKManager.Zones)
            {
                //Получаем зону
                var zone = currentZone;
                //Заносим в нее статус
                var gkStates = ClientManager.FiresecService.GKGetStates();
                foreach (var remoteZoneState in gkStates.ZoneStates)
                {
                    if (currentZone.UID == remoteZoneState.UID)
                    {
                        remoteZoneState.CopyTo(zone.State);
                        break;
                    }
                }
                FireZone data = new FireZone();
                data.StateLabel = Convert.ToString(zone.State.StateClasses[0]);
                data.DescriptorPresentationName = zone.DescriptorPresentationName;
                data.Fire1Count = zone.Fire1Count;
                data.Fire2Count = zone.Fire2Count;
                data.ImageSource = InternalConverter.GetImageResource(zone.ImageSource);
                data.StateImageSource = InternalConverter.GetImageResource("StateClassIcons/" + Convert.ToString(zone.State.StateClass) + ".png");
                list.Add(data);
            }
            return list;
        }


        public List<DeviceNode> GetDevicesByZone(int zoneNumber)
        {
            List<DeviceNode> listTree = new List<DeviceNode>();

            DeviceNode data = new DeviceNode();

            int level = 0;

            if (GKManager.Zones.Count - 1 < zoneNumber)
            {
                return null;
            }

            foreach (var remoteDevice in GKManager.Zones[zoneNumber].Devices)
            {
                data.DeviceList.Add(new Device()
                {
                    Name = remoteDevice.PresentationName,
                    Address = remoteDevice.Address,
                    ImageBloom = InternalConverter.GetImageResource(remoteDevice.ImageSource),
                    StateImageSource = InternalConverter.GetImageResource("StateClassIcons/" + Convert.ToString(remoteDevice.State.StateClass) + ".png"),
                    Level = level
                });
            }

            listTree.Add(data);
            var device = GKManager.Zones[zoneNumber].Devices.FirstOrDefault();

            while (device != null && device.Parent != null)
            {
                level++;
                DeviceNode item = new DeviceNode();
                device = device.Parent;
                item.DeviceList.Add(new Device()
                {
                    Name = device.PresentationName,
                    Address = device.Address,
                    ImageBloom = InternalConverter.GetImageResource(device.ImageSource),
                    StateImageSource = InternalConverter.GetImageResource("StateClassIcons/" + Convert.ToString(device.State.StateClass) + ".png"),
                    Level = level
                });
                listTree.Add(item);
            }

            listTree.Reverse();
            return listTree;
        }

        /// <summary>
        /// private constructor
        /// </summary>
        private FireZonesDataProvider()
        {
            ClientManager.GetConfiguration("GKOPC/Configuration");
        }

        /// <summary>
        /// private instance of DataProvider
        /// </summary>
        private static FireZonesDataProvider _instance;
    }
}