
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class EmployeesFilterItemViewModel : OrganisationElementViewModel<EmployeesFilterItemViewModel, ShortEmployee> 
	{
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
			}
		}
	}
}
