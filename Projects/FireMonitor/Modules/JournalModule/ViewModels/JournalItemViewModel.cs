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
        readonly JournalRecord _journalItem;
        string _deviceId;
        string _zoneNo;

        public JournalItemViewModel(JournalRecord journalItem)
        {
            _journalItem = journalItem;
            Initialize();

            ShowPlanCommand = new RelayCommand(OnShowPlan, (o) => { return (string.IsNullOrEmpty(_deviceId) == false); });
            ShowTreeCommand = new RelayCommand(OnShowTree, (o) => { return (string.IsNullOrEmpty(_deviceId) == false); });
            ShowZoneCommand = new RelayCommand(OnShowZone, (o) => { return (string.IsNullOrEmpty(_zoneNo) == false); });
        }

        void Initialize()
        {
            string databaseId = null;
            if (string.IsNullOrEmpty(_journalItem.PanelDatabaseId) == false)
            {
                databaseId = _journalItem.PanelDatabaseId;
            }
            if (string.IsNullOrEmpty(_journalItem.DeviceDatabaseId) == false)
            {
                databaseId = _journalItem.DeviceDatabaseId;
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
                return _journalItem.No;
            }
        }

        public string DeviceTime
        {
            get
            {
                return _journalItem.DeviceTime.ToString();
            }
        }

        public string SystemTime
        {
            get
            {
                return _journalItem.SystemTime.ToString();
            }
        }

        public string ZoneName
        {
            get
            {
                return _journalItem.ZoneName;
            }
        }

        public string Description
        {
            get
            {
                return _journalItem.Description;
            }
        }

        public string Device
        {
            get
            {
                return _journalItem.DeviceName;
            }
        }

        public string Panel
        {
            get
            {
                return _journalItem.PanelName;
            }
        }

        public string User
        {
            get
            {
                return _journalItem.User;
            }
        }

        public StateType StateType
        {
            get
            {
                return _journalItem.State.StateType;
            }
        }

        public int ClassId
        {
            get
            {
                return _journalItem.State.Id;
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

        DateTime ConvertTime(string firesecTime)
        {
            int year = Convert.ToInt32(firesecTime.Substring(0, 4));
            int month = Convert.ToInt32(firesecTime.Substring(4, 2));
            int day = Convert.ToInt32(firesecTime.Substring(6, 2));
            int hour = Convert.ToInt32(firesecTime.Substring(9, 2));
            int minute = Convert.ToInt32(firesecTime.Substring(12, 2));
            int secunde = Convert.ToInt32(firesecTime.Substring(15, 2));
            DateTime dt = new DateTime(year, month, day, hour, minute, secunde);
            return dt;
        }
    }
}
