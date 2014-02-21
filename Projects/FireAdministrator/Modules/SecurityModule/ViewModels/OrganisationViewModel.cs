using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SecurityModule.ViewModels
{
	public class OrganisationViewModel : BaseViewModel
	{
		public Organization Organisation { get; private set; }

		public OrganisationViewModel(Organization organisation)
		{
			Organisation = organisation;
		}

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}