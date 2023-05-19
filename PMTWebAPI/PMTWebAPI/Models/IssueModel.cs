using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.Models
{
    public class IssueModel
    {
        public Guid IssueUId { get; set; }
      //  public Guid ProjectUID { get; set; }
      //  public Guid WorkPackagesUID { get; set; }
      //  public Guid TaskUID { get; set; }
       // public int SerialNo { get; set; }
        //public string IssueDescription { get; set; }
        //public string ReportingUser { get; set; }
        //public DateTime IssueDate { get; set; }
        //public DateTime AssignedDate { get; set; }
        //public DateTime ApprovingDate { get; set; }
        //public DateTime ProposedClosingDate { get; set; }
        //public string Status { get; set; }
        //public virtual IEnumerable<IssueAttachedModel> issue_attachements { get; set; }
        public virtual IEnumerable<IssueStatusModel> issue_all_status { get; set; }
    }
}