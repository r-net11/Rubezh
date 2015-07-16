using FiresecAPI.GK;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GKModule.Plans.ViewModels
{
    public class DelayPropertiesViewModel : SaveCancelDialogViewModel
    {
        public DelayPropertiesViewModel(IElementDelay element, DelaysViewModel delaysViewModel)
        {
            _delaysViewModel = delaysViewModel;
            _element = element;
            Title = "Свойства фигуры: ГК Задержка";
            CreateCommand = new RelayCommand(OnCreate);
            EditCommand = new RelayCommand(OnEdit, CanEdit);

            ShowState = element.ShowState;
        }


        bool _showState;
        public bool ShowState
        {
            get { return _showState; }
            set
            {
                _showState = value;
                OnPropertyChanged(() => ShowState);
            }
        }

        bool _showDelay;
        public bool ShowDelay
        {
            get { return _showDelay; }
            set
            {
                _showDelay = value;
                OnPropertyChanged(() => ShowDelay);
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        private void OnCreate()
        {
            Guid delayUID = _element.DelayUID;
            var createDelayEventArg = new CreateGKDirectionEventArg();
            ServiceFactory.Events.GetEvent<CreateGKDirectionEvent>().Publish(createDelayEventArg);
            GKPlanExtension.Instance.Cache.BuildSafe<GKDelay>();
            GKPlanExtension.Instance.SetItem<GKDelay>(_element);
            if (!createDelayEventArg.Cancel)
                Close(true);
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
        }
        bool CanEdit()
        {
            return true;
        }

        protected override bool Save()
        {
            _element.ShowState = ShowState;
            Guid delayUID = _element.DelayUID;
            return base.Save();
        }
        void Update(Guid directionUID)
        {
        }

        #region Fields
        private IElementDelay _element;
        private DelaysViewModel _delaysViewModel;

        #endregion
    }
}
