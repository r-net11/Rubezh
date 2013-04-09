using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace FireMonitor.Views
{
	public partial class AutoActivationView : UserControl, INotifyPropertyChanged
	{
		public AutoActivationView()
		{
			InitializeComponent();
			DataContext = this;

			ChangeAutoActivationCommand = new RelayCommand(OnChangeAutoActivation);
			ChangePlansAutoActivationCommand = new RelayCommand(OnChangePlansAutoActivation);

			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Unsubscribe(OnNewJournalRecord);
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Subscribe(OnNewJournalRecord);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
		}

		void OnUserChanged(bool isReconnect)
		{
			OnPropertyChanged("HasPermission");
		}

		public bool HasPermission
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_ChangeView); }
		}

		public bool IsAutoActivation
		{
			get { return ClientSettings.AutoActivationSettings.IsAutoActivation; }
			set
			{
				ClientSettings.AutoActivationSettings.IsAutoActivation = value;
				OnPropertyChanged("IsAutoActivation");
			}
		}

		public bool IsPlansAutoActivation
		{
			get { return ClientSettings.AutoActivationSettings.IsPlansAutoActivation; }
			set
			{
				ClientSettings.AutoActivationSettings.IsPlansAutoActivation = value;
				OnPropertyChanged("IsPlansAutoActivation");
			}
		}

		public RelayCommand ChangeAutoActivationCommand { get; private set; }
		void OnChangeAutoActivation()
		{
			IsAutoActivation = !IsAutoActivation;
		}

		public RelayCommand ChangePlansAutoActivationCommand { get; private set; }
		void OnChangePlansAutoActivation()
		{
			IsPlansAutoActivation = !IsPlansAutoActivation;
			OnPropertyChanged("IsPlansAutoActivation");
		}

		void OnNewJournalRecord(List<JournalRecord> journalRecords)
		{
			if (IsAutoActivation)
			{
				if ((App.Current.MainWindow != null) && (!App.Current.MainWindow.IsActive))
				{
					App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
					App.Current.MainWindow.Activate();
					App.Current.MainWindow.BringIntoView();
					App.Current.MainWindow.Focus();
					App.Current.MainWindow.Show();
				}
			}
			if (IsPlansAutoActivation)
			{
                foreach (var journalRecord in journalRecords)
                {
                    if (!string.IsNullOrWhiteSpace(journalRecord.DeviceDatabaseId))
                    {
                        var globalStateType = StateType.No;
                        foreach (var device in FiresecManager.Devices)
                        {
                            if (device.DeviceState.StateType < globalStateType)
                                globalStateType = device.DeviceState.StateType;
                        }

                        var journalDevice = FiresecManager.Devices.FirstOrDefault(x => x.DatabaseId == journalRecord.DeviceDatabaseId);
                        if (journalDevice != null)
                        {
                            if (journalDevice.DeviceState.StateType <= globalStateType)
                            {
                                var existsOnPlan = FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementDevices.Any(y => y.DeviceUID == journalDevice.UID); });
                                if (existsOnPlan)
                                {
                                    ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(journalDevice.UID);
                                }
                            }
                        }
                    }
                }
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}