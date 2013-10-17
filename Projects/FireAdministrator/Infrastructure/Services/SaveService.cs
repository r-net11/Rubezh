using System;

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
		public bool XLibraryChanged
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
		 
		public event Action Changed;
		private void OnChanged()
		{
			if (Changed != null)
				Changed();
		}

		public bool HasChanges
		{
			get
			{
				return FSChanged || FSParametersChanged || PlansChanged || FilterChanged || SecurityChanged || SoundsChanged || InstructionsChanged || LibraryChanged || XLibraryChanged || XInstructionsChanged || GKChanged || CamerasChanged || OPCChanged || EmailsChanged || LayoutsChanged;
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
			XLibraryChanged = value;
			XInstructionsChanged = value;
			FilterChanged = value;
			SecurityChanged = value;
			SoundsChanged = value;
			InstructionsChanged = value;
			CamerasChanged = value;
			OPCChanged = value;
			EmailsChanged = value;
			LayoutsChanged = value;
			OnChanged();
		}
	}
}