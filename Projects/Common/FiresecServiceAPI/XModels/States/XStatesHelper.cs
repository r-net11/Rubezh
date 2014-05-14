using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.GK
{
	public static class XStatesHelper
	{
		public static List<XStateBit> StatesFromInt(int intValue)
		{
			var states = new List<XStateBit>();
			var bitArray = new BitArray(new int[1] { intValue });
			for (int bitIndex = 0; bitIndex < bitArray.Count; bitIndex++)
			{
				var b = bitArray[bitIndex];
				if (b)
				{
					var stateTupe = (XStateBit)bitIndex;
					states.Add(stateTupe);
				}
			}
			return states;
		}

		public static List<XStateClass> StateBitsToStateClasses(List<XStateBit> stateBits)
		{
			var stateClasses = new HashSet<XStateClass>();
			foreach (var stateBit in stateBits)
			{
				switch (stateBit)
				{
					case XStateBit.Fire2:
						stateClasses.Add(XStateClass.Fire2);
						break;
					case XStateBit.Fire1:
						stateClasses.Add(XStateClass.Fire1);
						break;
					case XStateBit.Attention:
						stateClasses.Add(XStateClass.Attention);
						break;
					case XStateBit.Failure:
						stateClasses.Add(XStateClass.Failure);
						break;
					case XStateBit.Ignore:
						stateClasses.Add(XStateClass.Ignore);
						break;
					case XStateBit.On:
						stateClasses.Add(XStateClass.On);
						break;
					case XStateBit.Off:
						stateClasses.Add(XStateClass.Off);
						break;
					case XStateBit.TurningOn:
						stateClasses.Add(XStateClass.TurningOn);
						break;
					case XStateBit.TurningOff:
						stateClasses.Add(XStateClass.TurningOff);
						break;
					case XStateBit.Test:
						stateClasses.Add(XStateClass.Test);
						break;
				}
			}

			if (!stateBits.Contains(XStateBit.Norm))
			{
				stateClasses.Add(XStateClass.AutoOff);
			}

			if (stateBits.Contains(XStateBit.Ignore))
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


		#region Converters
		static StateType OLD_XStateClassToStateType(XStateClass stateClass)
		{
			switch (stateClass)
			{
				case XStateClass.Fire1:
				case XStateClass.Fire2:
					return StateType.Fire;

				case XStateClass.Attention:
					return StateType.Attention;

				case XStateClass.Failure:
					return StateType.Failure;

				case XStateClass.Ignore:
					return StateType.Off;

				case XStateClass.DBMissmatch:
					return StateType.Service;

				case XStateClass.Test:
				case XStateClass.Info:
				case XStateClass.On:
				case XStateClass.TurningOn:
					return StateType.Info;

				default:
					return StateType.Norm;
			}
		}

		public static StateType XStateTypesToState(List<XStateClass> stateClasses)
		{
			var minPriority = 7;
			foreach (var state in stateClasses)
			{
				var priority = OLD_XStateClassToStateType(state);
				if ((int)priority < minPriority)
				{
					minPriority = (int)priority;
				}
			}
			StateType stateType = (StateType)minPriority;
			return stateType;
		}

		public static XStateClass StateTypeToXStateClass(StateType stateType)
		{
			switch(stateType)
			{
				case StateType.Attention:
					return XStateClass.Attention;
				case StateType.Fire:
					return XStateClass.Fire1;
				case StateType.Info:
					return XStateClass.Test;
				case StateType.No:
					return XStateClass.No;
				case StateType.Norm:
					return XStateClass.Norm;
				case StateType.Off:
					return XStateClass.Ignore;
				case StateType.Service:
					return XStateClass.Service;
				case StateType.Unknown:
					return XStateClass.Unknown;
				default:
					return XStateClass.Norm;
			}
		}

		#endregion
	}
}