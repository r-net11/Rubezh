using System.Collections.Generic;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public abstract class DetailsBaseViewModel<T> : SaveCancelDialogContent
    {
        public DetailsBaseViewModel()
        {
            Items = new List<T>();
        }

        public List<T> Items { get; private set; }
        public T SelectedItem { get; set; }

        protected override bool CanSave()
        {
            return SelectedItem != null;
        }
    }
}