using System;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Diagnostics;
using Infrastructure;
using Infrastructure.Events;
using FiresecClient;
using Infrustructure.Plans.Elements;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class DelayDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public XDelay Delay { get; private set; }
		public XDelayState DelayState { get; private set; }
		public DelayViewModel DelayViewModel { get; private set; }

		public DelayDetailsViewModel(XDelay delay)
		{
			Delay = delay;
			DelayState = Delay.DelayState;
			DelayViewModel = new DelayViewModel(DelayState);
			DelayState.StateChanged += new Action(OnStateChanged);

			ShowCommand = new RelayCommand(OnShow);
			SetAutomaticStateCommand = new RelayCommand(OnSetAutomaticState);
			SetManualStateCommand = new RelayCommand(OnSetManualState);
			SetIgnoreStateCommand = new RelayCommand(OnSetIgnoreState);
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);

			Title = Delay.Name;
			TopMost = true;
		}

		void OnStateChanged()
		{
			OnPropertyChanged("StateClasses");
			OnPropertyChanged("ControlRegime");
			OnPropertyChanged("IsControlRegime");
			OnPropertyChanged("DelayState");
			OnPropertyChanged("HasOnDelay");
			OnPropertyChanged("HasHoldDelay");
		}

		public List<XStateClass> StateClasses
		{
			get
			{
				var stateClasses = DelayState.StateClasses.ToList();
				stateClasses.Sort();
				return stateClasses;
			}
		}

		public DeviceControlRegime ControlRegime
		{
			get
			{
				if (DelayState.StateClasses.Contains(XStateClass.Ignore))
					return DeviceControlRegime.Ignore;

				if (!DelayState.StateClasses.Contains(XStateClass.AutoOff))
					return DeviceControlRegime.Automatic;

				return DeviceControlRegime.Manual;
			}
		}

		public bool IsControlRegime
		{
			get { return ControlRegime == DeviceControlRegime.Manual; }
		}

		public RelayCommand SetAutomaticStateCommand { get; private set; }
		void OnSetAutomaticState()
		{
			ObjectCommandSendHelper.SetAutomaticRegime(Delay);
		}

		public RelayCommand SetManualStateCommand { get; private set; }
		void OnSetManualState()
		{
			ObjectCommandSendHelper.SetManualRegime(Delay);
		}

		public RelayCommand SetIgnoreStateCommand { get; private set; }
		void OnSetIgnoreState()
		{
			ObjectCommandSendHelper.SetIgnoreRegime(Delay);
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			ObjectCommandSendHelper.TurnOn(Delay);
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			ObjectCommandSendHelper.TurnOnNow(Delay);
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			ObjectCommandSendHelper.TurnOff(Delay);
		}

		public bool HasOnDelay
		{
			get { return DelayState.StateClasses.Contains(XStateClass.TurningOn) && DelayState.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return DelayState.StateClasses.Contains(XStateClass.On) && DelayState.HoldDelay > 0; }
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowXDelayEvent>().Publish(Delay.UID);
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }
		public bool HasPlans
		{
			get { return Plans.Count > 0; }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Delay.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			DelayState.StateChanged -= new Action(OnStateChanged);
		}
	}
}