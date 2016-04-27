using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class PumpStationPropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementPumpStation IElementPumpStation;

		public PumpStationPropertiesViewModel(IElementPumpStation element)
		{
			IElementPumpStation = element;
			Title = "Свойства фигуры: Насосная станция";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			PumpStations = new ObservableCollection<PumpStationViewModel>();
			foreach (var pumpStation in GKManager.PumpStations)
			{
				var pumpStationViewModel = new PumpStationViewModel(pumpStation);
				PumpStations.Add(pumpStationViewModel);
			}
			if (IElementPumpStation.PumpStationUID != Guid.Empty)
				SelectedPumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.UID == IElementPumpStation.PumpStationUID);

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
			Guid pumpStationUID = IElementPumpStation.PumpStationUID;
			var createPumpStationEventArg = new CreateGKPumpStationEventArgs();
			ServiceFactory.Events.GetEvent<CreateGKPumpStationEvent>().Publish(createPumpStationEventArg);
			if (createPumpStationEventArg.PumpStation != null)
				GKPlanExtension.Instance.RewriteItem(IElementPumpStation, createPumpStationEventArg.PumpStation);
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
			IElementPumpStation.ShowState = ShowState;
			IElementPumpStation.ShowDelay = ShowDelay;
			GKPlanExtension.Instance.RewriteItem(IElementPumpStation, SelectedPumpStation.PumpStation);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedPumpStation != null;
		}
	}
}