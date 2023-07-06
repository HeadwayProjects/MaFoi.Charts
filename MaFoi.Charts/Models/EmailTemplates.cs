using System;
namespace MaFoi.Charts.Models
{
    public class EmailTemplates : BaseTableEntity
    {
        public int TemplateTypeId { get; set; }
        public TemplateType TemplateType { get; set; }
        public string Subject { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailCC { get; set; }
        public string Body { get; set; }
        public string Signature { get; set; }
    }
}
