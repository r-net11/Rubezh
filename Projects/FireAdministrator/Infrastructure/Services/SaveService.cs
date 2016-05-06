using System;
using StrazhAPI.SKD;
using System.Collections.Generic;

namespace Infrastructure
{
	public class SaveService
	{
		public int FSChangesCount { get; private set; }

		bool _plansChanged;
		public bool PlansChanged
		{
			get { return _plansChanged; }
			set
			{
				_plansChanged = value;
				OnChanged();
			}
		}

		bool _filterChanged;
		public bool FilterChanged
		{
			get { return _filterChanged; }
			set
			{
				_filterChanged = value;
				OnChanged();
			}
		}

		bool _securityChanged;
		public bool SecurityChanged
		{
			get { return _securityChanged; }
			set
			{
				_securityChanged = value;
				OnChanged();
			}
		}

		bool _soundsChanged;
		public bool SoundsChanged
		{
			get { return _soundsChanged; }
			set
			{
				_soundsChanged = value;
				OnChanged();
			}
		}

		bool _camerasChanged;
		public bool CamerasChanged
		{
			get { return _camerasChanged; }
			set
			{
				_camerasChanged = value;
				OnChanged();
			}
		}

		bool _emailsChanged;
		public bool EmailsChanged
		{
			get { return _emailsChanged; }
			set
			{
				_emailsChanged = value;
				OnChanged();
			}
		}

		bool _skdChanged;
		public bool SKDChanged
		{
			get { return _skdChanged; }
			set
			{
				_skdChanged = value;
				OnChanged();
			}
		}

		public void TimeIntervalChanged()
		{
			ClientSettings.SKDMissmatchSettings.MissmatchControllerUIDs = new List<Guid>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.IsController)
				{
					ClientSettings.SKDMissmatchSettings.MissmatchControllerUIDs.Add(device.UID);
				}
			}
		}

		bool _skdLibraryChanged;
		public bool SKDLibraryChanged
		{
			get { return _skdLibraryChanged; }
			set
			{
				_skdLibraryChanged = value;
				OnChanged();
			}
		}

		bool _layoutsChanged;
		public bool LayoutsChanged
		{
			get { return _layoutsChanged; }
			set
			{
				_layoutsChanged = value;
				OnChanged();
			}
		}

		bool _automationChanged;
		public bool AutomationChanged
		{
			get { return _automationChanged; }
			set
			{
				_automationChanged = value;
				OnChanged();
			}
		}

		public event Action Changed;
		void OnChanged()
		{
			if (Changed != null)
				Changed();
		}

		public bool HasChanges
		{
			get
			{
				return PlansChanged || FilterChanged || SecurityChanged || SoundsChanged || CamerasChanged || EmailsChanged || SKDChanged || SKDLibraryChanged || LayoutsChanged || AutomationChanged;
			}
		}

		public void Reset()
		{
			SetAllValues(false);
		}

		public void Set()
		{
			SetAllValues(true);
		}

		void SetAllValues(bool value)
		{
			PlansChanged = value;
			FilterChanged = value;
			SecurityChanged = value;
			SoundsChanged = value;
			CamerasChanged = value;
			SKDChanged = value;
			SKDLibraryChanged = value;
			EmailsChanged = value;
			LayoutsChanged = value;
			AutomationChanged = value;
			OnChanged();
		}
	}
}