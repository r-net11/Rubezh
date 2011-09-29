using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportsModule.Models;
using FiresecClient;
using Common;

namespace ReportsModule.Reports
{
    public class ReportDriverCounterDataTable
    {
        public string RdlcFileName = "ReportDriverCounterRDLC.rdlc";
        public string DataTableName = "DataSetDriverCounter";

        public ReportDriverCounterDataTable()
        {
            _driverCounterList = new List<ReportDriverCounterModel>();
        }

        public void Initialize()
        {
            _driverCounterList = new List<ReportDriverCounterModel>();

            foreach (var driver in FiresecManager.Drivers)
            {
                if (driver.IsPlaceable && driver.ShortName != "Компьютер")
                {
                    var devices = FiresecManager.DeviceConfiguration.Devices.FindAll(x => x.Driver.UID == driver.UID);
                    if (devices.IsNotNullOrEmpty())
                    {
                        _driverCounterList.Add(new ReportDriverCounterModel()
                        {
                            DriverName = driver.ShortName,
                            Count = devices.Count
                        });
                    }
                }
            }

            _driverCounterList.Add(new ReportDriverCounterModel()
            {
                DriverName = "Всего устройств",
                Count = CountDrivers()
            });
        }

        int CountDrivers()
        {
            int count = 0;
            foreach (var driverCounter in _driverCounterList)
            {
                count += driverCounter.Count;
            }
            return count;
        }

        List<ReportDriverCounterModel> _driverCounterList;
        public List<ReportDriverCounterModel> DriverCounterList 
        {
            get { return new List<ReportDriverCounterModel>(_driverCounterList); }
        }
    }
}
