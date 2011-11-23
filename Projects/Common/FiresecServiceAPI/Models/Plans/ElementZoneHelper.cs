using System.Windows.Media;

namespace FiresecAPI.Models
{
    public static class ElementZoneHelper
    {
        public static Color GetZoneColor(Zone zone)
        {
            Color color = Colors.Gray;
            if (zone != null)
            {
                if (zone.ZoneType == ZoneType.Fire)
                    color = Colors.Green;

                if (zone.ZoneType == ZoneType.Guard)
                    color = Colors.Brown;
            }
            return color;
        }
    }
}
