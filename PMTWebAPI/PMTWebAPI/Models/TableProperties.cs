using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.Models
{
    public class TableProperties
    {
    }

    public class UserRolesMaster
    {
        public Guid UserRole_ID { get; set; }
        public string UserRole_Desc { get; set; }
        public string UserRole_Name { get; set; }
    }

    public class UserDetails
    {
        public Guid UserUID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public string Phonenumber { get; set; }
        public string Mobilenumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Username { get; set; }
        public string password { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DeletedFlag { get; set; }
        public string TypeOfUser { get; set; }
        public string Profile_Pic { get; set; }
        public Nullable<Guid> Admin_Under { get; set; }
    }

    public class ProjectCategory
    {
        public Guid ProjectClass_UID { get; set; }
        public string ProjectClass_Name { get; set; }
        public string ProjectClass_Description { get; set; }
        public string Delete_Flag { get; set; }
    }

    public class ProjectDetails
    {
        public Guid ProjectUID { get; set; }
        public Guid ProjectClass_UID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectAbbrevation { get; set; }
        public List<WorkPackages> workPackages = new List<WorkPackages>();
        //public string OwnerName { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime PlannedEndDate { get; set; }
        //public DateTime ProjectedEndDate { get; set; }
        //public string Status { get; set; }
        //public string Currency { get; set; }
        //public string Currency_CultureInfo { get; set; }
        //public double Budget { get; set; }
        //public double ActualExpenditure { get; set; }
        //public string Funding_Agency { get; set; }
        //public string Delete_Flag { get; set; }

    }


    public class UserProjects
    {
        public Guid AssignID { get; set; }
        public Guid UserUID { get; set; }
        public Guid ProjectUID { get; set; }
        public string UserRole { get; set; }
        public DateTime AssignDate { get; set; }
    }

    public class Contractors
    {
        public Guid Contractor_UID { get; set; }
        public string Contractor_Name { get; set; }
        public string Contractor_Code { get; set; }
        public string Contractor_Representatives_Phone { get; set; }
        public string Contractor_Representatives { get; set; }
        public string NJSEI_Number { get; set; }
        public string ProjectSpecific_Number { get; set; }
        public string Type_of_Contract { get; set; }
        public string Currency { get; set; }
        public string Currency_CultureInfo { get; set; }
        public double Contract_Value { get; set; }
        public string Contractor_Address { get; set; }
        public DateTime Letter_of_Acceptance { get; set; }
        public DateTime Contract_Agreement_Date { get; set; }
        public int Contract_Duration { get; set; }
        public DateTime Contract_StartDate { get; set; }
        public DateTime Contract_Completion_Date { get; set; }
        public string Delete_Flag { get; set; }

    }

    public class MasterWorkPackages
    {
        public Guid MasterWorkPackageUID { get; set; }
        public Guid ProjectUID { get; set; }
        public string MasterWorkPackageName { get; set; }
        public string MasterWorkPackageCode { get; set; }
    }

    public class MasterLocations
    {
        public Guid LocationMasterUID { get; set; }
        public Guid ProjectUID { get; set; }
        public string LocationMaster_Name { get; set; }
        public string LocationMaster_Code { get; set; }
    }

    public class MasterClients
    {
        public Guid ClientMasterUID { get; set; }
        public Guid ProjectUID { get; set; }
        public string ClientMaster_Name { get; set; }
        public string ClientMaster_Code { get; set; }
    }

    public class WorkpackageOptions
    {
        public Guid Workpackage_OptionUID { get; set; }
        public string Workpackage_OptionName { get; set; }
        public int Workpackage_OptionOrder { get; set; }
        public string Workpackage_OptionFor { get; set; }
    }

    public class WorkPackages
    {
        public Guid WorkPackageUID { get; set; }
        public Guid ProjectUID { get; set; }
        //public Guid Workpackage_OptionUID { get; set; }
        //public Guid Contractor_UID { get; set; }
        //public Guid UserUID { get; set; }
        public string Name { get; set; }
        public List<WorkpackageSelectedOptions> selectedOptions = new List<WorkpackageSelectedOptions>();
        //public DateTime StartDate { get; set; }
        //public DateTime PlannedEndDate { get; set; }
        //public DateTime ProjectedEndDate { get; set; }
        //public string Status { get; set; }
        //public string Currency { get; set; }
        //public string Currency_CultureInfo { get; set; }
        //public double Budget { get; set; }
        //public double ActualExpenditure { get; set; }
        //public string WorkPackage_Location { get; set; }
        //public string WorkPackage_Client { get; set; }
        //public string Delete_Flag { get; set; }
        //public int WorkPackage_Order { get; set; }

    }

    public class WorkpackageSelectedOptions
    {
        public Guid WorkpackageSelectedOption_UID { get; set; }
        public Guid WorkPackageUID { get; set; }
        public Guid Workpackage_OptionUID { get; set; }
        public string WorkpackageSelectedOption_Name { get; set; }
        public List<Tasks> tasks = new List<Tasks>();
        //public int WorkpackageSelectedOption_Order { get; set; }
        //public string Delete_Flag { get; set; }
    }

    public class Tasks
    {
        public Guid TaskUID { get; set; }
        public Guid ProjectUID { get; set; }
        public Guid WorkPackageUID { get; set; }
        public Guid Workpackage_Option { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime PlannedEndDate { get; set; }
        //public DateTime ProjectedEndDate { get; set; }
        public string Status { get; set; }
        public List<ActivityMileStone> activityMileStones = new List<ActivityMileStone>();
        public List<ActivityResourcesAllocated> activityResourcesAllocateds = new List<ActivityResourcesAllocated>();
        public List<Tasks> SubTasks = new List<Tasks>();
        public List<FinancialMileStones> financialMileStones = new List<FinancialMileStones>();
        //public string Currency { get; set; }
        //public string Currency_CultureInfo { get; set; }
        //public double Basic_Budget { get; set; }
        //public float GST { get; set; }
        //public double Total_Budget { get; set; }
        //public double ActualExpenditure { get; set; }
        //public int TaskLevel { get; set; }
        //public Guid ParentTaskID { get; set; }
        //public float StatusPer { get; set; }
        //public string UnitforProgress { get; set; }
        //public float UnitQuantity { get; set; }
        //public DateTime PlannedStartDate { get; set; }
        //public DateTime ProjectedStartDate { get; set; }
        //public DateTime ActualEndDate { get; set; }
        //public string Discipline { get; set; }
        //public int Task_Weightage { get; set; }
        //public string Task_Type { get; set; }
        //public string Delete_Flag { get; set; }
        //public int Task_Order { get; set; }

    }

    public class ActivityMileStone
    {
        public Guid MileStoneUID { get; set; }
        public Guid TaskUID { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime MileStoneDate { get; set; }
    }
    public class ActivityResourcesAllocated
    {
        public Guid UID { get; set; }
        public Guid ResourceUID { get; set; }
        public Guid TaskUID { get; set; }
        public int AllocatedUnits { get; set; }
    }
    public class FinancialMileStones
    {
        public Guid Finance_MileStoneUID { get; set; }
        public Guid TaskUID { get; set; }
        public string Finance_MileStoneName { get; set; }
        public double Finance_AllowedPayment { get; set; }
        public DateTime Finance_MileStoneCreatedDate { get; set; }
    }
    public class Issues
    {
        public Guid Issue_Uid { get; set; }
        public Guid ProjectUID { get; set; }
        public Guid WorkPackagesUID { get; set; }
        public Guid TaskUID { get; set; }
        public string Issue_Description { get; set; }
        public DateTime Issue_Date { get; set; }
        public string Issue_Status { get; set; }
    }
    public class DocumentCategory
    {
        public string ProjectName { get; set; }
        public string status { get; set; }
        public string flowid { get; set; }
    }
    public class UserAppTokens
    {
        public string username { get; set; }
        public string deviceid { get; set; }
    }
    public class UserProjectDocuments
    {
        public string Username { get; set; }
        public string ProjectName { get; set; }
    }
    //added on 15/11/2022 for Venkat
    public class AddIssue
    {
        public string projectname { get; set; }
        public string Issuedescription { get; set; }

        public string reportinguser { get; set; }
        public string reportingdate { get; set; }

        public string issueproposedclosedate { get; set; }
        public string remarks { get; set; }
        public List<FileData> issuedocument { get; set; }
    }
    public class FileData
    {
        public string fileName { get; set; }
        public string filebase64 { get; set; }
    }

    public class ProjectWorkPackage
    {
        public string projectname { get; set; }
        public string workpackage { get; set; }

    }
    public class Issueid
    {
        public string Issueuid { get; set; }
    }
    public class UpdateIssueStatus
    {
        public string ProjectName { get; set; }
        public string issueuid { get; set; }
        public string issuestatus { get; set; }
        public string issuestatusupdatedate { get; set; }
        public string Issue_status_update_user_id { get; set; }
        public List<FileData> issue_status_document { get; set; }
        public string remarks { get; set; }

    }
}