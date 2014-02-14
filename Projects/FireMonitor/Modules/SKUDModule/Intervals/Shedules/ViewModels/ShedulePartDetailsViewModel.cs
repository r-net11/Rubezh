using System;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class ShedulePartDetailsViewModel : SaveCancelDialogViewModel
	{
		bool IsNew;
		public EmployeeShedulePart ShedulePart { get; private set; }

		public ShedulePartDetailsViewModel(EmployeeShedulePart shedulePart = null)
		{
			if (shedulePart == null)
			{
				Title = "Выбор помещения";
				IsNew = true;
				shedulePart = new EmployeeShedulePart();
			}
			else
			{
				Title = "Редактирование помещения";
				IsNew = false;
			}
			ShedulePart = shedulePart;
			IsControl = shedulePart.IsControl;
		}

		bool _isControl;
		public bool IsControl
		{
			get { return _isControl; }
			set
			{
				_isControl = value;
				OnPropertyChanged("IsControl");
			}
		}

		protected override bool Save()
		{
			if (!Validate())
				return false;
			ShedulePart.IsControl = IsControl;
			return true;
		}

		bool Validate()
		{
			
			return true;
		}
	}
}