using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class SearchParams
    {
        public string Search { get; set; }
        public List<Filtering> Filters { get; set; }
        public Paging Pagination { get; set; }
        public Sorting Sort { get; set; }
        public bool? IncludeCentral { get; set; }

        
    }

    public class Searching
    {
        public string ColumnName { get; set; }
        public string Value { get; set; }
    }
    public class Filtering
    {
        public string ColumnName { get; set; }
        public string Value { get; set; }
    }

    public class Paging
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

    public class Sorting
    {
        public string ColumnName { get; set; }
        public string Order { get; set; }
    }

    public class  dashparams: SearchParams
    {
        public string JwtToken { get; set; }
    }

}