using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using System.Linq;
using GKWebService.Models.FireZone;
using GKWebService.Utils;
using System;

namespace GKWebService.DataProviders.FireZones
{
    public class FireZonesDataProvider
    {
        private FireZonesDataProvider()
        {
            ClientManager.GetConfiguration("GKOPC/Configuration");
		}

        public FireZone GetZone()
        {
            var gkStates = ClientManager.FiresecService.GKGetStates();
            GKZone zone = GKManager.Zones[0];
            foreach (var remoteZoneState in gkStates.ZoneStates)
            {
                zone = GKManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
                if (zone != null)
                {
                    remoteZoneState.CopyTo(zone.State);
                    break;
                }
            }

            //Создали объект для передачи на клиент и заполняем его данными
            FireZone data = new FireZone();

            data.StateLabel = Convert.ToString(gkStates.ZoneStates[0].StateClasses[0]);

            //Имя зоны
            data.DescriptorPresentationName = zone.DescriptorPresentationName;

            //Количество датчиков для перевода в состояние Пожар1
            data.Fire1Count = zone.Fire1Count;

            //Количество датчиков для перевода в состояние Пожар2
            data.Fire2Count = zone.Fire2Count;

            //Иконка текущей зоны
            data.ImageSource = InternalConverter.GetImageResource(zone.ImageSource);

            //Изображение, сигнализирующее о состоянии зоны
            data.StateImageSource = InternalConverter.GetImageResource("StateClassIcons/" + Convert.ToString(zone.State.StateClass) + ".png");

            //Переносим устройства для этой зоны
            foreach (var deviceItem in zone.Devices)
            {
                var device = deviceItem;
                do
                {
                    data.devicesList.Add(new Device(device.Address, device.ImageSource, device.ShortName, device.State.StateClass));
                    device = device.Parent;
                } while (device != null);
            }
            data.devicesList.Reverse();
            return data;
        }



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

        private static FireZonesDataProvider _instance;
    }
}