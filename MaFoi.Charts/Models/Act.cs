using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class Act : BaseTableEntity
    {
        
        public string Name { get; set; }
        
        public string EstablishmentType { get; set; }
        public Guid LawId { get; set; }
        public Law Law { get; set; }
    }
}