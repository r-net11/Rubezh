using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : BaseViewModel
    {
        public PlanViewModel(Plan plan)
        {
            Plan = plan;
        }

        public Plan Plan { get; private set; }

        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}
