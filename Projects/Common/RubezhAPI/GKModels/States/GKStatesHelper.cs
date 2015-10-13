using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RubezhAPI.GK
{
	public static class GKStatesHelper
	{
		public static List<GKStateBit> StatesFromInt(int intValue)
		{
			var states = new List<GKStateBit>();
			var bitArray = new BitArray(new int[1] { intValue });
			for (int bitIndex = 0; bitIndex < bitArray.Count; bitIndex++)
			{
				var b = bitArray[bitIndex];
				if (b)
				{
					var stateTupe = (GKStateBit)bitIndex;
					states.Add(stateTupe);
				}
			}
			return states;
		}

		public static List<XStateClass> StateBitsToStateClasses(List<GKStateBit> stateBits)
		{
			var stateClasses = new HashSet<XStateClass>();
			foreach (var stateBit in stateBits)
			{
				switch (stateBit)
				{
					case GKStateBit.Fire2:
						stateClasses.Add(XStateClass.Fire2);
						break;
					case GKStateBit.Fire1:
						stateClasses.Add(XStateClass.Fire1);
						break;
					case GKStateBit.Attention:
						stateClasses.Add(XStateClass.Attention);
						break;
					case GKStateBit.Failure:
						stateClasses.Add(XStateClass.Failure);
						break;
					case GKStateBit.Ignore:
						stateClasses.Add(XStateClass.Ignore);
						break;
					case GKStateBit.On:
						stateClasses.Add(XStateClass.On);
						break;
					case GKStateBit.Off:
						stateClasses.Add(XStateClass.Off);
						break;
					case GKStateBit.TurningOn:
						stateClasses.Add(XStateClass.TurningOn);
						break;
					case GKStateBit.TurningOff:
						stateClasses.Add(XStateClass.TurningOff);
						break;
					case GKStateBit.Test:
						stateClasses.Add(XStateClass.Test);
						break;
				}
			}

			if (!stateBits.Contains(GKStateBit.Norm))
			{
				stateClasses.Add(XStateClass.AutoOff);
			}

			if (stateBits.Contains(GKStateBit.Ignore))
			{
				stateClasses.Remove(XStateClass.AutoOff);
			}

			if (stateClasses.Count == 0)
			{
				stateClasses.Add(XStateClass.Norm);
			}
			var result = stateClasses.ToList();
			result.OrderBy(x => x);
			return result;
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