using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class SelectationPositionViewModel : BaseViewModel
	{
		public ShortPosition Position { get; private set; }

		public SelectationPositionViewModel(ShortPosition position)
		{
			Position = position;
		}

		public string Name { get { return Position.Name; } }
		public string Description { get { return Position.Description; } }

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
