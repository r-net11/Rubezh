using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ComServer;

namespace Common
{
    public class ComDevice : TreeBase
    {
        public string DeviceName { get; set; }
        public string DriverId { get; set; }
        public string MetadataDriverId { get; set; }

        public List<ComState> States { get; set; }
        public List<ComServer.Metadata.paramInfoType> Parameters { get; set; }
        public List<ComServer.Metadata.propInfoType> Properties { get; set; }

        public ComDevice(ComDevice Parent, ComServer.CoreConfig.devType InnerDevice)
        {
            this.Parent = Parent;
            this.InnerType = InnerDevice;
        }

        ComServer.CoreConfig.devType innerType;
        public ComServer.CoreConfig.devType InnerType
        {
            get { return innerType; }
            set
            {
                innerType = value;

                DriverId = innerType.drv;
                Address = innerType.addr;
                DeviceName = ComDeviceManager.coreConfig.drv.FirstOrDefault(x => x.idx == DriverId).name;
                MetadataDriverId = ComDeviceManager.coreConfig.drv.FirstOrDefault(x => x.idx == DriverId).id;

                ComServer.Metadata.drvType metadataDriver = ComDeviceManager.metadataConfig.drv.First(x => x.id == MetadataDriverId);
                if (metadataDriver.state != null)
                {
                    States = new List<ComState>();
                    foreach (ComServer.Metadata.stateType innerState in metadataDriver.state)
                    {
                        ComState state = new ComState()
                        {
                            Id = innerState.id,
                            Name = innerState.name,
                            Priority = Convert.ToInt32(innerState.@class)
                        };
                        state.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(state_PropertyChanged);
                        States.Add(state);
                    }
                }
                if (metadataDriver.paramInfo != null)
                    Parameters = new List<ComServer.Metadata.paramInfoType>(metadataDriver.paramInfo);
                if (metadataDriver.propInfo != null)
                    Properties = new List<ComServer.Metadata.propInfoType>(metadataDriver.propInfo);
            }
        }

        int maxPriority = 0;
        public int MaxPriority
        {
            get { return maxPriority; }
            set
            {
                maxPriority = value;
            }
        }

        void state_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //ComState state = sender as ComState;
            //int MaxState = States.FindAll(x => x.IsActive).Max(x => x.StateClass);
            //EventAggregator.OnPropertyChanged(Address, ParentAddress, state, MaxState);
        }
    }
}
