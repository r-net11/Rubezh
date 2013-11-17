using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI.XModels;

namespace XFiresecAPI
{
	public abstract class XBaseState
	{
		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		public XBaseState()
		{
			AdditionalStates = new List<XAdditionalState>();
			AdditionalStateProperties = new List<AdditionalXStateProperty>();
			IsInitialState = true;
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			IsInitialState = false;
			SafeCall(() =>
			{
				if (StateChanged != null)
					StateChanged();
			});
		}

		public bool IsInitialState { get; protected set; }

		protected bool _isGKConnectionLost;
		public bool IsGKConnectionLost
		{
			get { return _isGKConnectionLost; }
			set
			{
				if (_isGKConnectionLost != value)
				{
					_isGKConnectionLost = value;
					OnStateChanged();
				}
			}
		}

		protected bool _isNoLicense;
		public bool IsNoLicense
		{
			get { return _isNoLicense; }
			set
			{
				if (_isNoLicense != value)
				{
					_isNoLicense = value;
					OnStateChanged();
				}
			}
		}

		protected bool _isConnectionLost;
		public bool IsConnectionLost
		{
			get { return _isConnectionLost; }
			set
			{
				if (_isConnectionLost != value)
				{
					_isConnectionLost = value;
					OnStateChanged();
				}
			}
		}

		bool _isGKMissmatch;
		public bool IsGKMissmatch
		{
			get { return _isGKMissmatch; }
			set
			{
				if (_isGKMissmatch != value)
				{
					_isGKMissmatch = value;
					OnStateChanged();
				}
			}
		}

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

		bool _isRealMissmatch;
		public bool IsRealMissmatch
		{
			get { return _isRealMissmatch; }
			set
			{
				if (_isRealMissmatch != value)
				{
					_isRealMissmatch = value;
					OnStateChanged();
				}
			}
		}

		public DateTime LastDateTime { get; set; }
		public abstract List<XStateBit> StateBits { get; set; }

		public virtual List<XStateClass> StateClasses
		{
			get
			{
				if (IsNoLicense)
				{
					return new List<XStateClass>() { XStateClass.HasNoLicense };
				}
				if (IsConnectionLost)
				{
					return new List<XStateClass>() { XStateClass.ConnectionLost };
				}
				if (IsGKMissmatch)
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
		public List<AdditionalXStateProperty> AdditionalStateProperties { get; set; }
		public int OnDelay { get; set; }
		public int HoldDelay { get; set; }
		public int OffDelay { get; set; }
	}
}