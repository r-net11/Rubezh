using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;
using DevicesModule.Events;

namespace DevicesModule.ViewModels
{
    public class ZonesMenuViewModel
    {
        public ZonesMenuViewModel(RelayCommand addCommand, RelayCommand deleteCommand, RelayCommand editCommand)
        {
            AddCommand = addCommand;
            DeleteCommand = deleteCommand;
            EditCommand = editCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
    }
}
