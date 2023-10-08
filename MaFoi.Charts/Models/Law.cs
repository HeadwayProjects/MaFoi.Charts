using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace MaFoi.Charts.Models
{
    public class Law : BaseTableEntity
    {
        [StringLength(255), Column(TypeName = "varchar")]
        public string Name { get; set; }
        [StringLength(1000), Column(TypeName = "varchar")]
        public string Description { get; set; }
    }
}


