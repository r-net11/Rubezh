using System.Collections.Generic;
using Common;
using FiresecClient;
using ReportsModule.Models;

namespace ReportsModule.Reports
{
    public class ReportDriverCounter : BaseReportGeneric<ReportDriverCounterModel>
    {
        public ReportDriverCounter()
        {
            base.ReportFileName = "DriverCounterCrystalReport.rpt";
        }

        public override void LoadData()
        {
            DataList = new List<ReportDriverCounterModel>();

            foreach (var driver in FiresecManager.Drivers)
            {
                if (driver.IsPlaceable && driver.ShortName != "Компьютер")
                {
                    var devices = FiresecManager.DeviceConfiguration.Devices.FindAll(x => x.Driver.UID == driver.UID);
                    if (devices.IsNotNullOrEmpty())
                    {
                        DataList.Add(new ReportDriverCounterModel()
                        {
                            DriverName = driver.ShortName,
                            Count = devices.Count
                        });
                    }
                }
            }

            DataList.Add(new ReportDriverCounterModel()
            {
                DriverName = "Всего устройств",
                Count = CountDrivers()
            });
        }

        int CountDrivers()
        {
            int count = 0;

            foreach (var driverCounter in DataList)
            {
                count += driverCounter.Count;
            }

            return count;
        }
    }
}