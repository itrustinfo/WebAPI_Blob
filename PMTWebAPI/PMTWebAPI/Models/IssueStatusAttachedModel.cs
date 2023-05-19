using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.Models
{
    public class IssueStatusAttachedModel
    {
       // public int IssueStatusAttachedId { get; set; }
        public string AttachedFileName { get; set; }
        //public string AttachedFilePath { get; set; }
       // public string IssueStatusId { get; set; }
        public string FileAsBase64 { get; set; }
       // public virtual  IssueStatusModel issue_status_model { get; set; }
    }
}