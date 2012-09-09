using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecAPI.Models;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static FiresecDriver FiresecDriver { get; private set; }
        static FiresecWatcher DriverWatcher;

        public static void InitializeDriverManager()
        {
            FiresecDriver = new FiresecDriver();
            var lastId = FiresecService.FiresecService.GetJournalLastId().Result;
            DriverWatcher = new FiresecWatcher(FiresecDriver.FiresecSerializedClient, true, lastId);
            DriverWatcher.DevicesStateChanged += new Action<List<FiresecAPI.Models.DeviceState>>(OnDevicesStateChanged);
            DriverWatcher.DevicesParametersChanged += new Action<List<DeviceState>>(OnDevicesParametersChanged);
            DriverWatcher.ZonesStateChanged += new Action<List<ZoneState>>(OnZonesStateChanged);
            DriverWatcher.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
            DriverWatcher.Progress += new Action<int, string, int, int>(OnProgress);
        }

        static void OnDevicesStateChanged(List<DeviceState> deviceStates)
        {
            FiresecCallbackService.Current.DeviceStateChanged(deviceStates);
        }

        static void OnDevicesParametersChanged(List<DeviceState> deviceStates)
        {
            FiresecCallbackService.Current.DeviceParametersChanged(deviceStates);
        }

        static void OnZonesStateChanged(List<ZoneState> zoneStates)
        {
            FiresecCallbackService.Current.ZonesStateChanged(zoneStates);
        }

        static void OnNewJournalRecords(List<JournalRecord> journalRecords)
        {
            FiresecCallbackService.Current.NewJournalRecords(journalRecords);
        }

        static void OnProgress(int stage, string comment, int percentComplete, int bytesRW)
        {
            FiresecCallbackService.Current.Progress(stage, comment, percentComplete, bytesRW);
        }
    }
}