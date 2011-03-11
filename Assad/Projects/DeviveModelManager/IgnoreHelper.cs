using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviveModelManager
{
    public static class IgnoreHelper
    {
        static IgnoreHelper()
        {
            driverIgnorList = new List<string>();
            driverIgnorList.Add("{0695ADC6-4D28-44D4-8E24-7F13D91F62ED}");
            driverIgnorList.Add("{07C5D4D8-19AC-4786-832A-7A81ACCE364C}");
            driverIgnorList.Add("8CE7A914-4FF2-41F2-B991-70E84228D38D");
            driverIgnorList.Add("{FD91CD1A-4F3B-4F76-AA74-AB9C8B9E79F3}");
            driverIgnorList.Add("{F8EBE5F5-A012-4DB7-B300-49552B458931}");
            driverIgnorList.Add("{E613E421-68A2-4A31-96CC-B9CAB7D64216}");
            driverIgnorList.Add("{4F83823A-2C4E-4F4E-BF67-12EFC82B4FEC}");
            driverIgnorList.Add("{AB9C8B4C-43CA-44BB-86DA-527F0D8B2F75}");
            driverIgnorList.Add("50CDD49E-4981-475C-9083-ADB79458B0B0");
            driverIgnorList.Add("75D4399D-EC01-42E0-B77E-31F5E1248905");
            driverIgnorList.Add("{C87E5BBD-2E0C-4213-84D0-2376DB27BDF2}");
            driverIgnorList.Add("ABDE5AF2-2B77-4421-879C-2A14E7F056B2");
            driverIgnorList.Add("1EDE7282-0003-424E-B76C-BB7B413B4F3B");
            driverIgnorList.Add("7CED3D07-C8AF-4141-8D3D-528050EEA72D");
            driverIgnorList.Add("4A3D1FA3-4F13-44D8-B9AD-825B53416A71");
        }

        static List<string> driverIgnorList;

        public static bool IsIgnore(ComServer.Metadata.drvType driver)
        {
            return driverIgnorList.Contains(driver.id);
        }
    }
}
