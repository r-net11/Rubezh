using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI;
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
			AdditionalStates = new List<string>();
			AdditionalStateProperties = new List<AdditionalXStateProperty>();
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			SafeCall(() =>
			{
				if (StateChanged != null)
					StateChanged();
			});
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
					StateBits = new List<XStateBit>() { XStateBit.Norm };
					_isInTechnologicalRegime = value;
					OnStateChanged();
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
			get { return XStatesHelper.StateBitsToStateClasses(StateBits, IsConnectionLost, IsGKMissmatch, IsInTechnologicalRegime, IsNoLicense); }
		}

		public virtual XStateClass StateClass
		{
			get { return XStatesHelper.GetMinStateClass(StateClasses); }
		}

		List<string> _additionalStates;
		public List<string> AdditionalStates
		{
			get { return _additionalStates; }
			set
			{
				_additionalStates = value;
				OnStateChanged();
			}
		}

		List<AdditionalXStateProperty> _additionalStateProperties;
		public List<AdditionalXStateProperty> AdditionalStateProperties
		{
			get { return _additionalStateProperties; }
			set
			{
				_additionalStateProperties = value;
				OnStateChanged();
			}
		}

		int _onDelay;
		public int OnDelay
		{
			get { return _onDelay; }
			set
			{
				_onDelay = value;
				OnStateChanged();
			}
		}

		int _holdDelay;
		public int HoldDelay
		{
			get { return _holdDelay; }
			set
			{
				_holdDelay = value;
				OnStateChanged();
			}
		}

		int _offDelay;
		public int OffDelay
		{
			get { return _offDelay; }
			set
			{
				_offDelay = value;
				OnStateChanged();
			}
		}
	}
}