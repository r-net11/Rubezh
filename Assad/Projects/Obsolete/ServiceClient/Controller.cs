using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceClient
{
    public class Controller : StateServiceReference.IStateServiceCallback
    {
        public ViewModel viewModel { get; set; }
        StateServiceReference.IStateService service;
        List<StateServiceReference.ComDevice> plainComDevices;

        public void Start()
        {
            service = new StateServiceReference.StateServiceClient(new System.ServiceModel.InstanceContext(this));
            service.Initialize();
            GetComDevices();
        }

        void ServiceClient.StateServiceReference.IStateServiceCallback.Notify(string message)
        {
            viewModel.Messages += message + Environment.NewLine;
        }

        void ServiceClient.StateServiceReference.IStateServiceCallback.ComChanged(ServiceClient.StateServiceReference.ComDevice comDevice)
        {
            StateServiceReference.ComDevice device = plainComDevices.First(x => x.Id == comDevice.Id);
            device.State = comDevice.State;
            device.LastEvents = comDevice.LastEvents;
            foreach(StateServiceReference.ComState comState in device.States)
            {
                comState.IsActive = comDevice.States.First(x => x.Id == comState.Id).IsActive;
            }
        }

        public void GetComDevices()
        {
            List<StateServiceReference.ComDevice> devices = service.GetComDevices();

            plainComDevices = new List<ServiceClient.StateServiceReference.ComDevice>(devices);

            foreach (StateServiceReference.ComDevice device in devices)
            {
                int parentId = device.ParentId;
                if (parentId != 0)
                {
                    StateServiceReference.ComDevice parentDevice = devices.First(x => x.Id == parentId);
                    device.Parent = parentDevice;

                    if (parentDevice != null)
                    {
                        if (parentDevice.Children == null)
                            parentDevice.Children = new List<ServiceClient.StateServiceReference.TreeBase>();
                        parentDevice.Children.Add(device);
                    }
                }
            }

            viewModel.ComDevices = new System.Collections.ObjectModel.ObservableCollection<ServiceClient.StateServiceReference.ComDevice>();
            viewModel.ComDevices.Add(devices[0]);
        }
    }
}
