using Infrastructure.Common;
using System.Collections.ObjectModel;
using GroupControllerModule.Models;
using System.Collections.Generic;

namespace GroupControllerModule.ViewModels
{
    public class DeviceConverterViewModel : DialogContent
    {
        public DeviceConverterViewModel()
        {
            Title = "Бинарный формат конфигурации";

            BinObjects = new List<BinObjectViewModel>();
            foreach (var device in XManager.DeviceConfiguration.Devices)
            {
                var binObjectViewModel = new BinObjectViewModel()
                {
                    Caption = device.Driver.ShortName + " - " + device.Address,
                    ImageSource = device.Driver.ImageSource,
                    Level = device.AllParents.Count,
                    //DeviceBinaryFormatter = new DeviceBinaryFormatter()
                };
                //binObjectViewModel.DeviceBinaryFormatter.Initialize(device);

                BinObjects.Add(binObjectViewModel);
            }
            if (BinObjects.Count > 0)
                SelectedBinObject = BinObjects[0];
        }

        public List<BinObjectViewModel> BinObjects { get; private set; }

        BinObjectViewModel _selectedBinObject;
        public BinObjectViewModel SelectedBinObject
        {
            get { return _selectedBinObject; }
            set
            {
                _selectedBinObject = value;
                OnPropertyChanged("SelectedBinObject");
            }
        }
    }
}