using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class BaseTableEntity
    {
        public Guid Id { get; set; }
     
        public DateTime CreatedDate { get; set; }

     
        public DateTime LastUpdatedDate { get; set; }
    }
}