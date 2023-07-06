using System;
namespace MaFoi.Charts.Models
{
    public class RuleComplianceDetail : BaseTableEntity
  {
        public string ComplianceName { get; set; }
        public string ComplianceDescription { get; set; }
        public Guid StateId { get; set; }
        public State State { get; set; }
        public Guid RuleId { get; set; }
        public Rule Rule { get; set; }
        public string ProofOfCompliance { get; set; }
        public string Penalty { get; set; }
        public string Risk { get; set; }
        public decimal MaximumPenaltyAmount { get; set; }
        public bool Impriosonment { get; set; }
        public bool ContinuingPenalty { get; set; }
        public bool CancellationSuspensionOfLicense { get; set; }
        public string StatutoryAuthority { get; set; }
        public string ComplianceNature { get; set; }
        public string AuditType { get; set; }
    }
}
