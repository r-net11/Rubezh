using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionsKeys
    {
        public InstructionsKeys(RelayCommand addCommand, RelayCommand editCommand,
            RelayCommand deleteCommand, RelayCommand deleteAllCommand)
        {
            AddCommand = addCommand;
            DeleteCommand = deleteCommand;
            EditCommand = editCommand;
            DeleteAllCommand = deleteAllCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand DeleteAllCommand { get; private set; }
    }
}
