using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutomationModule.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using FiresecClient;

namespace AutomationModule.ViewModels
{
    public class ShowDialogStepViewModel : BaseStepViewModel
    {
        public ShowDialogArguments ShowDialogArguments { get; private set; }

        public ShowDialogStepViewModel(StepViewModel stepViewModel)
            : base(stepViewModel)
        {
            ShowDialogArguments = stepViewModel.Step.ShowDialogArguments;
        }

        public bool ForAllClients
        {
            get { return ShowDialogArguments.ForAllClients; }
            set
            {
                ShowDialogArguments.ForAllClients = value;
                OnPropertyChanged(() => ForAllClients);
            }
        }

        public bool IsModal
        {
            get { return ShowDialogArguments.IsModal; }
            set
            {
                ShowDialogArguments.IsModal = value;
                OnPropertyChanged(() => IsModal);
            }
        }

        private ProcedureLayoutCollectionViewModel _procedureLayoutCollectionViewModel;
        public ProcedureLayoutCollectionViewModel ProcedureLayoutCollectionViewModel
        {
            get { return _procedureLayoutCollectionViewModel; }
            private set
            {
                _procedureLayoutCollectionViewModel = value;
                OnPropertyChanged(() => ProcedureLayoutCollectionViewModel);
            }
        }

        public ObservableCollection<LayoutViewModel> Layouts { get; private set; }
        private LayoutViewModel _selectedLayout;
        public LayoutViewModel SelectedLayout
        {
            get { return _selectedLayout; }
            set
            {
                _selectedLayout = value;
                ShowDialogArguments.Layout = SelectedLayout == null ? Guid.Empty : SelectedLayout.Layout.UID;
                OnPropertyChanged(() => SelectedLayout);
            }
        }

        public override string Description
        {
            get
            {
                return string.Format("Открыть диалог: {0} {1}", SelectedLayout == null ? ArgumentViewModel.EmptyText : SelectedLayout.Name, IsModal ? "(модальный)" : "(не модальный)");
            }
        }
        public override void UpdateContent()
        {
            Layouts = new ObservableCollection<LayoutViewModel>(FiresecManager.LayoutsConfiguration.Layouts.Select(item => new LayoutViewModel(item)));
            SelectedLayout = Layouts.FirstOrDefault(x => x.Layout.UID == ShowDialogArguments.Layout);
            ProcedureLayoutCollectionViewModel = new ProcedureLayoutCollectionViewModel(ShowDialogArguments.LayoutFilter);
        }
    }
}
