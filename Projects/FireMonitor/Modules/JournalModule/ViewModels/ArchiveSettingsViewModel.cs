using System;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ArchiveSettingsViewModel : DialogContent
    {
        public ArchiveSettingsViewModel(ArchiveDefaultStateType archiveDefaultStateType)
        {
            ArchiveDefaultStates = new ObservableCollection<ArchiveDefaultStateViewModel>();
            foreach (ArchiveDefaultStateType item in Enum.GetValues(typeof(ArchiveDefaultStateType)))
            {
                if (item == archiveDefaultStateType)
                    ArchiveDefaultStates.Add(new ArchiveDefaultStateViewModel(item) { IsActive = true });
                else
                    ArchiveDefaultStates.Add(new ArchiveDefaultStateViewModel(item) { IsActive = false });
            }

            Initialize();
        }

        void Initialize()
        {
            Title = "Настройки архива";

            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public ObservableCollection<ArchiveDefaultStateViewModel> ArchiveDefaultStates { get; private set; }

        ArchiveDefaultStateViewModel _selectedArchiveDefaultState;
        public ArchiveDefaultStateViewModel SelectedArchiveDefaultState
        {
            get { return _selectedArchiveDefaultState; }
            set
            {
                _selectedArchiveDefaultState = value;
                OnPropertyChanged("SelectedArchiveDefaultState");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}