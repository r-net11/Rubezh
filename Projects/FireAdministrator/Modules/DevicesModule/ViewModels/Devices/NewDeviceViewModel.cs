using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using FiresecClient;
using System.Windows;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class NewDeviceViewModel : DialogContent
    {
        public NewDeviceViewModel(DeviceViewModel parent, bool addMany)
        {
            //ViewModels.Devices.DriversView driversView = new ViewModels.Devices.DriversView();
            //driversView.ShowDialog();

            Title = "Новое устройство";
            AddMany = addMany;
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;

            if (AddMany)
                SetAddressRange();
        }

        DeviceViewModel _parentDeviceViewModel;
        Device _parent;

        public bool AddMany { get; private set; }

        void SetAddressRange()
        {
            int maxAddress = _parentDeviceViewModel.Device.Children.Max(x => x.IntAddress);
            StartAddress = IntToAddress(maxAddress + 1);
            EndAddress = IntToAddress(maxAddress + 2);
        }

        string _startAddress;
        public string StartAddress
        {
            get { return _startAddress; }
            set
            {
                _startAddress = value;
                OnPropertyChanged("StartAddress");
            }
        }

        string _endAddress;
        public string EndAddress
        {
            get { return _endAddress; }
            set
            {
                _endAddress = value;
                OnPropertyChanged("EndAddress");
            }
        }

        public IEnumerable<Driver> Drivers
        {
            get
            {
                return from Driver driver in FiresecManager.Configuration.Drivers
                       where _parentDeviceViewModel.Device.Driver.AvaliableChildren.Contains(driver.Id)
                       select driver;
            }
        }

        Driver _selectedDriver;
        public Driver SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                _selectedDriver = value;
                OnPropertyChanged("SelectedDriver");
            }
        }

        int GetNewAddress()
        {
            if (SelectedDriver.IsRangeEnabled)
            {
                var maxRangeAddress = (from Device device in _parent.Children
                                       where ((device.IntAddress >= SelectedDriver.MinAddress) && ((device.IntAddress < SelectedDriver.MaxAddress)))
                           select (int?)device.IntAddress).Max();

                if (maxRangeAddress.HasValue)
                    return maxRangeAddress.Value + 1;
                else
                    return SelectedDriver.MinAddress;
            }

            string addressMask = _parent.Driver.AddressMask;
            if (addressMask == null)
                addressMask = SelectedDriver.AddressMask;

            int shleifCount = 0;
            if (addressMask != null)
            {
                switch (addressMask)
                {
                    case "[8(1)-15(2)];[0(1)-7(255)]":
                        shleifCount = 2;
                        break;

                    case "[8(1)-15(4)];[0(1)-7(255)]":
                        shleifCount = 4;
                        break;

                    case "[8(1)-15(10)];[0(1)-7(255)]":
                        shleifCount = 10;
                        break;

                    default:
                        shleifCount = 0;
                        break;
                }
            }
            

            var maxAddress_2 = (from Device device in _parent.Children
                                   select (int?)device.IntAddress).Max();

            if (maxAddress_2.HasValue)
                return maxAddress_2.Value + 1;
            else
                return 257;

            //return "";

            string mask = _parentDeviceViewModel.Device.Driver.ChildAddressMask;

            List<int> items = new List<int>();

            while (true)
            {
                int indexOf = mask.IndexOf("(");
                if (indexOf == -1)
                    break;

                mask = mask.Remove(0, indexOf + 1);
                string item = mask.Substring(0, mask.IndexOf(")"));
                int intItem = Convert.ToInt32(item);
                items.Add(intItem);
            }

            int minShleif = Convert.ToInt32(items[0]);
            int maxShleif = Convert.ToInt32(items[1]);
            int minAddress = Convert.ToInt32(items[2]);
            int maxAddress = Convert.ToInt32(items[3]);
            int minAvailableAddress = 0;

            for (int shleif = minShleif; shleif <= maxShleif; shleif++)
            {
                for (int addr = minAddress; addr <= maxAddress; addr++)
                {
                    int fullIntAddress = shleif * 256 + addr;

                    if (_parentDeviceViewModel.Device.Children.Any(x => x.IntAddress == fullIntAddress) == false)
                    {
                        minAvailableAddress = fullIntAddress;
                        shleif = maxShleif + 1;
                        addr = maxAddress + 1;
                        break;
                    }
                }
            }

            //return IntToAddress(minAvailableAddress);
        }

        int AddressToInt(string address)
        {
            int shleif = Convert.ToInt32(address.Split('.')[0]);
            int addr = Convert.ToInt32(address.Split('.')[1]);
            int fullAddr = shleif * 256 + addr;
            return fullAddr;
        }

        string IntToAddress(int addr)
        {
            int shleif = addr / 256;
            int a = addr - shleif * 256;
            string ad = shleif.ToString() + "." + a.ToString();
            return ad;
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (SelectedDriver != null)
            {
                Device device = new Device();
                device.Driver = SelectedDriver;

                device.IntAddress = 0;
                if (SelectedDriver.HasAddress)
                {
                    device.IntAddress = GetNewAddress();
                }

                _parentDeviceViewModel.Device.Children.Add(device);

                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Initialize(device, _parentDeviceViewModel.Source);
                deviceViewModel.Parent = _parentDeviceViewModel;
                _parentDeviceViewModel.Children.Add(deviceViewModel);

                foreach (var autoCreateDriverId in deviceViewModel.Device.Driver.AutoCreateChildren)
                {
                    var autoCreateDriver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == autoCreateDriverId);

                    if ((autoCreateDriver.ShortName == "АСПТ") && (deviceViewModel.Driver.DriverName == "Прибор Рубеж-2AM"))
                        continue;

                    for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
                    {
                        Device childDevice = new Device();
                        childDevice.Driver = autoCreateDriver;
                        childDevice.IntAddress = i;
                        device.Children.Add(childDevice);

                        DeviceViewModel childDeviceViewModel = new DeviceViewModel();
                        childDeviceViewModel.Initialize(childDevice, _parentDeviceViewModel.Source);
                        childDeviceViewModel.Parent = deviceViewModel;
                        deviceViewModel.Children.Add(childDeviceViewModel);
                    }

                    deviceViewModel.IsExpanded = true;
                }

                _parentDeviceViewModel.Update();
            }

            FiresecManager.Configuration.Update();
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
