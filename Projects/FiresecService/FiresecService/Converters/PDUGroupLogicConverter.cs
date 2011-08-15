using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.XSD;

namespace FiresecService.Converters
{
    public static class PDUGroupLogicConverter
    {
        public static PDUGroupLogic Convert(RCGroupProperties rCGroupProperties)
        {
            var pDUGroupLogic = new PDUGroupLogic();

            pDUGroupLogic.AMTPreset = (rCGroupProperties.AMTPreset == "1");
            if ((rCGroupProperties != null) && (rCGroupProperties.device != null))
            {
                foreach (var groupDevice in rCGroupProperties.device)
                {
                    var pDUGroupDevice = new PDUGroupDevice();
                    pDUGroupDevice.DeviceUID = groupDevice.UID;
                    pDUGroupDevice.IsInversion = (groupDevice.Inverse == "1");
                    pDUGroupDevice.OnDelay = int.Parse(groupDevice.DelayOn);
                    pDUGroupDevice.OffDelay = int.Parse(groupDevice.DelayOff);
                    pDUGroupLogic.Devices.Add(pDUGroupDevice);
                }
            }

            return pDUGroupLogic;
        }

        public static RCGroupProperties ConvertBack(PDUGroupLogic pDUGroupLogic)
        {
            var rCGroupProperties = new RCGroupProperties();

            if ((pDUGroupLogic != null) && (pDUGroupLogic.Devices.Count > 0))
            {
                rCGroupProperties.DevCount = pDUGroupLogic.Devices.Count.ToString();
                rCGroupProperties.AMTPreset = pDUGroupLogic.AMTPreset ? "1" : "0";
                var groupDevices = new List<RCGroupPropertiesDevice>();
                foreach (var device in pDUGroupLogic.Devices)
                {
                    var groupDevice = new RCGroupPropertiesDevice();
                    groupDevice.UID = device.DeviceUID;
                    groupDevice.Inverse = device.IsInversion ? "1" : "0";
                    groupDevice.DelayOn = device.OnDelay.ToString();
                    groupDevice.DelayOff = device.OffDelay.ToString();
                    groupDevices.Add(groupDevice);
                }
                rCGroupProperties.device = groupDevices.ToArray();
            }

            return rCGroupProperties;
        }
    }
}