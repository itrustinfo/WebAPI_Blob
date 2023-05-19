using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.Models
{
    public class IssueStatusModel
    {
        public string IssueStatusId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public DateTime StatusDate { get; set; }
        //public Guid IssueUId { get; set; }
       // public virtual IssueModel issue_model { get; set; }
        public virtual IEnumerable<IssueStatusAttachedModel> issue_status_attachments { get; set; }
    }
}