using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecService.XSD;
using Common;

namespace FiresecService.Converters
{
    public static class PDUGroupLogicConverter
    {
        public static PDUGroupLogic Convert(RCGroupProperties rCGroupProperties)
        {
            var pDUGroupLogic = new PDUGroupLogic();

            pDUGroupLogic.AMTPreset = (rCGroupProperties.AMTPreset == "1");
            if (rCGroupProperties != null && rCGroupProperties.device != null)
            {
                foreach (var groupDevice in rCGroupProperties.device)
                {
                    pDUGroupLogic.Devices.Add(new PDUGroupDevice()
                    {
                        DeviceUID = GuidHelper.ToGuid(groupDevice.UID),
                        IsInversion = (groupDevice.Inverse == "1"),
                        OnDelay = int.Parse(groupDevice.DelayOn),
                        OffDelay = int.Parse(groupDevice.DelayOff)
                    });
                }
            }

            return pDUGroupLogic;
        }

        public static RCGroupProperties ConvertBack(PDUGroupLogic pDUGroupLogic)
        {
            var rCGroupProperties = new RCGroupProperties();

            if (pDUGroupLogic != null && pDUGroupLogic.Devices.Count > 0)
            {
                rCGroupProperties.DevCount = pDUGroupLogic.Devices.Count.ToString();
                rCGroupProperties.AMTPreset = pDUGroupLogic.AMTPreset ? "1" : "0";
                var groupDevices = new List<RCGroupPropertiesDevice>();
                foreach (var device in pDUGroupLogic.Devices)
                {
                    groupDevices.Add(new RCGroupPropertiesDevice()
                    {
                        UID = device.DeviceUID.ToString(),
                        Inverse = device.IsInversion ? "1" : "0",
                        DelayOn = device.OnDelay.ToString(),
                        DelayOff = device.OffDelay.ToString()
                    });
                }
                rCGroupProperties.device = groupDevices.ToArray();
            }

            return rCGroupProperties;
        }
    }
}