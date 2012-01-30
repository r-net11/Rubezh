using System;
namespace Infrastructure
{
    public class SaveService
    {
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

        bool _devicesChanged;
        public bool DevicesChanged
        {
            get { return _devicesChanged; }
            set
            {
                _devicesChanged = value;
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

        public event Action Changed;
        void OnChanged()
        {
            if (Changed != null)
                Changed();
        }

        public void Reset()
        {
            DevicesChanged = false;
            PlansChanged = false;
            FilterChanged = false;
            SecurityChanged = false;
            SoundsChanged = false;
            InstructionsChanged = false;
            LibraryChanged = false;
        }
    }
}