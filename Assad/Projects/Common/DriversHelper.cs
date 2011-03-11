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
            drivers.Add(new Driver() { Name = "Компьютер", DriverId = "F8340ECE-C950-498D-88CD-DCBABBC604F3" });
            //drivers.Add(new Driver() { Name = "COM порт (V1)", DriverId = "&#123;0695ADC6-4D28-44D4-8E24-7F13D91F62ED&#125;" });
            //drivers.Add(new Driver() { Name = "Прибор Рубеж 10A", DriverId = "&#123;07C5D4D8-19AC-4786-832A-7A81ACCE364C&#125;" });
            //drivers.Add(new Driver() { Name = "Прибор Рубеж 2A", DriverId = "8CE7A914-4FF2-41F2-B991-70E84228D38D" });
            //drivers.Add(new Driver() { Name = "Пожарный комбинированный извещатель ИП212&#047;101-64-А2R1", DriverId = "&#123;FD91CD1A-4F3B-4F76-AA74-AB9C8B9E79F3&#125;" });
            //drivers.Add(new Driver() { Name = "Пожарный дымовой извещатель ИП 212-64", DriverId = "&#123;F8EBE5F5-A012-4DB7-B300-49552B458931&#125;" });
            //drivers.Add(new Driver() { Name = "Пожарный тепловой извещатель ИП 101-29-A3R1", DriverId = "&#123;E613E421-68A2-4A31-96CC-B9CAB7D64216&#125;" });
            //drivers.Add(new Driver() { Name = "Ручной извещатель ИПР514-3", DriverId = "&#123;4F83823A-2C4E-4F4E-BF67-12EFC82B4FEC&#125;" });
            //drivers.Add(new Driver() { Name = "Пожарная адресная метка АМ1", DriverId = "&#123;AB9C8B4C-43CA-44BB-86DA-527F0D8B2F75&#125;" });
            //drivers.Add(new Driver() { Name = "Метка контроля питания", DriverId = "50CDD49E-4981-475C-9083-ADB79458B0B0" });
            //drivers.Add(new Driver() { Name = "Релейный исполнительный модуль РМ-1", DriverId = "75D4399D-EC01-42E0-B77E-31F5E1248905" });
            //drivers.Add(new Driver() { Name = "АСПТ", DriverId = "&#123;C87E5BBD-2E0C-4213-84D0-2376DB27BDF2&#125;" });
            //drivers.Add(new Driver() { Name = "COM порт (V2)", DriverId = "ABDE5AF2-2B77-4421-879C-2A14E7F056B2" });
            drivers.Add(new Driver() { Name = "Страница", DriverId = "6298807D-850B-4C65-8792-A4EAB2A4A72A" });
            drivers.Add(new Driver() { Name = "Индикатор", DriverId = "E486745F-6130-4027-9C01-465DE5415BBF" });
            drivers.Add(new Driver() { Name = "Прибор Рубеж-2AM", DriverId = "B476541B-5298-4B3E-A9BA-605B839B1011" });
            drivers.Add(new Driver() { Name = "БУНС", DriverId = "02CE2CC4-D71F-4EAA-ACCC-4F2E870F548C" });
            drivers.Add(new Driver() { Name = "Модуль сопряжения МС-3", DriverId = "F966D47B-468D-40A5-ACA7-9BE30D0A3847" });
            drivers.Add(new Driver() { Name = "Модуль сопряжения МС-4", DriverId = "&#123;868ED643-0ED6-48CD-A0E0-4AD46104C419&#125;" });
            drivers.Add(new Driver() { Name = "Блок индикации", DriverId = "28A7487A-BA32-486C-9955-E251AF2E9DD4" });
            drivers.Add(new Driver() { Name = "Прибор Рубеж-10AM", DriverId = "E750EF8F-54C3-4B00-8C72-C7BEC9E59BFC" });
            drivers.Add(new Driver() { Name = "Прибор Рубеж-4A", DriverId = "F3485243-2F60-493B-8A4E-338C61EF6581" });
            drivers.Add(new Driver() { Name = "Релейный исполнительный модуль РМ-1", DriverId = "4A60242A-572E-41A8-8B87-2FE6B6DC4ACE" });
            drivers.Add(new Driver() { Name = "Модуль пожаротушения", DriverId = "33A85F87-E34C-45D6-B4CE-A4FB71A36C28" });
            drivers.Add(new Driver() { Name = "Пожарный дымовой извещатель ИП 212-64", DriverId = "1E045AD6-66F9-4F0B-901C-68C46C89E8DA" });
            drivers.Add(new Driver() { Name = "Пожарный тепловой извещатель ИП 101-29-A3R1", DriverId = "799686B6-9CFA-4848-A0E7-B33149AB940C" });
            drivers.Add(new Driver() { Name = "Пожарный комбинированный извещатель ИП212//101-64-А2R1", DriverId = "37F13667-BC77-4742-829B-1C43FA404C1F" });
            drivers.Add(new Driver() { Name = "Пожарная адресная метка АМ1", DriverId = "DBA24D99-B7E1-40F3-A7F7-8A47D4433392" });
            drivers.Add(new Driver() { Name = "Кнопка останова СПТ", DriverId = "CD7FCB14-F808-415C-A8B7-11C512C275B4" });
            drivers.Add(new Driver() { Name = "Кнопка запуска СПТ", DriverId = "E8C04507-0C9D-429C-9BBE-166C3ECA4B5C" });
            drivers.Add(new Driver() { Name = "Кнопка управления автоматикой", DriverId = "1909EBDF-467D-4565-AD5C-CD5D9084E4C3" });
            drivers.Add(new Driver() { Name = "Ручной извещатель ИПР513-11", DriverId = "641FA899-FAA0-455B-B626-646E5FBE785A" });
            drivers.Add(new Driver() { Name = "Модуль Управления Клапанами Дымоудаления", DriverId = "44EEDF03-0F4C-4EBA-BD36-28F96BC6B16E" });
            drivers.Add(new Driver() { Name = "Модуль Управления Клапанами Огнезащиты", DriverId = "B603CEBA-A3BF-48A0-BFC8-94BF652FB72A" });
            drivers.Add(new Driver() { Name = "Насосная Станция", DriverId = "AF05094E-4556-4CEE-A3F3-981149264E89" });
            drivers.Add(new Driver() { Name = "Насос", DriverId = "8BFF7596-AEF4-4BEE-9D67-1AE3DC63CA94" });
            drivers.Add(new Driver() { Name = "Жокей-насос", DriverId = "68E8E353-8CFC-4C54-A1A8-D6B6BF4FD20F" });
            drivers.Add(new Driver() { Name = "Компрессор", DriverId = "ED58E7EB-BA88-4729-97FF-427EBC822E81" });
            drivers.Add(new Driver() { Name = "Дренажный насос", DriverId = "8AFC9569-9725-4C27-8815-18167642CA29" });
            drivers.Add(new Driver() { Name = "Насос компенсации утечек", DriverId = "40DAB36C-2353-4BFD-A1FE-8F542EC15D49" });
            drivers.Add(new Driver() { Name = "Пожарная адресная метка АМП-4", DriverId = "D8997F3B-64C4-4037-B176-DE15546CE568" });
            drivers.Add(new Driver() { Name = "Модуль речевого оповещения", DriverId = "2D078D43-4D3B-497C-9956-990363D9B19B" });
            drivers.Add(new Driver() { Name = "Задвижка", DriverId = "4935848F-0084-4151-A0C8-3A900E3CB5C5" });
            drivers.Add(new Driver() { Name = "Технологическая адресная метка АМ1-Т", DriverId = "F5A34CE2-322E-4ED9-A75F-FC8660AE33D8" });
            drivers.Add(new Driver() { Name = "АСПТ", DriverId = "FD200EDF-94A4-4560-81AA-78C449648D45" });
            drivers.Add(new Driver() { Name = "Модуль дымоудаления-1.02//3", DriverId = "043FBBE0-8733-4C8D-BE0C-E5820DBF7039" });
            drivers.Add(new Driver() { Name = "USB преобразователь МС-2", DriverId = "CD0E9AA0-FD60-48B8-B8D7-F496448FADE6" });
            drivers.Add(new Driver() { Name = "USB преобразователь МС-1", DriverId = "FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6" });
            drivers.Add(new Driver() { Name = "USB Канал МС-2", DriverId = "F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E" });
            drivers.Add(new Driver() { Name = "USB Канал МС-1", DriverId = "780DE2E6-8EDD-4CFA-8320-E832EB699544" });
            drivers.Add(new Driver() { Name = "Канал с резервированием", DriverId = "2863E7A3-5122-47F8-BB44-4358450CD0EE" });
            drivers.Add(new Driver() { Name = "Состав", DriverId = "C2E0F845-D836-4AAE-9894-D5CBE2B9A7DD" });
            drivers.Add(new Driver() { Name = "USB Канал", DriverId = "B9680002-511D-4505-9EF6-0C322E61135F" });
            //drivers.Add(new Driver() { Name = "USB Рубеж-2AM", DriverId = "1EDE7282-0003-424E-B76C-BB7B413B4F3B" });
            //drivers.Add(new Driver() { Name = "USB Рубеж-4A", DriverId = "7CED3D07-C8AF-4141-8D3D-528050EEA72D" });
            //drivers.Add(new Driver() { Name = "USB БУНС", DriverId = "4A3D1FA3-4F13-44D8-B9AD-825B53416A71" });
            drivers.Add(new Driver() { Name = "zone", DriverId = "zone" });
        }

        public static string GetDriverNameById(string driverId)
        {
            return drivers.FirstOrDefault(x => x.DriverId == driverId).Name;
        }
    }

    public class Driver
    {
        public string Name { get; set; }
        public string DriverId { get; set; }
    }
}
