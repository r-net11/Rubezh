using GKWebService.Models.Devices;
using GKWebService.Utils;
using RubezhAPI;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.DataProviders.Devices
{
    public class DevicesDataProvider
    {
        /// <summary>
        /// Инстанс провайдера данных
        /// </summary>
        public static DevicesDataProvider Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                return _instance = new DevicesDataProvider();
            }
        }

        /// <summary>
        /// private instance of DataProvider
        /// </summary>
        private static DevicesDataProvider _instance;

        public IEnumerable<Device> GetDevices()
        {
            var root = GKManager.DeviceConfiguration.RootDevice;
            return BuildTreeList(root);
        }

        private List<Device> BuildTreeList(GKDevice node, int level = 0)
        {
            List<Device> list = new List<Device>();

            list.Add(new Device
            {
                UID = node.UID,
                ParentUID = node.Parent != null ? node.Parent.UID : (Guid?) null,
                Name = node.PresentationName,
                Address = node.Address,
                Level = level,
                IsLeaf = node.Children.Count == 0,
                ImageBloom = InternalConverter.GetImageResource(node.ImageSource),
                StateImageSource = InternalConverter.GetImageResource("StateClassIcons/" + Convert.ToString(node.State.StateClass) + ".png"),
            });

            foreach (var child in node.Children)
            {
                list.AddRange(BuildTreeList(child, level + 1));
            }

            return list;
        }
    }
}