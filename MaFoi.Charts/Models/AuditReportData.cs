using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;

namespace MaFoi.Charts.Models
{
   
    public class AuditReportData
    {
        public List<ToDoWithRuleCompliance> ToDoList { get; set; }
        public List<ToDoRecommendations> ToDoRecommendations { get; set; }
        public AuditReportSummary AuditReportSummary { get; set; }
        public AuditorPerformance AuditorPerformance { get; set; }
    }

    public class ToDoWithRuleCompliance
    {
        public ToDo ToDo { get; set; }
        public RuleComplianceDetail RuleComplianceDetails { get; set; }
    }
}