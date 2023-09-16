using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class InternalCompliancesMapping
    {
        public string InternalCompliance { get; set; }
        public string AuditType { get; set; }
        public string Risk { get; set; }
        public string Nature { get; set; }
        public string Auditstatus { get; set; }
        public string Status { get; set; }
        public Dictionary<string,string> ImpectDetails { get; set; }

    }

    public class InternalCompliancesMappingresponse
    {
        public string Name { get; set; }
        public List<InternalCompliancesMapping> listdata {get;set;}
    }
}