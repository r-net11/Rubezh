//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Infrastructure;
//using Infrastructure.Events;

//namespace PlansModule.ViewModels
//{
//    public class SelectedZoneViewModel : BaseViewModel
//    {
//        public SelectedZoneViewModel()
//        {
//            ShowInListCommand = new RelayCommand(OnShowInList);
//        }

//        string name;
//        public string Name
//        {
//            get { return name; }
//            set
//            {
//                name = value;
//                OnPropertyChanged("Name");
//            }
//        }

//        bool isActive;
//        public bool IsActive
//        {
//            get { return isActive; }
//            set
//            {
//                isActive = value;
//                OnPropertyChanged("IsActive");
//            }
//        }

//        public RelayCommand ShowInListCommand { get; private set; }
//        void OnShowInList()
//        {
//            //ServiceFactory.Events.GetEvent<ShowZonesEvent>().Publish(Name);
//        }
//    }
//}
