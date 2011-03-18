using System;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
//using TestServiceClient.Window1;
using Common;
using ServiceApi;
using ClientApi;

namespace RubezhAX
{


    public class ViewModel : INotifyPropertyChanged
    {

        public AXPropertyPage form;
        UCRubezh parentForm;
        public static ServiceClient controllerAPI;

        ObservableCollection<MetaDataID> mDataID;
        public ObservableCollection<MetaDataID> MDataID
        {
            get { return mDataID; }
        }


        public bool goMethodAPI()
        {
            form.MyMetadataDriverID = mDataID[0].StrID;

            form.ChoiceIDDevice.SelectedItem = 0;

            controllerAPI.Start();


            //            devicesAPI =  figuration;

            List<DeviceDescriptor> innerdevices = new List<DeviceDescriptor>();
            foreach (Device dev in ClientApi.ServiceClient.Configuration.Devices)
            {
                DeviceDescriptor innerdevice = new DeviceDescriptor();
                innerdevice.DriverId = dev.DriverId;
                innerdevice.Address = dev.Address;
                innerdevice.DeviceName = dev.DriverName;
                innerdevice.LastEvents = dev.LastEvents;
                innerdevice.State = dev.State;
                if (dev.States != null)
                {
                    if (dev.States.Count != 0)
                    {
                        innerdevice.States = dev.States.ToList();
                    }

                }
                innerdevice.Zones = dev.Zones;
                innerdevice.Path = dev.Path;
                if (innerdevice.DriverId == form.MyMetadataDriverID)
                    innerdevice.Enable = true;

                string strName = innerdevice.DeviceName;
                strName = strName.Replace("Пожарный", " ");
                strName = strName.Replace("дымовой", " ");
                strName = strName.Replace("тепловой", " ");
                strName = strName.Replace("извещатель", " ");
                strName = strName.Replace("Ручной", "");
                strName = strName.Trim();
                innerdevice.DeviceName = strName.TrimStart(' ');

                innerdevices.Add(innerdevice);
            }


            foreach (Device device in ClientApi.ServiceClient.Configuration.Devices)
            {

                //                Device parent = device.Parent;
                DeviceDescriptor mydevice = innerdevices.First(x => x.Path == device.Path);
                if (device.Parent != null)
                {
                    DeviceDescriptor parentDevice = innerdevices.First(x => x.Path == device.Parent.Path);
                    mydevice.Parent = parentDevice;
                    //if (parentDevice != null)
                    //{
                    //    if (parentDevice.Children == null)
                    //    {
                    //        parentDevice.Children = new List<DeviceDescriptor>();

                    //    }
                    //    parentDevice.Children.Add(mydevice);
                    //}
                }

            }

            //viewModel.ComDevices = new System.Collections.ObjectModel.ObservableCollection<TestServiceClient.ServiceReference.ComDevice>();
            //viewModel.ComDevices.Add(devices[0]);
            Devices = new System.Collections.ObjectModel.ObservableCollection<RubezhAX.DeviceDescriptor>();
            //                viewModel.Devices.Add(innerdevices[0]);
            //for (int i = 0; i < innerdevices.Count; i++ )
            Devices.Add(innerdevices[0]);
            return true;


        }




