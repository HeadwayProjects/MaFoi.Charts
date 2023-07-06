using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class AuditReportData
    {
        public List<ToDo> ToDoList { get; set; }
        public List<ToDoRecommendations> ToDoRecommendations { get; set; }
        public AuditReportSummary AuditReportSummary { get; set; }
        public List<RuleComplianceDetail> RuleComplianceDetails { get; set; }
        public AuditorPerformance AuditorPerformance { get; set; }
    }
}