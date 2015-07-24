﻿using System;
using System.Windows;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKSDK
{
	public class DirectionViewModel : BaseViewModel
	{
        public GKDirection Direction { get; private set; }
        public GKState State
        {
            get { return Direction.State; }
        }
        public DirectionViewModel(GKDirection direction)
		{
            Direction = direction;
			DirectionState = direction.State;
            _stateClass = direction.State.StateClass;
            direction.State.StateChanged += new Action(OnStateChanged);
            No = direction.State.Direction.No;
            Name = direction.State.Direction.Name;

            SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState, CanSetAutomaticState);
            SetManualStateCommand = new RelayCommand(OnSetManualState, CanSetManualState);
            SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState, CanSetIgnoreState);
            TurnOnCommand = new RelayCommand(OnTurnOn);
            TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
            TurnOffCommand = new RelayCommand(OnTurnOff);
            ForbidStartCommand = new RelayCommand(OnForbidStart);


		}
        public DeviceControlRegime ControlRegime
        {
            get
            {
                if (State.StateClasses.Contains(XStateClass.Ignore))
                    return DeviceControlRegime.Ignore;

                if (!State.StateClasses.Contains(XStateClass.AutoOff))
                    return DeviceControlRegime.Automatic;

                return DeviceControlRegime.Manual;
            }
        }
        public bool IsControlRegime
        {
            get { return ControlRegime == DeviceControlRegime.Manual; }
        }
		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		void OnStateChanged()
		{
			StateClass = DirectionState.StateClass;
		}

        public GKState DirectionState { get; private set; }
		public int No { get; private set; }
		public string Name { get; private set; }

		XStateClass _stateClass;
		public XStateClass StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
				OnPropertyChanged(()=>StateClass);
			}
		}
        public RelayCommand SetAutomaticStateCommand { get; private set; }
        void OnSetAutomaticState()
        {
                FiresecManager.FiresecService.GKSetAutomaticRegime(Direction);
        }
        bool CanSetAutomaticState()
        {
            return ControlRegime != DeviceControlRegime.Automatic;
        }
        public RelayCommand SetManualStateCommand { get; private set; }
        void OnSetManualState()
        {
                FiresecManager.FiresecService.GKSetManualRegime(Direction);
        }
        bool CanSetManualState()
        {
            return ControlRegime != DeviceControlRegime.Manual;
        }
        public RelayCommand SetIgnoreStateCommand { get; private set; }
        void OnSetIgnoreState()
        {
                FiresecManager.FiresecService.GKSetIgnoreRegime(Direction);
        }
        bool CanSetIgnoreState()
        {
            return ControlRegime != DeviceControlRegime.Ignore;
        }
        public RelayCommand TurnOnCommand { get; private set; }
        void OnTurnOn()
        {
                FiresecManager.FiresecService.GKTurnOn(Direction);
        }
        public RelayCommand TurnOnNowCommand { get; private set; }
        void OnTurnOnNow()
        {
                FiresecManager.FiresecService.GKTurnOnNow(Direction);
        }
        public RelayCommand TurnOffCommand { get; private set; }
        void OnTurnOff()
        {
                FiresecManager.FiresecService.GKTurnOff(Direction);
        }
        public RelayCommand ForbidStartCommand { get; private set; }
        void OnForbidStart()
        {
                FiresecManager.FiresecService.GKStop(Direction);
        }

	}
}