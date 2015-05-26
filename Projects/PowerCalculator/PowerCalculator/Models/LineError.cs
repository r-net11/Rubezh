using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PowerCalculator.Models
{
    public enum ErrorType
    {
		[Description("")]
		None,

		[Description("Напряжение")]
        Voltage,

		[Description("Ток")]
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
