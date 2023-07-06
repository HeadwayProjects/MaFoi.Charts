using System;
namespace MaFoi.Charts.Models
{
  public class ActStateCompanyLocationMapping : BaseTableEntity
  {
    public Guid ActStateCompanyMappingId { get; set; }
    public ActStateCompanyMapping ActStateCompanyMapping { get; set; }
    public Guid  LocationId { get; set; }
    public Location  Location { get; set; }
  }
}
