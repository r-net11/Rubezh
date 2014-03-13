//using System.Collections.ObjectModel;
//using FiresecAPI;
//using Infrastructure.Common;
//using Infrastructure.Common.TreeList;
//using Infrastructure.Common.Windows;
//using System.Linq;
//using FiresecClient.SKDHelpers;
//using System.Collections.Generic;
//using System;
//using FiresecClient;
//using Infrastructure.Common.Windows.ViewModels;

//namespace SKDModule.ViewModels
//{
//    public class UserAccessViewModel : BaseViewModel // TreeNodeViewModel<ZoneViewModel>
//    {
//        public Employee Employee { get; private set; }

//        public UserAccessViewModel(Employee employee)
//        {
//            Employee = employee;
//            AddCardCommand = new RelayCommand(OnAddCard, CanAddCard);
//            ChangeIsExpandedCommand = new RelayCommand(OnChangeIsExpanded);

//            var filter = new CardFilter{ EmployeeUIDs = new List<Guid>() { Employee.UID } };
//            Cards = new ObservableCollection<EmployeeCardViewModel>();
//            var cards = CardHelper.Get(filter);
//            if (cards != null)
//            {
//                foreach (var item in cards)
//                    Cards.Add(new EmployeeCardViewModel(this, item));
//            }
//            SelectedCard = Cards.FirstOrDefault();

//            _isExpanded = false;
//        }

//        bool _isExpanded;
//        public bool IsExpanded
//        {
//            get { return _isExpanded; }
//            set
//            {
//                _isExpanded = !_isExpanded;
//                OnPropertyChanged("IsExpanded");
//            }
//        }

//        public RelayCommand ChangeIsExpandedCommand { get; private set; }
//        void OnChangeIsExpanded()
//        {
//            IsExpanded = !IsExpanded;
//        }

//        public ObservableCollection<EmployeeCardViewModel> Cards { get; private set; }

//        EmployeeCardViewModel _selectedCard;
//        public EmployeeCardViewModel SelectedCard
//        {
//            get { return _selectedCard; }
//            set
//            {
//                _selectedCard = value;
//                OnPropertyChanged("SelectedCard");
//            }
//        }

//        public RelayCommand AddCardCommand { get; private set; }
//        void OnAddCard()
//        {
//            var cardDetailsViewModel = new EmployeeCardDetailsViewModel();
//            if (DialogService.ShowModalWindow(cardDetailsViewModel))
//            {
//                var card = cardDetailsViewModel.Card;
//                card.HolderUID = Employee.UID;
//                var saveResult = CardHelper.Save(card);
//                if (!saveResult)
//                    return;
//                var cardViewModel = new EmployeeCardViewModel(this, card);
//                Cards.Add(cardViewModel);
//                SelectedCard = cardViewModel;
//            }
//        }
//        public bool CanAddCard()
//        { 
//            return Cards.Count < 10;
//        }

//        public void RemoveCard(EmployeeCardViewModel cardViewModel)
//        {
//            var cardRemovalReasonViewModel = new CardRemovalReasonViewModel();
//            if (DialogService.ShowModalWindow(cardRemovalReasonViewModel))
//            {
//                var card = cardViewModel.Card;
//                var cardRemovalReason = cardRemovalReasonViewModel.CardRemovalReason;
//                var toStopListResult =  CardHelper.ToStopList(card, cardRemovalReason);
//                if (!toStopListResult)
//                    return;
//                Cards.Remove(cardViewModel);
//                SelectedCard = Cards.FirstOrDefault();
//            }
//        }
//        public bool CanRemoveCard()
//        {
//            return SelectedCard != null;
//        }
//    } 
//}