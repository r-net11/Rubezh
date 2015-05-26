using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerCalculator.Models
{
    public enum ErrorType
    {
        Voltage,
        Current
    }

    public class LineError
    {
        public Device Device { get; private set; }
        public ErrorType ErrorType { get; private set; }
        public double ErrorScale { get; private set; }

        public LineError(Device device, ErrorType errorType, double errorScale)
        {
            Device = device;
            ErrorType = errorType;
            ErrorScale = errorScale;
        }
    }
}
