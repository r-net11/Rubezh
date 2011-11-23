using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using System.Collections.ObjectModel;
using FiresecClient;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
    public class ElementsViewModel : DialogContent
    {
        DesignerCanvas DesignerCanvas;

        public ElementsViewModel(DesignerCanvas designerCanvas)
        {
            DesignerCanvas = designerCanvas;
            Title = "Элементы на плане";

            Devices = new ObservableCollection<ElementViewModel>();

            foreach (var designerItem in designerCanvas.Items)
            {
                string name = "";
                if (designerItem.ElementBase is ElementDevice)
                {
                    ElementDevice elementDevice = designerItem.ElementBase as ElementDevice;
                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                    name = device.DottedAddress + " " + device.Driver.ShortName;
                }

                var elementViewModel = new ElementViewModel(designerItem);
                elementViewModel.Name = name;
                Devices.Add(elementViewModel);
            }
        }

        public ObservableCollection<ElementViewModel> Devices { get; private set; }
    }
}
