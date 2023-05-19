using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.Controllers
{
    public class Issue
    {
        public Guid IssueUId { get; set; }
        public string IssueDescription { get; set; }
        public string ReportingUser { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Status { get; set; }
    }
}