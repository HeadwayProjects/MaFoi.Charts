using System;
namespace MaFoi.Charts.Models
{
    public class ToDoRecommendations 
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        public Guid AssociateCompanyId { get; set; }
        public Company AssociateCompany { get; set; }
        public Guid LocationId { get; set; }
        public Location Location { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string Recommendations { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set;}
    }
}