        public ViewModel(Object parent)
        {
            controllerAPI = new ServiceClient();
            parentForm = parent as UCRubezh;
            
            mDataID = new ObservableCollection<MetaDataID>();
            mDataID.Add(new MetaDataID("Компьютер", "F8340ECE-C950-498D-88CD-DCBABBC604F3"));
            mDataID.Add(new MetaDataID("Страница", "6298807D-850B-4C65-8792-A4EAB2A4A72A"));
            mDataID.Add(new MetaDataID("Индикатор", "E486745F-6130-4027-9C01-465DE5415BBF"));
            mDataID.Add(new MetaDataID("Прибор Рубеж-2AM", "B476541B-5298-4B3E-A9BA-605B839B1011"));
            mDataID.Add(new MetaDataID("БУНС", "02CE2CC4-D71F-4EAA-ACCC-4F2E870F548C"));
            mDataID.Add(new MetaDataID("Модуль сопряжения МС-3", "F966D47B-468D-40A5-ACA7-9BE30D0A3847"));
            mDataID.Add(new MetaDataID("Модуль сопряжения МС-4", "868ED643-0ED6-48CD-A0E0-4AD46104C419"));
            mDataID.Add(new MetaDataID("Блок индикации", "28A7487A-BA32-486C-9955-E251AF2E9DD4"));
            mDataID.Add(new MetaDataID("Прибор Рубеж-10AM", "E750EF8F-54C3-4B00-8C72-C7BEC9E59BFC"));
            mDataID.Add(new MetaDataID("Прибор Рубеж-4A", "F3485243-2F60-493B-8A4E-338C61EF6581"));
            mDataID.Add(new MetaDataID("Релейный исполнительный модуль РМ-1", "4A60242A-572E-41A8-8B87-FE6B6DC4ACE"));
            mDataID.Add(new MetaDataID("Пожарный дымовой извещатель ИП 212-64", "1E045AD6-66F9-4F0B-901C-68C46C89E8DA"));
            mDataID.Add(new MetaDataID("Пожарный тепловой извещатель ИП 101-29-A3R1", "799686B6-9CFA-4848-A0E7-B33149AB940C"));
            mDataID.Add(new MetaDataID("Пожарный комбинированный извещатель ИП212 101-64-А2R1", "37F13667-BC77-4742-829B-1C43FA404C1F"));
            mDataID.Add(new MetaDataID("Пожарная адресная метка АМ1", "DBA24D99-B7E1-40F3-A7F7-8A47D4433392"));
            mDataID.Add(new MetaDataID("Кнопка останова СПТ", "CD7FCB14-F808-415C-A8B7-11C512C275B4"));
            mDataID.Add(new MetaDataID("Кнопка запуска СПТ", "E8C04507-0C9D-429C-9BBE-166C3ECA4B5C"));
            mDataID.Add(new MetaDataID("Кнопка управления автоматикой", "1909EBDF-467D-4565-AD5C-CD5D9084E4C3"));
            mDataID.Add(new MetaDataID("Ручной извещатель ИПР513-11", "641FA899-FAA0-455B-B626-646E5FBE785A"));
            mDataID.Add(new MetaDataID("Модуль Управления Клапанами Дымоудаления", "44EEDF03-0F4C-4EBA-BD36-28F96BC6B16E"));
            mDataID.Add(new MetaDataID("Модуль Управления Клапанами Огнезащиты", "B603CEBA-A3BF-48A0-BFC8-94BF652FB72A"));
            mDataID.Add(new MetaDataID("Насосная Станция", "AF05094E-4556-4CEE-A3F3-981149264E89"));
            mDataID.Add(new MetaDataID("Насос", "8BFF7596-AEF4-4BEE-9D67-1AE3DC63CA94"));
            mDataID.Add(new MetaDataID("Жокей-насос", "68E8E353-8CFC-4C54-A1A8-D6B6BF4FD20F"));
            mDataID.Add(new MetaDataID("Компрессор", "ED58E7EB-BA88-4729-97FF-427EBC822E81"));
            mDataID.Add(new MetaDataID("Дренажный насос", "8AFC9569-9725-4C27-8815-18167642CA29"));
            mDataID.Add(new MetaDataID("Насос компенсации утечек", "40DAB36C-2353-4BFD-A1FE-8F542EC15D49"));
            mDataID.Add(new MetaDataID("Пожарная адресная метка АМП-4", "D8997F3B-64C4-4037-B176-DE15546CE568"));
            mDataID.Add(new MetaDataID("Модуль речевого оповещения", "2D078D43-4D3B-497C-9956-990363D9B19B"));
            mDataID.Add(new MetaDataID("Задвижка", "4935848F-0084-4151-A0C8-3A900E3CB5C5"));
            mDataID.Add(new MetaDataID("Технологическая адресная метка АМ1-Т", "F5A34CE2-322E-4ED9-A75F-FC8660AE33D8"));
            mDataID.Add(new MetaDataID("АСПТ", "FD200EDF-94A4-4560-81AA-78C449648D45"));
            mDataID.Add(new MetaDataID("Модуль дымоудаления-1.02/3", "043FBBE0-8733-4C8D-BE0C-E5820DBF7039"));
            mDataID.Add(new MetaDataID("USB преобразователь МС-2", "CD0E9AA0-FD60-48B8-B8D7-F496448FADE6"));
            mDataID.Add(new MetaDataID("USB преобразователь МС-1", "FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6"));
            mDataID.Add(new MetaDataID("USB Канал", "F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E"));
            mDataID.Add(new MetaDataID("USB Канал", "780DE2E6-8EDD-4CFA-8320-E832EB699544"));
            mDataID.Add(new MetaDataID("USB Канал c резервированием", "2863E7A3-5122-47F8-BB44-4358450CD0EE"));
            mDataID.Add(new MetaDataID("Состав", "C2E0F845-D836-4AAE-9894-D5CBE2B9A7DD"));
            mDataID.Add(new MetaDataID("USB Рубеж-2AM", "1EDE7282-0003-424E-B76C-BB7B413B4F3B"));
            mDataID.Add(new MetaDataID("USB Рубеж-4A", "7CED3D07-C8AF-4141-8D3D-528050EEA72D"));
            mDataID.Add(new MetaDataID("USB БУНС", "4A3D1FA3-4F13-44D8-B9AD-825B53416A71"));

            SelectCommand = new RelayCommand(OnSelect, can);

        
        }

