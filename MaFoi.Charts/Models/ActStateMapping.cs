using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;

namespace MaFoi.Charts.Models
{
  public class ActStateMapping : BaseTableEntity
  {

    public string FilePath { get; set; }

    public string FileName { get; set; }


    public string FormName { get; set; }
    public Guid ActRuleActivityMappingId { get; set; }
    public ActRuleActivityMapping ActRuleActivityMapping { get; set; }
    public Guid StateId { get; set; }
    public State State { get; set; }
    public bool FileRequired { get; set; }
  }
}
