using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class DriversHelper
    {
        static List<Driver> drivers;
        static DriversHelper()
        {
            drivers = new List<Driver>();
            drivers.Add(new Driver("80A37AF1-B1AD-45D5-A34C-6FA2960F9706", 2, "Виртуальная панель"));
            drivers.Add(new Driver("743CEBD1-B91D-4521-9B02-1E674F94789A", 2, "Виртуальный порт"));
            drivers.Add(new Driver("F8340ECE-C950-498D-88CD-DCBABBC604F3", 0, "Компьютер"));
            drivers.Add(new Driver("{0695ADC6-4D28-44D4-8E24-7F13D91F62ED}", 2, "COM порт (V1)"));
            drivers.Add(new Driver("{07C5D4D8-19AC-4786-832A-7A81ACCE364C}", 2, "Прибор Рубеж 10A"));
            drivers.Add(new Driver("8CE7A914-4FF2-41F2-B991-70E84228D38D", 2, "Прибор Рубеж 2A"));
            drivers.Add(new Driver("{FD91CD1A-4F3B-4F76-AA74-AB9C8B9E79F3}", 2, "Пожарный комбинированный извещатель ИП212&#047;101-64-А2R1"));
            drivers.Add(new Driver("{F8EBE5F5-A012-4DB7-B300-49552B458931}", 2, "Пожарный дымовой извещатель ИП 212-64"));
            drivers.Add(new Driver("{E613E421-68A2-4A31-96CC-B9CAB7D64216}", 2, "Пожарный тепловой извещатель ИП 101-29-A3R1"));
            drivers.Add(new Driver("{4F83823A-2C4E-4F4E-BF67-12EFC82B4FEC}", 2, "Ручной извещатель ИПР514-3"));
            drivers.Add(new Driver("{AB9C8B4C-43CA-44BB-86DA-527F0D8B2F75}", 2, "Пожарная адресная метка АМ1"));
            drivers.Add(new Driver("50CDD49E-4981-475C-9083-ADB79458B0B0", 2, "Метка контроля питания"));
            drivers.Add(new Driver("75D4399D-EC01-42E0-B77E-31F5E1248905", 2, "Релейный исполнительный модуль РМ-1"));
            drivers.Add(new Driver("{C87E5BBD-2E0C-4213-84D0-2376DB27BDF2}", 2, "АСПТ"));
            drivers.Add(new Driver("ABDE5AF2-2B77-4421-879C-2A14E7F056B2", 2, "COM порт (V2)"));
            drivers.Add(new Driver("6298807D-850B-4C65-8792-A4EAB2A4A72A", 0, "Страница"));
            drivers.Add(new Driver("E486745F-6130-4027-9C01-465DE5415BBF", 0, "Индикатор"));
            drivers.Add(new Driver("B476541B-5298-4B3E-A9BA-605B839B1011", 0, "Прибор Рубеж-2AM"));
            drivers.Add(new Driver("02CE2CC4-D71F-4EAA-ACCC-4F2E870F548C", 0, "БУНС"));
            drivers.Add(new Driver("F966D47B-468D-40A5-ACA7-9BE30D0A3847", 0, "Модуль сопряжения МС-3"));
            drivers.Add(new Driver("{868ED643-0ED6-48CD-A0E0-4AD46104C419}", 0, "Модуль сопряжения МС-4"));
            drivers.Add(new Driver("{584BC59A-28D5-430B-90BF-592E40E843A6}", 0, "Модуль доставки сообщений"));
            drivers.Add(new Driver("28A7487A-BA32-486C-9955-E251AF2E9DD4", 0, "Блок индикации"));
            drivers.Add(new Driver("E750EF8F-54C3-4B00-8C72-C7BEC9E59BFC", 0, "Прибор Рубеж-10AM"));
            drivers.Add(new Driver("F3485243-2F60-493B-8A4E-338C61EF6581", 0, "Прибор Рубеж-4A"));
            drivers.Add(new Driver("4A60242A-572E-41A8-8B87-2FE6B6DC4ACE", 0, "Релейный исполнительный модуль РМ-1"));
            drivers.Add(new Driver("33A85F87-E34C-45D6-B4CE-A4FB71A36C28", 0, "Модуль пожаротушения"));
            drivers.Add(new Driver("1E045AD6-66F9-4F0B-901C-68C46C89E8DA", 0, "Пожарный дымовой извещатель ИП 212-64"));
            drivers.Add(new Driver("799686B6-9CFA-4848-A0E7-B33149AB940C", 0, "Пожарный тепловой извещатель ИП 101-29-A3R1"));
            drivers.Add(new Driver("37F13667-BC77-4742-829B-1C43FA404C1F", 0, "Пожарный комбинированный извещатель ИП212//101-64-А2R1"));
            drivers.Add(new Driver("DBA24D99-B7E1-40F3-A7F7-8A47D4433392", 0, "Пожарная адресная метка АМ1"));
            drivers.Add(new Driver("CD7FCB14-F808-415C-A8B7-11C512C275B4", 0, "Кнопка останова СПТ"));
            drivers.Add(new Driver("E8C04507-0C9D-429C-9BBE-166C3ECA4B5C", 0, "Кнопка запуска СПТ"));
            drivers.Add(new Driver("1909EBDF-467D-4565-AD5C-CD5D9084E4C3", 0, "Кнопка управления автоматикой"));
            drivers.Add(new Driver("641FA899-FAA0-455B-B626-646E5FBE785A", 0, "Ручной извещатель ИПР513-11"));
            drivers.Add(new Driver("44EEDF03-0F4C-4EBA-BD36-28F96BC6B16E", 0, "Модуль Управления Клапанами Дымоудаления"));
            drivers.Add(new Driver("B603CEBA-A3BF-48A0-BFC8-94BF652FB72A", 0, "Модуль Управления Клапанами Огнезащиты"));
            drivers.Add(new Driver("AF05094E-4556-4CEE-A3F3-981149264E89", 0, "Насосная Станция"));
            drivers.Add(new Driver("8BFF7596-AEF4-4BEE-9D67-1AE3DC63CA94", 0, "Насос"));
            drivers.Add(new Driver("68E8E353-8CFC-4C54-A1A8-D6B6BF4FD20F", 0, "Жокей-насос"));
            drivers.Add(new Driver("ED58E7EB-BA88-4729-97FF-427EBC822E81", 0, "Компрессор"));
            drivers.Add(new Driver("8AFC9569-9725-4C27-8815-18167642CA29", 0, "Дренажный насос"));
            drivers.Add(new Driver("40DAB36C-2353-4BFD-A1FE-8F542EC15D49", 0, "Насос компенсации утечек"));
            drivers.Add(new Driver("D8997F3B-64C4-4037-B176-DE15546CE568", 0, "Пожарная адресная метка АМП-4"));
            drivers.Add(new Driver("2D078D43-4D3B-497C-9956-990363D9B19B", 0, "Модуль речевого оповещения"));
            drivers.Add(new Driver("4935848F-0084-4151-A0C8-3A900E3CB5C5", 0, "Задвижка"));
            drivers.Add(new Driver("F5A34CE2-322E-4ED9-A75F-FC8660AE33D8", 0, "Технологическая адресная метка АМ1-Т"));
            drivers.Add(new Driver("FD200EDF-94A4-4560-81AA-78C449648D45", 0, "АСПТ"));
            drivers.Add(new Driver("043FBBE0-8733-4C8D-BE0C-E5820DBF7039", 0, "Модуль дымоудаления-1.02//3"));
            drivers.Add(new Driver("05323D14-9070-44B8-B91C-BE024F10E267", 0, "Выход"));
            drivers.Add(new Driver("CD0E9AA0-FD60-48B8-B8D7-F496448FADE6", 0, "USB преобразователь МС-2"));
            drivers.Add(new Driver("FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6", 0, "USB преобразователь МС-1"));
            drivers.Add(new Driver("F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E", 0, "USB Канал МС-2"));
            drivers.Add(new Driver("780DE2E6-8EDD-4CFA-8320-E832EB699544", 0, "USB Канал МС-1"));
            drivers.Add(new Driver("2863E7A3-5122-47F8-BB44-4358450CD0EE", 0, "Канал с резервированием"));
            drivers.Add(new Driver("C2E0F845-D836-4AAE-9894-D5CBE2B9A7DD", 0, "Состав"));
            drivers.Add(new Driver("B9680002-511D-4505-9EF6-0C322E61135F", 0, "USB Канал"));
            drivers.Add(new Driver("1EDE7282-0003-424E-B76C-BB7B413B4F3B", 1, "USB Рубеж-2AM"));
            drivers.Add(new Driver("7CED3D07-C8AF-4141-8D3D-528050EEA72D", 1, "USB Рубеж-4A"));
            drivers.Add(new Driver("4A3D1FA3-4F13-44D8-B9AD-825B53416A71", 1, "USB БУНС"));
            drivers.Add(new Driver("zone", 0, "zone"));
        }

        public static string GetDriverNameById(string driverId)
        {
            return drivers.FirstOrDefault(x => (x.DriverId == driverId) && (x.IgnoreLevel == 0)).Name;
        }

        public static bool IsIgnore(string driverId)
        {
            return (drivers.FirstOrDefault(x => (x.DriverId == driverId)).IgnoreLevel > 0);
        }
    }

    public class Driver
    {
        public Driver(string DriverId, int IgnoreLevel, string Name)
        {
            this.Name = Name;
            this.DriverId = DriverId;
            this.IgnoreLevel = IgnoreLevel;
        }

        public string Name { get; set; }
        public string DriverId { get; set; }
        public int IgnoreLevel { get; set; }
    }
}
