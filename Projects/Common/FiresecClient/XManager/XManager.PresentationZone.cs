using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static string GetPresentationZone(XDevice device)
		{
			if (device.Driver.HasZone)
			{
				var stringBuilder = new StringBuilder();
				var indx = 0;
				foreach(var zoneUID in device.Zones)
				{
					var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						if (indx > 0)
						stringBuilder.Append(", ");
						stringBuilder.Append(zone.PresentationName);
						indx++;
					}
				}
				return stringBuilder.ToString();
			}

			if (device.Driver.HasLogic && device.DeviceLogic != null)
				return GetPresentationZone(device.DeviceLogic);

			return "";
		}

		public static string GetPresentationZone(XDeviceLogic DeviceLogic)
		{
			return "";
			//string result = "";

			//for (int i = 0; i < zoneLogic.Clauses.Count; i++)
			//{
			//    var clause = zoneLogic.Clauses[i];

			//    if (i > 0)
			//    {
			//        switch (zoneLogic.JoinOperator)
			//        {
			//            case ZoneLogicJoinOperator.And:
			//                result += " и ";
			//                break;
			//            case ZoneLogicJoinOperator.Or:
			//                result += " или ";
			//                break;
			//            default:
			//                break;
			//        }
			//    }

			//    if (clause.DeviceUID != Guid.Empty)
			//    {
			//        result += "Сработка устройства " + clause.Device.PresentationAddress + " - " + clause.Device.Driver.Name;
			//        continue;
			//    }

			//    if (clause.State == ZoneLogicState.Failure)
			//    {
			//        result += "состояние неисправность прибора";
			//        continue;
			//    }

			//    result += "состояние " + clause.State.ToDescription();

			//    string stringOperation = null;
			//    switch (clause.Operation)
			//    {
			//        case ZoneLogicOperation.All:
			//            stringOperation = "во всех зонах из";
			//            break;

			//        case ZoneLogicOperation.Any:
			//            stringOperation = "в любой зоне из";
			//            break;

			//        default:
			//            break;
			//    }

			//    result += " " + stringOperation + " " + GetClausePresentationName(clause.Zones);
			//}

			//return result;
		}
	}
}