using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlBase
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CanBindAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class FunctionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Event)]
    public class EventAttribute : Attribute
    {
    }
}
