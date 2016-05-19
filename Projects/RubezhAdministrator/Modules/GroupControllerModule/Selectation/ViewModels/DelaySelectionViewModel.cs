using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DelaySelectionViewModel : SaveCancelDialogViewModel
	{
		public DelaySelectionViewModel(GKDelay delay)
		{
			Title = "Выбор задержки";
			Delays = new ObservableCollection<DelayViewModel>();
			GKManager.Delays.ForEach(x => Delays.Add(new DelayViewModel(x)));
			if (delay != null)
				SelectedDelay = Delays.FirstOrDefault(x => x.Delay.UID == delay.UID);
			if (SelectedDelay == null)
				SelectedDelay = Delays.FirstOrDefault();
		}

		public ObservableCollection<DelayViewModel> Delays { get; private set; }

		DelayViewModel _selectedDelay;
		public DelayViewModel SelectedDelay
		{
			get { return _selectedDelay; }
			set
			{
				_selectedDelay = value;
				OnPropertyChanged(() => SelectedDelay);
			}
		}
	}
}
