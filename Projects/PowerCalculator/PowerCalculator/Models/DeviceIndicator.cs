using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerCalculator.Models
{
    public class DeviceIndicator
    {
        public Device Device { get; private set; }
        public double I { get; private set; }
        public double U { get; private set; }
        public bool NeedSupplier { get; private set; }
        
        public bool HasIError
        {
            get
            {
                if (Device == null)
                    return false;
                else
                    return I > Device.Driver.Imax;
            }
        }

        public bool HasUError
        {
            get
            {
                if (Device == null)
                    return false;
                else
                    return U < Device.Driver.Umin;
            }
        }

        public DeviceIndicator(Device device, double i, double u, bool needSupplier = false)
        {
            Device = device;
            I = i;
            U = u;
            NeedSupplier = needSupplier;
        }
    }
}
