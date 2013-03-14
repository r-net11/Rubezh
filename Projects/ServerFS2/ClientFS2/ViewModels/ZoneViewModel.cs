﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace ClientFS2.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public Zone Zone { get; private set; }

        public ZoneViewModel(Zone zone)
        {
            Zone = zone;
        }

        public string DetectorCount
        {
            get
            {
                if (Zone.ZoneType == ZoneType.Fire)
                    return Zone.DetectorCount.ToString();
                return null;
            }
        }

        public void Update(Zone zone)
        {
            Zone = zone;
            OnPropertyChanged("Zone");
            OnPropertyChanged("DetectorCount");
        }
    }
}
