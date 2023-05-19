using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTWebAPI.Models_Doc
{
    public class TableProperties_sub
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
       // public List<ActivityMileStone> activityMileStones = new List<ActivityMileStone>();
       // public List<ActivityResourcesAllocated> activityResourcesAllocateds = new List<ActivityResourcesAllocated>();
        public List<Tasks> SubTasks = new List<Tasks>();
        public List<Submittals> SubMittal = new List<Submittals>();
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
    }
    public class ActivityResourcesAllocated
    {
        public Guid UID { get; set; }
        public Guid ResourceUID { get; set; }
        public Guid TaskUID { get; set; }
        public int AllocatedUnits { get; set; }
    }

    // added by  Zuber
    public class Submittals
    {
        public Guid SubmittalUID { get; set; }
      //  public Guid WorkPackageUID { get; set; }
      //  public Guid ProjectUID { get; set; }
        public Guid TaskUID { get; set; }
        public string SubmittalName { get; set; }
        public Guid Submittal_Category { get; set; }
      //  public string Doc_ProjectRefNo { get; set; }
       // public string Doc_RefNumber { get; set; }
      //  public string Doc_Type { get; set; }
     //   public DateTime IncomingRec_Date { get; set; }
        public Guid FlowUID { get; set; }
        public string FlowName { get; set; }
        public DateTime Flow_StartDate { get; set; }
      //  public string Doc_Media_HC { get; set; }
      //  public string Doc_Media_SC { get; set; }
      //  public string Doc_Media_SCEF { get; set; }
      //  public string Doc_Media_HCR { get; set; }
       // public string Doc_Media_SCR { get; set; }
      //  public string Doc_Media_NA { get; set; }
       // public string Doc_FileRefNumber { get; set; }
      //  public string Doc_Remarks { get; set; }
      //  public DateTime CreatedDate { get; set; }
        public string Submitter_UserUID { get; set; }
        public string Submitter_UserName { get; set; }
        public string Submitter_TargetDate { get; set; }
        public string Reviewer_UserUID { get; set; }
        public string Reviewer_UserName { get; set; }
        public string Reviewer_TargetDate { get; set; }
        public string Approver_UserUID { get; set; }
        public string Approver_UserName { get; set; }
        public string Approver_TargetDate { get; set; }
        public List<Submittal_Documents> SubmittalDocuments = new List<Submittal_Documents>();
    }

    public class Submittal_Documents
    {
        public Guid DocumentUID { get; set; }
      //  public Guid ProjectUID { get; set; }
      //  public Guid WorkPackageUID { get; set; }
        public Guid SubmittalUID { get; set; }
        public string ProjectRef_Number { get; set; }
        public string Ref_Number { get; set; }
        public string Doc_Type { get; set; }
        public string IncomingRec_Date { get; set; }
     //   public Guid FlowUID { get; set; }
        public string ActualDocument_Name { get; set; }
        public string Description { get; set; }
        public float ActualDocument_Version { get; set; }
        public string ActualDocument_Type { get; set; }
        public string ActualDocument_Originator { get; set; }
        public string Media_HC { get; set; }
        public string Media_SC { get; set; }
        public string Media_SCEF { get; set; }
        public string Media_HCR { get; set; }
        public string Media_SCR { get; set; }
        public string Media_NA { get; set; }
        public string Document_Date { get; set; }
     //   public string ActualDocument_Path { get; set; }
        public string ActualDocument_RelativePath { get; set; }
        public string ActualDocument_DirectoryName { get; set; }
        public string Remarks { get; set; }
        public string FileRef_Number { get; set; }
        public string ActualDocument_CreatedDate { get; set; }
        public string ActualDocument_CurrentStatus { get; set; }
        //   public DateTime FlowStep1_TargetDate { get; set; }
        // public DateTime FlowStep2_TargetDate { get; set; }
        // public DateTime FlowStep3_TargetDate { get; set; }
        public List<Documents_Status> DocumentHistoryStatus = new List<Documents_Status>();

    }

    public class Documents_Status
    {
        public Guid StatusUID { get; set; }
        public Guid DocumentUID { get; set; }
        public Guid AcivityUserUID { get; set; }
        public string AcivityUserName { get; set; }
        public string Ref_Number { get; set; }
        public float Version { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDate { get; set; }
        public string DocumentDate { get; set; }
      //  public string CoverLetterFile { get; set; }
      //  public string LinkToReviewFile { get; set; }
        public string Status_Comments { get; set; }
        public string Current_Status { get; set; }
        public List<DocumentVersion> Documents_Status_Version = new List<DocumentVersion>();
    }

    public class DocumentVersion
    {
        public Guid DocVersion_UID { get; set; }
        public Guid DocStatus_UID { get; set; }
      //  public Guid DocumentUID { get; set; }
        public float Doc_Version { get; set; }
        public string Doc_Type { get; set; }
      //  public string Doc_FileName { get; set; }
        public string Doc_Status { get; set; }
        public string Doc_StatusDate { get; set; }
        public string Doc_Comments { get; set; }
    }
}