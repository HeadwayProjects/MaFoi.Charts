using System;
namespace MaFoi.Charts.Models
{
  public class ToDoDetails : BaseTableEntity
  {
    public Guid ToDoId { get; set; }
    public ToDo ToDo { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
  }
}
