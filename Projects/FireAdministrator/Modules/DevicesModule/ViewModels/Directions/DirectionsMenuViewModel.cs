using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;
using DevicesModule.Events;

namespace DevicesModule.ViewModels
{
    public class DirectionsMenuViewModel
    {
        public DirectionsMenuViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete);
            EditCommand = new RelayCommand(OnEdit);
        }

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            ServiceFactory.Events.GetEvent<RemoveDirectionEvent>().Publish(null);
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            ServiceFactory.Events.GetEvent<AddDirectionEvent>().Publish(null);
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            ServiceFactory.Events.GetEvent<EditDirectionEvent>().Publish(null);
        }
    }
}
