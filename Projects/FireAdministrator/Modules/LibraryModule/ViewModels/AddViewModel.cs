using System.Collections.ObjectModel;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public abstract class AddViewModel<TViewModel1, TViewModel2> : Infrastructure.Common.DialogContent
    {
        public AddViewModel(TViewModel1 parent)
        {
            Parent = parent;
            OkCommand = new RelayCommand(OnOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public abstract void Initialize();

        public TViewModel1 Parent { get; private set; }

        ObservableCollection<TViewModel2> _items;
        public ObservableCollection<TViewModel2> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        TViewModel2 _selectedItem;
        public TViewModel2 SelectedItem
        {
            get { return _selectedItem; }
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
