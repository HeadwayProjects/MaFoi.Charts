using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class AuditReportSummary
    {
        public string AuditorName { get; set; }
        public string Company { get; set; }
        public string AssociateCompany { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int Audited { get; set; }
        public int Rejected { get; set; }
        public int Compliant { get; set; }
        public int NonCompliance { get; set; }
        public int NotApplicable { get; set; }
        public int TotalRecords { get; set; }
    }
}