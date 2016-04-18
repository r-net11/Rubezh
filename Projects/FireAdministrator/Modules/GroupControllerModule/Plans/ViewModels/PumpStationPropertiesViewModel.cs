using System;
using System.Collections.ObjectModel;
using System.Linq;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKModule.Plans.ViewModels
{
	public class PumpStationPropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementPumpStation _element;
		PumpStationsViewModel _pumpStationsViewModel;

		public PumpStationPropertiesViewModel(IElementPumpStation element, PumpStationsViewModel pumpStationsViewModel)
		{
			_pumpStationsViewModel = pumpStationsViewModel;
			_element = element;
			Title = "Свойства фигуры: Насосная станция";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			PumpStations = new ObservableCollection<PumpStationViewModel>();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				var pumpStationViewModel = new PumpStationViewModel(pumpStation);
				PumpStations.Add(pumpStationViewModel);
			}
			if (_element.PumpStationUID != Guid.Empty)
				SelectedPumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.UID == _element.PumpStationUID);

			ShowState = element.ShowState;
			ShowDelay = element.ShowDelay;
		}

		public ObservableCollection<PumpStationViewModel> PumpStations { get; private set; }

		PumpStationViewModel _selectedPumpStation;
		public PumpStationViewModel SelectedPumpStation
		{
			get { return _selectedPumpStation; }
			set
			{
				_selectedPumpStation = value;
				OnPropertyChanged(() => SelectedPumpStation);
			}
		}

		bool _showState;
		public bool ShowState
		{
			get { return _showState; }
			set
			{
				_showState = value;
				OnPropertyChanged(() => ShowState);
			}
		}

		bool _showDelay;
		public bool ShowDelay
		{
			get { return _showDelay; }
			set
			{
				_showDelay = value;
				OnPropertyChanged(() => ShowDelay);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			Guid pumpStationUID = _element.PumpStationUID;
			var createPumpStationEventArg = new CreateGKPumpStationEventArgs();
			ServiceFactory.Events.GetEvent<CreateGKPumpStationEvent>().Publish(createPumpStationEventArg);
			if (createPumpStationEventArg.PumpStation != null)
				_element.PumpStationUID = createPumpStationEventArg.PumpStation.UID;
			GKPlanExtension.Instance.Cache.BuildSafe<GKPumpStation>();
			GKPlanExtension.Instance.SetItem<GKPumpStation>(_element);
			UpdatePumpStations(pumpStationUID);
			if (!createPumpStationEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKPumpStationEvent>().Publish(SelectedPumpStation.PumpStation.UID);
			SelectedPumpStation.Update();
		}
		bool CanEdit()
		{
			return SelectedPumpStation != null;
		}

		protected override bool Save()
		{
			_element.ShowState = ShowState;
			_element.ShowDelay = ShowDelay;
			Guid pumpStationUID = _element.PumpStationUID;
			GKPlanExtension.Instance.SetItem<GKPumpStation>(_element, SelectedPumpStation == null ? null : SelectedPumpStation.PumpStation);
			UpdatePumpStations(pumpStationUID);
			return base.Save();
		}
		void UpdatePumpStations(Guid pumpStationUID)
		{
			if (_pumpStationsViewModel != null)
			{
				if (pumpStationUID != _element.PumpStationUID)
					Update(pumpStationUID);
				Update(_element.PumpStationUID);
				_pumpStationsViewModel.LockedSelect(_element.PumpStationUID);
			}
		}
		void Update(Guid pumpStationUID)
		{
			var pumpStation = _pumpStationsViewModel.PumpStations.FirstOrDefault(x => x.PumpStation.UID == pumpStationUID);
			if (pumpStation != null)
				pumpStation.Update();
		}
	}
}