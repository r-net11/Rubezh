using System.Collections.ObjectModel;
using Commom.GK;
using Common.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class StatesViewModel : DialogViewModel
	{
		public StatesViewModel()
		{
			Title = "Состояния устройств";
			States = new ObservableCollection<BinaryDeviceState>();

			DatabaseProcessor.Convert();
			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				GetStatesFromDB(gkDatabase);
			}

			foreach (var kauDatabase in DatabaseProcessor.DatabaseCollection.KauDatabases)
			{
				GetStatesFromDB(kauDatabase);
			}
		}

		void GetStatesFromDB(CommonDatabase commonDatabase)
		{
			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				var rootDevice = commonDatabase.RootDevice;
				var no = binaryObject.GetNo();
				var bytes = SendManager.Send(rootDevice, 2, 12, 68, BytesHelper.ShortToBytes(no));
				if (bytes.Count > 0)
				{
					var binaryDeviceState = new BinaryDeviceState(bytes, binaryObject.DatabaseType);
					States.Add(binaryDeviceState);
				}
			}
		}

		ObservableCollection<BinaryDeviceState> _states;
		public ObservableCollection<BinaryDeviceState> States
		{
			get { return _states; }
			set
			{
				_states = value;
				OnPropertyChanged("States");
			}
		}

		BinaryDeviceState _selectedState;
		public BinaryDeviceState SelectedState
		{
			get { return _selectedState; }
			set
			{
				_selectedState = value;
				OnPropertyChanged("SelectedState");
			}
		}
	}
}