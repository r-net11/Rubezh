using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Events;
using FiresecClient.Models;
using ServiceAPI.Models;

namespace JournalModule.ViewModels
{
    public class JournalItemViewModel : BaseViewModel
    {
        JournalItem _journalItem;
        string _deviceId;
        string _zoneNo;

        public JournalItemViewModel(JournalItem journalItem)
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
            var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.DatabaseId == databaseId);
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

        public DateTime DeviceTime
        {
            get
            {
                return _journalItem.DeviceTime;
            }
        }

        public DateTime SystemTime
        {
            get
            {
                return _journalItem.SystemTime;
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

        public string State
        {
            get
            {
                return _journalItem.State.StateType.ToString();
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
