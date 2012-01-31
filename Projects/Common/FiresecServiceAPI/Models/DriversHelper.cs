using System.Collections.Generic;
using System.Linq;

namespace FiresecAPI.Models
{
    public static class DriversHelper
    {
        public class DriverData
        {
            public DriverData(string DriverId, int IgnoreLevel, string Name, DriverType DriverType)
            {
                this.Name = Name;
                this.DriverId = DriverId;
                this.IgnoreLevel = IgnoreLevel;
                this.DriverType = DriverType;
            }

            public string Name { get; private set; }
            public string DriverId { get; private set; }
            public int IgnoreLevel { get; private set; }
            public DriverType DriverType { get; private set; }
        }

        public static string GetDriverNameById(string driverId)
        {
            return DriverDataList.FirstOrDefault(x => x.DriverId == driverId).Name;
        }

        public static List<DriverData> DriverDataList { get; private set; }

        static DriversHelper()
        {
            DriverDataList = new List<DriverData>();
            DriverDataList.Add(new DriverData("80A37AF1-B1AD-45D5-A34C-6FA2960F9706", 2, "Виртуальная панель", DriverType.OLD_VirtualPanel));
            DriverDataList.Add(new DriverData("743CEBD1-B91D-4521-9B02-1E674F94789A", 2, "Виртуальный порт", DriverType.OLD_VirtualPort));
            DriverDataList.Add(new DriverData("F8340ECE-C950-498D-88CD-DCBABBC604F3", 0, "Компьютер", DriverType.Computer));
            DriverDataList.Add(new DriverData("{0695ADC6-4D28-44D4-8E24-7F13D91F62ED}", 2, "COM порт (V1)", DriverType.OLD_ComPort_V2));
            DriverDataList.Add(new DriverData("{07C5D4D8-19AC-4786-832A-7A81ACCE364C}", 2, "Прибор Рубеж 10A", DriverType.OLD_Rubezh_10A));
            DriverDataList.Add(new DriverData("8CE7A914-4FF2-41F2-B991-70E84228D38D", 2, "Прибор Рубеж 2A", DriverType.OLD_Rubezh_2A));
            DriverDataList.Add(new DriverData("{FD91CD1A-4F3B-4F76-AA74-AB9C8B9E79F3}", 2, "Пожарный комбинированный извещатель ИП212&#047;101-64-А2R1", DriverType.OLD_CombinedDetector));
            DriverDataList.Add(new DriverData("{F8EBE5F5-A012-4DB7-B300-49552B458931}", 2, "Пожарный дымовой извещатель ИП 212-64", DriverType.OLD_SmpkeDetector));
            DriverDataList.Add(new DriverData("{E613E421-68A2-4A31-96CC-B9CAB7D64216}", 2, "Пожарный тепловой извещатель ИП 101-29-A3R1", DriverType.OLD_HeatDetector));
            DriverDataList.Add(new DriverData("{4F83823A-2C4E-4F4E-BF67-12EFC82B4FEC}", 2, "Ручной извещатель ИПР514-3", DriverType.OLD_HandDetector));
            DriverDataList.Add(new DriverData("{AB9C8B4C-43CA-44BB-86DA-527F0D8B2F75}", 2, "Пожарная адресная метка АМ1", DriverType.OLD_AM_1));
            DriverDataList.Add(new DriverData("50CDD49E-4981-475C-9083-ADB79458B0B0", 2, "Метка контроля питания", DriverType.OLD_MKP));
            DriverDataList.Add(new DriverData("75D4399D-EC01-42E0-B77E-31F5E1248905", 2, "Релейный исполнительный модуль РМ-1", DriverType.OLD_RM_1));
            DriverDataList.Add(new DriverData("{C87E5BBD-2E0C-4213-84D0-2376DB27BDF2}", 2, "АСПТ", DriverType.OLD_ASPT));
            DriverDataList.Add(new DriverData("ABDE5AF2-2B77-4421-879C-2A14E7F056B2", 2, "COM порт (V2)", DriverType.OLD_ComPort_V1));
            DriverDataList.Add(new DriverData("6298807D-850B-4C65-8792-A4EAB2A4A72A", 0, "Страница", DriverType.Page));
            DriverDataList.Add(new DriverData("E486745F-6130-4027-9C01-465DE5415BBF", 0, "Индикатор", DriverType.Indicator));
            DriverDataList.Add(new DriverData("7C4B1A3E-BC00-4542-9AB7-061D2AE92BA2", 0, "Направление", DriverType.Direction));
            DriverDataList.Add(new DriverData("B476541B-5298-4B3E-A9BA-605B839B1011", 0, "Прибор Рубеж-2AM", DriverType.Rubezh_2AM));
            DriverDataList.Add(new DriverData("02CE2CC4-D71F-4EAA-ACCC-4F2E870F548C", 0, "БУНС", DriverType.BUNS));
            DriverDataList.Add(new DriverData("A7BB2FD0-0088-49AE-8C04-7D6FA22C79D6", 0, "БУНС-2", DriverType.BUNS_2));
            DriverDataList.Add(new DriverData("F966D47B-468D-40A5-ACA7-9BE30D0A3847", 0, "Модуль сопряжения МС-3", DriverType.MS_3));
            DriverDataList.Add(new DriverData("{868ED643-0ED6-48CD-A0E0-4AD46104C419}", 0, "Модуль сопряжения МС-4", DriverType.MS_4));
            DriverDataList.Add(new DriverData("{584BC59A-28D5-430B-90BF-592E40E843A6}", 0, "Устройство оконечное объектовое", DriverType.UOO_TL));
            DriverDataList.Add(new DriverData("28A7487A-BA32-486C-9955-E251AF2E9DD4", 0, "Блок индикации", DriverType.IndicationBlock));
            DriverDataList.Add(new DriverData("E750EF8F-54C3-4B00-8C72-C7BEC9E59BFC", 0, "Прибор Рубеж-10AM", DriverType.Rubezh_10AM));
            DriverDataList.Add(new DriverData("F3485243-2F60-493B-8A4E-338C61EF6581", 0, "Прибор Рубеж-4A", DriverType.Rubezh_4A));
            DriverDataList.Add(new DriverData("96CDBD7E-29F6-45D4-9028-CF10332FAB1A", 0, "Прибор Рубеж-2ОП", DriverType.Rubezh_2OP));
            DriverDataList.Add(new DriverData("B1DF571E-8786-4987-94B2-EC91F7578D20", 0, "Пульт дистанционного управления", DriverType.PDU));
            DriverDataList.Add(new DriverData("4A60242A-572E-41A8-8B87-2FE6B6DC4ACE", 0, "Релейный исполнительный модуль РМ-1", DriverType.RM_1));
            DriverDataList.Add(new DriverData("EA5F5372-C76C-4E92-B879-0AFA0EE979C7", 0, "Релейный исполнительный модуль РМ-2", DriverType.RM_2));
            DriverDataList.Add(new DriverData("15E38FA6-DC41-454B-83E5-D7789064B2E1", 0, "Релейный исполнительный модуль РМ-3", DriverType.RM_3));
            DriverDataList.Add(new DriverData("3CB0E7FB-670F-4F32-8123-4B310AEE1DB8", 0, "Релейный исполнительный модуль РМ-4", DriverType.RM_4));
            DriverDataList.Add(new DriverData("A7C09BA8-DD00-484C-8BEA-245F2920DFBB", 0, "Релейный исполнительный модуль РМ-5", DriverType.RM_5));
            DriverDataList.Add(new DriverData("33A85F87-E34C-45D6-B4CE-A4FB71A36C28", 0, "Модуль пожаротушения", DriverType.MPT));
            DriverDataList.Add(new DriverData("1E045AD6-66F9-4F0B-901C-68C46C89E8DA", 0, "Пожарный дымовой извещатель ИП 212-64", DriverType.SmokeDetector));
            DriverDataList.Add(new DriverData("799686B6-9CFA-4848-A0E7-B33149AB940C", 0, "Пожарный тепловой извещатель ИП 101-29-A3R1", DriverType.HeatDetector));
            DriverDataList.Add(new DriverData("37F13667-BC77-4742-829B-1C43FA404C1F", 0, "Пожарный комбинированный извещатель ИП212//101-64-А2R1", DriverType.CombinedDetector));
            DriverDataList.Add(new DriverData("DBA24D99-B7E1-40F3-A7F7-8A47D4433392", 0, "Пожарная адресная метка АМ1", DriverType.AM_1));
            DriverDataList.Add(new DriverData("CD7FCB14-F808-415C-A8B7-11C512C275B4", 0, "Кнопка останова СПТ", DriverType.StopButton));
            DriverDataList.Add(new DriverData("E8C04507-0C9D-429C-9BBE-166C3ECA4B5C", 0, "Кнопка запуска СПТ", DriverType.StartButton));
            DriverDataList.Add(new DriverData("1909EBDF-467D-4565-AD5C-CD5D9084E4C3", 0, "Кнопка управления автоматикой", DriverType.AutomaticButton));
            DriverDataList.Add(new DriverData("2F875F0C-54AA-47CE-B639-FE5E3ED9841B", 0, "Кнопка вкл автоматики ШУЗ и насосов в направлении", DriverType.ShuzOnButton));
            DriverDataList.Add(new DriverData("032CDF7B-6787-4612-B3D1-03E0D3FD2F53", 0, "Кнопка выкл автоматики ШУЗ и насосов в направлении", DriverType.ShuzOffButton));
            DriverDataList.Add(new DriverData("935B0020-889B-4A94-9563-EC0E4127E8E3", 1, "Кнопка разблокировки автоматики ШУЗ в направлении", DriverType.ShuzUnblockButton));
            DriverDataList.Add(new DriverData("641FA899-FAA0-455B-B626-646E5FBE785A", 0, "Ручной извещатель ИПР513-11", DriverType.HandDetector));
            DriverDataList.Add(new DriverData("EFCA74B2-AD85-4C30-8DE8-8115CC6DFDD2", 0, "Охранная адресная метка АМ1-О", DriverType.AM1_O));
            DriverDataList.Add(new DriverData("E495C37A-A414-4B47-AF24-FEC1F9E43D86", 0, "Адресная метка  АМ4", DriverType.AM4));
            DriverDataList.Add(new DriverData("29F67E91-AD29-410C-B473-EFD341AF1D79", 0, "Технологическая адресная метка АМ4-Т", DriverType.AM4_T));
            DriverDataList.Add(new DriverData("A15D9258-D5B5-4A81-A60A-3C9A308FB528", 0, "Пожарная адресная метка АМ4-П", DriverType.AM4_P));
            DriverDataList.Add(new DriverData("1B6D6509-DEF0-42B2-B31C-F8383040BF18", 0, "Охранная адресная метка АМ4-О", DriverType.AM4_O));
            DriverDataList.Add(new DriverData("44EEDF03-0F4C-4EBA-BD36-28F96BC6B16E", 0, "Модуль Управления Клапанами Дымоудаления", DriverType.MUKD));
            DriverDataList.Add(new DriverData("B603CEBA-A3BF-48A0-BFC8-94BF652FB72A", 0, "Модуль Управления Клапанами Огнезащиты", DriverType.MUKO));
            DriverDataList.Add(new DriverData("AF05094E-4556-4CEE-A3F3-981149264E89", 0, "Насосная Станция", DriverType.PumpStation));
            DriverDataList.Add(new DriverData("8BFF7596-AEF4-4BEE-9D67-1AE3DC63CA94", 0, "Насос", DriverType.Pump));
            DriverDataList.Add(new DriverData("68E8E353-8CFC-4C54-A1A8-D6B6BF4FD20F", 0, "Жокей-насос", DriverType.JokeyPump));
            DriverDataList.Add(new DriverData("ED58E7EB-BA88-4729-97FF-427EBC822E81", 0, "Компрессор", DriverType.Compressor));
            DriverDataList.Add(new DriverData("8AFC9569-9725-4C27-8815-18167642CA29", 0, "Дренажный насос", DriverType.DrenazhPump));
            DriverDataList.Add(new DriverData("40DAB36C-2353-4BFD-A1FE-8F542EC15D49", 0, "Насос компенсации утечек", DriverType.CompensationPump));
            DriverDataList.Add(new DriverData("D8997F3B-64C4-4037-B176-DE15546CE568", 0, "Пожарная адресная метка АМП-4", DriverType.AMP_4));
            DriverDataList.Add(new DriverData("2D078D43-4D3B-497C-9956-990363D9B19B", 0, "Модуль речевого оповещения", DriverType.MRO));
            DriverDataList.Add(new DriverData("4935848F-0084-4151-A0C8-3A900E3CB5C5", 0, "Задвижка", DriverType.Valve));
            DriverDataList.Add(new DriverData("F5A34CE2-322E-4ED9-A75F-FC8660AE33D8", 0, "Технологическая адресная метка АМ1-Т", DriverType.AM1_T));
            DriverDataList.Add(new DriverData("C707299B-CAE0-46FD-A68A-4E04755332E4", 2, "Технологическая адресная метка АМТ-4", DriverType.AMT_4));
            DriverDataList.Add(new DriverData("3C4B8739-A1F4-4241-A760-C6B906A19BF0", 2, "Прибор пожарный управления", DriverType.PPU));
            DriverDataList.Add(new DriverData("FD200EDF-94A4-4560-81AA-78C449648D45", 0, "АСПТ", DriverType.ASPT));
            DriverDataList.Add(new DriverData("043FBBE0-8733-4C8D-BE0C-E5820DBF7039", 0, "Модуль дымоудаления-1.02//3", DriverType.MDU));
            DriverDataList.Add(new DriverData("05323D14-9070-44B8-B91C-BE024F10E267", 0, "Выход", DriverType.Exit));
            DriverDataList.Add(new DriverData("AB3EF7B1-68AD-4A1B-88A8-997357C3FC5B", 0, "Модуль радиоканала МРК-30", DriverType.MRK_30));
            DriverDataList.Add(new DriverData("D57CDEF3-ACBC-4773-955E-22A1F016D025", 0, "Ручной радиоканальный извещатель ИПР513-11", DriverType.RadioHandDetector));
            DriverDataList.Add(new DriverData("CFD407D1-5D19-43EC-9650-A86EC4422EC6", 0, "Пожарный дымовой радиоканальный извещатель ИП 212-64Р", DriverType.RadioSmokeDetector));
            DriverDataList.Add(new DriverData("CD0E9AA0-FD60-48B8-B8D7-F496448FADE6", 0, "USB преобразователь МС-2", DriverType.MS_1));
            DriverDataList.Add(new DriverData("FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6", 0, "USB преобразователь МС-1", DriverType.MS_2));
            DriverDataList.Add(new DriverData("F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E", 0, "USB Канал МС-2", DriverType.USB_Channel_2));
            DriverDataList.Add(new DriverData("780DE2E6-8EDD-4CFA-8320-E832EB699544", 0, "USB Канал МС-1", DriverType.USB_Channel_1));
            DriverDataList.Add(new DriverData("2863E7A3-5122-47F8-BB44-4358450CD0EE", 0, "Канал с резервированием", DriverType.ReserveChannel));
            DriverDataList.Add(new DriverData("C2E0F845-D836-4AAE-9894-D5CBE2B9A7DD", 0, "Состав", DriverType.Sostav));
            DriverDataList.Add(new DriverData("B9680002-511D-4505-9EF6-0C322E61135F", 0, "USB Канал", DriverType.USB_Channel));
            DriverDataList.Add(new DriverData("1EDE7282-0003-424E-B76C-BB7B413B4F3B", 1, "USB Рубеж-2AM", DriverType.USB_Rubezh_2AM));
            DriverDataList.Add(new DriverData("7CED3D07-C8AF-4141-8D3D-528050EEA72D", 1, "USB Рубеж-4A", DriverType.USB_Rubezh_4A));
            DriverDataList.Add(new DriverData("39DBC715-C4B5-4AE6-A809-4F214BBBD6C1", 1, "USB Рубеж-2ОП", DriverType.USB_Rubezh_2OP));
            DriverDataList.Add(new DriverData("4A3D1FA3-4F13-44D8-B9AD-825B53416A71", 1, "USB БУНС", DriverType.USB_BUNS));
            DriverDataList.Add(new DriverData("64CB0AB4-D9BE-4C71-94A1-CF24406DAF92", 1, "USB БУНС-2", DriverType.USB_BUNS_2));
            //DriverDataList.Add(new DriverData("zone", 0, "zone"));
            //DriverDataList.Add(new DriverData("monitor", 0, "monitor"));
        }

        public static List<DriverType> PanelDrivers
        {
            get
            {
                var panelDrivers = new List<DriverType>();
                panelDrivers.Add(DriverType.BUNS);
                panelDrivers.Add(DriverType.BUNS_2);
                panelDrivers.Add(DriverType.Rubezh_2AM);
                panelDrivers.Add(DriverType.Rubezh_2OP);
                panelDrivers.Add(DriverType.Rubezh_4A);
                return panelDrivers;
            }
        }

        public static List<DriverType> UsbPanelDrivers
        {
            get
            {
                var usbPanelDrivers = new List<DriverType>();
                usbPanelDrivers.Add(DriverType.USB_BUNS);
                usbPanelDrivers.Add(DriverType.USB_BUNS_2);
                usbPanelDrivers.Add(DriverType.USB_Rubezh_2AM);
                usbPanelDrivers.Add(DriverType.USB_Rubezh_2OP);
                usbPanelDrivers.Add(DriverType.USB_Rubezh_4A);
                return usbPanelDrivers;
            }
        }
    }
}