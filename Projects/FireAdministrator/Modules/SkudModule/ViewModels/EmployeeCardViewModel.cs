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
		public EmployeeCard EmployeeCard { get; private set; }

		public EmployeeCardViewModel(EmployeeCard employeeCard)
		{
		    EmployeeCard = employeeCard;
		}

        public void Update()
        {
            OnPropertyChanged("EmployeeCard");
        }
    }
}