using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using PMTWebAPI.DAL;
using PMTWebAPI.Models_Doc;




namespace PMTWebAPI.Controllers
{
    public class FinalDataDoc
    {
        public Guid ProjectCategoryUID { get; set; }
        public string ProjectCategoryName { get; set; }
        public List<ProjectDetails> projectDetails { get; set; } = new List<ProjectDetails>();
       
    }
    public class DocumentController : ApiController
    {
        DBGetData db = new DBGetData();
        TableProperties_sub tp = new TableProperties_sub();

        public string GetIp()
        {
            return GetClientIp();
        }

        private string GetClientIp(HttpRequestMessage request = null)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public IHttpActionResult getSubmittals()
        {
            List<FinalDataDoc> fData = new List<FinalDataDoc>();
           
            var httpRequest = HttpContext.Current.Request;
            var UserName = httpRequest.Params["UserName"];
            var Password = Security.Encrypt(httpRequest.Params["Password"]);
            DataSet ds = new DataSet();
            ds = db.CheckLogin(UserName, Password);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
                string UserType = ds.Tables[0].Rows[0]["TypeOfUser"].ToString();
                if (UserType == "U" || UserType == "MD" || UserType == "VP")
                {
                    ds = db.ProjectClass_Select_All();
                }
                else if (UserType == "PA")
                {
                    ds = db.ProjectClass_Select_By_UserUID(new Guid(UserUID));
                }
                else
                {
                    ds = db.ProjectClass_Select_By_UserUID(new Guid(UserUID));
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    //JSONString += "'ProjectCategory': {'ProjectCategoryUID':'" + new Guid(ds.Tables[0].Rows[i]["ProjectClass_UID"].ToString()) + "','ProjectCategoryName':'" + ds.Tables[0].Rows[i]["ProjectClass_Name"].ToString() + "'}";

                    //Category += "{ 'ProjectCategoryUID': '" + new Guid(ds.Tables[0].Rows[i]["ProjectClass_UID"].ToString()) + "', 'ProjectCategoryName': '" + ds.Tables[0].Rows[i]["ProjectClass_Name"].ToString() + "'},";

                    FinalDataDoc fd = new FinalDataDoc();
                    fd.ProjectCategoryUID = new Guid(ds.Tables[0].Rows[i]["ProjectClass_UID"].ToString());
                    fd.ProjectCategoryName = ds.Tables[0].Rows[i]["ProjectClass_Name"].ToString();
                    DataSet dsProject = new DataSet();
                    if (UserType == "U" || UserType == "MD" || UserType == "VP")
                    {
                        dsProject = db.GetProjects_by_ClassUID(new Guid(ds.Tables[0].Rows[i]["ProjectClass_UID"].ToString()));
                    }
                    else
                    {
                        dsProject = db.GetUserProjects_by_ClassUID(new Guid(ds.Tables[0].Rows[i]["ProjectClass_UID"].ToString()), new Guid(UserUID));
                    }


                    for (int j = 0; j < dsProject.Tables[0].Rows.Count; j++)
                    {
                        //Projects += "{ 'ProjectUID': '" + new Guid(dsProject.Tables[0].Rows[j]["ProjectUID"].ToString()) + "', 'ProjectClass_UID': '" + dsProject.Tables[0].Rows[j]["ProjectClass_UID"].ToString() + "', 'ProjectName': '" + dsProject.Tables[0].Rows[j]["ProjectName"].ToString() + "'},";
                        // JSONString += "'ProjectDetails': { 'ProjectUID': '" + new Guid(dsProject.Tables[0].Rows[j]["ProjectUID"].ToString()) + "', 'ProjectClass_UID': '" + new Guid(dsProject.Tables[0].Rows[j]["ProjectClass_UID"].ToString()) + "', 'ProjectName': '" + dsProject.Tables[0].Rows[j]["Name"].ToString() + "' }";
                        ProjectDetails pd = new ProjectDetails
                        {
                            ProjectUID = new Guid(dsProject.Tables[0].Rows[j]["ProjectUID"].ToString()),
                            ProjectClass_UID = new Guid(dsProject.Tables[0].Rows[j]["ProjectClass_UID"].ToString()),
                            ProjectName = dsProject.Tables[0].Rows[j]["Name"].ToString(),
                            ProjectAbbrevation = dsProject.Tables[0].Rows[j]["ProjectAbbrevation"].ToString()
                        };
                        fd.projectDetails.Add(pd);
                        //fd.projectDetails.Add(pd);
                        //fd.projectDetails = new List<ProjectDetails>
                        //{
                        //    pd
                        //};

                        DataSet dsworkPackage = db.GetWorkPackages_By_ProjectUID(new Guid(dsProject.Tables[0].Rows[j]["ProjectUID"].ToString()));
                        for (int k = 0; k < dsworkPackage.Tables[0].Rows.Count; k++)
                        {
                            WorkPackages wp = new WorkPackages();
                            wp.WorkPackageUID = new Guid(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString());
                            wp.ProjectUID = new Guid(dsworkPackage.Tables[0].Rows[k]["ProjectUID"].ToString());
                            wp.Name = dsworkPackage.Tables[0].Rows[k]["Name"].ToString();
                            //fd.workPackages = new List<WorkPackages>
                            //{
                            //    wp
                            //};
                            pd.workPackages.Add(wp);

                            //fd.workPackages.Add(wp);
                            DataSet dsoption = db.GetSelectedOption_By_WorkpackageUID(new Guid(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString()));
                            for (int l = 0; l < dsoption.Tables[0].Rows.Count; l++)
                            {
                                WorkpackageSelectedOptions so = new WorkpackageSelectedOptions();
                                so.WorkpackageSelectedOption_UID = new Guid(dsoption.Tables[0].Rows[l]["WorkpackageSelectedOption_UID"].ToString());
                                so.WorkPackageUID = new Guid(dsoption.Tables[0].Rows[l]["WorkPackageUID"].ToString());
                                so.Workpackage_OptionUID = new Guid(dsoption.Tables[0].Rows[l]["Workpackage_OptionUID"].ToString());
                                so.WorkpackageSelectedOption_Name = dsoption.Tables[0].Rows[l]["WorkpackageSelectedOption_Name"].ToString();
                                //fd.selectedOptions = new List<WorkpackageSelectedOptions>
                                //{
                                //    so
                                //};
                                //fd.selectedOptions.Add(so);
                                wp.selectedOptions.Add(so);
                                //fd.workPackages.Add(wp);
                                DataSet dstask = db.GetTasks_by_WorkpackageOptionUID(new Guid(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString()), new Guid(dsoption.Tables[0].Rows[l]["Workpackage_OptionUID"].ToString()));
                                for (int m = 0; m < dstask.Tables[0].Rows.Count; m++)
                                {
                                    Tasks tk = new Tasks()
                                    {
                                        TaskUID = new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()),
                                        ProjectUID = new Guid(dstask.Tables[0].Rows[m]["ProjectUID"].ToString()),
                                        WorkPackageUID = new Guid(dstask.Tables[0].Rows[m]["WorkPackageUID"].ToString()),
                                        Workpackage_Option = new Guid(dstask.Tables[0].Rows[m]["Workpackage_Option"].ToString()),
                                        Name = dstask.Tables[0].Rows[m]["Name"].ToString(),
                                        Description = dstask.Tables[0].Rows[m]["Description"].ToString(),
                                        Status = dstask.Tables[0].Rows[m]["Status"].ToString(),
                                        Owner = dstask.Tables[0].Rows[m]["Owner"].ToString()
                                    };
                                    so.tasks.Add(tk);

                                    DataSet Submittals = db.getDocumentsForTasks(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()));
                                    for (int n= 0;n < Submittals.Tables[0].Rows.Count; n++)
                                    {
                                        Submittals SubM = new Submittals();
                                        SubM.TaskUID = new Guid(Submittals.Tables[0].Rows[n]["TaskUID"].ToString());
                                        SubM.SubmittalUID = new Guid(Submittals.Tables[0].Rows[n]["DocumentUID"].ToString());
                                        SubM.SubmittalName = Submittals.Tables[0].Rows[n]["DocName"].ToString();
                                        SubM.Submittal_Category = new Guid(Submittals.Tables[0].Rows[n]["Doc_Category"].ToString());
                                        SubM.FlowUID = new Guid(Submittals.Tables[0].Rows[n]["FlowUID"].ToString());
                                        SubM.FlowName = db.GetFlowName_by_SubmittalID(new Guid(Submittals.Tables[0].Rows[n]["DocumentUID"].ToString()));
                                        SubM.Flow_StartDate = Convert.ToDateTime(Submittals.Tables[0].Rows[n]["Flow_StartDate"].ToString());
                                        SubM.Submitter_UserUID = Submittals.Tables[0].Rows[n]["FlowStep1_UserUID"].ToString();
                                        SubM.Submitter_UserName = db.getUserNameby_UID(new Guid(Submittals.Tables[0].Rows[n]["FlowStep1_UserUID"].ToString()));
                                        SubM.Submitter_TargetDate = Submittals.Tables[0].Rows[n]["FlowStep1_TargetDate"].ToString();
                                        SubM.Reviewer_UserUID = Submittals.Tables[0].Rows[n]["FlowStep2_UserUID"] != DBNull.Value ? Submittals.Tables[0].Rows[n]["FlowStep2_UserUID"].ToString() : "";
                                        SubM.Reviewer_UserName = Submittals.Tables[0].Rows[n]["FlowStep2_UserUID"] != DBNull.Value ? db.getUserNameby_UID(new Guid(Submittals.Tables[0].Rows[n]["FlowStep2_UserUID"].ToString())) : "";
                                        SubM.Reviewer_TargetDate = Submittals.Tables[0].Rows[n]["FlowStep2_TargetDate"].ToString();
                                        SubM.Approver_UserUID = Submittals.Tables[0].Rows[n]["FlowStep3_UserUID"] != DBNull.Value ? Submittals.Tables[0].Rows[n]["FlowStep3_UserUID"].ToString() : "";
                                        SubM.Reviewer_UserName = Submittals.Tables[0].Rows[n]["FlowStep3_UserUID"] != DBNull.Value ? db.getUserNameby_UID(new Guid(Submittals.Tables[0].Rows[n]["FlowStep3_UserUID"].ToString())) : "";
                                        SubM.Approver_TargetDate = Submittals.Tables[0].Rows[n]["FlowStep3_TargetDate"].ToString();

                                        DataSet SubmDocuments = db.ActualDocuments_SelectBy_DocumentUID(new Guid(Submittals.Tables[0].Rows[n]["DocumentUID"].ToString()));
                                        for(int s=0; s < SubmDocuments.Tables[0].Rows.Count; s++)
                                        {
                                            Submittal_Documents SubDoc = new Submittal_Documents();
                                            SubDoc.SubmittalUID = new Guid(SubmDocuments.Tables[0].Rows[s]["DocumentUID"].ToString());
                                            SubDoc.DocumentUID = new Guid(SubmDocuments.Tables[0].Rows[s]["ActualDocumentUID"].ToString());
                                            SubDoc.Description = SubmDocuments.Tables[0].Rows[s]["Description"].ToString();
                                            SubDoc.Ref_Number = SubmDocuments.Tables[0].Rows[s]["Ref_Number"].ToString();
                                            SubDoc.ProjectRef_Number = SubmDocuments.Tables[0].Rows[s]["ProjectRef_Number"].ToString();
                                            SubDoc.FileRef_Number = SubmDocuments.Tables[0].Rows[s]["FileRef_Number"].ToString();
                                            SubDoc.Remarks = SubmDocuments.Tables[0].Rows[s]["Remarks"].ToString();
                                            SubDoc.Doc_Type = SubmDocuments.Tables[0].Rows[s]["Doc_Type"].ToString();
                                            SubDoc.IncomingRec_Date = SubmDocuments.Tables[0].Rows[s]["IncomingRec_Date"].ToString();
                                            SubDoc.ActualDocument_Name = SubmDocuments.Tables[0].Rows[s]["ActualDocument_Name"].ToString();
                                            SubDoc.ActualDocument_Version =Convert.ToInt16(SubmDocuments.Tables[0].Rows[s]["ActualDocument_Version"].ToString());
                                            SubDoc.ActualDocument_Type = SubmDocuments.Tables[0].Rows[s]["ActualDocument_Type"].ToString();
                                            SubDoc.ActualDocument_Originator = SubmDocuments.Tables[0].Rows[s]["ActualDocument_Originator"].ToString();
                                            SubDoc.ActualDocument_CreatedDate = SubmDocuments.Tables[0].Rows[s]["ActualDocument_CreatedDate"].ToString();
                                            SubDoc.ActualDocument_RelativePath = SubmDocuments.Tables[0].Rows[s]["ActualDocument_RelativePath"].ToString();
                                          //  SubDoc.ActualDocument_Path = SubmDocuments.Tables[0].Rows[s]["ActualDocument_Path"].ToString();
                                            SubDoc.ActualDocument_DirectoryName = SubmDocuments.Tables[0].Rows[s]["ActualDocument_DirectoryName"].ToString();
                                            SubDoc.ActualDocument_CurrentStatus = SubmDocuments.Tables[0].Rows[s]["ActualDocument_CurrentStatus"].ToString();

                                            DataSet SubDocStatus = db.getActualDocumentStatusList(new Guid(SubmDocuments.Tables[0].Rows[s]["ActualDocumentUID"].ToString()));
                                            for (int d = 0; d < SubDocStatus.Tables[0].Rows.Count; d++)
                                            {
                                                Documents_Status docst = new Documents_Status();
                                                docst.StatusUID = new Guid(SubDocStatus.Tables[0].Rows[d]["StatusUID"].ToString());
                                                docst.DocumentUID = new Guid(SubDocStatus.Tables[0].Rows[d]["DocumentUID"].ToString());
                                                docst.AcivityUserUID = new Guid(SubDocStatus.Tables[0].Rows[d]["AcivityUserUID"].ToString());
                                                docst.AcivityUserName = db.getUserNameby_UID(new Guid(SubDocStatus.Tables[0].Rows[d]["AcivityUserUID"].ToString()));
                                                docst.Ref_Number = SubDocStatus.Tables[0].Rows[d]["Ref_Number"].ToString();
                                                docst.Version =Convert.ToInt16(SubDocStatus.Tables[0].Rows[d]["Version"].ToString());
                                                docst.ActivityType = SubDocStatus.Tables[0].Rows[d]["ActivityType"].ToString();
                                                docst.ActivityDate = SubDocStatus.Tables[0].Rows[d]["ActivityDate"].ToString();
                                                docst.DocumentDate = SubDocStatus.Tables[0].Rows[d]["DocumentDate"].ToString();
                                                docst.Status_Comments = SubDocStatus.Tables[0].Rows[d]["Status_Comments"].ToString();
                                                docst.Current_Status = SubDocStatus.Tables[0].Rows[d]["Current_Status"].ToString();

                                                DataSet DocVersion = db.getDocumentVersions_by_StatusUID(new Guid(SubDocStatus.Tables[0].Rows[d]["StatusUID"].ToString()));
                                                for (int dv= 0; dv < DocVersion.Tables[0].Rows.Count; dv++)
                                                {
                                                    DocumentVersion docver = new DocumentVersion();
                                                    docver.DocVersion_UID = new Guid(DocVersion.Tables[0].Rows[dv]["DocVersion_UID"].ToString());
                                                    docver.DocStatus_UID = new Guid(DocVersion.Tables[0].Rows[dv]["DocStatus_UID"].ToString());
                                                    docver.DocStatus_UID = new Guid(DocVersion.Tables[0].Rows[dv]["DocStatus_UID"].ToString());
                                                    docver.Doc_Comments = DocVersion.Tables[0].Rows[dv]["Doc_Comments"].ToString();
                                                    docver.Doc_Type =  DocVersion.Tables[0].Rows[dv]["Doc_Type"].ToString();
                                                    docver.Doc_Version =  Convert.ToInt16(DocVersion.Tables[0].Rows[dv]["Doc_Version"].ToString());
                                                    docver.Doc_Status = DocVersion.Tables[0].Rows[dv]["Doc_Status"].ToString();
                                                    docver.Doc_StatusDate = DocVersion.Tables[0].Rows[dv]["Doc_StatusDate"].ToString();
                                                    docst.Documents_Status_Version.Add(docver);
                                                }

                                                    SubDoc.DocumentHistoryStatus.Add(docst);
                                            }
                                                SubM.SubmittalDocuments.Add(SubDoc);
                                        }
                                        tk.SubMittal.Add(SubM);

                                        // add doceumnets to submittals
                                    }
                                   
                                    //DataSet Milsestones = db.getTaskMileStones(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()));
                                    //for (int n = 0; n < Milsestones.Tables[0].Rows.Count; n++)
                                    //{
                                    //    ActivityMileStone AM = new ActivityMileStone()
                                    //    {
                                    //        MileStoneUID = new Guid(Milsestones.Tables[0].Rows[n]["MileStoneUID"].ToString()),
                                    //        TaskUID = new Guid(Milsestones.Tables[0].Rows[n]["TaskUID"].ToString()),
                                    //        Description = Milsestones.Tables[0].Rows[n]["Description"].ToString(),
                                    //        Status = Milsestones.Tables[0].Rows[n]["Status"].ToString()
                                    //    };
                                    //    tk.activityMileStones.Add(AM);
                                    //}

                                    //DataSet Resources = db.getTaskResourceAllocated(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()));
                                    //for (int p = 0; p < Resources.Tables[0].Rows.Count; p++)
                                    //{
                                    //    ActivityResourcesAllocated ARA = new ActivityResourcesAllocated()
                                    //    {
                                    //        UID = new Guid(Resources.Tables[0].Rows[p]["UID"].ToString()),
                                    //        TaskUID = new Guid(Resources.Tables[0].Rows[p]["TaskUID"].ToString()),
                                    //        ResourceUID = new Guid(Resources.Tables[0].Rows[p]["ResourceUID"].ToString()),
                                    //        AllocatedUnits = Convert.ToInt32(Resources.Tables[0].Rows[p]["AllocatedUnits"].ToString())
                                    //    };
                                    //    tk.activityResourcesAllocateds.Add(ARA);
                                    //}

                                    //TaskDataAdd(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()), tk);
                                    //fd.Tasks.Add(tk);
                                    //fd.Tasks = new List<Tasks>
                                    //{
                                    //    tk
                                    //};

                                    //DataSet subtask = db.GetTask_by_ParentTaskUID(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()));

                                }
                            }
                        }
                    }


