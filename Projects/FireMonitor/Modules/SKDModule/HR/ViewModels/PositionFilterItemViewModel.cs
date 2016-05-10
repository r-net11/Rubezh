using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class PositionFilterItemViewModel : OrganisationElementViewModel<PositionFilterItemViewModel, ShortPosition>
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