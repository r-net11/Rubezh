using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using System.Collections.ObjectModel;

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
            ElementSubPlan = subplan;
        }

        public Plan Parent { get; private set; }
        public string Name { get; private set; }
        public ElementSubPlan ElementSubPlan { get; private set; }

        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}