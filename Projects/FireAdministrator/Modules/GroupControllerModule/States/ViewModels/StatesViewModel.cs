using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using GKModule.Database;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public class StatesViewModel : DialogViewModel
	{
		public StatesViewModel()
		{
			Title = "Состояния устройств";
			States = new ObservableCollection<StateViewModel>();

			DatabaseProcessor.Convert();
			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				foreach (var binaryObject in gkDatabase.BinaryObjects)
				{
					var rootDevice = gkDatabase.RootDevice;
					var no = binaryObject.GetNo();
					var bytes = SendManager.Send(rootDevice, 2, 12, 68, BytesHelper.ShortToBytes(no));
					if (bytes.Count > 0)
					{
						var stateViewModel = new StateViewModel(bytes);
						States.Add(stateViewModel);
					}
				}
			}

			foreach (var gkDatabase in DatabaseProcessor.DatabaseCollection.GkDatabases)
			{
				foreach (var binaryObject in gkDatabase.BinaryObjects)
				{
					var rootDevice = gkDatabase.RootDevice;
					var no = binaryObject.GetNo();
					var bytes = SendManager.Send(rootDevice, 2, 9, -1, BytesHelper.ShortToBytes(no));
					Trace.WriteLine("Parameter " + no + " : " + BytesHelper.BytesToString(bytes));
				}
			}
		}

		ObservableCollection<StateViewModel> _states;
		public ObservableCollection<StateViewModel> States
		{
			get { return _states; }
			set
			{
				_states = value;
				OnPropertyChanged("States");
			}
		}

		StateViewModel _selectedState;
		public StateViewModel SelectedState
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