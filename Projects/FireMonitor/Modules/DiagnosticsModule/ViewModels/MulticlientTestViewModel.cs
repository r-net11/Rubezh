using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace DiagnosticsModule.ViewModels
{
	public class MulticlientTestViewModel : BaseViewModel
	{
		public MulticlientTestViewModel()
		{
			ChangeStateCommand = new RelayCommand(OnChangeState);
			StateTypes = Enum.GetValues(typeof(StateType)).Cast<StateType>().ToList(); ;
		}

		public RelayCommand ChangeStateCommand { get; private set; }
		void OnChangeState()
		{
			ServiceFactory.Events.GetEvent<MulticlientStateChangedEvent>().Publish(SelectedState);
		}

		public List<StateType> StateTypes { get; private set; }

		StateType _selectedState;
		public StateType SelectedState
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