using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class FiresecService : IFiresecService
    {
        static FiresecService()
        {
            callbacks = new List<IFiresecCallback>();
        }

        IFiresecCallback currentCallbacks;
        static List<IFiresecCallback> callbacks;

        public static void OnNewJournalItem(JournalItem journalItem)
        {
            foreach (IFiresecCallback callback in callbacks)
            {
                try
                {
                    if (callback != null)
                    {
                        callback.NewJournalItem(journalItem);
                    }
                }
                catch
                {
                    //callback = null;
                    //callbacks.Remove(callback);
                }
            }
        }

        public static void OnDeviceStateChanged(string deviceId)
        {
            foreach (IFiresecCallback callback in callbacks)
            {
                try
                {
                    if (callback != null)
                    {
                        callback.DeviceStateChanged(deviceId);
                    }
                }
                catch
                {
                    //callback = null;
                    //callbacks.Remove(callback);
                }
            }
        }

        public static void OnDeviceParametersChanged(string deviceId)
        {
            foreach (IFiresecCallback callback in callbacks)
            {
                try
                {
                    if (callback != null)
                    {
                        callback.DeviceParametersChanged(deviceId);
                    }
                }
                catch
                {
                    //callback = null;
                    //callbacks.Remove(callback);
                }
            }
        }

        public static void OnZoneStateChanged(string zoneNo)
        {
            foreach (IFiresecCallback callback in callbacks)
            {
                try
                {
                    if (callback != null)
                    {
                        callback.ZoneStateChanged(zoneNo);
                    }
                }
                catch
                {
                    //callback = null;
                    //callbacks.Remove(callback);
                }
            }
        }
        public void Connect()
        {
            currentCallbacks = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            callbacks.Add(currentCallbacks);
        }

        public void Disconnect()
        {
            callbacks.Remove(currentCallbacks);
        }

        public CurrentConfiguration GetConfiguration()
        {
            return FiresecManager.Configuration;
        }

        public CurrentStates GetStates()
        {
            return FiresecManager.States;
        }

        public void SetConfiguration(CurrentConfiguration currentConfiguration)
        {
            FiresecManager.SetNewConfig();
        }

        public void WriteConfiguration(string devicePath)
        {
            FiresecInternalClient.DeviceWriteConfig(FiresecManager.CoreConfig, devicePath);
        }

        public List<JournalItem> ReadJournal(int startIndex, int count)
        {
            var internalJournal = FiresecInternalClient.ReadEvents(startIndex, count);

            List<JournalItem> journalItems = new List<JournalItem>();
            if ((internalJournal != null) && (internalJournal.Journal != null))
            {
                foreach (var innerJournalItem in internalJournal.Journal)
                {
                    var journalItem = JournalConverter.Convert(innerJournalItem);
                    journalItems.Add(journalItem);
                }
            }

            return journalItems;
        }

        public void AddToIgnoreList(List<string> devicePaths)
        {
            FiresecInternalClient.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<string> devicePaths)
        {
            FiresecInternalClient.RemoveFromIgnoreList(devicePaths);
        }

        public void ResetStates(List<ResetItem> resetItems)
        {
            FiresecResetHelper.ResetMany(resetItems);
        }

        public void AddUserMessage(string message)
        {
            FiresecInternalClient.AddUserMessage(message);
        }

        public void ExecuteCommand(string devicePath, string methodName)
        {
            FiresecInternalClient.ExecuteCommand(devicePath, methodName);
        }

        public Stream GetFile()
        {
            string filePath = @"D:\xxx.txt";

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File was not found", Path.GetFileName(filePath));

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
