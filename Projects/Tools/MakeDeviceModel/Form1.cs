using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MakeDeviceModel
{
    public partial class Form1 : Form
    {
        public struct tagDevModel
        {
            public string NameDevice;
            public string Marker;
            public string FileName;
        }

        static public DeviceCollection deviceCollection;
        static string outputFileName = "DeviceModel.xml";
        string outString;
        string inString;
        public Form1()
        {
            InitializeComponent();
            deviceCollection = new DeviceCollection();
            tagDevModel item = new tagDevModel();

// "Пожарный комбинированный извещатель ИП212/101-64-А2R1" id="37F13667-BC77-4742-829B-1C43FA404C1F"
            item.NameDevice = "@Пожарный комбинированный извещатель ИП-212/101";
            item.Marker = "_IP_212_101_USB_";
            item.FileName = "IP_212_101_USB.xml";
            deviceCollection.AddItem(item);

//"Пожарный тепловой извещатель ИП 101-29-A3R1" id="799686B6-9CFA-4848-A0E7-B33149AB940C"
            item.NameDevice = "@Пожарный тепловой извещатель ИП-101";
            item.Marker = "_IP_101_USB_";
            item.FileName = "IP_101_USB.xml";
            deviceCollection.AddItem(item);

//"Пожарный дымовой извещатель ИП 212-64" id="1E045AD6-66F9-4F0B-901C-68C46C89E8DA" 
            item.NameDevice = "@Пожарный дымовой извещатель ИП-212";
            item.Marker = "_IP_212_USB_";
            item.FileName = "IP_212_USB.xml";
            deviceCollection.AddItem(item);

//"Ручной извещатель ИПР513-11" id="641FA899-FAA0-455B-B626-646E5FBE785A"
            item.NameDevice = "@Ручной извещатель ИПР513-11";
            item.Marker = "_IPR_513_USB_";
            item.FileName = "IPR_513_USB.xml";
            deviceCollection.AddItem(item);

//"Пожарная адресная метка АМ1" id="DBA24D99-B7E1-40F3-A7F7-8A47D4433392"
            item.NameDevice = "@Пожарная адресная метка АМ1";
            item.Marker = "_AM_1_USB_";
            item.FileName = "AM_1_USB.xml";
            deviceCollection.AddItem(item);

//"Технологическая адресная метка АМ1-Т"  id="F5A34CE2-322E-4ED9-A75F-FC8660AE33D8"
            item.NameDevice = "@Технологическая адресная метка АМ1-Т";
            item.Marker = "_AM_1_T_USB_";
            item.FileName = "AM_1_T_USB.xml";
            deviceCollection.AddItem(item);

//"Пожарная адресная метка АМП-4            id="D8997F3B-64C4-4037-B176-DE15546CE568""
            item.NameDevice = "@Пожарная адресная метка АМП-4";
            item.Marker = "_AMP_4_USB_";
            item.FileName = "AMP_4_USB.xml";
            deviceCollection.AddItem(item);

//"Технологическая адресная метка АМТ-4   id="C707299B-CAE0-46FD-A68A-4E04755332E4" 
            item.NameDevice = "@Технологическая адресная метка АМТ-4";
            item.Marker = "_AMT_4_USB_";
            item.FileName = "AMT_4_USB.xml";
            deviceCollection.AddItem(item);

//"Кнопка запуска СПТ" 
            item.NameDevice = "@Кнопка запуска СПТ";
            item.Marker = "_KZ_SPT_USB_";
            item.FileName = "KZ_SPT_USB.xml";
            deviceCollection.AddItem(item);

//"Кнопка останова СПТ" 
            item.NameDevice = "@Кнопка останова СПТ";
            item.Marker = "_KO_SPT_USB_";
            item.FileName = "KO_SPT_USB.xml";
            deviceCollection.AddItem(item);

//"Кнопка управления автоматикой" 
            item.NameDevice = "@Кнопка управления автоматикой";
            item.Marker = "_KUA_USB_";
            item.FileName = "KUA_USB.xml";
            deviceCollection.AddItem(item);

//"Задвижка" 
            item.NameDevice = "@Задвижка";
            item.Marker = "_VALVE_USB_";
            item.FileName = "VALVE_USB.xml";
            deviceCollection.AddItem(item);

//"Модуль пожаротушения" id="33A85F87-E34C-45D6-B4CE-A4FB71A36C28"
            item.NameDevice = "@Модуль пожаротушения";
            item.Marker = "_MP_USB_";
            item.FileName = "MP_USB.xml";
            deviceCollection.AddItem(item);


//"Модуль речевого оповещения" id="2D078D43-4D3B-497C-9956-990363D9B19B"
            item.NameDevice = "@Модуль речевого оповещения";
            item.Marker = "_MRO_USB_";
            item.FileName = "MRO_USB.xml";
            deviceCollection.AddItem(item);

//"Модуль Управления Клапанами Дымоудаления" id="44EEDF03-0F4C-4EBA-BD36-28F96BC6B16E"
            item.NameDevice = "@Модуль Управления Клапанами Дымоудаления";
            item.Marker = "_MUKD_USB_";
            item.FileName = "MUKD_USB.xml";
            deviceCollection.AddItem(item);

//"Модуль Управления Клапанами Огнезащиты" id="B603CEBA-A3BF-48A0-BFC8-94BF652FB72A"
            item.NameDevice = "@Модуль Управления Клапанами Огнезащиты";
            item.Marker = "_MUKO_USB_";
            item.FileName = "MUKO_USB.xml";
            deviceCollection.AddItem(item);
        
//"Релейный исполнительный модуль РМ-1" id="4A60242A-572E-41A8-8B87-2FE6B6DC4ACE"
            item.NameDevice = "@Релейный исполнительный модуль РМ-1";
            item.Marker = "_RM_1_USB_";
            item.FileName = "RM_1_USB.xml";
            deviceCollection.AddItem(item);
        
        }


        private void button1_Click(object sender, EventArgs e)
        {
            RTB_Memo.Clear();
            StreamReader inFile = File.OpenText("DeviceModel.xprj");
            StreamWriter outFile;
            inString = inFile.ReadToEnd();
            inFile.Close();
            int count = deviceCollection.ColCount;
            for (int i = 0; i < count; i++)
            {
                tagDevModel item;
                item = deviceCollection.GetElem(i);
                string marker = item.Marker;
                string fname = item.FileName;
                RTB_Memo.Text += "Маркер" + marker + "... ";
                if (inString.Contains(marker) == true)
                {
                    StreamReader extFile = File.OpenText(fname);
                    string extStr = extFile.ReadToEnd();
                    extFile.Close();
                    inString =  inString.Replace(marker, extStr);
                    RTB_Memo.Text += "найден ";
                    RTB_Memo.Text += "  вставка из файла " + fname + "\n";
                }
                else
                {
                    RTB_Memo.Text += " не найден \n";  
                }
            }

            RTB_Memo.Text += "Замена маркеров версии.... \n";
            string api_version = Edit_Version.Text;

            inString = inString.Replace("__VERSION__", api_version);
            inString = inString.Replace("__TEXT_VERSION__", Edit_TextVersion.Text);
           

            RTB_Memo.Text += "Запись результирующей строки в файл DeviceModel.xml \n";
            outFile = File.CreateText("DeviceModel.xml");
            outFile.Write(inString);
            outFile.Close();
            RTB_Memo.Text += "\n\n\n==== ФАЙЛ DeviceModel.xml =========== : \n\n";
            inFile = File.OpenText("DeviceModel.xml");
            inString = inFile.ReadToEnd();
            RTB_Memo.Text += inString;
            inFile.Close();    
    
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        [Serializable]
        public class DeviceCollection
        {
            List<tagDevModel> deviceList;
         
            public DeviceCollection()
            {
             deviceList = new List<tagDevModel>();
            }
           public void AddItem(tagDevModel item)
           {
            deviceList.Add(item);
           }
          public tagDevModel GetElem(int index)
            {
            return deviceList[index];
            }

          public int ColCount
        {
            get
            {
                return deviceList.Count;
            }

        }
        
        
        }

        private void deviceCollectionBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }



    }
}
