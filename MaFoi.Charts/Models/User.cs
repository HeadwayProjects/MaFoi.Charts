using System;
using System.Collections.Generic;

namespace MaFoi.Charts.Models
{
  public class User : BaseTableEntity
  {
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime LastLoginDate { get; set; }
    public DateTime DateOfJoining { get; set; }
    public string Status { get; set; }
    public bool IsActive { get; set; }
    public string LoginOTP { get; set; }
    public IEnumerable<UserRole> UserRoles { get; set; }
  }
}
