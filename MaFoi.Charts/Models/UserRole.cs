using System;
namespace MaFoi.Charts.Models
{
  public class UserRole : BaseTableEntity
  {
    public Guid RoleId { get; set; }
    public Guid UserId { get; set; }

    public Role Role { get; set; }  
    public User User { get; set; }
  }
}
