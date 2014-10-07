using System;
using FiresecAPI.SKD;
using System.Collections.Generic;

namespace Infrastructure
{
	public class SaveService
	{
		public int FSChangesCount { get; private set; }
		bool _fsChanged;
		public bool FSChanged
		{
			get { return _fsChanged; }
			set
			{
				_fsChanged = value;
				OnChanged();
				FSChangesCount++;
			}
		}

		bool _fsParametersChanged;
		public bool FSParametersChanged
		{
			get { return _fsParametersChanged; }
			set
			{
				_fsParametersChanged = value;
				OnChanged();
			}
		}

		bool _gkChanged;
		public bool GKChanged
		{
			get { return _gkChanged; }
			set
			{
				_gkChanged = value;
				OnChanged();
			}
		}

		bool _xLibraryChanged;
		public bool GKLibraryChanged
		{
			get { return _xLibraryChanged; }
			set
			{
				_xLibraryChanged = value;
				OnChanged();
			}
		}

		bool _xinstructionsChanged;
		public bool XInstructionsChanged
		{
			get { return _xinstructionsChanged; }
			set
			{
				_xinstructionsChanged = value;
				OnChanged();
			}
		}

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

		bool _libraryChanged;
		public bool LibraryChanged
		{
			get { return _libraryChanged; }
			set
			{
				_libraryChanged = value;
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

		bool _instructionsChanged;
		public bool InstructionsChanged
		{
			get { return _instructionsChanged; }
			set
			{
				_instructionsChanged = value;
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

		bool _opcChanged;
		public bool OPCChanged
		{
			get { return _opcChanged; }
			set
			{
				_opcChanged = value;
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
				return FSChanged || FSParametersChanged || PlansChanged || FilterChanged || SecurityChanged || SoundsChanged || InstructionsChanged || LibraryChanged || GKLibraryChanged || XInstructionsChanged || GKChanged || CamerasChanged || OPCChanged || EmailsChanged || SKDChanged || SKDLibraryChanged || LayoutsChanged || AutomationChanged;
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
			FSChanged = value;
			FSParametersChanged = value;
			GKChanged = value;
			PlansChanged = value;
			LibraryChanged = value;
			GKLibraryChanged = value;
			XInstructionsChanged = value;
			FilterChanged = value;
			SecurityChanged = value;
			SoundsChanged = value;
			InstructionsChanged = value;
			CamerasChanged = value;
			OPCChanged = value;
			SKDChanged = value;
			SKDLibraryChanged = value;
			EmailsChanged = value;
			LayoutsChanged = value;
			AutomationChanged = value;
			OnChanged();
		}
	}
}