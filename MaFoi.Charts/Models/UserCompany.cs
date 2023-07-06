using System;
namespace MaFoi.Charts.Models
{
  public class UserCompany : BaseTableEntity
  {
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid CompanyLocationMappingId { get; set; }
    public CompanyLocationMapping CompanyLocationMapping { get; set; }
  }
}
