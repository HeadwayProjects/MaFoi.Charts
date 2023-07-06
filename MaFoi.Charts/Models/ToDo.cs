using System;

namespace MaFoi.Charts.Models
{
    public class ToDo : BaseTableEntity
    {
        public string AuditStatus { get; set; }
        public string AuditRemarks { get; set; }
        public int Day { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime SavedDate { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime AuditedDate { get; set; }
        public Guid ActId { get; set; }
        public Act Act { get; set; }
        public Guid RuleId { get; set; }
        public Rule Rule { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        public Guid AssociateCompanyId { get; set; }
        public Company AssociateCompany { get; set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        public Guid ActivityId { get; set; }
        public Activity Activity { get; set; }
        public string ActStateMappingId { get; set; }
        public string Status { get; set; }
        public string FormsStatusRemarks { get; set; }
        public bool Published { get; set; }
        public string Auditted { get; set; }
    }
}