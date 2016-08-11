using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class AccessTemplateFilterItemViewModel : OrganisationElementViewModel<AccessTemplateFilterItemViewModel, AccessTemplate>
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