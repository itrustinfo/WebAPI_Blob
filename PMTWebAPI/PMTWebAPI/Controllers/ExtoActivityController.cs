using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using PMTWebAPI.DAL;

namespace PMTWebAPI.Controllers
{
    public class ExtoActivityController : ApiController
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
        [HttpPost]
        [Route("api/ExtoActivity/MeasurementUpdate")]
        public IHttpActionResult MeasurementUpdate()
        {
            bool sError = false;
            string ErrorText = "";
            string MeasurementUID = Guid.NewGuid().ToString();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "MeasurementUID=" + MeasurementUID + "&ProjectName=" + httpRequest.Params["ProjectName"] + "&WorkpackageName=" + httpRequest.Params["WorkpackageName"] + "&Tasks=" + httpRequest.Params["Tasks"] + "&Quantity=" + httpRequest.Params["Quantity"] + "&Remarks=" + httpRequest.Params["Remarks"] + "&UserEmail=" + httpRequest.Params["UserEmail"] + "&TLevel=" + httpRequest.Params["TLevel"] + "&AchievedDate=" + httpRequest.Params["AchievedDate"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    //var MeasurementUID = httpRequest.Params["MeasurementUID"];
                    //MeasurementUID = Guid.NewGuid().ToString();
                    var Quantity = httpRequest.Params["Quantity"];
                    var Remarks = httpRequest.Params["Remarks"];
                    var UserEmail = httpRequest.Params["UserEmail"];
                    var AchievedDate = httpRequest.Params["AchievedDate"];
                    if (float.TryParse(Quantity, out float Quant))
                    {
                        if (!string.IsNullOrEmpty(AchievedDate))
                        {
                            AchievedDate = db.ConvertDateFormat(AchievedDate);

                            if (DateTime.TryParse(AchievedDate, out DateTime sDate))
                            {
                                //string sDate1 = Convert.ToDateTime(AchievedDate).ToString("dd/MM/yyyy");
                                DateTime aDate = new DateTime();
                                aDate = Convert.ToDateTime(sDate);

                                var UserUID = db.GetUserUIDfrom_UserEmail(UserEmail);
                                if (!string.IsNullOrEmpty(UserUID))
                                {
                                    var ActivityUID = httpRequest.Params["ActivityUID"];
                                    if (!string.IsNullOrEmpty(ActivityUID))
                                    {
                                        if (Guid.TryParse(ActivityUID, out Guid AUID))
                                        {
                                            bool ActivityExists = db.TaskUIDExists(new Guid(ActivityUID));
                                            if (ActivityExists)
                                            {
                                                DateTime CDate1 = DateTime.Now;
                                                DataSet dsTask = db.GetTaskDetails(ActivityUID);
                                                if (dsTask.Tables[0].Rows.Count > 0)
                                                {
                                                    if (dsTask.Tables[0].Rows[0]["GroupBOQItems"].ToString() == "1" || dsTask.Tables[0].Rows[0]["GroupBOQItems"].ToString() == "2")
                                                    {
                                                        int rs = db.InsertorUpdateTaskMeasurementBook(new Guid(MeasurementUID), new Guid(ActivityUID), "", Quantity, "", CDate1, "", new Guid(UserUID), Remarks, "Y", aDate);
                                                        if (rs > 0)
                                                        {
                                                            sError = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            DataSet dsSchedule = db.GetTaskSchedule_By_TaskUID(new Guid(ActivityUID));
                                                            //Logic added  on 03 May 2022 to handle the taskschedule is not avaialble 
                                                            if (dsSchedule == null)
                                                            {
                                                                sError = true;
                                                                ErrorText = "Task Schedule doesn't exists";
                                                            }
                                                            if (dsSchedule.Tables[0].Rows.Count > 0)
                                                            {
                                                                int rs = db.InsertMeasurementBookWithoutTaskGrouping(new Guid(MeasurementUID), new Guid(ActivityUID), "", Quantity, "", aDate, "", new Guid(db.GetUserUIDfrom_UserEmail(UserEmail)), Remarks, CDate1);
                                                                if (rs > 0)
                                                                {
                                                                    sError = false;
                                                                }
                                                            }
                                                            //Logic added  on 03 May 2022 to handle the taskschedule is not avaialble 
                                                            else
                                                            {
                                                                sError = true;
                                                                ErrorText = "Task Schedule is not avaiable";
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            sError = false;
                                                            ErrorText = ex.Message;
                                                        }
                                                    }
                                                }
                                                //Logic added  on 03 May 2022 to handle the task is not avaialble 
                                                else
                                                {
                                                    sError = true;
                                                    ErrorText = "Invalid Task";
                                                }
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "ActivityUID doesn't exists.";
                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Invalid Activity UID.";
                                        }

                                    }
                                    else
                                    {
                                        var ProjectName = httpRequest.Params["ProjectName"];
                                        var WorkpackageName = httpRequest.Params["WorkpackageName"];
                                        var TaskParam = httpRequest.Params["Tasks"];
                                        var TaskLevel = httpRequest.Params["TaskLevel"];

                                        var pExists = db.ProjectNameExists(ProjectName);
                                        if (!string.IsNullOrEmpty(pExists))
                                        {
                                            string[] TaskList = TaskParam.Split('$');

                                            var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                                            if (!string.IsNullOrEmpty(wExists))
                                            {
                                                var tLevel = Convert.ToInt32(TaskLevel);
                                                if (tLevel > 0)
                                                {
                                                    string TaskUID = string.Empty;
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
                                                            sError = true;
                                                            ErrorText = "Invalid Task Name";
                                                            break;
                                                        }
                                                    }
                                                    if (!string.IsNullOrEmpty(TaskUID))
                                                    {
                                                        DateTime CDate1 = DateTime.Now;
                                                        DataSet dsTask = db.GetTaskDetails(TaskUID);
                                                        if (dsTask.Tables[0].Rows.Count > 0)
                                                        {
                                                            if (dsTask.Tables[0].Rows[0]["GroupBOQItems"].ToString() == "1" || dsTask.Tables[0].Rows[0]["GroupBOQItems"].ToString() == "2")
                                                            {
                                                                int rs = db.InsertorUpdateTaskMeasurementBook(new Guid(MeasurementUID), new Guid(TaskUID), "", Quantity, "", CDate1, "", new Guid(UserUID), Remarks, "Y", aDate);
                                                                if (rs > 0)
                                                                {
                                                                    sError = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                DataSet dsSchedule = db.GetTaskSchedule_By_TaskUID(new Guid(TaskUID));
                                                                //Logic added  on 03 May 2022 to handle the taskschedule is not avaialble 
                                                                if (dsSchedule == null)
                                                                {
                                                                    sError = true;
                                                                    ErrorText = "Task Schedule doesn't exists";
                                                                }
                                                                if (dsSchedule.Tables[0].Rows.Count > 0)
                                                                {
                                                                    int rs = db.InsertMeasurementBookWithoutTaskGrouping(new Guid(MeasurementUID), new Guid(TaskUID), "", Quantity, "", aDate, "", new Guid(db.GetUserUIDfrom_UserEmail(UserEmail)), Remarks, CDate1);
                                                                    if (rs > 0)
                                                                    {
                                                                        sError = false;
                                                                    }
                                                                }

                                                                //Logic added  on 03 May 2022 to handle the task schedule is not avaialble 
                                                                else
                                                                {
                                                                    sError = true;
                                                                    ErrorText = "Task Schedule is not avaiable";
                                                                }
                                                            }
                                                        }
                                                        //Logic added  on 03 May 2022 to handle the task is not avaialble 
                                                        else
                                                        {
                                                            sError = true;
                                                            ErrorText = "Invalid Task";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        sError = true;
                                                        ErrorText = "Invalid Task Name";
                                                    }
                                                }
                                                else
                                                {
                                                    sError = true;
                                                    ErrorText = "Invalid Task Level";

                                                }
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Workpackage Name is not found";

                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Project Name is not found";
                                        }
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "User Email doesn't exists.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Achieved Date";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Achieved Date cannot be empty.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Quantity";
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
            else
            {
                return Json(new
                {
                    Status = "Success",
                    MeasurementID = MeasurementUID,
                    Message = "Successfully Updated Activity Mesurement Book"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/MeasurementUpdatewithoutGrouping")]
        public IHttpActionResult MeasurementUpdatewithoutGrouping()
        {
            bool sError = false;
            string ErrorText = "";
            var httpRequest = HttpContext.Current.Request;
            //var MeasurementUID = httpRequest.Params["MeasurementUID"];
            var MeasurementUID = Guid.NewGuid().ToString();
           
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var ProjectName = httpRequest.Params["ProjectName"];
                    var WorkpackageName = httpRequest.Params["WorkpackageName"];
                    var TaskParam = httpRequest.Params["Tasks"];
                    var Quantity = httpRequest.Params["Quantity"];
                    var Remarks = httpRequest.Params["Remarks"];
                    var UserEmail = httpRequest.Params["UserEmail"];
                    var UnitforProgress = httpRequest.Params["UnitforProgress"];
                    var AchievedDate = httpRequest.Params["AchievedDate"];

                    //Insert
                    var BaseURL = HttpContext.Current.Request.Url.ToString();
                    string postData = "MeasurementUID=" + MeasurementUID + "&ProjectName=" + httpRequest.Params["ProjectName"] + "&WorkpackageName=" + httpRequest.Params["WorkpackageName"] + "&Tasks=" + httpRequest.Params["Tasks"] + "&Quantity=" + httpRequest.Params["Quantity"] + "&Remarks=" + httpRequest.Params["Remarks"] + "&UserEmail=" + httpRequest.Params["UserEmail"] + "&TLevel=" + httpRequest.Params["TLevel"] + "&AchievedDate=" + httpRequest.Params["AchievedDate"] + "&UnitforProgress=" + UnitforProgress;
                    db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                    if (float.TryParse(Quantity, out float Quant))
                    {
                        if (!string.IsNullOrEmpty(AchievedDate))
                        {
                            AchievedDate = db.ConvertDateFormat(AchievedDate);
                            if (DateTime.TryParse(AchievedDate, out DateTime sDate))
                            {
                                var pExists = db.ProjectNameExists(ProjectName);
                                if (!string.IsNullOrEmpty(pExists))
                                {
                                    string[] TaskList = TaskParam.Split('$');
                                    //var objects = JArray.Parse(TaskParam); // parse as array 
                                    //var result = JsonConvert.DeserializeObject<RootObject>(TaskParam);

                                    var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                                    if (!string.IsNullOrEmpty(wExists))
                                    {
                                        var tLevel = Convert.ToInt32(httpRequest.Params["TLevel"]);
                                        if (tLevel > 0)
                                        {
                                            string TaskUID = string.Empty;
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
                                                    sError = true;
                                                    ErrorText = "Invalid Task Name";
                                                    break;
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(TaskUID))
                                            {
                                                DateTime CDate1 = DateTime.Now;

                                                //string sDate1 = Convert.ToDateTime(AchievedDate).ToString("dd/MM/yyyy");
                                                DateTime aDate = new DateTime();
                                                //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                                //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                                aDate = Convert.ToDateTime(sDate);

                                                int rs = db.InsertMeasurementBookWithoutTaskGrouping(new Guid(MeasurementUID), new Guid(TaskUID), UnitforProgress, Quantity, "", aDate, "", new Guid(db.GetUserUIDfrom_UserEmail(UserEmail)), Remarks, CDate1);
                                                if (rs > 0)
                                                {
                                                    sError = false;
                                                }
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Invalid Task Name";
                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Invalid Task Level";

                                        }
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Workpackage Name is not found";

                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Project Name is not found";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Selected Date Format";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Achieved Date cannot be empty";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Quantity";
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
            else
            {
                return Json(new
                {
                    Status = "Success",
                    MeasurementID = MeasurementUID,
                    Message = "Successfully Updated Mesurement Book"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/MeasurementEdit")]
        public IHttpActionResult MeasurementEdit()
        {
            var httpRequest = HttpContext.Current.Request;
            var MeasurementUID = httpRequest.Params["MeasurementUID"];
            var Quantity = httpRequest.Params["Quantity"];
            var Remarks = httpRequest.Params["Remarks"];
            var UserEmail = httpRequest.Params["UserEmail"];
            var MeasurementDate = httpRequest.Params["MeasurementDate"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "MeasurementUID=" + MeasurementUID + "&Quantity=" + Quantity + "&Remarks=" + Remarks + "&UserEmail=" + UserEmail + "&MeasurementDate=" + MeasurementDate;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(MeasurementUID, out Guid MUID))
                    {
                        if (float.TryParse(Quantity, out float Quant))
                        {
                            if (!string.IsNullOrEmpty(MeasurementDate))
                            {
                                MeasurementDate = db.ConvertDateFormat(MeasurementDate);
                                if (DateTime.TryParse(MeasurementDate, out DateTime sDate))
                                {
                                    string UserGUID = db.GetUserUIDfrom_UserEmail(UserEmail);
                                    if (!string.IsNullOrEmpty(UserGUID))
                                    {
                                        DataSet ds = db.GetMeasurementBook_By_UID(new Guid(MeasurementUID));
                                        if (ds.Tables[0].Rows.Count > 0)
                                        {
                                            int rs = db.MeasurementBookUpdate(new Guid(MeasurementUID), new Guid(ds.Tables[0].Rows[0]["TaskUID"].ToString()), ds.Tables[0].Rows[0]["UnitforProgress"].ToString(), Quantity, "", sDate, "", new Guid(), Remarks);
                                            if (rs > 0)
                                            {
                                                sError = false;

                                                rs = db.ServerFlagsUpdate(MeasurementUID, 2, "MeasurementBook", "Y", "UID");
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Measurement book not updated";
                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Measurement ID doesn't exists";
                                        }
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "UserEmail cannot be empty";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Invalid Measurement Date";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Measurement Date";
                            }
                            
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid Quantity";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Measurement ID";
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
            else
            {
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Updated Mesurement Book"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/MeasurementDelete")]
        public IHttpActionResult MeasurementDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var MeasurementUID = httpRequest.Params["MeasurementUID"];
            var UserName = httpRequest.Params["UserName"];
            var UserUID = "";
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "MeasurementUID=" + MeasurementUID + "&UserUID=" + UserName;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(MeasurementUID, out Guid MUID))
                    {
                        DataSet dsUsers = db.getUserDetails_by_EmailID(UserName);
                        if (dsUsers.Tables[0].Rows.Count > 0)
                        {
                            UserUID = dsUsers.Tables[0].Rows[0]["UserUID"].ToString();

                        }
                        else
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:UserName not exists"
                            });
                        }
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataSet ds = db.GetMeasurementBook_By_UID(MUID);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int cnt = db.Measurement_Delete(MUID, new Guid(UserUID));
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(MeasurementUID, 2, "MeasurementBook", "Y", "UID");
                                    if (cnt > 0)
                                    {
                                        sError = false;
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Measurement ID doesn't exists";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Measurement book not deleted.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid UserUID";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Measurement ID";
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
                ErrorText = "Error : " + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Measurement."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/PlannedResourceDeploymentAddorEdit")]
        public IHttpActionResult ResourceDeploymentAddorEdit()
        {
            var httpRequest = HttpContext.Current.Request;
            var ResourceUID = Guid.NewGuid().ToString();

            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var ResourceName = httpRequest.Params["ResourceName"];
            var sMonth = httpRequest.Params["sMonth"];
            var dType = httpRequest.Params["DeploymentType"];
            var Planned = httpRequest.Params["PlannedDeployment"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ResourceUID=" + ResourceUID + "&ProjectName=" + ProjectName + "&WorkpackageName=" + WorkpackageName + "&ResourceName=" + ResourceName + "&sMonth=" + sMonth + "&DeploymentType=" + dType + "&PlannedDeployment=" + Planned;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var pExists = db.ProjectNameExists(ProjectName);
                    if (!string.IsNullOrEmpty(pExists))
                    {
                        var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                        if (!string.IsNullOrEmpty(wExists))
                        {
                            var rUID = db.RsourceNameExists(ResourceName, wExists);
                            if (!string.IsNullOrEmpty(rUID))
                            {
                                if (!string.IsNullOrEmpty(sMonth))
                                {
                                    sMonth = db.ConvertDateFormat(sMonth);
                                    if (DateTime.TryParse(sMonth, out DateTime sDate))
                                    {
                                        DateTime d = sDate;
                                        string sDate1 = "", sDate2 = "";
                                        DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now;
                                        sDate1 = d.ToString("dd/MM/yyyy");
                                        sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        CDate1 = Convert.ToDateTime(sDate1);

                                        int Days = DateTime.DaysInMonth(Convert.ToInt32(d.Year), Convert.ToInt32(d.Month)) - 1;
                                        CDate2 = CDate1.AddDays(Days);

                                        if (float.TryParse(Planned, out float p))
                                        {
                                            int sresult = db.InsertorUpdateResourceDeploymentPlanned(new Guid(ResourceUID), new Guid(wExists), new Guid(rUID), CDate1, CDate2, "Month", p, DateTime.Now, "Y");
                                            if (sresult != 0)
                                            {
                                                sError = false;
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Resource Planned not updated.";
                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Invalid Planned Value.";
                                        }
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Invalid Datetime format.";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Resource Name is not found";
                                }
                                
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Resource Name not found.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Workpackage Name is not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Project Name is not found";
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
                ErrorText = "Error : " + ex.Message;
            }
            

            if (sError)
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri;

                int cnt = db.WebAPIStatusInsert(Guid.NewGuid(), url, httpRequest.Params.ToString(), ErrorText, "Failure");
                if (cnt <= 0)
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "WebAPI Status table insert failed"
                    });
                }
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error" + ErrorText
                });
            }
            else
            {
                return Json(new
                {
                    ResourceUID= ResourceUID,
                    Status = "Success",
                    Message = "Successfully Add Resource Deployment"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/ResourceDeploymentDelete")]
        public IHttpActionResult ResourceDeploymentDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var UID = httpRequest.Params["UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "UID=" + UID + "&UserUID=" + UserUID;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(UID, out Guid uid))
                    {
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataSet ds = db.GetResourceDeployment_by_ReourceDeploymentUID(uid);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int cnt = db.ResourceDeploymentUpdate_Delete(uid, new Guid(UserUID));
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(UID, 2, "ResourceDeployment", "Y", "ReourceDeploymentUID");
                                    if (cnt > 0)
                                    {
                                        cnt = db.ServerFlagsUpdate(UID, 2, "ResourceDeploymentUpdate", "Y", "ReourceDeploymentUID");

                                        sError = false;
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Resource Deployment not deleted. Contact system admin";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Resource Deployment UID not found";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid User UID";
                        }
                            
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid ResourceDeployment UID.";
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
                ErrorText = "Error : " + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Resource Deployment."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/ActualResourceDeploymentUpdate")]
        public IHttpActionResult ResourceDeploymentUpdate()
        {
            var httpRequest = HttpContext.Current.Request;
            //var UID = httpRequest.Params["UID"];
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var ResourceName = httpRequest.Params["ResourceName"];
            //var sMonth = httpRequest.Params["sMonth"];          
            var Deployed = httpRequest.Params["ActualDeployment"];
            var Remarks = httpRequest.Params["Remarks"];
            var DeployedDate = httpRequest.Params["DeployedDate"];
            bool sError = false;
            string ErrorText = "";
            string UID = Guid.NewGuid().ToString();
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "UID=" + UID + "&ProjectName=" + ProjectName + "&WorkpackageName=" + WorkpackageName + "&ResourceName=" + ResourceName + "&DeployedResource=" + Deployed + "&Remarks=" + Remarks + "&DeployedDate=" + DeployedDate;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var pExists = db.ProjectNameExists(ProjectName);
                    
                    if (!string.IsNullOrEmpty(pExists))
                    {
                        var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                        if (!string.IsNullOrEmpty(wExists))
                        {
                            var rUID = db.RsourceNameExists(ResourceName, wExists);
                            if (!string.IsNullOrEmpty(rUID))
                            {
                                if (!string.IsNullOrEmpty(DeployedDate))
                                {
                                    DeployedDate = db.ConvertDateFormat(DeployedDate);
                                    if (DateTime.TryParse(DeployedDate, out DateTime sDate))
                                    {
                                        DateTime d = sDate;
                                        string sDate1 = "", sDate2 = "";
                                        DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now;
                                        sDate1 = d.ToString("dd/MM/yyyy");
                                        sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        CDate1 = Convert.ToDateTime(sDate1);

                                        if (float.TryParse(Deployed, out float p))
                                        {

                                            int sresult = db.ResourceDeployment_Update(new Guid(UID), new Guid(wExists), new Guid(rUID), p, CDate1, Remarks, "Y");
                                            if (sresult > 0)
                                            {
                                                sError = false;
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Resource Deployment not updated.";
                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Invalid Deployed Value.";
                                        }
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Invalid Deployed Datetime format.";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Deployed Datetime cannot be empty.";
                                }
                                    
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Resource Name is not found";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Workpackage Name is not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Project Name is not found";
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
                ErrorText = "Error : " + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    UID= UID,
                    Message = "Successfully Updated Resource Deployment"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/ResourceDeploymentUpdateDelete")]
        public IHttpActionResult ResourceDeploymentUpdateDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var UID = httpRequest.Params["UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "UID=" + UID + "&UserUID=" + UserUID;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(UID, out Guid uid))
                    {
                        DataSet ds = db.GetResourceDeploymentUpdate_by_UID(uid);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int cnt = db.ResourceDeploymentUpdate_Delete(uid, new Guid(UserUID));
                            if (cnt > 0)
                            {
                                cnt = db.ServerFlagsUpdate(UID, 2, "ResourceDeploymentUpdate", "Y", "UID");
                                if (cnt > 0)
                                {
                                    sError = false;
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Resource Deployment update not deleted. Contact system admin";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Resource Deployment update UID not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Resource Deployment update UID.";
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
                ErrorText = "Error : " + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Resource Deployment update."
                });
            }
        }

        //[Authorize]
        //[HttpPost]
        //[Route("api/ExtoActivity/AddJointInspectionReport")]
        //public IHttpActionResult AddJointInspectionReport()
        //{
        //    var httpRequest = HttpContext.Current.Request;
        //    var inspectionUid = Guid.NewGuid().ToString();
        //    var ProjectName = httpRequest.Params["ProjectName"];
        //    var BOQUID = httpRequest.Params["BOQUID"];
        //    var DiaPipe = httpRequest.Params["DiaPipe"];
        //    var PipeNumber = httpRequest.Params["PipeNumber"];
        //    var invoice_number = httpRequest.Params["Invoice_number"];
        //    var invoicedate = httpRequest.Params["Invoicedate"];
        //    var unit = httpRequest.Params["unit"];
        //    var quantity = httpRequest.Params["quantity"];
        //    var Inspection_Type = httpRequest.Params["InspectionType"];
        //    //var ItemLevel = httpRequest.Params["ItemLevel"];
        //    var StartingPointVal = httpRequest.Params["StartingPoint"];
        //    var LenghtVal = httpRequest.Params["Lenght"];
        //    var Chainage_Number = httpRequest.Params["Chainage_Number"];
        //    var Chainage_Desc = httpRequest.Params["Chainage_Desc"];
        //    var Qty_in_RMTVal = httpRequest.Params["Qty_in_RMT"];
        //    var Qty_for_UnitVal = httpRequest.Params["Qty_for_Unit"];
        //    //var Deductions = httpRequest.Params["Deductions"];
        //    var Remarks = httpRequest.Params["Remarks"];
        //    bool sError = false;
        //    string ErrorText = "";
        //    //var rUID = db.BOQNameExists(BOQName, wExists);
        //    try
        //    {
        //        string postData = "inspectionUid=" + inspectionUid + "&ProjectName=" + ProjectName + "&BOQUID=" + BOQUID + "&DiaPipe=" + DiaPipe + "&PipeNumber=" + PipeNumber + "&Invoice_number=" + invoice_number + "&Invoicedate=" + invoicedate + "&unit=" + unit + "&quantity=" + quantity + "&InspectionType=" + Inspection_Type + "&StartingPoint=" + StartingPointVal + "&Lenght=" + LenghtVal + "&Chainage_Number=" + Chainage_Number + "&Chainage_Desc=" + Chainage_Desc + "&Qty_in_RMT=" + Qty_in_RMTVal + "&Qty_for_Unit=" + Qty_for_UnitVal + "&Remarks=" + Remarks;
        //        var BaseURL = HttpContext.Current.Request.Url.ToString();
        //        db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

        //        var identity = (ClaimsIdentity)User.Identity;
        //        if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
        //        {
        //            if (!string.IsNullOrEmpty(inspectionUid))
        //            {
        //                var pExists = db.ProjectNameExists(ProjectName);
        //                if (!string.IsNullOrEmpty(pExists))
        //                {
        //                    //if (BOQUID != "")
        //                    if (Guid.TryParse(BOQUID, out Guid boquid))
        //                    {
        //                        string sDate1 = "";
        //                        DateTime CDate1 = DateTime.Now;
        //                        if (!string.IsNullOrEmpty(invoicedate))
        //                        {
        //                            invoicedate = db.ConvertDateFormat(invoicedate);
        //                            CDate1 = Convert.ToDateTime(invoicedate);
        //                        }
        //                        else
        //                        {
        //                            sDate1 = CDate1.ToString("dd/MM/yyyy");
        //                            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
        //                            sDate1 = db.ConvertDateFormat(sDate1);
        //                            CDate1 = Convert.ToDateTime(sDate1);
        //                        }
                                
        //                        float StartingPoint = 0;
        //                        float Length = 0;
        //                        float Qty_in_RMT = 0;
        //                        double Qty_for_Unit = 0;
        //                        double Deductions = 0;
        //                        if (Inspection_Type == "Laying & Jointing")
        //                        {
        //                            StartingPoint = float.Parse(StartingPointVal);
        //                            Length = float.Parse(LenghtVal);
        //                        }
        //                        else if (Inspection_Type == "Guniting" || Inspection_Type == "Epoxy")
        //                        {
        //                            string DiaofPipe_in_mm = db.GetBOQDiaofPipe_by_BOQDetailsUID(new Guid(BOQUID));
        //                            if (DiaofPipe_in_mm == "Error1:" || DiaofPipe_in_mm == "")
        //                            {
        //                                DiaofPipe_in_mm = "0";
        //                            }

        //                            Qty_in_RMT = float.Parse(Qty_in_RMTVal);
        //                            Qty_for_Unit = Math.Round(Convert.ToDouble(Qty_in_RMTVal) * Convert.ToDouble(3.14159) * Convert.ToDouble(DiaofPipe_in_mm), 2);
        //                            Deductions = float.Parse(quantity) * 0.3;

        //                        }

        //                        int sresult = db.InsertjointInspection(new Guid(inspectionUid), new Guid(BOQUID), DiaPipe, unit, invoice_number, CDate1.ToString(), quantity, Inspection_Type, Chainage_Number, Chainage_Desc, StartingPoint, Length, Qty_in_RMT, Qty_for_Unit, Deductions, Remarks, new Guid(pExists), PipeNumber, "Y");
        //                        if (sresult > 0)
        //                        {
        //                            sError = false;
        //                        }
        //                        else
        //                        {
        //                            sError = true;
        //                            ErrorText = "Joint Inspection Data not updated.";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sError = true;
        //                        ErrorText = "BOQ Name is not found";
        //                    }
        //                }
        //                else
        //                {
        //                    sError = true;
        //                    ErrorText = "Project Name is not found";
        //                }
        //            }
        //            else
        //            {
        //                sError = true;
        //                ErrorText = "Invalid Inspection UID.";
        //            }
        //        }
        //        else
        //        {
        //            sError = true;
        //            ErrorText = "Not Authorized IP address.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sError = true;
        //        ErrorText = "Error:" + ex.Message;
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
        //        return Json(new
        //        {
        //            Status = "Success",
        //            InspectionUID= inspectionUid,
        //            Message = "Successfully Added Joint Inspection"
        //        });
        //    }
        //}

            //added on 22/01/2022 for Arun changes
        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/AddJointInspectionReport")]
        public IHttpActionResult AddJointInspectionReport()
        {
            var httpRequest = HttpContext.Current.Request;
            var inspectionUid = Guid.NewGuid().ToString();
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var BOQItems = httpRequest.Params["BOQItems"];
            //var pExists1 = db.ProjectNameExists(ProjectName);
            //if (!string.IsNullOrEmpty(pExists1))
            //{
            //    var wExists = db.WorkpackageNameExists(WorkpackageName, pExists1);
            //    if (!string.IsNullOrEmpty(wExists))
            //    {

            //    }
            //}
            //var BOQUID = httpRequest.Params["BOQUID"];
            var DiaPipe = httpRequest.Params["DiaPipe"];
            var PipeNumber = httpRequest.Params["PipeNumber"];
            var invoice_number = httpRequest.Params["Invoice_number"];
            var invoicedate = httpRequest.Params["Invoicedate"];
            var unit = httpRequest.Params["unit"];
            //added on 13/05/2022
            if (httpRequest.Params["quantity"] == null)
            {
                var resultmain = new
                {
                    Status = "Failure",
                    Message = "Please Enter quantity !",
                };
                return Json(resultmain);
            }
            else
            {
                if (string.IsNullOrWhiteSpace((httpRequest.Params["quantity"].ToString())))
                {
                    var resultmain = new
                    {
                        Status = "Failure",
                        Message = "quantity cannot be Empty !",
                    };
                    return Json(resultmain);
                }
            }
            //
            var quantity = httpRequest.Params["quantity"];
            var Inspection_Type = httpRequest.Params["InspectionType"];
            //var ItemLevel = httpRequest.Params["ItemLevel"];
            var StartingPointVal = httpRequest.Params["StartingPoint"];
            var LenghtVal = httpRequest.Params["Lenght"];
            var Chainage_Number = httpRequest.Params["Chainage_Number"];
            var width = httpRequest.Params["width"];
            var number = httpRequest.Params["Number"];
            var depth = httpRequest.Params["depth"];
            var Chainage_Desc = httpRequest.Params["Chainage_Desc"];
            var Qty_in_RMTVal = httpRequest.Params["Qty_in_RMT"];
            var Qty_for_UnitVal = httpRequest.Params["Qty_for_Unit"];
            //var Deductions = httpRequest.Params["Deductions"];
            var Remarks = httpRequest.Params["Remarks"];
            bool sError = false;
            string ErrorText = "";
            //var rUID = db.BOQNameExists(BOQName, wExists);
            try
            {
                string postData = "inspectionUid=" + inspectionUid + "&ProjectName=" + ProjectName + "&BOQItems=" + BOQItems + "&DiaPipe=" + DiaPipe + "&PipeNumber=" + PipeNumber + "&Invoice_number=" + invoice_number + "&Invoicedate=" + invoicedate + "&unit=" + unit + "&quantity=" + quantity + "&InspectionType=" + Inspection_Type + "&StartingPoint=" + StartingPointVal + "&Lenght=" + LenghtVal + "&Chainage_Number=" + Chainage_Number + "&Chainage_Desc=" + Chainage_Desc + "&Qty_in_RMT=" + Qty_in_RMTVal + "&Qty_for_Unit=" + Qty_for_UnitVal + "&Remarks=" + Remarks;
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (!string.IsNullOrEmpty(inspectionUid))
                    {
                        var pExists = db.ProjectNameExists(ProjectName);
                        if (!string.IsNullOrEmpty(pExists))
                        {
                            //if (BOQUID != "")
                            var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                            if (!string.IsNullOrEmpty(wExists))
                            {
                                string BOQUID = string.Empty;
                                string ParentBOQUID = "";
                                string[] bList = BOQItems.Split('$');
                                for (int i = 0; i < bList.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        BOQUID = db.GetBOQUID_FromName(new Guid(wExists), bList[i], "Workpackage");
                                        ParentBOQUID = BOQUID;
                                    }
                                    else
                                    {
                                        BOQUID = db.GetBOQUID_FromName(new Guid(ParentBOQUID), bList[i], "BOQItem");
                                        ParentBOQUID = BOQUID;
                                    }
                                }

                                if (Guid.TryParse(BOQUID, out Guid boquid))
                                {
                                    string sDate1 = "";
                                    DateTime CDate1 = DateTime.Now;
                                    if (!string.IsNullOrEmpty(invoicedate))
                                    {
                                        invoicedate = db.ConvertDateFormat(invoicedate);
                                        CDate1 = Convert.ToDateTime(invoicedate);
                                    }
                                    else
                                    {
                                        sDate1 = CDate1.ToString("dd/MM/yyyy");
                                        //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        sDate1 = db.ConvertDateFormat(sDate1);
                                        CDate1 = Convert.ToDateTime(sDate1);
                                    }

                                    float StartingPoint = 0;
                                    float Length = 0;
                                    float Qty_in_RMT = 0;
                                    double Qty_for_Unit = 0;
                                    double Deductions = 0;
                                   

                                    if (Inspection_Type == "Laying & Jointing")
                                    {
                                        StartingPoint = float.Parse(StartingPointVal);
                                        Length = float.Parse(LenghtVal);
                                    }
                                    else if (Inspection_Type == "Guniting" || Inspection_Type == "Epoxy")
                                    {
                                        string DiaofPipe_in_mm = db.GetBOQDiaofPipe_by_BOQDetailsUID(new Guid(BOQUID));
                                        if (DiaofPipe_in_mm == "Error1:" || DiaofPipe_in_mm == "")
                                        {
                                            return Json(new
                                            {
                                                Status = "Failure",
                                                Message = "Error: For Guniting or Epoxy Dia of Pipe is not present in BOQ"
                                            });
                                           // DiaofPipe_in_mm = "0";

                                        }
                                        //added on 13/05/2022
                                        if (httpRequest.Params["Qty_in_RMT"] == null)
                                        {
                                            var resultmain = new
                                            {
                                                Status = "Failure",
                                                Message = "Please Enter Qty_in_RMT For Guniting or Epoxy!",
                                            };
                                            return Json(resultmain);
                                        }
                                        else
                                        {
                                            if (string.IsNullOrWhiteSpace((httpRequest.Params["Qty_in_RMT"].ToString())))
                                            {
                                                var resultmain = new
                                                {
                                                    Status = "Failure",
                                                    Message = "Qty_in_RMT cannot be Empty For Guniting or Epoxy !",
                                                };
                                                return Json(resultmain);
                                            }
                                        }
                                        //
                                        Qty_in_RMT = float.Parse(Qty_in_RMTVal);
                                        if(!string.IsNullOrWhiteSpace(Qty_for_UnitVal))
                                        {
                                            Qty_for_Unit = Math.Round(Convert.ToDouble(Qty_for_UnitVal), 2);
                                        }
                                        else
                                        {
                                            Qty_for_Unit = Math.Round(Convert.ToDouble(Qty_in_RMTVal) * Convert.ToDouble(3.14159) * Convert.ToDouble(DiaofPipe_in_mm), 2);

                                        }
                                        //
                                        Deductions = float.Parse(quantity) * 0.3;

                                    }

                                    else if(Inspection_Type.ToLower() == "earthwork/excavation")
                                    {
                                        if(number == null || LenghtVal == null || DiaPipe == null || width == null || depth == null)
                                        {
                                            var resultmain = new
                                            {
                                                Status = "Failure",
                                                Message = "Number/Length/Diapipe/width/depth cannot be Empty For EarthWork/Excavation !",
                                            };
                                            return Json(resultmain);
                                        }
                                    }
                                    else if (Inspection_Type.ToLower() == "barricading")
                                    {
                                        
                                        if (number == null || LenghtVal == null )
                                        {
                                            var resultmain = new
                                            {
                                                Status = "Failure",
                                                Message = "Number/Length cannot be Empty For Barricading !",
                                            };
                                            return Json(resultmain);
                                        }
                                    }
                                    else if (Inspection_Type.ToLower() == "dewatering the sewerage")
                                    {
                                        if (number == null || LenghtVal == null )
                                        {
                                            var resultmain = new
                                            {
                                                Status = "Failure",
                                                Message = "Number/Length cannot be Empty For DeWatering the Sewage !",
                                            };
                                            return Json(resultmain);
                                        }
                                    }
                                    else if (Inspection_Type.ToLower() == "shoring and strutting")
                                    {

                                        if (number == null || LenghtVal == null )
                                        {
                                            var resultmain = new
                                            {
                                                Status = "Failure",
                                                Message = "Number/Length cannot be Empty For Shoring and Strutting !",
                                            };
                                            return Json(resultmain);
                                        }
                                    }
                                    else if(Inspection_Type.ToLower() != "supply"  )
                                    {
                                        var resultmain = new
                                        {
                                            Status = "Failure",
                                            Message = "Invalid Inspection Type!",
                                        };
                                        return Json(resultmain);
                                    }

                                 //   int sresult = db.InsertjointInspection(new Guid(inspectionUid), new Guid(BOQUID), DiaPipe, unit, invoice_number, CDate1.ToString(), quantity, Inspection_Type, Chainage_Number, Chainage_Desc, StartingPoint, Length, Qty_in_RMT, Qty_for_Unit, Deductions, Remarks, new Guid(pExists), PipeNumber, "Y");

                                    int sresult = db.InsertjointInspection(new Guid(inspectionUid), new Guid(BOQUID), DiaPipe, unit, invoice_number, CDate1.ToString(), quantity,Inspection_Type, Chainage_Number, Chainage_Desc, StartingPoint, Length, Qty_in_RMT, Qty_for_Unit, Deductions, Remarks, new Guid(pExists), PipeNumber, number, LenghtVal,width, depth,new Guid(wExists)); 
                                    if (sresult > 0)
                                    {
                                        sError = false;
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Joint Inspection Data not updated.";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Invalid BOQ Items";
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
                            ErrorText = "Project Name is not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Inspection UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    InspectionUID = inspectionUid,
                    Message = "Successfully Added Joint Inspection"
                });
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/EditJointInspectionReport")]
        public IHttpActionResult EditJointInspectionReport()
        {
            var httpRequest = HttpContext.Current.Request;
            var inspectionUid = httpRequest.Params["inspectionUid"];
            var ProjectName = httpRequest.Params["ProjectName"];
            var BOQUID = httpRequest.Params["BOQUID"];
            var DiaPipe = httpRequest.Params["DiaPipe"];
            var PipeNumber = httpRequest.Params["PipeNumber"];
            var invoice_number = httpRequest.Params["Invoice_number"];
            var invoicedate = httpRequest.Params["Invoicedate"];
            var unit = httpRequest.Params["unit"];
            var quantity = httpRequest.Params["quantity"];
            var Inspection_Type = httpRequest.Params["InspectionType"];
            //var ItemLevel = httpRequest.Params["ItemLevel"];
            var StartingPointVal = httpRequest.Params["StartingPoint"];
            var LenghtVal = httpRequest.Params["Lenght"];
            var Chainage_Number = httpRequest.Params["Chainage_Number"];
            var Chainage_Desc = httpRequest.Params["Chainage_Desc"];
            var Qty_in_RMTVal = httpRequest.Params["Qty_in_RMT"];
            var Qty_for_UnitVal = httpRequest.Params["Qty_for_Unit"];
            //var Deductions = httpRequest.Params["Deductions"];
            var Remarks = httpRequest.Params["Remarks"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                string postData = "inspectionUid=" + inspectionUid + "&ProjectName=" + ProjectName + "&BOQUID=" + BOQUID + "&DiaPipe=" + DiaPipe + "&PipeNumber=" + PipeNumber + "&Invoice_number=" + invoice_number + "&Invoicedate=" + invoicedate + "&unit=" + unit + "&quantity=" + quantity + "&InspectionType=" + Inspection_Type + "&StartingPoint=" + StartingPointVal + "&Lenght=" + LenghtVal + "&Chainage_Number=" + Chainage_Number + "&Chainage_Desc=" + Chainage_Desc + "&Qty_in_RMT=" + Qty_in_RMTVal + "&Qty_for_Unit=" + Qty_for_UnitVal + "&Remarks=" + Remarks;
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    //var rUID = db.BOQNameExists(BOQName, wExists);
                    if (Guid.TryParse(inspectionUid, out Guid InspectionUID))
                    {
                        var pExists = db.ProjectNameExists(ProjectName);
                        if (!string.IsNullOrEmpty(pExists))
                        {
                            if (Guid.TryParse(BOQUID, out Guid boquid))
                            {
                                string sDate1 = "";
                                DateTime CDate1 = DateTime.Now;
                                if (!string.IsNullOrEmpty(invoicedate))
                                {
                                    invoicedate = db.ConvertDateFormat(invoicedate);
                                    CDate1 = Convert.ToDateTime(invoicedate);
                                }
                                else
                                {
                                    sDate1 = CDate1.ToString("dd/MM/yyyy");
                                    //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                    sDate1 = db.ConvertDateFormat(sDate1);
                                    CDate1 = Convert.ToDateTime(sDate1);
                                }

                                float StartingPoint = 0;
                                float Length = 0;
                                float Qty_in_RMT = 0;
                                double Qty_for_Unit = 0;
                                double Deductions = 0;
                                if (Inspection_Type == "Laying & Jointing")
                                {
                                    StartingPoint = float.Parse(StartingPointVal);
                                    Length = float.Parse(LenghtVal);
                                }
                                else if (Inspection_Type == "Guniting" || Inspection_Type == "Epoxy")
                                {
                                    string DiaofPipe_in_mm = db.GetBOQDiaofPipe_by_BOQDetailsUID(new Guid(BOQUID));
                                    if (DiaofPipe_in_mm == "Error1:" || DiaofPipe_in_mm == "")
                                    {
                                        DiaofPipe_in_mm = "0";
                                    }

                                    Qty_in_RMT = float.Parse(Qty_in_RMTVal);
                                    Qty_for_Unit = Math.Round(Convert.ToDouble(Qty_in_RMTVal) * Convert.ToDouble(3.14159) * Convert.ToDouble(DiaofPipe_in_mm), 2);
                                    Deductions = float.Parse(quantity) * 0.3;

                                }

                                int sresult = db.InsertjointInspection(new Guid(inspectionUid), new Guid(BOQUID), DiaPipe, unit, invoice_number, CDate1.ToString(), quantity, Inspection_Type, Chainage_Number, Chainage_Desc, StartingPoint, Length, Qty_in_RMT, Qty_for_Unit, Deductions, Remarks, new Guid(pExists), PipeNumber, "Y");
                                if (sresult > 0)
                                {
                                    sError = false;
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Joint Inspection Data not updated.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid BOQ UID";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Project Name is not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Inspection UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Updated Joint Inspection"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/DeleteJointInspection")]
        public IHttpActionResult DeleteJointInspection()
        {
            var httpRequest = HttpContext.Current.Request;
            var UID = httpRequest.Params["UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";

            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "UID=" + UID + "&UserUID=" + UserUID;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(UID, out Guid uid))
                    {
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataTable ds = db.getJointInspection_by_inspectionUid(UID);
                            if (ds.Rows.Count > 0)
                            {
                                int cnt = db.JointInspection_Delete(uid, new Guid(UserUID));
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(UID, 2, "JointInspection", "Y", "inspectionUid");
                                    if (cnt > 0)
                                    {
                                        cnt = db.ServerFlagsUpdate(UID, 2, "JointInspectionDocuments", "Y", "inspectionUid");
                                        if (cnt > 0)
                                        {
                                            sError = false;
                                        }
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Joint Inspection UID not found.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Joint Inspection UID.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Joint Inspection UID not found.";
                        }

                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Joint Inspection UID";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Joint Inspection."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/AddJointInspectionDocument")]
        public IHttpActionResult AddJointInspectionDocument()
        {
            var httpRequest = HttpContext.Current.Request;
            var InspectionDocumentUID = Guid.NewGuid().ToString();
            var inspectionUid = httpRequest.Params["inspectionUid"];
            var FileName = httpRequest.Params["FileName"];
            var Extension = httpRequest.Params["Extension"];
            var UploadFilePhysicalpath = httpRequest.Params["UploadFilePhysicalpath"];
            var RelativePath = httpRequest.Params["RelativePath"];

            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "InspectionDocumentUID=" + InspectionDocumentUID + "&inspectionUid=" + inspectionUid + "&FileName=" + FileName + "&Extension=" + Extension + "&UploadFilePhysicalpath=" + UploadFilePhysicalpath + "&RelativePath=" + RelativePath;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(inspectionUid, out Guid iuid))
                    {
                        for (int i = 0; i < httpRequest.Files.Count; i++)
                        {
                            HttpPostedFile httpPostedFile = httpRequest.Files[i];

                            string sDocumentPath = string.Empty;
                            sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + RelativePath;

                            if (!Directory.Exists(sDocumentPath))
                            {
                                Directory.CreateDirectory(sDocumentPath);
                            }

                            string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                            string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                            httpPostedFile.SaveAs(sDocumentPath + sFileName + "_1" + Extn);

                            int count = db.InsertJointInspectionDocuments(new Guid(InspectionDocumentUID), new Guid(inspectionUid), sFileName, Extn, UploadFilePhysicalpath, "Y");
                            if (count > 0)
                            {
                                sError = false;
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Inspection Report Document not Updated";
                            }
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Inspection UID.";
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
                ErrorText = "Error:"+ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    InspectionDocumentUID= InspectionDocumentUID,
                    Message = "Successfully Added Inspection Report Document."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/JointInspectionDocumentDelete")]
        public IHttpActionResult JointInspectionDocumentDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var InspectionDocumentUID = httpRequest.Params["InspectionDocumentUID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;

            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "InspectionDocumentUID=" + InspectionDocumentUID + "&UserUID=" + UserUID;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(InspectionDocumentUID, out Guid uid))
                    {
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataSet ds = db.GetJointInspectionDocuments_by_InspectionDocumentUID(new Guid(InspectionDocumentUID));
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int cnt = db.JointInspectionDocument_Delete(uid, useruid);
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(InspectionDocumentUID, 2, "JointInspectionDocuments", "Y", "InspectionDocumentUID");
                                    if (cnt > 0)
                                    {
                                        sError = false;
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Joint Inspection Document not deleted.. Please contact system admin.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Joint Inspection Document UID not found.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid User UID.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Inspection Document UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Bank Guarantee Document."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/AddorUpdateBankGuarantee")]
        public IHttpActionResult AddorUpdateBankGuarantee()
        {
            var httpRequest = HttpContext.Current.Request;
            var Bank_GuaranteeUID = Guid.NewGuid().ToString();
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var Vendor_Name = httpRequest.Params["Vendor_Name"];
            //var sMonth = httpRequest.Params["sMonth"];          
            var Vendor_Address = httpRequest.Params["Vendor_Address"];
            var Amount = httpRequest.Params["Amount"];
            var Validity = httpRequest.Params["Validity"];
            var Date_of_Guarantee = httpRequest.Params["Date_of_Guarantee"];
            var No_of_Collaterals = httpRequest.Params["No_of_Collaterals"];
            var Bank_Name = httpRequest.Params["Bank_Name"];
            var Bank_Branch = httpRequest.Params["Bank_Branch"];
            var IFSC_Code = httpRequest.Params["IFSC_Code"];
            var BG_Number = httpRequest.Params["BG_Number"];
            var Date_of_Expiry = httpRequest.Params["Date_of_Expiry"];
            var Currency = httpRequest.Params["Currency"];
            var Currency_CultureInfo = httpRequest.Params["Currency_CultureInfo"];
            var Bank_Address = httpRequest.Params["Bank_Address"];
            var Cliam_Date = httpRequest.Params["Cliam_Date"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "Bank_GuaranteeUID=" + Bank_GuaranteeUID + "&ProjectName=" + ProjectName + "&WorkpackageName=" + WorkpackageName + "&Vendor_Name=" + Vendor_Name + "&Vendor_Address=" + Vendor_Address + "&Amount=" + Amount +
                    "&Validity=" + Validity + "&Date_of_Guarantee=" + Date_of_Guarantee + "&No_of_Collaterals=" + No_of_Collaterals + "&Bank_Name=" + Bank_Name + "&Bank_Branch=" + Bank_Branch + "&IFSC_Code=" + IFSC_Code +
                    "&BG_Number=" + BG_Number + "&Date_of_Expiry=" + Date_of_Expiry + "&Currency=" + Currency + "&Currency_CultureInfo=" + Currency_CultureInfo + "&Bank_Address=" + Bank_Address + "&Cliam_Date=" + Cliam_Date;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");


                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (!string.IsNullOrEmpty(BG_Number))
                    {
                        var pExists = db.ProjectNameExists(ProjectName);
                        if (!string.IsNullOrEmpty(pExists))
                        {
                            var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                            if (!string.IsNullOrEmpty(wExists))
                            {
                                if (!string.IsNullOrEmpty(Date_of_Guarantee))
                                {
                                    Date_of_Guarantee = db.ConvertDateFormat(Date_of_Guarantee);
                                    if (DateTime.TryParse(Date_of_Guarantee, out DateTime sDate))
                                    {
                                        Date_of_Expiry = db.ConvertDateFormat(Date_of_Expiry);
                                        if (DateTime.TryParse(Date_of_Expiry, out DateTime expiryDate))
                                        {
                                            Cliam_Date = db.ConvertDateFormat(Cliam_Date);

                                            DateTime d = sDate;
                                            string sDate1 = "", sDate2 = "", sDate3 = "";
                                            DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now;
                                            sDate1 = d.ToString("dd/MM/yyyy");
                                            sDate1 = db.ConvertDateFormat(sDate1);
                                            CDate1 = Convert.ToDateTime(sDate1);

                                            //sDate2 = Convert.ToDateTime(Date_of_Expiry).ToString("dd/MM/yyyy");
                                            //sDate2 = db.ConvertDateFormat(sDate2);
                                            CDate2 = expiryDate;

                                            //sDate3 = Convert.ToDateTime(Cliam_Date).ToString("dd/MM/yyyy");
                                            sDate3 = Cliam_Date;
                                            CDate3 = Convert.ToDateTime(sDate3);

                                            string Currecncy_CultureInfo = "";
                                            //string Currency = "";
                                            if (Currency == "INR")
                                            {
                                                Currecncy_CultureInfo = "en-IN";
                                                Currency = "&#x20B9;";
                                            }
                                            else if (Currency == "USD")
                                            {
                                                Currecncy_CultureInfo = "en-US";
                                                Currency = "&#36;";
                                            }
                                            else
                                            {
                                                Currecncy_CultureInfo = "ja-JP";
                                                Currency = "&#165;";
                                            }

                                            if (float.TryParse(Amount, out float p))
                                            {
                                                bool sresult = db.Dbsync_InsertorUpdateBankGuarantee(new Guid(Bank_GuaranteeUID), Vendor_Name, Vendor_Address, Convert.ToDouble(Amount), Validity, CDate1, Convert.ToInt32(No_of_Collaterals), Bank_Name, Bank_Branch, IFSC_Code, new Guid(wExists), new Guid(pExists), BG_Number, CDate2, Currency, Currecncy_CultureInfo, CDate3, "Y", Bank_Address);
                                                if (sresult)
                                                {
                                                    sError = false;
                                                }
                                                else
                                                {
                                                    sError = true;
                                                    ErrorText = "Bank Guarantee not updated.";
                                                }
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Invalid Amount Value.";
                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Invalid Expiry Datetime format.";
                                        }
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Invalid Date of Guarantee Datetime format.";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Date of Guarantee cannot be empty.";
                                }
                                
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Workpackage Name is not found";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Project Name is not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "BG Number cannot be empty.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    BankGuaranteeUID= Bank_GuaranteeUID,
                    Message = "Successfully Updated Bank Guarantee"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/BankGuaranteeDelete")]
        public IHttpActionResult BankGuaranteeDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var Bank_GuaranteeUID = httpRequest.Params["Bank_GuaranteeUID"];
            var UserName = httpRequest.Params["UserName"];
            bool sError = false;
            var UserUID = "";
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "Bank_GuaranteeUID=" + Bank_GuaranteeUID + "&UserName=" + UserName;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(Bank_GuaranteeUID, out Guid uid))
                    {
                        DataSet dsUsers = db.getUserDetails_by_EmailID(UserName);
                        if (dsUsers.Tables[0].Rows.Count > 0)
                        {
                            UserUID = dsUsers.Tables[0].Rows[0]["UserUID"].ToString();

                        }
                        else
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:UserName not exists"
                            });
                        }
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataSet ds = db.GetBankGuarantee_by_Bank_GuaranteeUID(new Guid(Bank_GuaranteeUID));
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int cnt = db.BankGuarantee_Delete_by_Bank_GuaranteeUID(new Guid(Bank_GuaranteeUID), new Guid(UserUID));
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(Bank_GuaranteeUID, 2, "BankGuarantee", "Y", "Bank_GuaranteeUID");
                                    if (cnt > 0)
                                    {
                                        cnt = db.ServerFlagsUpdate(Bank_GuaranteeUID, 2, "BankDocuments", "Y", "Bank_GuaranteeUID");
                                        if (cnt > 0)
                                        {
                                            sError = false;
                                        }
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Bank Guarantee not deleted. Please contact system admin.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Bank Guarantee UID not found.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid UserUID.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid BankGuarantee UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Bank Guarantee."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/AddBankGuaranteeDocuments")]
        public IHttpActionResult AddBankGuaranteeDocuments()
        {
            var httpRequest = HttpContext.Current.Request;
            var BankDoc_UID = Guid.NewGuid().ToString();
            var Bank_GuaranteeUID = httpRequest.Params["Bank_GuaranteeUID"];
            var Document_Name = httpRequest.Params["Document_Name"];
            //var Document_Type = httpRequest.Params["Document_Type"];
            var Document_File = httpRequest.Params["Document_File"];
            // var RelativePath = httpRequest.Params["RelativePath"] ?? "//Documents//";
            var RelativePath = "//Documents//";
            bool sError = false;
            string ErrorText = "";
            int cnt = 1;
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "BankDoc_UID=" + BankDoc_UID + "&Bank_GuaranteeUID=" + Bank_GuaranteeUID + "&Document_Name=" + Document_Name + "&Document_File=" + Document_File + "&RelativePath=" + RelativePath;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(Bank_GuaranteeUID, out Guid uid))
                    {
                        if (httpRequest.Files.Count > 0)
                        {
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];
                                string sDocumentPath = string.Empty;
                                sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + RelativePath;

                                if (!Directory.Exists(sDocumentPath))
                                {
                                    Directory.CreateDirectory(sDocumentPath);
                                }
                                string Extn = Path.GetExtension(httpPostedFile.FileName);
                                string Fullpath = sDocumentPath + Path.GetFileName(httpPostedFile.FileName);
                                RelativePath = "~/Documents/" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + Extn;
                                // added on 04/05/2022 for venkats change
                                while (File.Exists(Fullpath))
                                {
                                    Fullpath = sDocumentPath + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_" + cnt + Extn;
                                    RelativePath = "~/Documents/" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_" + cnt + Extn;
                                    cnt += 1;
                                }

                                //if (File.Exists(Fullpath))
                                //{
                                //    Fullpath = sDocumentPath + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_1" + Extn;
                                //    RelativePath = "~/Documents/" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_1" + Extn;
                                //}
                                //else
                                //{
                                //    // put the relative path correctly
                                //    RelativePath = "~/Documents/" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + Extn;
                                //}
                                //string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);

                                httpPostedFile.SaveAs(Fullpath);

                                bool count = db.InsertorUpdateBankDocuments(new Guid(BankDoc_UID), new Guid(Bank_GuaranteeUID), Document_Name, Extn, RelativePath, "Y");
                                if (count)
                                {
                                    sError = false;

                                    // Blob

                                    byte[] filetobytes = db.FileToByteArray(Fullpath);

                                    Guid new_guid = Guid.NewGuid();
                                    db.InsertUploadedBankDocumentBlob(new_guid, BankDoc_UID.ToString(), filetobytes, Document_Name, RelativePath);
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Bank Guarantee Document not Updated";
                                }
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Please upload Bank Guarantee document.";
                        }

                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Bank Guarantee UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    BankDocumentUID = BankDoc_UID,
                    Message = "Successfully Added Bank Guarantee Document."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/BankGuaranteeDocumentsDelete")]
        public IHttpActionResult BankGuaranteeDocumentsDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var BankDoc_UID = httpRequest.Params["BankDoc_UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;

            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "BankDoc_UID=" + BankDoc_UID + "&UserUID=" + UserUID;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");
                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(BankDoc_UID, out Guid uid))
                    {
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataSet ds = db.GetBankDocuments_by_BankDoc_UID(new Guid(BankDoc_UID));
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int cnt = db.BankDocuments_Delete(new Guid(BankDoc_UID), new Guid(UserUID));
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(BankDoc_UID, 2, "BankDocuments", "Y", "BankDoc_UID");
                                    if (cnt > 0)
                                    {
                                        sError = false;
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Bank Guarantee Document not deleted. Please contact system admin.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Bank Guarantee Document not found.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid User UID.";
                        }
                            
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Bank Guarantee Document UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Bank Guarantee Document."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/AddorEditInsurance")]
        public IHttpActionResult AddorEditInsurance()
        {
            var httpRequest = HttpContext.Current.Request;
            var InsuranceUID = Guid.NewGuid().ToString();
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var Vendor_Name = httpRequest.Params["Vendor_Name"];
            //var sMonth = httpRequest.Params["sMonth"];          
            var Vendor_Address = httpRequest.Params["Vendor_Address"];
            var Name_of_InsuranceCompany = httpRequest.Params["Name_of_InsuranceCompany"];
            var Branch = httpRequest.Params["Branch"];
            var Policy_Number = httpRequest.Params["Policy_Number"];
            var Policy_Status = httpRequest.Params["Policy_Status"];
            var Maturity_Date = httpRequest.Params["Maturity_Date"];
            var Nominee = httpRequest.Params["Nominee"];
            var Insured_Date = httpRequest.Params["Insured_Date"];
            var Insured_Amount = httpRequest.Params["Insured_Amount"];
            var Premium_Amount = httpRequest.Params["Premium_Amount"];
            var Frequency = httpRequest.Params["Frequency"];
            var Currency = httpRequest.Params["Currency"];
            var Currency_CultureInfo = httpRequest.Params["Currency_CultureInfo"];
            var FirstPremium_Duedate = httpRequest.Params["FirstPremium_Duedate"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "InsuranceUID=" + InsuranceUID + "&ProjectName=" + ProjectName + "&WorkpackageName=" + WorkpackageName + "&Vendor_Name=" + Vendor_Name + "&Vendor_Address=" + Vendor_Address + "&Name_of_InsuranceCompany=" + Name_of_InsuranceCompany +
                    "&Branch=" + Branch + "&Policy_Number=" + Policy_Number + "&Policy_Status=" + Policy_Status + "&Maturity_Date=" + Maturity_Date + "&Nominee=" + Nominee + "&Insured_Date=" + Insured_Date +
                    "&Insured_Amount=" + Insured_Amount + "&Premium_Amount=" + Premium_Amount + "&Frequency=" + Frequency + "&Currency=" + Currency + "&Currency_CultureInfo=" + Currency_CultureInfo + "&FirstPremium_Duedate=" + FirstPremium_Duedate;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var pExists = db.ProjectNameExists(ProjectName);

                    if (!string.IsNullOrEmpty(pExists))
                    {
                        var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                        if (!string.IsNullOrEmpty(wExists))
                        {
                            Insured_Date = db.ConvertDateFormat(Insured_Date);
                            if (DateTime.TryParse(Insured_Date, out DateTime insureddate))
                            {
                                Maturity_Date = db.ConvertDateFormat(Maturity_Date);
                                if (DateTime.TryParse(Maturity_Date, out DateTime sDate))
                                {
                                    //DateTime d = sDate;

                                    //added on 09/05/2022 by zuber
                                    FirstPremium_Duedate = db.ConvertDateFormat(FirstPremium_Duedate);
                                    if (DateTime.TryParse(FirstPremium_Duedate, out DateTime sDate3))
                                    {

                                        string sDate1 = "", sDate2 = "";
                                        DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now;
                                        //sDate1 = d.ToString("dd/MM/yyyy");
                                        //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        CDate1 = sDate;

                                        //sDate2 = Convert.ToDateTime(Insured_Date).ToString("dd/MM/yyyy");
                                        //sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                                        CDate2 = insureddate;

                                        sDate1 = sDate3.ToString("dd/MM/yyyy");
                                        sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];

                                        if (float.TryParse(Insured_Amount, out float p))
                                        {
                                            if (float.TryParse(Premium_Amount, out float pAmount))
                                            {
                                                string Currecncy_CultureInfo = "";
                                                //string Currency = "";
                                                if (Currency == "INR")
                                                {
                                                    Currecncy_CultureInfo = "en-IN";
                                                    Currency = "&#x20B9;";
                                                }
                                                else if (Currency == "USD")
                                                {
                                                    Currecncy_CultureInfo = "en-US";
                                                    Currency = "&#36;";
                                                }
                                                else
                                                {
                                                    Currecncy_CultureInfo = "ja-JP";
                                                    Currency = "&#165;";
                                                }

                                                bool sresult = db.DbSyncInsertorUpdateInsurance(new Guid(InsuranceUID), Vendor_Name, Vendor_Address, Name_of_InsuranceCompany, Branch, Policy_Number, Policy_Status, CDate1, Nominee, new Guid(pExists), new Guid(wExists), CDate2, p, pAmount, Convert.ToInt32(Frequency), Currency, Currecncy_CultureInfo, sDate1, "Y");
                                                if (sresult)
                                                {
                                                    sError = false;
                                                }
                                                else
                                                {
                                                    sError = true;
                                                    ErrorText = "Insurance not updated.";
                                                }
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Invalid Premium Amount Value.";
                                            }

                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Invalid Insured Amount Value.";
                                        }
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Invalid FirstPremium Duedate Datetime format.";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Invalid Maturity Date Datetime format.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Insured Date.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Workpackage Name is not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Project Name is not found";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    InsuranceUID= InsuranceUID,
                    Message = "Successfully Updated Insurance"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/InsuranceDelete")]
        public IHttpActionResult InsuranceDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var InsuranceUID = httpRequest.Params["InsuranceUID"];
            var userName = httpRequest.Params["UserName"];
            var UserUID = "";
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "InsuranceUID=" + InsuranceUID + "&userName=" + userName;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");


                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    DataSet dsUsers = db.getUserDetails_by_EmailID(userName);
                    if (dsUsers.Tables[0].Rows.Count > 0)
                    {
                        UserUID = dsUsers.Tables[0].Rows[0]["UserUID"].ToString();
                       
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:UserName not exists"
                        });
                    }
                    if (Guid.TryParse(InsuranceUID, out Guid insuranceuid))
                    {
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataSet ds = db.GetInsuranceSelect_by_InsuranceUID(new Guid(InsuranceUID));
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int cnt = db.Insurance_Delete(new Guid(InsuranceUID), new Guid(UserUID));
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(InsuranceUID, 2, "Insurance", "Y", "InsuranceUID");
                                    if (cnt > 0)
                                    {
                                        cnt = db.ServerFlagsUpdate(InsuranceUID, 2, "InsuranceDocuments", "Y", "InsuranceUID");
                                        if (cnt > 0)
                                        {
                                            cnt = db.ServerFlagsUpdate(InsuranceUID, 2, "Insurance_Premiums", "Y", "InsuranceUID");
                                            if (cnt > 0)
                                            {
                                                sError = false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Invalid Insurance UID.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Insurance not deleted.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid User UID.";
                        }
                            
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Insurance UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Insurance."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/AddInsuranceDocuments")]
        public IHttpActionResult AddInsuranceDocuments()
        {
            var httpRequest = HttpContext.Current.Request;
            var InsuranceDoc_UID = Guid.NewGuid().ToString();
            var InsuranceUID = httpRequest.Params["InsuranceUID"];
            var InsuranceDoc_Name = httpRequest.Params["InsuranceDoc_Name"];
            //var InsuranceDoc_Type = httpRequest.Params["InsuranceDoc_Type"];
            var InsuranceDoc_FilePath = httpRequest.Params["InsuranceDoc_FilePath"];
            // var RelativePath = httpRequest.Params["RelativePath"] ?? "//Documents//";
            var RelativePath = "//Documents//";
            int cnt = 1;
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "InsuranceDoc_UID=" + InsuranceDoc_UID + "&InsuranceUID=" + InsuranceUID + "&InsuranceDoc_Name=" + InsuranceDoc_Name + "&InsuranceDoc_FilePath=" + InsuranceDoc_FilePath + "&RelativePath=" + RelativePath;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(InsuranceUID, out Guid insuranceuid))
                {
                    if (httpRequest.Files.Count > 0)
                    {
                        DataSet ds = db.GetInsuranceSelect_by_InsuranceUID(insuranceuid);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                string sDocumentPath = string.Empty;
                                sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + RelativePath;

                                if (!Directory.Exists(sDocumentPath))
                                {
                                    Directory.CreateDirectory(sDocumentPath);
                                }
                                string Fullpath = sDocumentPath + Path.GetFileName(httpPostedFile.FileName);
                                string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                    RelativePath = "~/Documents/" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + Extn;
                                    // added on 04/05/2022 for venkats change
                                    while (File.Exists(Fullpath))
                                    {
                                        Fullpath = sDocumentPath + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_" + cnt + Extn;
                                        RelativePath = "~/Documents/" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_" + cnt + Extn;
                                        cnt += 1;
                                    }
                                    //----------------------------------
                                //    if (File.Exists(Fullpath))
                                //{
                                //    Fullpath = sDocumentPath + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_1" + Extn;
                                //        // put the relative path correctly
                                //        RelativePath = "~/Documents/" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_1" + Extn;
                                // }
                                // else
                                //  {
                                //        // put the relative path correctly
                                //        RelativePath = "~/Documents/" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + Extn;
                                //    }
                                    httpPostedFile.SaveAs(Fullpath);
                                   
                               
                                bool count = db.DbSync_InsertorUpdateInsuranceDocuments(new Guid(InsuranceDoc_UID), new Guid(InsuranceUID), InsuranceDoc_Name, Extn, RelativePath, "Y");
                                if (count)
                                {
                                    sError = false;
                                        //for blob
                                        byte[] filetobytes = null;
                                        filetobytes = db.FileToByteArray(Fullpath);

                                        Guid new_guid = Guid.NewGuid();
                                        db.InsertUploadedInsuranceDocumentBlob(new_guid, InsuranceDoc_UID.ToString(), filetobytes, httpPostedFile.FileName, RelativePath);
                                        //

                                    }
                                    else
                                {
                                    sError = true;
                                    ErrorText = "Insurance Document not Updated.Please contact system admin.";
                                }
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Insurance UID not found.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Please upload Insurance document.";
                    }

                }
                else
                {
                    sError = true;
                    ErrorText = "Invalid Insurance UID.";
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
                ErrorText = "Error:"+ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    InsuranceDocumentUID= InsuranceDoc_UID,
                    Message = "Successfully Added Insurance Document."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/InsuranceDocumentsDelete")]
        public IHttpActionResult InsuranceDocumentsDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var InsuranceDoc_UID = httpRequest.Params["InsuranceDoc_UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "InsuranceDoc_UID=" + InsuranceDoc_UID + "&UserUID=" + UserUID;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(InsuranceDoc_UID, out Guid insurancedocuid))
                    {
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataSet ds = db.GetInsuranceDocuments_by_InsuranceDoc_UID(new Guid(InsuranceDoc_UID));
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int cnt = db.InsuranceDocuments_Delete(new Guid(InsuranceDoc_UID), new Guid(UserUID));
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(InsuranceDoc_UID, 2, "InsuranceDocuments", "Y", "InsuranceDoc_UID");
                                    if (cnt > 0)
                                    {
                                        sError = false;
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Invalid Insurance Document UID.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Insurance Document not deleted.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid UserUID.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Insurance Document UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Insurance Document."
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/AddInsurancePremium")]
        public IHttpActionResult AddInsurancePremium()
        {
            var httpRequest = HttpContext.Current.Request;
            var PremiumUID = Guid.NewGuid().ToString();
            var InsuranceUID = httpRequest.Params["InsuranceUID"];
            var Premium_Paid = httpRequest.Params["Premium_Paid"];
            var Interest = httpRequest.Params["Interest"];
            var Penalty = httpRequest.Params["Penalty"];
            var Premium_PaidDate = httpRequest.Params["Premium_PaidDate"];
            var Premium_DueDate = httpRequest.Params["Premium_DueDate"];
            //var Next_PremiumDate = httpRequest.Params["Next_PremiumDate"];
            var Premium_Receipt = httpRequest.Params["Premium_Receipt"];
            var Remarks = httpRequest.Params["Remarks"];
            //  var RelativePath = httpRequest.Params["RelativePath"] ?? "//Documents//";
            var RelativePath = "//Documents//";
            int cnt = 1;
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "PremiumUID=" + PremiumUID + "&InsuranceUID=" + InsuranceUID + "&Premium_Paid=" + Premium_Paid + "&Interest=" + Interest + "&Penalty=" + Penalty + "&Premium_PaidDate=" + Premium_PaidDate +
                    "&Premium_DueDate=" + Premium_DueDate + "&Premium_Receipt=" + Premium_Receipt + "&Remarks=" + Remarks + "&RelativePath=" + RelativePath;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(InsuranceUID, out Guid insuranceuid))
                    {
                        DataSet ds = db.GetInsuranceSelect_by_InsuranceUID(insuranceuid);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (float.TryParse(Premium_Paid, out float pPaidAmount))
                            {
                                if (float.TryParse(Interest, out float Inte))
                                {
                                    Premium_PaidDate = db.ConvertDateFormat(Premium_PaidDate);
                                    if (DateTime.TryParse(Premium_PaidDate, out DateTime pPaidDate))
                                    {
                                        Premium_DueDate = db.ConvertDateFormat(Premium_DueDate);
                                        byte[] filetobytes = null;
                                        if (DateTime.TryParse(Premium_DueDate, out DateTime pDueDate))
                                        {
                                            bool FileSaved = false;
                                            string Fullpath = string.Empty;
                                            for (int i = 0; i < httpRequest.Files.Count; i++)
                                            {
                                                HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                                string sDocumentPath = string.Empty;
                                                sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + RelativePath;

                                                if (!Directory.Exists(sDocumentPath))
                                                {
                                                    Directory.CreateDirectory(sDocumentPath);
                                                }
                                                Fullpath = sDocumentPath + Path.GetFileName(httpPostedFile.FileName);
                                                string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                                while (File.Exists(Fullpath))
                                                {
                                                    Fullpath = sDocumentPath + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_" + cnt + Extn;
                                                    cnt += 1;
                                                }
                                                httpPostedFile.SaveAs(Fullpath);
                                                //for blob
                                               
                                                filetobytes = db.FileToByteArray(Fullpath);

                                                FileSaved = true;
                                            }


                                            string sDate1 = "", sDate2 = "", sDate3 = "";
                                            DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now;

                                            sDate1 = pPaidDate.ToString();
                                            sDate1 = db.ConvertDateFormat(sDate1);
                                            CDate1 = pPaidDate;

                                            sDate2 = pDueDate.ToString();
                                            sDate2 = db.ConvertDateFormat(sDate2);
                                            CDate2 = pDueDate;

                                            string Next_PremiumDate = string.Empty;
                                            DataSet dsnextdueDate = db.GetInsurancePremium_DueDate_NextDueDate(insuranceuid);
                                            if (dsnextdueDate.Tables[0].Rows[0]["NextDueDate"].ToString() != null && dsnextdueDate.Tables[0].Rows[0]["NextDueDate"].ToString() != "")
                                            {
                                                Next_PremiumDate = Convert.ToDateTime(dsnextdueDate.Tables[0].Rows[0]["NextDueDate"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            }
                                            if (!string.IsNullOrEmpty(Next_PremiumDate))
                                            {
                                                sDate3 = Next_PremiumDate;
                                                sDate3 = db.ConvertDateFormat(Next_PremiumDate);
                                                CDate3 = Convert.ToDateTime(sDate3);
                                            }
                                            else
                                            {
                                                sDate3 = CDate1.AddMonths(1).ToString();
                                                sDate3 = db.ConvertDateFormat(Next_PremiumDate);
                                                CDate3 = Convert.ToDateTime(sDate3);
                                            }

                                            //bool result = db.DbSync_InsertorUpdateInsurancePremium(new Guid(PremiumUID), new Guid(InsuranceUID), pPaidAmount, Inte, float.Parse(Penalty), CDate1, CDate2, CDate3, (FileSaved ? RelativePath + Path.GetFileName(Fullpath) : ""), Remarks, "Y");
                                            bool result = db.InsertorUpdateInsurancePremium(new Guid(PremiumUID), new Guid(InsuranceUID), pPaidAmount, Inte, float.Parse(Penalty), CDate1, CDate2, CDate3, (FileSaved ? RelativePath + Path.GetFileName(Fullpath) : ""), Remarks, filetobytes);

                                            if (result)
                                            {
                                                sError = false;
                                            }
                                            else
                                            {
                                                sError = true;
                                                ErrorText = "Insurance Premium not Updated. Please contact system admin.";

                                            }
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Invalid Premium Due date.";
                                        }
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Invalid Premium Paid date.";
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Invalid Interest amount.";
                                }

                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Premium amount.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Insurance UID not found.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Insurance UID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    PremiumUID = PremiumUID,
                    Message = "Successfully Added Insurance Premium."
                });
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/InsurancePremiumDelete")]
        public IHttpActionResult InsurancePremiumDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var PremiumUID = httpRequest.Params["PremiumUID"];
            var UserName = httpRequest.Params["UserName"];
            bool sError = false;
            var UserUID = "";
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "PremiumUID=" + PremiumUID + "&UserName=" + UserName;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (Guid.TryParse(PremiumUID, out Guid premiumuid))
                    {
                        DataSet dsUsers = db.getUserDetails_by_EmailID(UserName);
                        if (dsUsers.Tables[0].Rows.Count > 0)
                        {
                            UserUID = dsUsers.Tables[0].Rows[0]["UserUID"].ToString();

                        }
                        else
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:UserName not exists"
                            });
                        }
                        if (Guid.TryParse(UserUID, out Guid useruid))
                        {
                            DataSet ds = db.GetInsurancePremiumSelect_by_PremiumUID(new Guid(PremiumUID));
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                int cnt = db.InsurancePremium_Delete(new Guid(PremiumUID), new Guid(UserUID));
                                if (cnt > 0)
                                {
                                    cnt = db.ServerFlagsUpdate(PremiumUID, 2, "Insurance_Premiums", "Y", "PremiumUID");
                                    if (cnt > 0)
                                    {
                                        sError = false;
                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Invalid Insurance Premium UID.";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Insurance Premium not deleted.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid UserUID.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Inavalid PremiumUID.";
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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Deleted Insurance Premium."
                });
            }
        }

        //Added by venkat on 08 May 2022
        [Authorize]
        [HttpPost]
        [Route("api/ExtoActivity/AddInsurancePremiumReceipt")]
        public IHttpActionResult AddInsurancePremiumReceipt()
        {
            var httpRequest = HttpContext.Current.Request;
            // var PremiumUID = Guid.NewGuid().ToString();


            var RelativePath = "//Documents//";
            int cnt = 1;
            bool sError = false;
            string ErrorText = "";
            try
            {
                //Insert
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                var InsurancePremiumUID = httpRequest.Params["InsurancePremiumUID"];
                string postData = "InsurancePremiumUID=" + httpRequest.Params["InsurancePremiumUID"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    if (httpRequest.Params["InsurancePremiumUID"] == null)
                    {
                        sError = true;
                        ErrorText = "PremiumUid is mandatory";
                    }
                    else if (httpRequest.Files.Count == 0)
                    {
                        sError = true;
                        ErrorText = "Receipt doc is mandatory";

                    }
                    if (!sError)
                    {
                        DataSet ds = db.GetInsurancePremiumSelect_by_PremiumUID(new Guid(InsurancePremiumUID));
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows[0]["Premium_Receipt"].ToString() == "")
                            {
                                string Fullpath = string.Empty;
                                for (int i = 0; i < httpRequest.Files.Count; i++)
                                {
                                    try
                                    {
                                        HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                        string sDocumentPath = string.Empty;
                                        sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + RelativePath;

                                        if (!Directory.Exists(sDocumentPath))
                                        {
                                            Directory.CreateDirectory(sDocumentPath);
                                        }
                                        Fullpath = sDocumentPath + Path.GetFileName(httpPostedFile.FileName);
                                        string Extn = System.IO.Path.GetExtension(httpPostedFile.FileName);
                                        string filePath = RelativePath + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + Extn;

                                        while (File.Exists(Fullpath))
                                        {
                                            Fullpath = sDocumentPath + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_" + cnt + Extn;
                                            filePath = RelativePath + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + "_" + cnt + Extn;
                                            cnt += 1;
                                        }
                                        httpPostedFile.SaveAs(Fullpath);
                                        byte[] filetobytes = db.FileToByteArray(Fullpath);

                                        bool result = db.UpdatePremiumReceipt(InsurancePremiumUID, filePath, filetobytes);
                                        //db.DbSync_InsertorUpdateInsurancePremium(new Guid(PremiumUID), new Guid(InsuranceUID), pPaidAmount, Inte, float.Parse(Penalty), CDate1, CDate2, CDate3, (FileSaved ? RelativePath + Path.GetFileName(Fullpath) : ""), Remarks, "Y");
                                        if (result)
                                        {
                                            sError = false;
                                        }
                                        else
                                        {
                                            sError = true;
                                            ErrorText = "Insurance Premium not Updated. Please contact system admin.";

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        sError = true;
                                        ErrorText = ex.Message;
                                    }

                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Premium Receipt already updated";

                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Insurance Premium not Available. Please contact system admin.";

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
                ErrorText = "Error:" + ex.Message;
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
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully added premium receipt file"
                });
            }
        }
    }
}