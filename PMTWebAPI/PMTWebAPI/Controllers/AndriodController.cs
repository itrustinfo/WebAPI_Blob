using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using PMTWebAPI.DAL;
using PMTWebAPI.Models;

namespace PMTWebAPI.Controllers
{
    public class AndriodController : ApiController
    {

        DBGetData db = new DBGetData();

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
        [HttpPost]
        [Route("api/Andriod/GetPhysicalProgressChart")]
        public IHttpActionResult GetPhysicalProgressChart([FromBody] ProjectDetails projectObj)
        {
            bool sError = false;
            string ErrorText = "";
            DataSet dsResponse = new DataSet();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + projectObj.ProjectName;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataTable dtProject = db.GetWorkPackages_ProjectName(projectObj.ProjectName);
                    DataSet dsvalues = new DataSet();

                    if (dtProject.Rows.Count > 0)
                    {
                        dsResponse = db.GetTaskScheduleDatesforGraph(new Guid(dtProject.Rows[0]["workPackageUid"].ToString()));
                        if (dsResponse.Tables[0].Rows.Count > 0)
                        {
                            dsResponse.Tables[0].Columns.Add("Monthly Plan");
                            dsResponse.Tables[0].Columns.Add("Monthly Actual");
                            dsResponse.Tables[0].Columns.Add("Cumulative Plan");
                            dsResponse.Tables[0].Columns.Add("Cumulative Actual");
                            decimal planvalue = 0;
                            decimal actualvalue = 0;
                            decimal cumplanvalue = 0;
                            decimal cumactualvalue = 0;
                            foreach (DataRow dr in dsResponse.Tables[0].Rows)
                            {
                                //get the actual and planned values....
                                dsvalues.Clear();
                                dsvalues = db.GetTaskScheduleValuesForGraph(new Guid(dtProject.Rows[0]["workPackageUid"].ToString()), Convert.ToDateTime(dr["StartDate"].ToString()), Convert.ToDateTime(dr["StartDate"].ToString()).AddMonths(1));
                                if (dsvalues.Tables[0].Rows.Count > 0)
                                {
                                    planvalue = decimal.Parse(dsvalues.Tables[0].Rows[0]["TotalSchValue"].ToString());
                                    actualvalue = decimal.Parse(dsvalues.Tables[0].Rows[0]["TotalAchValue"].ToString());
                                    cumplanvalue += planvalue;
                                    cumactualvalue += actualvalue;
                                    dr["Monthly Plan"] = decimal.Round(planvalue, 2);
                                    dr["Monthly Actual"] = decimal.Round(actualvalue, 2);
                                    dr["Cumulative Plan"] = decimal.Round(cumplanvalue, 2);
                                    dr["Cumulative Actual"] = decimal.Round(cumactualvalue, 2);
                                }
                            }
                        }
                    }

                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }

            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dsResponse.Tables[0]));
            return Json(new { response = JsonConvert.SerializeObject(dsResponse.Tables[0]) });
        }

        //changed GetReportDocumentSummary to GetReportDocumentSummary2 on Dec-20
        [Authorize]
        [HttpPost]
        [Route("api/Andriod/GetReportDocumentSummary2")]
        public IHttpActionResult GetReportDocumentSummary2([FromBody] ProjectDetails projectObj)
        {
            bool sError = false;
            string ErrorText = "";
            DataTable dt = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + projectObj.ProjectName;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataTable dtProject = db.GetWorkPackages_ProjectName(projectObj.ProjectName);
                    DataSet dsvalues = new DataSet();

                    if (dtProject.Rows.Count > 0)
                    {

                        dt.Columns.Add("Sl_No");
                        dt.Columns.Add("Documents");
                        dt.Columns.Add("Status");
                        dt.Columns.Add("Number_Of_Documents");
                        //dt.Columns.Add("Submitted_by_the_Contractor");
                        //dt.Columns.Add("Recommended_Returned_by_PMC");
                        //dt.Columns.Add("Approved_by_BWSSB");
                        int TotalReturnedbyPMC = 0;
                        int TotalSubmitted = 0;
                        int TotalApproved = 0;
                        DataSet ds = db.GetDocumentSummary_by_WorkpackgeUID(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), new Guid(dtProject.Rows[0]["workPackageUid"].ToString()));
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                            {
                                DataRow dr = dt.NewRow();
                                if (i == 0)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts submitted by the Contractor";
                                    dr["Status"] = "Submitted";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["SubmittedDocuments"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = ds.Tables[0].Rows[0]["SubmittedDocuments"].ToString();
                                    //dr["Recommended_Returned_by_PMC"] = "";
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalSubmitted += ds.Tables[0].Rows[0]["SubmittedDocuments"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["SubmittedDocuments"].ToString()) : 0;
                                }
                                else if (i == 1)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts under category A";
                                    dr["Status"] = "Code A";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["CodeA"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = ds.Tables[0].Rows[0]["CodeA"].ToString();
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalReturnedbyPMC += ds.Tables[0].Rows[0]["CodeA"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["CodeA"].ToString()) : 0;
                                }
                                else if (i == 2)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts under category B";
                                    dr["Status"] = "Code B";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["CodeB"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = ds.Tables[0].Rows[0]["CodeB"].ToString();
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalReturnedbyPMC += ds.Tables[0].Rows[0]["CodeB"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["CodeB"].ToString()) : 0;
                                }
                                else if (i == 3)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts under category C";
                                    dr["Status"] = "Code C";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["CodeC"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = ds.Tables[0].Rows[0]["CodeC"].ToString();
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalReturnedbyPMC += ds.Tables[0].Rows[0]["CodeC"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["CodeC"].ToString()) : 0;
                                }
                                else if (i == 4)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts under category D";
                                    dr["Status"] = "Code D";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["CodeD"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = ds.Tables[0].Rows[0]["CodeD"].ToString();
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalReturnedbyPMC += ds.Tables[0].Rows[0]["CodeD"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["CodeD"].ToString()) : 0;
                                }
                                else if (i == 5)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts under category E";
                                    dr["Status"] = "Code E";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["CodeE"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = ds.Tables[0].Rows[0]["CodeE"].ToString();
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalReturnedbyPMC += ds.Tables[0].Rows[0]["CodeE"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["CodeE"].ToString()) : 0;
                                }
                                else if (i == 6)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts under category F";
                                    dr["Status"] = "Code F";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["CodeF"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = ds.Tables[0].Rows[0]["CodeF"].ToString();
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalReturnedbyPMC += ds.Tables[0].Rows[0]["CodeF"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["CodeF"].ToString()) : 0;
                                }
                                else if (i == 7)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts under category G";
                                    dr["Status"] = "Code G";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["CodeG"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = ds.Tables[0].Rows[0]["CodeG"].ToString();
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalReturnedbyPMC += ds.Tables[0].Rows[0]["CodeG"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["CodeG"].ToString()) : 0;
                                }
                                else if (i == 8)
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "No. of Documemts under category H";
                                    dr["Status"] = "Code H";
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["CodeH"].ToString();
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = ds.Tables[0].Rows[0]["CodeH"].ToString();
                                    //dr["Approved_by_BWSSB"] = "-";
                                    TotalReturnedbyPMC += ds.Tables[0].Rows[0]["CodeH"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["CodeH"].ToString()) : 0;
                                }
                                else
                                {
                                    dr["Sl_No"] = (i + 1);
                                    dr["Documents"] = "Client Approved Documents";
                                    dr["Status"] = "Client Approved";
                                    //dr["Submitted_by_the_Contractor"] = "-";
                                    //dr["Recommended_Returned_by_PMC"] = "-";
                                    //dr["Approved_by_BWSSB"] = ds.Tables[0].Rows[0]["ClientApproved"].ToString();
                                    dr["Number_Of_Documents"] = ds.Tables[0].Rows[0]["ClientApproved"].ToString();
                                    TotalApproved += ds.Tables[0].Rows[0]["ClientApproved"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[0]["ClientApproved"].ToString()) : 0;
                                }
                                dt.Rows.Add(dr);
                            }
                            DataRow drtot = dt.NewRow();
                            drtot["Sl_No"] = "";
                            drtot["Documents"] = "Total No. of Documents";
                            drtot["Number_Of_Documents"] = TotalSubmitted + TotalReturnedbyPMC + TotalApproved;
                            //drtot["Submitted_by_the_Contractor"] = TotalSubmitted;
                            //drtot["Recommended_Returned_by_PMC"] = TotalReturnedbyPMC;
                            //drtot["Approved_by_BWSSB"] = TotalApproved;
                            dt.Rows.Add(drtot);
                        }

                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Project Name";
                    }
                }

                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }

            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            if (dt.Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Data Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dt));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dt) });
        }

        // changed GetDocumentByStatus2 to GetDocumentByStatus on 21st Dec --By Nikhil
        [Authorize]
        [HttpPost]
        [Route("api/Andriod/GetDocumentByStatus")]
        public IHttpActionResult GetDocumentByStatus([FromBody] DocumentCategory docObj)
        {
            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + docObj.ProjectName + ";Status=" + docObj.status;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataTable dtProject = db.GetWorkPackages_ProjectName(docObj.ProjectName);
                    if (dtProject.Rows.Count > 0)
                    {
                        DataSet dsDocumentInfo = db.ActualDocuments_SelectBy_WorkpackageUID_Search(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), new Guid(dtProject.Rows[0]["WorkPackageUid"].ToString()), "", "All", "", docObj.status, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, 4);
                        // db.LogWrite("Documents Count:" + dsDocumentInfo.Tables[0].Rows.Count);
                        if (dsDocumentInfo.Tables[0].Rows.Count > 0)
                        {
                            dtRespone.Columns.Add("Submittal Name");
                            dtRespone.Columns.Add("Document Name");
                            dtRespone.Columns.Add("Document Type");
                            dtRespone.Columns.Add("Current Status");
                            dtRespone.Columns.Add("Incoming Recv. Date");
                            dtRespone.Columns.Add("Document Date");
                            dtRespone.Columns.Add("Document UID");
                            for (int cnt = 0; cnt < dsDocumentInfo.Tables[0].Rows.Count; cnt++)
                            {
                                try
                                {
                                    DataRow drtot = dtRespone.NewRow();
                                    drtot["Submittal Name"] = db.getDocumentName_by_DocumentUID(new Guid(dsDocumentInfo.Tables[0].Rows[cnt]["DocumentUID"].ToString()));
                                    drtot["Document Name"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_Name"].ToString();
                                    drtot["Document Type"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_Type"].ToString();
                                    drtot["Current Status"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_CurrentStatus"].ToString();
                                    drtot["Incoming Recv. Date"] = Convert.ToDateTime(dsDocumentInfo.Tables[0].Rows[cnt]["IncomingRec_Date"]).ToString("dd/MM/yyyy");
                                    drtot["Document Date"] = Convert.ToDateTime(dsDocumentInfo.Tables[0].Rows[cnt]["Document_Date"]).ToString("dd/MM/yyyy");
                                    drtot["Document UID"] = dsDocumentInfo.Tables[0].Rows[cnt]["DocumentUID"].ToString();
                                    dtRespone.Rows.Add(drtot);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Project Name";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            if (dtRespone.Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Documents Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dtRespone));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone) });
        }

        [Authorize]
        [HttpPost]
        [Route("api/Andriod/GetTaskByProjectName")]
        public IHttpActionResult GetTaskByProjectName([FromBody] ProjectDetails projectObj)
        {
            var BaseURL = HttpContext.Current.Request.Url.ToString();

            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table

                string postData = "ProjectName=" + projectObj.ProjectName;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataTable dtProject = db.GetWorkPackages_ProjectName(projectObj.ProjectName);
                    if (dtProject.Rows.Count > 0)
                    {
                        dtRespone = db.GetTasks_by_ProjectName(projectObj.ProjectName);
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Project Name";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            if (dtRespone.Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:No Tasks Available");
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Tasks Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dtRespone));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone) });
        }

        [Authorize]
        [HttpPost]
        [Route("api/Andriod/SendDeviceId")]
        public IHttpActionResult SendDeviceId([FromBody] UserAppTokens userAppTokens)
        {
            var BaseURL = HttpContext.Current.Request.Url.ToString();

            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table

                string postData = "username=" + userAppTokens.username + ";deviceid=" + userAppTokens.deviceid;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataSet dsUsers = db.getUserDetails_by_EmailID(userAppTokens.username);
                    if (dsUsers.Tables[0].Rows.Count > 0)
                    {

                        int cnt = db.InserUserAppTokenDetails(dsUsers.Tables[0].Rows[0]["UserUID"].ToString(), userAppTokens.deviceid);
                        if (cnt > 0)
                        {
                            sError = false;
                            ErrorText = "Inserted Successfully";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid UserName";
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
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:No Tasks Available");
            return Json(new
            {
                Status = "Success",
                Message = ErrorText
            });


        }

        [Authorize]
        [HttpPost]
        [Route("api/Andriod/gettaskdetailsfromtaskuid")]
        public IHttpActionResult gettaskdetailswfromtaskuid([FromBody] Tasks taskObj)
        {
            var BaseURL = HttpContext.Current.Request.Url.ToString();

            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table

                string postData = "taskuid=" + taskObj.TaskUID;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    dtRespone = db.getNextLevelTaskDetails(taskObj.TaskUID);
                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            if (dtRespone.Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:No Tasks Available");
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Tasks Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dtRespone));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone) });

        }


        // chanhed getProjectListByUserName to getProjectListByUserName2 on 20 Dec
        [Authorize]
        [HttpPost]
        [Route("api/Andriod/getProjectListByUserName2")]
        public IHttpActionResult getProjectListByUserName2([FromBody] UserAppTokens users)
        {
            var BaseURL = HttpContext.Current.Request.Url.ToString();

            bool sError = false;
            string ErrorText = "";
            DataSet dtRespone = new DataSet();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table

                string postData = "username=" + users.username;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataSet dsUsers = db.getUserDetails_by_EmailID(users.username);
                    if (users.username == "superadmin")
                    {
                        dtRespone = db.GetProjects();
                    }
                    else
                    {
                        if (dsUsers.Tables[0].Rows.Count > 0)
                        {

                            dtRespone = db.GetAssignedProjects_by_UserUID2(new Guid(dsUsers.Tables[0].Rows[0]["UserUId"].ToString()));
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid UserName";
                        }
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
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }

            if (dtRespone.Tables[0].Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:No Projects Available");
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Projects Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dtRespone.Tables[0]));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone.Tables[0]) });
        }
        [Authorize]
        [HttpPost]
        [Route("api/Andriod/GetProjectUserdocuments")]
        public IHttpActionResult GetProjectUserdocuments([FromBody] UserProjectDocuments projectObj)
        {
            var BaseURL = HttpContext.Current.Request.Url.ToString();
            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table

                string postData = "ProjectName=" + projectObj.ProjectName + ";UserName=" + projectObj.Username;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataTable dtProject = db.GetWorkPackages_ProjectName(projectObj.ProjectName);
                    if (dtProject.Rows.Count > 0)
                    {
                        dtRespone = db.GetNextUserDocuments(new Guid(dtProject.Rows[0]["ProjectUID"].ToString()), new Guid(dtProject.Rows[0]["WorkpackageUID"].ToString()));
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
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }

            if (dtRespone.Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:No Projects Available");
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Projects Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dtRespone));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone) });

        }


        [Authorize]
        [HttpPost]
        [Route("api/Android/AddIssues-Android")]
        public IHttpActionResult AddIssues([FromBody] AddIssue addIssue)
        {
            bool sError = false;

            string ErrorText = "";
            Guid InvoiceMaster_UID = Guid.NewGuid();
            var httpRequest = HttpContext.Current.Request;

            var transactionUid = Guid.NewGuid();
            var issue_uid = Guid.NewGuid();
            try
            {
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + addIssue.projectname + ";Issue description=" + addIssue.Issuedescription +
                    ";reporting user=" + addIssue.reportinguser + ";reporting date=" + addIssue.reportingdate +
                     ";remarks=" + addIssue.remarks +
                    "issue document=" + addIssue.issuedocument.Count;
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(addIssue.projectname) || String.IsNullOrEmpty(addIssue.Issuedescription) ||
                      String.IsNullOrEmpty(addIssue.reportinguser) || String.IsNullOrEmpty(addIssue.reportingdate))
                {

                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    });
                }


                var identity = (ClaimsIdentity)User.Identity;

                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var projectName = addIssue.projectname;
                    DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                    if (dtWorkPackages.Rows.Count > 0)
                    {
                        DataTable dtIssuesExist = db.getIssuesByDescription(new Guid(dtWorkPackages.Rows[0]["WorkpackageUid"].ToString()), addIssue.Issuedescription);
                        if (dtIssuesExist.Rows.Count > 0)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Issue already Exists"

                            });
                        }

                        DataSet dsUserDetails = db.getUserDetails_by_EmailID(addIssue.reportinguser);

                        if (dsUserDetails.Tables[0].Rows.Count == 0)
                        {

                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Reporting user is not available"

                            });

                        }
                        //if (dsUserDetails.Tables[0].Rows[0]["IsContractor"].ToString() != "Y")
                        //{
                        //    return Json(new
                        //    {
                        //        Status = "Failure",
                        //        Message = "Not Authorized to add issue"

                        //    });
                        //}
                        string reportUserUid = dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString();

                        DateTime reportDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(addIssue.reportingdate.ToString()))
                        {
                            string tmpDate = db.ConvertDateFormat(addIssue.reportingdate.ToString());
                            reportDate = Convert.ToDateTime(tmpDate);
                        }

                        if (DateTime.TryParse(reportDate.ToString(), out reportDate))
                        {

                            //AddDays need to removed when upload in indian server
                            if (reportDate.Date > DateTime.Now.Date.AddDays(1))
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Reporting Date should not be greater then today date"

                                });
                            }

                            string DecryptPagePath = "";
                            for (int i = 0; i < addIssue.issuedocument.Count; i++)
                            {
                                string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + "/Documents/Issues/";
                                string FileDirectory = "~/Documents/Issues/";
                                if (!Directory.Exists(sDocumentPath))
                                {
                                    Directory.CreateDirectory(sDocumentPath);
                                }

                                string fileName = addIssue.issuedocument[i].fileName;
                                string sFileName = Path.GetFileNameWithoutExtension(fileName);
                                string Extn = Path.GetExtension(fileName);
                                Byte[] bytes = Convert.FromBase64String(addIssue.issuedocument[i].filebase64);
                                File.WriteAllBytes(sDocumentPath + "/" + sFileName + Extn, bytes);

                                string savedPath = sDocumentPath + "/" + sFileName + Extn;
                                DecryptPagePath = sDocumentPath + "/" + sFileName + "_DE" + Extn;
                                db.EncryptFile(savedPath, DecryptPagePath);
                                DecryptPagePath = FileDirectory + "/" + sFileName + "_DE" + Extn;
                                db.InsertUploadedIssueDocument(sFileName + "_DE" + Extn, FileDirectory, issue_uid.ToString());

                            }
                            string remarks = "";
                            if (!string.IsNullOrEmpty(addIssue.remarks))
                            {
                                remarks = addIssue.remarks;
                            }
                            int Cnt = 0;
                            dsUserDetails = db.getUserDetails_by_EmailID("bm.srinivasamurthy@njsei.com");
                            if (projectName.ToUpper() == "CP-10" && dsUserDetails.Tables[0].Rows.Count > 0)
                            {
                                Cnt = db.InsertorUpdateIssues(issue_uid, Guid.Empty, addIssue.Issuedescription, reportDate, new Guid(reportUserUid), new Guid(dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString()), DateTime.Now, DateTime.Now.AddDays(10), new Guid(dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString()), DateTime.Now.AddDays(10), "Open", addIssue.remarks, new Guid(dtWorkPackages.Rows[0]["WorkpackageUid"].ToString()), new Guid(dtWorkPackages.Rows[0]["ProjectUid"].ToString()), DecryptPagePath);
                            }
                            else
                            {
                                Cnt = db.InsertorUpdateIssues(issue_uid, Guid.Empty, addIssue.Issuedescription, reportDate, new Guid(reportUserUid), "Open", remarks, new Guid(dtWorkPackages.Rows[0]["WorkpackageUid"].ToString()), new Guid(dtWorkPackages.Rows[0]["ProjectUid"].ToString()), DecryptPagePath);
                            }
                            if (Cnt > 0)
                            {
                                sError = false;
                                ErrorText = "Inserted successfully";
                            }
                            else
                            {
                                sError = false;
                                ErrorText = "Status is not inserted,Please contact administrator";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Date are not correct format.";
                        }

                    }
                    else
                    {


                        sError = true;
                        ErrorText = "ProjectName not exists";

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
                IssueUid = issue_uid,
                Message = ErrorText

            });
        }


        [Authorize]
        [HttpPost]
        [Route("api/Android/UpdateIssueStatus-Android")]
        public IHttpActionResult UpdateIssueStatus(UpdateIssueStatus updateIssueStatus)  //changed on 15/09/2022 
        {
            bool sError = false;
            string ErrorText = "";
            string files_path = "";
            Guid InvoiceMaster_UID = Guid.NewGuid();
            var httpRequest = HttpContext.Current.Request;
            var transactionUid = Guid.NewGuid();
            bool transactionStatus = false;
            DateTime updatingDate = DateTime.MinValue;
            Guid issue_remarks_uid = Guid.NewGuid();
            string projectUID = "";
            string workpackageUID = "";
            string userUid = "";
            try
            {
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + updateIssueStatus.ProjectName + ";issue uid=" + updateIssueStatus.issueuid + ";issue status=" + updateIssueStatus.issuestatus + ";issue status update date=" + updateIssueStatus.issuestatusupdatedate +
                    ";Issue status update user id=" + updateIssueStatus.Issue_status_update_user_id + ";issue status document=" + updateIssueStatus.issue_status_document.Count + ";remarks=" + updateIssueStatus.remarks;
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(updateIssueStatus.ProjectName) || String.IsNullOrEmpty(updateIssueStatus.issueuid) || String.IsNullOrEmpty(updateIssueStatus.issuestatus) || String.IsNullOrEmpty(updateIssueStatus.Issue_status_update_user_id)
                    || String.IsNullOrEmpty(updateIssueStatus.issuestatusupdatedate))
                {

                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    });
                }


                var identity = (ClaimsIdentity)User.Identity;

                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var projectName = updateIssueStatus.ProjectName;
                    DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                    if (dtWorkPackages.Rows.Count == 0)
                    {
                        sError = true;
                        ErrorText = "ProjectName not exists";
                    }
                    else
                    {
                        projectUID = dtWorkPackages.Rows[0]["ProjectUID"].ToString();
                        workpackageUID = dtWorkPackages.Rows[0]["WorkPackageUID"].ToString();
                        DataSet dsIssues = db.getIssuesList_by_UID(new Guid(updateIssueStatus.issueuid));
                        if (dsIssues.Tables[0].Rows.Count == 0)
                        {
                            sError = true;
                            ErrorText = "Issue uid is not available";
                        }
                        else
                        {

                            DataSet dsUserDetails = db.getUserDetails_by_EmailID(updateIssueStatus.Issue_status_update_user_id);
                            if (dsUserDetails.Tables[0].Rows.Count == 0)
                            {
                                sError = true;
                                ErrorText = "User is not available";
                            }
                            else
                            {
                                DateTime reportDate = DateTime.Now;
                                if (!string.IsNullOrEmpty(updateIssueStatus.issuestatusupdatedate.ToString()))
                                {
                                    string tmpDate = db.ConvertDateFormat(updateIssueStatus.issuestatusupdatedate.ToString());
                                    reportDate = Convert.ToDateTime(tmpDate);
                                }

                                if (DateTime.TryParse(reportDate.ToString(), out updatingDate))
                                {
                                    userUid = dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString();


                                    if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "Open" && (updateIssueStatus.issuestatus == "In-Progress" ||
                                               updateIssueStatus.issuestatus == "Close" || updateIssueStatus.issuestatus == "Reject"))
                                    {
                                        transactionStatus = true;
                                    }
                                    else if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "In-Progress" && (updateIssueStatus.issuestatus == "In-Progress" ||
                                              updateIssueStatus.issuestatus == "Close"))
                                    {

                                        transactionStatus = true;
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "User not allowed to update the status";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Update date is not correct format";
                                }

                            }
                            if (transactionStatus)
                            {
                                string DecryptPagePath = "";
                                for (int i = 0; i < updateIssueStatus.issue_status_document.Count; i++)
                                {

                                    string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + "/Documents/Issues/";
                                    string FileDirectory = "/Documents/Issues/";
                                    if (!Directory.Exists(sDocumentPath))
                                    {
                                        Directory.CreateDirectory(sDocumentPath);
                                    }
                                    string fileName = updateIssueStatus.issue_status_document[i].fileName;
                                    string sFileName = Path.GetFileNameWithoutExtension(fileName);
                                    string Extn = Path.GetExtension(fileName);
                                    Byte[] bytes = Convert.FromBase64String(updateIssueStatus.issue_status_document[i].filebase64);
                                    File.WriteAllBytes(sDocumentPath + "/" + sFileName + Extn, bytes);

                                    string savedPath = sDocumentPath + "/" + sFileName + Extn;
                                    DecryptPagePath = sDocumentPath + "/" + sFileName + "_DE" + Extn;

                                    files_path = files_path + DecryptPagePath + ",";

                                    db.EncryptFile(savedPath, DecryptPagePath);
                                    DecryptPagePath = FileDirectory + "/" + sFileName + "_DE" + Extn;


                                    db.InsertUploadedDocument(sFileName + "_DE" + Extn, FileDirectory, issue_remarks_uid.ToString());

                                }

                                int cnt = db.Issues_Status_Remarks_Insert(issue_remarks_uid, new Guid(updateIssueStatus.issueuid), updateIssueStatus.issuestatus, updateIssueStatus.remarks, DecryptPagePath, updatingDate);
                                if (cnt > 0)
                                {
                                    DataSet ds_issue = db.GetIssueByIssueUid(updateIssueStatus.issueuid);


                                    string work_package_name = "";
                                    string issue_description = "";

                                    if (ds_issue.Tables[0].Rows.Count > 0)
                                    {
                                        work_package_name = ds_issue.Tables[0].Rows[0].ItemArray[0].ToString();
                                        issue_description = ds_issue.Tables[0].Rows[0].ItemArray[1].ToString();
                                    }

                                    string sHtmlString = "";


                                    sHtmlString = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" + "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                                                              "<head>" + "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" + "<style>table, th, td {border: 1px solid black; padding:6px;}</style></head>" +
                                                                 "<body style='font-family:Verdana, Arial, sans-serif; font-size:12px; font-style:normal;'>";
                                    sHtmlString += "<div style='width:80%; float:left; padding:1%; border:2px solid #011496; border-radius:5px;'>" +
                                                       "<div style='float:left; width:100%; border-bottom:2px solid #011496;'>";

                                    sHtmlString += "<div style='float:left; width:7%;'><h2>ONTB</h2></div>";


                                    sHtmlString += "<div style='float:left; width:70%;'><h2 style='margin-top:10px;'>Project Monitoring Tool</h2></div>" +
                                               "</div>";
                                    sHtmlString += "<div style='width:100%; float:left;'><br/>Dear Users,<br/><br/><span style='font-weight:bold;'>Contractor has changed issue status" + "</span> <br/><br/></div>";
                                    sHtmlString += "<div style='width:100%; float:left;'><table style='width:100%;'>" +
                                                    "<tr><td><b>Project Name </b></td><td style='text-align:center;'><b>:</b></td><td>" + projectName + "</td></tr>" +
                                                   "<tr><td><b>Issue </b></td><td style='text-align:center;'><b>:</b></td><td>" + issue_description + "</td></tr>" +
                                                    "<tr><td><b>Issue Status </b></td><td style='text-align:center;'><b>:</b></td><td>" + updateIssueStatus.issuestatus + "</td></tr>" +
                                                    "<tr><td><b>Remarks </b></td><td style='text-align:center;'><b>:</b></td><td>" + updateIssueStatus.remarks + "</td></tr>" +
                                                    "<tr><td><b>Date </b></td><td style='text-align:center;'><b>:</b></td><td>" + DateTime.Today.ToString("dd-MMM-yyyy") + "</td></tr>";
                                    sHtmlString += "</table></div>";
                                    sHtmlString += "<div style='width:100%; float:left;'><br/><br/>Sincerely, <br/> MIS System.</div></div></body></html>";

                                    DataTable dtemailCred = db.GetEmailCredentials();

                                    DataSet ds_wp_emails = db.GetWorkPackageEmails(new Guid(projectUID), new Guid(workpackageUID));

                                    string to_email_ids = "";

                                    foreach (DataRow email in ds_wp_emails.Tables[0].Rows)
                                    {
                                        to_email_ids = to_email_ids + email.ItemArray[1].ToString() + ",";
                                    }

                                    if (to_email_ids.Length > 0)
                                        to_email_ids = to_email_ids.Substring(0, to_email_ids.Length - 1);

                                    if (files_path.Length > 0)
                                        files_path = files_path.Substring(0, files_path.Length - 1);


                                    Guid MailUID = Guid.NewGuid();

                                    if (ds_wp_emails.Tables[0].Rows.Count > 0)
                                        db.StoreEmaildataToMailQueue(MailUID, new Guid(userUid), dtemailCred.Rows[0][0].ToString(), to_email_ids, "Issue Status", sHtmlString, "", files_path);



                                    sError = false;
                                    ErrorText = "Updated Successfully";
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Status is not Updated,Please contact administrator";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "User not allowed to update the status";
                            }
                        }

                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";

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
                Message = ErrorText

            });

        }

        [Authorize]
        [HttpPost]
        [Route("api/Android/GetIssueList-Android")]
        public IHttpActionResult GetIssueList(ProjectWorkPackage projectWork)
        {
            List<Issue> finallist = new List<Issue>();
            var httpRequest = HttpContext.Current.Request;

            var transactionUid = Guid.NewGuid();
            var BaseURL = HttpContext.Current.Request.Url.ToString();
            string postData = "ProjectName=" + projectWork.projectname + ";WorkPackageName=" + projectWork.workpackage;

            db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

            if (String.IsNullOrEmpty(projectWork.projectname) || String.IsNullOrEmpty(projectWork.workpackage))
            {

                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:Mandatory fields are missing"
                });
            }


            DataSet ds1 = db.GetProjectUIDfromName(projectWork.projectname);

            if (ds1.Tables[0].Rows.Count == 0)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:Wrong Project Name"
                });
            }

            string workpackage_uid = db.GetWorkPackageID_ProjectId_WorkPackageName(new Guid(ds1.Tables[0].Rows[0].ItemArray[0].ToString()), projectWork.workpackage);

            if (workpackage_uid == "")
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:Wrong WorkPackageUID"
                });
            }

            DataSet workpackaeissues = db.getIssuesList_by_WorkPackageUID(new Guid(workpackage_uid));

            for (int l = 0; l < workpackaeissues.Tables[0].Rows.Count; l++)
            {

                DateTime? assigned_date = null;

                if (!string.IsNullOrEmpty(workpackaeissues.Tables[0].Rows[l]["Actual_Closer_Date"].ToString()))
                {
                    assigned_date = Convert.ToDateTime(workpackaeissues.Tables[0].Rows[l]["Actual_Closer_Date"].ToString());
                }

                Issue issue = new Issue()
                {
                    IssueUId = new Guid(workpackaeissues.Tables[0].Rows[l]["Issue_Uid"].ToString()),
                    IssueDescription = workpackaeissues.Tables[0].Rows[l]["Issue_Description"].ToString(),
                    Status = workpackaeissues.Tables[0].Rows[l]["Issue_Status"].ToString(),
                    IssueDate = Convert.ToDateTime(workpackaeissues.Tables[0].Rows[l]["Issue_Date"].ToString()),
                    ApprovedDate = assigned_date,
                    ReportingUser = db.getUserNameby_UID(new Guid(workpackaeissues.Tables[0].Rows[l]["Issued_User"].ToString())) // workpackaeissues.Tables[0].Rows[l]["Issued_User"].ToString(),
                };

                finallist.Add(issue);

            }
            //return Json(finallist.ToList().OrderBy(a => a.IssueDate));
            //return Json(finallist.ToList().OrderBy(a => a.IssueDate).Reverse());
            return Json(finallist.ToList());
        }

        [Authorize]
        [HttpPost]
        [Route("api/Android/GetIssueStatus-Android")]
        public IHttpActionResult GetIssueStatus(Issueid issueUID)
        {
            List<IssueModel> finallist = new List<IssueModel>();

            var httpRequest = HttpContext.Current.Request;

            //Insert into WebAPITransctions table

            var transactionUid = Guid.NewGuid();
            var BaseURL = HttpContext.Current.Request.Url.ToString();
            string postData = "IssueUID=" + issueUID.Issueuid;

            db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

            if (String.IsNullOrEmpty(issueUID.Issueuid))
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:Mandatory fields are missing"
                });
            }

            DataSet workpackageissues = db.getIssue_by_IssueUID(new Guid(issueUID.Issueuid));

            for (int l = 0; l < workpackageissues.Tables[0].Rows.Count; l++)
            {

                IssueModel issue = new IssueModel()
                {
                    IssueUId = new Guid(workpackageissues.Tables[0].Rows[l]["Issue_Uid"].ToString()),
                };

                DataSet issue_all_status = db.GetIssueStatus_by_Issue_Uid(issue.IssueUId);

                List<IssueStatusModel> issue_status_list = new List<IssueStatusModel>();

                IssueStatusModel new_issue_status_model = new IssueStatusModel()
                {
                    IssueStatusId = "",
                    Status = "Open",
                    Remarks = workpackageissues.Tables[0].Rows[l]["Issue_Description"].ToString(),
                    StatusDate = Convert.ToDateTime(workpackageissues.Tables[0].Rows[l]["Issue_Date"].ToString()),
                    issue_status_attachments = null
                };

                issue_status_list.Add(new_issue_status_model);

                DataSet issue_all_attached = db.GetUploadedIssueDocuments(issue.IssueUId.ToString());

                if (issue_all_attached.Tables[0].Rows.Count > 0)
                {
                    List<IssueStatusAttachedModel> issue_status_attached_list = new List<IssueStatusAttachedModel>();

                    foreach (DataRow attached in issue_all_attached.Tables[0].Rows)
                    {
                        string fileName = ConfigurationManager.AppSettings["DocumentsPath"] + "\\Documents\\Issues\\" + attached.ItemArray[1].ToString();
                        string fileExtension = System.IO.Path.GetExtension(attached.ItemArray[1].ToString());
                        string outPath = ConfigurationManager.AppSettings["DocumentsPath"] + "\\Documents\\Issues\\" + attached.ItemArray[1].ToString().Remove(0, 2) + "_download" + fileExtension;

                        String converted_file = "";

                        if (File.Exists(fileName))
                        {
                            db.DecryptFile(fileName, outPath);
                            Byte[] bytes = File.ReadAllBytes(outPath);
                            converted_file = Convert.ToBase64String(bytes);
                        }

                        IssueStatusAttachedModel issue_attached_model = new IssueStatusAttachedModel()
                        {
                            AttachedFileName = attached.ItemArray[1].ToString(),
                            FileAsBase64 = converted_file
                        };

                        issue_status_attached_list.Add(issue_attached_model);
                    }

                    new_issue_status_model.issue_status_attachments = issue_status_attached_list;
                }

                if (issue_all_status.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow status in issue_all_status.Tables[0].Rows)
                    {
                        IssueStatusModel issue_status_model = new IssueStatusModel()
                        {
                            IssueStatusId = status.ItemArray[0].ToString(),
                            Status = status.ItemArray[2].ToString(),
                            Remarks = status.ItemArray[3].ToString(),
                            StatusDate = Convert.ToDateTime(status.ItemArray[5].ToString()),
                            issue_status_attachments = null
                        };

                        DataSet issue_status_all_attached = db.GetUploadedDocuments(issue_status_model.IssueStatusId);

                        if (issue_status_all_attached.Tables[0].Rows.Count > 0)
                        {
                            List<IssueStatusAttachedModel> issue_status_attached_list = new List<IssueStatusAttachedModel>();

                            foreach (DataRow attached in issue_status_all_attached.Tables[0].Rows)
                            {
                                string fileName = ConfigurationManager.AppSettings["DocumentsPath"] + "\\Documents\\Issues\\" + attached.ItemArray[1].ToString();
                                string fileExtension = System.IO.Path.GetExtension(attached.ItemArray[1].ToString());
                                string outPath = ConfigurationManager.AppSettings["DocumentsPath"] + "\\Documents\\Issues\\" + attached.ItemArray[1].ToString().Remove(0, 2) + "_download" + fileExtension;

                                String converted_file = "";

                                if (File.Exists(fileName))
                                {
                                    db.DecryptFile(fileName, outPath);
                                    Byte[] bytes = File.ReadAllBytes(outPath);
                                    converted_file = Convert.ToBase64String(bytes);
                                }

                                IssueStatusAttachedModel issue_status_attached_model = new IssueStatusAttachedModel()
                                {
                                    AttachedFileName = attached.ItemArray[1].ToString(),
                                    FileAsBase64 = converted_file
                                };

                                issue_status_attached_list.Add(issue_status_attached_model);
                            }

                            issue_status_model.issue_status_attachments = issue_status_attached_list;
                        }

                        issue_status_list.Add(issue_status_model);
                    }

                    issue.issue_all_status = issue_status_list.OrderBy(a => a.StatusDate);
                }
                else
                {
                    issue.issue_all_status = issue_status_list.OrderBy(a => a.StatusDate);
                }

                finallist.Add(issue);


            }
            return Json(finallist.ToList());

        }


        // changed GetDocumentByStatus to GetDocumentByFlowName on Dec-21st -by Nikhil
        [Authorize]
        [HttpPost]
        [Route("api/Andriod/GetDocumentByFlow")]
        public IHttpActionResult GetDocumentByFlow([FromBody] DocumentCategory docObj)
        {
            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + docObj.ProjectName + ";Flow=" + docObj.flowid;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataTable dtProject = db.GetWorkPackages_ProjectName(docObj.ProjectName);
                    string flowuid = "";
                    if (dtProject.Rows.Count > 0)
                    {
                        if (docObj.flowid != "All")

                        {
                            var flowUiddataTable = db.GetDocumentFlow().AsEnumerable().Where(r => r.Field<string>("Flow_Name").Equals(docObj.flowid)).ToList();
                            if (flowUiddataTable.Count == 0)
                            {
                                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:flowid not exists");
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:flowid not exists"
                                });
                            }
                            flowuid = flowUiddataTable.CopyToDataTable().Rows[0][0].ToString();
                        }

                        DataSet dsDocumentInfo = db.ActualDocuments_SelectBy_WorkpackageUID_Search(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), new Guid(dtProject.Rows[0]["WorkPackageUid"].ToString()), "", "All", "", "All", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, 4, "", "", flowuid);
                        // db.LogWrite("Documents Count:" + dsDocumentInfo.Tables[0].Rows.Count);
                        if (dsDocumentInfo.Tables[0].Rows.Count > 0)
                        {
                            dtRespone.Columns.Add("Submittal Name");
                            dtRespone.Columns.Add("Document Name");
                            dtRespone.Columns.Add("Document Type");
                            dtRespone.Columns.Add("Current Status");
                            dtRespone.Columns.Add("Incoming Recv. Date");
                            dtRespone.Columns.Add("Document Date");
                            dtRespone.Columns.Add("Document UID");
                            for (int cnt = 0; cnt < dsDocumentInfo.Tables[0].Rows.Count; cnt++)
                            {
                                try
                                {
                                    DataRow drtot = dtRespone.NewRow();
                                    drtot["Submittal Name"] = db.getDocumentName_by_DocumentUID(new Guid(dsDocumentInfo.Tables[0].Rows[cnt]["DocumentUID"].ToString()));
                                    drtot["Document Name"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_Name"].ToString();
                                    drtot["Document Type"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_Type"].ToString();
                                    drtot["Current Status"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_CurrentStatus"].ToString();
                                    drtot["Incoming Recv. Date"] = Convert.ToDateTime(dsDocumentInfo.Tables[0].Rows[cnt]["IncomingRec_Date"]).ToString("dd/MM/yyyy");
                                    drtot["Document Date"] = Convert.ToDateTime(dsDocumentInfo.Tables[0].Rows[cnt]["Document_Date"]).ToString("dd/MM/yyyy");
                                    drtot["Document UID"] = dsDocumentInfo.Tables[0].Rows[cnt]["DocumentUID"].ToString();
                                    dtRespone.Rows.Add(drtot);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Project Name";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            if (dtRespone.Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Documents Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dtRespone));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone) });
        }

        //changed GetReportDocumentSummary2 to GetReportDocumentSummary on Dec-20
        [Authorize]
        [HttpPost]
        [Route("api/Andriod/GetReportDocumentSummary")]
        public IHttpActionResult GetReportDocumentSummary([FromBody] ProjectWorkPackage projectObj)
        {
            bool sError = false;
            string ErrorText = "";
            DataTable dt = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + projectObj.projectname;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataSet ds1 = db.GetProjectUIDfromName(projectObj.projectname);

                    if (ds1.Tables[0].Rows.Count == 0)
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:Wrong Project Name"
                        });
                    }

                    string workpackage_uid = db.GetWorkPackageID_ProjectId_WorkPackageName(new Guid(ds1.Tables[0].Rows[0].ItemArray[0].ToString()), projectObj.workpackage);

                    if (workpackage_uid == "")
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:Wrong WorkPackageUID"
                        });
                    }
                    DataSet dsvalues = new DataSet();

                    dt.Columns.Add("Sl_No");
                    dt.Columns.Add("TypeOfFlow");
                    dt.Columns.Add("Number_Of_Documents");
                    int TotalSubmitted = 0;
                    DataSet ds = db.getDocumentCount_by_ProjectUID_WorkPackageUID2(new Guid(ds1.Tables[0].Rows[0].ItemArray[0].ToString()), new Guid(workpackage_uid));
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 1; i <ds.Tables[0].Rows.Count; i++)
                        //for (int i = 0; i <ds.Tables[0].Rows.Count; i++) --by nikhil on 03/01/2023
                       //for (int i = 0; i < ds.Tables[0].Columns.Count; i++)  changed to above line by Nikhil on 20/12/2022
                        {
                            DataRow dr = dt.NewRow();
                            // Since not to show first row--changed by Nikhil -02/01/2023
                            //dr["Sl_No"] = (i+1);
                            dr["Sl_No"] = i;

                            dr["TypeOfFlow"] = ds.Tables[0].Rows[i]["flowName"].ToString();
                            dr["Number_Of_Documents"] = ds.Tables[0].Rows[i]["DocCount"].ToString();
                            TotalSubmitted += ds.Tables[0].Rows[i]["DocCount"].ToString() != "" ? Convert.ToInt32(ds.Tables[0].Rows[i]["DocCount"].ToString()) : 0;

                            dt.Rows.Add(dr);
                        }
                        //Commented below 6 lines for correction of API Result-by Nikhil 21/12/2022
                        //DataRow drtot = dt.NewRow();
                        //drtot["Sl_No"] = "";
                        //drtot["TypeOfFlow"] = "Total No. of Documents";
                        //drtot["Number_Of_Documents"] = TotalSubmitted;
                        //dt.Rows.Add(drtot);
                    }

                }

                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }

            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            if (dt.Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Data Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dt));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dt) });
        }


        // chanhed getProjectListByUserName2 to getProjectListByUserName on 20 Dec
        [Authorize]
        [HttpPost]
        [Route("api/Andriod/getProjectListByUserName")]
        public IHttpActionResult getProjectListByUserName([FromBody] UserAppTokens users)
        {
            var BaseURL = HttpContext.Current.Request.Url.ToString();

            bool sError = false;
            string ErrorText = "";
            DataSet dtRespone = new DataSet();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table

                string postData = "username=" + users.username;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataSet dsUsers = db.getUserDetails_by_EmailID(users.username);
                    if (users.username == "superadmin")
                    {
                        dtRespone = db.GetProjects();
                    }
                    else
                    {
                        if (dsUsers.Tables[0].Rows.Count > 0)
                        {

                            dtRespone = db.GetAssignedProjects_by_UserUID2(new Guid(dsUsers.Tables[0].Rows[0]["UserUId"].ToString()));
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid UserName";
                        }
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
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }

            if (dtRespone.Tables[0].Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:No Projects Available");
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Projects Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dtRespone.Tables[0]));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone.Tables[0]) });
        }

        // added for Android Person on 25-12-2022 by nikhil

        [Authorize]
        [HttpPost]
        [Route("api/Andriod/GetDocumentByStatusFlow")]
        public IHttpActionResult GetDocumentByStatusFlow([FromBody] DocumentCategory docObj)
        {
            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + docObj.ProjectName + ";Flow=" + docObj.flowid + ";status=" + docObj.status;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataTable dtProject = db.GetWorkPackages_ProjectName(docObj.ProjectName);
                    string flowuid = "";
                    if (dtProject.Rows.Count > 0)
                    {
                        if (docObj.flowid != "All")

                        {
                            var flowUiddataTable = db.GetDocumentFlow().AsEnumerable().Where(r => r.Field<string>("Flow_Name").Equals(docObj.flowid)).ToList();
                            if (flowUiddataTable.Count == 0)
                            {
                                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:flowid not exists");
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:flowid not exists"
                                });
                            }
                            flowuid = flowUiddataTable.CopyToDataTable().Rows[0][0].ToString();
                        }

                        DataSet dsDocumentInfo = db.ActualDocuments_SelectBy_WorkpackageUID_Search(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), new Guid(dtProject.Rows[0]["WorkPackageUid"].ToString()), "", "All", "", docObj.status, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, 4, "", "", flowuid);

                        // db.LogWrite("Documents Count:" + dsDocumentInfo.Tables[0].Rows.Count);
                        if (dsDocumentInfo.Tables[0].Rows.Count > 0)
                        {
                            dtRespone.Columns.Add("Submittal Name");
                            dtRespone.Columns.Add("Document Name");
                            dtRespone.Columns.Add("Document Type");
                            dtRespone.Columns.Add("Current Status");
                            dtRespone.Columns.Add("Incoming Recv. Date");
                            dtRespone.Columns.Add("Document Date");
                            dtRespone.Columns.Add("Document UID");
                            for (int cnt = 0; cnt < dsDocumentInfo.Tables[0].Rows.Count; cnt++)
                            {
                                try
                                {
                                    DataRow drtot = dtRespone.NewRow();
                                    drtot["Submittal Name"] = db.getDocumentName_by_DocumentUID(new Guid(dsDocumentInfo.Tables[0].Rows[cnt]["DocumentUID"].ToString()));
                                    drtot["Document Name"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_Name"].ToString();
                                    drtot["Document Type"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_Type"].ToString();
                                    drtot["Current Status"] = dsDocumentInfo.Tables[0].Rows[cnt]["ActualDocument_CurrentStatus"].ToString();
                                    drtot["Incoming Recv. Date"] = Convert.ToDateTime(dsDocumentInfo.Tables[0].Rows[cnt]["IncomingRec_Date"]).ToString("dd/MM/yyyy");
                                    drtot["Document Date"] = Convert.ToDateTime(dsDocumentInfo.Tables[0].Rows[cnt]["Document_Date"]).ToString("dd/MM/yyyy");
                                    drtot["Document UID"] = dsDocumentInfo.Tables[0].Rows[cnt]["DocumentUID"].ToString();
                                    dtRespone.Rows.Add(drtot);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Project Name";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            if (dtRespone.Rows.Count == 0)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "No Documents Available"
                }); ;
            }
            db.WebAPITransctionUpdate(transactionUid, "Success", JsonConvert.SerializeObject(dtRespone));
            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone) });
        }

        // added for Android Person on 25-12-2022 by nikhil

        [Authorize]
        [HttpPost]
        [Route("api/Andriod/GetDocumentSummaryByStatus")]
        public IHttpActionResult GetDocumentSummaryByStatus([FromBody] DocumentCategory docObj)
        {
            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + docObj.ProjectName + ";Flow=" + docObj.flowid + ";status=" + docObj.status;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    try
                    {
                        db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                        DataTable dtProject = db.GetWorkPackages_ProjectName(docObj.ProjectName);
                        string flowuid = "";
                        if (dtProject.Rows.Count > 0)
                        {
                            if (docObj.flowid != "All")

                            {
                                var flowUiddataTable = db.GetDocumentFlow().AsEnumerable().Where(r => r.Field<string>("Flow_Name").Equals(docObj.flowid)).ToList();
                                if (flowUiddataTable.Count == 0)
                                {
                                    db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:flowid not exists");
                                    return Json(new
                                    {
                                        Status = "Failure",
                                        Message = "Error:flowid not exists"
                                    });
                                }
                                flowuid = flowUiddataTable.CopyToDataTable().Rows[0][0].ToString();
                            }

                            DataSet dsDocumentInfo = db.ActualDocuments_SelectBy_WorkpackageUID_Search(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()), new Guid(dtProject.Rows[0]["WorkPackageUid"].ToString()), "", "All", "", "All", DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, 4, "", "", flowuid);
                            var docSummary = dsDocumentInfo.Tables[0].AsEnumerable()
                                .GroupBy(c => c.Field<string>("ActualDocument_CurrentStatus")).Select(g => new { status = g.Key, NoOfDocuments = g.Count() }).ToList();

                            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(docSummary) });
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid ProjectName";
                        }
                    }
                    catch (Exception ex)
                    {
                        sError = true;
                        ErrorText = ex.Message;
                    }

                }
                else
                {
                    sError = true;
                    ErrorText = "Not Authorized IP address";
                }
            }
            catch (Exception ex)
            {
                sError = true;
                ErrorText = ex.Message;
            }
            if (sError)
            {
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            return Json("");
        }

        [Authorize]
        [HttpPost]
        [Route("api/Andriod/Get_Andoird_Camera_details")]
        public IHttpActionResult Get_Andoird_Camera_details([FromBody] UserProjectDocuments projectUser)
        {
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + projectUser.ProjectName + ";Username=" + projectUser.Username;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    try
                    {
                        db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                        DataTable dtProject = db.GetWorkPackages_ProjectName(projectUser.ProjectName);
                        if (dtProject.Rows.Count == 0)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:Invalid ProjectName"
                            });
                        }
                        if (!string.IsNullOrEmpty(projectUser.Username))
                        {
                            DataSet dsUsers = db.getUserDetails_by_EmailID(projectUser.Username);
                            if (dsUsers.Tables[0].Rows.Count == 0)
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:Invalid UserName"
                                });
                            }
                        }
                        dtRespone = db.getAndroidCameraDetails(new Guid(dtProject.Rows[0]["ProjectUid"].ToString()));
                        if (dtRespone.Rows.Count == 0)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:No Camera details available"
                            });
                        }


                    }
                    catch (Exception ex)
                    {

                    }
                }
                return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone) });
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

        //added on 12/04/2023
        [Authorize]
        [HttpPost]
        [Route("api/Andriod/getTabsForUser")]
        public IHttpActionResult getTabsForUser([FromBody] UserAppTokens users)
        {
            var BaseURL = HttpContext.Current.Request.Url.ToString();

            bool sError = false;
            string ErrorText = "";
            DataTable dtRespone = new DataTable();
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                dtRespone.Columns.Add("TabName", typeof(string));
                string postData = "username=" + users.username;

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");
                    DataSet dsUsers = db.getUserDetails_by_EmailID(users.username);
                    if (users.username == "superadmin")
                    {
                        dtRespone.Rows.Add(new Object[] { "Documents" });
                        dtRespone.Rows.Add(new Object[] { "Issues" });
                        dtRespone.Rows.Add(new Object[] { "Notifications" });
                        dtRespone.Rows.Add(new Object[] { "Graph" });
                        dtRespone.Rows.Add(new Object[] { "Field Camera" });

                    }
                    else
                    {
                        if (dsUsers.Tables[0].Rows.Count > 0)
                        {
                            if (dsUsers.Tables[0].Rows[0]["TypeOfUser"].ToString() == "SP")
                            {
                               
                                dtRespone.Rows.Add(new Object[] { "Issues" });
                                dtRespone.Rows.Add(new Object[] { "Notifications" });
                                dtRespone.Rows.Add(new Object[] { "Field Camera" });

                            }
                            else if (dsUsers.Tables[0].Rows[0]["TypeOfUser"].ToString() == "DDE")
                            {

                                dtRespone.Rows.Add(new Object[] { "Documents" });
                                dtRespone.Rows.Add(new Object[] { "Notifications" });
                              
                                
                            }
                            else
                            {
                                dtRespone.Rows.Add(new Object[] { "Documents" });
                                dtRespone.Rows.Add(new Object[] { "Issues" });
                                dtRespone.Rows.Add(new Object[] { "Notifications" });
                                dtRespone.Rows.Add(new Object[] { "Graph" });
                                dtRespone.Rows.Add(new Object[] { "Field Camera" });
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid UserName";
                        }
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
                db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }

            return Json(new { Status = "Success", response = JsonConvert.SerializeObject(dtRespone)});


        }
    }
}
