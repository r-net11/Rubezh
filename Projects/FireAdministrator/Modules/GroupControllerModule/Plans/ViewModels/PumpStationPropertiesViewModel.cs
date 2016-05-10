using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class PumpStationPropertiesViewModel : SaveCancelDialogViewModel
	{
		const int _sensivityFactor = 100;
		IElementPumpStation IElementPumpStation;
		ElementBaseRectangle ElementBaseRectangle { get; set; }
		public bool CanEditPosition { get; private set; }
		public PumpStationPropertiesViewModel(IElementPumpStation element)
		{
			IElementPumpStation = element;
			ElementBaseRectangle = element as ElementBaseRectangle;
			CanEditPosition = ElementBaseRectangle != null;
			if (CanEditPosition)
			{
				Left = (int)(ElementBaseRectangle.Left * _sensivityFactor);
				Top = (int)(ElementBaseRectangle.Top * _sensivityFactor);
			}
			Title = "Свойства фигуры: Насосная станция";
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			PumpStations = new ObservableCollection<GKPumpStation>(GKManager.PumpStations);
			if (IElementPumpStation.PumpStationUID != Guid.Empty)
				SelectedPumpStation = PumpStations.FirstOrDefault(x => x.UID == IElementPumpStation.PumpStationUID);

			ShowState = element.ShowState;
			ShowDelay = element.ShowDelay;
		}

		int _left;
		public int Left
		{
			get { return _left; }
			set
			{
				_left = value;
				OnPropertyChanged(() => Left);
			}
		}
		int _top;
		public int Top
		{
			get { return _top; }
			set
			{
				_top = value;
				OnPropertyChanged(() => Top);
			}
		}

		public ObservableCollection<GKPumpStation> PumpStations { get; private set; }

		GKPumpStation _selectedPumpStation;
		public GKPumpStation SelectedPumpStation
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
			ServiceFactory.Events.GetEvent<EditGKPumpStationEvent>().Publish(SelectedPumpStation.UID);
			PumpStations = new ObservableCollection<GKPumpStation>(GKManager.PumpStations);
			OnPropertyChanged(() => PumpStations);
		}
		bool CanEdit()
		{
			return SelectedPumpStation != null;
		}

		protected override bool Save()
		{
			IElementPumpStation.ShowState = ShowState;
			IElementPumpStation.ShowDelay = ShowDelay;
			ElementBaseRectangle.Left = (double)Left / _sensivityFactor;
			ElementBaseRectangle.Top = (double)Top / _sensivityFactor;
			GKPlanExtension.Instance.RewriteItem(IElementPumpStation, SelectedPumpStation);
			return base.Save();
		}
		protected override bool CanSave()
		{
			return SelectedPumpStation != null;
		}
	}
}