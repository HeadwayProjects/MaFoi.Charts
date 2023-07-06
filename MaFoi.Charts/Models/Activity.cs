using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaFoi.Charts.Models
{
    public class Activity : BaseTableEntity
  {
    [StringLength(255), Column(TypeName = "varchar")]
    public string Name { get; set; }
    [StringLength(255), Column(TypeName = "varchar")]
    public string Type { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string Periodicity { get; set; }
    [StringLength(50), Column(TypeName = "varchar")]
    public string CalendarType { get; set; }
    }
}
