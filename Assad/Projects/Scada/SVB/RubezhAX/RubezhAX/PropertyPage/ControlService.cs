using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.IO;

namespace RubezhAX
{

    public class ControlService : ServiceReference.IStateServiceCallback
        {
        public ViewModel viewModel { get; set; }
        public AXPropertyPage form;
        public event Action EventChange;

            void OnEventChange()
            {
                if (EventChange != null)
                    EventChange();
            }
            
            public ServiceReference.IStateService service = null;
            List<ServiceReference.ComDevice> plainComDevices;

            public bool Start()
            {

                try
                {

                    MetadataExchangeClient mexClient = new MetadataExchangeClient(new Uri("http://localhost:8001/StateService"), MetadataExchangeClientMode.HttpGet);
                    MetadataSet metadata = mexClient.GetMetadata();
                }
                catch (Exception ex)
                {
                    return false;
                } 



                service = new ServiceReference.StateServiceClient(new System.ServiceModel.InstanceContext(this));
                if (service == null)
                {
                    return false;
                }

                service.Initialize();
                GetComDevices();
                return true;
            }

            void RubezhAX.ServiceReference.IStateServiceCallback.Notify(string message)
            {
//                viewModel.Messages += message + Environment.NewLine;
            }


//            void OPCRubezhServer.StateServiceReference.IStateServiceCallback.ExecuteCommand(int deviceId, string command)
//            {
            
            
 //           }

            void OPCRubezhExecuteCommand(int id, string command)
            {
                service.ExecuteCommand(id, command);
            
            }

            void RubezhAX.ServiceReference.IStateServiceCallback.ComChanged(RubezhAX.ServiceReference.ComDevice comDevice)
            {
                ServiceReference.ComDevice device = plainComDevices.First(x => x.Id == comDevice.Id);
                device.State = comDevice.State;
                device.LastEvents = comDevice.LastEvents;
                foreach (ServiceReference.ComState comState in device.States)
                {
                    comState.IsActive = comDevice.States.First(x => x.Id == comState.Id).IsActive;
                }


            }

            public void GetComDevices()
            {
                List<RubezhAX.ServiceReference.ComDevice> devices = service.GetComDevices();
                plainComDevices = new List<RubezhAX.ServiceReference.ComDevice>(devices);
                List<DeviceDescriptor> innerdevices = new List<DeviceDescriptor>();
                for (int i = 0; i < devices.Count; i++)
                {
                    DeviceDescriptor innerdevice = new DeviceDescriptor();
                    innerdevice.MetadataDriverId = devices[i].MetadataDriverId;
                    innerdevice.Address = devices[i].Address;
                    innerdevice.AvailableEvents = devices[i].AvailableEvents;
                    innerdevice.AvailableFunctions = devices[i].AvailableFunctions;
                    innerdevice.DeviceName = devices[i].DeviceName;
                    innerdevice.DriverId = devices[i].DriverId;
                    innerdevice.ExtensionData = devices[i].ExtensionData;
                    innerdevice.Id = devices[i].Id;
                    innerdevice.LastEvents = devices[i].LastEvents;
                    innerdevice.Name = devices[i].Name;
                    innerdevice.Parent = devices[i].Parent;
                    innerdevice.ParentAddress = devices[i].ParentAddress;
                    innerdevice.ParentId = devices[i].ParentId;
                    innerdevice.Children = devices[i].Children;
                    innerdevice.State = devices[i].State;
                    innerdevice.States = devices[i].States;
                    innerdevice.Zone = devices[i].Zone;
                    if (innerdevice.MetadataDriverId == form.MyMetadataDriverID)
                        innerdevice.Enable = true;

                    innerdevices.Add(innerdevice);
                }


                    foreach (DeviceDescriptor device in innerdevices)
                    {
                        string strName = device.DeviceName;
                        strName = strName.Replace("Пожарный", " ");
                        strName = strName.Replace("дымовой", " ");
                        strName = strName.Replace("тепловой", " ");
                        strName = strName.Replace("извещатель", " ");
                        strName = strName.Replace("Ручной", "");
                        strName = strName.Trim();
                        device.DeviceName = strName.TrimStart(' ');

                        int parentId = device.ParentId;
                        if (parentId != 0)
                        {
                            DeviceDescriptor parentDevice = innerdevices.First(x => x.Id == parentId);
                            device.Parent = parentDevice;
                            if (parentDevice != null)
                            {
                                if (parentDevice.Children == null)
                                {
                                    parentDevice.Children = new List<RubezhAX.ServiceReference.TreeBase>();

                                }
                                parentDevice.Children.Add(device);
                            }
                        }
                    }

                //viewModel.ComDevices = new System.Collections.ObjectModel.ObservableCollection<RubezhAX.ServiceReference.ComDevice>();
                //viewModel.ComDevices.Add(devices[0]);
                viewModel.Devices = new System.Collections.ObjectModel.ObservableCollection<RubezhAX.DeviceDescriptor>();
//                viewModel.Devices.Add(innerdevices[0]);
                                //for (int i = 0; i < innerdevices.Count; i++ )
                    viewModel.Devices.Add(innerdevices[0]);

            }
        }
}