        public RelayCommand SelectCommand { get; private set; }

        void OnSelect(object o)
        {
            
//            MessageBox.Show(selectedDevice.DeviceName);
            parentForm.DeviceNameLabel.Text = selectedDevice.DeviceName;
            parentForm.DeviceName = selectedDevice.DeviceName;
            form.Close();      
        
        }

        bool flag;
        bool can(object o)
        {
            if (selectedDevice == null) return false;
            return selectedDevice.Enable;
        }
       


  //      public RelayCommand Start {get; private set;}

        public void OnStart(object parameter)
        {
//            controller.Start();
        }

        string messages;
        public string Messages
        {
            get { return messages; }
            set
            {
                messages = value;
                OnPropertyChanged("Messages");
            }
        }
        ObservableCollection<DeviceDescriptor> devices;
        public ObservableCollection<DeviceDescriptor> Devices
        {
            get { return devices; }
            set
            {
                devices = value;
                OnPropertyChanged("Devices");
            }
        }

        DeviceDescriptor selectedDevice;
        public DeviceDescriptor SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }


        MetaDataID selectedID;
        public MetaDataID SelectedID
        {
            get { return selectedID; }
            set
            {
                selectedID = value;
                string id = selectedID.StrID;
                form.MyMetadataDriverID = id;
                OnPropertyChanged("SelectedID");
                // работаем с деревом устройств
                foreach (DeviceDescriptor device in devices)
                {
                    if (device.DriverId == id)
                        device.Enable = true;
                    else
                        device.Enable = false;

                    if (device.Children != null)
                    {
                        foreach (DeviceDescriptor deviceLevel1 in device.Children)
                        {
                            if (deviceLevel1.DriverId == id)
                                deviceLevel1.Enable = true;
                            else
                                deviceLevel1.Enable = false;
                            if (deviceLevel1.Children != null)
                            {
                                foreach (DeviceDescriptor deviceLevel2 in deviceLevel1.Children)
                                {
                                    if (deviceLevel2.DriverId == id)
                                        deviceLevel2.Enable = true;
                                    else
                                        deviceLevel2.Enable = false;
                                }
                            }
                        }
                    }
                
                
                
                
                }



            
            }
        }


        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class MetaDataID
    {
        string strID;
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string StrID
        {
            get { return strID; }
            set { strID = value; }
        }



        public MetaDataID(string name, string id)
        {
            strID = id;
            Name = name;
        }


    
    
    }




}
