using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class ToDoFilterCriteria
    {
        public Guid Company { get; set; }
        public Guid AssociateCompany { get; set; }
        public Guid Location { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string[] Statuses { get; set; }
        public string Month { get; set; }
        public int? Year { get; set; }
        //public string? DateFilter { get; set; }
        //public Guid AuditorId { get;set; }
    }
}