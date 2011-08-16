using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class GuardLevelsSelectationViewModel : DialogContent
    {
        public GuardLevelsSelectationViewModel()
        {
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
            Title = "Выбор уровней доступа";
        }

        public void Initialize(GuardUser guardUser)
        {
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
