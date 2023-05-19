using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using PMTWebAPI.DAL;
using PMTWebAPI.Models;

namespace PMTWebAPI.Controllers
{
    public class ExtoIntegrationController : ApiController
    {
        DBGetData db = new DBGetData();
        DBSyncData dbsync = new DBSyncData();
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
            else
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/ExtoIntegration/authenticate")]
        public IHttpActionResult GetForAuthenticate()
        {
            var identity = (ClaimsIdentity)User.Identity;
            if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
            {
                return Ok("Hello " + identity.Name);
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Message = "Not Authorized IP address"
                });
            }

        }


        [Authorize]
        [HttpPost] // for getting Submittal Document flows
        [Route("api/ExtoIntegration/GetIpAddress")]
        public IHttpActionResult GetClientIpAddress()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var httpRequest = HttpContext.Current.Request;
                var UserName = httpRequest.Params["UserName"];
                var Password = Security.Encrypt(httpRequest.Params["Password"]);
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    DataSet ds = new DataSet();
                    ds = db.GetDocumentFlows();
                    return Json(new
                    {
                        Success = true,
                        Ipaddress = GetIp()

                    });
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Not Authorized IP address"
                    });
                }
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

        [Authorize]
        [HttpPost] // this is for adding submittals
        [Route("api/ExtoIntegration/AddSubmittals")]
        public IHttpActionResult AddSubmittals()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                string TaskUID = string.Empty;
                string SubCategory = string.Empty;
                var FlowUID = string.Empty;
                string FlowName = string.Empty;
                var SubmitterUID = string.Empty;
                var ReviewerUID = string.Empty;
                var ApproverUID = string.Empty;
                var tmpData = string.Empty;

                //var transactionUid = Guid.NewGuid();
                //var BaseURL = HttpContext.Current.Request.Url.ToString();
                //for (int p=0;p<httpRequest.Params.Count;p++)
                //{
                //    tmpData += httpRequest.Params[p];
                //}
                //db.WebAPITransctionInsert(transactionUid, BaseURL, tmpData, "");
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) == 0)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Not Authorized IP address"
                    });
                }
                else
                {
                    if (httpRequest.Params["SubmittalName"] == null)
                    {
                        var resultmain = new
                        {
                            Status = "Failure",
                            Message = "Please Enter Submittal Name !",
                        };
                        return Json(resultmain);
                    }

                    if (httpRequest.Params["SubmittalName"] != null)
                    {
                        if (string.IsNullOrWhiteSpace((httpRequest.Params["SubmittalName"].ToString())))
                        {
                            var resultmain = new
                            {
                                Status = "Failure",
                                Message = "Submittal Name cannot be Empty !",
                            };
                            return Json(resultmain);
                        }
                    }
                    FlowName = httpRequest.Params["FlowName"];
                    if (httpRequest.Params["FlowUID"] == null)
                    {
                        FlowUID = db.CheckSubmittalFlowExists(Guid.NewGuid(), httpRequest.Params["FlowName"], "Name");
                        if (String.IsNullOrEmpty(FlowUID))
                        {
                            var resultmain = new
                            {
                                Status = "Failure",
                                Message = "Flow Name does not Exists !",
                            };
                            return Json(resultmain);
                        }
                    }
                    else
                    {
                        //check the flowUID in db

                        if (String.IsNullOrEmpty(db.CheckSubmittalFlowExists(new Guid(httpRequest.Params["FlowUID"]), "", "UID")))
                        {
                            var resultmain = new
                            {
                                Status = "Failure",
                                Message = "FlowUID does not Exists !",
                            };
                            return Json(resultmain);
                        }
                        else
                        {
                            FlowUID = httpRequest.Params["FlowUID"];
                        }
                    }
                    if (httpRequest.Params["Submitter"] == null)
                    {
                        var resultmain = new
                        {
                            Status = "Failure",
                            Message = "Please Enter Submitter !",
                        };
                        return Json(resultmain);
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(db.CheckUserExists(Guid.NewGuid(), httpRequest.Params["Submitter"], "Name")))
                        {
                            var resultmain = new
                            {
                                Status = "Failure",
                                Message = "Submitter does not Exists !",
                            };
                            return Json(resultmain);
                        }
                        else
                        {
                            SubmitterUID = db.CheckUserExists(Guid.NewGuid(), httpRequest.Params["Submitter"], "Name");
                        }
                    }
                    if (httpRequest.Params["SubmitterTargetDate"] == null)
                    {
                        var resultmain = new
                        {
                            Status = "Failure",
                            Message = "Please Enter SubmitterTargetDate !",
                        };
                        return Json(resultmain);
                    }


                    if (httpRequest.Params["taskUID"] == null) //if null then only go for tasls,tasks level,prjname,wrkpkgname checking
                    {
                        var TaskParam = httpRequest.Params["Tasks"];
                        var tLevel = Convert.ToInt32(httpRequest.Params["TLevel"]);
                        var ProjectName = httpRequest.Params["ProjectName"];
                        var WorkpackageName = httpRequest.Params["WorkpackageName"];

                        var pExists = db.ProjectNameExists(ProjectName);
                        if (pExists != "")
                        {
                            string[] TaskList = TaskParam.Split('$');
                            //var objects = JArray.Parse(TaskParam); // parse as array 
                            //var result = JsonConvert.DeserializeObject<RootObject>(TaskParam);

                            var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                            if (wExists != "")
                            {

                                if (tLevel > 0)
                                {

                                    string ParentTaskUID = "";
                                    for (int i = 0; i < tLevel; i++)
                                    {
                                        if (i == 0)
                                        {
                                            TaskUID = db.TaskNameExists(new Guid(wExists), TaskList[i], "Workpackage");
                                            ParentTaskUID = TaskUID;
                                        }
                                        else
                                        {
                                            TaskUID = db.TaskNameExists(new Guid(ParentTaskUID), TaskList[i], "Task");
                                            ParentTaskUID = TaskUID;
                                        }

                                        if (TaskUID == null || TaskUID == "")
                                        {
                                            var resultmain = new
                                            {
                                                Status = "Failure",
                                                Message = "Invalid Task Name !",
                                            };
                                            return Json(resultmain);

                                        }
                                    }
                                    if (TaskUID != "")
                                    {

                                    }
                                    else
                                    {
                                        var resultmain = new
                                        {
                                            Status = "Failure",
                                            Message = "Invalid Task Name !",
                                        };
                                        return Json(resultmain);

                                    }
                                }
                                else
                                {
                                    var resultmain = new
                                    {
                                        Status = "Failure",
                                        Message = "Invalid Task Level !",
                                    };
                                    return Json(resultmain);


                                }
                            }
                            else
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Workpackage does not Exists !",
                                };
                                return Json(resultmain);
                            }
                        }
                        else
                        {
                            var resultmain = new
                            {
                                Status = "Failure",
                                Message = "Project Name does not Exists !",
                            };
                            return Json(resultmain);

                        }
                    }
                    else
                    {
                        TaskUID = httpRequest.Params["taskUID"];
                    }



                    var taskUID = TaskUID;
                    var SubmittalName = httpRequest.Params["SubmittalName"];
                    var SubmittalCategory = SubCategory;

                    var sDocumentUID = Guid.NewGuid();
                    bool isUpdateSubmital = false;

                    var SubmitterTargetDate = httpRequest.Params["SubmitterTargetDate"];

                    var RevieweTargetDate = httpRequest.Params["ReviewerTargetDate"];

                    var ApproverTargetDate = httpRequest.Params["ApproverTargetDate"];
                    var EstimatedDocuments = httpRequest.Params["EstimatedDocuments"];
                    var Remarks = httpRequest.Params["Remarks"];
                    var DocumentSearchType = httpRequest.Params["SubmittalDocType"];
                    Guid projectId = Guid.Empty;
                    Guid workpackageid = Guid.Empty;
                    if (string.IsNullOrEmpty(DocumentSearchType))
                        DocumentSearchType = string.Empty;



                    string sDate1 = "", sDate2 = "", sDate3 = "", sDate4 = "", sDate5 = "", DocStartString = "";
                    DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now, CDate4 = DateTime.Now, CDate5 = DateTime.Now, DocStartDate = DateTime.Now;

                    DataTable dttasks = db.GetTaskDetails_TaskUID(taskUID);
                    if (dttasks.Rows.Count > 0)
                    {
                        projectId = new Guid(dttasks.Rows[0]["ProjectUID"].ToString());
                        workpackageid = new Guid(dttasks.Rows[0]["workPackageUID"].ToString());

                        if (httpRequest.Params["SubmittalCategory"] == null)
                        {
                            var resultmain = new
                            {
                                Status = "Failure",
                                Message = "Please Enter Submittal Category !",
                            };
                            return Json(resultmain);
                        }
                        else
                        {
                            //check 
                            SubCategory = db.CheckSubmittalCategoryExists(Guid.NewGuid(), httpRequest.Params["SubmittalCategory"], "Name", workpackageid);
                            SubmittalCategory = SubCategory;
                            if (String.IsNullOrEmpty(SubCategory))
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "SubmittalCategory does not Exists !",
                                };
                                return Json(resultmain);
                            }
                        }
                    }
                    else
                    {
                        var resultmain = new
                        {
                            Status = "Failure",
                            Message = "TaskUID does not Exists !",
                        };
                        return Json(resultmain);
                    }

                    Submitals submitals = new Submitals();
                    DateTime inputDate = DateTime.Now;
                    DateTime.TryParse(SubmitterTargetDate, out inputDate);


                   
                    var isExist = db.IsSubmitalExist(workpackageid.ToString(), taskUID, SubmittalName);
                    if (isExist)
                    {
                        var resultmain = new
                        {
                            Status = "Failure",
                            Message = "Submital name Exists !",
                        };
                        return Json(resultmain);
                    }

                    string sDate6 = submitals.FlowStep6_Date, sDate7 = submitals.FlowStep7_Date, sDate8 = submitals.FlowStep8_Date, sDate9 = submitals.FlowStep9_Date, sDate10 = submitals.FlowStep10_Date, sDate11 = submitals.FlowStep11_Date, sDate12 = submitals.FlowStep12_Date, sDate13 = submitals.FlowStep13_Date, sDate14 = submitals.FlowStep14_Date, sDate15 = submitals.FlowStep15_Date, sDate16 = submitals.FlowStep16_Date, sDate17 = submitals.FlowStep17_Date, sDate18 = submitals.FlowStep18_Date, sDate19 = submitals.FlowStep19_Date, sDate20 = submitals.FlowStep20_Date, DocPath = "", CoverPagePath = "";
                    DateTime CDate6 = DateTime.Now, CDate7 = DateTime.Now, CDate8 = DateTime.Now, CDate9 = DateTime.Now, CDate10 = DateTime.Now, CDate11 = DateTime.Now, CDate12 = DateTime.Now, CDate13 = DateTime.Now, CDate14 = DateTime.Now, CDate15 = DateTime.Now, CDate16 = DateTime.Now, CDate17 = DateTime.Now, CDate18 = DateTime.Now, CDate19 = DateTime.Now, CDate20 = DateTime.Now;

                    string FlowStep1_IsMUser = "N", FlowStep2_IsMUser = "N", FlowStep3_IsMUser = "N", FlowStep4_IsMUser = "N", FlowStep5_IsMUser = "N", FlowStep6_IsMUser = "N", FlowStep7_IsMUser = "N", FlowStep8_IsMUser = "N", FlowStep9_IsMUser = "N", FlowStep10_IsMUser = "N", FlowStep11_IsMUser = "N", FlowStep12_IsMUser = "N", FlowStep13_IsMUser = "N", FlowStep14_IsMUser = "N", FlowStep15_IsMUser = "N", FlowStep16_IsMUser = "N", FlowStep17_IsMUser = "N", FlowStep18_IsMUser = "N", FlowStep19_IsMUser = "N", FlowStep20_IsMUser = "N";

                    string IsSync = "N";



                    DataSet dsFlow = db.GetDocumentFlows_by_UID(new Guid(FlowUID));
                    int result = 0;
                    if (dsFlow.Tables[0].Rows.Count > 0)
                    {
                        DocStartString = DateTime.Now.ToString("dd/MM/yyyy");
                        DocStartString = DocStartString.Split('/')[1] + "/" + DocStartString.Split('/')[0] + "/" + DocStartString.Split('/')[2];
                        DocStartDate = Convert.ToDateTime(DocStartString);


                        //
                        //sDate1 = dtSubTargetDate.Text;
                        //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                        //CDate1 = Convert.ToDateTime(sDate1);
                        ////

                        //sDate2 = dtQualTargetDate.Text;
                        //sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                        //CDate2 = Convert.ToDateTime(sDate2);

                        //sDate3 = dtRev_B_TargetDate.Text;
                        //sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                        //CDate3 = Convert.ToDateTime(sDate3);

                        //result = getdata.DoumentMaster_Insert_or_Update_Flow_2(sDocumentUID, workpackageid, projectId, TaskUID, txtDocumentName.Text, new Guid(DDLDocumentCategory.SelectedValue),
                        //"", "Submittle Document", 0.0, new Guid(DDLDocumentFlow.SelectedValue), DocStartDate, new Guid(ddlSubmissionUSer.SelectedValue), CDate1,
                        //new Guid(ddlQualityEngg.SelectedValue), CDate2, new Guid(ddlReviewer_B.SelectedValue), CDate3);

                        if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "1")
                        {
                            //


                            sDate1 = SubmitterTargetDate;
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);
                            result = db.DoumentMaster_Insert_or_Update_Flow_0(sDocumentUID, workpackageid, projectId, new Guid(taskUID), SubmittalName, new Guid(SubmittalCategory),
                            "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1, Convert.ToInt16(EstimatedDocuments), Remarks, DocumentSearchType);

                        }
                        else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "2")
                        {
                            //

                            if (httpRequest.Params["Approver"] == null)
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Please Enter Approver !",
                                };
                                return Json(resultmain);
                            }
                            else
                            {
                                //if (String.IsNullOrEmpty(db.CheckUserExists(Guid.NewGuid(), httpRequest.Params["Approver"], "Name")))
                                //{
                                //    var resultmain = new
                                //    {
                                //        Status = "Failure",
                                //        Message = "Approver does not Exists !",
                                //    };
                                //    return Json(resultmain);
                                //}
                                //else
                                //{
                                DataTable dtApiSubmittalUsers = db.getapi_submittal_users(httpRequest.Params["ProjectName"]);
                                if (dtApiSubmittalUsers.Rows.Count == 0)
                                {
                                    var resultmain = new
                                    {
                                        Status = "Failure",
                                        Message = "Apporver is not exists",
                                    };
                                    return Json(resultmain);
                                }
                                string approverName = dtApiSubmittalUsers.Rows[0]["approver"].ToString();
                                ApproverUID = db.CheckUserExists(Guid.NewGuid(), approverName, "Name");
                                //}
                            }
                            if (httpRequest.Params["ApproverTargetDate"] == null)
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Please Enter Approver TargetDate !",
                                };
                                return Json(resultmain);
                            }
                            sDate1 = SubmitterTargetDate;
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);
                            //
                            sDate2 = ApproverTargetDate;
                            sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                            CDate2 = Convert.ToDateTime(sDate2);

                            CDate2 = Convert.ToDateTime(sDate2);
                            result = db.DoumentMaster_Insert_or_Update_Flow_1(sDocumentUID, workpackageid, projectId, new Guid(taskUID), SubmittalName, new Guid(SubmittalCategory),
                            "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1,
                            new Guid(ApproverUID), CDate2, Convert.ToInt16(EstimatedDocuments), Remarks, DocumentSearchType);
                        }
                        else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "3")
                        {
                            //
                            if (httpRequest.Params["Reviewer"] == null)
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Please Enter Reviewer !",
                                };
                                return Json(resultmain);
                            }
                            else
                            {
                                //if (String.IsNullOrEmpty(db.CheckUserExists(Guid.NewGuid(), httpRequest.Params["Reviewer"], "Name")))
                                //{
                                //    var resultmain = new
                                //    {
                                //        Status = "Failure",
                                //        Message = "Reviewer does not Exists !",
                                //    };
                                //    return Json(resultmain);
                                //}
                                //else
                                //{
                                DataTable dtApiSubmittalUsers = db.getapi_submittal_users(httpRequest.Params["ProjectName"]);
                                if (dtApiSubmittalUsers.Rows.Count == 0)
                                {
                                    var resultmain = new
                                    {
                                        Status = "Failure",
                                        Message = "Reviewer is not exists",
                                    };
                                    return Json(resultmain);
                                }
                                string reviewerName = dtApiSubmittalUsers.Rows[0]["reviewer"].ToString();
                                ReviewerUID = db.CheckUserExists(Guid.NewGuid(), reviewerName, "Name");
                                //}
                            }
                            if (httpRequest.Params["ReviewerTargetDate"] == null)
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Please Enter Reviewer TargetDate !",
                                };
                                return Json(resultmain);
                            }
                            if (httpRequest.Params["Approver"] == null)
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Please Enter Approver !",
                                };
                                return Json(resultmain);
                            }
                            else
                            {
                                //if (String.IsNullOrEmpty(db.CheckUserExists(Guid.NewGuid(), httpRequest.Params["Approver"], "Name")))
                                //{
                                //    var resultmain = new
                                //    {
                                //        Status = "Failure",
                                //        Message = "Approver does not Exists !",
                                //    };
                                //    return Json(resultmain);
                                //}
                                //else
                                //{

                                //    ApproverUID = db.CheckUserExists(Guid.NewGuid(), httpRequest.Params["Approver"], "Name");
                                DataTable dtApiSubmittalUsers = db.getapi_submittal_users(httpRequest.Params["ProjectName"]);
                                if (dtApiSubmittalUsers.Rows.Count == 0)
                                {
                                    var resultmain = new
                                    {
                                        Status = "Failure",
                                        Message = "Apporver is not exists",
                                    };
                                    return Json(resultmain);
                                }
                                string approverName = dtApiSubmittalUsers.Rows[0]["approver"].ToString();
                                ApproverUID = db.CheckUserExists(Guid.NewGuid(), approverName, "Name");

                                //}
                            }
                            if (httpRequest.Params["ApproverTargetDate"] == null)
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Please Enter Approver TargetDate !",
                                };
                                return Json(resultmain);
                            }
                            //
                            sDate1 = SubmitterTargetDate;
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);
                            //

                            sDate2 = RevieweTargetDate;
                            sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                            CDate2 = Convert.ToDateTime(sDate2);

                            sDate3 = ApproverTargetDate;
                            sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                            CDate3 = Convert.ToDateTime(sDate3);

                            result = db.DoumentMaster_Insert_or_Update_Flow_2(sDocumentUID, workpackageid, projectId, new Guid(taskUID), SubmittalName, new Guid(SubmittalCategory),
                            "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1,
                            new Guid(ReviewerUID), CDate2, new Guid(ApproverUID), CDate3, Convert.ToInt16(EstimatedDocuments), Remarks, DocumentSearchType);
                        }
                        else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "4")
                        {
                            //
                            sDate1 = submitals.FlowStep1_Date != "" ? submitals.FlowStep1_Date : CDate1.ToString("dd/MM/yyyy");
                            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            sDate1 = db.ConvertDateFormat(sDate1);
                            CDate1 = Convert.ToDateTime(sDate1);

                            sDate2 = submitals.FlowStep2_Date != "" ? submitals.FlowStep2_Date : CDate2.ToString("dd/MM/yyyy");
                            //sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                            sDate2 = db.ConvertDateFormat(sDate2);
                            CDate2 = Convert.ToDateTime(sDate2);

                            //
                            sDate3 = submitals.FlowStep3_Date != "" ? submitals.FlowStep3_Date : CDate3.ToString("dd/MM/yyyy");
                            //sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                            sDate3 = db.ConvertDateFormat(sDate3);
                            CDate3 = Convert.ToDateTime(sDate3);

                            //
                            sDate4 = submitals.FlowStep4_Date != "" ? submitals.FlowStep4_Date : CDate4.ToString("dd/MM/yyyy");
                            //sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
                            sDate4 = db.ConvertDateFormat(sDate4);
                            CDate4 = Convert.ToDateTime(sDate4);

                            result = db.DoumentMaster_Insert_or_Update_Flow_3(sDocumentUID, workpackageid, projectId, new Guid(TaskUID), SubmittalName, new Guid(SubmittalCategory),
                            "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1,
                            submitals.lstQualityEngg[0].UserUID, CDate2, submitals.lstReviewer_B[0].UserUID, CDate3, submitals.lstReviewer[0].UserUID, CDate4,
                            Convert.ToInt16(EstimatedDocuments), Remarks, DocumentSearchType, IsSync);
                        }
                        else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "5")
                        {

                            sDate1 = submitals.FlowStep1_Date != "" ? submitals.FlowStep1_Date : CDate1.ToString("dd/MM/yyyy");
                            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            sDate1 = db.ConvertDateFormat(sDate1);
                            CDate1 = Convert.ToDateTime(sDate1);
                            //

                            sDate2 = submitals.FlowStep2_Date != "" ? submitals.FlowStep2_Date : CDate2.ToString("dd/MM/yyyy");
                            //sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                            sDate2 = db.ConvertDateFormat(sDate2);
                            CDate2 = Convert.ToDateTime(sDate2);




                            sDate3 = submitals.FlowStep3_Date != "" ? submitals.FlowStep3_Date : CDate3.ToString("dd/MM/yyyy");
                            //sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                            sDate3 = db.ConvertDateFormat(sDate3);
                            CDate3 = Convert.ToDateTime(sDate3);


                            sDate4 = submitals.FlowStep4_Date != "" ? submitals.FlowStep4_Date : CDate4.ToString("dd/MM/yyyy");
                            //sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
                            sDate4 = db.ConvertDateFormat(sDate4);
                            CDate4 = Convert.ToDateTime(sDate4);
                            //
                            sDate5 = submitals.FlowStep5_Date != "" ? submitals.FlowStep5_Date : CDate5.ToString("dd/MM/yyyy");
                            //sDate5 = sDate5.Split('/')[1] + "/" + sDate5.Split('/')[0] + "/" + sDate5.Split('/')[2];
                            sDate5 = db.ConvertDateFormat(sDate5);
                            CDate5 = Convert.ToDateTime(sDate5);

                            result = db.DoumentMaster_Insert_or_Update_Flow_4(sDocumentUID, workpackageid, projectId, new Guid(TaskUID), SubmittalName, new Guid(SubmittalCategory),
                            "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1,
                            submitals.lstQualityEngg[0].UserUID, CDate2, submitals.lstReviewer_B[0].UserUID, CDate3, submitals.lstReviewer[0].UserUID, CDate4,
                            submitals.lstApproval[0].UserUID, CDate5, Convert.ToInt16(EstimatedDocuments), Remarks, DocumentSearchType, IsSync);
                        }
                        else // for all flows with step > 5 ( 6 to 20) added on 04/03/2022
                        {
                            // changed on 26/05/2022 after error reported by nikhil
                            submitals.SetDatesForFlow(inputDate, FlowUID);

                            //added on 26/07/2022
                            //-----------------------------------------
                            DataTable updatedUsers = db.GetWorksTaskUpdatedStepUsers(projectId, new Guid(FlowUID));
                            if (updatedUsers != null && updatedUsers.Rows.Count > 0)
                            {
                              

                                var allUpdatedUsersWithNoTask = updatedUsers.AsEnumerable().Where(r => string.IsNullOrEmpty(r.Field<string>("TaskUID")));
                                if (allUpdatedUsersWithNoTask.FirstOrDefault() != null)
                                {
                                    updatedUsers = allUpdatedUsersWithNoTask.CopyToDataTable();
                                }
                                else
                                {
                                    var filterredData = updatedUsers.AsEnumerable().Where(r => r.Field<string>("TaskUID") == taskUID.ToUpper());
                                    if (filterredData.FirstOrDefault() != null)
                                    {
                                        updatedUsers = filterredData.CopyToDataTable();
                                    }
                                    else
                                    {
                                        while (true)
                                        {
                                            DataTable dt = db.GetTaskDetails_TaskUID(taskUID);
                                            if (dt != null && dt.Rows.Count > 0)
                                            {
                                                taskUID = dt.Rows[0]["ParentTaskID"].ToString();
                                                filterredData = updatedUsers.AsEnumerable().Where(r => r.Field<string>("TaskUID") == taskUID.ToUpper());
                                                if (filterredData.FirstOrDefault() != null)
                                                {
                                                    updatedUsers = filterredData.CopyToDataTable();
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                updatedUsers = new DataTable();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                           //----------------------------------------------------

                            submitals.BindFlowMasterUsers(workpackageid, new Guid(FlowUID), new Guid(SubmittalCategory), FlowName, updatedUsers);
                            if (submitals.IsValidationFailed)
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = submitals.ErrorMessage,
                                };
                                return Json(resultmain);
                            }
                            ////

                            sDate1 = submitals.FlowStep1_Date != "" ? submitals.FlowStep1_Date : CDate1.ToString("dd/MM/yyyy");

                            sDate1 = db.ConvertDateFormat(sDate1);
                            CDate1 = Convert.ToDateTime(sDate1);
                            //

                            sDate2 = submitals.FlowStep2_Date != "" ? submitals.FlowStep2_Date : CDate2.ToString("dd/MM/yyyy");
                            sDate2 = db.ConvertDateFormat(sDate2);
                            CDate2 = Convert.ToDateTime(sDate2);
                            if (isUpdateSubmital)
                                db.DeleteSubmittal_MultipleUsers(sDocumentUID, 2);
                            int selectedCount = submitals.lstQualityEngg.Count;
                            if (selectedCount > 1)
                            {

                                FlowStep2_IsMUser = "Y";
                                foreach (var listItem in submitals.lstQualityEngg)
                                {
                                    db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 2, listItem.UserUID);
                                }
                            }

                            sDate3 = submitals.FlowStep3_Date != "" ? submitals.FlowStep3_Date : CDate3.ToString("dd/MM/yyyy");
                            sDate3 = db.ConvertDateFormat(sDate3);
                            CDate3 = Convert.ToDateTime(sDate3);
                            if (isUpdateSubmital)
                                db.DeleteSubmittal_MultipleUsers(sDocumentUID, 3);
                            selectedCount = submitals.lstReviewer_B.Count;
                            if (selectedCount > 1)
                            {

                                FlowStep3_IsMUser = "Y";
                                foreach (var listItem in submitals.lstReviewer_B)
                                {
                                    db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 3, listItem.UserUID);
                                }
                            }


                            sDate4 = submitals.FlowStep4_Date != "" ? submitals.FlowStep4_Date : CDate4.ToString("dd/MM/yyyy");
                            sDate4 = db.ConvertDateFormat(sDate4);
                            CDate4 = Convert.ToDateTime(sDate4);
                            if (isUpdateSubmital)
                                db.DeleteSubmittal_MultipleUsers(sDocumentUID, 4);
                            selectedCount = submitals.lstReviewer.Count;
                            if (selectedCount > 1)
                            {

                                FlowStep4_IsMUser = "Y";
                                foreach (var listItem in submitals.lstReviewer)
                                {
                                    db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 4, listItem.UserUID);
                                }
                            }
                            //
                            sDate5 = submitals.FlowStep5_Date != "" ? submitals.FlowStep5_Date : CDate5.ToString("dd/MM/yyyy");
                            sDate5 = db.ConvertDateFormat(sDate5);
                            CDate5 = Convert.ToDateTime(sDate5);
                            if (isUpdateSubmital)
                                db.DeleteSubmittal_MultipleUsers(sDocumentUID, 5);
                            selectedCount = submitals.lstApproval.Count;
                            if (selectedCount > 1)
                            {

                                FlowStep5_IsMUser = "Y";
                                foreach (var listItem in submitals.lstApproval)
                                {
                                    db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 5, listItem.UserUID);
                                }
                            }
                            //--------------------------------------------- for new steps
                            sDate6 = submitals.FlowStep6_Date != "" ? submitals.FlowStep6_Date : CDate6.ToString("dd/MM/yyyy");
                            sDate6 = db.ConvertDateFormat(sDate6);
                            CDate6 = Convert.ToDateTime(sDate6);
                            if (isUpdateSubmital)
                                db.DeleteSubmittal_MultipleUsers(sDocumentUID, 6);
                            selectedCount = submitals.lstUser6.Count;
                            if (selectedCount > 1)
                            {

                                FlowStep6_IsMUser = "Y";
                                foreach (var listItem in submitals.lstUser6)
                                {
                                    db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 6, listItem.UserUID);
                                }
                            }

                            int steps = int.Parse(dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString());
                            Guid User7 = Guid.NewGuid(), User8 = Guid.NewGuid(), User9 = Guid.NewGuid(), User10 = Guid.NewGuid(), User11 = Guid.NewGuid(), User12 = Guid.NewGuid(), User13 = Guid.NewGuid(), User14 = Guid.NewGuid(), User15 = Guid.NewGuid(), User16 = Guid.NewGuid(), User17 = Guid.NewGuid(), User18 = Guid.NewGuid(), User19 = Guid.NewGuid(), User20 = Guid.NewGuid();
                            if (steps >= 7)
                            {
                                sDate7 = submitals.FlowStep7_Date != "" ? submitals.FlowStep7_Date : CDate7.ToString("dd/MM/yyyy");
                                sDate7 = db.ConvertDateFormat(sDate7);
                                CDate7 = Convert.ToDateTime(sDate7);
                                User7 = submitals.lstUser7[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 7);
                                selectedCount = submitals.lstUser7.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep7_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser7)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 7, listItem.UserUID);
                                    }
                                }

                            }
                            if (steps >= 8)
                            {
                                sDate8 = submitals.FlowStep8_Date != "" ? submitals.FlowStep8_Date : CDate8.ToString("dd/MM/yyyy");
                                sDate8 = db.ConvertDateFormat(sDate8);
                                CDate8 = Convert.ToDateTime(sDate8);
                                User8 = submitals.lstUser8[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 8);
                                selectedCount = submitals.lstUser8.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep8_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser8)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 8, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 9)
                            {
                                sDate9 = submitals.FlowStep9_Date != "" ? submitals.FlowStep9_Date : CDate9.ToString("dd/MM/yyyy");
                                sDate9 = db.ConvertDateFormat(sDate9);
                                CDate9 = Convert.ToDateTime(sDate9);
                                User9 = submitals.lstUser9[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 9);
                                selectedCount = submitals.lstUser9.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep9_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser9)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 9, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 10)
                            {
                                sDate10 = submitals.FlowStep10_Date != "" ? submitals.FlowStep10_Date : CDate10.ToString("dd/MM/yyyy");
                                sDate10 = db.ConvertDateFormat(sDate10);
                                CDate10 = Convert.ToDateTime(sDate10);
                                User10 = submitals.lstUser10[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 10);
                                selectedCount = submitals.lstUser10.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep10_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser10)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 10, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 11)
                            {
                                sDate11 = submitals.FlowStep11_Date != "" ? submitals.FlowStep11_Date : CDate11.ToString("dd/MM/yyyy");
                                sDate11 = db.ConvertDateFormat(sDate11);
                                CDate11 = Convert.ToDateTime(sDate11);
                                User11 = submitals.lstUser11[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 11);
                                selectedCount = submitals.lstUser11.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep11_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser11)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 11, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 12)
                            {
                                sDate12 = submitals.FlowStep12_Date != "" ? submitals.FlowStep12_Date : CDate12.ToString("dd/MM/yyyy");
                                sDate12 = db.ConvertDateFormat(sDate12);
                                CDate12 = Convert.ToDateTime(sDate12);
                                User12 = submitals.lstUser12[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 12);
                                selectedCount = submitals.lstUser12.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep12_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser12)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 12, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 13)
                            {
                                sDate13 = submitals.FlowStep13_Date != "" ? submitals.FlowStep13_Date : CDate13.ToString("dd/MM/yyyy");
                                sDate13 = db.ConvertDateFormat(sDate13);
                                CDate13 = Convert.ToDateTime(sDate13);
                                User13 = submitals.lstUser13[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 13);
                                selectedCount = submitals.lstUser13.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep13_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser13)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 13, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 14)
                            {
                                sDate14 = submitals.FlowStep14_Date != "" ? submitals.FlowStep14_Date : CDate14.ToString("dd/MM/yyyy");
                                sDate14 = db.ConvertDateFormat(sDate14);
                                CDate14 = Convert.ToDateTime(sDate14);
                                User14 = submitals.lstUser14[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 14);
                                selectedCount = submitals.lstUser14.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep14_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser14)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 14, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 15)
                            {
                                sDate15 = submitals.FlowStep15_Date != "" ? submitals.FlowStep15_Date : CDate15.ToString("dd/MM/yyyy");
                                sDate15 = db.ConvertDateFormat(sDate15);
                                CDate15 = Convert.ToDateTime(sDate15);
                                User15 = submitals.lstUser15[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 15);
                                selectedCount = submitals.lstUser15.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep15_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser15)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 15, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 16)
                            {
                                sDate16 = submitals.FlowStep16_Date != "" ? submitals.FlowStep16_Date : CDate16.ToString("dd/MM/yyyy");
                                sDate16 = db.ConvertDateFormat(sDate16);
                                CDate16 = Convert.ToDateTime(sDate16);
                                User16 = submitals.lstUser16[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 16);
                                selectedCount = submitals.lstUser16.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep16_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser16)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 16, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 17)
                            {
                                sDate17 = submitals.FlowStep17_Date != "" ? submitals.FlowStep17_Date : CDate17.ToString("dd/MM/yyyy");
                                sDate17 = db.ConvertDateFormat(sDate17);
                                CDate17 = Convert.ToDateTime(sDate17);
                                User17 = submitals.lstUser17[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 17);
                                selectedCount = submitals.lstUser17.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep17_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser17)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 17, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 18)
                            {
                                sDate18 = submitals.FlowStep18_Date != "" ? submitals.FlowStep18_Date : CDate18.ToString("dd/MM/yyyy");
                                sDate18 = db.ConvertDateFormat(sDate18);
                                CDate18 = Convert.ToDateTime(sDate18);
                                User18 = submitals.lstUser18[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 18);
                                selectedCount = submitals.lstUser18.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep18_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser18)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 18, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 19)
                            {
                                sDate19 = submitals.FlowStep19_Date != "" ? submitals.FlowStep19_Date : CDate19.ToString("dd/MM/yyyy");
                                sDate19 = db.ConvertDateFormat(sDate19);
                                CDate19 = Convert.ToDateTime(sDate19);
                                User19 = submitals.lstUser19[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 19);
                                selectedCount = submitals.lstUser19.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep19_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser19)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 19, listItem.UserUID);
                                    }
                                }
                            }
                            if (steps >= 20)
                            {
                                sDate20 = submitals.FlowStep20_Date != "" ? submitals.FlowStep20_Date : CDate20.ToString("dd/MM/yyyy");
                                sDate20 = db.ConvertDateFormat(sDate20);
                                CDate20 = Convert.ToDateTime(sDate20);
                                User20 = submitals.lstUser20[0].UserUID;
                                if (isUpdateSubmital)
                                    db.DeleteSubmittal_MultipleUsers(sDocumentUID, 20);
                                selectedCount = submitals.lstUser20.Count;
                                if (selectedCount > 1)
                                {

                                    FlowStep20_IsMUser = "Y";
                                    foreach (var listItem in submitals.lstUser20)
                                    {
                                        db.InsertSubmittal_MultipleUsers(Guid.NewGuid(), sDocumentUID, 20, listItem.UserUID);
                                    }
                                }
                            }

                            result = db.DoumentMaster_Insert_or_Update_FlowAll(sDocumentUID, workpackageid, projectId, new Guid(TaskUID), SubmittalName, new Guid(SubmittalCategory),
                            "", "Submittle Document", 0.0, new Guid(FlowUID), DocStartDate, new Guid(SubmitterUID), CDate1,
                            submitals.lstQualityEngg[0].UserUID, CDate2, submitals.lstReviewer_B[0].UserUID, CDate3, submitals.lstReviewer[0].UserUID, CDate4,
                            submitals.lstApproval[0].UserUID, CDate5, Convert.ToInt16(EstimatedDocuments), Remarks, DocumentSearchType, IsSync,
                            submitals.lstUser6[0].UserUID, CDate6,
                            User7, CDate7,
                              User8, CDate8,
                               User9, CDate9,
                                User10, CDate10,
                                 User11, CDate11,
                                  User12, CDate12,
                                  User13, CDate13,
                                    User14, CDate14,
                                    User15, CDate15,
                                      User16, CDate16,
                                       User17, CDate17,
                                       User18, CDate18,
                                        User19, CDate19,
                                          User20, CDate20,
                                          int.Parse(dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString()),
                                          FlowStep1_IsMUser, FlowStep2_IsMUser, FlowStep3_IsMUser, FlowStep4_IsMUser
                                          , FlowStep5_IsMUser
                                          , FlowStep6_IsMUser
                                          , FlowStep7_IsMUser
                                          , FlowStep8_IsMUser
                                          , FlowStep9_IsMUser
                                          , FlowStep10_IsMUser
                                          , FlowStep11_IsMUser
                                          , FlowStep12_IsMUser
                                          , FlowStep13_IsMUser
                                          , FlowStep14_IsMUser
                                          , FlowStep15_IsMUser
                                          , FlowStep16_IsMUser
                                          , FlowStep17_IsMUser
                                          , FlowStep18_IsMUser
                                          , FlowStep19_IsMUser
                                          , FlowStep20_IsMUser);
                            if (result == 0)
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Submital did not created !",
                                };
                                return Json(resultmain);
                            }
                        }



                    }

                    if (result == -1)
                    {
                        var result_a = new
                        {
                            Status = "Failure",
                            Message = "Submittal already Exists.Try using diferent name !",
                        };
                        return Json(result_a);
                    }

                    var result_1 = new
                    {
                        Status = "Success",
                        Message = "Submittal Added Sucessfully !",
                        SubmittalUID = sDocumentUID
                    };
                    return Json(result_1);

                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    Status = "Failure",
                    Message = ex.Message,
                };
                return Json(result);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/ExtoIntegration/AddDocument")]
        public IHttpActionResult AddDocument()
        {
            bool sError = false;
            string ErrorText = "";
            string CoverUID = string.Empty;
            string DocUID = string.Empty;
            string AttatchUID = string.Empty;
            string Flowtype = string.Empty;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var ProjectName = httpRequest.Params["ProjectName"];
                    var WorkpackageName = httpRequest.Params["WorkpackageName"];
                    var SubmittalUID = httpRequest.Params["SubmittalUID"];
                    var ReferenceNumber = httpRequest.Params["ReferenceNumber"];
                    var Description = httpRequest.Params["Description"];
                    var DocumentType = httpRequest.Params["DocumentType"];
                    var DocMedia_HardCopy = httpRequest.Params["DocMedia_HardCopy"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_HardCopy"]) : false;
                    var DocMedia_SoftCopy_PDF = httpRequest.Params["DocMedia_SoftCopy_PDF"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_PDF"]) : false;
                    var DocMedia_SoftCopy_Editable = httpRequest.Params["DocMedia_SoftCopy_Editable"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_Editable"]) : false;
                    var DocMedia_SoftCopy_Ref = httpRequest.Params["DocMedia_SoftCopy_Ref"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_Ref"]) : false;
                    var DocMedia_HardCopy_Ref = httpRequest.Params["DocMedia_HardCopy_Ref"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_HardCopy_Ref"]) : false;
                    var DocMedia_NoMedia = httpRequest.Params["DocMedia_NoMedia"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_NoMedia"]) : false;
                    var Originator = httpRequest.Params["Originator"];
                    var OriginatorRefNo = httpRequest.Params["OriginatorRefNo"];
                    var IncomingReciveDate = httpRequest.Params["IncomingReciveDate"];
                    //var CoverLetterDate = httpRequest.Params["CoverLetterDate"];
                    var DocumentDate = httpRequest.Params["DocumentDate"];
                    var FileReferenceNumber = httpRequest.Params["FileReferenceNumber"];
                    var Remarks = httpRequest.Params["Remarks"];

                    bool CoverLetterUpload = false, DocumentsUpload = false;
                    var wExists = string.Empty;
                    var pExists = db.ProjectNameExists(ProjectName);
                    string ActualDocumentUID = string.Empty;

                    //added on 28/10/2022 to check file physical path length for file
                    string filepath = string.Empty;
                    for (int i = 0; i < httpRequest.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = httpRequest.Files[i];
                       
                        if (httpPostedFile != null)
                        {
                            filepath = ConfigurationManager.AppSettings["DocumentsPath"] + pExists + "//" + wExists + "//" + httpPostedFile.FileName;
                            if(filepath.Length > 220)
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error: One of the file name is too long,please reduce and send it again" 
                                });
                            }

                        }
                    }
                    //
                            if (!string.IsNullOrEmpty(pExists))
                    {
                        wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                        if (!string.IsNullOrEmpty(wExists))
                        {
                            if (Guid.TryParse(SubmittalUID, out Guid submittaluid))
                            {
                                if (!string.IsNullOrEmpty(Description))
                                {
                                    if (!string.IsNullOrEmpty(DocumentType))
                                    {
                                        string[] formats = { "dd/MM/yyyy" };
                                        if (DateTime.TryParseExact(DocumentDate, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime documentDate))
                                        {
                                            if (DocumentType == "Cover Letter")
                                            {
                                                if (string.IsNullOrEmpty(Originator))
                                                {
                                                    sError = true;
                                                    ErrorText = "Originator cannot be empty.";
                                                }
                                                if (string.IsNullOrEmpty(OriginatorRefNo))
                                                {
                                                    sError = true;
                                                    ErrorText = "Originator Reference number cannot be empty.";
                                                }
                                                if (!DateTime.TryParseExact(IncomingReciveDate, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime incomingReceiveDate))
                                                {
                                                    sError = true;
                                                    ErrorText = "Invalid Incoming Receive Date.";
                                                }
                                                if (httpRequest.Files.Count == 0)
                                                {
                                                    sError = true;
                                                    ErrorText = "Please upload a document.";
                                                }
                                                if (documentDate > incomingReceiveDate)
                                                {
                                                    sError = true;
                                                    ErrorText = "Document Date connot be greater than Incoming Receive date.";
                                                }
                                                for (int i = 0; i < httpRequest.Files.Count; i++)
                                                {
                                                    HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                                    if (httpPostedFile != null)
                                                    {
                                                        if (httpPostedFile.FileName.StartsWith("Cover") || httpPostedFile.FileName.StartsWith("Cover_Letter") || httpPostedFile.FileName.StartsWith("Cover Letter"))
                                                        {
                                                            CoverLetterUpload = true;
                                                        }
                                                        else
                                                        {
                                                            DocumentsUpload = true;
                                                        }
                                                    }
                                                }
                                                if (!CoverLetterUpload && httpRequest.Params["Coverletteruid"] == null)
                                                {
                                                    sError = true;
                                                    ErrorText = "Please upload a cover letter file.";
                                                }
                                                if (!DocumentsUpload & httpRequest.Params["Coverletteruid"] != null)
                                                {
                                                    sError = true;
                                                    ErrorText = "Please upload document file.";
                                                }
                                            }
                                            else
                                            {
                                                if (httpRequest.Files.Count == 0)
                                                {
                                                    sError = true;
                                                    ErrorText = "Please upload a document.";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Invalid Document Date.";
                                        }

                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Document Type cannot be empty.";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Description cannot be empty.";
                                }

                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Submittal UID.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Workpackage name not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Project name not found.";
                    }
                    //if(DocumentType != "Cover Letter")
                    //{
                    //    sError = true;
                    //    ErrorText = "Invalid document Type";
                    //}
                    // To handle coverletteruid 
                    // Added by Venkat on 25th May 2022
                    bool coverLetterUidExists = false;
                    bool docExists = false;
                    DataTable dtDocuments = new DataTable();
                    if (!sError && DocumentType == "Cover Letter")
                    {
                        string coverLetterUid = httpRequest.Params["Coverletteruid"];
                        if (coverLetterUid != null)
                        {
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];
                                if (httpPostedFile != null)
                                {
                                    string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                    if (Extn.ToLower() != ".exe" && Extn.ToLower() != ".msi" && Extn.ToLower() != ".db")
                                    {   
                                        //Added code by Venkat on 13 May 2022
                                        if (!httpPostedFile.FileName.StartsWith("Cover") && httpPostedFile.FileName != "")
                                        {
                                            docExists = true;
                                            break;
                                        }
                                    }
                                }

                            }
                           
                            coverLetterUidExists = true;
                            dtDocuments = db.getActualDocumentStatus(new Guid(pExists), new Guid(wExists), new Guid(coverLetterUid));
                            if (dtDocuments.Rows.Count == 0)
                            {
                                sError = true;
                                ErrorText = "Invalid Coverletteruid.";
                                //
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:" + ErrorText
                                });
                            }
                            if (!docExists)
                            {
                                sError = true;
                                ErrorText = "Document not available";
                                //
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:" + ErrorText
                                });
                            }
                        }
                    }

                    //added on 16/05/2023 for blob
                    //for blob
                    string Connectionstring = db.getProjectConnectionString(new Guid(pExists));

                    //
                    //if (!sError && DocumentType == "Cover Letter")
                    //{
                    DataSet ds = db.getDocumentsbyDocID(new Guid(SubmittalUID));
                        string ExtnSTPC = string.Empty;
                       string FilenameSTPC = string.Empty;
                    string UploadphysicalpathSTPC = string.Empty;
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string cStatus = "Submitted", FileDatetime = DateTime.Now.ToString("dd MMM yyyy hh-mm-ss tt"), sDocumentPath = string.Empty,
                                DocumentFor = string.Empty, IncomingRec_Date_String = string.Empty, CoverPagePath = string.Empty, CoverLetterUID = ""; ;
                            DataSet dsFlowcheck = db.GetDocumentFlows_by_UID(new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()));
                            if (dsFlowcheck.Tables[0].Rows[0]["Type"] != DBNull.Value)
                            {
                                Flowtype = dsFlowcheck.Tables[0].Rows[0]["Type"].ToString();
                                if (dsFlowcheck.Tables[0].Rows[0]["Type"].ToString() == "STP" || dsFlowcheck.Tables[0].Rows[0]["Type"].ToString() == "STP-C")
                                {
                                    cStatus = "Reconciliation";
                                }
                            }

                            if (!string.IsNullOrEmpty(IncomingReciveDate))
                            {
                                IncomingRec_Date_String = IncomingReciveDate;
                            }
                            else
                            {
                                IncomingRec_Date_String = DocumentDate;
                            }
                            IncomingRec_Date_String = db.ConvertDateFormat(IncomingRec_Date_String);
                            DateTime IncomingRec_Date = Convert.ToDateTime(IncomingRec_Date_String);

                            if (string.IsNullOrEmpty(DocumentDate))
                            {
                                DocumentDate = DateTime.MinValue.ToString("dd/MM/yyyy");
                            }
                            DocumentDate = db.ConvertDateFormat(DocumentDate);
                            DateTime Document_Date = Convert.ToDateTime(DocumentDate);
                            if (!coverLetterUidExists)
                            {
                                //Cover Letter Insert
                                for (int i = 0; i < httpRequest.Files.Count; i++)
                                {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];
                                if (httpPostedFile != null)
                                {
                                    string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                    //for STP-C
                                    ExtnSTPC = Extn;
                                    //for STP-C
                                    if (Extn.ToLower() != ".exe" && Extn.ToLower() != ".msi" && Extn.ToLower() != ".db")
                                    {
                                        if ((httpPostedFile.FileName.StartsWith("Cover") || httpPostedFile.FileName.StartsWith("Cover_Letter")) && DocumentType == "Cover Letter")
                                        {
                                            DocumentFor = "Cover Letter";
                                            sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + pExists + "//" + wExists + "//CoverLetter";

                                            if (!Directory.Exists(sDocumentPath))
                                            {
                                                Directory.CreateDirectory(sDocumentPath);
                                            }
                                            string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                            //for STP-C
                                            FilenameSTPC = sFileName;
                                            //
                                            //httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_1_copy" + Extn);
                                            //string savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
                                            //CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;

                                            int RandomNo = db.GenerateRandomNo();
                                            httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn);
                                            string savedPath = sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn;
                                            CoverPagePath = sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1" + Extn;
                                            //for STP-C
                                            UploadphysicalpathSTPC = CoverPagePath;
                                            //
                                            EncryptFile(savedPath, CoverPagePath);

                                            //for blob storage Cover letter
                                            byte[] filetobytes = db.FileToByteArray(CoverPagePath);
                                            //
                                            //

                                            //  savedPath = "~/" + pExists + "/" + wExists + "/CoverLetter" + "/" + sFileName + "_1_copy" + Extn;
                                            //  CoverPagePath = "~/" + pExists + "/" + wExists + "/CoverLetter" + "/" + sFileName + "_1" + Extn;
                                            //
                                            savedPath = "~/" + pExists + "/" + wExists + "/CoverLetter" + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn;
                                            CoverPagePath = "~/" + pExists + "/" + wExists + "/CoverLetter" + "/" + sFileName + "_" + RandomNo + "_1" + Extn;


                                            CoverLetterUID = Guid.NewGuid().ToString();

                                           

                                            int RetCount = db.DocumentCoverLetter_Insert_or_Update(new Guid(CoverLetterUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, "",
                                            DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn, DocMedia_HardCopy ? "true" : "false",
                                            DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                            DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus, Originator ?? "", Document_Date);
                                            if (RetCount <= 0)
                                            {
                                                sError = true;
                                                ErrorText = "Error occured while inserting cover letter. Please contact system admin.";
                                            }
                                            else
                                            {
                                                //store the blob for cover letter
                                                db.InsertActualDocumentsBlob(new Guid(CoverLetterUID), filetobytes, Connectionstring);

                                            }
                                            CoverUID = CoverLetterUID;
                                            try
                                            {
                                                if (File.Exists(savedPath))
                                                {
                                                    File.Delete(savedPath);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                //throw
                                            }
                                        }
                                    }
                                }
                            }
                            }
                            else
                            {
                                // If Coverletteruid  is added in parameter, it will insert here.
                                // Added by Venkat on 25th May 2022
                                //if (dtDocuments.Rows.Count > 0)
                                //{
                                //    CoverPagePath = dtDocuments.Rows[0]["ActualDocument_Path"].ToString();
                                //    string sFileName = Path.GetFileName(dtDocuments.Rows[0]["ActualDocument_Path"].ToString());
                                //    string Extn = Path.GetExtension(dtDocuments.Rows[0]["ActualDocument_Path"].ToString());
                                //    CoverLetterUID = Guid.NewGuid().ToString();

                                //    int RetCount = db.DocumentCoverLetter_Insert_or_Update(new Guid(CoverLetterUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, "",
                                //    DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn, DocMedia_HardCopy ? "true" : "false",
                                //    DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                //    DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus, Originator ?? "", Document_Date);
                                //    if (RetCount <= 0)
                                //    {
                                //        sError = true;
                                //        ErrorText = "Error occured while inserting cover letter. Please contact system admin.";
                                //    }
                                //    else
                                //    {
                                //        CoverUID = CoverLetterUID;
                                //    }
                                //}
                            }
                        // added on 03/06/2022
                        if(sError)
                        {
                            //
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:" + ErrorText
                            });
                        }

                        DocUID = string.Empty;
                        //ActualDocument Insert
                        string sDate1 = "", sDate2 = "", sDate3 = "", sDate4 = "", sDate5 = "", sDate6 = "", sDate7 = "", sDate8 = "", sDate9 = "", sDate10 = "", sDate11 = "", sDate12 = "", sDate13 = "", sDate14 = "", sDate15 = "", sDate16 = "", sDate17 = "", sDate18 = "", sDate19 = "", sDate20 = "";
                        DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now, CDate4 = DateTime.Now, CDate5 = DateTime.Now, DocStartDate = DateTime.Now, CDate6 = DateTime.Now, CDate7 = DateTime.Now, CDate8 = DateTime.Now, CDate9 = DateTime.Now, CDate10 = DateTime.Now, CDate11 = DateTime.Now, CDate12 = DateTime.Now, CDate13 = DateTime.Now, CDate14 = DateTime.Now, CDate15 = DateTime.Now, CDate16 = DateTime.Now, CDate17 = DateTime.Now, CDate18 = DateTime.Now, CDate19 = DateTime.Now, CDate20 = DateTime.Now;
                        DataSet dsFlow = db.GetDocumentFlows_by_UID(new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()));

                        if (dsFlowcheck.Tables[0].Rows[0]["Type"].ToString() != "STP-C")
                        {
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];
                                if (httpPostedFile != null)
                                {
                                    string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                    if (Extn.ToLower() != ".exe" && Extn.ToLower() != ".msi" && Extn.ToLower() != ".db")
                                    {                                                       //Added code by Venkat on 13 May 2022
                                        if (!httpPostedFile.FileName.StartsWith("Cover") && httpPostedFile.FileName != "")
                                        {
                                            
                                            if (!checkDocumentExists(Path.GetFileNameWithoutExtension(httpPostedFile.FileName), SubmittalUID))
                                            {
                                                if (DocumentType == "General Document")
                                                {
                                                    DocumentFor = "General Document";
                                                }
                                                else if (DocumentType == "Photographs")
                                                {
                                                    DocumentFor = "Photographs";
                                                }
                                                else
                                                {
                                                    DocumentFor = "Document";
                                                }

                                                sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + pExists + "//" + wExists + "//Documents";

                                                if (!Directory.Exists(sDocumentPath))
                                                {
                                                    Directory.CreateDirectory(sDocumentPath);
                                                }

                                                string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);

                                                string savedPath = string.Empty;
                                                int RandomNo = db.GenerateRandomNo();
                                                if (DocumentType != "Photographs")
                                                {
                                                    if (db.IsDocNameExists(wExists, sFileName))
                                                    {
                                                        savedPath = sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn;
                                                        httpPostedFile.SaveAs(savedPath);

                                                        CoverPagePath = sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1" + Extn;
                                                        EncryptFile(savedPath, CoverPagePath);
                                                    }
                                                    else
                                                    {
                                                        savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
                                                        httpPostedFile.SaveAs(savedPath);

                                                        CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;
                                                        EncryptFile(savedPath, CoverPagePath);
                                                    }
                                                }
                                                else
                                                {
                                                    savedPath = sDocumentPath + "/" + DateTime.Now.Ticks.ToString() + Path.GetFileName(httpPostedFile.FileName);
                                                    httpPostedFile.SaveAs(savedPath);

                                                }
                                                ActualDocumentUID = Guid.NewGuid().ToString();

                                                //for blob storage
                                                byte[] filetobytes = db.FileToByteArray(CoverPagePath);

                                                //

                                                string UploadFilePhysicalpath = CoverPagePath; 
                                                //
                                                if (DocumentType != "Photographs")
                                                {
                                                    if (db.IsDocNameExists(wExists, sFileName))
                                                    {
                                                        savedPath = "~/" + pExists + "/" + wExists + "/Documents" + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn;
                                                        CoverPagePath = "~/" + pExists + "/" + wExists + "/Documents" + "/" + sFileName + "_" + RandomNo + "_1" + Extn;
                                                    }
                                                    else
                                                    {
                                                        savedPath = "~/" + pExists + "/" + wExists + "/Documents" + "/" + sFileName + "_1_copy" + Extn;
                                                        CoverPagePath = "~/" + pExists + "/" + wExists + "/Documents" + "/" + sFileName  +  "_1" + Extn;

                                                    }
                                                }
                                                //
                                                string Flow1DisplayName = "", Flow2DisplayName = "", Flow3DisplayName = "", Flow4DisplayName = "", Flow5DisplayName = "";
                                                Guid sStatusUID = Guid.NewGuid();
                                                Guid sDocVersionUID = Guid.NewGuid();
                                                if (dsFlow.Tables[0].Rows.Count > 0)
                                                {
                                                    if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "-1")
                                                    {
                                                        Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();

                                                        sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate1 = db.ConvertDateFormat(sDate1);
                                                        CDate1 = Convert.ToDateTime(sDate1);

                                                        int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow1(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo ?? "",
                                                        DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                                        DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                                        DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", savedPath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
                                                        new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, Flow1DisplayName, Originator ?? "", Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission");
                                                        if (cnt <= 0)
                                                        {
                                                            sError = true;
                                                            ErrorText = "Error occured while inserting document. Please contact system admin.";
                                                        }
                                                        else
                                                        {
                                                            //store the blob
                                                            db.InsertActualDocumentsBlob(new Guid(ActualDocumentUID), filetobytes, Connectionstring);
                                                        }
                                                    }
                                                    else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "1")
                                                    {
                                                        Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();

                                                        sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate1 = db.ConvertDateFormat(sDate1);
                                                        CDate1 = Convert.ToDateTime(sDate1);

                                                        int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow1(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo ?? "",
                                                        DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                                        DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                                        DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
                                                        new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, Flow1DisplayName, Originator ?? "", Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission");
                                                        if (cnt <= 0)
                                                        {
                                                            sError = true;
                                                            ErrorText = "Error occured while inserting document. Please contact system admin.";
                                                        }
                                                        else
                                                        {
                                                            //store the blob
                                                            db.InsertActualDocumentsBlob(new Guid(ActualDocumentUID), filetobytes, Connectionstring);
                                                        }
                                                    }
                                                    else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "2")
                                                    {
                                                        Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
                                                        Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();

                                                        sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate1 = db.ConvertDateFormat(sDate1);
                                                        CDate1 = Convert.ToDateTime(sDate1);

                                                        sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate2 = db.ConvertDateFormat(sDate2);
                                                        CDate2 = Convert.ToDateTime(sDate2);

                                                        int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow2(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                                                          DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                                          DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                                          DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
                                                          new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, Flow1DisplayName, Flow2DisplayName, Originator, Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission", sStatusUID, sDocVersionUID);
                                                        if (cnt <= 0)
                                                        {
                                                            sError = true;
                                                            ErrorText = "Error occured while inserting document. Please contact system admin.";
                                                        }
                                                        else
                                                        {
                                                            //store the blob
                                                            db.InsertActualDocumentsBlob(new Guid(ActualDocumentUID), filetobytes, Connectionstring);
                                                            //for status blob table
                                                            db.InsertDocumentStatusBlob_FirstTime(sStatusUID, new Guid(ActualDocumentUID), new Guid(CoverLetterUID), Connectionstring);
                                                            //for DocVersion Table
                                                            db.InsertDocumentVersionBlob(sDocVersionUID, new Guid(ActualDocumentUID), filetobytes, null, Connectionstring);

                                                        }

                                                    }
                                                    else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "3")
                                                    {
                                                        Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
                                                        Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
                                                        Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();

                                                        sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate1 = db.ConvertDateFormat(sDate1);
                                                        CDate1 = Convert.ToDateTime(sDate1);


                                                        sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate2 = db.ConvertDateFormat(sDate2);
                                                        CDate2 = Convert.ToDateTime(sDate2);

                                                        sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate3 = db.ConvertDateFormat(sDate3);
                                                        CDate3 = Convert.ToDateTime(sDate3);

                                                        sDate4 = DateTime.Now.ToString("dd/MM/yyyy");
                                                        sDate4 = db.ConvertDateFormat(sDate4);
                                                        CDate4 = Convert.ToDateTime(CDate4);

                                                        int cnt = db.Document_Insert_or_Update(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                                                       DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                                       DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                                       DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
                                                       new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3,
                                                       Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Originator, Document_Date, UploadFilePhysicalpath, CoverLetterUID, "Submission", sStatusUID, sDocVersionUID);
                                                        if (cnt <= 0)
                                                        {
                                                            sError = true;
                                                            ErrorText = "Error occured while inserting document. Please contact system admin.";
                                                        }
                                                        else
                                                        {
                                                            //store the blob
                                                            db.InsertActualDocumentsBlob(new Guid(ActualDocumentUID), filetobytes, Connectionstring);
                                                            //for status blob table
                                                            db.InsertDocumentStatusBlob_FirstTime(sStatusUID, new Guid(ActualDocumentUID), new Guid(CoverLetterUID), Connectionstring);
                                                            //for DocVersion Table
                                                            db.InsertDocumentVersionBlob(sDocVersionUID, new Guid(ActualDocumentUID), filetobytes, null, Connectionstring);

                                                        }

                                                    }
                                                    else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "4")
                                                    {
                                                        Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
                                                        Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
                                                        Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();
                                                        Flow4DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep4_DisplayName"].ToString();

                                                        sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate1 = db.ConvertDateFormat(sDate1);
                                                        CDate1 = Convert.ToDateTime(sDate1);


                                                        sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate2 = db.ConvertDateFormat(sDate2);
                                                        CDate2 = Convert.ToDateTime(sDate2);

                                                        sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate3 = db.ConvertDateFormat(sDate3);
                                                        CDate3 = Convert.ToDateTime(sDate3);


                                                        sDate4 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate4 = db.ConvertDateFormat(sDate4);
                                                        CDate4 = Convert.ToDateTime(sDate4);

                                                        int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow4(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                                                          DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                                          DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                                          DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
                                                          new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3, new Guid(ds.Tables[0].Rows[0]["FlowStep4_UserUID"].ToString()), CDate4,
                                                          Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Flow4DisplayName, Originator, Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission", sStatusUID, sDocVersionUID);
                                                        if (cnt <= 0)
                                                        {
                                                            sError = true;
                                                            ErrorText = "Error occured while inserting document. Please contact system admin.";
                                                        }
                                                        else
                                                        {
                                                            //store the blob
                                                            db.InsertActualDocumentsBlob(new Guid(ActualDocumentUID), filetobytes, Connectionstring);
                                                            //for status blob table
                                                            db.InsertDocumentStatusBlob_FirstTime(sStatusUID, new Guid(ActualDocumentUID), new Guid(CoverLetterUID), Connectionstring);
                                                            //for DocVersion Table
                                                            db.InsertDocumentVersionBlob(sDocVersionUID, new Guid(ActualDocumentUID), filetobytes, null, Connectionstring);

                                                        }

                                                    }
                                                    else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "5")
                                                    {
                                                        Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
                                                        Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
                                                        Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();
                                                        Flow4DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep4_DisplayName"].ToString();
                                                        Flow5DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep5_DisplayName"].ToString();

                                                        sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate1 = db.ConvertDateFormat(sDate1);
                                                        CDate1 = Convert.ToDateTime(sDate1);

                                                        sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate2 = db.ConvertDateFormat(sDate2);
                                                        CDate2 = Convert.ToDateTime(sDate2);

                                                        sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate3 = db.ConvertDateFormat(sDate3);
                                                        CDate3 = Convert.ToDateTime(sDate3);


                                                        sDate4 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate4 = db.ConvertDateFormat(sDate4);
                                                        CDate4 = Convert.ToDateTime(sDate4);

                                                        sDate5 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        sDate5 = db.ConvertDateFormat(sDate5);
                                                        CDate5 = Convert.ToDateTime(sDate5);

                                                        int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow5(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                                                          DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                                          DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                                          DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
                                                          new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3, new Guid(ds.Tables[0].Rows[0]["FlowStep4_UserUID"].ToString()), CDate4, new Guid(ds.Tables[0].Rows[0]["FlowStep5_UserUID"].ToString()), CDate5,
                                                          Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Flow4DisplayName, Flow5DisplayName, Originator, Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission", sStatusUID, sDocVersionUID);
                                                        if (cnt <= 0)
                                                        {
                                                            sError = true;
                                                            ErrorText = "Error occured while inserting document. Please contact system admin.";
                                                        }
                                                        else
                                                        {
                                                            //store the blob
                                                            db.InsertActualDocumentsBlob(new Guid(ActualDocumentUID), filetobytes, Connectionstring);
                                                            //for status blob table
                                                            db.InsertDocumentStatusBlob_FirstTime(sStatusUID, new Guid(ActualDocumentUID), new Guid(CoverLetterUID), Connectionstring);
                                                            //for DocVersion Table
                                                            db.InsertDocumentVersionBlob(sDocVersionUID, new Guid(ActualDocumentUID), filetobytes, null, Connectionstring);

                                                        }
                                                    }
                                                    else // for all flows with step > 5 ( 6 to 20) added on 07/03/2022
                                                    {
                                                        #region ALl except

                                                        sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                                        sDate1 = db.ConvertDateFormat(sDate1);
                                                        CDate1 = Convert.ToDateTime(sDate1);


                                                        sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        //sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                                                        sDate2 = db.ConvertDateFormat(sDate2);
                                                        CDate2 = Convert.ToDateTime(sDate2);

                                                        sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        //sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                                                        sDate3 = db.ConvertDateFormat(sDate3);
                                                        CDate3 = Convert.ToDateTime(sDate3);


                                                        sDate4 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        //sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
                                                        sDate4 = db.ConvertDateFormat(sDate4);
                                                        CDate4 = Convert.ToDateTime(sDate4);

                                                        sDate5 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep5_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        //sDate5 = sDate5.Split('/')[1] + "/" + sDate5.Split('/')[0] + "/" + sDate5.Split('/')[2];
                                                        sDate5 = db.ConvertDateFormat(sDate5);
                                                        CDate5 = Convert.ToDateTime(sDate5);

                                                        sDate6 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep6_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                        //sDate5 = sDate5.Split('/')[1] + "/" + sDate5.Split('/')[0] + "/" + sDate5.Split('/')[2];
                                                        sDate6 = db.ConvertDateFormat(sDate6);
                                                        CDate6 = Convert.ToDateTime(sDate6);

                                                        int steps = int.Parse(dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString());
                                                        Guid User7 = Guid.NewGuid(), User8 = Guid.NewGuid(), User9 = Guid.NewGuid(), User10 = Guid.NewGuid(), User11 = Guid.NewGuid(), User12 = Guid.NewGuid(), User13 = Guid.NewGuid(), User14 = Guid.NewGuid(), User15 = Guid.NewGuid(), User16 = Guid.NewGuid(), User17 = Guid.NewGuid(), User18 = Guid.NewGuid(), User19 = Guid.NewGuid(), User20 = Guid.NewGuid();

                                                        if (steps >= 7)
                                                        {
                                                            sDate7 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep7_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate7 = db.ConvertDateFormat(sDate7);
                                                            CDate7 = Convert.ToDateTime(sDate7);
                                                            User7 = new Guid(ds.Tables[0].Rows[0]["FlowStep7_UserUID"].ToString());
                                                        }

                                                        if (steps >= 8)
                                                        {
                                                            sDate8 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep8_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate8 = db.ConvertDateFormat(sDate8);
                                                            CDate8 = Convert.ToDateTime(sDate8);
                                                            User8 = new Guid(ds.Tables[0].Rows[0]["FlowStep8_UserUID"].ToString());
                                                        }

                                                        if (steps >= 9)
                                                        {
                                                            sDate9 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep9_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate9 = db.ConvertDateFormat(sDate9);
                                                            CDate9 = Convert.ToDateTime(sDate9);
                                                            User9 = new Guid(ds.Tables[0].Rows[0]["FlowStep9_UserUID"].ToString());
                                                        }

                                                        if (steps >= 10)
                                                        {
                                                            sDate10 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep10_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate10 = db.ConvertDateFormat(sDate10);
                                                            CDate10 = Convert.ToDateTime(sDate10);
                                                            User10 = new Guid(ds.Tables[0].Rows[0]["FlowStep10_UserUID"].ToString());
                                                        }

                                                        if (steps >= 11)
                                                        {
                                                            sDate11 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep11_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate11 = db.ConvertDateFormat(sDate11);
                                                            CDate11 = Convert.ToDateTime(sDate11);
                                                            User11 = new Guid(ds.Tables[0].Rows[0]["FlowStep11_UserUID"].ToString());
                                                        }

                                                        if (steps >= 12)
                                                        {
                                                            sDate12 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep12_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate12 = db.ConvertDateFormat(sDate12);
                                                            CDate12 = Convert.ToDateTime(sDate12);
                                                            User12 = new Guid(ds.Tables[0].Rows[0]["FlowStep12_UserUID"].ToString());
                                                        }

                                                        if (steps >= 13)
                                                        {
                                                            sDate13 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep13_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate13 = db.ConvertDateFormat(sDate13);
                                                            CDate13 = Convert.ToDateTime(sDate13);
                                                            User13 = new Guid(ds.Tables[0].Rows[0]["FlowStep13_UserUID"].ToString());
                                                        }

                                                        if (steps >= 14)
                                                        {
                                                            sDate14 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep14_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate14 = db.ConvertDateFormat(sDate14);
                                                            CDate14 = Convert.ToDateTime(sDate14);
                                                            User14 = new Guid(ds.Tables[0].Rows[0]["FlowStep14_UserUID"].ToString());
                                                        }

                                                        if (steps >= 15)
                                                        {
                                                            sDate15 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep15_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate15 = db.ConvertDateFormat(sDate15);
                                                            CDate15 = Convert.ToDateTime(sDate15);
                                                            User15 = new Guid(ds.Tables[0].Rows[0]["FlowStep15_UserUID"].ToString());
                                                        }

                                                        if (steps >= 16)
                                                        {
                                                            sDate16 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep16_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate16 = db.ConvertDateFormat(sDate16);
                                                            CDate16 = Convert.ToDateTime(sDate16);
                                                            User16 = new Guid(ds.Tables[0].Rows[0]["FlowStep16_UserUID"].ToString());
                                                        }

                                                        if (steps >= 17)
                                                        {
                                                            sDate17 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep17_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate17 = db.ConvertDateFormat(sDate17);
                                                            CDate17 = Convert.ToDateTime(sDate17);
                                                            User17 = new Guid(ds.Tables[0].Rows[0]["FlowStep17_UserUID"].ToString());
                                                        }

                                                        if (steps >= 18)
                                                        {
                                                            sDate18 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep18_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate18 = db.ConvertDateFormat(sDate18);
                                                            CDate18 = Convert.ToDateTime(sDate18);
                                                            User18 = new Guid(ds.Tables[0].Rows[0]["FlowStep18_UserUID"].ToString());
                                                        }

                                                        if (steps >= 19)
                                                        {
                                                            sDate19 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep19_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate19 = db.ConvertDateFormat(sDate19);
                                                            CDate19 = Convert.ToDateTime(sDate19);
                                                            User19 = new Guid(ds.Tables[0].Rows[0]["FlowStep19_UserUID"].ToString());
                                                        }

                                                        if (steps >= 20)
                                                        {
                                                            sDate20 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep20_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                                            sDate20 = db.ConvertDateFormat(sDate20);
                                                            CDate20 = Convert.ToDateTime(sDate20);
                                                            User20 = new Guid(ds.Tables[0].Rows[0]["FlowStep20_UserUID"].ToString());
                                                        }
                                                        #endregion

                                                        int cnt = db.Document_Insert_or_Update_with_RelativePath_FlowAll(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                                                          DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
                                                          DocMedia_HardCopy == true ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                                                          DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks, FileReferenceNumber ?? "", cStatus,
                                                          new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3, new Guid(ds.Tables[0].Rows[0]["FlowStep4_UserUID"].ToString()), CDate4, new Guid(ds.Tables[0].Rows[0]["FlowStep5_UserUID"].ToString()), CDate5,
                                                          new Guid(ds.Tables[0].Rows[0]["FlowStep6_UserUID"].ToString()), CDate6,
                                                          User7, CDate7,
                                                          User8, CDate8,
                                                          User9, CDate9,
                                                          User10, CDate10,
                                                          User11, CDate11,
                                                          User12, CDate12,
                                                          User13, CDate13,
                                                          User14, CDate14,
                                                          User15, CDate15,
                                                          User16, CDate16,
                                                          User17, CDate17,
                                                          User18, CDate18,
                                                          User19, CDate19,
                                                          User20, CDate20,
                                                          Originator, Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission", steps, sStatusUID, sDocVersionUID);
                                                        if (cnt <= 0)
                                                        {
                                                            sError = true;
                                                            ErrorText = "Error occured while inserting document. Please contact system admin.";
                                                        }
                                                        else
                                                        {
                                                            //store the blob
                                                            db.InsertActualDocumentsBlob(new Guid(ActualDocumentUID), filetobytes, Connectionstring);
                                                            //for status blob table
                                                            db.InsertDocumentStatusBlob_FirstTime(sStatusUID, new Guid(ActualDocumentUID), new Guid(CoverLetterUID), Connectionstring);
                                                            //for DocVersion Table
                                                            db.InsertDocumentVersionBlob(sDocVersionUID, new Guid(ActualDocumentUID), filetobytes, null, Connectionstring);

                                                        }
                                                    }
                                                    // store the origintaor reference no in separate table...added on 05/04/2022
                                                    db.InsertorUpdateRefNoHistroy(Guid.NewGuid(), new Guid(ActualDocumentUID), OriginatorRefNo, "");







                                                    DocUID += ActualDocumentUID + ",";
                                                }

                                                try
                                                {
                                                    if (DocumentType != "Photographs")
                                                    {
                                                        if (File.Exists(savedPath))
                                                        {
                                                            File.Delete(savedPath);
                                                        }
                                                    }

                                                }
                                                catch (Exception ex)
                                                {
                                                    //throw
                                                }
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Document is already attached";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (dsFlowcheck.Tables[0].Rows[0]["Type"].ToString() == "STP-C")
                        {
                            ActualDocumentUID = Guid.NewGuid().ToString();
                            string UploadFilePhysicalpath = UploadphysicalpathSTPC;
                            DocumentFor = "Document";
                            string attachmentUID = string.Empty;

                            //for blob storage
                            byte[] filetobytes = db.FileToByteArray(UploadFilePhysicalpath);

                            //
                            Guid sStatusUID = Guid.NewGuid();
                            Guid sDocVersionUID = Guid.NewGuid();
                            #region ALl except

                            sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            sDate1 = db.ConvertDateFormat(sDate1);
                            CDate1 = Convert.ToDateTime(sDate1);


                            sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                            //sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                            sDate2 = db.ConvertDateFormat(sDate2);
                            CDate2 = Convert.ToDateTime(sDate2);

                            sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                            //sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                            sDate3 = db.ConvertDateFormat(sDate3);
                            CDate3 = Convert.ToDateTime(sDate3);


                            sDate4 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                            //sDate4 = sDate4.Split('/')[1] + "/" + sDate4.Split('/')[0] + "/" + sDate4.Split('/')[2];
                            sDate4 = db.ConvertDateFormat(sDate4);
                            CDate4 = Convert.ToDateTime(sDate4);

                            sDate5 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep5_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                            //sDate5 = sDate5.Split('/')[1] + "/" + sDate5.Split('/')[0] + "/" + sDate5.Split('/')[2];
                            sDate5 = db.ConvertDateFormat(sDate5);
                            CDate5 = Convert.ToDateTime(sDate5);

                            sDate6 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep6_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                            //sDate5 = sDate5.Split('/')[1] + "/" + sDate5.Split('/')[0] + "/" + sDate5.Split('/')[2];
                            sDate6 = db.ConvertDateFormat(sDate6);
                            CDate6 = Convert.ToDateTime(sDate6);

                            int steps = int.Parse(dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString());
                            Guid User7 = Guid.NewGuid(), User8 = Guid.NewGuid(), User9 = Guid.NewGuid(), User10 = Guid.NewGuid(), User11 = Guid.NewGuid(), User12 = Guid.NewGuid(), User13 = Guid.NewGuid(), User14 = Guid.NewGuid(), User15 = Guid.NewGuid(), User16 = Guid.NewGuid(), User17 = Guid.NewGuid(), User18 = Guid.NewGuid(), User19 = Guid.NewGuid(), User20 = Guid.NewGuid();

                            if (steps >= 7)
                            {
                                sDate7 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep7_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate7 = db.ConvertDateFormat(sDate7);
                                CDate7 = Convert.ToDateTime(sDate7);
                                User7 = new Guid(ds.Tables[0].Rows[0]["FlowStep7_UserUID"].ToString());
                            }

                            if (steps >= 8)
                            {
                                sDate8 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep8_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate8 = db.ConvertDateFormat(sDate8);
                                CDate8 = Convert.ToDateTime(sDate8);
                                User8 = new Guid(ds.Tables[0].Rows[0]["FlowStep8_UserUID"].ToString());
                            }

                            if (steps >= 9)
                            {
                                sDate9 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep9_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate9 = db.ConvertDateFormat(sDate9);
                                CDate9 = Convert.ToDateTime(sDate9);
                                User9 = new Guid(ds.Tables[0].Rows[0]["FlowStep9_UserUID"].ToString());
                            }

                            if (steps >= 10)
                            {
                                sDate10 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep10_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate10 = db.ConvertDateFormat(sDate10);
                                CDate10 = Convert.ToDateTime(sDate10);
                                User10 = new Guid(ds.Tables[0].Rows[0]["FlowStep10_UserUID"].ToString());
                            }

                            if (steps >= 11)
                            {
                                sDate11 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep11_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate11 = db.ConvertDateFormat(sDate11);
                                CDate11 = Convert.ToDateTime(sDate11);
                                User11 = new Guid(ds.Tables[0].Rows[0]["FlowStep11_UserUID"].ToString());
                            }

                            if (steps >= 12)
                            {
                                sDate12 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep12_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate12 = db.ConvertDateFormat(sDate12);
                                CDate12 = Convert.ToDateTime(sDate12);
                                User12 = new Guid(ds.Tables[0].Rows[0]["FlowStep12_UserUID"].ToString());
                            }

                            if (steps >= 13)
                            {
                                sDate13 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep13_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate13 = db.ConvertDateFormat(sDate13);
                                CDate13 = Convert.ToDateTime(sDate13);
                                User13 = new Guid(ds.Tables[0].Rows[0]["FlowStep13_UserUID"].ToString());
                            }

                            if (steps >= 14)
                            {
                                sDate14 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep14_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate14 = db.ConvertDateFormat(sDate14);
                                CDate14 = Convert.ToDateTime(sDate14);
                                User14 = new Guid(ds.Tables[0].Rows[0]["FlowStep14_UserUID"].ToString());
                            }

                            if (steps >= 15)
                            {
                                sDate15 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep15_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate15 = db.ConvertDateFormat(sDate15);
                                CDate15 = Convert.ToDateTime(sDate15);
                                User15 = new Guid(ds.Tables[0].Rows[0]["FlowStep15_UserUID"].ToString());
                            }

                            if (steps >= 16)
                            {
                                sDate16 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep16_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate16 = db.ConvertDateFormat(sDate16);
                                CDate16 = Convert.ToDateTime(sDate16);
                                User16 = new Guid(ds.Tables[0].Rows[0]["FlowStep16_UserUID"].ToString());
                            }

                            if (steps >= 17)
                            {
                                sDate17 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep17_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate17 = db.ConvertDateFormat(sDate17);
                                CDate17 = Convert.ToDateTime(sDate17);
                                User17 = new Guid(ds.Tables[0].Rows[0]["FlowStep17_UserUID"].ToString());
                            }

                            if (steps >= 18)
                            {
                                sDate18 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep18_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate18 = db.ConvertDateFormat(sDate18);
                                CDate18 = Convert.ToDateTime(sDate18);
                                User18 = new Guid(ds.Tables[0].Rows[0]["FlowStep18_UserUID"].ToString());
                            }

                            if (steps >= 19)
                            {
                                sDate19 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep19_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate19 = db.ConvertDateFormat(sDate19);
                                CDate19 = Convert.ToDateTime(sDate19);
                                User19 = new Guid(ds.Tables[0].Rows[0]["FlowStep19_UserUID"].ToString());
                            }

                            if (steps >= 20)
                            {
                                sDate20 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep20_TargetDate"].ToString()).ToString("dd/MM/yyyy");
                                sDate20 = db.ConvertDateFormat(sDate20);
                                CDate20 = Convert.ToDateTime(sDate20);
                                User20 = new Guid(ds.Tables[0].Rows[0]["FlowStep20_UserUID"].ToString());
                            }
                            #endregion

                            int cnt = db.Document_Insert_or_Update_with_RelativePath_FlowAll(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
                              DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), FilenameSTPC, Description, 1, ExtnSTPC,
                              DocMedia_HardCopy == true ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
                              DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks, FileReferenceNumber ?? "", cStatus,
                              new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3, new Guid(ds.Tables[0].Rows[0]["FlowStep4_UserUID"].ToString()), CDate4, new Guid(ds.Tables[0].Rows[0]["FlowStep5_UserUID"].ToString()), CDate5,
                              new Guid(ds.Tables[0].Rows[0]["FlowStep6_UserUID"].ToString()), CDate6,
                              User7, CDate7,
                              User8, CDate8,
                              User9, CDate9,
                              User10, CDate10,
                              User11, CDate11,
                              User12, CDate12,
                              User13, CDate13,
                              User14, CDate14,
                              User15, CDate15,
                              User16, CDate16,
                              User17, CDate17,
                              User18, CDate18,
                              User19, CDate19,
                              User20, CDate20,
                              Originator, Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission", steps, sStatusUID, sDocVersionUID);
                            if (cnt <= 0)
                            {
                                sError = true;
                                ErrorText = "Error occured while inserting document. Please contact system admin.";
                            }
                            else
                            {
                                //store the blob
                                db.InsertActualDocumentsBlob(new Guid(ActualDocumentUID), filetobytes, Connectionstring);
                                //for status blob table
                                db.InsertDocumentStatusBlob_FirstTime(sStatusUID, new Guid(ActualDocumentUID), new Guid(CoverLetterUID), Connectionstring);
                                //for DocVersion Table
                                db.InsertDocumentVersionBlob(sDocVersionUID, new Guid(ActualDocumentUID), filetobytes, null, Connectionstring);

                            }
                            // store the origintaor reference no in separate table...
                            db.InsertorUpdateRefNoHistroy(Guid.NewGuid(), new Guid(ActualDocumentUID), OriginatorRefNo, "");

                            DocUID += ActualDocumentUID + ",";

                            // added on 01/09/2022 for Contractor Correspondence attachments
                            #region STP Correspondence code
                           
                                DataSet dscheck = db.getTop1_DocumentStatusSelect(new Guid(ActualDocumentUID));
                                Guid StatusUID = new Guid(ActualDocumentUID);
                                if (dscheck.Tables[0].Rows.Count > 0)
                                {
                                    StatusUID = new Guid(dscheck.Tables[0].Rows[0]["StatusID"].ToString());
                                }
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];
                                if (httpPostedFile != null)
                                {
                                    attachmentUID = Guid.NewGuid().ToString();
                                    string ExtnA = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                    if (ExtnA.ToLower() != ".exe" && ExtnA.ToLower() != ".msi" && ExtnA.ToLower() != ".db")
                                    {

                                        if (!httpPostedFile.FileName.StartsWith("Cover") && httpPostedFile.FileName != "")
                                        {

                                            sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + pExists + "//" + wExists + "//Attachments";

                                            if (!Directory.Exists(sDocumentPath))
                                            {
                                                Directory.CreateDirectory(sDocumentPath);
                                            }


                                            int RandomNo = db.GenerateRandomNo();
                                            string sFileNameatttch = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);

                                            string savedPath = sDocumentPath + "/" + RandomNo + sFileNameatttch;
                                            httpPostedFile.SaveAs(savedPath);

                                            CoverPagePath = sDocumentPath + "/" + sFileNameatttch + "_"  + RandomNo  +  "_1" + ExtnA;
                                            EncryptFile(savedPath, CoverPagePath);
                                            //
                                            //for blob storage attachments
                                            byte[] filetobytesAttach = db.FileToByteArray(CoverPagePath);
                                            //
                                            CoverPagePath = "~/" + pExists + "/" + wExists + "/Attachments" + "/" + sFileNameatttch + "_" + RandomNo + "_1" + ExtnA;
                                            //
                                            AttatchUID += attachmentUID + ",";
                                            cnt = db.InsertDocumentsAttachments(new Guid(attachmentUID), new Guid(ActualDocumentUID), StatusUID, sFileNameatttch, CoverPagePath, new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), DateTime.Now);

                                            if (cnt <= 0)
                                            {
                                                // Page.ClientScript.RegisterStartupScript(Page.GetType(), "Error", "<script language='javascript'>alert('Error:11,There is some problem while inserting attachment. Please contact administrator');</script>");
                                            }
                                            db.InsertDocumentsAttachmentsBlob(new Guid(attachmentUID), new Guid(ActualDocumentUID), StatusUID, filetobytesAttach, Connectionstring);
                                        }
                                    }
                                }
                            }
                            
                            #endregion
                        }
                    }
                   
                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address.";
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            else
            {
                //Added code by Venkat on 13 May 2022
                //if (DocUID == "")
                //{
                //    return Json(new
                //    {
                //        Status = "Failure",
                //        Message = "No Document was Attached."
                //    });
                //}
                //else
                if (Flowtype == "STP-C")
                {
                    if (string.IsNullOrEmpty(AttatchUID))
                    {
                        return Json(new
                        {
                            Status = "Success",
                            ActualDocumentUID = DocUID.TrimEnd(','),
                            Message = "Documents Uploaded Successfully."
                        });
                    }
                    else
                    {
                        // AttachmentUID = AttatchUID.TrimEnd(','),
                        return Json(new
                        {
                            Status = "Success",
                            ActualDocumentUID = DocUID.TrimEnd(','),
                            Message = "Documents Uploaded Successfully."
                        });
                    }
                }
                    else if (!string.IsNullOrEmpty(CoverUID) && DocUID == "")
                {
                    return Json(new
                    {
                        Status = "Success",
                        CoverLetterUID = CoverUID,
                        Message = "Documents Uploaded Successfully."
                    });
                }
                else if (!string.IsNullOrEmpty(CoverUID) && DocUID != "")
                {
                    return Json(new
                    {
                        Status = "Success",
                        CoverLetterUID = CoverUID,
                        ActualDocumentUID = DocUID.TrimEnd(','),
                        Message = "Documents Uploaded Successfully."
                    });
                }
                else if (DocUID != "")
                {
                    return Json(new
                    {
                        Status = "Success",
                        ActualDocumentUID = DocUID.TrimEnd(','),
                        Message = "Documents Uploaded Successfully."
                    });
                }
                else 
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "No Document was Attached."
                    });
                }

            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/ExtoIntegration/ReSubmitDocument")]
        public IHttpActionResult ReSubmitDocument()
        {
            bool sError = false;
            string ErrorText = "";
            string CoverUID = string.Empty;
            string DocumentUid = string.Empty;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var ProjectName = httpRequest.Params["ProjectName"];
                    var WorkpackageName = httpRequest.Params["WorkpackageName"];
                    var SubmittalUID = httpRequest.Params["SubmittalUID"];
                     DocumentUid = httpRequest.Params["DocumentUid"];
                   // var CoverLetterDate = httpRequest.Params["CoverLetterDate"];
                    var CoverLetterFile = httpRequest.Params["CoverLetterFile"];
                    var DocumentFile = httpRequest.Params["DocumentFile"];
                    var Comments = httpRequest.Params["Comments"];
                
                    bool CoverLetterUpload = false, DocumentsUpload = false;
                    var wExists = string.Empty;
                    var pExists = db.ProjectNameExists(ProjectName);
                    string ActualDocumentUID = string.Empty;

                    if (!string.IsNullOrEmpty(pExists))
                    {
                        if(WorkpackageName.ToLower() == "somapura")
                        {
                            WorkpackageName = "V-Valley";
                        }
                        wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                        if (!string.IsNullOrEmpty(wExists))
                        {
                            if (Guid.TryParse(SubmittalUID, out Guid submittaluid))
                            {
                              
                                if (httpRequest.Files.Count == 0)
                                {
                                    sError = true;
                                    ErrorText = "Please upload a document.";
                                }
                                                
                                for (int i = 0; i < httpRequest.Files.Count; i++)
                                {
                                    HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                    if (httpPostedFile.FileName != null && httpPostedFile.FileName != string.Empty)
                                    {
                                        if (httpPostedFile.FileName.StartsWith("Cover") || httpPostedFile.FileName.StartsWith("Cover_Letter") || httpPostedFile.FileName.StartsWith("Cover Letter"))
                                        {
                                            CoverLetterUpload = true;
                                        }
                                        else
                                        {
                                            DocumentsUpload = true;
                                        }
                                    }
                                    else
                                    {
                                            sError = true;
                                            ErrorText = "Please upload a document.";
                                        
                                    }
                                }
                                //if (!CoverLetterUpload)
                                //{
                                //    sError = true;
                                //    ErrorText = "Please upload a cover letter file.";
                                //}
                                if (!DocumentsUpload)
                                {
                                    sError = true;
                                    ErrorText = "Please upload document file.";
                                }   
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Submittal UID.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Workpackage name not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Project name not found.";
                    }
                    DataTable  dtWorkpackages=  db.GetWorkPackages_ProjectName(ProjectName);
                    if(dtWorkpackages.Rows.Count==0)
                    {
                        sError = true;
                        ErrorText = "Workpackage not available for this project";
                    }
                    if(dtWorkpackages.Rows[0]["Name"].ToString() != WorkpackageName)
                    {
                        sError = true;
                        ErrorText = "Workpackage not matching with project";
                    }

                    DataTable dsDocuments=  db.getActualDocumentStatus(new Guid(dtWorkpackages.Rows[0]["ProjectUid"].ToString()), new Guid(dtWorkpackages.Rows[0]["WorkpackageUid"].ToString()), new Guid(DocumentUid));
                    if(dsDocuments.Rows.Count==0)
                    {
                        sError = true;
                        ErrorText = "Invalid documentuid";
                    }
                    DataSet dsdocs = db.getDocumentsbyDocID(new Guid(SubmittalUID));
                    if (dsdocs.Tables[0].Rows.Count ==0)
                    {
                        sError = true;
                        ErrorText = "Invalid Submitaluid";
                    }

                    string Extn = "";
                   
                    if (!sError)
                    {
                        byte[] coverfiletobytes = null;
                        byte[] docVersionfiletobytes = null;
                        DataSet ds = db.getDocumentsbyDocID(new Guid(SubmittalUID));
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string  sDocumentPath = string.Empty,
                               CoverPagePath = string.Empty , documentPagePath=string.Empty;


                            //Cover Letter Insert
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];
                                if (httpPostedFile != null)
                                {
                                    Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                    if (Extn.ToLower() != ".exe" && Extn.ToLower() != ".msi" && Extn.ToLower() != ".db")
                                    {
                                        if ((httpPostedFile.FileName.StartsWith("Cover") || httpPostedFile.FileName.StartsWith("Cover_Letter")))
                                        {
                                            //DocumentFor = "Cover Letter";
                                            
                                            sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + pExists + "//" + wExists + "//CoverLetter";

                                            if (!Directory.Exists(sDocumentPath))
                                            {
                                                Directory.CreateDirectory(sDocumentPath);
                                            }
                                            string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                            httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_1_copy" + Extn);
                                            string savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
                                            CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;
                                            EncryptFile(sDocumentPath + "/" + sFileName + "_1_copy" + Extn, sDocumentPath + "/" + sFileName + "_1" + Extn);

                                            //for blob
                                            coverfiletobytes = db.FileToByteArray(CoverPagePath);
                                            //
                                            savedPath = "~/" + pExists + "/" + wExists + "/CoverLetter" + "/" + sFileName + "_1_copy" + Extn;
                                            CoverPagePath = "~/" + pExists + "/" + wExists + "/CoverLetter" + "/" + sFileName + "_1" + Extn;
                                            // db.EncryptFile(sDocumentPath + "/" + sFileName + "_1_copy" + Extn, sDocumentPath + "/" + sFileName + "_1" + Extn);

                                        }

                                        if (!httpPostedFile.FileName.StartsWith("Cover"))
                                        {
                                            
                                            sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + pExists + "//" + wExists + "//Documents";

                                            if (!Directory.Exists(sDocumentPath))
                                            {
                                                Directory.CreateDirectory(sDocumentPath);
                                            }

                                            string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);

                                            string savedPath = string.Empty;

                                            savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
                                            httpPostedFile.SaveAs(savedPath);

                                            documentPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;
                                            EncryptFile(sDocumentPath + "/" + sFileName + "_1_copy" + Extn, sDocumentPath + "/" + sFileName + "_1" + Extn);
                                            //for blob
                                            docVersionfiletobytes = db.FileToByteArray(documentPagePath);
                                            //
                                            // ActualDocumentUID = Guid.NewGuid().ToString();
                                            string UploadFilePhysicalpath = documentPagePath;

                                            savedPath = "~/" + pExists + "/" + wExists + "/Documents" + "/" + sFileName + "_1_copy" + Extn;
                                            documentPagePath = "~/" + pExists + "/" + wExists + "/Documents" + "/" + sFileName + "_1" + Extn;
                                            //db.EncryptFile(savedPath, sDocumentPath + "/" + sFileName + "_1" + Extn);
                                        }
                                    }
                                }
                            }
                            DataSet dsdocStatus= db.getActualDocumentStatusList(new Guid(DocumentUid));
                            if(dsdocStatus.Tables[0].Rows.Count==0)
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:document Status is not available" 
                                });
                            }

                            //added on 08/08/2022 by zuber for exto work A,Works B and Vendor Approval
                            string FlowName = db.GetFlowName_by_SubmittalID(new Guid(SubmittalUID));
                            string FlowUID = db.GetFlowUIDBySubmittalUID(new Guid(SubmittalUID));
                            string currentstatus = dsdocStatus.Tables[0].Rows[dsdocStatus.Tables[0].Rows.Count - 1]["ActivityType"].ToString();

                            DateTime CDate1 = DateTime.Now;
                            //
                            string sDate1 = DateTime.Now.ToString("MM/dd/yyyy");
                            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            sDate1 = db.ConvertDateFormat(sDate1);
                            Guid StatusUID = new Guid(dsdocStatus.Tables[0].Rows[dsdocStatus.Tables[0].Rows.Count - 1]["StatusUID"].ToString());
                            CDate1 = Convert.ToDateTime(sDate1);
                            if (FlowName == "Works A" || FlowName == "Works B" || FlowName == "Vendor Approval")
                            {
                                var ISpresent = db.GetStatusForReconciliation(new Guid(FlowUID)).Tables[0].AsEnumerable().Where(r =>
                                                            r.Field<string>("Current_Status") == currentstatus).FirstOrDefault();
                                if(ISpresent != null)
                                {
                                    StatusUID = Guid.NewGuid();
                                    int Cnt = db.InsertorUpdateDocumentStatus(StatusUID, new Guid(DocumentUid), 1, "Reconciliation", 0, CDate1, "",
                            new Guid("419D71BD-FDD4-4A92-898C-A044BFA7803D"), Comments, "Reconciliation", "", CDate1, "", "NotAll", "Contractor");
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Status = "Failure",
                                        Message = "Error:Resubmission not allowed ! Current Status is not for resubmission !"
                                    });
                                }
                            }
                            Guid DocVersion_UID = Guid.NewGuid();
                            int DocVersion = db.getDocumentStatusVersion(new Guid(dsdocStatus.Tables[0].Rows[dsdocStatus.Tables[0].Rows.Count-1]["StatusUID"].ToString()), new Guid(DocumentUid));
                            int cnt = db.InsertDocumentVersion(DocVersion_UID, StatusUID, new Guid(DocumentUid), Extn, documentPagePath, Comments, CoverPagePath);
                            if (cnt > 0)
                            {
                                //DataSet dsUsers = db.getAllUsers();
                                //if (ds.Tables[0].Rows.Count > 0)
                                //{
                                //    string CC = string.Empty;
                                //    for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                                //    {
                                //        CC += ds.Tables[0].Rows[i]["EmailID"].ToString() + ",";
                                //    }
                                //    CC = CC.TrimEnd(',');
                                //    string sHtmlString = string.Empty;
                                //    sHtmlString = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" + "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                                //            "<head>" + "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" + "</head>" +
                                //                "<body style='font-family:Verdana, Arial, sans-serif; font-size:12px; font-style:normal;'>";
                                //    sHtmlString += "<div style='float:left; width:100%; height:30px;'>" +
                                //                        "Dear, " + "Users" +
                                //                        "<br/><br/></div>";
                                //    sHtmlString += "<div style='width:100%; float:left;'><div style='width:100%; float:left;'>Below are the Status details. <br/><br/></div>";
                                //    sHtmlString += "<div style='width:100%; float:left;'><div style='width:100%; float:left;'>Document Name : " + db.getDocumentName_by_DocumentUID(new Guid(dsDocuments.Rows[0]["DocumentUID"].ToString())) + "<br/></div>";
                                //    sHtmlString += "<div style='width:100%; float:left;'><div style='width:100%; float:left;'>Version : " + DocVersion + "<br/></div>";
                                //    sHtmlString += "<div style='width:100%; float:left;'><div style='width:100%; float:left;'>Status : " + db.getDocumentStatus_by_StatusUID(new Guid(dsdocStatus.Tables[0].Rows[0]["StatusUID"].ToString())) + "<br/></div>";
                                //    sHtmlString += "<div style='width:100%; float:left;'><div style='width:100%; float:left;'>Document Type : " + Extn + "<br/></div>";
                                //    sHtmlString += "<div style='width:100%; float:left;'><div style='width:100%; float:left;'>Date : " + DateTime.Now.ToShortDateString() + "<br/></div>";
                                //    sHtmlString += "<div style='width:100%; float:left;'><div style='width:100%; float:left;'>Comments : " + Comments + "<br/></div>";
                                //    sHtmlString += "<div style='width:100%; float:left;'><br/><br/>Sincerely, <br/> Project Manager Tool.</div></div></body></html>";
                                //    //string ret = db.SendMail(ds.Tables[0].Rows[0]["EmailID"].ToString(), Subject, sHtmlString, CC, Server.MapPath(DocPath));
                                //}   

                                //for blob
                                string Connectionstring = db.getProjectConnectionString(new Guid(pExists));
                                db.InsertDocumentVersionBlob(DocVersion_UID, new Guid(DocumentUid), docVersionfiletobytes, coverfiletobytes, Connectionstring);
                                //
                            }


                        }
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address.";
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(CoverUID))
                {
                    return Json(new
                    {
                        Status = "Success",
                        CoverLetterUID = CoverUID,
                        ActualDocumentUID = DocumentUid.TrimEnd(','),
                        Message = "Documents Uploaded Successfully."
                    });
                }
                else
                {
                    return Json(new
                    {
                        Status = "Success",
                        ActualDocumentUID = DocumentUid.TrimEnd(','),
                        Message = "Documents Uploaded Successfully."
                    });
                }

            }
        }

        [Authorize]
        [HttpPost] // this is for adding submittals
        [Route("api/ExtoIntegration/GetUsersforAddSubmittal")]
         // for getting Users for Add Submittal
        public IHttpActionResult GetUsersforAddSubmittal()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                DataSet ds = new DataSet();
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) == 0)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Not Authorized IP address"
                    });
                }
                else
                {
                    var ProjectName = httpRequest.Params["ProjectName"];
                    

                    var pExists = db.ProjectNameExists(ProjectName);
                    if (pExists != "")
                    {
                        ds = db.GetUsers_under_ProjectUID(new Guid(pExists));

                        return Json(new
                        {
                            Status = "Success",
                            Users = ds.Tables[0]
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Project Name does not Exists !"
                        });
                    }
                }
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

       



        
        [Authorize]
        [HttpPost]
        [Route("api/ExtoIntegration/GetDocumentStatus")]
        public IHttpActionResult GetDocumentStatus()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                DataSet dtLastestStatus = new DataSet();
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "documentUid=" + httpRequest.Params["document"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
                //for blob
                string projectuid = string.Empty;
                
                //
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var documentUid = httpRequest.Params["document"];
                    dtLastestStatus = db.getLatesDocumentStatus(new Guid(documentUid));
                    if (dtLastestStatus.Tables[0].Rows.Count == 0)
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "No data available"
                        });
                    }
                    //for blob
                    DataSet ds = db.ActualDocuments_SelectBy_ActualDocumentUID(new Guid(documentUid));
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        projectuid = ds.Tables[0].Rows[0]["ProjectUID"].ToString();
                    }
                    //
                    string base64 = "";
                    string getExtension = "";
                    string fileName = "";
                    //added on 21/09/2022
                    string FlowType = db.GetFlowTypeBySubmittalUID(new Guid(db.GetSubmittalUID_By_ActualDocumentUID(new Guid(documentUid))));
                    string Connectionstring = db.getProjectConnectionString(new Guid(projectuid));
                    if (!string.IsNullOrEmpty(dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString()))
                    {
                      

                        string path = ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString();
                        string fileExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString());
                        string outPath = ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString().Replace(fileExtension, "") + "_download" + fileExtension;

                        //for blob
                        byte[] Blobbytes=null;
                        DataSet docBlob = db.GetDocumentStatusBlob_by_StatusUID(new Guid(dtLastestStatus.Tables[0].Rows[0]["StatusUID"].ToString()), Connectionstring);
                        if (docBlob.Tables[0].Rows.Count > 0)
                        {
                            Blobbytes = (byte[])docBlob.Tables[0].Rows[0]["ReviewFileBlob_Data"];

                            BinaryWriter Writer = null;
                            Writer = new BinaryWriter(File.OpenWrite(path));

                            // Writer raw data                
                            Writer.Write(Blobbytes);
                            Writer.Flush();
                            Writer.Close();
                        }


                        db.DecryptFile(path, outPath);

                        Byte[] bytes = File.ReadAllBytes(outPath);
                        base64 = Convert.ToBase64String(bytes);

                        getExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString());
                        fileName = System.IO.Path.GetFileName(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString());
                    }
                    if (FlowType == "STP-C")
                    {
                        if (dtLastestStatus.Tables[0].Rows[0]["Status"].ToString().ToLower().Contains("reply to"))
                        {
                            //make cover letter file data
                            string path = string.Empty;
                            string fileExtension = string.Empty;
                            string outPath = string.Empty;
                            Byte[] bytes;
                            if (!string.IsNullOrEmpty(dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString()))
                            {
                                 path = ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2);
                                fileExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2));
                               outPath = ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2).Replace(fileExtension, "") + "_download" + fileExtension;

                                //for blob
                                byte[] Blobbytes = null;
                                DataSet docBlob = db.GetDocumentStatusBlob_by_StatusUID(new Guid(dtLastestStatus.Tables[0].Rows[0]["StatusUID"].ToString()), Connectionstring);
                                if (docBlob.Tables[0].Rows.Count > 0)
                                {
                                    Blobbytes = (byte[])docBlob.Tables[0].Rows[0]["CoverFileBlob_Data"];

                                    BinaryWriter Writer = null;
                                    Writer = new BinaryWriter(File.OpenWrite(path));

                                    // Writer raw data                
                                    Writer.Write(Blobbytes);
                                    Writer.Flush();
                                    Writer.Close();
                                }
                                //
                                db.DecryptFile(path, outPath);

                                bytes = File.ReadAllBytes(outPath);
                                base64 = Convert.ToBase64String(bytes);

                                getExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2));
                                fileName = System.IO.Path.GetFileNameWithoutExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2));
                            }
                            List<LetterAttachmentsModel> attachemnetlist = new List<LetterAttachmentsModel>();
                            DataSet attachmentdata = db.GetDocumentAttachments(new Guid(dtLastestStatus.Tables[0].Rows[0]["StatusUID"].ToString()));
                            foreach (DataRow dr in attachmentdata.Tables[0].Rows)
                            {
                                string pathA = ConfigurationManager.AppSettings["DocumentsPath"] + dr["AttachmentFile"].ToString().Remove(0, 2);
                                string fileExtensionA = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dr["AttachmentFile"].ToString().Remove(0, 2));
                                string outPathA = ConfigurationManager.AppSettings["DocumentsPath"] + dr["AttachmentFile"].ToString().Remove(0, 2).Replace(fileExtension, "") + "_download" + fileExtension;
                                string fileNameA = dr["AttachmentFileName"].ToString();

                                //for blob
                                byte[] Blobbytes = null;
                                
                               
                                DataSet docBlob = db.GetAttachmentBlob_by_attachmentUID(new Guid(dr["AttachmentUID"].ToString()), Connectionstring);
                                if (docBlob.Tables[0].Rows.Count > 0)
                                {
                                    Blobbytes = (byte[])docBlob.Tables[0].Rows[0]["BlobData"];

                                    BinaryWriter Writer = null;
                                    Writer = new BinaryWriter(File.OpenWrite(pathA));

                                    // Writer raw data                
                                    Writer.Write(Blobbytes);
                                    Writer.Flush();
                                    Writer.Close();
                                }

                                String converted_file = "";

                                if (File.Exists(pathA))
                                {
                                    db.DecryptFile(pathA, outPathA);
                                    bytes = File.ReadAllBytes(outPathA);
                                    converted_file = Convert.ToBase64String(bytes);
                                }

                                LetterAttachmentsModel attachments = new LetterAttachmentsModel()
                                {
                                    AttachedFileName = fileNameA,
                                    AttachedFileType = fileExtensionA,
                                    FileAsBase64 = converted_file
                                };

                                attachemnetlist.Add(attachments);
                            }
                            return Json(new
                            {
                                status = "Success",
                                documentstatus = dtLastestStatus.Tables[0].Rows[0]["Status"].ToString(),
                                statusupadteddate = Convert.ToDateTime(dtLastestStatus.Tables[0].Rows[0]["statusUpdatedDate"].ToString()).ToString("dd/MM/yyyy"),
                                remarks = dtLastestStatus.Tables[0].Rows[0]["remarks"].ToString(),
                                fileContent = base64,
                                fileName = fileName,
                                fileType = getExtension,
                                Attachments = attachemnetlist
                            });
                        }
                        else if (dtLastestStatus.Tables[0].Rows[0]["Status"].ToString().ToLower().Contains("rejected"))
                        {
                            return Json(new
                            {
                                status = "Success",
                                documentstatus = dtLastestStatus.Tables[0].Rows[0]["Status"].ToString(),
                                statusupadteddate = Convert.ToDateTime(dtLastestStatus.Tables[0].Rows[0]["statusUpdatedDate"].ToString()).ToString("dd/MM/yyyy"),
                                remarks = dtLastestStatus.Tables[0].Rows[0]["remarks"].ToString(),
                                fileContent = base64,
                                fileName = fileName,
                                fileType = getExtension
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                status = "Success",
                                documentstatus = "Being Processed",
                                statusupadteddate = Convert.ToDateTime(dtLastestStatus.Tables[0].Rows[0]["statusUpdatedDate"].ToString()).ToString("dd/MM/yyyy"),
                                remarks = dtLastestStatus.Tables[0].Rows[0]["remarks"].ToString(),
                                fileContent = base64,
                                fileName = fileName,
                                fileType = getExtension
                            });
                        }
                    }
                    else if (FlowType == "STP-OB")
                    {
                        //make cover letter file data
                        string path = string.Empty;
                        string fileExtension = string.Empty;
                        string outPath = string.Empty;
                        Byte[] bytes;
                        if (!string.IsNullOrEmpty(dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString()))
                        {
                            path = ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2);
                            fileExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2));
                            outPath = ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2).Replace(fileExtension, "") + "_download" + fileExtension;

                            //for blob
                            byte[] Blobbytes = null;
                            DataSet docBlob = db.GetDocumentStatusBlob_by_StatusUID(new Guid(dtLastestStatus.Tables[0].Rows[0]["StatusUID"].ToString()), Connectionstring);
                            if (docBlob.Tables[0].Rows.Count > 0)
                            {
                                Blobbytes = (byte[])docBlob.Tables[0].Rows[0]["CoverFileBlob_Data"];

                                BinaryWriter Writer = null;
                                Writer = new BinaryWriter(File.OpenWrite(path));

                                // Writer raw data                
                                Writer.Write(Blobbytes);
                                Writer.Flush();
                                Writer.Close();
                            }
                            //

                            db.DecryptFile(path, outPath);

                            bytes = File.ReadAllBytes(outPath);
                            base64 = Convert.ToBase64String(bytes);


                            getExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2));
                            fileName = System.IO.Path.GetFileNameWithoutExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dtLastestStatus.Tables[0].Rows[0]["CoverLetterFile"].ToString().Remove(0, 2));
                        }
                        List<LetterAttachmentsModel> attachemnetlist = new List<LetterAttachmentsModel>();
                        DataSet attachmentdata = db.GetDocumentAttachments(new Guid(dtLastestStatus.Tables[0].Rows[0]["StatusUID"].ToString()));
                        foreach (DataRow dr in attachmentdata.Tables[0].Rows)
                        {
                            string pathA = ConfigurationManager.AppSettings["DocumentsPath"] + dr["AttachmentFile"].ToString().Remove(0, 2);
                            string fileExtensionA = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dr["AttachmentFile"].ToString().Remove(0, 2));
                            string outPathA = ConfigurationManager.AppSettings["DocumentsPath"] + dr["AttachmentFile"].ToString().Remove(0, 2).Replace(fileExtension, "") + "_download" + fileExtension;
                            string fileNameA = dr["AttachmentFileName"].ToString();

                            //for blob
                            byte[] Blobbytes = null;


                            DataSet docBlob = db.GetAttachmentBlob_by_attachmentUID(new Guid(dr["AttachmentUID"].ToString()), Connectionstring);
                            if (docBlob.Tables[0].Rows.Count > 0)
                            {
                                Blobbytes = (byte[])docBlob.Tables[0].Rows[0]["BlobData"];

                                BinaryWriter Writer = null;
                                Writer = new BinaryWriter(File.OpenWrite(pathA));

                                // Writer raw data                
                                Writer.Write(Blobbytes);
                                Writer.Flush();
                                Writer.Close();
                            }
                            //

                            String converted_file = "";

                            if (File.Exists(pathA))
                            {
                                db.DecryptFile(pathA, outPathA);
                                bytes = File.ReadAllBytes(outPathA);
                                converted_file = Convert.ToBase64String(bytes);
                            }

                            LetterAttachmentsModel attachments = new LetterAttachmentsModel()
                            {
                                AttachedFileName = fileNameA,
                                AttachedFileType = fileExtensionA,
                                FileAsBase64 = converted_file
                            };

                            attachemnetlist.Add(attachments);
                        }

                        if (dtLastestStatus.Tables[0].Rows[0]["Status"].ToString() == "Reply to EE by Contractor" || dtLastestStatus.Tables[0].Rows[0]["Status"].ToString() == "Reply to ACE by Contractor" || dtLastestStatus.Tables[0].Rows[0]["Status"].ToString() == "Reply to CE by Contractor")
                        {
                            return Json(new
                            {
                                status = "Success",
                                documentstatus = dtLastestStatus.Tables[0].Rows[0]["Status"].ToString(),
                                statusupadteddate = Convert.ToDateTime(dtLastestStatus.Tables[0].Rows[0]["statusUpdatedDate"].ToString()).ToString("dd/MM/yyyy"),
                                remarks = dtLastestStatus.Tables[0].Rows[0]["remarks"].ToString()
                               
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                status = "Success",
                                documentstatus = dtLastestStatus.Tables[0].Rows[0]["Status"].ToString(),
                                statusupadteddate = Convert.ToDateTime(dtLastestStatus.Tables[0].Rows[0]["statusUpdatedDate"].ToString()).ToString("dd/MM/yyyy"),
                                remarks = dtLastestStatus.Tables[0].Rows[0]["remarks"].ToString(),
                                fileContent = base64,
                                fileName = fileName,
                                fileType = getExtension,
                                Attachments = attachemnetlist
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            status = "Success",
                            documentstatus = dtLastestStatus.Tables[0].Rows[0]["Status"].ToString(),
                            statusupadteddate = Convert.ToDateTime(dtLastestStatus.Tables[0].Rows[0]["statusUpdatedDate"].ToString()).ToString("dd/MM/yyyy"),
                            remarks = dtLastestStatus.Tables[0].Rows[0]["remarks"].ToString(),
                            fileContent = base64,
                            fileName = fileName,
                            fileType = getExtension
                        });
                    }

                }
                else
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Not Authorized IP address"
                    });
                }




            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error" + ex.Message
                });
            }

        }


        // added for Venkat on 06 August 2022
        [Authorize]
        [HttpPost]
        [Route("api/ExtoIntegration/GetDocumentStatusComments")]
        public IHttpActionResult GetDocumentStatusComments()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                DataSet dtLastestStatus = new DataSet();
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "documentUid=" + httpRequest.Params["document"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var documentUid = httpRequest.Params["document"];
                    dtLastestStatus = db.ActualDocuments_SelectBy_ActualDocumentUID(new Guid(documentUid));
                    if (dtLastestStatus.Tables[0].Rows.Count == 0)
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "No data available"
                        });
                    }
                    DataSet documentSTatusList = db.getActualDocumentStatusList(new Guid(documentUid));
                    string SubmittalUID = db.GetSubmittalUID_By_ActualDocumentUID(new Guid(documentUid));
                    if (SubmittalUID == null || SubmittalUID =="")
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "No submittal exists for documentuid"
                        });
                    }
                    string Flowtype = db.GetFlowTypeBySubmittalUID(new Guid(SubmittalUID));
                    if (Flowtype == "STP")
                    {
                        DataTable dtNewTable = new DataTable();
                        dtNewTable.Columns.Add("Sl");
                        dtNewTable.Columns.Add("StatusUID", typeof(Guid));
                        dtNewTable.Columns.Add("ActivityType");
                        dtNewTable.Columns.Add("Phase");
                        var phaseAndStatus = db.GetPhaseAndCurrentStatusByDocument(new Guid(documentUid));
                        int counter = 0;
                        foreach (DataRow dr in documentSTatusList.Tables[0].Rows)
                        {
                            var statusUID = dr["StatusUID"].ToString();
                            string phaseForCurrentSTatus = string.Empty;
                            var phase = phaseAndStatus.AsEnumerable().Where(r => r.Field<string>("Current_Status") == dr["ActivityType"].ToString()).FirstOrDefault();
                            if (phase != null)
                            {
                                phaseForCurrentSTatus = phase[1].ToString();
                            }
                            dtNewTable.Rows.Add(counter.ToString(), new Guid(dr["StatusUID"].ToString()), dr["ActivityType"].ToString(), phaseForCurrentSTatus);
                            counter++;
                        }

                        List<Guid> StatusUIDToRemove = new List<Guid>();
                        DataTable dtNewIntermediate = dtNewTable.Copy();
                        //First remove all the empty phases
                        var EmptyPhases = dtNewTable.AsEnumerable().Where(r => r.Field<string>("Phase") == string.Empty);
                        if (EmptyPhases.Any())
                        {
                            foreach (var eachEmptyPhase in EmptyPhases)
                            {
                                string activity = eachEmptyPhase["ActivityType"].ToString();
                                if (activity.ToLower().Contains("client ce") || activity.ToLower().Contains("code a-ce approval"))
                                {

                                }
                                else
                                {
                                    StatusUIDToRemove.Add(new Guid(eachEmptyPhase["StatusUID"].ToString()));
                                }
                            }
                            dtNewIntermediate.AsEnumerable().Where(x => StatusUIDToRemove.Contains(x.Field<Guid>("StatusUID"))).ToList().ForEach(r => r.Delete());
                        }

                        var groupByPhase = dtNewTable.AsEnumerable().GroupBy(r => r.Field<string>("Phase").ToLower()).Select(r => new { r.Key, r });
                        foreach (var eachPhase in groupByPhase)
                        {
                            var eachItem = eachPhase.r.ToList();
                            if (eachItem.Count > 1)
                            {
                                if (eachPhase.Key != "reconciliation")

                                {
                                    for (int count = 0; count < eachItem.Count - 1; count++)
                                    {
                                        StatusUIDToRemove.Add(new Guid(eachItem[count]["StatusUID"].ToString()));
                                    }
                                }
                            }
                        }
                        if (StatusUIDToRemove.Count > 0)
                        {
                            dtNewIntermediate.AsEnumerable().Where(x => StatusUIDToRemove.Contains(x.Field<Guid>("StatusUID"))).ToList().ForEach(r => r.Delete());

                            string statusIDRemove = RemovePhaseFromDataTable("Approved", "Under Client Approval Process", dtNewIntermediate);
                            if (!string.IsNullOrEmpty(statusIDRemove))
                                StatusUIDToRemove.Add(new Guid(statusIDRemove));

                            statusIDRemove = RemovePhaseFromDataTable("Network Design DTL Reviewed", "Network Design by ONTB", dtNewIntermediate);
                            if (!string.IsNullOrEmpty(statusIDRemove))
                                StatusUIDToRemove.Add(new Guid(statusIDRemove));

                            statusIDRemove = RemovePhaseFromDataTable("ONTB DTL Approved", "Review by ONTB", dtNewIntermediate);
                            if (!string.IsNullOrEmpty(statusIDRemove))
                                StatusUIDToRemove.Add(new Guid(statusIDRemove));

                            statusIDRemove = RemovePhaseFromDataTable("ONTB DTL Approved", "ONTB Specialist Verified", dtNewIntermediate);
                            if (!string.IsNullOrEmpty(statusIDRemove))
                                StatusUIDToRemove.Add(new Guid(statusIDRemove));

                            statusIDRemove = RemovePhaseFromDataTable("Approved by ONTB", "Review by ONTB", dtNewIntermediate);
                            if (!string.IsNullOrEmpty(statusIDRemove))
                                StatusUIDToRemove.Add(new Guid(statusIDRemove));




                            dtNewIntermediate.AsEnumerable().Where(x => StatusUIDToRemove.Contains(x.Field<Guid>("StatusUID"))).ToList().ForEach(r => r.Delete());

                            documentSTatusList.Tables[0].AsEnumerable().Where(x => StatusUIDToRemove.Contains(x.Field<Guid>("StatusUID"))).ToList().ForEach(r => r.Delete());
                            documentSTatusList.AcceptChanges();

                        }

                    }
                    var sortedTable = documentSTatusList.Tables[0].AsEnumerable().OrderByDescending(r => r.Field<DateTime>("CreatedDate")).FirstOrDefault();
                   
                    string FlowName = db.GetFlowName_by_SubmittalID(new Guid(SubmittalUID));
                    string current_Status = sortedTable["Current_Status"].ToString();
                    string phases = db.GetPhaseforStatus(new Guid(db.GetFlowUIDBySubmittalUID(new Guid(SubmittalUID))), current_Status);
                    string status_comments = sortedTable["Status_comments"].ToString();


                        if (current_Status == "Code A-CE Approval")
                        {
                        phases = "Approved By BWSSB Under Code A";

                        }
                        else if (current_Status == "Code B-CE Approval")
                        {
                        phases = "Approved By BWSSB Under Code B";
                        }
                        else if (current_Status == "Code C-CE Approval")
                        {
                        phases = "Under Client Approval Process";

                        }
                        else if (current_Status == "Client CE GFC Approval")
                        {
                        phases = "Approved GFC by BWSSB";
                        }
                        //added on 19/07/2022 for saladins changes for contractor showing consolidated comments
                        if (phases.Contains("DTL"))
                        {
                           status_comments = "DTL added - " + status_comments + "<br/><br/>---------------------<br/>PMC Comments <br/>";
                            if (FlowName == "Works A")
                            {
                                DataSet dtlcomments = db.GetConsolidatedPMCComments(new Guid(documentUid), FlowName);
                                string[] stringSeparators = new string[] { "<br/>" };

                                foreach (DataRow dr in dtlcomments.Tables[0].Rows)
                                {
                                   status_comments += dr["Status_Comments"].ToString().Split(stringSeparators, StringSplitOptions.None)[0] + "<br/><br/>";
                                }
                            }
                            else if (FlowName == "Works B")
                            {
                                DataSet dtlcomments = db.GetConsolidatedPMCComments(new Guid(documentUid), FlowName);
                                string[] stringSeparators = new string[] { "<br/>" };

                                foreach (DataRow dr in dtlcomments.Tables[0].Rows)
                                {
                                  status_comments += dr["Status_Comments"].ToString().Split(stringSeparators, StringSplitOptions.None)[0] + "<br/><br/>";
                                }
                            }
                            else if (FlowName == "Vendor Approval")
                            {
                                DataSet dtlcomments = db.GetConsolidatedPMCComments(new Guid(documentUid), FlowName);
                                string[] stringSeparators = new string[] { "<br/>" };

                                foreach (DataRow dr in dtlcomments.Tables[0].Rows)
                                {
                                   status_comments += dr["Status_Comments"].ToString().Split(stringSeparators, StringSplitOptions.None)[0] + "<br/><br/>";
                                }
                            }
                        }
                        else
                        {
                           status_comments = "";
                        }
                    
                    //   dtLastestStatus = db.getDocumentVersions_by_StatusUID(new Guid(sortedTable["StatusUid"].ToString()));
                    string base64 = "";
                    string getExtension = "";
                    string fileName = "";
                    if (!string.IsNullOrEmpty(sortedTable["LinktoReviewFile"].ToString()))
                    {
                        string path = ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + sortedTable["LinktoReviewFile"].ToString().TrimStart('~');
                        string fileExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + sortedTable["LinktoReviewFile"].ToString().TrimStart('~'));
                        string outPath = ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + sortedTable["LinktoReviewFile"].ToString().TrimStart('~').Replace(fileExtension, "") + "_download" + fileExtension;
                        db.DecryptFile(path, outPath);

                        Byte[] bytes = File.ReadAllBytes(outPath);
                        base64 = Convert.ToBase64String(bytes);

                        getExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + sortedTable["LinktoReviewFile"].ToString().TrimStart('~'));
                        fileName = System.IO.Path.GetFileName(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + sortedTable["LinktoReviewFile"].ToString().TrimStart('~'));
                    }
                    return Json(new
                    {
                        status = "Success",
                        documentstatus = phases,
                        remarks = status_comments.ToString(),
                        fileContent = base64,
                        fileName = fileName,
                        fileType = getExtension
                    });

                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Not Authorized IP address"
                    });
                }




            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "Failure",
                    Message = "Error" + ex.Message
                });
            }

        }

        private string RemovePhaseFromDataTable(string PhaseExist, string RemovePhase, DataTable dtNewIntermediate)
        {
            string StatusUIDToRemove = string.Empty;
            var isPhaseExist = dtNewIntermediate.AsEnumerable().Where(r => r.Field<string>("Phase").ToLower().Contains(PhaseExist.ToLower())).FirstOrDefault();
            if (isPhaseExist != null)
            {
                var isRemovePhaseExist = dtNewIntermediate.AsEnumerable().Where(r => r.Field<string>("Phase").ToLower() == RemovePhase.ToLower()).FirstOrDefault();
                if (isRemovePhaseExist != null)
                {
                    StatusUIDToRemove = isRemovePhaseExist["StatusUID"].ToString();
                }
            }
            return StatusUIDToRemove;
        }


        //added on 21/12/2022 for venkat
        
        [Authorize]
        [HttpPost]
        [Route("api/ExtoIntegration/GetOnlineflow2List")]
        public IHttpActionResult GetOnlineflow2List()
        {
            bool sError = false;
            string ErrorText = "";
            DataTable dtResponse = new DataTable();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var ProjectName = httpRequest.Params["projectname"];
                    var WorkpackageName = httpRequest.Params["workpackage"];
                    DataTable dtProject = db.GetWorkPackages_ProjectName(ProjectName);

                    if (dtProject.Rows.Count > 0)
                    {
                        dtResponse.Columns.Add("ActualDocumentUID");
                        dtResponse.Columns.Add("Description");
                        dtResponse.Columns.Add("ONTB Ref No");
                        dtResponse.Columns.Add("Originator Ref No");
                        dtResponse.Columns.Add("Submittal Name");
                        dtResponse.Columns.Add("Contractor Upload");

                        DataSet ds = db.GetFlow2Olddocs(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), "Pending Documents");
                        for (int cnt = 0; cnt < ds.Tables[0].Rows.Count; cnt++)
                        {
                            DataRow drRow = dtResponse.NewRow();
                            drRow["ActualDocumentUID"] = ds.Tables[0].Rows[cnt]["ActualdocumentUid"].ToString();
                            drRow["Description"] = ds.Tables[0].Rows[cnt]["Description"].ToString();
                            drRow["ONTB Ref No"] = ds.Tables[0].Rows[cnt]["ProjectRef_Number"].ToString();
                            drRow["Originator Ref No"] = ds.Tables[0].Rows[cnt]["Ref_Number"].ToString(); 
                             drRow["Submittal Name"] = ds.Tables[0].Rows[cnt]["DocName"].ToString();
                            drRow["Contractor Upload"] = "Pending";
                            dtResponse.Rows.Add(drRow);
                        }
                        ds = db.GetFlow2Olddocs(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), "Action Taken Documents");
                        for (int cnt = 0; cnt < ds.Tables[0].Rows.Count; cnt++)
                        {
                            DataRow drRow = dtResponse.NewRow();
                            drRow["ActualDocumentUID"] = ds.Tables[0].Rows[cnt]["ActualdocumentUid"].ToString();
                            drRow["Description"] = ds.Tables[0].Rows[cnt]["Description"].ToString();
                            drRow["ONTB Ref No"] = ds.Tables[0].Rows[cnt]["ProjectRef_Number"].ToString();
                            drRow["Originator Ref No"] = ds.Tables[0].Rows[cnt]["Ref_Number"].ToString();
                            drRow["Submittal Name"] = ds.Tables[0].Rows[cnt]["DocName"].ToString();
                            drRow["Contractor Upload"] = "Not Pending";
                            dtResponse.Rows.Add(drRow);
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Project Name";
                    }
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            return Json(new
            {
                Status = "Success",
                Documents = JsonConvert.SerializeObject(dtResponse)
            });
        }


        //added on 21/12/2022 for venkat
        [Authorize]
        [HttpPost]
        [Route("api/ExtoIntegration/Replace-OnLineFlow2Doc")]
        public IHttpActionResult ReplaceOnLineFlow2Doc()
        {
            bool sError = false;
            string ErrorText = "";
            bool CoverLetterUpload = false;
            bool DocumentsUpload = false;

            string CoverPagePath = "";
            string DocumentPagePath = "";
            string CoverLetterName = "";
            string ActualDocumentName = "";
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {

                    if (String.IsNullOrEmpty(httpRequest.Params["ActualdocumentUID"]))
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:ActualdocumentUID is Manadatory"
                        }); ;
                    }
                    var actualdocumentUID = httpRequest.Params["ActualdocumentUID"];

                    if (String.IsNullOrEmpty(httpRequest.Params["DocumentDate"]))
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:DocumentDate is Manadatory"
                        }); ;
                    }
                    //added on 07/03/2023
                    if (String.IsNullOrEmpty(httpRequest.Params["OriginatorRefNumber"]))
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:OriginatorRefNumber is Manadatory"
                        }); ;
                    }
                    //
                    var documentDate = httpRequest.Params["DocumentDate"];
                    if (documentDate == null)
                    {
                        documentDate = DateTime.Now.ToString();
                    }
                    var referenceNumber = httpRequest.Params["OriginatorRefNumber"];
                    var projectname = httpRequest.Params["projectname"];
                    DataTable dtProject = db.GetWorkPackages_ProjectName(projectname);

                    if (dtProject.Rows.Count > 0)
                    {
                        string projectuid = dtProject.Rows[0]["projectuid"].ToString();
                        if (httpRequest.Files.Count == 0)
                        {
                            sError = true;
                            ErrorText = "Please upload a document.";
                        }
                        
                        for (int i = 0; i < httpRequest.Files.Count; i++)
                        {
                            HttpPostedFile httpPostedFile = httpRequest.Files[i];

                            if (httpPostedFile != null)
                            {
                                if (httpPostedFile.FileName.ToUpper().StartsWith("COVER"))
                                {
                                    CoverLetterUpload = true;
                                }
                                else if(! string.IsNullOrEmpty(httpPostedFile.FileName))
                                {
                                    DocumentsUpload = true;
                                }
                            }
                        }
                        if (!CoverLetterUpload)
                        {
                            sError = true;
                            ErrorText = "Please upload a cover letter file.";
                        }
                        if (!DocumentsUpload)
                        {
                            sError = true;
                            ErrorText = "Please upload document file.";
                        }
                        if (!sError)
                        {
                            //
                            byte[] coverfiletobytes = null;
                            byte[] docfiletobytes = null;
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];
                                if (httpPostedFile != null)
                                {
                                    string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);

                                    if (Extn.ToLower() != ".exe" && Extn.ToLower() != ".msi" && Extn.ToLower() != ".db")
                                    {
                                        if (httpPostedFile.FileName.ToUpper().StartsWith("COVER"))
                                        {
                                            string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + projectuid + "//CoverLetter";

                                            if (!Directory.Exists(sDocumentPath))
                                            {
                                                Directory.CreateDirectory(sDocumentPath);
                                            }
                                            string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);


                                            int RandomNo = db.GenerateRandomNo();
                                            httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn);
                                            string savedPath = sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn;
                                            CoverPagePath = sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1" + Extn;

                                            EncryptFile(savedPath, CoverPagePath);
                                            //for blob
                                            coverfiletobytes = db.FileToByteArray(CoverPagePath);
                                            //
                                            CoverPagePath = "~/" + projectuid + "/CoverLetter" + "/" + sFileName + "_" + RandomNo + "_1" + Extn;
                                        }
                                        else
                                        {
                                            string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + projectuid + "//Documents";

                                            if (!Directory.Exists(sDocumentPath))
                                            {
                                                Directory.CreateDirectory(sDocumentPath);
                                            }
                                            string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);


                                            int RandomNo = db.GenerateRandomNo();
                                            httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn);
                                            string savedPath = sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1_copy" + Extn;
                                            DocumentPagePath = sDocumentPath + "/" + sFileName + "_" + RandomNo + "_1" + Extn;

                                            EncryptFile(savedPath, DocumentPagePath);
                                            //for blob
                                            docfiletobytes = db.FileToByteArray(DocumentPagePath);
                                            //
                                            DocumentPagePath = "~/" + projectuid + "/Documents" + "/" + sFileName + "_" + RandomNo + "_1" + Extn;
                                        }
                                    }

                                }
                            }
                            int cnt = db.ReplaceDocsFlow2old_API(new Guid(actualdocumentUID), ActualDocumentName, DocumentPagePath, CoverLetterName, CoverPagePath, Convert.ToDateTime(db.ConvertDateFormat(documentDate)), referenceNumber);
                            if (cnt == -1)
                            {
                                //for blob storage
                                DataSet dsUIDs = db.GetReplaceDocsFlow2old_UIDs_ForBlob(new Guid(actualdocumentUID));
                                if (dsUIDs.Tables[0].Rows.Count > 0)
                                {
                                    //for blob
                                    string Connectionstring = db.getProjectConnectionString(new Guid(projectuid));
                                    db.ReplaceDocsFlow2old_Blob(new Guid(actualdocumentUID), new Guid(dsUIDs.Tables[0].Rows[0]["CoverLetterUID"].ToString()), new Guid(dsUIDs.Tables[0].Rows[0]["StatusUID"].ToString()), new Guid(dsUIDs.Tables[0].Rows[0]["DocVersionUID"].ToString()), Connectionstring, docfiletobytes, coverfiletobytes);

                                }
                                sError = false;
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Actualdocumentuid";
                            }
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid ProjectName";
                    }

                }
                else
                {
                    return Json(new
                    {
                        Status = false,
                        Message = "Not Authorized IP address"
                    });
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            return Json(new
            {
                Status = "Success",
                Message = "Documents Replaced Successfully."

            });
        }


        [Authorize]
        [HttpPost]
        [Route("api/ExtoIntegration/ReplyCorrespondence")]
        public IHttpActionResult ReplyCorrespondence()
        {
            bool sError = false;
            string ErrorText = "";
            bool CoverLetterUpload = false;

            string CoverPagePath = "";
            string DocumentPagePath = "";
            string UserUID = string.Empty;
            Guid StatusUID = Guid.NewGuid();
            string Status = string.Empty;
            
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                var BaseURL = HttpContext.Current.Request.Url.ToString(); 
                string postData = "ActualdocumentUID=" + httpRequest.Params["ActualdocumentUID"] + "&UserID= " +
                    httpRequest.Params["UserID"] + "&CoverLetterDate= " +
                    httpRequest.Params["CoverLetterDate"]  + "&Remarks= " +
                    httpRequest.Params["Remarks"] + "&RefNumber= " +
                    httpRequest.Params["RefNumber"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (String.IsNullOrEmpty(db.CheckUserExists(Guid.NewGuid(), httpRequest.Params["UserID"], "Name")))
                    {
                        var resultmain = new
                        {
                            Status = "Failure",
                            Message = "UserID does not Exists !",
                        };
                        return Json(resultmain);
                    }
                    else
                    {
                        UserUID = db.CheckUserExists(Guid.NewGuid(), httpRequest.Params["UserID"], "Name");
                    }

                    if (String.IsNullOrEmpty(httpRequest.Params["ActualdocumentUID"]))
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:ActualdocumentUID is Manadatory"
                        }); ;
                    }
                    var actualdocumentUID = httpRequest.Params["ActualdocumentUID"];
                    var remarks = httpRequest.Params["Remarks"];
                    if (String.IsNullOrEmpty(httpRequest.Params["CoverLetterDate"]))
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:CoverLetterDate is Manadatory"
                        }); ;
                    }
                    //make the status
                    string FlowName = db.GetFlowName_by_SubmittalID(new Guid(db.GetSubmittalUID_By_ActualDocumentUID(new Guid(actualdocumentUID))));
                    if(FlowName == "EE Correspondence")
                    {
                        Status = "Reply to EE by Contractor";
                    }else if (FlowName == "CE Correspondence")
                    {
                        Status = "Reply to CE by Contractor";
                    }
                    else if (FlowName == "ACE Correspondence")
                    {
                        Status = "Reply to ACE by Contractor";
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:Flow Name not found for ActualdocumentUID!"
                        }); 
                    }
                    //
                    DataSet ds = db.ActualDocuments_SelectBy_ActualDocumentUID(new Guid(actualdocumentUID));
                    string projectuid = string.Empty;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        projectuid = ds.Tables[0].Rows[0]["ProjectUID"].ToString();
                        if (ds.Tables[0].Rows[0]["ActualDocument_CurrentStatus"].ToString() == "Submitted to Contractor" || ds.Tables[0].Rows[0]["ActualDocument_CurrentStatus"].ToString().Contains("Reply to Contractor"))
                        {

                        }
                        else
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:Contractor cannot reply to current status"
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:ActualdocumentUID not found"
                        });
                    }
                    //
                    var CoverLetterDate = httpRequest.Params["CoverLetterDate"];
                    if (CoverLetterDate == null)
                    {
                        CoverLetterDate = DateTime.Now.ToString();
                    }
                    CoverLetterDate = db.ConvertDateFormat(CoverLetterDate);
                    DateTime Document_Date = Convert.ToDateTime(CoverLetterDate);
                    var referenceNumber = httpRequest.Params["RefNumber"];
                  
             
                        if (httpRequest.Files.Count == 0)
                        {
                            sError = true;
                            ErrorText = "Please upload a document.";
                        }

                        for (int i = 0; i < httpRequest.Files.Count; i++)
                        {
                            HttpPostedFile httpPostedFile = httpRequest.Files[i];

                            if (httpPostedFile != null)
                            {
                                if (httpPostedFile.FileName.ToUpper().StartsWith("COVER"))
                                {
                                    CoverLetterUpload = true;
                                }
                                //else if (!string.IsNullOrEmpty(httpPostedFile.FileName))
                                //{
                                //    DocumentsUpload = true;
                                //}
                            }
                        }
                        if (!CoverLetterUpload)
                        {
                            sError = true;
                            ErrorText = "Please upload a cover letter file.";
                        }
                        //if (!DocumentsUpload)
                        //{
                        //    sError = true;
                        //    ErrorText = "Please upload attachment documents file.";
                        //}
                        if (!sError)
                        {
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];
                                if (httpPostedFile != null)
                                {
                                    string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                string Connectionstring = db.getProjectConnectionString(new Guid(projectuid));
                                if (Extn.ToLower() != ".exe" && Extn.ToLower() != ".msi" && Extn.ToLower() != ".db")
                                    {
                                        if (httpPostedFile.FileName.ToUpper().StartsWith("COVER"))
                                        {
                                        byte[] coverfiletobytes = null;
                                        string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"]  + projectuid +  "/" + StatusUID + "/CoverLetter";

                                            if (!Directory.Exists(sDocumentPath))
                                            {
                                                Directory.CreateDirectory(sDocumentPath);
                                            }
                                            string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);


                                           
                                            httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_1_copy" + Extn);
                                            string savedPath = sDocumentPath + "/" + sFileName  + "_1_copy" + Extn;
                                            CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;

                                            EncryptFile(savedPath, CoverPagePath);

                                           //for blob
                                           coverfiletobytes = db.FileToByteArray(CoverPagePath);
                                          //

                                        CoverPagePath = "~/" + projectuid +  "/" + StatusUID + "/CoverLetter" + "/" + sFileName  + "_1" + Extn;
                                            int CntDoc = db.InsertorUpdateDocumentStatus(StatusUID, new Guid(actualdocumentUID), 1, Status, 0, DateTime.Now, "",
                           new Guid(UserUID), remarks, Status, referenceNumber, Document_Date, CoverPagePath, "NotAll", "Contractor");
                                            if(CntDoc == 0)
                                            {
                                                return Json(new
                                                {
                                                    Success = false,
                                                    Message = "Error in Status Update"
                                                });
                                            }
                                            else
                                            {
                                            //store blob for document status tables
                                            //for blob
                                            byte[] docfiletobytes = null;
                                           
                                            db.InsertDocumentStatusBlob(StatusUID, new Guid(actualdocumentUID), coverfiletobytes, docfiletobytes, Connectionstring);
                                            //
                                            db.InsertIntoCorrespondenceCCToUsers(Guid.NewGuid(), new Guid(actualdocumentUID), StatusUID, "EE");
                                                db.InsertIntoCorrespondenceCCToUsers(Guid.NewGuid(), new Guid(actualdocumentUID), StatusUID, "CE");
                                                db.InsertIntoCorrespondenceCCToUsers(Guid.NewGuid(), new Guid(actualdocumentUID), StatusUID, "ACE");
                                                db.InsertIntoCorrespondenceCCToUsers(Guid.NewGuid(), new Guid(actualdocumentUID), StatusUID, "DTL");
                                                db.InsertIntoCorrespondenceCCToUsers(Guid.NewGuid(), new Guid(actualdocumentUID), StatusUID, "Contractor");
                                        }
                                        }
                                        else
                                        {
                                            string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + projectuid + "/" + StatusUID + "//Attachments";

                                            if (!Directory.Exists(sDocumentPath))
                                            {
                                                Directory.CreateDirectory(sDocumentPath);
                                            }
                                            string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                            httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_1_copy" + Extn);
                                            string savedPath = sDocumentPath + "/" + sFileName  + "_1_copy" + Extn;
                                            DocumentPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;

                                            EncryptFile(savedPath, DocumentPagePath);
                                        //for blob storage attachments
                                        byte[] filetobytesAttach = db.FileToByteArray(DocumentPagePath);
                                        //
                                        DocumentPagePath = "~/" + projectuid + "/" + StatusUID + "/Attachments" + "/" + sFileName  + "_1" + Extn;
                                        Guid AttachmentUID = Guid.NewGuid();
                                        int cntattach = db.InsertDocumentsAttachments(AttachmentUID, new Guid(actualdocumentUID), StatusUID, sFileName, DocumentPagePath, new Guid(UserUID), DateTime.Now);
                                        //blob
                                        db.InsertDocumentsAttachmentsBlob(AttachmentUID, new Guid(actualdocumentUID), StatusUID, filetobytesAttach, Connectionstring);

                                    }
                                }

                                }
                            }
                           
                        }

                   
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Not Authorized IP address"
                    });
                }


            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            return Json(new
            {
                Status = "Success",
                StatusUID = StatusUID,
                Message = "Documents Status Uploaded Successfully."

            });
        }

        [Authorize]//added on 18/01/2023
        [HttpPost]
        [Route("api/ExtoIntegration/GetBWSSBCorrespondence")]
        public IHttpActionResult GetBWSSBCorrespondence()
        {
            bool sError = false;
            string ErrorText = "";
            DataTable dtResponse = new DataTable();
            try
            {

                var httpRequest = HttpContext.Current.Request;
                var identity = (ClaimsIdentity)User.Identity;
                DataSet dtLastestStatus = new DataSet();
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "projectName=" + httpRequest.Params["ProjectName"] + ";WorkpackageName=" + httpRequest.Params["WorkpackageName"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
                DataTable dtProject = db.GetWorkPackages_ProjectName(httpRequest.Params["ProjectName"]);

                dtResponse.Columns.Add("FlowUid");
                dtResponse.Columns.Add("Flow Name");
                dtResponse.Columns.Add("Documents");
                if (dtProject.Rows.Count == 0)
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Project name is not exists"
                    });
                }

                string[] statusList = { "EE Correspondence", "ACE Correspondence", "CE Correspondence" };
                for (int cnt = 0; cnt < statusList.Length; cnt++)
                {
                    DataRow drNewRow = dtResponse.NewRow();
                    var flowUiddataTable = db.GetDocumentFlow().AsEnumerable().Where(r => r.Field<string>("Flow_Name").Equals(statusList[cnt].ToString())).ToList();
                    if (flowUiddataTable.Count == 0)
                    {

                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:flowid not exists"
                        });
                    }
                    string flowuid = flowUiddataTable.CopyToDataTable().Rows[0][0].ToString();


                    DataSet dsDocumentInfo = db.ActualDocuments_SelectBy_WorkpackageUID_Search(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), new Guid(dtProject.Rows[0]["WorkPackageUid"].ToString()), "", "All", "", "All", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, 4, "", "", flowuid);
                    DataTable dtSubResponse = new DataTable();
                    if (dsDocumentInfo.Tables[0].Rows.Count > 0)
                    {
                        dtSubResponse.Columns.Add("ActualDocumentUID ");
                        dtSubResponse.Columns.Add("Document Name");
                        dtSubResponse.Columns.Add("Current Status");
                        dtSubResponse.Columns.Add("Description");
                        dtSubResponse.Columns.Add("CCTo");
                        for (int i = 0; i < dsDocumentInfo.Tables[0].Rows.Count; i++)
                        {
                            try
                            {
                                 DataSet dsStatus = db.getActualDocumentStatusList(new Guid(dsDocumentInfo.Tables[0].Rows[i]["ActualDocumentUID"].ToString()));
                                if (dsStatus.Tables[0].Rows.Count > 0)
                                {
                                    if (dsStatus.Tables[0].Rows[0]["Current_Status"].ToString() == "Submitted to Contractor")
                                    {
                                        DataRow drDocs = dtSubResponse.NewRow();
                                        drDocs[0] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocumentUID"].ToString();

                                        drDocs[1] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_Name"].ToString();
                                        drDocs[2] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_CurrentStatus"].ToString();
                                        //if (dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_CurrentStatus"].ToString().StartsWith("Closed by"))
                                        //{

                                        //    if (dsStatus.Tables[0].Rows.Count > 0)
                                        //    {
                                        //        drDocs[1] = dsStatus.Tables[0].Rows[0]["ActualDocument_CurrentStatus"].ToString();
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    drDocs[2] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_CurrentStatus"].ToString();
                                        //}
                                        drDocs[3] = dsDocumentInfo.Tables[0].Rows[i]["Description"].ToString();
                                        drDocs[4] = "No";
                                        dtSubResponse.Rows.Add(drDocs);
                                    }
                                    else if (db.CheckCCtoUser(new Guid(dsStatus.Tables[0].Rows[0]["StatusUID"].ToString()), "Contractor"))
                                    {
                                        DataRow drDocs = dtSubResponse.NewRow();
                                        drDocs[0] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocumentUID"].ToString();

                                        drDocs[1] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_Name"].ToString();
                                        drDocs[2] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_CurrentStatus"].ToString();
                                        //if (dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_CurrentStatus"].ToString().StartsWith("Closed by"))
                                        //{

                                        //    if (dsStatus.Tables[0].Rows.Count > 0)
                                        //    {
                                        //        drDocs[1] = dsStatus.Tables[0].Rows[0]["ActualDocument_CurrentStatus"].ToString();
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    drDocs[2] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_CurrentStatus"].ToString();
                                        //}
                                        drDocs[3] = dsDocumentInfo.Tables[0].Rows[i]["Description"].ToString();
                                        drDocs[4] = "Yes";
                                        dtSubResponse.Rows.Add(drDocs);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                sError = true;
                                ErrorText = ex.Message;
                            }
                        }
                    }
                    if (dsDocumentInfo.Tables[0].Rows.Count > 0)
                    {
                        drNewRow["FlowUid"] = flowuid;
                        drNewRow["Flow Name"] = statusList[cnt].ToString();

                        drNewRow["Documents"] = JsonConvert.SerializeObject(dtSubResponse);
                        dtResponse.Rows.Add(drNewRow);
                    }
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            return Json(new
            {
                Status = "Success",
                Flows = dtResponse

            });
        }


        //[Authorize]
        //[HttpPost]
        //[Route("api/ExtoIntegration/GetBWSSBCorrespondence")]
        //public IHttpActionResult GetBWSSBCorrespondence()
        //{
        //    bool sError = false;
        //    string ErrorText = "";
        //    DataTable dtResponse = new DataTable();
        //    try
        //    {

        //        var httpRequest = HttpContext.Current.Request;
        //        var identity = (ClaimsIdentity)User.Identity;
        //        DataSet dtLastestStatus = new DataSet();
        //        var BaseURL = HttpContext.Current.Request.Url.ToString();
        //        string postData = "projectName=" + httpRequest.Params["ProjectName"] + ";WorkpackageName=" + httpRequest.Params["WorkpackageName"];
        //        db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
        //        DataTable dtProject = db.GetWorkPackages_ProjectName(httpRequest.Params["ProjectName"]);

        //        dtResponse.Columns.Add("FlowUid");
        //        dtResponse.Columns.Add("Flow Name");
        //        dtResponse.Columns.Add("Documents");
        //        if (dtProject.Rows.Count == 0)
        //        {
        //            return Json(new
        //            {
        //                Status = "Failure",
        //                Message = "Project name is not exists"
        //            });
        //        }
        //        string[] statusList = { "EE Correspondence", "ACE Correspondence", "CE Correspondence" };
        //        for (int cnt = 0; cnt < statusList.Length; cnt++)
        //        {
        //            DataRow drNewRow = dtResponse.NewRow();
        //            var flowUiddataTable = db.GetDocumentFlow().AsEnumerable().Where(r => r.Field<string>("Flow_Name").Equals(statusList[cnt].ToString())).ToList();
        //            if (flowUiddataTable.Count == 0)
        //            {

        //                return Json(new
        //                {
        //                    Status = "Failure",
        //                    Message = "Error:flowid not exists"
        //                });
        //            }
        //            string flowuid = flowUiddataTable.CopyToDataTable().Rows[0][0].ToString();
        //            drNewRow["FlowUid"] = flowuid;
        //            drNewRow["Flow Name"] = statusList[cnt].ToString();

        //            DataSet dsDocumentInfo = db.ActualDocuments_SelectBy_WorkpackageUID_Search(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), new Guid(dtProject.Rows[0]["WorkPackageUid"].ToString()), "", "All", "", "All", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, 4, "", "", flowuid);
        //            DataTable dtSubResponse = new DataTable();
        //            if (dsDocumentInfo.Tables[0].Rows.Count > 0)
        //            {
        //                dtSubResponse.Columns.Add("ActualDocumentUID ");
        //                dtSubResponse.Columns.Add("Document Name");
        //                dtSubResponse.Columns.Add("Current Status");

        //                for (int i = 0; i < dsDocumentInfo.Tables[0].Rows.Count; i++)
        //                {
        //                    try
        //                    {
        //                        DataRow drDocs = dtSubResponse.NewRow();
        //                        drDocs[0] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocumentUID"].ToString();

        //                        drDocs[1] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_Name"].ToString();
        //                        drDocs[2] = dsDocumentInfo.Tables[0].Rows[i]["ActualDocument_CurrentStatus"].ToString();
        //                        dtSubResponse.Rows.Add(drDocs);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        sError = true;
        //                        ErrorText = ex.Message;
        //                    }
        //                }
        //            }

        //            drNewRow["Documents"] = JsonConvert.SerializeObject(dtSubResponse);
        //            dtResponse.Rows.Add(drNewRow);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sError = true;
        //        ErrorText = ex.Message;
        //    }
        //    if (sError)
        //    {
        //        return Json(new
        //        {
        //            Status = "Failure",
        //            Message = "Error:" + ErrorText
        //        });
        //    }
        //    return Json(new
        //    {
        //        Status = "Success",
        //        Flows = dtResponse

        //    });
        //}

        //added on 21/12/2022 for venkat

        //added on 06/02/2022  for venkat
        //[Authorize]
        //[HttpPost]
        //[Route("api/ExtoIntegration/GetDocumentStatus")]
        //public IHttpActionResult GetDocumentStatus()
        //{

        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        var identity = (ClaimsIdentity)User.Identity;
        //        DataSet dtLastestStatus = new DataSet();
        //        var BaseURL = HttpContext.Current.Request.Url.ToString();
        //        string postData = "documentUid=" + httpRequest.Params["document"];
        //        db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
        //        if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
        //        {
        //            var documentUid = httpRequest.Params["document"];
        //            dtLastestStatus = db.getLatesDocumentStatus(new Guid(documentUid));
        //            if (dtLastestStatus.Tables[0].Rows.Count == 0)
        //            {
        //                return Json(new
        //                {
        //                    Status = "Failure",
        //                    Message = "No data available"
        //                });
        //            }
        //            string base64 = "";
        //            string getExtension = "";
        //            string fileName = "";
        //            if (!string.IsNullOrEmpty(dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString()))
        //            {
        //                Byte[] bytes = File.ReadAllBytes(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString());
        //                base64 = Convert.ToBase64String(bytes);
        //                getExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString());
        //                fileName = System.IO.Path.GetFileName(ConfigurationManager.AppSettings["DocumentsPathReviewFile"] + dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString());
        //            }
        //            return Json(new
        //            {
        //                status = dtLastestStatus.Tables[0].Rows[0]["Status"].ToString(),
        //                statusupadteddate = dtLastestStatus.Tables[0].Rows[0]["statusUpdatedDate"].ToString(),
        //                remarks = dtLastestStatus.Tables[0].Rows[0]["remarks"].ToString(),
        //                fileContent = base64,
        //                fileName = fileName,
        //                fileType = getExtension
        //            });

        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                Success = false,
        //                Message = "Not Authorized IP address"
        //            });
        //        }




        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            Success = false,
        //            Message = "Error" + ex.Message
        //        });
        //    }

        //}








        //[Authorize]
        //[HttpPost]
        //[Route("api/ExtoIntegration/AddDocument")]
        //public IHttpActionResult AddDocument()
        //{
        //    bool sError = false;
        //    string ErrorText = "";
        //    string CoverUID = string.Empty;
        //    string DocUID = string.Empty;
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        var ProjectName = httpRequest.Params["ProjectName"];
        //        var WorkpackageName = httpRequest.Params["WorkpackageName"];
        //        var SubmittalUID = httpRequest.Params["SubmittalUID"];
        //        var ReferenceNumber = httpRequest.Params["ReferenceNumber"];
        //        var Description = httpRequest.Params["Description"];
        //        var DocumentType = httpRequest.Params["DocumentType"];
        //        var DocMedia_HardCopy = httpRequest.Params["DocMedia_HardCopy"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_HardCopy"]) : false;
        //        var DocMedia_SoftCopy_PDF = httpRequest.Params["DocMedia_SoftCopy_PDF"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_PDF"]) : false;
        //        var DocMedia_SoftCopy_Editable = httpRequest.Params["DocMedia_SoftCopy_Editable"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_Editable"]) : false;
        //        var DocMedia_SoftCopy_Ref = httpRequest.Params["DocMedia_SoftCopy_Ref"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_SoftCopy_Ref"]) : false;
        //        var DocMedia_HardCopy_Ref = httpRequest.Params["DocMedia_HardCopy_Ref"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_HardCopy_Ref"]) : false;
        //        var DocMedia_NoMedia = httpRequest.Params["DocMedia_NoMedia"] != string.Empty ? Convert.ToBoolean(httpRequest.Params["DocMedia_NoMedia"]) : false;
        //        var Originator = httpRequest.Params["Originator"];
        //        var OriginatorRefNo = httpRequest.Params["OriginatorRefNo"];
        //        var IncomingReciveDate = httpRequest.Params["IncomingReciveDate"];
        //        //var CoverLetterDate = httpRequest.Params["CoverLetterDate"];
        //        var DocumentDate = httpRequest.Params["DocumentDate"];
        //        var FileReferenceNumber = httpRequest.Params["FileReferenceNumber"];
        //        var Remarks = httpRequest.Params["Remarks"];

        //        bool CoverLetterUpload = false, DocumentsUpload = false;
        //        var wExists = string.Empty;
        //        var pExists = db.ProjectNameExists(ProjectName);
        //        string ActualDocumentUID = string.Empty;

        //        if (!string.IsNullOrEmpty(pExists))
        //        {
        //            wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
        //            if (!string.IsNullOrEmpty(wExists))
        //            {
        //                if (Guid.TryParse(SubmittalUID, out Guid submittaluid))
        //                {
        //                    if (!string.IsNullOrEmpty(Description))
        //                    {
        //                        if (!string.IsNullOrEmpty(DocumentType))
        //                        {
        //                            string[] formats = { "dd/MM/yyyy" };
        //                            if (DateTime.TryParseExact(DocumentDate, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime documentDate))
        //                            {
        //                                if (DocumentType == "Cover Letter")
        //                                {
        //                                    if (string.IsNullOrEmpty(Originator))
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "Originator cannot be empty.";
        //                                    }
        //                                    if (string.IsNullOrEmpty(OriginatorRefNo))
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "Originator Reference number cannot be empty.";
        //                                    }
        //                                    if (!DateTime.TryParseExact(IncomingReciveDate, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime incomingReceiveDate))
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "Invalid Incoming Receive Date.";
        //                                    }
        //                                    if (httpRequest.Files.Count == 0)
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "Please upload a document.";
        //                                    }
        //                                    if (documentDate > incomingReceiveDate)
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "Document Date connot be greater than Incoming Receive date.";
        //                                    }
        //                                    for (int i = 0; i < httpRequest.Files.Count; i++)
        //                                    {
        //                                        HttpPostedFile httpPostedFile = httpRequest.Files[i];

        //                                        if (httpPostedFile != null)
        //                                        {
        //                                            if (httpPostedFile.FileName.StartsWith("Cover") || httpPostedFile.FileName.StartsWith("Cover_Letter") || httpPostedFile.FileName.StartsWith("Cover Letter"))
        //                                            {
        //                                                CoverLetterUpload = true;
        //                                            }
        //                                            else
        //                                            {
        //                                                DocumentsUpload = true;
        //                                            }
        //                                        }
        //                                    }
        //                                    if (!CoverLetterUpload)
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "Please upload a cover letter file.";
        //                                    }
        //                                    if (!DocumentsUpload)
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "Please upload document file.";
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    if (httpRequest.Files.Count == 0)
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "Please upload a document.";
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                sError = true;
        //                                ErrorText = "Invalid Document Date.";
        //                            }

        //                        }
        //                        else
        //                        {
        //                            sError = true;
        //                            ErrorText = "Document Type cannot be empty.";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sError = true;
        //                        ErrorText = "Description cannot be empty.";
        //                    }

        //                }
        //                else
        //                {
        //                    sError = true;
        //                    ErrorText = "Invalid Submittal UID.";
        //                }
        //            }
        //            else
        //            {
        //                sError = true;
        //                ErrorText = "Workpackage name not found";
        //            }
        //        }
        //        else
        //        {
        //            sError = true;
        //            ErrorText = "Project name not found.";
        //        }

        //        if (!sError)
        //        {
        //            DataSet ds = db.getDocumentsbyDocID(new Guid(SubmittalUID));
        //            if (ds.Tables[0].Rows.Count > 0)
        //            {
        //                string cStatus = "Submitted", FileDatetime = DateTime.Now.ToString("dd MMM yyyy hh-mm-ss tt"), sDocumentPath = string.Empty,
        //                    DocumentFor = string.Empty, IncomingRec_Date_String = string.Empty, CoverPagePath = string.Empty, CoverLetterUID = ""; ;

        //                if (!string.IsNullOrEmpty(IncomingReciveDate))
        //                {
        //                    IncomingRec_Date_String = IncomingReciveDate;
        //                }
        //                else
        //                {
        //                    IncomingRec_Date_String = DocumentDate;
        //                }
        //                IncomingRec_Date_String = db.ConvertDateFormat(IncomingRec_Date_String);
        //                DateTime IncomingRec_Date = Convert.ToDateTime(IncomingRec_Date_String);

        //                if (string.IsNullOrEmpty(DocumentDate))
        //                {
        //                    DocumentDate = DateTime.MinValue.ToString("dd/MM/yyyy");
        //                }
        //                DocumentDate = db.ConvertDateFormat(DocumentDate);
        //                DateTime Document_Date = Convert.ToDateTime(DocumentDate);

        //                //Cover Letter Insert
        //                for (int i = 0; i < httpRequest.Files.Count; i++)
        //                {
        //                    HttpPostedFile httpPostedFile = httpRequest.Files[i];
        //                    if (httpPostedFile != null)
        //                    {
        //                        string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
        //                        if (Extn.ToLower() != ".exe" && Extn.ToLower() != ".msi" && Extn.ToLower() != ".db")
        //                        {
        //                            if ((httpPostedFile.FileName.StartsWith("Cover") || httpPostedFile.FileName.StartsWith("Cover_Letter")) && DocumentType == "Cover Letter")
        //                            {
        //                                DocumentFor = "Cover Letter";
        //                                sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + pExists + "//" + wExists + "//CoverLetter";

        //                                if (!Directory.Exists(sDocumentPath))
        //                                {
        //                                    Directory.CreateDirectory(sDocumentPath);
        //                                }
        //                                string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
        //                                httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + "_1_copy" + Extn);
        //                                string savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
        //                                CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;
        //                                EncryptFile(savedPath, CoverPagePath);

        //                                CoverLetterUID = Guid.NewGuid().ToString();

        //                                int RetCount = db.DocumentCoverLetter_Insert_or_Update(new Guid(CoverLetterUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, "",
        //                                DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn, DocMedia_HardCopy ? "true" : "false",
        //                                DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
        //                                DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus, Originator ?? "", Document_Date);
        //                                if (RetCount <= 0)
        //                                {
        //                                    sError = true;
        //                                    ErrorText = "Error occured while inserting cover letter. Please contact system admin.";
        //                                }
        //                                CoverUID = CoverLetterUID;
        //                                try
        //                                {
        //                                    if (File.Exists(savedPath))
        //                                    {
        //                                        File.Delete(savedPath);
        //                                    }
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    //throw
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                DocUID = string.Empty;
        //                //ActualDocument Insert
        //                for (int i = 0; i < httpRequest.Files.Count; i++)
        //                {
        //                    HttpPostedFile httpPostedFile = httpRequest.Files[i];
        //                    if (httpPostedFile != null)
        //                    {
        //                        string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
        //                        if (Extn.ToLower() != ".exe" && Extn.ToLower() != ".msi" && Extn.ToLower() != ".db")
        //                        {
        //                            if (!httpPostedFile.FileName.StartsWith("Cover"))
        //                            {

        //                                string sDate1 = "", sDate2 = "", sDate3 = "", sDate4 = "", sDate5 = "";
        //                                DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now, CDate4 = DateTime.Now, CDate5 = DateTime.Now;
        //                                if (!checkDocumentExists(Path.GetFileNameWithoutExtension(httpPostedFile.FileName), SubmittalUID))
        //                                {
        //                                    if (DocumentType == "General Document")
        //                                    {
        //                                        DocumentFor = "General Document";
        //                                    }
        //                                    else if (DocumentType == "Photographs")
        //                                    {
        //                                        DocumentFor = "Photographs";
        //                                    }
        //                                    else
        //                                    {
        //                                        DocumentFor = "Document";
        //                                    }

        //                                    sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + pExists + "//" + wExists + "//Documents";

        //                                    if (!Directory.Exists(sDocumentPath))
        //                                    {
        //                                        Directory.CreateDirectory(sDocumentPath);
        //                                    }

        //                                    string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);

        //                                    string savedPath = string.Empty;
        //                                    if (DocumentType != "Photographs")
        //                                    {
        //                                        savedPath = sDocumentPath + "/" + sFileName + "_1_copy" + Extn;
        //                                        httpPostedFile.SaveAs(savedPath);

        //                                        CoverPagePath = sDocumentPath + "/" + sFileName + "_1" + Extn;
        //                                        EncryptFile(savedPath, CoverPagePath);
        //                                    }
        //                                    else
        //                                    {
        //                                        savedPath = sDocumentPath + "/" + DateTime.Now.Ticks.ToString() + Path.GetFileName(httpPostedFile.FileName);
        //                                        httpPostedFile.SaveAs(savedPath);

        //                                    }
        //                                    ActualDocumentUID = Guid.NewGuid().ToString();
        //                                    string UploadFilePhysicalpath = CoverPagePath;
        //                                    string Flow1DisplayName = "", Flow2DisplayName = "", Flow3DisplayName = "", Flow4DisplayName = "", Flow5DisplayName = "";
        //                                    DataSet dsFlow = db.GetDocumentFlows_by_UID(new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()));
        //                                    if (dsFlow.Tables[0].Rows.Count > 0)
        //                                    {
        //                                        if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "-1")
        //                                        {
        //                                            Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();

        //                                            sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate1 = db.ConvertDateFormat(sDate1);
        //                                            CDate1 = Convert.ToDateTime(sDate1);

        //                                            int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow1(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo ?? "",
        //                                            DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
        //                                            DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
        //                                            DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", savedPath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
        //                                            new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, Flow1DisplayName, Originator ?? "", Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission");
        //                                            if (cnt <= 0)
        //                                            {
        //                                                sError = true;
        //                                                ErrorText = "Error occured while inserting document. Please contact system admin.";
        //                                            }
        //                                        }
        //                                        else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "1")
        //                                        {
        //                                            Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();

        //                                            sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate1 = db.ConvertDateFormat(sDate1);
        //                                            CDate1 = Convert.ToDateTime(sDate1);

        //                                            int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow1(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo ?? "",
        //                                            DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
        //                                            DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
        //                                            DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
        //                                            new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, Flow1DisplayName, Originator ?? "", Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission");
        //                                            if (cnt <= 0)
        //                                            {
        //                                                sError = true;
        //                                                ErrorText = "Error occured while inserting document. Please contact system admin.";
        //                                            }
        //                                        }
        //                                        else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "2")
        //                                        {
        //                                            Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
        //                                            Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();

        //                                            sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate1 = db.ConvertDateFormat(sDate1);
        //                                            CDate1 = Convert.ToDateTime(sDate1);

        //                                            sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate2 = db.ConvertDateFormat(sDate2);
        //                                            CDate2 = Convert.ToDateTime(sDate2);

        //                                            int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow2(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
        //                                              DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
        //                                              DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
        //                                              DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
        //                                              new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, Flow1DisplayName, Flow2DisplayName, Originator, Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission");
        //                                            if (cnt <= 0)
        //                                            {
        //                                                sError = true;
        //                                                ErrorText = "Error occured while inserting document. Please contact system admin.";
        //                                            }

        //                                        }
        //                                        else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "3")
        //                                        {
        //                                            Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
        //                                            Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
        //                                            Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();

        //                                            sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate1 = db.ConvertDateFormat(sDate1);
        //                                            CDate1 = Convert.ToDateTime(sDate1);


        //                                            sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate2 = db.ConvertDateFormat(sDate2);
        //                                            CDate2 = Convert.ToDateTime(sDate2);

        //                                            sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate3 = db.ConvertDateFormat(sDate3);
        //                                            CDate3 = Convert.ToDateTime(sDate3);

        //                                            sDate4 = DateTime.Now.ToString("dd/MM/yyyy");
        //                                            sDate4 = db.ConvertDateFormat(sDate4);
        //                                            CDate4 = Convert.ToDateTime(CDate4);

        //                                            int cnt = db.Document_Insert_or_Update(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
        //                                           DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
        //                                           DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
        //                                           DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
        //                                           new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3,
        //                                           Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Originator, Document_Date, UploadFilePhysicalpath, CoverLetterUID, "Submission");
        //                                            if (cnt <= 0)
        //                                            {
        //                                                sError = true;
        //                                                ErrorText = "Error occured while inserting document. Please contact system admin.";
        //                                            }

        //                                        }
        //                                        else if (dsFlow.Tables[0].Rows[0]["Steps_Count"].ToString() == "4")
        //                                        {
        //                                            Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
        //                                            Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
        //                                            Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();
        //                                            Flow4DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep4_DisplayName"].ToString();

        //                                            sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate1 = db.ConvertDateFormat(sDate1);
        //                                            CDate1 = Convert.ToDateTime(sDate1);


        //                                            sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate2 = db.ConvertDateFormat(sDate2);
        //                                            CDate2 = Convert.ToDateTime(sDate2);

        //                                            sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate3 = db.ConvertDateFormat(sDate3);
        //                                            CDate3 = Convert.ToDateTime(sDate3);


        //                                            sDate4 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate4 = db.ConvertDateFormat(sDate4);
        //                                            CDate4 = Convert.ToDateTime(sDate4);

        //                                            int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow4(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
        //                                              DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
        //                                              DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
        //                                              DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
        //                                              new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3, new Guid(ds.Tables[0].Rows[0]["FlowStep4_UserUID"].ToString()), CDate4,
        //                                              Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Flow4DisplayName, Originator, Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission");
        //                                            if (cnt <= 0)
        //                                            {
        //                                                sError = true;
        //                                                ErrorText = "Error occured while inserting document. Please contact system admin.";
        //                                            }

        //                                        }
        //                                        else
        //                                        {
        //                                            Flow1DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep1_DisplayName"].ToString();
        //                                            Flow2DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep2_DisplayName"].ToString();
        //                                            Flow3DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep3_DisplayName"].ToString();
        //                                            Flow4DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep4_DisplayName"].ToString();
        //                                            Flow5DisplayName = dsFlow.Tables[0].Rows[0]["FlowStep5_DisplayName"].ToString();

        //                                            sDate1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep1_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate1 = db.ConvertDateFormat(sDate1);
        //                                            CDate1 = Convert.ToDateTime(sDate1);

        //                                            sDate2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep2_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate2 = db.ConvertDateFormat(sDate2);
        //                                            CDate2 = Convert.ToDateTime(sDate2);

        //                                            sDate3 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep3_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate3 = db.ConvertDateFormat(sDate3);
        //                                            CDate3 = Convert.ToDateTime(sDate3);


        //                                            sDate4 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate4 = db.ConvertDateFormat(sDate4);
        //                                            CDate4 = Convert.ToDateTime(sDate4);

        //                                            sDate5 = Convert.ToDateTime(ds.Tables[0].Rows[0]["FlowStep4_TargetDate"].ToString()).ToString("dd/MM/yyyy");
        //                                            sDate5 = db.ConvertDateFormat(sDate5);
        //                                            CDate5 = Convert.ToDateTime(sDate5);

        //                                            int cnt = db.Document_Insert_or_Update_with_RelativePath_Flow5(new Guid(ActualDocumentUID), new Guid(pExists), new Guid(wExists), new Guid(SubmittalUID), ReferenceNumber, OriginatorRefNo,
        //                                              DocumentFor, IncomingRec_Date, new Guid(ds.Tables[0].Rows[0]["FlowUID"].ToString()), sFileName, Description, 1, Extn,
        //                                              DocMedia_HardCopy ? "true" : "false", DocMedia_SoftCopy_PDF == true ? "true" : "false", DocMedia_SoftCopy_Editable == true ? "true" : "false", DocMedia_SoftCopy_Ref == true ? "true" : "false",
        //                                              DocMedia_HardCopy_Ref == true ? "true" : "false", DocMedia_NoMedia == true ? "true" : "false", CoverPagePath, Remarks ?? "", FileReferenceNumber ?? "", cStatus,
        //                                              new Guid(ds.Tables[0].Rows[0]["FlowStep1_UserUID"].ToString()), CDate1, new Guid(ds.Tables[0].Rows[0]["FlowStep2_UserUID"].ToString()), CDate2, new Guid(ds.Tables[0].Rows[0]["FlowStep3_UserUID"].ToString()), CDate3, new Guid(ds.Tables[0].Rows[0]["FlowStep4_UserUID"].ToString()), CDate4, new Guid(ds.Tables[0].Rows[0]["FlowStep5_UserUID"].ToString()), CDate5,
        //                                              Flow1DisplayName, Flow2DisplayName, Flow3DisplayName, Flow4DisplayName, Flow5DisplayName, Originator, Document_Date, "", "", UploadFilePhysicalpath, CoverLetterUID, "Submission");
        //                                            if (cnt <= 0)
        //                                            {
        //                                                sError = true;
        //                                                ErrorText = "Error occured while inserting document. Please contact system admin.";
        //                                            }
        //                                        }

        //                                        DocUID += ActualDocumentUID + ",";
        //                                    }

        //                                    try
        //                                    {
        //                                        if (DocumentType != "Photographs")
        //                                        {
        //                                            if (File.Exists(savedPath))
        //                                            {
        //                                                File.Delete(savedPath);
        //                                            }
        //                                        }

        //                                    }
        //                                    catch (Exception ex)
        //                                    {
        //                                        //throw
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sError = true;
        //        ErrorText = ex.Message;
        //    }
        //    if (sError)
        //    {
        //        return Json(new
        //        {
        //            Status = "Failure",
        //            Message = "Error:" + ErrorText
        //        });
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(CoverUID))
        //        {
        //            return Json(new
        //            {
        //                Status = "Success",
        //                CoverLetterUID = CoverUID,
        //                ActualDocumentUID = DocUID.TrimEnd(','),
        //                Message = "Documents Uploaded Successfully."
        //            });
        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                Status = "Success",
        //                ActualDocumentUID = DocUID.TrimEnd(','),
        //                Message = "Documents Uploaded Successfully."
        //            });
        //        }

        //    }
        //}




        //added on 21/12/2022 for Venkat

    }
}
