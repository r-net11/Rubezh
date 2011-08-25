using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
//using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class SubPlanViewModel : BaseViewModel
    {
        public SubPlanViewModel()
        {
            //ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        public SubPlanViewModel(Plan plan, ElementSubPlan subplan)
        {
            Parent = plan;
            Name = subplan.Name;

            //ShowPropertiesCommand = new RelayCommand(OnShowProperties);
        }

        public Plan Parent { get; private set; }
        public string Name { get; private set; }

        
        public string PresentationName { get; private set; }
        
        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}