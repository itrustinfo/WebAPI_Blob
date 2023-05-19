using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.Models
{
    public class LetterAttachmentsModel
    {
        
            // public int IssueAttachedId { get; se
            public string AttachedFileName { get; set; }
            public string AttachedFileType { get; set; }
            public string FileAsBase64 { get; set; }
            // public Guid IssueUId { get; set; }
            // public virtual IssueModel issue_model { get; set; }
        
    }
}