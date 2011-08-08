using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace JournalModule.ViewModels
{
    public class JournalRecordViewModel : BaseViewModel
    {
        readonly JournalRecord _journalRecord;
        readonly FiresecAPI.Models.Device _device;

        public JournalRecordViewModel(JournalRecord journalRecord)
        {
            _journalRecord = journalRecord;
            if (string.IsNullOrEmpty(_journalRecord.PanelDatabaseId) == false)
            {
                _device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(
                    x => x.DatabaseId == _journalRecord.PanelDatabaseId);
            }
            else if (string.IsNullOrEmpty(_journalRecord.DeviceDatabaseId) == false)
            {
                _device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(
                    x => x.DatabaseId == _journalRecord.DeviceDatabaseId);
            }

            Initialize();
        }

        void Initialize()
        {
            ShowPlanCommand = new RelayCommand(OnShowPlan, (o) => _device != null);
            ShowTreeCommand = new RelayCommand(OnShowTree, (o) => _device != null);
            ShowZoneCommand = new RelayCommand(OnShowZone, (o) => _device != null);
        }

        public int Id
        {
            get { return _journalRecord.No; }
        }

        public string DeviceTime
        {
            get { return _journalRecord.DeviceTime.ToString(); }
        }

        public string SystemTime
        {
            get { return _journalRecord.SystemTime.ToString(); }
        }

        public string ZoneName
        {
            get { return _journalRecord.ZoneName; }
        }

        public string Description
        {
            get { return _journalRecord.Description; }
        }

        public string Device
        {
            get { return _journalRecord.DeviceName; }
        }

        public string Panel
        {
            get { return _journalRecord.PanelName; }
        }

        public string User
        {
            get { return _journalRecord.User; }
        }

        public StateType StateType
        {
            get { return _journalRecord.State.StateType; }
        }

        public int ClassId
        {
            get { return _journalRecord.State.Id; }
        }

        public RelayCommand ShowPlanCommand { get; private set; }
        void OnShowPlan()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(_device.Id);
        }

        public RelayCommand ShowTreeCommand { get; private set; }
        void OnShowTree()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(_device.Id);
        }

        public RelayCommand ShowZoneCommand { get; private set; }
        void OnShowZone()
        {
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(_device.ZoneNo);
        }
    }
}
