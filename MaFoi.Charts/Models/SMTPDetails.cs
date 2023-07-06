
using System;
namespace MaFoi.Charts.Models
{
    public class SMTPDetails : BaseTableEntity
    {
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        
        
        public string EmailAddress { get; set; }
        
        public string Password { get; set; }
        
        public string Host { get; set; }        
        public int Port { get; set; }
    }
}
