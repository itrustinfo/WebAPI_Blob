using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.Models
{
    public class IssueAttachedModel
    {
       // public int IssueAttachedId { get; set; }
        public string AttachedFileName { get; set; }
       // public string AttachedFilePath { get; set; }
        public string FileAsBase64 { get; set; }
       // public Guid IssueUId { get; set; }
       // public virtual IssueModel issue_model { get; set; }
    }
}