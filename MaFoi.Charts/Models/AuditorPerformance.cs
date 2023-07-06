using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class AuditorPerformance
    {
        public int Audited { get; set; }
        public int NotAudited { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}