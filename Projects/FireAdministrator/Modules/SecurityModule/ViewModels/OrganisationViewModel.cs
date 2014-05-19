using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class OrganisationViewModel : BaseViewModel
	{
		public Organisation Organisation { get; private set; }

		public OrganisationViewModel(Organisation organisation)
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