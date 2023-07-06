using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace MaFoi.Charts.Models
{
  public class State : BaseTableEntity
  {
    
    public string Code { get; set; }
    
    public string Name { get; set; }
  }
}
