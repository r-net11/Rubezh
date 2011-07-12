using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Events;
using Firesec;
using FiresecClient.Models;

namespace JournalModule.ViewModels
{
    public class JournalItemViewModel : BaseViewModel
    {
        Firesec.ReadEvents.journalType _journalItem;
        string _deviceId;
        string _zoneNo;

        public JournalItemViewModel(Firesec.ReadEvents.journalType journalItem)
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
            if (string.IsNullOrEmpty(_journalItem.IDDevicesSource) == false)
            {
                databaseId = _journalItem.IDDevicesSource;
            }
            if (string.IsNullOrEmpty(_journalItem.IDDevices) == false)
            {
                databaseId = _journalItem.IDDevices;
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
                return Convert.ToInt32(_journalItem.IDEvents);
            }
        }

        public DateTime DeviceTime
        {
            get
            {
                return ConvertTime(_journalItem.Dt);
            }
        }

        public DateTime SystemTime
        {
            get
            {
                return ConvertTime(_journalItem.SysDt);
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
                return _journalItem.EventDesc;
            }
        }

        public string Device
        {
            get
            {
                return _journalItem.CLC_Device;
            }
        }

        public string Panel
        {
            get
            {
                return _journalItem.CLC_DeviceSource;
            }
        }

        public string User
        {
            get
            {
                return _journalItem.UserInfo;
            }
        }

        public StateType StateType
        {
            get
            {
                int id = Convert.ToInt32(_journalItem.IDTypeEvents);
                return new State(id).StateType;
            }
        }

        public string State
        {
            get
            {
                int id = Convert.ToInt32(_journalItem.IDTypeEvents);
                return new State(id).ToString();
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
