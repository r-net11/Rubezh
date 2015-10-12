using System;
using System.Windows;
using RubezhClient;
using RubezhAPI.GK;
using RubezhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace GKSDK
{
	public class ZoneViewModel : BaseViewModel
	{
        public GKZone Zone { get; private set; }
        public GKState State
        {
            get { return Zone.State; }
        }
        public ZoneViewModel(GKZone zone)
		{
            Zone = zone;
			ZoneState = zone.State;
			_stateClass = zone.State.StateClass;
			State.StateChanged += new Action(OnStateChanged);
			No = zone.No;
			Name = zone.Name;
            SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
            ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
            ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
		}

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => ResetFireCommand);
			OnPropertyChanged(() => SetIgnoreCommand);
			OnPropertyChanged(() => ResetIgnoreCommand);
			StateClass = ZoneState.StateClass;
		}

        public GKState ZoneState { get; private set; }
		public int No { get; private set; }
		public string Name { get; private set; }

		XStateClass _stateClass;
		public XStateClass StateClass
		{
			get { return _stateClass; }
			set
			{
				_stateClass = value;
				OnPropertyChanged(() => StateClass);
			}
		}

        public RelayCommand SetIgnoreCommand { get; private set; }
        void OnSetIgnore()
        {
			ClientManager.FiresecService.GKSetIgnoreRegime(Zone);
        }
        bool CanSetIgnore()
        {
			return !State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control);
        }
        public RelayCommand ResetIgnoreCommand { get; private set; }
        void OnResetIgnore()
        {
			ClientManager.FiresecService.GKSetAutomaticRegime(Zone);
        }
        bool CanResetIgnore()
        {
			return State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control);
        }
        public RelayCommand ResetFireCommand { get; private set; }
        void OnResetFire()
        {
			ClientManager.FiresecService.GKReset(Zone);
        }
        bool CanResetFire()
        {
                return State.StateClasses.Contains(XStateClass.Fire2) || State.StateClasses.Contains(XStateClass.Fire1) || State.StateClasses.Contains(XStateClass.Attention);
        }
	}
}