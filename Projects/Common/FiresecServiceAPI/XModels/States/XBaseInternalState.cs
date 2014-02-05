using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
	public abstract class XBaseInternalState
	{
		public XBaseInternalState()
		{
			Clear();
		}

		public void Clear()
		{
			AdditionalStates = new List<XAdditionalState>();
			IsInitialState = true;
			IsNoLicense = false;
			IsConnectionLost = false;
			IsInTechnologicalRegime = false;
			IsDBMissmatch = false;
			IsDBMissmatchDuringMonitoring = false;
			OnDelay = 0;
			HoldDelay = 0;
			OffDelay = 0;
		}

		public bool IsInitialState { get; set; }
		public bool IsSuspending { get; set; }
		public bool IsNoLicense { get; set; }
		public bool IsConnectionLost { get; set; }
		public bool IsDBMissmatch { get; set; }
		public bool IsDBMissmatchDuringMonitoring { get; set; }

		public bool IsService { get; set; }

		bool _isInTechnologicalRegime;
		public bool IsInTechnologicalRegime
		{
			get { return _isInTechnologicalRegime; }
			set
			{
				if (_isInTechnologicalRegime != value)
				{
					_isInTechnologicalRegime = value;
					StateBits = new List<XStateBit>() { XStateBit.Norm };
				}
			}
		}

		public DateTime LastDateTime { get; set; }
		public int ZeroHoldDelayCount { get; set; }

		List<XStateBit> _stateBits = new List<XStateBit>();
		public virtual List<XStateBit> StateBits
		{
			get
			{
				if (IsConnectionLost)
					return new List<XStateBit>();
				else
				{
					if (_stateBits == null)
						_stateBits = new List<XStateBit>();
					return _stateBits;
				}
			}
			set
			{
				_stateBits = value;
				if (_stateBits == null)
					_stateBits = new List<XStateBit>();
			}
		}

		public virtual List<XStateClass> StateClasses
		{
			get
			{
				if (IsSuspending)
				{
					return new List<XStateClass>() { XStateClass.Unknown };
				}
				if (IsNoLicense)
				{
					return new List<XStateClass>() { XStateClass.HasNoLicense };
				}
				if (IsConnectionLost)
				{
					return new List<XStateClass>() { XStateClass.ConnectionLost };
				}
				if (IsDBMissmatch || IsDBMissmatchDuringMonitoring)
				{
					return new List<XStateClass>() { XStateClass.DBMissmatch };
				}
				if (IsInTechnologicalRegime)
				{
					return new List<XStateClass>() { XStateClass.TechnologicalRegime };
				}
				if (IsInitialState)
				{
					return new List<XStateClass>() { XStateClass.Unknown };
				}
				return XStatesHelper.StateBitsToStateClasses(StateBits);
			}
		}

		public virtual XStateClass StateClass
		{
			get { return XStatesHelper.GetMinStateClass(StateClasses); }
		}

		public List<XAdditionalState> AdditionalStates { get; set; }
		public int OnDelay { get; set; }
		public int HoldDelay { get; set; }
		public int OffDelay { get; set; }

		public void CopyToXState(XState state)
		{
			state.StateClasses = StateClasses.ToList();
			state.StateClass = StateClass;
			state.AdditionalStates = AdditionalStates.ToList();
			state.OnDelay = OnDelay;
			state.OffDelay = OffDelay;
			state.HoldDelay = HoldDelay;

			if (IsInitialState || IsSuspending || IsNoLicense || IsConnectionLost)
			{
				state.AdditionalStates = new List<XAdditionalState>();
				state.OnDelay = 0;
				state.OffDelay = 0;
				state.HoldDelay = 0;
			}
		}
	}
}