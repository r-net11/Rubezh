using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public abstract class AddViewModel<ViewModel1, ViewModel2> : Infrastructure.Common.DialogContent
    {
        public AddViewModel(ViewModel1 parent)
        {
            Parent = parent;
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public abstract void Initialize();

        public ViewModel1 Parent { get; private set; }

        ObservableCollection<ViewModel2> _items;
        public ObservableCollection<ViewModel2> Items
        {
            get
            {
                return _items;
            }

            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        ViewModel2 _selectedItem;
        public ViewModel2 SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
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
