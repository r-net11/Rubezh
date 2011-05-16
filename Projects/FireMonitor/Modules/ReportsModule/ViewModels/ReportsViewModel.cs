using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ReportsModule.ViewModels
{
    public class ReportsViewModel : RegionViewModel
    {
        public ReportsViewModel()
        {
            PrintCommand = new RelayCommand(OnPrint);
        }

        public void Initialize()
        {
            DriverCounters = new ObservableCollection<DriverCounter>();

            foreach (var driver in FiresecManager.CurrentConfiguration.Metadata.drv)
            {
                if ((driver.options != null) && (driver.options.Contains("Placeable")) && (driver.shortName != "Компьютер"))
                {
                    var devices = FiresecManager.CurrentConfiguration.AllDevices.FindAll(x => x.DriverId == driver.id);
                    if (devices.Count > 0)
                    {
                        DriverCounter driverCounter = new DriverCounter();
                        driverCounter.DriverName = driver.shortName;
                        driverCounter.Count = devices.Count;
                        DriverCounters.Add(driverCounter);
                    }
                }
            }
        }

        public RelayCommand PrintCommand {get; private set;}
        void OnPrint()
        {
        }

        ObservableCollection<DriverCounter> _driverCounters;
        public ObservableCollection<DriverCounter> DriverCounters
        {
            get { return _driverCounters; }
            set
            {
                _driverCounters = value;
                OnPropertyChanged("DriverCounters");
            }
        }

        public override void Dispose()
        {
        }
    }

    public class DriverCounter
    {
        public string DriverName { get; set; }
        public int Count { get; set; }
    }
}