                    fData.Add(fd);
                }
                //Category = Category.TrimEnd(',') + "]";
                //Projects = Projects.TrimEnd(',') + "]";
                //JSONString += Category + Projects + " }]";
                //string str = "{ 'context_name': { 'lower_bound': 'value', 'pper_bound': 'value', 'values': [ 'value1', 'valueN' ] } }";
                //JSONString = "{ 'ProjectCategory': { 'ProjectCategoryUID': '5a0b9bb3-5dc1-4e78-b460-bee5c8febf7e', 'ProjectCategoryName': 'STP', 'ProjectDetails': { 'ProjectUID': '2bbfa1ef-b427-4e19-add1-97df91390f97', 'ProjectClass_UID': '5a0b9bb3-5dc1-4e78-b460-bee5c8febf7e', 'ProjectName': 'Test Project 1' },'ProjectDetails': { 'ProjectUID': 'd99cd5c7-2dec-42c5-afc1-706d33592f98', 'ProjectClass_UID': '5a0b9bb3-5dc1-4e78-b460-bee5c8febf7e', 'ProjectName': 'Test Project 2' } } }";
                //JavaScriptSerializer b = new JavaScriptSerializer();
                //object a = b.Deserialize(JSONString, typeof(object));
                //return Json(a);
                return Json(fData.ToList());
                //return Json(new
                //{
                //    Status = "Success",
                //    UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString(),
                //    TypeofUser = ds.Tables[0].Rows[0]["TypeOfUser"].ToString(),
                //    UserName = ds.Tables[0].Rows[0]["FirstName"].ToString()
                //});
            }
            else
            {
                var result = new
                {
                    Status = "Failure",
                    Message = "Invalid UserName or Password",
                };
                return Json(result);
            }
        }

        public void TaskDataAdd(Guid ParentTaskUID, Tasks tkparent)
        {
            DataSet ds = db.GetTask_by_ParentTaskUID(ParentTaskUID);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                Tasks tk = new Tasks()
                {
                    TaskUID = new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()),
                    ProjectUID = new Guid(ds.Tables[0].Rows[i]["ProjectUID"].ToString()),
                    WorkPackageUID = new Guid(ds.Tables[0].Rows[i]["WorkPackageUID"].ToString()),
                    Workpackage_Option = new Guid(ds.Tables[0].Rows[i]["Workpackage_Option"].ToString()),
                    Name = ds.Tables[0].Rows[i]["Name"].ToString(),
                    Description = ds.Tables[0].Rows[i]["Description"].ToString(),
                    Status = ds.Tables[0].Rows[i]["Status"].ToString(),
                    Owner = ds.Tables[0].Rows[i]["Owner"].ToString()
                };
                tkparent.SubTasks.Add(tk);

                DataSet Submittals = db.getDocumentsForTasks(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()));
                for (int n = 0; n < Submittals.Tables[0].Rows.Count; n++)
                {
                    Submittals SubM = new Submittals();
                    SubM.TaskUID = new Guid(Submittals.Tables[0].Rows[n]["TaskUID"].ToString());
                    SubM.SubmittalUID = new Guid(Submittals.Tables[0].Rows[n]["DocumentUID"].ToString());
                    SubM.SubmittalName = Submittals.Tables[0].Rows[n]["DocName"].ToString();
                    SubM.Submittal_Category = new Guid(Submittals.Tables[0].Rows[n]["Doc_Category"].ToString());
                    SubM.FlowUID = new Guid(Submittals.Tables[0].Rows[n]["FlowUID"].ToString());
                    SubM.FlowName = db.GetFlowName_by_SubmittalID(new Guid(Submittals.Tables[0].Rows[n]["DocumentUID"].ToString()));
                    SubM.Flow_StartDate = Convert.ToDateTime(Submittals.Tables[0].Rows[n]["Flow_StartDate"].ToString());
                    SubM.Submitter_UserUID = Submittals.Tables[0].Rows[n]["FlowStep1_UserUID"].ToString();
                    SubM.Submitter_UserName = db.getUserNameby_UID(new Guid(Submittals.Tables[0].Rows[n]["FlowStep1_UserUID"].ToString()));
                    SubM.Submitter_TargetDate = Submittals.Tables[0].Rows[n]["FlowStep1_TargetDate"].ToString();
                    SubM.Reviewer_UserUID = Submittals.Tables[0].Rows[n]["FlowStep2_UserUID"] != DBNull.Value ? Submittals.Tables[0].Rows[n]["FlowStep2_UserUID"].ToString() : "";
                    SubM.Reviewer_UserName = Submittals.Tables[0].Rows[n]["FlowStep2_UserUID"] != DBNull.Value ? db.getUserNameby_UID(new Guid(Submittals.Tables[0].Rows[n]["FlowStep2_UserUID"].ToString())) : "";
                    SubM.Reviewer_TargetDate = Submittals.Tables[0].Rows[n]["FlowStep2_TargetDate"].ToString();
                    SubM.Approver_UserUID = Submittals.Tables[0].Rows[n]["FlowStep3_UserUID"] != DBNull.Value ? Submittals.Tables[0].Rows[n]["FlowStep3_UserUID"].ToString() : "";
                    SubM.Reviewer_UserName = Submittals.Tables[0].Rows[n]["FlowStep3_UserUID"] != DBNull.Value ? db.getUserNameby_UID(new Guid(Submittals.Tables[0].Rows[n]["FlowStep3_UserUID"].ToString())) : "";
                    SubM.Approver_TargetDate = Submittals.Tables[0].Rows[n]["FlowStep3_TargetDate"].ToString();

                    DataSet SubmDocuments = db.ActualDocuments_SelectBy_DocumentUID(new Guid(Submittals.Tables[0].Rows[n]["DocumentUID"].ToString()));
                    for (int s = 0; s < SubmDocuments.Tables[0].Rows.Count; s++)
                    {
                        Submittal_Documents SubDoc = new Submittal_Documents();
                        SubDoc.SubmittalUID = new Guid(SubmDocuments.Tables[0].Rows[s]["DocumentUID"].ToString());
                        SubDoc.DocumentUID = new Guid(SubmDocuments.Tables[0].Rows[s]["ActualDocumentUID"].ToString());
                        SubDoc.Description = SubmDocuments.Tables[0].Rows[s]["Description"].ToString();
                        SubDoc.Ref_Number = SubmDocuments.Tables[0].Rows[s]["Ref_Number"].ToString();
                        SubDoc.ProjectRef_Number = SubmDocuments.Tables[0].Rows[s]["ProjectRef_Number"].ToString();
                        SubDoc.FileRef_Number = SubmDocuments.Tables[0].Rows[s]["FileRef_Number"].ToString();
                        SubDoc.Remarks = SubmDocuments.Tables[0].Rows[s]["Remarks"].ToString();
                        SubDoc.Doc_Type = SubmDocuments.Tables[0].Rows[s]["Doc_Type"].ToString();
                        SubDoc.IncomingRec_Date = SubmDocuments.Tables[0].Rows[s]["IncomingRec_Date"].ToString();
                        SubDoc.ActualDocument_Name = SubmDocuments.Tables[0].Rows[s]["ActualDocument_Name"].ToString();
                        SubDoc.ActualDocument_Version = Convert.ToInt16(SubmDocuments.Tables[0].Rows[s]["ActualDocument_Version"].ToString());
                        SubDoc.ActualDocument_Type = SubmDocuments.Tables[0].Rows[s]["ActualDocument_Type"].ToString();
                        SubDoc.ActualDocument_Originator = SubmDocuments.Tables[0].Rows[s]["ActualDocument_Originator"].ToString();
                        SubDoc.ActualDocument_CreatedDate = SubmDocuments.Tables[0].Rows[s]["ActualDocument_CreatedDate"].ToString();
                        SubDoc.ActualDocument_RelativePath = SubmDocuments.Tables[0].Rows[s]["ActualDocument_RelativePath"].ToString();
                        //  SubDoc.ActualDocument_Path = SubmDocuments.Tables[0].Rows[s]["ActualDocument_Path"].ToString();
                        SubDoc.ActualDocument_DirectoryName = SubmDocuments.Tables[0].Rows[s]["ActualDocument_DirectoryName"].ToString();
                        SubDoc.ActualDocument_CurrentStatus = SubmDocuments.Tables[0].Rows[s]["ActualDocument_CurrentStatus"].ToString();

                        DataSet SubDocStatus = db.getActualDocumentStatusList(new Guid(SubmDocuments.Tables[0].Rows[s]["ActualDocumentUID"].ToString()));
                        for (int d = 0; d < SubDocStatus.Tables[0].Rows.Count; d++)
                        {
                            Documents_Status docst = new Documents_Status();
                            docst.StatusUID = new Guid(SubDocStatus.Tables[0].Rows[d]["StatusUID"].ToString());
                            docst.DocumentUID = new Guid(SubDocStatus.Tables[0].Rows[d]["DocumentUID"].ToString());
                            docst.AcivityUserUID = new Guid(SubDocStatus.Tables[0].Rows[d]["AcivityUserUID"].ToString());
                            docst.AcivityUserName = db.getUserNameby_UID(new Guid(SubDocStatus.Tables[0].Rows[d]["AcivityUserUID"].ToString()));
                            docst.Ref_Number = SubDocStatus.Tables[0].Rows[d]["Ref_Number"].ToString();
                            docst.Version = Convert.ToInt16(SubDocStatus.Tables[0].Rows[d]["Version"].ToString());
                            docst.ActivityType = SubDocStatus.Tables[0].Rows[d]["ActivityType"].ToString();
                            docst.ActivityDate = SubDocStatus.Tables[0].Rows[d]["ActivityDate"].ToString();
                            docst.DocumentDate = SubDocStatus.Tables[0].Rows[d]["DocumentDate"].ToString();
                            docst.Status_Comments = SubDocStatus.Tables[0].Rows[d]["Status_Comments"].ToString();
                            docst.Current_Status = SubDocStatus.Tables[0].Rows[d]["Current_Status"].ToString();

                            DataSet DocVersion = db.getDocumentVersions_by_StatusUID(new Guid(SubDocStatus.Tables[0].Rows[d]["StatusUID"].ToString()));
                            for (int dv = 0; dv < DocVersion.Tables[0].Rows.Count; dv++)
                            {
                                DocumentVersion docver = new DocumentVersion();
                                docver.DocVersion_UID = new Guid(DocVersion.Tables[0].Rows[dv]["DocVersion_UID"].ToString());
                                docver.DocStatus_UID = new Guid(DocVersion.Tables[0].Rows[dv]["DocStatus_UID"].ToString());
                                docver.DocStatus_UID = new Guid(DocVersion.Tables[0].Rows[dv]["DocStatus_UID"].ToString());
                                docver.Doc_Comments = DocVersion.Tables[0].Rows[dv]["Doc_Comments"].ToString();
                                docver.Doc_Type = DocVersion.Tables[0].Rows[dv]["Doc_Type"].ToString();
                                docver.Doc_Version = Convert.ToInt16(DocVersion.Tables[0].Rows[dv]["Doc_Version"].ToString());
                                docver.Doc_Status = DocVersion.Tables[0].Rows[dv]["Doc_Status"].ToString();
                                docver.Doc_StatusDate = DocVersion.Tables[0].Rows[dv]["Doc_StatusDate"].ToString();
                                docst.Documents_Status_Version.Add(docver);
                            }

                            SubDoc.DocumentHistoryStatus.Add(docst);
                        }
                        SubM.SubmittalDocuments.Add(SubDoc);
                    }
                    tk.SubMittal.Add(SubM);
                }

                    // add doceumnets to submittals
                    //DataSet Milsestones = db.getTaskMileStones(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()));
                    //for (int n = 0; n < Milsestones.Tables[0].Rows.Count; n++)
                    //{
                    //    ActivityMileStone AM = new ActivityMileStone()
                    //    {
                    //        MileStoneUID = new Guid(Milsestones.Tables[0].Rows[n]["MileStoneUID"].ToString()),
                    //        TaskUID = new Guid(Milsestones.Tables[0].Rows[n]["TaskUID"].ToString()),
                    //        Description = Milsestones.Tables[0].Rows[n]["Description"].ToString(),
                    //        Status = Milsestones.Tables[0].Rows[n]["Status"].ToString()
                    //    };
                    //    tk.activityMileStones.Add(AM);
                    //}

                    //DataSet Resources = db.getTaskResourceAllocated(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()));
                    //for (int p = 0; p < Resources.Tables[0].Rows.Count; p++)
                    //{
                    //    ActivityResourcesAllocated ARA = new ActivityResourcesAllocated()
                    //    {
                    //        UID = new Guid(Resources.Tables[0].Rows[p]["UID"].ToString()),
                    //        TaskUID = new Guid(Resources.Tables[0].Rows[p]["TaskUID"].ToString()),
                    //        ResourceUID = new Guid(Resources.Tables[0].Rows[p]["ResourceUID"].ToString()),
                    //        AllocatedUnits = Convert.ToInt32(Resources.Tables[0].Rows[p]["AllocatedUnits"].ToString())
                    //    };
                    //    tk.activityResourcesAllocateds.Add(ARA);
                    //}

                    TaskDataAdd(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()), tk);
            }
        }

        //[HttpPost] // this is for adding submittals
        //public IHttpActionResult AddSubmittals()
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        var UserName = httpRequest.Params["UserName"];
        //        var Password = Security.Encrypt(httpRequest.Params["Password"]);
        //        DataSet ds = new DataSet();
        //        ds = db.CheckLogin(UserName, Password);
        //        if (ds.Tables[0].Rows.Count == 0)
        //        {
        //            var resultmain = new
        //            {
        //                Status = "Failure",
        //                Message = "Invalid UserName or Password",
        //            };
        //            return Json(resultmain);
        //        }
        //        if(httpRequest.Params["SubmittalName"] == null)
        //        {
        //            var resultmain = new
        //            {
        //                Status = "Failure",
        //                Message = "Please Enter Submittal Name !",
        //            };
        //            return Json(resultmain);
        //        }
        //        if (httpRequest.Params["SubmittalCategory"] == null)
        //        {
        //            var resultmain = new
        //            {
        //                Status = "Failure",
        //                Message = "Please Enter Submittal Category !",
        //            };
        //            return Json(resultmain);
        //        }
        //        if (httpRequest.Params["FlowUID"] == null)
        //        {
        //            var resultmain = new
        //            {
        //                Status = "Failure",
        //                Message = "Please Enter FlowUID !",
        //            };
        //            return Json(resultmain);
        //        }
        //        if (httpRequest.Params["SubmitterUID"] == null)
        //        {
        //            var resultmain = new
        //            {
        //                Status = "Failure",
        //                Message = "Please Enter SubmitterUID !",
        //            };
        //            return Json(resultmain);
        //        }
        //        if (httpRequest.Params["SubmitterTargetDate"] == null)
        //        {
        //            var resultmain = new
        //            {
        //                Status = "Failure",
        //                Message = "Please Enter SubmitterTargetDate !",
        //            };
        //            return Json(resultmain);
        //        }

        //        var taskUID =  httpRequest.Params["taskUID"];
        //        var SubmittalName = httpRequest.Params["SubmittalName"];
        //        var SubmittalCategory   = httpRequest.Params["SubmittalCategory"];
        //        var FlowUID = httpRequest.Params["FlowUID"];
        //        var sDocumentUID = Guid.NewGuid();
        //        var SubmitterUID = httpRequest.Params["SubmitterUID"];
        //        var SubmitterTargetDate = httpRequest.Params["SubmitterTargetDate"];
        //        var ReviewerUID = httpRequest.Params["ReviewerUID"];
        //        var RevieweTargetDate = httpRequest.Params["RevieweTargetDate"];
        //        var ApproverUID = httpRequest.Params["ApproverUID"];
        //        var ApproverTargetDate = httpRequest.Params["ApproverTargetDate"];
        //        var EstimatedDocuments = httpRequest.Params["EstimatedDocuments"];
        //        var Remarks = httpRequest.Params["Remarks"];
        //        var DocumentSearchType = httpRequest.Params["SubmittalDocType"];
        //        Guid projectId = Guid.Empty;
        //        Guid workpackageid = Guid.Empty;
        //        string sDate1 = "", sDate2 = "", sDate3 = "", sDate4 = "", sDate5 = "", DocStartString = "";
        //        DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now, CDate4 = DateTime.Now, CDate5 = DateTime.Now, DocStartDate = DateTime.Now;
        //        DataTable dttasks = db.GetTaskDetails_TaskUID(taskUID);
        //        if (dttasks.Rows.Count > 0)
        //        {
        //            projectId = new Guid(dttasks.Rows[0]["ProjectUID"].ToString());
        //            workpackageid = new Guid(dttasks.Rows[0]["workPackageUID"].ToString());
                   
        //        }
        //        DataSet dsFlow = db.GetDocumentFlows_by_UID(new Guid(FlowUID));
        //        int result = 0;
        //        if (dsFlow.Tables[0].Rows.Count > 0)
        //        {
        //            DocStartString = DateTime.Now.ToString("dd/MM/yyyy");
        //            DocStartString = DocStartString.Split('/')[1] + "/" + DocStartString.Split('/')[0] + "/" + DocStartString.Split('/')[2];
        //            DocStartDate = Convert.ToDateTime(DocStartString);


        //            //
        //            //sDate1 = dtSubTargetDate.Text;
        //            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
        //            //CDate1 = Convert.ToDateTime(sDate1);
        //            ////

        //            //sDate2 = dtQualTargetDate.Text;
        //            //sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
        //            //CDate2 = Convert.ToDateTime(sDate2);

        //            //sDate3 = dtRev_B_TargetDate.Text;
        //            //sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
        //            //CDate3 = Convert.ToDateTime(sDate3);

        //            //result = getdata.DoumentMaster_Insert_or_Update_Flow_2(sDocumentUID, workpackageid, projectId, TaskUID, txtDocumentName.Text, new Guid(DDLDocumentCategory.SelectedValue),
        //            //"", "Submittle Document", 0.0, new Guid(DDLDocumentFlow.SelectedValue), DocStartDate, new Guid(ddlSubmissionUSer.SelectedValue), CDate1,
        //            //new Guid(ddlQualityEngg.SelectedValue), CDate2, new Guid(ddlReviewer_B.SelectedValue), CDate3);
                   
        //            if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "1")
        //            {
        //                //
                       

        //                sDate1 =SubmitterTargetDate;
        //                sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
        //                CDate1 = Convert.ToDateTime(sDate1);
        //                result = db.DoumentMaster_Insert_or_Update_Flow_0(sDocumentUID, workpackageid, projectId, new Guid(taskUID), SubmittalName, new Guid(SubmittalCategory),
        //                "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1,Convert.ToInt16(EstimatedDocuments),Remarks,DocumentSearchType);

        //            }
        //            else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "2")
        //            {
        //                //
                       
        //                if (httpRequest.Params["ApproverUID"] == null)
        //                {
        //                    var resultmain = new
        //                    {
        //                        Status = "Failure",
        //                        Message = "Please Enter ApproverUID !",
        //                    };
        //                    return Json(resultmain);
        //                }
        //                if (httpRequest.Params["ApproverTargetDate"] == null)
        //                {
        //                    var resultmain = new
        //                    {
        //                        Status = "Failure",
        //                        Message = "Please Enter Approver TargetDate !",
        //                    };
        //                    return Json(resultmain);
        //                }
        //                sDate1 = SubmitterTargetDate;
        //                sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
        //                CDate1 = Convert.ToDateTime(sDate1);
        //                //
        //                sDate2 = ApproverTargetDate;
        //                sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
        //                CDate2 = Convert.ToDateTime(sDate2);

        //                CDate2 = Convert.ToDateTime(sDate2);
        //                result = db.DoumentMaster_Insert_or_Update_Flow_1(sDocumentUID, workpackageid, projectId, new Guid(taskUID), SubmittalName, new Guid(SubmittalCategory),
        //                "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1,
        //                new Guid(ReviewerUID), CDate2, Convert.ToInt16(EstimatedDocuments), Remarks, DocumentSearchType);
        //            }
        //            else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "3")
        //            {
        //                //
        //                if (httpRequest.Params["ReviewerUID"] == null)
        //                {
        //                    var resultmain = new
        //                    {
        //                        Status = "Failure",
        //                        Message = "Please Enter ReviewerUID !",
        //                    };
        //                    return Json(resultmain);
        //                }
        //                if (httpRequest.Params["ReviewerTargetDate"] == null)
        //                {
        //                    var resultmain = new
        //                    {
        //                        Status = "Failure",
        //                        Message = "Please Enter Reviewer TargetDate !",
        //                    };
        //                    return Json(resultmain);
        //                }
        //                if (httpRequest.Params["ApproverUID"] == null)
        //                {
        //                    var resultmain = new
        //                    {
        //                        Status = "Failure",
        //                        Message = "Please Enter ApproverUID !",
        //                    };
        //                    return Json(resultmain);
        //                }
        //                if (httpRequest.Params["ApproverTargetDate"] == null)
        //                {
        //                    var resultmain = new
        //                    {
        //                        Status = "Failure",
        //                        Message = "Please Enter Approver TargetDate !",
        //                    };
        //                    return Json(resultmain);
        //                }
        //                //
        //                sDate1 = SubmitterTargetDate;
        //                sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
        //                CDate1 = Convert.ToDateTime(sDate1);
        //                //

        //                sDate2 = RevieweTargetDate;
        //                sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
        //                CDate2 = Convert.ToDateTime(sDate2);

        //                sDate3 = ApproverTargetDate;
        //                sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
        //                CDate3 = Convert.ToDateTime(sDate3);

        //                result = db.DoumentMaster_Insert_or_Update_Flow_2(sDocumentUID, workpackageid, projectId, new Guid(taskUID), SubmittalName, new Guid(SubmittalCategory),
        //                "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1,
        //                new Guid(ReviewerUID), CDate2, new Guid(ApproverUID), CDate3, Convert.ToInt16(EstimatedDocuments), Remarks, DocumentSearchType);
        //            }
        //            //else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "4")
        //            //{
        //            //    //
        //            //    sDate1 = dtSubTargetDate.Text;
        //            //    sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
        //            //    CDate1 = Convert.ToDateTime(sDate1);

        //            //    sDate2 = dtQualTargetDate.Text;
        //            //    sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
        //            //    CDate2 = Convert.ToDateTime(sDate2);

        //            //    //
        //            //    sDate3 = dtRev_B_TargetDate.Text;
        //            //    sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
        //            //    CDate3 = Convert.ToDateTime(sDate3);

        //            //    //
        //            //    sDate4 = dtRevTargetDate.Text;
        //            //    sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
        //            //    CDate4 = Convert.ToDateTime(sDate4);

        //            //    result = getdata.DoumentMaster_Insert_or_Update_Flow_3(sDocumentUID, workpackageid, projectId, TaskUID, txtDocumentName.Text, new Guid(DDLDocumentCategory.SelectedValue),
        //            //    "", "Submittle Document", 0.0, new Guid(DDLDocumentFlow.SelectedValue), DocStartDate, new Guid(ddlSubmissionUSer.SelectedValue), CDate1,
        //            //    new Guid(ddlQualityEngg.SelectedValue), CDate2, new Guid(ddlReviewer_B.SelectedValue), CDate3, new Guid(ddlReviewer.SelectedValue), CDate4);
        //            //}
        //            //else
        //            //{

        //            //    sDate1 = dtSubTargetDate.Text;
        //            //    sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
        //            //    CDate1 = Convert.ToDateTime(sDate1);
        //            //    //

        //            //    sDate2 = dtQualTargetDate.Text;
        //            //    sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
        //            //    CDate2 = Convert.ToDateTime(sDate2);

        //            //    sDate3 = dtRev_B_TargetDate.Text;
        //            //    sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
        //            //    CDate3 = Convert.ToDateTime(sDate3);


        //            //    sDate4 = dtRevTargetDate.Text;
        //            //    sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
        //            //    CDate4 = Convert.ToDateTime(sDate4);
        //            //    //
        //            //    sDate5 = dtAppTargetDate.Text;
        //            //    sDate5 = sDate5.Split('/')[1] + "/" + sDate5.Split('/')[0] + "/" + sDate5.Split('/')[2];
        //            //    CDate5 = Convert.ToDateTime(sDate5);

        //            //    result = getdata.DoumentMaster_Insert_or_Update_Flow_4(sDocumentUID, workpackageid, projectId, TaskUID, txtDocumentName.Text, new Guid(DDLDocumentCategory.SelectedValue),
        //            //    "", "Submittle Document", 0.0, new Guid(DDLDocumentFlow.SelectedValue), DocStartDate, new Guid(ddlSubmissionUSer.SelectedValue), CDate1,
        //            //    new Guid(ddlQualityEngg.SelectedValue), CDate2, new Guid(ddlReviewer_B.SelectedValue), CDate3, new Guid(ddlReviewer.SelectedValue), CDate4, new Guid(ddlApproval.SelectedValue), CDate5);
        //            //}
        //        }
              
             
        //        var result_1 = new
        //        {
        //            Status = "Success",
        //            Message = "Submittal Added Sucessfully !",
        //        };
        //        return Json(result_1);

        //    }
        //    catch(Exception ex)
        //    {
        //        var result = new
        //        {
        //            Status = "Failure",
        //            Message = ex.Message,
        //        };
        //        return Json(result);
        //    }
        //}

        [HttpPost] // This is for adding documents for Submittals
        public IHttpActionResult AddDocuments()
        {
            try
            {
                // Please  SubmittalUID is nothing but documentUID in the db.....
                var httpRequest = HttpContext.Current.Request;
                var UserName = httpRequest.Params["UserName"];
                var Password = Security.Encrypt(httpRequest.Params["Password"]);
                var UserUID = "";
                DataSet dslogin = new DataSet();
                dslogin = db.CheckLogin(UserName, Password);
                string CoverLetterUID = string.Empty;
                if (dslogin.Tables[0].Rows.Count == 0)
                {
                   
                    var result = new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    };
                    return Json(result);
                }
                else
                {
                    UserUID = dslogin.Tables[0].Rows[0]["UserUID"].ToString();
                }
                var SubmittalUID = httpRequest.Params["SubmittalUID"];
               
                var ProjectUID = httpRequest.Params["ProjectUID"];
                var WorkPackageUID = httpRequest.Params["WorkPackageUID"];
                var Description = httpRequest.Params["Description"];
                var DocumentType = httpRequest.Params["DocumentType"];
                var ReferenceNumber = httpRequest.Params["ReferenceNumber"];
                var DocumentMedia = httpRequest.Params["DocumentMedia"];
                var FileReferenceNumber = httpRequest.Params["FileReferenceNumber"];
                var Remarks = httpRequest.Params["Remarks"];
                var DocMedia_HardCopy =Convert.ToBoolean(httpRequest.Params["DocMedia_HardCopy"]);
                var DocMedia_SoftCopy_PDF = Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_PDF"]);
                var DocMedia_SoftCopy_Editable = Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_Editable"]);
                var DocMedia_SoftCopy_Ref = Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_PDF"]);
                var DocMedia_HardCopy_Ref = Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_PDF"]);
                var DocMedia_NoMedia = Convert.ToBoolean(httpRequest.Params["DocMedia_NoMedia"]);
                var Originator = httpRequest.Params["Originator"];
                var OriginatorRefNo = httpRequest.Params["OriginatorRefNo"];
                var IncomingReciveDate = httpRequest.Params["IncomingReciveDate"];
                var CoverLetterDate = httpRequest.Params["CoverLetterDate"];
                //
                bool coverlttr = false;
                bool filedoc = false;
                if(string.IsNullOrEmpty(Description))
                {
                    var result1 = new
                    {
                        Status = "Failure",
                        Message = "Please Enter Description !",
                    };
                    return Json(result1);
                }

                if (DocumentType == "Cover Letter")
                {
                    if (string.IsNullOrEmpty(CoverLetterDate))
                    {
                        var result1 = new
                        {
                            Status = "Failure",
                            Message = "Please Enter Cover Letter Date !",
                        };
                        return Json(result1);
                    }
                    if (string.IsNullOrEmpty(Originator))
                    {
                        var result1 = new
                        {
                            Status = "Failure",
                            Message = "Please Originator !",
                        };
                        return Json(result1);
                    }
                    if (string.IsNullOrEmpty(OriginatorRefNo))
                    {
                        var result1 = new
                        {
                            Status = "Failure",
                            Message = "Please Originator Ref No !",
                        };
                        return Json(result1);
                    }

                    for (int i = 0; i < httpRequest.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = httpRequest.Files[i];

                        if (httpPostedFile != null)
                        {
                            if (httpPostedFile.FileName.Contains("Cover") || httpPostedFile.FileName.Contains("Cover_Letter") || httpPostedFile.FileName.Contains("Cover Letter"))
                            {
                                coverlttr = true;
                            }
                            else
                            {
                                filedoc = true;
                            }
                        }
                    }
                    if(!coverlttr)
                    {
                        var result1 = new
                        {
                            Status = "Failure",
                            Message = "Please upload cover letter !",
                        };
                        return Json(result1);
                    }
                    if (!filedoc)
                    {
                        var result1 = new
                        {
                            Status = "Failure",
                            Message = "Please upload document file !",
                        };
                        return Json(result1);
                    }
                }

                DataSet ds = db.getDocumentsbyDocID(new Guid(SubmittalUID));
                string sDate1 = "", sDate2 = "", sDate3 = "", sDate4 = "", sDate5 = "", CoverPagePath = "";
                DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now, CDate4 = DateTime.Now, CDate5 = DateTime.Now, DocStartDate = DateTime.Now;
                // 
                DateTime startdate;
                DateTime endate;
                double TimeDuration = 0;
             
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (httpRequest.Files.Count > 0)
                    {
                        string cStatus = "Submitted";
                        string FileDatetime = DateTime.Now.ToString("dd MMM yyyy hh-mm-ss tt");
                        string sDocumentPath = string.Empty;
                        string DocumentFor = "";

                        string IncomingRec_Date_String = string.Empty;
                        if (IncomingReciveDate != "")
                        {
                            IncomingRec_Date_String = IncomingReciveDate;
                        }
                        else
                        {
                            IncomingRec_Date_String = DateTime.Now.ToString("dd/MM/yyyy");
                        }
                        IncomingRec_Date_String = IncomingRec_Date_String.Split('/')[1] + "/" + IncomingRec_Date_String.Split('/')[0] + "/" + IncomingRec_Date_String.Split('/')[2];
                        DateTime IncomingRec_Date = Convert.ToDateTime(IncomingRec_Date_String);
                        for (int i = 0; i < httpRequest.Files.Count; i++)
                        {
                            HttpPostedFile httpPostedFile = httpRequest.Files[i];
                          
                            if (httpPostedFile != null)
                            {
                                if ((httpPostedFile.FileName.Contains("Cover") || httpPostedFile.FileName.Contains("Cover_Letter")) && DocumentType == "Cover Letter")
                                {

                                    string DocumentDate = string.Empty;

                                    DocumentFor = "Cover Letter";


                                    DocumentDate = CoverLetterDate;


                                    DocumentDate = DocumentDate.Split('/')[1] + "/" + DocumentDate.Split('/')[0] + "/" + DocumentDate.Split('/')[2];
                                    DateTime Document_Date = Convert.ToDateTime(DocumentDate);

                                    //if (Request.QueryString["tUID"] != "00000000-0000-0000-0000-000000000000")
                                    //{
                                    //    //sDocumentPath = "~/" + projectId + "/" + workpackageid + "/" + sDocumentUID + "/" + FileDatetime + "/CoverLetter";
                                    //    sDocumentPath = "~/" + projectId + "/" + workpackageid + "/CoverLetter";
                                    //}
                                    //else if (Request.QueryString["wUID"] != "00000000-0000-0000-0000-000000000000")
                                    //{
                                    //    //sDocumentPath = "~/" + workpackageid + "/" + sDocumentUID + "/" + FileDatetime + "/CoverLetter";
                                    //    sDocumentPath = "~/" + workpackageid + "/CoverLetter";
                                    //}
                                    //else if (Request.QueryString["pUID"] != "00000000-0000-0000-0000-000000000000")
                                    //{
                                    //    //sDocumentPath = "~/" + projectId + "/" + sDocumentUID + "/" + FileDatetime + "/CoverLetter";
                                    //    sDocumentPath = "~/" + projectId + "/CoverLetter";
                                    //}
                                    sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + ProjectUID + "//" + WorkPackageUID + "//CoverLetter";

                                    if (!Directory.Exists(sDocumentPath))
                                    {
                                        Directory.CreateDirectory(sDocumentPath);

                                    }

                                    string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                    string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                    httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_1_copy" + Extn);
                                    //FileUploadDoc.SaveAs(Server.MapPath("~/Documents/Encrypted/" + sDocumentUID + "_" + txtDocName.Text + "_1"  + "_enp" + InputFile));
                                    string savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
                                    CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;
                                    EncryptFile(savedPath,CoverPagePath);
                                    //

                                    savedPath = "~/" + ProjectUID + "/" + WorkPackageUID + "/CoverLetter" + "/" + sFileName + "_1_copy" + Extn;
                                    CoverPagePath = "~/" + ProjectUID + "/" + WorkPackageUID + "/CoverLetter" + "/" + sFileName + "_1" + Extn;
                                    //
                                    CoverLetterUID = Guid.NewGuid().ToString();
                                    int RetCount = db.DocumentCoverLetter_Insert_or_Update(new Guid(CoverLetterUID), new Guid(ProjectUID), new Guid(WorkPackageUID), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                                        DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn, DocMedia_HardCopy == true ? "true" : "false",
                                        DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                        DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks, FileReferenceNumber, cStatus, Originator, Document_Date);
                                    if (RetCount <= 0)
                                    {
                                        //Page.ClientScript.RegisterStartupScript(Page.GetType(), "Error", "<script language='javascript'>alert('Error:11,There is some problem while inserting document. Please contact administrator');</script>");
                                    }
                                }

                                else
                                {

                                    if (!checkDocumentExists(Path.GetFileNameWithoutExtension(httpPostedFile.FileName), SubmittalUID)) // added on 20/11/2020
                                    {
                                        startdate = DateTime.Now;

                                        if (DocumentType == "General Document")
                                        {
                                            DocumentFor = "General Document";
                                        }
                                        else
                                        {
                                            DocumentFor = "Document";
                                        }

                                        //sDocumentPath = "~/" + projectId + "/" + workpackageid + "/" + sDocumentUID + "/" + FileDatetime + "/Documents";
                                        sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + ProjectUID + "//" + WorkPackageUID + "//Documents";


                                        if (!Directory.Exists(sDocumentPath))
                                        {
                                            Directory.CreateDirectory(sDocumentPath);

                                        }

                                        string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                        string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);

                                        httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_1_copy" + Extn);
                                        //FileUploadDoc.SaveAs(Server.MapPath("~/Documents/Encrypted/" + sDocumentUID + "_" + txtDocName.Text + "_1" + "_enp" + InputFile));
                                        string savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
                                        CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;


                                        EncryptFile(savedPath, CoverPagePath);
                                        //
                                        endate = DateTime.Now;
                                        Guid ActualDocumentUID = Guid.NewGuid();
                                        TimeDuration = (endate - startdate).TotalSeconds;

                                        db.InsertIntoDocumentUploadLog(ActualDocumentUID, startdate, endate, new Guid(UserUID), (float)TimeDuration);
                                        //.................................
                                        string UploadFilePhysicalpath = CoverPagePath;
                                        //

                                        savedPath = "~/" + ProjectUID + "/" + WorkPackageUID + "/Documents" + "/" + sFileName + "_1_copy" + Extn;
                                        CoverPagePath = "~/" + ProjectUID + "/" + WorkPackageUID + "/Documents" + "/" + sFileName + "_1" + Extn;
                                        //
                                        string Flow1DisplayName = "", Flow2DisplayName = "", Flow3DisplayName = "";// Flow4DisplayName = "", Flow5DisplayName = "";
                                        DataSet dsFlow = db.GetDocumentFlows_by_UID(new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()));
                                        if (dsFlow.Tables[0].Rows.Count > 0)
                                        {
                                            if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "1")
                                            {
                                                Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();

                                                sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                                CDate1 = Convert.ToDateTime(sDate1);

                                                int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow1(ActualDocumentUID, new Guid(ProjectUID), new Guid(WorkPackageUID), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                                                DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                                 DocMedia_HardCopy == true ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                                DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks, FileReferenceNumber, cStatus,
                                                new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, Flow1DisplayName, Originator, Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy")), "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission");
                                                if (cnt <= 0)
                                                {
                                                    // Page.ClientScript.RegisterStartupScript(Page.GetType(), "Error", "<script language='javascript'>alert('Error:11,There is some problem while inserting document. Please contact administrator');</script>");
                                                }
                                            }
                                            //else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "2")
                                            //{
                                            //    Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
                                            //    Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();

                                            //    sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                            //    CDate1 = Convert.ToDateTime(sDate1);


                                            //    sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                                            //    CDate2 = Convert.ToDateTime(sDate2);

                                            //    int cnt = getdata.Document_Insert_or_Update_with_RelativePath_Flow2(ActualDocumentUID, new Guid(projectId), new Guid(workpackageid), new Guid(sDocumentUID), txtprefnumber.Text, txtRefNumber.Text,
                                            //      DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, txtdesc.Text, 1, Extn,
                                            //      DDLDocumentMedia.Items[0].Selected == true ? "true" : "false", DDLDocumentMedia.Items[1].Selected == true ? "true" : "false", DDLDocumentMedia.Items[2].Selected == true ? "true" : "false", DDLDocumentMedia.Items[3].Selected == true ? "true" : "false",
                                            //      DDLDocumentMedia.Items[4].Selected == true ? "true" : "false", DDLDocumentMedia.Items[5].Selected == true ? "true" : "false", CoverPagePath, txtremarks.Text, txtFileRefNumber.Text, cStatus,
                                            //      new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, Flow1DisplayName, Flow2DisplayName, RBLOriginator.SelectedValue, Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy")), "", "", UploadFilePhysicalpath);
                                            //    if (cnt <= 0)
                                            //    {
                                            //        Page.ClientScript.RegisterStartupScript(Page.GetType(), "Error", "<script language='javascript'>alert('Error:11,There is some problem while inserting document. Please contact administrator');</script>");
                                            //    }

                                            //}
                                            else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "3")
                                            {
                                                Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
                                                Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
                                                Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();

                                                sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                                CDate1 = Convert.ToDateTime(sDate1);


                                                sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                                                CDate2 = Convert.ToDateTime(sDate2);

                                                sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                                                CDate3 = Convert.ToDateTime(sDate3);

                                                sDate4 = DateTime.Now.ToString("dd/MM/yyyy");
                                                sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
                                                CDate4 = Convert.ToDateTime(CDate4);
                                                int cnt = db.Document_Insert_or_Update(ActualDocumentUID, new Guid(ProjectUID), new Guid(WorkPackageUID), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                                               DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                               DocMedia_HardCopy == true ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                               DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks, FileReferenceNumber, cStatus,
                                               new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3,
                                               Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Originator, CDate4, UploadFilePhysicalpath, CoverLetterUID, "Submissin");
                                                if (cnt <= 0)
                                                {
                                                    // Page.ClientScript.RegisterStartupScript(Page.GetType(), "Error", "<script language='javascript'>alert('Error:11,There is some problem while inserting document. Please contact administrator');</script>");
                                                }

                                            }
                                            //else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "4")
                                            //{
                                            //    Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
                                            //    Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
                                            //    Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();
                                            //    Flow4DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep4_DisplayName"].ToString();

                                            //    sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                            //    CDate1 = Convert.ToDateTime(sDate1);


                                            //    sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                                            //    CDate2 = Convert.ToDateTime(sDate2);

                                            //    sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                                            //    CDate3 = Convert.ToDateTime(sDate3);


                                            //    sDate4 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
                                            //    CDate4 = Convert.ToDateTime(sDate4);

                                            //    int cnt = getdata.Document_Insert_or_Update_with_RelativePath_Flow4(ActualDocumentUID, new Guid(projectId), new Guid(workpackageid), new Guid(sDocumentUID), txtprefnumber.Text, txtRefNumber.Text,
                                            //      DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, txtdesc.Text, 1, Extn,
                                            //      DDLDocumentMedia.Items[0].Selected == true ? "true" : "false", DDLDocumentMedia.Items[1].Selected == true ? "true" : "false", DDLDocumentMedia.Items[2].Selected == true ? "true" : "false", DDLDocumentMedia.Items[3].Selected == true ? "true" : "false",
                                            //      DDLDocumentMedia.Items[4].Selected == true ? "true" : "false", DDLDocumentMedia.Items[5].Selected == true ? "true" : "false", CoverPagePath, txtremarks.Text, txtFileRefNumber.Text, cStatus,
                                            //      new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3, new Guid(ds.Tables[0].Rows[0]["FlowStep4_UserUID"].ToString()), CDate4,
                                            //      Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Flow4DisplayName, RBLOriginator.SelectedValue, Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy")), "", "", UploadFilePhysicalpath);
                                            //    if (cnt <= 0)
                                            //    {
                                            //        Page.ClientScript.RegisterStartupScript(Page.GetType(), "Error", "<script language='javascript'>alert('Error:11,There is some problem while inserting document. Please contact administrator');</script>");
                                            //    }

                                            //}
                                            //else
                                            //{
                                            //    Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
                                            //    Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
                                            //    Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();
                                            //    Flow4DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep4_DisplayName"].ToString();
                                            //    Flow5DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep5_DisplayName"].ToString();

                                            //    sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                            //    CDate1 = Convert.ToDateTime(sDate1);


                                            //    sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                                            //    CDate2 = Convert.ToDateTime(sDate2);

                                            //    sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                                            //    CDate3 = Convert.ToDateTime(sDate3);


                                            //    sDate4 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
                                            //    CDate4 = Convert.ToDateTime(sDate4);

                                            //    sDate5 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                            //    sDate5 = sDate5.Split('/')[1] + "/" + sDate5.Split('/')[0] + "/" + sDate5.Split('/')[2];
                                            //    CDate5 = Convert.ToDateTime(sDate5);

                                            //    int cnt = getdata.Document_Insert_or_Update_with_RelativePath_Flow5(ActualDocumentUID, new Guid(projectId), new Guid(workpackageid), new Guid(sDocumentUID), txtprefnumber.Text, txtRefNumber.Text,
                                            //      DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, txtdesc.Text, 1, Extn,
                                            //      DDLDocumentMedia.Items[0].Selected == true ? "true" : "false", DDLDocumentMedia.Items[1].Selected == true ? "true" : "false", DDLDocumentMedia.Items[2].Selected == true ? "true" : "false", DDLDocumentMedia.Items[3].Selected == true ? "true" : "false",
                                            //      DDLDocumentMedia.Items[4].Selected == true ? "true" : "false", DDLDocumentMedia.Items[5].Selected == true ? "true" : "false", CoverPagePath, txtremarks.Text, txtFileRefNumber.Text, cStatus,
                                            //      new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3, new Guid(ds.Tables[0].Rows[0]["FlowStep4_UserUID"].ToString()), CDate4, new Guid(ds.Tables[0].Rows[0]["FlowStep5_UserUID"].ToString()), CDate5,
                                            //      Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Flow4DisplayName, Flow5DisplayName, RBLOriginator.SelectedValue, Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy")), "", "", UploadFilePhysicalpath);
                                            //    if (cnt <= 0)
                                            //    {
                                            //        Page.ClientScript.RegisterStartupScript(Page.GetType(), "Error", "<script language='javascript'>alert('Error:11,There is some problem while inserting document. Please contact administrator');</script>");
                                            //    }
                                            //}

                                        }
                                    }
                                    // contentType = httpContext.Request.Files[i].ContentType;
                                }
                            }

                        }
                        var result1 = new
                        {
                            Status = "Success",
                            Message ="Documents Uploaded Successfully !"

                        };
                        return Json(result1);
                    }
                    else
                    {
                        var result1 = new
                        {
                            Status = "Failure",
                            Message = "Please Upload Files !",
                        };
                        return Json(result1);
                    }
                 
                }
                else
                {
                    var result1 = new
                    {
                        Status = "Failure",
                        Message = "No Submittal Found !",
                    };
                    return Json(result1);
                }
             

            }
            catch(Exception ex)
            {
                var result = new
                {
                    Status = "Failure",
                    Message = ex.Message,
                };
                return Json(result);
            }
        }

        [HttpPost] // for adding document status 
        public IHttpActionResult AddDocumentStatus()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var UserName = httpRequest.Params["UserName"];
                var Password = Security.Encrypt(httpRequest.Params["Password"]);
                bool coverlttr = false;
                string CoverPagePath = string.Empty;
                string DocPath = string.Empty;
                string UserUID = string.Empty;
                DataSet ds = new DataSet();
                ds = db.CheckLogin(UserName, Password);
                if (ds.Tables[0].Rows.Count == 0)
                {

                    var resultmain = new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    };
                    return Json(resultmain);
                }
                else
                {
                        UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
                }
                Guid StatusUID = Guid.NewGuid();
                var DocumentUID = httpRequest.Params["DocumentUID"];
                var ProjectUID = httpRequest.Params["ProjectUID"];
                var Status = httpRequest.Params["Status"];
                var ActualDate = httpRequest.Params["ActualDate"];
                var CoverLetterDate = httpRequest.Params["CoverLetterDate"];
                var RefNumber = httpRequest.Params["RefNumber"];
                var Comments = httpRequest.Params["Comments"];
                //
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    HttpPostedFile httpPostedFile = httpRequest.Files[i];

                    if (httpPostedFile != null)
                    {
                        if (httpPostedFile.FileName.Contains("Cover") || httpPostedFile.FileName.Contains("Cover_Letter") || httpPostedFile.FileName.Contains("Cover Letter"))
                        {
                            coverlttr = true;
                        }
                       
                    }
                }
                if (!coverlttr)
                {
                    var result1 = new
                    {
                        Status = "Failure",
                        Message = "Please upload cover letter .File Name should contain name Cover_Letter !",
                    };
                    return Json(result1);
                }
                //
                string sDate1 = "";
                DateTime CDate1 = DateTime.Now;
                //
                sDate1 = ActualDate;
                sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                CDate1 = Convert.ToDateTime(sDate1);
                //
                DateTime lastUpdated = db.GetDocumentMax_ActualDate(new Guid(DocumentUID));
                if (lastUpdated.Date > CDate1.Date)
                {
                    var result1 = new
                    {
                        Status = "Failure",
                        Message = "Actual Date should be greater than previous date. !"

                    };
                    return Json(result1);
                    //  Page.ClientScript.RegisterStartupScript(Page.GetType(), "CLOSE", "<script language='javascript'>alert('Actual Date should be greater than previous date.');</script>");
                }
                else
                {
                    for (int i = 0; i < httpRequest.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = httpRequest.Files[i];

                        if (httpPostedFile != null)
                        {
                            if (httpPostedFile.FileName.Contains("Cover") || httpPostedFile.FileName.Contains("Cover_Letter") || httpPostedFile.FileName.Contains("Cover Letter"))
                            {
                                string FileDatetime = DateTime.Now.ToString("dd MMM yyyy hh-mm-ss tt");
                                string sDocumentPath = string.Empty;
                                //sDocumentPath = "~/" + Request.QueryString["ProjectUID"] + "/" + Request.QueryString["DocID"] + "/" + StatusUID + "/" + FileDatetime + "/CoverLetter";
                                sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + ProjectUID + "/" + StatusUID + "/CoverLetter";

                                if (!Directory.Exists(sDocumentPath))
                                {
                                    Directory.CreateDirectory(sDocumentPath);
                                }

                                string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_1_copy" + Extn);
                                string savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
                                CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;
                                db.EncryptFile(savedPath, CoverPagePath);
                                //
                                sDocumentPath = "~/" + ProjectUID + "/" + StatusUID + "/CoverLetter";
                                CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;
                                //
                            }
                            else
                            {
                                string LinkPath = ConfigurationManager.AppSettings["DocumentsPath"] + "/" + StatusUID + "/Link Document";
                                if (!Directory.Exists(LinkPath))
                                {
                                    Directory.CreateDirectory(LinkPath);
                                }
                                string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);

                                httpPostedFile.SaveAs(LinkPath + "/" + sFileName + "_1_copy" + Extn);
                                //FileUploadDoc.SaveAs(Server.MapPath("~/Documents/Encrypted/" + StatusUID + "_" + "1" + "_enp" + InputFile));
                                string savedPath = LinkPath + "/" + sFileName + "_1_copy" + Extn;
                                DocPath = LinkPath + "/" + sFileName + "_1" + Extn;
                                db.EncryptFile(savedPath, DocPath);
                                //
                                LinkPath = "~/" + ProjectUID + "/" + StatusUID + "/Link Document";

                            }
                        }
                    }

                    string DocumentDate = CoverLetterDate;
                    DocumentDate = DocumentDate.Split('/')[1] + "/" + DocumentDate.Split('/')[0] + "/" + DocumentDate.Split('/')[2];
                    DateTime Document_Date = Convert.ToDateTime(DocumentDate);

                    int Cnt = db.InsertorUpdateDocumentStatus(StatusUID, new Guid(DocumentUID), 1, Status, 0, CDate1, DocPath,
                           new Guid(UserUID), Comments, Status, RefNumber, Document_Date, CoverPagePath);
                    if (Cnt > 0)
                    {
                        var result2 = new
                        {
                            Status = "Success",
                            Message = "Documents Status Updated SuccesFully !"

                        };
                        return Json(result2);
                    }
                    else
                    {
                        var result2 = new
                        {
                            Status = "Success",
                            Message = "Documents Status Updated SuccesFully !"

                        };
                        return Json(result2);
                    }
                }
            }
            catch(Exception ex)
            {
                var result1 = new
                {
                    Status = "Failure",
                    Message = ex.Message

                };
                return Json(result1);
            }
        }

        [HttpPost] // for getting Submittal CAtegory
        public IHttpActionResult GetSubmittalCategory()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var UserName = httpRequest.Params["UserName"];
                var Password = Security.Encrypt(httpRequest.Params["Password"]);
                DataSet ds = new DataSet();
                ds = db.CheckLogin(UserName, Password);
                if (ds.Tables[0].Rows.Count == 0)
                {

                    var resultmain = new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    };
                    return Json(resultmain);
                }
              
                ds = db.GetActualDocument_Type_Master();
                return Json(new
                {
                    Success = true,
                    SubmittalCategory = ds.Tables[0]
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error" + ex.Message
                });
            }
        }


        [HttpPost] // for getting Submittal Document flows
        public IHttpActionResult GetSubmittalDocumentFlows()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var UserName = httpRequest.Params["UserName"];
                var Password = Security.Encrypt(httpRequest.Params["Password"]);
                DataSet ds = new DataSet();
                ds = db.GetDocumentFlows();
                return Json(new
                {
                    Success = true,
                    SubmittalDocumentFlows = ds.Tables[0]
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error" + ex.Message
                });
            }
        }

        [HttpPost] // for getting Users for Add Submittal
        public IHttpActionResult GetUsersforAddSubmittal()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var UserName = httpRequest.Params["UserName"];
                var Password = Security.Encrypt(httpRequest.Params["Password"]);
                var ProjectUID = httpRequest.Params["ProjectUID"];
                string Usertype = string.Empty;
                DataSet ds = new DataSet();
                ds = db.CheckLogin(UserName, Password);
                if (ds.Tables[0].Rows.Count == 0)
                {

                    var resultmain = new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    };
                    return Json(resultmain);
                }
                else
                {
                    Usertype = ds.Tables[0].Rows[0]["TypeOfUser"].ToString();
                }
                ds.Clear();
                if (Usertype == "U" || Usertype == "MD" || Usertype == "VP")
                {
                    ds = db.getAllUsers();
                }
                else if (Usertype == "PA")
                {
                    //ds = getdata.getUsers_by_ProjectUnder(new Guid(DDlProject.SelectedValue));
                    ds = db.GetUsers_under_ProjectUID(new Guid(ProjectUID));
                }
                else
                {
                    ds = db.GetUsers_under_ProjectUID(new Guid(ProjectUID));
                }
                return Json(new
                {
                    Success = true,
                    Users = ds.Tables[0]
                });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Error" + ex.Message
                });
            }
        }

        // added on 19/10/2020 zuber
        private bool checkDocumentExists(string filename, string sDocumentUID)
        {
            bool result = false;
            DataSet dsfiles = new DataSet();
            try
            {
                dsfiles = db.ActualDocuments_SelectBy_DocumentUID(new Guid(sDocumentUID));
                foreach (DataRow dr in dsfiles.Tables[0].Rows)
                {
                    if (dr["ActualDocument_Name"].ToString() == filename)
                    {
                        result = true;
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        private void EncryptFile(string inputFile, string outputFile)
        {

            try
            {
                string password = @"myKey123"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch (Exception ex)
            {
                string exmsg = ex.Message;
                //MessageBox.Show("Encryption failed!", "Error");
            }
        }

       
    }
}
