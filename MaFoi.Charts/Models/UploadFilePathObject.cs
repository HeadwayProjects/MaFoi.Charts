using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaFoi.Charts.Models
{
    public class UploadFilePathObject
    {
        public string BlobContainerName { get; set; }
        public string CompanyId { get; set; }
        public string AssociateCompanyId { get; set; }
        public string LocationId { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public string FilePathToSave { get; set; }
        public string FileName { get; set; }
    }
}