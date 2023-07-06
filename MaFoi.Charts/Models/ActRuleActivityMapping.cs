using System;
namespace MaFoi.Charts.Models
{
  public class ActRuleActivityMapping : BaseTableEntity
  {
    public Guid ActId { get; set; }
    public Act Act { get; set; }
    public Guid RuleId { get; set; }
    public Rule Rule { get; set; }
    public Guid ActivityId { get; set; }
    public Activity Activity { get; set; }
  }
}
