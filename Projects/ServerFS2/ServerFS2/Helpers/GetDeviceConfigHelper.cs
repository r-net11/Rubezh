using FiresecAPI.Models;

namespace ServerFS2.Helpers
{
    public static class GetDeviceConfigHelper
    {
        public static ZoneLogicState GetEventTypeByCode(byte code)
        {
            ZoneLogicState zoneLogicState;
            switch (code)
            {
                case 0x01:
                    zoneLogicState = ZoneLogicState.MPTAutomaticOn;
                    break;
                case 0x02:
                    zoneLogicState = ZoneLogicState.Alarm;
                    break;
                case 0x03:
                    zoneLogicState = ZoneLogicState.GuardSet;
                    break;
                case 0x04:
                    zoneLogicState = ZoneLogicState.Fire;
                    break;
                case 0x05:
                    zoneLogicState = ZoneLogicState.GuardUnSet;
                    break;
                case 0x06:
                    zoneLogicState = ZoneLogicState.PCN;
                    break;
                //case 0x07: Меандр?
                case 0x08:
                    zoneLogicState = ZoneLogicState.Failure;
                    break;
                case 0x09:
                    zoneLogicState = ZoneLogicState.PumpStationOn;
                    break;
                case 0x0A:
                    zoneLogicState = ZoneLogicState.PumpStationAutomaticOff;
                    break;
                case 0x0B:
                    zoneLogicState = ZoneLogicState.AM1TOn;
                    break;
                //case 0x10: Выходная задержка?
                case 0x20:
                    zoneLogicState = ZoneLogicState.Attention;
                    break;
                case 0x40:
                    zoneLogicState = ZoneLogicState.MPTOn;
                    break;
                case 0x80:
                    zoneLogicState = ZoneLogicState.Firefighting;
                    break;
                default:
                    zoneLogicState = new ZoneLogicState();
                    break;
            }
            return zoneLogicState;
        }
    }
}
