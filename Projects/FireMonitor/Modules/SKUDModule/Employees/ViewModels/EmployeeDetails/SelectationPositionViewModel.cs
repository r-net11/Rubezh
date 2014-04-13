
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
namespace SKDModule.ViewModels
{
	public class SelectationPositionViewModel : BaseViewModel
	{
		public Position Position { get; private set; }

		public SelectationPositionViewModel(Position position)
		{
			Position = position;
		}

		public string Name { get { return Position.Name; } }
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
