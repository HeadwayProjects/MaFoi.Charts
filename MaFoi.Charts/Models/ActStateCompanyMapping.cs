using System;
namespace MaFoi.Charts.Models
{
  public class ActStateCompanyMapping : BaseTableEntity
  {
    public Guid ActStateMappingId { get; set; }
    public ActStateMapping ActStateMapping { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
  }
}
