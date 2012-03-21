using System;
using System.Collections.Generic;

namespace GroupControllerModule.Models
{
    public class XDeviceLogic
    {
        public XDeviceLogic()
        {
            Clauses = new XClause();
        }

        public XClause Clauses { get; set; }
    }

    public class XClause
    {
        public XClause()
        {
            Devices = new List<Guid>();
            Zones = new List<short>();
        }

        List<Guid> Devices { get; set; }
        List<short> Zones { get; set; }

        public ClauseOperandType ClauseOperandType { get; set; }
        public ClauseOperationType ClauseOperationType { get; set; }
        public ClauseJounOperationType ClauseJounOperationType { get; set; }
    }

    public enum ClauseOperandType
    {
        Device,
        Zone
    }

    public enum ClauseOperationType
    {
        All,
        One
    }

    public enum ClauseJounOperationType
    {
        And,
        Or
    }
}