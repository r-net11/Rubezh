using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Windows.Media;

namespace ServiceVisualizer
{
    public class JournalItemViewModel : BaseViewModel
    {
        Firesec.ReadEvents.journalType JournalItem;

        public JournalItemViewModel()
        {
        }

        public void Initialize(Firesec.ReadEvents.journalType journalItem)
        {
            JournalItem = journalItem;
        }

        public string DeviceTime
        {
            get
            {
                return JournalItem.Dt;
            }
        }

        public string SystemTime
        {
            get
            {
                return JournalItem.SysDt;
            }
        }

        public string ZoneName
        {
            get
            {
                return JournalItem.ZoneName;
            }
        }

        public string Description
        {
            get
            {
                return JournalItem.EventDesc;
            }
        }

        public string Device
        {
            get
            {
                return JournalItem.CLC_Device;
            }
        }

        public string Panel
        {
            get
            {
                return JournalItem.CLC_DeviceSource;
            }
        }

        public string User
        {
            get
            {
                return JournalItem.UserInfo;
            }
        }

        public string EventColor
        {
            get
            {
                if (string.IsNullOrEmpty(Panel))
                    return "Transparent";
                return "DarkSeaGreen";
            }
        }
    }
}
