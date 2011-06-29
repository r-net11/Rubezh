using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class NewDeviceRangeViewModel : DialogContent
    {
        public NewDeviceRangeViewModel(DeviceViewModel parent, bool addMany)
        {
            Title = "Новоые устройства";
            AddMany = addMany;
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
            _parentDeviceViewModel = parent;
            _parent = _parentDeviceViewModel.Device;
        }

        DeviceViewModel _parentDeviceViewModel;
        Device _parent;

        public bool AddMany { get; private set; }

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
                if (AddMany)
                {
                    SetNewAddressRange();
                }
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
            else
            {
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

                var maxAddress = (from Device device in _parent.Children
                                  where ((device.IntAddress > 256) && (device.IntAddress < shleifCount * 256 + 255))
                                  select (int?)device.IntAddress).Max();

                if (maxAddress.HasValue)
                {
                    if ((maxAddress.Value + 1) % 256 == 0)
                    {
                        return maxAddress.Value + 2;
                    }
                    else
                    {
                        return maxAddress.Value + 1;
                    }
                }
                else
                    return 257;
            }
        }

        void SetNewAddressRange()
        {
            int minAddress = 0;
            int maxAddress = 0;
            bool isOnShleif;

            if (SelectedDriver.IsRangeEnabled)
            {
                isOnShleif = false;
                minAddress = SelectedDriver.MinAddress;
                maxAddress = SelectedDriver.MaxAddress;
            }
            else
            {
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

                isOnShleif = true;
                minAddress = 256;
                maxAddress = shleifCount * 256 + 255;
            }

            int startAddress = 0;
            int endAddress = 0;
            int rangeLength = 0;

            var maxRangeAddress = (from Device device in _parent.Children
                                   where ((device.IntAddress >= minAddress) && ((device.IntAddress < maxAddress)))
                                   select (int?)device.IntAddress).Max();

            if (maxRangeAddress.HasValue)
            {
                for (int i = minAddress; i < maxAddress; i++)
                {
                    if (i > maxRangeAddress.Value)
                    {
                        if ((isOnShleif) && ((i % 256 == 0) || ((i + 1) % 256 == 0)))
                            continue;

                        if ((_parent.Children.Any(x => x.IntAddress != i)) && (_parent.Children.Any(x => x.IntAddress != i + 1)))
                        {
                            startAddress = i;
                            endAddress = i + 1;
                            rangeLength = 2;
                            break;
                        }
                    }
                }
            }

            if (rangeLength == 0)
            {
                for (int i = minAddress; i < maxAddress; i++)
                {
                    if ((isOnShleif) && ((i % 256 == 0) || ((i + 1) % 256 == 0)))
                        continue;

                    if ((_parent.Children.Any(x => x.IntAddress != i)) && (_parent.Children.Any(x => x.IntAddress != i + 1)))
                    {
                        startAddress = i;
                        endAddress = i + 1;
                        rangeLength = 2;
                    }
                }
            }
            if (rangeLength == 0)
            {
                for (int i = minAddress; i < maxAddress; i++)
                {
                    if ((isOnShleif) && (i % 256 == 0))
                        continue;

                    if (_parent.Children.Any(x => x.IntAddress != i))
                    {
                        startAddress = i;
                        endAddress = i;
                        rangeLength = 1;
                    }
                }
            }
            if (rangeLength == 0)
            {
                startAddress = minAddress;
                endAddress = minAddress;
                rangeLength = 1;
            }

            if (SelectedDriver.IsDeviceOnShleif)
            {
                StartAddress = IntAddressToString(startAddress);
                EndAddress = IntAddressToString(endAddress);
            }
            else
            {
                StartAddress = startAddress.ToString();
                EndAddress = endAddress.ToString();
            }
        }

        string IntAddressToString(int intAddress)
        {
            int intShleifAddress = intAddress / 256;
            int intSelfAddress = intAddress % 256;
            string address = intShleifAddress.ToString() + "." + intSelfAddress.ToString();
            return address;
        }

        int StringAddressToInt(string address)
        {
            var addresses = address.Split('.');
            int intShleifAddress = System.Convert.ToInt32(addresses[0]);
            int intAddress = System.Convert.ToInt32(addresses[1]);
            int fullIntAddress = intShleifAddress * 256 + intAddress;
            return fullIntAddress;
        }

        void CreateAddressRange()
        {
            if (AddMany)
            {
                int startAddress = 0;
                int endAddress = 0;

                if (SelectedDriver.IsDeviceOnShleif)
                {
                    startAddress = StringAddressToInt(StartAddress);
                    endAddress = StringAddressToInt(EndAddress);
                }
                else
                {
                    startAddress = Convert.ToInt32(StartAddress);
                    endAddress = Convert.ToInt32(EndAddress);
                }

                if (startAddress < endAddress)
                {
                    ;
                }
                if (startAddress < SelectedDriver.MinAddress)
                {
                    ;
                }
                if (endAddress < SelectedDriver.MaxAddress)
                {
                    ;
                }

                if (_parent.Children.Any(x => ((x.IntAddress >= startAddress) || (x.IntAddress <= endAddress))))
                {
                    ;
                }

                for (int i = startAddress; i <= endAddress; i++)
                {
                    if ((SelectedDriver.IsDeviceOnShleif) && (i % 256 == 0))
                        continue;

                    Create(i);
                }
            }
        }

        void Create(int address)
        {
            Device device = _parent.AddChild(SelectedDriver, address);
            AddDevice(device, _parentDeviceViewModel);
        }

        void AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.Initialize(device, _parentDeviceViewModel.Source);
            deviceViewModel.Parent = parentDeviceViewModel;
            parentDeviceViewModel.Children.Add(deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                AddDevice(childDevice, deviceViewModel);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            if (SelectedDriver != null)
            {
                if (AddMany)
                {
                    CreateAddressRange();
                }
                else
                {
                    Create(GetNewAddress());
                }
            }

            _parentDeviceViewModel.Update();
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
