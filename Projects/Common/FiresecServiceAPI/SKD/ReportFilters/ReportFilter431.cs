using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.ReportFilters
{
    [DataContract]
    public class ReportFilter431 : SKDReportFilter, IReportFilterOrganisation, IReportFilterZone, IReportFilterDoor
    {

        #region IReportFilterOrganisation Members

        [DataMember]
        public List<Guid> Organisations { get; set; }

        #endregion

        #region IReportFilterZone Members

        [DataMember]
        public List<Guid> Zones { get; set; }

        #endregion

        #region IReportFilterDoor Members

        [DataMember]
        public List<Guid> Doors { get; set; }

        #endregion
    }
}
