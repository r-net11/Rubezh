using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
    public class JournalItemViewModel : BaseViewModel
    {
        readonly JournalRecord _journalRecord;
        string _deviceId;
        string _zoneNo;

        public JournalItemViewModel(JournalRecord journalRecord)
        {
            _journalRecord = journalRecord;
            Initialize();

            ShowPlanCommand = new RelayCommand(OnShowPlan, (o) => { return (string.IsNullOrEmpty(_deviceId) == false); });
            ShowTreeCommand = new RelayCommand(OnShowTree, (o) => { return (string.IsNullOrEmpty(_deviceId) == false); });
            ShowZoneCommand = new RelayCommand(OnShowZone, (o) => { return (string.IsNullOrEmpty(_zoneNo) == false); });
        }

        void Initialize()
        {
            string databaseId = null;
            if (string.IsNullOrEmpty(_journalRecord.PanelDatabaseId) == false)
            {
                databaseId = _journalRecord.PanelDatabaseId;
            }
            if (string.IsNullOrEmpty(_journalRecord.DeviceDatabaseId) == false)
            {
                databaseId = _journalRecord.DeviceDatabaseId;
            }
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == databaseId);
            if (device != null)
            {
                _deviceId = device.Id;
                _zoneNo = device.ZoneNo;
            }
        }

        public int Id
        {
            get
            {
                return _journalRecord.No;
            }
        }

        public string DeviceTime
        {
            get
            {
                return _journalRecord.DeviceTime.ToString();
            }
        }

        public string SystemTime
        {
            get
            {
                return _journalRecord.SystemTime.ToString();
            }
        }

        public string ZoneName
        {
            get
            {
                return _journalRecord.ZoneName;
            }
        }

        public string Description
        {
            get
            {
                return _journalRecord.Description;
            }
        }

        public string Device
        {
            get
            {
                return _journalRecord.DeviceName;
            }
        }

        public string Panel
        {
            get
            {
                return _journalRecord.PanelName;
            }
        }

        public string User
        {
            get
            {
                return _journalRecord.User;
            }
        }

        public StateType StateType
        {
            get
            {
                return _journalRecord.State.StateType;
            }
        }

        public int ClassId
        {
            get
            {
                return _journalRecord.State.Id;
            }
        }

        public RelayCommand ShowPlanCommand { get; private set; }
        void OnShowPlan()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(_deviceId);
        }

        public RelayCommand ShowTreeCommand { get; private set; }
        void OnShowTree()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_deviceId);
        }

        public RelayCommand ShowZoneCommand { get; private set; }
        void OnShowZone()
        {
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(_zoneNo);
        }
    }
}
