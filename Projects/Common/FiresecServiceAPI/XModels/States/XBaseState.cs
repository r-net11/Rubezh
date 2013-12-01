using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI.XModels;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public abstract class XBaseState
	{
		public XBaseState()
		{
			AdditionalStates = new List<XAdditionalState>();
			IsInitialState = true;
		}

		public event Action InternalStateChanged;
		public void OnInternalStateChanged()
		{
			IsInitialState = false;
			if (InternalStateChanged != null)
				InternalStateChanged();
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
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
					OnInternalStateChanged();
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
					OnInternalStateChanged();
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
					OnInternalStateChanged();
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
					OnInternalStateChanged();
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
					OnInternalStateChanged();
				}
			}
		}

		public DateTime LastDateTime { get; set; }
		public abstract List<XStateBit> StateBits { get; set; }

		public virtual List<XStateClass> InternalStateClasses
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

		public virtual XStateClass InternalStateClass
		{
			get { return XStatesHelper.GetMinStateClass(InternalStateClasses); }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XAdditionalState> AdditionalStates { get; set; }

		[DataMember]
		public int OnDelay { get; set; }

		[DataMember]
		public int HoldDelay { get; set; }

		[DataMember]
		public int OffDelay { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }
	}
}