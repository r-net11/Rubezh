using System.Collections.Generic;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public abstract class DetailsBaseViewModel<T> : Infrastructure.Common.DialogContent
    {
        public DetailsBaseViewModel()
        {
            Items = new List<T>();

            OkCommand = new RelayCommand(OnOk, CanOk);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public List<T> Items { get; private set; }

        public T SelectedItem { get; set; }

        public RelayCommand OkCommand { get; private set; }
        void OnOk()
        {
            Close(true);
        }

        bool CanOk()
        {
            return SelectedItem != null;
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}