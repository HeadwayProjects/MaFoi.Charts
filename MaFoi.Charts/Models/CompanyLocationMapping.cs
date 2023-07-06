using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace MaFoi.Charts.Models
{
  public class CompanyLocationMapping : BaseTableEntity
  {
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
    public Guid LocationId { get; set; }
    public Location Location { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string CompanyLocationAddress { get; set; }
    [StringLength(1000), Column(TypeName = "varchar")]
    public string Address { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string ContactPersonEmail { get; set; }
    [StringLength(20), Column(TypeName = "varchar")]
    public string ContactPersonMobile { get; set; }
    [StringLength(250), Column(TypeName = "varchar")]
    public string ContactPersonName { get; set; }
  }
}
