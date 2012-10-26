using System;

namespace Infrastructure
{
	public class SaveService
	{
		bool _fsChanged;
		public bool FSChanged
		{
			get { return _fsChanged; }
			set
			{
				_fsChanged = value;
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
                return FSChanged || PlansChanged || FilterChanged || SecurityChanged || SoundsChanged || InstructionsChanged || LibraryChanged || XLibraryChanged || GKChanged || CamerasChanged || OPCChanged;
            }
        }

		public void Reset()
		{
			FSChanged = false;
			GKChanged = false;
			PlansChanged = false;
			LibraryChanged = false;
		    XLibraryChanged = false;
			FilterChanged = false;
			SecurityChanged = false;
			SoundsChanged = false;
			InstructionsChanged = false;
			CamerasChanged = false;
            OPCChanged = false;
            OnChanged();
		}
	}
}