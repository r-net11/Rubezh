using System.Collections.Generic;
using System.Linq;

namespace XFiresecAPI
{
	public static class XStateClassHelper
	{
		public static List<XStateClass> Convert(List<XStateType> stateTypes, bool isConnectionLost, bool isMissmatch)
		{
			var stateClasses = new HashSet<XStateClass>();
			if (isConnectionLost)
			{
				stateClasses.Add(XStateClass.ConnectionLost);
				return stateClasses.ToList();
			}
			if (isMissmatch)
			{
				stateClasses.Add(XStateClass.DBMissmatch);
				return stateClasses.ToList();
			}

			foreach (var stateType in stateTypes)
			{
				switch (stateType)
				{
					case XStateType.Fire2:
						stateClasses.Add(XStateClass.Fire2);
						break;
					case XStateType.Fire1:
						stateClasses.Add(XStateClass.Fire1);
						break;
					case XStateType.Attention:
						stateClasses.Add(XStateClass.Attention);
						break;
					case XStateType.Failure:
						stateClasses.Add(XStateClass.Failure);
						break;
					case XStateType.Ignore:
						stateClasses.Add(XStateClass.Ignore);
						break;
					case XStateType.On:
						stateClasses.Add(XStateClass.On);
						break;
					case XStateType.Off:
						stateClasses.Add(XStateClass.Off);
						break;
					case XStateType.TurningOn:
						stateClasses.Add(XStateClass.TurningOn);
						break;
					case XStateType.TurningOff:
						stateClasses.Add(XStateClass.TurningOff);
						break;
					case XStateType.Test:
						stateClasses.Add(XStateClass.Info);
						break;
				}
			}

			if (!stateTypes.Contains(XStateType.Norm))
			{
				stateClasses.Add(XStateClass.AutoOff);
			}

			return stateClasses.ToList();
		}

		public static XStateClass GetMinStateClass(List<XStateClass> stateClasses)
		{
			XStateClass minStateClass = XStateClass.Norm;
			foreach (var stateClass in stateClasses)
			{
				if (stateClass < minStateClass)
					minStateClass = stateClass;
			}
			return minStateClass;
		}
	}
}