using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common;
using ClientApi;
using System.Collections.ObjectModel;

namespace ServiceVisualizer
{
    public class ViewModel : BaseViewModel
    {
        public ViewModel()
        {
            Current = this;
            ConnectCommand = new RelayCommand(OnConnect);
        }

        public static ViewModel Current { get; private set; }

        ObservableCollection<DeviceViewModel> devicesViewModels;
        public ObservableCollection<DeviceViewModel> DevicesViewModels
        {
            get { return devicesViewModels; }
            set
            {
                devicesViewModels = value;
                OnPropertyChanged("DevicesViewModels");
            }
        }

        DeviceViewModel selectedDevicesViewModel;
        public DeviceViewModel SelectedDevicesViewModel
        {
            get { return selectedDevicesViewModel; }
            set
            {
                selectedDevicesViewModel = value;
                OnPropertyChanged("SelectedDevicesViewModel");
            }
        }

        public RelayCommand ConnectCommand { get; private set; }
        void OnConnect(object obj)
        {
            ServiceClient serviceClient = new ServiceClient();
            serviceClient.Start();
            DevicesViewModels = new ObservableCollection<DeviceViewModel>();

            Device rootDevice = ServiceClient.Configuration.Devices[0];

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.SetDevice(rootDevice);
            rootDeviceViewModel.IsExpanded = true;
            DevicesViewModels.Add(rootDeviceViewModel);
            AddDevice(rootDevice, rootDeviceViewModel);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.SetDevice(device);
                deviceViewModel.IsExpanded = true;
                parentDeviceViewModel.Children.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
        }
    }
}

        //<ListBox DataContext="{Binding SelectedDevicesViewModel}">
        //    <ListBox.ItemsPanel>
        //        <ItemsPanelTemplate>
        //            <StackPanel>
        //                <Grid>
        //                    <Grid.ColumnDefinitions>
        //                        <ColumnDefinition/>
        //                        <ColumnDefinition/>
        //                    </Grid.ColumnDefinitions>
        //                    <Grid.RowDefinitions>
        //                        <RowDefinitions/>
        //                        <RowDefinitions/>
        //                        <RowDefinitions/>
        //                        <RowDefinitions/>
        //                    </Grid.RowDefinitions>
        //    <TextBlock Grid.Row="1" Grid.Column="0" Text="DriverID:"/>
        //                        <TextBlock Grid.Row="0" Grid.Column="0" Text="Имя:"/>
        //                        <TextBlock Grid.Row="2" Grid.Column="0" Text="Адрес:"/>
        //                        <TextBlock Grid.Row="0" Grid.Column="1" Text="{Binding DeviceName}" Background="{Binding Enable, Converter={StaticResource BoolToBackColorConverter}}" Foreground="{Binding Enable, Converter={StaticResource BoolToColorConverter}}"/>
        //                        <TextBlock Grid.Row="2" Grid.Column="1" Text="{Binding Address}" />
        //                        <TextBlock Grid.Row="1" Grid.Column="1" Text="{Binding DriverId}" />


        //                </Grid>
        //                <TextBlock Text="{Binding DriverName}" />
        //                <TextBlock Text="{Binding State}" />
        //                <TextBlock Text="{Binding States}" />
        //                <TextBlock Text="{Binding Parameters}" />
        //            </StackPanel>
        //        </ItemsPanelTemplate>
        //    </ListBox.ItemsPanel>
            
            
        //</ListBox>


           //<TextBlock Text="{Binding DriverName}" />
           //<TextBlock Text="{Binding State}" />
           //<TextBlock Text="{Binding States}" />
           // <TextBlock Text="{Binding Parameters}" />


            //            <Grid>
            //                <Grid.ColumnDefinitions>
            //                    <ColumnDefinition Width="150*" />
            //                    <ColumnDefinition Width="628*" />
            //                </Grid.ColumnDefinitions>
            //                <Grid.RowDefinitions>
            //                    <RowDefinition/>
            //                    <RowDefinition/>
            //                    <RowDefinition Height="Auto"/>
            //                    <RowDefinition/>
            //                </Grid.RowDefinitions>
            //                <TextBlock Grid.Column="0" Grid.Row="0" Text="Имя устройства"     />
            //                <TextBlock Grid.Column="0" Grid.Row="1" Text="Состояние"     />
            //                <TextBlock Grid.Column="0" Grid.Row="2" Text="Список состояний"     />
            //                <TextBlock Grid.Column="0" Grid.Row="3" Text="Параметры"     />
            //                <TextBlock Grid.Column="1" Grid.Row="0" Text="{Binding DriverName}"     />
            //                <TextBlock Grid.Column="1" Grid.Row="1" Text="{Binding State}"     />
            //                <TextBlock Grid.Column="1" Grid.Row="2" Text="{Binding States}"     />
            //                <TextBlock Grid.Column="1" Grid.Row="3" Text="{Binding Parameters}"     />


            //</Grid>
