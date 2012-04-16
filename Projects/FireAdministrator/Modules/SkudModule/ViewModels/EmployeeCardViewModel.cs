using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using FiresecAPI.Models.Skud;

namespace SkudModule.ViewModels
{
    public class EmployeeCardViewModel : BaseViewModel
    {
		public EmployeeCardIndex EmployeeCard { get; private set; }

		public EmployeeCardViewModel(EmployeeCardIndex employeeCard)
		{
		    EmployeeCard = employeeCard;
		}

        public void Update()
        {
            OnPropertyChanged("EmployeeCard");
        }
    }
}