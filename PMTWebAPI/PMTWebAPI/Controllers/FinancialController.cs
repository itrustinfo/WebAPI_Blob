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
    public class FinancialController : ApiController
    {
        DBGetData db = new DBGetData();
        Invoice invoice = new Invoice();
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

        //[Authorize]
        //[HttpPost]
        //[Route("api/Financial/AddEditRABills")]
        //public IHttpActionResult AddEditRABills()
        //{
        //    bool sError = false;
        //    string ErrorText = "";
        //    string rabillUid = "";
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["RAbillno"]) || String.IsNullOrEmpty(httpRequest.Params["Date"]) ||
        //            String.IsNullOrEmpty(httpRequest.Params["numberofJIRs"]) || String.IsNullOrEmpty(httpRequest.Params["JIR"]))
        //        {
        //            return Json(new
        //            {
        //                Status = "Failure",
        //                Message = "Error:Mandatory fields are missing"
        //            }); ;
        //        }
        //        //Insert into WebAPITransctions table
        //        var BaseURL = HttpContext.Current.Request.Url.ToString();
        //        string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + "&RAbillno=" + httpRequest.Params["RAbillno"] + "&Date=" + httpRequest.Params["Date"] + "&numberofJIRs=" + httpRequest.Params["numberofJIRs"] + "&JIR=" + httpRequest.Params["JIR"];
        //        db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

        //        var identity = (ClaimsIdentity)User.Identity;
        //        if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
        //        {

        //            DataSet dsWorkPackages = new DataSet();
        //            var projectName = httpRequest.Params["ProjectName"];
        //            var RAbillno = httpRequest.Params["RAbillno"];
        //            int cnt = 0;
        //            if (DateTime.TryParse(httpRequest.Params["Date"], out DateTime sDate))
        //            {
        //                var NoOfJIR = httpRequest.Params["NoOfJIR"];
        //                var InspectionUID = httpRequest.Params["JIR"];//InspectionUID
        //                DataTable dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUID);
        //                if(dtjoininspection.Rows.Count==0)
        //                {
        //                    return Json(new
        //                    {
        //                        Status = "Failure",
        //                        Message = "Error:InspectionUid is not exists"
        //                    }) ;
        //                }
        //                DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
        //                if (dtWorkPackages.Rows.Count > 0)
        //                {
        //                    rabillUid = db.AddRABillNumber(RAbillno, new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), sDate);
        //                    if (rabillUid == "Exists")
        //                    {
        //                        sError = true;
        //                        ErrorText = "RA Bill Number already exists.";
        //                    }
        //                    else if (rabillUid == "Error1")
        //                    {
        //                        sError = true;
        //                        ErrorText = "There is a problem with this feature. Please contact system admin.";
        //                    }
        //                    else
        //                    {
        //                        int ErrorCount = 0;
        //                        int ItemCount = 0;
        //                        double totamount = 0;
        //                        DataSet ds = db.GetBOQDetails_by_projectuid(new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()));
        //                        if (ds.Tables[0].Rows.Count > 0)
        //                        {

        //                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                            {
        //                                string sDate2 = "";
        //                                DateTime CDate2 = DateTime.Now;

        //                                sDate2 = DateTime.Now.ToString("dd/MM/yyyy");
        //                                //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
        //                                sDate2 = db.ConvertDateFormat(sDate2);
        //                                CDate2 = Convert.ToDateTime(sDate);

        //                                cnt = db.InsertRABillsItems(rabillUid, ds.Tables[0].Rows[i]["Item_Number"].ToString(), ds.Tables[0].Rows[i]["Description"].ToString(), CDate2.ToString(), "0", new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), new Guid(ds.Tables[0].Rows[i]["BOQDetailsUID"].ToString()));
        //                                if (cnt <= 0)
        //                                {
        //                                    ErrorCount += 1;
        //                                }
        //                                else
        //                                {
        //                                    ItemCount += 1;
        //                                    totamount += ds.Tables[0].Rows[i]["INR-Amount"].ToString() == "" ? 0 : Convert.ToDouble(ds.Tables[0].Rows[i]["INR-Amount"].ToString());
        //                                }
        //                            }
        //                        }

        //                        if (ErrorCount > 0)
        //                        {
        //                            sError = true;
        //                            ErrorText = "There is a problem linking BOQ details to this RABill. Please contact system admin";

        //                        }
        //                        else
        //                        {
        //                            Guid AssignJointInspectionUID = Guid.NewGuid();
        //                            cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(ds.Tables[0].Rows[0]["BOQDetailsUid"].ToString()), new Guid(InspectionUID));
        //                            if (cnt == 0)
        //                            {
        //                                sError = true;
        //                                ErrorText = "Join Inspection to RABill is not inserted";
        //                            }
        //                        }

        //                    }

        //                }

        //                else
        //                {
        //                    sError = true;
        //                    ErrorText = "No Workpackage available for select project";
        //                }
        //            }
        //            else
        //            {
        //                sError = true;
        //                ErrorText = "Date is not correct format";
        //            }

        //        }
        //        else
        //        {
        //            sError = true;
        //            ErrorText = "Not Authorized IP address";
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
        //        return Json(new
        //        {
        //            Status = "Success",
        //            RABillUId = rabillUid,
        //            Message = "Successfully Updated Inspection to RAbill"
        //        });
        //    }
        //}


        //-----------------------------------------------------------
        // added on 14/02/2022 for venkat
        [Authorize]
        [HttpPost]
        [Route("api/Financial/AddEditRABills")]
        public IHttpActionResult AddEditRABills()
        {
            bool sError = false;
            string ErrorText = "";
            string rabillUid = "";
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + "&RAbillno=" + httpRequest.Params["RAbillno"] + "&Date=" + httpRequest.Params["Date"] + "&numberofJIRs=" + httpRequest.Params["NoOfJIR"] + "&JIR=" + httpRequest.Params["JIR"];
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["RAbillno"]) || String.IsNullOrEmpty(httpRequest.Params["Date"]) ||
                    String.IsNullOrEmpty(httpRequest.Params["NoOfJIR"]) || String.IsNullOrEmpty(httpRequest.Params["JIR"]))
                {
                    //   db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:Project Name is Manadatory");
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    }); ;
                }

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {

                    DataSet dsWorkPackages = new DataSet();
                    var projectName = httpRequest.Params["ProjectName"];
                    var RAbillno = httpRequest.Params["RAbillno"];
                    int cnt = 0;
                    DateTime sDate=DateTime.Now;
                   if (!string.IsNullOrEmpty(httpRequest.Params["Date"]))
                   {
                        string cdate = db.ConvertDateFormat(httpRequest.Params["Date"]);
                      
                           sDate = Convert.ToDateTime(cdate);
                    }
                    //if (DateTime.TryParse(httpRequest.Params["Date"], out DateTime sDate))
                    //{

                        var NoOfJIR = httpRequest.Params["NoOfJIR"];
                        var InspectionUID = httpRequest.Params["JIR"];//InspectionUID
                        string[] InspectionUIDList = InspectionUID.Split('$');
                        DataTable dtjoininspection ;
                        //DataTable dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUID);
                        //if (dtjoininspection.Rows.Count == 0)
                        //{
                        //    //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:InspectionUid is not exists");
                        //    return Json(new
                        //    {
                        //        Status = "Failure",
                        //        Message = "Error:InspectionUid is not exists"
                        //    });
                        //}
                        // added by zuber on 17/02/2022

                        if (InspectionUID.Contains("$"))
                        {
                            for (int i = 0; i < InspectionUIDList.Length; i++)
                            {
                                dtjoininspection = new DataTable();
                                dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUIDList[i]);
                                if (dtjoininspection.Rows.Count == 0)
                                {
                                    //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:InspectionUid is not exists");
                                    return Json(new
                                    {
                                        Status = "Failure",
                                        Message = "Error:InspectionUid does not exists"
                                    });
                                }
                            }
                        }
                        else
                        {
                            dtjoininspection = new DataTable();
                            dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUID);
                            if (dtjoininspection.Rows.Count == 0)
                            {
                                //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:InspectionUid is not exists");
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:InspectionUid is not exists"
                                });
                            }
                        }
                        //-----------------------------------------------------------

                        DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                        if (dtWorkPackages.Rows.Count > 0)
                        {
                            rabillUid = db.AddRABillNumber(RAbillno, new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), sDate);
                            if (rabillUid == "Exists")
                            {
                                sError = true;
                                ErrorText = "RA Bill Number already exists.";
                            }
                            else if (rabillUid == "Error1")
                            {
                                sError = true;
                                ErrorText = "There is a problem with this feature. Please contact system admin.";
                            }
                            else
                            {
                                int ErrorCount = 0;
                                int ItemCount = 0;
                                double totamount = 0;
                                DataSet ds = db.GetBOQDetails_by_projectuid(new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()));
                                if (ds.Tables[0].Rows.Count > 0)
                                {

                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        string sDate2 = "";
                                        DateTime CDate2 = DateTime.Now;

                                        sDate2 = DateTime.Now.ToString("dd/MM/yyyy");
                                        //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        sDate2 = db.ConvertDateFormat(sDate2);
                                        CDate2 = Convert.ToDateTime(sDate);

                                        cnt = db.InsertRABillsItems(rabillUid, ds.Tables[0].Rows[i]["Item_Number"].ToString(), ds.Tables[0].Rows[i]["Description"].ToString(), CDate2.ToString(), "0", new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), new Guid(ds.Tables[0].Rows[i]["BOQDetailsUID"].ToString()));
                                        if (cnt <= 0)
                                        {
                                            ErrorCount += 1;
                                        }
                                        else
                                        {
                                            ItemCount += 1;
                                            totamount += ds.Tables[0].Rows[i]["INR-Amount"].ToString() == "" ? 0 : Convert.ToDouble(ds.Tables[0].Rows[i]["INR-Amount"].ToString());
                                        }
                                    }
                                }

                                if (ErrorCount > 0)
                                {
                                    sError = true;
                                    ErrorText = "There is a problem linking BOQ details to this RABill. Please contact system admin";

                                }
                                else
                                {
                                    Guid AssignJointInspectionUID = Guid.NewGuid();
                                    //Updated by Venkat on 11 Feb 2022
                                    //string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUID));
                                    //if (boqDetailsUid == "")
                                    //{
                                    //    sError = true;
                                    //    ErrorText = "BOQDetails Uid not available for selcted inspectionuid";
                                    //}
                                    //else
                                    //{
                                    //    cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(boqDetailsUid), new Guid(InspectionUID));
                                    //    if (cnt == 0)
                                    //    {
                                    //        sError = true;
                                    //        ErrorText = "Join Inspection to RABill is not inserted";
                                    //    }
                                    //}
                                    //

                                    // added by zuber on 17/02/2022
                                    if (InspectionUID.Contains("$"))
                                    {
                                        for (int i = 0; i < InspectionUIDList.Length; i++)
                                        {
                                            AssignJointInspectionUID = Guid.NewGuid();
                                            string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUIDList[i]));
                                            if (boqDetailsUid == "")
                                            {
                                                sError = true;
                                                ErrorText += "BOQDetails Uid not available for inspectionuid : " + InspectionUIDList[i];
                                            }
                                            else
                                            {
                                                cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(boqDetailsUid), new Guid(InspectionUIDList[i]));
                                                if (cnt == 0)
                                                {
                                                    sError = true;
                                                    ErrorText += "Join Inspection to RABill is not inserted for inspectionuid :" + InspectionUIDList[i];
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUID));
                                        if (boqDetailsUid == "")
                                        {
                                            sError = true;
                                            ErrorText = "BOQDetails Uid not available for selcted inspectionuid";
                                        }
                                        else
                                        {
                                            cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(boqDetailsUid), new Guid(InspectionUID));
                                            if (cnt == 0)
                                            {
                                                sError = true;
                                                ErrorText = "Join Inspection to RABill is not inserted";
                                            }
                                        }
                                    }
                                    //--------------------------------------------------
                                }

                            }

                        }

                        else
                        {
                            sError = true;
                            ErrorText = "No Workpackage available for select project";
                        }
                    //}
                    //else
                    //{
                    //    sError = true;
                    //    ErrorText = "Date is not correct format";
                    //}

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
                // db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            else
            {

                // db.WebAPITransctionUpdate(transactionUid, "Success", " RABillUId = "+rabillUid+",Message = Successfully Updated Inspection to RAbill");
                return Json(new
                {
                    Status = "Success",
                    RABillUId = rabillUid,
                    Message = "Successfully Updated Inspection to RAbill"
                });
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/Financial/AddJIRtoRABills")]
        public IHttpActionResult AddJIRtoRABills()
        {
            bool sError = false;
            string ErrorText = "";
          
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + "&rabillUID=" + httpRequest.Params["RAbillUID"] +   "&JIR=" + httpRequest.Params["JIR"];
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["RAbillUID"]) || String.IsNullOrEmpty(httpRequest.Params["JIR"]))
                {
                    //   db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:Project Name is Manadatory");
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    }); ;
                }

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {

                    DataSet dsWorkPackages = new DataSet();
                    var projectName = httpRequest.Params["ProjectName"];
                    var rabillUID = httpRequest.Params["RAbillUID"];
                    int cnt = 0;
                   
                        var InspectionUID = httpRequest.Params["JIR"];//InspectionUID
                        string[] InspectionUIDList = InspectionUID.Split('$');
                        DataTable dtjoininspection;
                      
                      
                        if (InspectionUID.Contains("$"))
                        {
                            for (int i = 0; i < InspectionUIDList.Length; i++)
                            {
                                dtjoininspection = new DataTable();
                                dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUIDList[i]);
                                if (dtjoininspection.Rows.Count == 0)
                                {
                                //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:InspectionUid is not exists");
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:InspectionUid does not exists for " + InspectionUIDList[i]
                                 });
                                }
                            }
                        }
                        else
                        {
                            dtjoininspection = new DataTable();
                            dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUID);
                            if (dtjoininspection.Rows.Count == 0)
                            {
                                //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:InspectionUid is not exists");
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:InspectionUid is not exists"
                                });
                            }
                        }
                        //-----------------------------------------------------------

                        DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                        if (dtWorkPackages.Rows.Count > 0)
                        {
                            
                                    Guid AssignJointInspectionUID = Guid.NewGuid();
                                    
                                    if (InspectionUID.Contains("$"))
                                    {
                                        for (int i = 0; i < InspectionUIDList.Length; i++)
                                        {
                                            AssignJointInspectionUID = Guid.NewGuid();
                                            string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUIDList[i]));
                                            if (boqDetailsUid == "")
                                            {
                                                sError = true;
                                                ErrorText += "BOQDetails Uid not available for inspectionuid : " + InspectionUIDList[i];
                                            }
                                            else
                                            {
                                                cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUID), new Guid(boqDetailsUid), new Guid(InspectionUIDList[i]));
                                                if (cnt == 0)
                                                {
                                                    sError = true;
                                                    ErrorText += "Join Inspection to RABill is not inserted for inspectionuid :" + InspectionUIDList[i];
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUID));
                                        if (boqDetailsUid == "")
                                        {
                                            sError = true;
                                            ErrorText = "BOQDetails Uid not available for selcted inspectionuid";
                                        }
                                        else
                                        {
                                            cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUID), new Guid(boqDetailsUid), new Guid(InspectionUID));
                                            if (cnt == 0)
                                            {
                                                sError = true;
                                                ErrorText = "Join Inspection to RABill is not inserted";
                                            }
                                        }
                                    }
                                    //--------------------------------------------------
                                

                         }

                        

                        else
                        {
                            sError = true;
                            ErrorText = "No Workpackage available for select project";
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
                // db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            else
            {

                // db.WebAPITransctionUpdate(transactionUid, "Success", " RABillUId = "+rabillUid+",Message = Successfully Updated Inspection to RAbill");
                return Json(new
                {
                    Status = "Success",
                    Message = "Successfully Updated Inspection to RAbill"
                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/Financial/GetDedudctionList")]
        public IHttpActionResult GetDedudctionList()
        {
            bool sError = false;
            string ErrorText = "";
            string deductionJson = "";
            int NoOflistitems = 0;
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]))
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Project Name is Manadatory"
                    }); ;
                }
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] ;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var projectName = httpRequest.Params["ProjectName"];
                   string projectExists = db.ProjectNameExists(projectName);
                    if (!string.IsNullOrEmpty(projectExists))
                    {
                        DataTable dtDeductionList = db.GetDeductionDesc();
                        deductionJson = JsonConvert.SerializeObject(dtDeductionList);
                        NoOflistitems = dtDeductionList.Rows.Count;
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Project Name not exists.";
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
            else if (NoOflistitems == 0)
            {
                return Json(new
                {
                    NoOflistitems = NoOflistitems,
                    Message = "No description found"

                });
            }
            else
            {
                return Json(new
                {
                    NoOflistitems = NoOflistitems,
                    DeductionDescription = deductionJson,

                });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/Financial/AddEditInvoices")]
        public IHttpActionResult AddEditInvoices()
        {
            bool sError = false;
            string ErrorText = "";
            Guid InvoiceMaster_UID = Guid.NewGuid();
            var httpRequest = HttpContext.Current.Request;
            try
            {
                if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["Invoice Date"]) || String.IsNullOrEmpty(httpRequest.Params["Invoice Date"]) ||
                     String.IsNullOrEmpty(httpRequest.Params["Currency"]) || String.IsNullOrEmpty(httpRequest.Params["Rabill number"]) || String.IsNullOrEmpty(httpRequest.Params["Deductiondetails"]) || String.IsNullOrEmpty(httpRequest.Params["Additiondetails"]))
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    }); ;
                }

                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + ";Invoice number=" + httpRequest.Params["Invoice Date"] + ";Invoice Date=" + httpRequest.Params["Invoice Date"] +
                    ";Currency=" + httpRequest.Params["Currency"] + ";Rabill number=" + httpRequest.Params["Rabill number"] + ";Deductiondetails=" + httpRequest.Params["Deductiondetails"] + ";Additiondetails=" + httpRequest.Params["Additiondetails"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var projectName = httpRequest.Params["ProjectName"];
                    DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                    if (dtWorkPackages.Rows.Count > 0)
                    {
                        var invoiceNumber = httpRequest.Params["Invoice number"];
                        var invoiceDate= httpRequest.Params["Invoice Date"]; ;
                        if (!string.IsNullOrEmpty(invoiceDate))
                        {
                            invoiceDate = db.ConvertDateFormat(invoiceDate);
                           // invoiceDate = Convert.ToDateTime(invoiceDate);
                        }
                       // var invoiceDate = httpRequest.Params["Invoice Date"];
                        var currency = httpRequest.Params["Currency"];
                        var raBillNumber = httpRequest.Params["Rabill number"];
                        //var numberofdeductions = httpRequest.Params["Number of deductions"];
                        var Deductiondetails = httpRequest.Params["Deductiondetails"];
                        var AdditionDetails = httpRequest.Params["Additiondetails"];
                        decimal invoiceAmount = 0;
                        decimal invoiceforRaBill = 0;
                        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                        deductionListClass deductionObj = jsonSerializer.Deserialize<deductionListClass>(Deductiondetails);
                        additionListClass AdditionObj = jsonSerializer.Deserialize<additionListClass>(AdditionDetails);
                        if (deductionObj.deductionList != null)
                        {
                            for (int i = 0; i < deductionObj.deductionList.Count; i++)
                            {

                                DataSet deductionDs = db.GetDeductionFromDesc(deductionObj.deductionList[i].deductionuid);

                                if (deductionDs.Tables[0].Rows.Count == 0)
                                {
                                    return Json(new
                                    {
                                        Status = "Failure",
                                        Message = "Error:  deducion id " + deductionObj.deductionList[i].deductionuid + " is invalid"
                                    });
                                }
                            }
                        }
                        //added on 29/09/2022
                        if (AdditionObj.additionList != null)
                        {
                            for (int i = 0; i < AdditionObj.additionList.Count; i++)
                            {

                                DataSet additionDs = db.GetAdditionFromDesc(AdditionObj.additionList[i].additionuid);

                                if (additionDs.Tables[0].Rows.Count == 0)
                                {
                                    return Json(new
                                    {
                                        Status = "Failure",
                                        Message = "Error:  addition id " + AdditionObj.additionList[i].additionuid + " is invalid"
                                    });
                                }
                            }
                        }
                        //
                        for (int t = 0; t < raBillNumber.ToString().Split(',').Length; t++)
                        {
                            invoiceforRaBill = db.GetRAbillPresentTotalAmount_by_RABill_UID(new Guid(raBillNumber.ToString().Split(',')[t].ToString()));
                            if (invoiceforRaBill == -123)
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:Rabilluid-" + raBillNumber.ToString().Split(',')[t].ToString() + " not exists"
                                });
                            }
                            invoiceAmount += db.GetRAbillPresentTotalAmount_by_RABill_UID(new Guid(raBillNumber.ToString().Split(',')[t].ToString()));
                        }
                        
                        string sDate1 = "";
                        DateTime CDate1 = DateTime.Now;
                        string percentage = "0";

                        if (DateTime.TryParse(invoiceDate, out DateTime sDate))
                        {
                            sDate1 = sDate.ToString("dd/MM/yyyy");
                            sDate1 = db.ConvertDateFormat(sDate1);
                            CDate1 = Convert.ToDateTime(sDate1);

                            string Currecncy_CultureInfo = "";
                            if (currency == "INR")
                            {
                                Currecncy_CultureInfo = "en-IN";
                            }
                            else if (currency == "USD")
                            {
                                Currecncy_CultureInfo = "en-US";
                            }
                            else
                            {
                                Currecncy_CultureInfo = "ja-JP";
                            }
                            int cnt = db.InvoiceMaster_InsertorUpdate(InvoiceMaster_UID, new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), invoiceNumber, "", CDate1, (currency == "INR") ? "&#x20B9;" : (currency == "USD") ? "&#36;" : "&#165;", Currecncy_CultureInfo);
                            if (cnt > 0)
                            {
                                string[] raBillsList = raBillNumber.ToString().Split(',');
                                for (int p = 0; p < raBillsList.Length; p++)
                                {
                                    int cntRAbill = invoice.Invoice_RABills_Insert(Guid.NewGuid(), InvoiceMaster_UID, new Guid(raBillsList[p].ToString()), CDate1);
                                    //int cnt= dbObj.AddRABillNumber_Invoice(hidInvoiceUId.Value,ddlRabillNumber.SelectedValue.ToString());
                                    if (cntRAbill > 0)
                                    {
                                        DataSet ds = invoice.GetInvoiceDeduction_by_InvoiceMaster_UID_With_Name(InvoiceMaster_UID);
                                        if (ds.Tables[0].Rows.Count > 0)
                                        {
                                            float Mobilization = 0;
                                            DataSet dsInvoice = invoice.GetInvoiceMaster_by_InvoiceMaster_UID(InvoiceMaster_UID);
                                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                            {
                                                float Percent = float.Parse(ds.Tables[0].Rows[i]["Percentage"].ToString());
                                                float InvoiceAmount = float.Parse(dsInvoice.Tables[0].Rows[0]["Invoice_TotalAmount"].ToString());
                                                if (i == 0)
                                                {
                                                    if (Percent > 0)
                                                    {
                                                        float finalamount = (InvoiceAmount * Percent) / 100;
                                                        Mobilization = finalamount;
                                                    }
                                                    else
                                                    {
                                                        Mobilization = InvoiceAmount;
                                                    }

                                                    int cnt1 = invoice.InvoiceDeduction_Amount_Update(new Guid(ds.Tables[0].Rows[i]["Invoice_DeductionUID"].ToString()), new Guid(ds.Tables[0].Rows[i]["InvoiceMaster_UID"].ToString()), Mobilization);
                                                    if (cnt1 <= 0)
                                                    {
                                                        sError = true;
                                                        ErrorText = "There is a problem with this feature. Please contact system admin.";
                                                        //  Page.ClientScript.RegisterStartupScript(Page.GetType(), "CLOSE", "<script language='javascript'>alert('There is a problem with this feature. Please contact system admin.');</script>");
                                                    }

                                                }
                                                else
                                                {
                                                    float finalamount = 0;
                                                    if (ds.Tables[0].Rows[i]["DeductionsDescription"].ToString() == "SGST" || ds.Tables[0].Rows[i]["DeductionsDescription"].ToString() == "CGST" || ds.Tables[0].Rows[i]["DeductionsDescription"].ToString() == "GST")
                                                    {
                                                        string sVal = invoice.GetGST_Calculation_Value("GST Calculation");
                                                        if (sVal != "" && !sVal.StartsWith("Error"))
                                                        {
                                                            finalamount = (Mobilization / float.Parse(sVal));
                                                            finalamount = (finalamount * Percent) / 100;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        finalamount = (Mobilization * Percent) / 100;
                                                    }

                                                    int cnt1 = invoice.InvoiceDeduction_Amount_Update(new Guid(ds.Tables[0].Rows[i]["Invoice_DeductionUID"].ToString()), new Guid(ds.Tables[0].Rows[i]["InvoiceMaster_UID"].ToString()), finalamount);
                                                    if (cnt1 <= 0)
                                                    {
                                                        sError = true;
                                                        ErrorText = "There is a problem with this feature. Please contact system admin.";
                                                        // Page.ClientScript.RegisterStartupScript(Page.GetType(), "CLOSE", "<script language='javascript'>alert('There is a problem with this feature. Please contact system admin.');</script>");
                                                    }
                                                }
                                            }
                                        }
                                        //Page.ClientScript.RegisterStartupScript(Page.GetType(), "CLOSE", "<script language='javascript'>parent.location.href=parent.location.href;</script>");
                                    }
                                    else if (cntRAbill == -1)
                                    {
                                        sError = true;
                                        ErrorText = "'RA Bill already exists for the invoice. Try with different RA Bill No.";
                                        // Page.ClientScript.RegisterStartupScript(Page.GetType(), "CLOSE", "<script language='javascript'>alert('RA Bill already exists for the invoice. Try with different RA Bill No.');</script>");
                                    }
                                }
                                if (deductionObj.deductionList != null)
                                {
                                    for (int i = 0; i < deductionObj.deductionList.Count; i++)
                                    {
                                        if (deductionObj.deductionList[i].deductionMode.ToLower() == "percentage")
                                        {
                                            percentage = "0";
                                            deductionObj.deductionList[i].Value = ((invoiceAmount / 100) * Convert.ToDecimal(deductionObj.deductionList[i].Value)).ToString();
                                        }
                                        else
                                        {
                                            percentage = deductionObj.deductionList[i].Value;
                                        }
                                        DataSet deductionDs = db.GetDeductionFromDesc(deductionObj.deductionList[i].deductionuid);

                                        if (deductionDs.Tables[0].Rows.Count > 0)
                                        {                                           
                                            db.InvoiceDeduction_InsertorUpdate(Guid.NewGuid(), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), InvoiceMaster_UID, new Guid(deductionDs.Tables[0].Rows[0]["UID"].ToString()), float.Parse(deductionObj.deductionList[i].Value), float.Parse(percentage), (currency == "INR") ? "&#x20B9;" : (currency == "USD") ? "&#36;" : "&#165;", Currecncy_CultureInfo, deductionObj.deductionList[i].deductionMode);
                                        }
                                    }
                                }
                                //
                                if (AdditionObj.additionList != null)
                                {
                                    for (int i = 0; i < AdditionObj.additionList.Count; i++)
                                    {
                                        if (AdditionObj.additionList[i].additionMode.ToLower() == "percentage")
                                        {
                                            percentage = "0";
                                            AdditionObj.additionList[i].Value = ((invoiceAmount / 100) * Convert.ToDecimal(AdditionObj.additionList[i].Value)).ToString();
                                        }
                                        else
                                        {
                                            percentage = AdditionObj.additionList[i].Value;
                                        }
                                        DataSet additionDs = db.GetAdditionFromDesc(AdditionObj.additionList[i].additionuid);

                                        if (additionDs.Tables[0].Rows.Count > 0)
                                        {
                                            db.InsertorUpdateInvoiceAdditions(Guid.NewGuid(), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), InvoiceMaster_UID, new Guid(additionDs.Tables[0].Rows[0]["UID"].ToString()), float.Parse(AdditionObj.additionList[i].Value), float.Parse(percentage), (currency == "INR") ? "&#x20B9;" : (currency == "USD") ? "&#36;" : "&#165;", Currecncy_CultureInfo, AdditionObj.additionList[i].additionMode);
                                        }
                                    }
                                }
                                //
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invoice already exists";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Date is not correct format.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Project Name not exists.";
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
                InvouceUid = InvoiceMaster_UID,

            });
        }

        [Authorize]
        [HttpPost]
        [Route("api/Financial/AddPaymentDetails")]
        public IHttpActionResult AddPaymentDetails()
        {
            //Insert into WebAPITransctions table
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + ";WorkPackageName=" + httpRequest.Params["WorkpackageName"] + "; Month=" + httpRequest.Params["Month"] + ";Year=" + httpRequest.Params["Year"] +
                    ";InvoiceNo=" + httpRequest.Params["InvoiceNo"] + ";InvoiceAmount=" + httpRequest.Params["InvoiceAmount"] + ";NetAmount=" + httpRequest.Params["NetAmount"] + ";PaymentDate=" + httpRequest.Params["PaymentDate"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                DataSet ds = new DataSet();
                DataSet dsInvDeduc = new DataSet();
                DataSet dsInvAdd = new DataSet();
                var identity = (ClaimsIdentity)User.Identity;
                var ProjectName = httpRequest.Params["ProjectName"];
                var WorkpackageName = httpRequest.Params["WorkpackageName"];
                var InvoiceNo = httpRequest.Params["InvoiceNo"];
                var Month = httpRequest.Params["Month"];
                int Year =int.Parse(httpRequest.Params["Year"]);
                Guid FinMonthUID = Guid.NewGuid();
                Guid InvoiceUID = Guid.NewGuid();
                Guid DeductionUID = new Guid();
                Guid PaymentUID = Guid.NewGuid();
                float TotalDeductions = 0.0f;
                float DeducAmnt = 0.0f;
                float Deducper = 0.0f;
                //
                float TotalAdditions = 0.0f;
                float AdditionsAmnt = 0.0f;
                float Additionsper = 0.0f;
                float Amount = float.Parse(httpRequest.Params["InvoiceAmount"]);
                float NetAmnt = float.Parse(httpRequest.Params["NetAmount"]);
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
                    var pExists = db.ProjectNameExists(ProjectName);
                    if (pExists != "")
                    {
                        var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                        if (wExists != "")
                        {
                           

                            if(db.GetFinMonthUID(new Guid(wExists),Month,Year) != "00000000-0000-0000-0000-000000000000")
                            {
                                FinMonthUID =new Guid(db.GetFinMonthUID(new Guid(wExists), Month, Year));

                                if (db.GetInvoiceUIDFromInvoiceNo(new Guid(wExists),InvoiceNo) != "00000000-0000-0000-0000-000000000000")
                                {
                                    InvoiceUID = new Guid(db.GetInvoiceUIDFromInvoiceNo(new Guid(wExists), InvoiceNo));

                                    if(db.checkInvoicePaid(InvoiceUID) > 0)
                                    {
                                        var resultmain1 = new
                                        {
                                            Status = "Failure",
                                            Message = "Payment already done for the Invoice No !",
                                        };
                                        return Json(resultmain1);
                                    }

                                    dsInvDeduc = db.GetInvoiceDeductions(InvoiceUID);

                 
                                    foreach (DataRow dr in dsInvDeduc.Tables[0].Rows)
                                    {
                                        DeducAmnt = float.Parse(dr["Amount"].ToString());
                                        Deducper = float.Parse(dr["Percentage"].ToString());
                                        ds = db.GetDeductionFromDesc(dr["DeductionsDescription"].ToString());
                                        if (ds.Tables[0].Rows.Count > 0)
                                        {
                                            DeductionUID = new Guid(ds.Tables[0].Rows[0]["UID"].ToString());
                                        }
                                        TotalDeductions = TotalDeductions + DeducAmnt;
                                       
                                        db.InsertRABillsDeductions(Guid.NewGuid(), PaymentUID, DeductionUID, DeducAmnt, Deducper);
                                    }

                                    //added on 29/09/2022 for Invoice Additions
                                    dsInvAdd = db.GetInvoiceAdditions_by_InvoiceMaster_UID(InvoiceUID);


                                    foreach (DataRow dr in dsInvAdd.Tables[0].Rows)
                                    {
                                        AdditionsAmnt = float.Parse(dr["Amount"].ToString());
                                        Additionsper = float.Parse(dr["Percentage"].ToString());
                                        //ds = db.GetAdditionFromDesc(dr["Description"].ToString());
                                        //if (ds.Tables[0].Rows.Count > 0)
                                        //{
                                        //    DeductionUID = new Guid(ds.Tables[0].Rows[0]["UID"].ToString());
                                        //}
                                        TotalAdditions = TotalAdditions + AdditionsAmnt;
                                        
                                    }


                                    string sDate2 = httpRequest.Params["PaymentDate"];
                                   //
                                    sDate2 = db.ConvertDateFormat(sDate2);
                                    DateTime CDate2 = Convert.ToDateTime(sDate2);
                                    db.InsertRABillPayments(PaymentUID, InvoiceUID, "Invoice Amount", Amount, TotalDeductions, NetAmnt, FinMonthUID, CDate2, TotalAdditions);
                                    var resultmain = new
                                    {
                                        Status = "Success",
                                        PaymentUID = PaymentUID,
                                        Message = "Successfully Updated Payment details !",
                                    };
                                    return Json(resultmain);
                                }
                                else
                                {

                                    var resultmain = new
                                    {
                                        Status = "Failure",
                                        Message = "Invoice Number does not Exists !",
                                    };
                                    return Json(resultmain);
                                }
                            }
                            else
                            {
                                var resultmain = new
                                {
                                    Status = "Failure",
                                    Message = "Month and Year does not Exists !",
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

        // added for  venkat on 19/02/2022
        /// <summary>
        /// Updated by venkat on 19 Feb 2022
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Financial/AddIssues")]
        public IHttpActionResult AddIssues()
        {
            //added on 17/04/2023 after salahuddins call to not aloow contractors to update status
            return Json(new
            {
                Status = "Failure",
                Message = "This API is no longer supported as requirement from the Client"
            });

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
                string postData = "ProjectName=" + httpRequest.Params["project name"] + ";Issue description=" + httpRequest.Params["Issue description"] + ";assigned user=" + httpRequest.Params["assigned user"] +
                    ";assigned date=" + httpRequest.Params["assigned date"] + ";reporting user=" + httpRequest.Params["reporting user"] + ";reporting date=" + httpRequest.Params["reporting date"] +
                    ";approving user=" + httpRequest.Params["approving user"] + ";issue proposed  close date=" + httpRequest.Params["issue proposed  close date"] + ";remarks=" + httpRequest.Params["remarks"] +
                    "issue document=" + httpRequest.Params["issue document"];
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(httpRequest.Params["project name"]) || String.IsNullOrEmpty(httpRequest.Params["Issue description"]) || 
                      String.IsNullOrEmpty(httpRequest.Params["reporting user"]) || String.IsNullOrEmpty(httpRequest.Params["reporting date"]) )
                {
                   // db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:Mandatory fields are missing");
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    });
                }


                var identity = (ClaimsIdentity)User.Identity;

                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var projectName = httpRequest.Params["project name"];
                    DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                    if (dtWorkPackages.Rows.Count > 0)
                    {
                        DataTable dtIssuesExist = db.getIssuesByDescription( new Guid(dtWorkPackages.Rows[0]["WorkpackageUid"].ToString()), httpRequest.Params["Issue description"].ToString());
                        if(dtIssuesExist.Rows.Count>0)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Issue already Exists"

                            });
                        }
                        //DataSet dsUserDetails = db.getUserDetails_by_EmailID(httpRequest.Params["approving user"].ToString());
                        //if (dsUserDetails.Tables[0].Rows.Count == 0)
                        //{
                        //   // db.WebAPITransctionUpdate(transactionUid, "Failure", "Approve user is not " +
                        //        //"available");
                        //    return Json(new
                        //    {
                        //        Status = "Failure",
                        //        Message = "Approve user is not available"

                        //    });

                        //}
                       // string approveUserUid = dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString();

                        DataSet dsUserDetails = db.getUserDetails_by_EmailID(httpRequest.Params["reporting user"].ToString());

                        if (dsUserDetails.Tables[0].Rows.Count == 0)
                        {
                          //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Reporting user is not  available");
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Reporting user is not available"

                            });

                        }
                        if(dsUserDetails.Tables[0].Rows[0]["IsContractor"].ToString() != "Y")
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Not Authorized to add issue"

                            });
                        }
                        string reportUserUid = dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString();
                        //dsUserDetails = db.getUserDetails_by_EmailID(httpRequest.Params["assigned user"].ToString());
                        //if (dsUserDetails.Tables[0].Rows.Count == 0)
                        //{
                        //   // db.WebAPITransctionUpdate(transactionUid, "Failure", "Assigned user is not available");
                        //    return Json(new
                        //    {
                        //        Status = "Failure",
                        //        Message = "Assigned user is not available"

                        //    });

                        //}
                        //string assignedUserUid = dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString();
                        DateTime reportDate = DateTime.Now;
                        if (!string.IsNullOrEmpty(httpRequest.Params["reporting date"]))
                        {
                            string tmpDate = db.ConvertDateFormat(httpRequest.Params["reporting date"]);
                            reportDate = Convert.ToDateTime(tmpDate);
                        }
                       
                        if (DateTime.TryParse(reportDate.ToString(), out  reportDate))// &&
                           // DateTime.TryParse(httpRequest.Params["assigned date"], out DateTime assignedDate) &&
                           // DateTime.TryParse(httpRequest.Params["issue proposed  close date"], out DateTime proposedClosedDate))
                        {

                            //AddDays need to removed when upload in indian server
                            if (reportDate.Date > DateTime.Now.Date.AddDays(1))
                            {
                              //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Reporting Date should be less then assigned date");
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Reporting Date should not be greater then today date"

                                });
                            }
                            //if (reportDate > assignedDate)
                            //{
                            //   // db.WebAPITransctionUpdate(transactionUid, "Failure", "Reporting Date should be less then assigned date");
                            //    return Json(new
                            //    {
                            //        Status = "Failure",
                            //        Message = "Reporting Date should be less then assigned date"

                            //    });
                            //}
                            string DecryptPagePath = "";
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                if (httpPostedFile != null)
                                {
                                    string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + "/Documents/Issues/";
                                    string FileDirectory = "~/Documents/Issues/";
                                    if (!Directory.Exists(sDocumentPath))
                                    {
                                        Directory.CreateDirectory(sDocumentPath);
                                    }

                                    string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                    string Extn = Path.GetExtension(httpPostedFile.FileName);
                                    httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + Extn);
                                    //FileUploadDoc.SaveAs(Server.MapPath("~/Documents/Encrypted/" + sDocumentUID + "_" + txtDocName.Text + "_1"  + "_enp" + InputFile));
                                    string savedPath = sDocumentPath + "/" + sFileName + Extn;
                                    DecryptPagePath = sDocumentPath + "/" + sFileName + "_DE" + Extn;
                                    db.EncryptFile(savedPath, DecryptPagePath);
                                    DecryptPagePath = FileDirectory + "/" + sFileName + "_DE" + Extn;
                                }
                            }
                            string remarks = "";
                            //int Cnt = db.InsertorUpdateIssues(issue_uid, Guid.Empty, httpRequest.Params["Issue description"].ToString(), reportDate, new Guid(reportUserUid), new Guid(assignedUserUid), assignedDate, proposedClosedDate, new Guid(approveUserUid), proposedClosedDate, "Open", httpRequest.Params["remarks"].ToString(), new Guid(dtWorkPackages.Rows[0]["WorkpackageUid"].ToString()), new Guid(dtWorkPackages.Rows[0]["ProjectUid"].ToString()), DecryptPagePath);
                            if(!string.IsNullOrEmpty( httpRequest.Params["remarks"]))
                            {
                                remarks = httpRequest.Params["remarks"].ToString();
                            }
                            int Cnt = 0;
                            dsUserDetails = db.getUserDetails_by_EmailID("bm.srinivasamurthy@njsei.com");
                            if (projectName.ToUpper() == "CP-10" && dsUserDetails.Tables[0].Rows.Count>0)
                            {
                                 Cnt = db.InsertorUpdateIssues(issue_uid, Guid.Empty, httpRequest.Params["Issue description"].ToString(), reportDate, new Guid(reportUserUid), new Guid(dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString()), DateTime.Now, DateTime.Now.AddDays(10), new Guid(dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString()), DateTime.Now.AddDays(10), "Open", httpRequest.Params["remarks"].ToString(), new Guid(dtWorkPackages.Rows[0]["WorkpackageUid"].ToString()), new Guid(dtWorkPackages.Rows[0]["ProjectUid"].ToString()), DecryptPagePath);
                            }
                            else
                            {
                                 Cnt = db.InsertorUpdateIssues(issue_uid, Guid.Empty, httpRequest.Params["Issue description"].ToString(), reportDate, new Guid(reportUserUid), "Open", remarks, new Guid(dtWorkPackages.Rows[0]["WorkpackageUid"].ToString()), new Guid(dtWorkPackages.Rows[0]["ProjectUid"].ToString()), DecryptPagePath);
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
                      //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Not Authorized IP address");

                        sError = true;
                        ErrorText = "ProjectName not exists";

                    }
                }
                else
                {
                  //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Not Authorized IP address");
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
              //  db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
           // db.WebAPITransctionUpdate(transactionUid, "Success", ErrorText);
            return Json(new
            {
                Status = "Success",
                IssueUid = issue_uid,
                Message = ErrorText

            });
        }

        //[Authorize]
        //[HttpPost]
        //[Route("api/Financial/UpdateIssueStatus")]
        //public IHttpActionResult UpdateIssueStatus()
        //{
        //    bool sError = false;
        //    string ErrorText = "";
        //    Guid InvoiceMaster_UID = Guid.NewGuid();
        //    var httpRequest = HttpContext.Current.Request;
        //    var transactionUid = Guid.NewGuid();
        //    bool transactionStatus = false;
        //    DateTime updatingDate = DateTime.MinValue;
        //    Guid issue_remarks_uid = Guid.NewGuid(); 
        //    try
        //    {
        //        //Insert into WebAPITransctions table
        //        var BaseURL = HttpContext.Current.Request.Url.ToString();
        //        string postData = "ProjectName=" + httpRequest.Params["project name"] + ";issue uid=" + httpRequest.Params["issue uid"] + ";issue status=" + httpRequest.Params["issue status"] + ";issue status update date=" + httpRequest.Params["updating date"] +
        //            ";Issue status update user id=" + httpRequest.Params["updating user id"] + ";issue status document=" + httpRequest.Params["issue status document"] + ";remarks=" + httpRequest.Params["remarks"];
        //        db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

        //        if (String.IsNullOrEmpty(httpRequest.Params["project name"]) || String.IsNullOrEmpty(httpRequest.Params["issue uid"]) || String.IsNullOrEmpty(httpRequest.Params["issue status"]) || String.IsNullOrEmpty(httpRequest.Params["updating user id"])
        //            || string.IsNullOrEmpty(httpRequest.Params["updating date"]))
        //        {
        //           // db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:Mandatory fields are missing");
        //            return Json(new
        //            {
        //                Status = "Failure",
        //                Message = "Error:Mandatory fields are missing"
        //            });
        //        }


        //        var identity = (ClaimsIdentity)User.Identity;

        //        if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
        //        {
        //            var projectName = httpRequest.Params["project name"];
        //            DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
        //            if (dtWorkPackages.Rows.Count == 0)
        //            {
        //                sError = true;
        //                ErrorText = "ProjectName not exists";
        //            }
        //            else
        //            {
        //                DataSet dsIssues = db.getIssuesList_by_UID(new Guid(httpRequest.Params["issue uid"]));
        //                if (dsIssues.Tables[0].Rows.Count == 0)
        //                {
        //                    sError = true;
        //                    ErrorText = "Issue uid is not available";
        //                }
        //                else
        //                {

        //                    DataSet dsUserDetails = db.getUserDetails_by_EmailID(httpRequest.Params["updating user id"].ToString());
        //                    if (dsUserDetails.Tables[0].Rows.Count == 0)
        //                    {
        //                        sError = true;
        //                        ErrorText = "User is not available";
        //                    }
        //                    else
        //                    {
        //                        DateTime reportDate = DateTime.Now;
        //                        if (!string.IsNullOrEmpty(httpRequest.Params["updating date"]))
        //                        {
        //                            string tmpDate = db.ConvertDateFormat(httpRequest.Params["updating date"]);
        //                            reportDate = Convert.ToDateTime(tmpDate);
        //                        }

        //                        if (DateTime.TryParse(reportDate.ToString(), out updatingDate))
        //                        {
        //                            string userUid = dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString();
        //                            if (userUid.ToString().ToUpper() == dsIssues.Tables[0].Rows[0]["Assigned_User"].ToString().ToUpper() || userUid.ToString().ToUpper() == dsIssues.Tables[0].Rows[0]["Approving_User"].ToString().ToUpper())
        //                            {
        //                                if (dsIssues.Tables[0].Rows[0]["Assigned_User"].ToString() == dsIssues.Tables[0].Rows[0]["Approving_User"].ToString())
        //                                {
        //                                    if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "Open" && (httpRequest.Params["issue status"].ToString() == "In-Progress" ||
        //                                               httpRequest.Params["issue status"].ToString() == "Close" || httpRequest.Params["issue status"].ToString() == "Reject"))
        //                                    {
        //                                        transactionStatus = true;
        //                                    }
        //                                    else if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "In-Progress" && (httpRequest.Params["issue status"].ToString() == "In-Progress" ||
        //                                              httpRequest.Params["issue status"].ToString() == "Close"))
        //                                    {

        //                                        transactionStatus = true;
        //                                    }
        //                                    else
        //                                    {
        //                                        sError = true;
        //                                        ErrorText = "User not allowed to update the status";
        //                                    }

        //                                }
        //                                else
        //                                {
        //                                    if (dsIssues.Tables[0].Rows[0]["Assigned_User"].ToString().ToUpper() == userUid.ToString().ToUpper())
        //                                    {
        //                                        if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "Open" && httpRequest.Params["issue status"].ToString() == "In-Progress")
        //                                        {
        //                                            transactionStatus = true;
        //                                        }
        //                                        else if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "In-Progress" && httpRequest.Params["issue status"].ToString() == "In-Progress")
        //                                        {
        //                                            transactionStatus = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            sError = true;
        //                                            ErrorText = "User not allowed to update the status";
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "In-Progress" && (httpRequest.Params["issue status"].ToString() == "Close" ||
        //                                            httpRequest.Params["issue status"].ToString() == "Reject"))
        //                                        {
        //                                            transactionStatus = true;
        //                                        }
        //                                        else
        //                                        {
        //                                            sError = true;
        //                                            ErrorText = "User not allowed to update the status";
        //                                        }

        //                                    }
        //                                }

        //                            }

        //                            else
        //                            {
        //                                sError = true;
        //                                ErrorText = "Issue uid and userid doen't match";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            sError = true;
        //                            ErrorText = "Update date is not correct format";
        //                        }

        //                    }
        //                    if (transactionStatus)
        //                    {
        //                        string DecryptPagePath = "";
        //                        for (int i = 0; i < httpRequest.Files.Count; i++)
        //                        {
        //                            HttpPostedFile httpPostedFile = httpRequest.Files[i];

        //                            if (httpPostedFile != null)
        //                            {
        //                                string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + "/Documents/Issues/";
        //                                string FileDirectory = "/Documents/Issues/";
        //                                if (!Directory.Exists(sDocumentPath))
        //                                {
        //                                    Directory.CreateDirectory(sDocumentPath);
        //                                }
        //                                string fileName = Path.GetFileName(httpPostedFile.FileName);
        //                                string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
        //                                string Extn = Path.GetExtension(httpPostedFile.FileName);
        //                                httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + Extn);
        //                                //FileUploadDoc.SaveAs(Server.MapPath("~/Documents/Encrypted/" + sDocumentUID + "_" + txtDocName.Text + "_1"  + "_enp" + InputFile));
        //                                string savedPath = sDocumentPath + "/" + sFileName + Extn;
        //                                DecryptPagePath = sDocumentPath + "/" + sFileName + "_DE" + Extn;
        //                                db.EncryptFile(savedPath, DecryptPagePath);
        //                                DecryptPagePath = FileDirectory + "/" + sFileName + "_DE" + Extn;
        //                                //added on 05/05/2022
        //                                db.InsertUploadedDocument(sFileName + "_DE" + Extn, FileDirectory, issue_remarks_uid.ToString());
        //                            }
        //                        }

        //                        int cnt = db.Issues_Status_Remarks_Insert(issue_remarks_uid, new Guid(httpRequest.Params["issue uid"]), httpRequest.Params["issue status"], httpRequest.Params["Remarks"], DecryptPagePath, updatingDate);
        //                        if (cnt > 0)
        //                        {
        //                            sError = false;
        //                            ErrorText = "Updated Successfully";
        //                        }
        //                        else
        //                        {
        //                            sError = true;
        //                            ErrorText = "Status is not Updated,Please contact administrator";
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //        else
        //        {
        //            sError = true;
        //            ErrorText = "Not Authorized IP address";

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sError = true;
        //        ErrorText = ex.Message;
        //    }
        //    if (sError)
        //    {
        //       // db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
        //        return Json(new
        //        {
        //            Status = "Failure",
        //            Message = "Error:" + ErrorText
        //        });
        //    }
        //   // db.WebAPITransctionUpdate(transactionUid, "Success", ErrorText);
        //    return Json(new
        //    {
        //        Status = "Success",
        //        Message = ErrorText

        //    });

        //}





        [Authorize]
        [HttpPost]
        [Route("api/Financial/UpdateIssueStatus")]
        public IHttpActionResult UpdateIssueStatus()  //changed on 15/09/2022 
        {
            //added on 17/04/2023 after salahuddins call to not aloow contractors to update status
            return Json(new
            {
                Status = "Failure",
                Message = "This API is no longer supported as requirement from the Client"
            });

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
                string postData = "ProjectName=" + httpRequest.Params["project name"] + ";issue uid=" + httpRequest.Params["issue uid"] + ";issue status=" + httpRequest.Params["issue status"] + ";issue status update date=" + httpRequest.Params["updating date"] +
                    ";Issue status update user id=" + httpRequest.Params["updating user id"] + ";issue status document=" + httpRequest.Params["issue status document"] + ";remarks=" + httpRequest.Params["remarks"];
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(httpRequest.Params["project name"]) || String.IsNullOrEmpty(httpRequest.Params["issue uid"]) || String.IsNullOrEmpty(httpRequest.Params["issue status"]) || String.IsNullOrEmpty(httpRequest.Params["updating user id"])
                    || string.IsNullOrEmpty(httpRequest.Params["updating date"]))
                {
                    // db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:Mandatory fields are missing");
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    });
                }
                //added on 22/02/2023
                if(httpRequest.Params["issue status"].ToLower() == "close")
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Not allowed to close the status"
                    });
                }
                //
                var identity = (ClaimsIdentity)User.Identity;

                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var projectName = httpRequest.Params["project name"];
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
                        DataSet dsIssues = db.getIssuesList_by_UID(new Guid(httpRequest.Params["issue uid"]));
                        if (dsIssues.Tables[0].Rows.Count == 0)
                        {
                            sError = true;
                            ErrorText = "Issue uid is not available";
                        }
                        else
                        {

                            DataSet dsUserDetails = db.getUserDetails_by_EmailID(httpRequest.Params["updating user id"].ToString());
                            if (dsUserDetails.Tables[0].Rows.Count == 0)
                            {
                                sError = true;
                                ErrorText = "User is not available";
                            }
                            else
                            {
                                DateTime reportDate = DateTime.Now;
                                if (!string.IsNullOrEmpty(httpRequest.Params["updating date"]))
                                {
                                    string tmpDate = db.ConvertDateFormat(httpRequest.Params["updating date"]);
                                    reportDate = Convert.ToDateTime(tmpDate);
                                }

                                if (DateTime.TryParse(reportDate.ToString(), out updatingDate))
                                {
                                    userUid = dsUserDetails.Tables[0].Rows[0]["UserUID"].ToString();
                                    
                                        
                                            if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "Open" && (httpRequest.Params["issue status"].ToString() == "In-Progress" ||
                                                       httpRequest.Params["issue status"].ToString() == "Close" || httpRequest.Params["issue status"].ToString() == "Reject"))
                                            {
                                                transactionStatus = true;
                                            }
                                            else if (dsIssues.Tables[0].Rows[0]["Issue_Status"].ToString() == "In-Progress" && (httpRequest.Params["issue status"].ToString() == "In-Progress" ||
                                                      httpRequest.Params["issue status"].ToString() == "Close"))
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
                                for (int i = 0; i < httpRequest.Files.Count; i++)
                                {
                                    HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                    if (httpPostedFile != null)
                                    {
                                        string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + "/Documents/Issues/";
                                        string FileDirectory = "/Documents/Issues/";
                                        if (!Directory.Exists(sDocumentPath))
                                        {
                                            Directory.CreateDirectory(sDocumentPath);
                                        }
                                        string fileName = Path.GetFileName(httpPostedFile.FileName);
                                        string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                        string Extn = Path.GetExtension(httpPostedFile.FileName);
                                        httpPostedFile.SaveAs(sDocumentPath + "/" + sFileName + Extn);
                                        //FileUploadDoc.SaveAs(Server.MapPath("~/Documents/Encrypted/" + sDocumentUID + "_" + txtDocName.Text + "_1"  + "_enp" + InputFile));
                                        string savedPath = sDocumentPath + "/" + sFileName + Extn;
                                        DecryptPagePath = sDocumentPath + "/" + sFileName + "_DE" + Extn;
                                        //
                                        files_path = files_path + DecryptPagePath + ",";
                                        //
                                        db.EncryptFile(savedPath, DecryptPagePath);
                                        DecryptPagePath = FileDirectory + "/" + sFileName + "_DE" + Extn;
                                       
                                        //added on 05/05/2022
                                        db.InsertUploadedDocument(sFileName + "_DE" + Extn, FileDirectory, issue_remarks_uid.ToString());
                                    }
                                }

                                int cnt = db.Issues_Status_Remarks_Insert(issue_remarks_uid, new Guid(httpRequest.Params["issue uid"]), httpRequest.Params["issue status"], httpRequest.Params["Remarks"], DecryptPagePath, updatingDate);
                                if (cnt > 0)
                                {
                                    DataSet ds_issue = db.GetIssueByIssueUid(httpRequest.Params["issue uid"]);


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
                                                    "<tr><td><b>Issue Status </b></td><td style='text-align:center;'><b>:</b></td><td>" + httpRequest.Params["issue status"] + "</td></tr>" +
                                                    "<tr><td><b>Remarks </b></td><td style='text-align:center;'><b>:</b></td><td>" + httpRequest.Params["Remarks"] + "</td></tr>" +
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
                // db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:" + ErrorText
                });
            }
            // db.WebAPITransctionUpdate(transactionUid, "Success", ErrorText);
            return Json(new
            {
                Status = "Success",
                Message = ErrorText

            });

        }

        /// <summary>
        /// Updated by venkat on 19 Feb 2022
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[Route("api/Financial/GetIssueStatus")]
        //public IHttpActionResult GetIssueStatus()
        //{
        //    bool sError = false;
        //    string ErrorText = "";
        //    Guid InvoiceMaster_UID = Guid.NewGuid();
        //    var httpRequest = HttpContext.Current.Request;
        //    var transactionUid = Guid.NewGuid();
        //    DataTable dtIssueList = new DataTable();
        //    DataTable dtResponse = new DataTable();
        //    try
        //    {
        //        //Insert into WebAPITransctions table
        //        var BaseURL = HttpContext.Current.Request.Url.ToString();
        //        string postData = "ProjectName=" + httpRequest.Params["project name"] + ";IssueUid=" + httpRequest.Params["IssueUid"];
        //        db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

        //        if (String.IsNullOrEmpty(httpRequest.Params["project name"]) || String.IsNullOrEmpty(httpRequest.Params["IssueUid"]))
        //        {
        //            //db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:Mandatory fields are missing");
        //            return Json(new
        //            {
        //                Status = "Failure",
        //                Message = "Error:Mandatory fields are missing"
        //            });
        //        }

        //        var identity = (ClaimsIdentity)User.Identity;
                
        //        if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
        //        {
        //            var projectName = httpRequest.Params["project name"];
        //            DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
        //            if (dtWorkPackages.Rows.Count > 0)
        //            {
        //                DataSet dsIssuesList = db.GetIssueStatus_by_Issue_Uid(new Guid(httpRequest.Params["IssueUid"]));
        //                if (dsIssuesList.Tables[0].Rows.Count > 0)
        //                {
        //                    dtIssueList.Columns.Add("Status");
        //                    dtIssueList.Columns.Add("Updated Date");
        //                    dtIssueList.Columns.Add("FileName");
        //                    dtIssueList.Columns.Add("Remarks");
        //                    dtIssueList.Columns.Add("base64");

        //                    for (int i = 0; i < dsIssuesList.Tables[0].Rows.Count; i++)
        //                    {
        //                        DataRow dtNewRow = dtIssueList.NewRow();
        //                        dtNewRow["Status"] = dsIssuesList.Tables[0].Rows[i]["Issue_Status"].ToString();
        //                        dtNewRow["Remarks"] = dsIssuesList.Tables[0].Rows[i]["Issue_Remarks"].ToString();
        //                        dtNewRow["Updated Date"] = Convert.ToDateTime(dsIssuesList.Tables[0].Rows[i]["IssueRemark_Date"]).ToString("dd MMM yyyy");
        //                        dtNewRow["base64"] = "";
        //                        if (!string.IsNullOrEmpty(dsIssuesList.Tables[0].Rows[i]["Issue_Document"].ToString()))
        //                        {
        //                            try
        //                            {
        //                                //  string path = System.Web.Hosting.HostingEnvironment.MapPath(dsIssuesList.Tables[0].Rows[i]["Issue_Document"].ToString());
        //                                //  string fileExtension = System.IO.Path.GetExtension(dsIssuesList.Tables[0].Rows[i]["Issue_Document"].ToString());
        //                                // string outPath = dsIssuesList.Tables[0].Rows[i]["Issue_Document"].ToString() + "_download" + fileExtension;
        //                                // changed by zuber on 19/02/2022
        //                                string path = ConfigurationManager.AppSettings["DocumentsPath"] + dsIssuesList.Tables[0].Rows[i]["Issue_Document"].ToString().Remove(0, 2);
        //                                string fileName = System.IO.Path.GetFileName(ConfigurationManager.AppSettings["DocumentsPath"] + dsIssuesList.Tables[0].Rows[i]["Issue_Document"].ToString().Remove(0, 2));
        //                                dtNewRow["FileName"] = fileName;
        //                                string fileExtension = System.IO.Path.GetExtension(ConfigurationManager.AppSettings["DocumentsPath"] + dsIssuesList.Tables[0].Rows[i]["Issue_Document"].ToString().Remove(0, 2));
        //                                string outPath = ConfigurationManager.AppSettings["DocumentsPath"] + dsIssuesList.Tables[0].Rows[i]["Issue_Document"].ToString().Remove(0, 2) + "_download" + fileExtension;
        //                                //------------------------------------------------------------------------
        //                                db.DecryptFile(path, outPath);

        //                                Byte[] bytes = File.ReadAllBytes(outPath);
        //                                dtNewRow["base64"] = Convert.ToBase64String(bytes);
                                        
        //                            }
        //                            catch (Exception ex)
        //                            { }
        //                            //getExtension = System.IO.Path.GetExtension(dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString());
        //                            //fileName = System.IO.Path.GetFileName(dtLastestStatus.Tables[0].Rows[0]["filePath"].ToString());
        //                        }

        //                        dtIssueList.Rows.Add(dtNewRow);
        //                    }
        //                }
        //                    DataSet dsIssues=  db.getIssuesList_by_UID(new Guid(httpRequest.Params["IssueUid"]));
        //                    dtResponse.Columns.Add("issue name");
        //                    dtResponse.Columns.Add("issue uid");
        //                    dtResponse.Columns.Add("reporting user");
        //                    dtResponse.Columns.Add("reporting Date");
        //                    dtResponse.Columns.Add("Assigned User");
        //                    dtResponse.Columns.Add("Assigned Date");
        //                    dtResponse.Columns.Add("Proposed closure Date");
        //                    dtResponse.Columns.Add("Status", typeof(DataTable));
        //                   for (int i = 0; i < dsIssues.Tables[0].Rows.Count; i++)
        //                    {
        //                        DataRow dtNewRow = dtResponse.NewRow();
        //                        dtNewRow["issue uid"] = dsIssues.Tables[0].Rows[i]["Issue_Uid"].ToString();
        //                        dtNewRow["issue name"] = dsIssues.Tables[0].Rows[i]["Issue_Description"].ToString();
        //                        DataSet dsusers = db.getUserDetails_Contractor(new Guid(dsIssues.Tables[0].Rows[i]["Issued_User"].ToString()));
        //                        if (dsusers.Tables[0].Rows.Count > 0)
        //                        {
        //                            dtNewRow["reporting user"] = dsusers.Tables[0].Rows[0]["UserName"].ToString();
        //                        }
        //                        if(!string.IsNullOrEmpty(dsIssues.Tables[0].Rows[i]["Issue_Date"].ToString()))
        //                        {
        //                            dtNewRow["reporting date"] = Convert.ToDateTime(dsIssues.Tables[0].Rows[i]["Issue_Date"]).ToString("dd MMM yyyy");
        //                        }
        //                        dsusers=  db.getUserDetails_Contractor(new Guid(dsIssues.Tables[0].Rows[i]["Assigned_User"].ToString()));
        //                        if (dsusers.Tables[0].Rows.Count > 0)
        //                        {
        //                            if (dsusers.Tables[0].Rows[0]["IsContractor"].ToString() == "Y")
        //                            {
        //                                dtNewRow["assigned user"] = dsusers.Tables[0].Rows[0]["UserName"].ToString();
        //                            }
        //                            else
        //                            {
        //                               dtResponse.Columns.Remove("assigned user");
        //                            }
        //                        }
        //                        if (!string.IsNullOrEmpty(dsIssues.Tables[0].Rows[i]["Assigned_Date"].ToString()))
        //                        {
        //                            dtNewRow["assigned date"] = Convert.ToDateTime(dsIssues.Tables[0].Rows[i]["Assigned_Date"]).ToString("dd MMM yyyy");
        //                        }

        //                        if (!string.IsNullOrEmpty(dsIssues.Tables[0].Rows[i]["Issue_ProposedCloser_Date"].ToString()))
        //                        {
        //                            dtNewRow["proposed closure date"] = Convert.ToDateTime(dsIssues.Tables[0].Rows[i]["Issue_ProposedCloser_Date"]).ToString("dd MMM yyyy");
        //                        }
        //                        dtNewRow["status"] = dtIssueList;
        //                    dtResponse.Rows.Add(dtNewRow);
        //                    }
                            
        //                }
        //            else
        //            {
        //                sError = true;
        //                ErrorText = "ProjectName not exists";
        //            }
        //        }
        //        else
        //        {
        //            sError = true;
        //            ErrorText = "Not Authorized IP address";

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sError = true;
        //        ErrorText = ex.Message;
        //    }
        //    if (sError)
        //    {
        //       // db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:" + ErrorText);
        //        return Json(new
        //        {
        //            Status = "Failure",
        //            Message = "Error:" + ErrorText
        //        });
        //    }
        //  //  db.WebAPITransctionUpdate(transactionUid, "Success", ErrorText);
        //    return Json(new
        //    {
        //        Status = "Success",
        //        Message = JsonConvert.SerializeObject(dtResponse)

        //    });
        //}

        /// <summary>
        /// Updated by venkat on 19 Feb 2022
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Financial/GetDocumentStatusCodes")]
        public IHttpActionResult GetDocumentStatusCodes()
        {
            bool sError = false;
            string ErrorText = "";
            var httpRequest = HttpContext.Current.Request;
            DataTable dtDcoumentStatus = new DataTable();
            var transactionUid = Guid.NewGuid();
            try
            {
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"];
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]))
                {
                    //db.WebAPITransctionUpdate(transactionUid, "Failure", "ErPor:Mandatory fields are missing");
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    });
                }

                var identity = (ClaimsIdentity)User.Identity;

                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var projectName = httpRequest.Params["ProjectName"];
                    DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                    if (dtWorkPackages.Rows.Count > 0)
                    {
                        dtDcoumentStatus = db.getDocumentStatusCodes(new Guid(dtWorkPackages.Rows[0]["ProjectUid"].ToString()));
                        
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "ProjectName not exists";
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
              db.WebAPITransctionUpdate(transactionUid, "Success", ErrorText);
            return Json(new
            {
                Status = "Success",
                Message = JsonConvert.SerializeObject(dtDcoumentStatus)

            });
        }

        // added on 14/02/2022 for venkat
        [Authorize]
        [HttpPost]
        [Route("api/Financial/AddEditRABillsST")]
        public IHttpActionResult AddEditRABillsST()
        {
           // string password = Security.Decrypt("v4rHNCt5nMRCK7KtYbGJ5Q==");
            bool sError = false;
            string ErrorText = "";
            string rabillUid = "";
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + "&RAbillno=" + httpRequest.Params["RAbillno"] + "&Date=" + httpRequest.Params["Date"] + "&numberofJIRs=" + httpRequest.Params["NoOfJIR"] + "&JIR=" + httpRequest.Params["JIR"]+"&enteredamount="+httpRequest.Params["enteredamount"];
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["RAbillno"]) || String.IsNullOrEmpty(httpRequest.Params["Date"]) ||
                    String.IsNullOrEmpty(httpRequest.Params["NoOfJIR"]) || String.IsNullOrEmpty(httpRequest.Params["JIR"]) || string.IsNullOrEmpty(httpRequest.Params["enteredamount"]) ||
                     String.IsNullOrEmpty(httpRequest.Params["UserId"]))
                {
                    
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    }); ;
                }

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {

                    DataSet dsWorkPackages = new DataSet();
                    var projectName = httpRequest.Params["ProjectName"];
                    var RAbillno = httpRequest.Params["RAbillno"];
                    var enteredAmount = httpRequest.Params["enteredamount"];
                    var userId = httpRequest.Params["UserId"].ToString();
                    int cnt = 0;
                    DateTime sDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(httpRequest.Params["Date"]))
                    {
                        string cdate = db.ConvertDateFormat(httpRequest.Params["Date"]);

                        sDate = Convert.ToDateTime(cdate);
                    }
                    
                    var NoOfJIR = httpRequest.Params["NoOfJIR"];
                    var InspectionUID = httpRequest.Params["JIR"];//InspectionUID
                    string[] InspectionUIDList = InspectionUID.Split('$');
                    DataTable dtjoininspection;
                   
                    // added by zuber on 17/02/2022

                    if (InspectionUID.Contains("$"))
                    {
                        for (int i = 0; i < InspectionUIDList.Length; i++)
                        {
                            dtjoininspection = new DataTable();
                            dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUIDList[i]);
                            if (dtjoininspection.Rows.Count == 0)
                            {
                                
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:InspectionUid does not exists"
                                });
                            }
                        }
                    }
                    else
                    {
                        dtjoininspection = new DataTable();
                        dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUID);
                        if (dtjoininspection.Rows.Count == 0)
                        {
                            
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:InspectionUid is not exists"
                            });
                        }
                    }
                    if(!decimal.TryParse(enteredAmount,out decimal RABillAmount))
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:Invalid enteredamount"
                        });
                    }
                    DataSet dsUserDetails = db.getUserDetails_by_EmailID(httpRequest.Params["UserId"].ToString());
                    if (dsUserDetails.Tables[0].Rows.Count == 0)
                    {
                        sError = true;
                        ErrorText = "User is not available";
                    }
                   
                    //-----------------------------------------------------------

                    DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                    if (dtWorkPackages.Rows.Count > 0)
                    {
                     DataSet dtUsersProject=   db.GetUsers_under_ProjectUID(new Guid(dtWorkPackages.Rows[0]["ProjectUId"].ToString()));
                        if(dtUsersProject.Tables[0].Rows.Count ==0)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:Invalid Project users"
                            });
                        }
                        string userAssigned = string.Empty;
                        for(int i=0;i<dtUsersProject.Tables[0].Rows.Count;i++)
                        {
                            if(dtUsersProject.Tables[0].Rows[i]["EmailID"].ToString() == userId )
                            {
                                userAssigned = dtUsersProject.Tables[0].Rows[i]["UserUID"].ToString();
                                break;
                            }
                        }
                        if(userAssigned == string.Empty)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:UserId is not assigned to Project"
                            });
                        }
                      rabillUid = db.AddRABillNumber(RAbillno, new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), sDate, RABillAmount.ToString());

                        if (rabillUid == "Exists")
                        {
                            sError = true;
                            ErrorText = "RA Bill Number already exists.";
                        }
                        else if (rabillUid == "Error1")
                        {
                            sError = true;
                            ErrorText = "There is a problem with this feature. Please contact system admin.";
                        }
                        else
                        {
                            string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + "/Documents/RABills/" + rabillUid;
                         //   string FileDirectory = "~/Documents/Issues/";

                            if (!Directory.Exists(sDocumentPath))
                            {
                                Directory.CreateDirectory(sDocumentPath);
                            }
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                if (httpPostedFile != null)
                                {
                                    string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                    string Extn = Path.GetExtension(httpPostedFile.FileName);
                                    httpPostedFile.SaveAs((sDocumentPath + "/" + sFileName + Extn));
                                    string savedPath = sDocumentPath + "/" + sFileName + Extn;
                                    string DecryptPagePath = sDocumentPath + "/" + sFileName + "_DE" + Extn;
                                    db.EncryptFile(savedPath, DecryptPagePath);

                                    int Cnt = db.RABill_Document_InsertUpdate(Guid.NewGuid(), new Guid(rabillUid), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), savedPath, "Fill-enteredamount", new Guid(userAssigned));
                                  

                                }
                            }
                            int ErrorCount = 0;
                            int ItemCount = 0;
                            double totamount = 0;
                            DataSet ds = db.GetBOQDetails_by_projectuid(new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()));
                            if (ds.Tables[0].Rows.Count > 0)
                            {

                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    string sDate2 = "";
                                    DateTime CDate2 = DateTime.Now;

                                    sDate2 = DateTime.Now.ToString("dd/MM/yyyy");
                                    sDate2 = db.ConvertDateFormat(sDate2);
                                    CDate2 = Convert.ToDateTime(sDate);

                                    cnt = db.InsertRABillsItems(rabillUid, ds.Tables[0].Rows[i]["Item_Number"].ToString(), ds.Tables[0].Rows[i]["Description"].ToString(), CDate2.ToString(), "0", new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), new Guid(ds.Tables[0].Rows[i]["BOQDetailsUID"].ToString()));
                                    if (cnt <= 0)
                                    {
                                        ErrorCount += 1;
                                    }
                                    else
                                    {
                                        ItemCount += 1;
                                        totamount += ds.Tables[0].Rows[i]["INR-Amount"].ToString() == "" ? 0 : Convert.ToDouble(ds.Tables[0].Rows[i]["INR-Amount"].ToString());
                                    }
                                }
                            }

                            if (ErrorCount > 0)
                            {
                                sError = true;
                                ErrorText = "There is a problem linking BOQ details to this RABill. Please contact system admin";

                            }
                            else
                            {
                                Guid AssignJointInspectionUID = Guid.NewGuid();
                               
                                // added by zuber on 17/02/2022
                                if (InspectionUID.Contains("$"))
                                {
                                    for (int i = 0; i < InspectionUIDList.Length; i++)
                                    {
                                        AssignJointInspectionUID = Guid.NewGuid();
                                        string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUIDList[i]));
                                        if (boqDetailsUid == "")
                                        {
                                            sError = true;
                                            ErrorText += "BOQDetails Uid not available for inspectionuid : " + InspectionUIDList[i];
                                        }
                                        else
                                        {
                                            cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(boqDetailsUid), new Guid(InspectionUIDList[i]));
                                            if (cnt == 0)
                                            {
                                                sError = true;
                                                ErrorText += "Join Inspection to RABill is not inserted for inspectionuid :" + InspectionUIDList[i];
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUID));
                                    if (boqDetailsUid == "")
                                    {
                                        sError = true;
                                        ErrorText = "BOQDetails Uid not available for selcted inspectionuid";
                                    }
                                    else
                                    {
                                        cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(boqDetailsUid), new Guid(InspectionUID));
                                        if (cnt == 0)
                                        {
                                            sError = true;
                                            ErrorText = "Join Inspection to RABill is not inserted";
                                        }
                                    }
                                }
                                //--------------------------------------------------
                            }

                        }

                    }

                    else
                    {
                        sError = true;
                        ErrorText = "No Workpackage available for select project";
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
                    RABillUId = rabillUid,
                    Message = "Successfully Updated Inspection to RAbill"
                });
            }
        }
        // added on 06 August 2022 for venkat
        // Inspectionuid is optional
        [Authorize]
        [HttpPost]
        [Route("api/Financial/AddEditRabillsWithAmount")]
        public IHttpActionResult AddEditRABillsWithAmount()
        {
            bool sError = false;
            string ErrorText = "";
            string rabillUid = "";
            Guid transactionUid = Guid.NewGuid();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + "&RAbillno=" + httpRequest.Params["RAbillno"] + "&Date=" + httpRequest.Params["Date"] + "&numberofJIRs=" + httpRequest.Params["NoOfJIR"] + "&JIR=" + httpRequest.Params["JIR"] + "&enteredamount=" + httpRequest.Params["enteredamount"];
                db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

                if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["RAbillno"]) || String.IsNullOrEmpty(httpRequest.Params["Date"]) ||
                      string.IsNullOrEmpty(httpRequest.Params["enteredamount"]) ||
                     String.IsNullOrEmpty(httpRequest.Params["UserId"]))
                {

                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    });
                }

                if (httpRequest.Files.Count == 0)
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:File  missing"
                    });

                }

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {

                    DataSet dsWorkPackages = new DataSet();
                    var projectName = httpRequest.Params["ProjectName"];
                    var RAbillno = httpRequest.Params["RAbillno"];
                    var enteredAmount = httpRequest.Params["enteredamount"];
                    var userId = httpRequest.Params["UserId"].ToString();
                    int cnt = 0;
                    DateTime sDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(httpRequest.Params["Date"]))
                    {
                        string cdate = db.ConvertDateFormat(httpRequest.Params["Date"]);

                        sDate = Convert.ToDateTime(cdate);
                    }

                    var NoOfJIR = httpRequest.Params["NoOfJIR"];
                    var InspectionUID = httpRequest.Params["JIR"];//InspectionUID
                    string[] InspectionUIDList = new string[0];
                    if (InspectionUID != null)
                    {
                        InspectionUIDList = InspectionUID.Split('$');
                    }
                    DataTable dtjoininspection;

                    // added by zuber on 17/02/2022

                    if (InspectionUIDList.Length > 0)
                    {
                        for (int i = 0; i < InspectionUIDList.Length; i++)
                        {
                            dtjoininspection = new DataTable();
                            dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUIDList[i]);
                            if (dtjoininspection.Rows.Count == 0)
                            {

                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:InspectionUid does not exists"
                                });
                            }
                        }
                    }
                    else if (InspectionUID != null)
                    {
                        dtjoininspection = new DataTable();
                        dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUID);
                        if (dtjoininspection.Rows.Count == 0)
                        {

                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:InspectionUid is not exists"
                            });
                        }
                    }
                    if (!decimal.TryParse(enteredAmount, out decimal RABillAmount))
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Error:Invalid enteredamount"
                        });
                    }
                    DataSet dsUserDetails = db.getUserDetails_by_EmailID(httpRequest.Params["UserId"].ToString());
                    if (dsUserDetails.Tables[0].Rows.Count == 0)
                    {
                        sError = true;
                        ErrorText = "User is not available";
                    }

                    //-----------------------------------------------------------

                    DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                    if (dtWorkPackages.Rows.Count > 0)
                    {
                        DataSet dtUsersProject = db.GetUsers_under_ProjectUID(new Guid(dtWorkPackages.Rows[0]["ProjectUId"].ToString()));
                        if (dtUsersProject.Tables[0].Rows.Count == 0)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:Invalid Project users"
                            });
                        }
                        string userAssigned = string.Empty;
                        for (int i = 0; i < dtUsersProject.Tables[0].Rows.Count; i++)
                        {
                            if (dtUsersProject.Tables[0].Rows[i]["EmailID"].ToString() == userId)
                            {
                                userAssigned = dtUsersProject.Tables[0].Rows[i]["UserUID"].ToString();
                                break;
                            }
                        }
                        if (userAssigned == string.Empty)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:UserId is not assigned to Project"
                            });
                        }
                        rabillUid = db.AddRABillNumber(RAbillno, new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), sDate, RABillAmount.ToString());

                        if (rabillUid == "Exists")
                        {
                            sError = true;
                            ErrorText = "RA Bill Number already exists.";
                        }
                        else if (rabillUid == "Error1")
                        {
                            sError = true;
                            ErrorText = "There is a problem with this feature. Please contact system admin.";
                        }
                        else
                        {
                            string sDocumentPath = ConfigurationManager.AppSettings["DocumentsPath"] + "/Documents/RABills/" + rabillUid;
                            //   string FileDirectory = "~/Documents/Issues/";

                            if (!Directory.Exists(sDocumentPath))
                            {
                                Directory.CreateDirectory(sDocumentPath);
                            }
                            for (int i = 0; i < httpRequest.Files.Count; i++)
                            {
                                HttpPostedFile httpPostedFile = httpRequest.Files[i];

                                if (httpPostedFile != null)
                                {
                                    string sFileName = Path.GetFileNameWithoutExtension(httpPostedFile.FileName);
                                    string Extn = Path.GetExtension(httpPostedFile.FileName);
                                    httpPostedFile.SaveAs((sDocumentPath + "/" + sFileName + Extn));
                                    string savedPath = sDocumentPath + "/" + sFileName + Extn;
                                    string DecryptPagePath = sDocumentPath + "/" + sFileName + "_DE" + Extn;
                                    db.EncryptFile(savedPath, DecryptPagePath);

                                    int Cnt = db.RABill_Document_InsertUpdate(Guid.NewGuid(), new Guid(rabillUid), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), "/Documents/RABills/" + rabillUid+"/"+ sFileName + Extn, "Fill-enteredamount", new Guid(userAssigned));


                                }
                            }
                            int ErrorCount = 0;
                            int ItemCount = 0;
                            double totamount = 0;
                            DataSet ds = db.GetBOQDetails_by_projectuid(new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()));
                            if (ds.Tables[0].Rows.Count > 0)
                            {

                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    string sDate2 = "";
                                    DateTime CDate2 = DateTime.Now;

                                    sDate2 = DateTime.Now.ToString("dd/MM/yyyy");
                                    sDate2 = db.ConvertDateFormat(sDate2);
                                    CDate2 = Convert.ToDateTime(sDate);

                                    cnt = db.InsertRABillsItems(rabillUid, ds.Tables[0].Rows[i]["Item_Number"].ToString(), ds.Tables[0].Rows[i]["Description"].ToString(), CDate2.ToString(), "0", new Guid(dtWorkPackages.Rows[0]["ProjectUID"].ToString()), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), new Guid(ds.Tables[0].Rows[i]["BOQDetailsUID"].ToString()));
                                    if (cnt <= 0)
                                    {
                                        ErrorCount += 1;
                                    }
                                    else
                                    {
                                        ItemCount += 1;
                                        totamount += ds.Tables[0].Rows[i]["INR-Amount"].ToString() == "" ? 0 : Convert.ToDouble(ds.Tables[0].Rows[i]["INR-Amount"].ToString());
                                    }
                                }
                            }

                            if (ErrorCount > 0)
                            {
                                sError = true;
                                ErrorText = "There is a problem linking BOQ details to this RABill. Please contact system admin";

                            }
                            else
                            {
                                Guid AssignJointInspectionUID = Guid.NewGuid();

                                // added by zuber on 17/02/2022
                                if (InspectionUIDList.Length > 0)
                                {
                                    for (int i = 0; i < InspectionUIDList.Length; i++)
                                    {
                                        AssignJointInspectionUID = Guid.NewGuid();
                                        string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUIDList[i]));
                                        if (boqDetailsUid == "")
                                        {
                                            sError = true;
                                            ErrorText += "BOQDetails Uid not available for inspectionuid : " + InspectionUIDList[i];
                                        }
                                        else
                                        {
                                            cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(boqDetailsUid), new Guid(InspectionUIDList[i]));
                                            if (cnt == 0)
                                            {
                                                sError = true;
                                                ErrorText += "Join Inspection to RABill is not inserted for inspectionuid :" + InspectionUIDList[i];
                                            }
                                        }

                                    }
                                }
                                else if (InspectionUID != null)
                                {
                                    string boqDetailsUid = db.gbGetBoqDetailsUId_InspectionUId(new Guid(InspectionUID));
                                    if (boqDetailsUid == "")
                                    {
                                        sError = true;
                                        ErrorText = "BOQDetails Uid not available for selcted inspectionuid";
                                    }
                                    else
                                    {
                                        cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(boqDetailsUid), new Guid(InspectionUID));
                                        if (cnt == 0)
                                        {
                                            sError = true;
                                            ErrorText = "Join Inspection to RABill is not inserted";
                                        }
                                    }
                                }
                                //--------------------------------------------------
                            }

                        }

                    }

                    else
                    {
                        sError = true;
                        ErrorText = "No Workpackage available for select project";
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
                    RABillUId = rabillUid,
                    Message = "Successfully Updated Inspection to RAbill"
                });
            }
        }


        // for saji
        //[Authorize]
        //[HttpGet]
        //[Route("api/Financial/GetIssues")]
        //public IHttpActionResult GetIssues()
        //{
        //    List<IssueModel> finallist = new List<IssueModel>();
        //    var httpRequest = HttpContext.Current.Request;
        //    var UserName = "superadmin"; // httpRequest.Params["UserName"];
        //    var Password = "lNrPvRRDrXwiPxN2LLSu2Q=="; // Security.Encrypt("trngpm1"); ; // Security.Encrypt(httpRequest.Params["Password"]);
        //    DataSet ds = new DataSet();
        //    ds = db.CheckLogin(UserName, Password);


        //    //Insert into WebAPITransctions table

        //    var transactionUid = Guid.NewGuid();
        //    var BaseURL = HttpContext.Current.Request.Url.ToString();
        //    string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + ";WorkPackageName=" + httpRequest.Params["WorkPackageName"];

        //    db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

        //    if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["WorkPackageName"]))
        //    {
        //        //db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:Mandatory fields are missing");
        //        return Json(new
        //        {
        //            Status = "Failure",
        //            Message = "Error:Mandatory fields are missing"
        //        });
        //    }


        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
        //        string UserType = ds.Tables[0].Rows[0]["TypeOfUser"].ToString();
        //        if (UserType == "U" || UserType == "MD" || UserType == "VP")
        //        {
        //            ds = db.ProjectClass_Select_All();
        //        }
        //        else if (UserType == "PA")
        //        {
        //            ds = db.ProjectClass_Select_By_UserUID(new Guid(UserUID));
        //        }
        //        else
        //        {
        //            ds = db.ProjectClass_Select_By_UserUID(new Guid(UserUID));
        //        }



        //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //        {
        //            DataSet dsProject = new DataSet();
        //            if (UserType == "U" || UserType == "MD" || UserType == "VP")
        //            {
        //                dsProject = db.GetProjects_by_ClassUID(new Guid(ds.Tables[0].Rows[i]["ProjectClass_UID"].ToString()));
        //            }
        //            else
        //            {
        //                dsProject = db.GetUserProjects_by_ClassUID(new Guid(ds.Tables[0].Rows[i]["ProjectClass_UID"].ToString()), new Guid(UserUID));
        //            }


        //            for (int j = 0; j < dsProject.Tables[0].Rows.Count; j++)
        //            {
        //                DataSet dsworkPackage = db.GetWorkPackages_By_ProjectUID(new Guid(dsProject.Tables[0].Rows[j]["ProjectUID"].ToString()));
        //                for (int k = 0; k < dsworkPackage.Tables[0].Rows.Count; k++)
        //                {
        //                    DataSet workpackaeissues = db.getIssuesList_by_WorkPackageUID(new Guid(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString()));
        //                    for (int l = 0; l < workpackaeissues.Tables[0].Rows.Count; l++)
        //                    {
        //                        if (workpackaeissues.Tables[0].Rows[l]["TaskUID"].ToString() == "00000000-0000-0000-0000-000000000000")
        //                        {
        //                            IssueModel issue = new IssueModel()
        //                            {
        //                                IssueUId = new Guid(workpackaeissues.Tables[0].Rows[l]["Issue_Uid"].ToString()),
        //                                ProjectUID = new Guid(workpackaeissues.Tables[0].Rows[l]["ProjectUID"].ToString()),
        //                                WorkPackagesUID = new Guid(workpackaeissues.Tables[0].Rows[l]["WorkPackagesUID"].ToString()),
        //                                TaskUID = new Guid(workpackaeissues.Tables[0].Rows[l]["TaskUID"].ToString()),
        //                                IssueDescription = workpackaeissues.Tables[0].Rows[l]["Issue_Description"].ToString(),
        //                                Status = workpackaeissues.Tables[0].Rows[l]["Issue_Status"].ToString(),
        //                                IssueDate = Convert.ToDateTime(workpackaeissues.Tables[0].Rows[l]["Issue_Date"].ToString()),
        //                                ReportingUser = UserName,
        //                                issue_all_status = null,
        //                                issue_attachements = null
        //                            };

        //                            DataSet issue_all_status =  db.GetIssueStatus_by_Issue_Uid(issue.IssueUId);

        //                            if (issue_all_status.Tables[0].Rows.Count >0)
        //                            {
        //                                List<IssueStatusModel> issue_status_list = new List<IssueStatusModel>();

        //                                foreach (DataRow status in issue_all_status.Tables[0].Rows)
        //                                {
        //                                    IssueStatusModel issue_status_model = new IssueStatusModel()
        //                                    {
        //                                        IssueStatusId = status.ItemArray[0].ToString(),
        //                                        IssueUId = new Guid(status.ItemArray[1].ToString()),
        //                                        Status = status.ItemArray[2].ToString(),
        //                                        Remarks = status.ItemArray[3].ToString(),
        //                                        StatusDate = Convert.ToDateTime(status.ItemArray[5].ToString()),
        //                                        issue_status_attachments = null
        //                                    };

        //                                    DataSet issue_status_all_attached = db.GetUploadedDocuments(issue_status_model.IssueStatusId);

        //                                    if (issue_status_all_attached.Tables[0].Rows.Count > 0)
        //                                    {
        //                                        List<IssueStatusAttachedModel> issue_status_attached_list = new List<IssueStatusAttachedModel>();

        //                                        foreach (DataRow attached in issue_status_all_attached.Tables[0].Rows)
        //                                        {
        //                                            IssueStatusAttachedModel issue_status_attached_model = new IssueStatusAttachedModel()
        //                                            {
        //                                                IssueStatusAttachedId = Convert.ToInt32(attached.ItemArray[0].ToString()),
        //                                                AttachedFileName = attached.ItemArray[1].ToString(),
        //                                                AttachedFilePath = attached.ItemArray[2].ToString(),
        //                                                IssueStatusId = attached.ItemArray[3].ToString()
        //                                            };

        //                                            issue_status_attached_list.Add(issue_status_attached_model);
        //                                        }

        //                                        issue_status_model.issue_status_attachments = issue_status_attached_list;
        //                                    }

        //                                    issue_status_list.Add(issue_status_model);
        //                                }

        //                                issue.issue_all_status = issue_status_list;
        //                            }

        //                            DataSet issue_all_attached = db.GetUploadedIssueDocuments(issue.IssueUId.ToString());

        //                            if (issue_all_attached.Tables[0].Rows.Count > 0)
        //                            {
        //                                List<IssueAttachedModel> issue_attached_list = new List<IssueAttachedModel>();

        //                                foreach (DataRow attached in issue_all_attached.Tables[0].Rows)
        //                                {
        //                                    IssueAttachedModel issue_attached_model = new IssueAttachedModel()
        //                                    {
        //                                         IssueAttachedId = Convert.ToInt32(attached.ItemArray[0].ToString()),
        //                                         AttachedFileName = attached.ItemArray[1].ToString(),
        //                                         AttachedFilePath = attached.ItemArray[2].ToString(),
        //                                         IssueUId = new Guid(attached.ItemArray[3].ToString())
        //                                    };

        //                                    issue_attached_list.Add(issue_attached_model);
        //                                }

        //                                issue.issue_attachements = issue_attached_list;
        //                            }
        //                            finallist.Add(issue);
        //                        }

        //                    }

        //                    DataSet task = db.GetTasksForWorkPackages(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString());

        //                    for (int m = 0; m < task.Tables[0].Rows.Count; m++)
        //                    {
        //                        DataSet taskissues = db.getIssuesList_by_TaskUID(new Guid(task.Tables[0].Rows[m]["TaskUID"].ToString()));
        //                        for (int n = 0; n < taskissues.Tables[0].Rows.Count; n++)
        //                        {
        //                            IssueModel taskissue = new IssueModel()
        //                            {
        //                                IssueUId = new Guid(taskissues.Tables[0].Rows[n]["Issue_Uid"].ToString()),
        //                                ProjectUID = new Guid(taskissues.Tables[0].Rows[n]["ProjectUID"].ToString()),
        //                                WorkPackagesUID = new Guid(taskissues.Tables[0].Rows[n]["WorkPackagesUID"].ToString()),
        //                                TaskUID = new Guid(taskissues.Tables[0].Rows[n]["TaskUID"].ToString()),
        //                                IssueDescription = taskissues.Tables[0].Rows[n]["Issue_Description"].ToString(),
        //                                Status = taskissues.Tables[0].Rows[n]["Issue_Status"].ToString(),
        //                                IssueDate = Convert.ToDateTime(taskissues.Tables[0].Rows[n]["Issue_Date"].ToString()),
        //                            };

        //                            DataSet issue_all_status = db.GetIssueStatus_by_Issue_Uid(taskissue.IssueUId);

        //                            if (issue_all_status.Tables[0].Rows.Count > 0)
        //                            {
        //                                List<IssueStatusModel> issue_status_list = new List<IssueStatusModel>();

        //                                foreach (DataRow status in issue_all_status.Tables[0].Rows)
        //                                {
        //                                    IssueStatusModel issue_status_model = new IssueStatusModel()
        //                                    {
        //                                        IssueStatusId = status.ItemArray[0].ToString(),
        //                                        IssueUId = new Guid(status.ItemArray[1].ToString()),
        //                                        Status = status.ItemArray[2].ToString(),
        //                                        Remarks = status.ItemArray[3].ToString(),
        //                                        StatusDate = Convert.ToDateTime(status.ItemArray[5].ToString()),
        //                                        issue_status_attachments = null
        //                                    };

        //                                    DataSet issue_status_all_attached = db.GetUploadedDocuments(issue_status_model.IssueStatusId);

        //                                    if (issue_status_all_attached.Tables[0].Rows.Count > 0)
        //                                    {
        //                                        List<IssueStatusAttachedModel> issue_status_attached_list = new List<IssueStatusAttachedModel>();

        //                                        foreach (DataRow attached in issue_status_all_attached.Tables[0].Rows)
        //                                        {
        //                                            IssueStatusAttachedModel issue_status_attached_model = new IssueStatusAttachedModel()
        //                                            {
        //                                                IssueStatusAttachedId = Convert.ToInt32(attached.ItemArray[0].ToString()),
        //                                                AttachedFileName = attached.ItemArray[1].ToString(),
        //                                                AttachedFilePath = attached.ItemArray[2].ToString(),
        //                                                IssueStatusId = attached.ItemArray[3].ToString()
        //                                            };

        //                                            issue_status_attached_list.Add(issue_status_attached_model);
        //                                        }

        //                                        issue_status_model.issue_status_attachments = issue_status_attached_list;
        //                                    }

        //                                    issue_status_list.Add(issue_status_model);
        //                                }

        //                                taskissue.issue_all_status = issue_status_list;
        //                            }

        //                            DataSet issue_all_attached = db.GetUploadedIssueDocuments(taskissue.IssueUId.ToString());

        //                            if (issue_all_attached.Tables[0].Rows.Count > 0)
        //                            {
        //                                List<IssueAttachedModel> issue_attached_list = new List<IssueAttachedModel>();

        //                                foreach (DataRow attached in issue_all_attached.Tables[0].Rows)
        //                                {
        //                                    IssueAttachedModel issue_attached_model = new IssueAttachedModel()
        //                                    {
        //                                        IssueAttachedId = Convert.ToInt32(attached.ItemArray[0].ToString()),
        //                                        AttachedFileName = attached.ItemArray[1].ToString(),
        //                                        AttachedFilePath = attached.ItemArray[2].ToString(),
        //                                        IssueUId = new Guid(attached.ItemArray[3].ToString())
        //                                    };

        //                                    issue_attached_list.Add(issue_attached_model);
        //                                }

        //                                taskissue.issue_attachements = issue_attached_list;
        //                            }


        //                            finallist.Add(taskissue);

        //                            IssuesAdd(new Guid(task.Tables[0].Rows[m]["TaskUID"].ToString()), finallist);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        return Json(finallist.ToList());
        //    }
        //    else
        //    {
        //        var result = new
        //        {
        //            Status = "Failure",
        //            Message = "Invalid UserName or Password",
        //        };
        //        return Json(result);
        //    }
        //}

        [Authorize]
        [HttpPost]
        [Route("api/Financial/GetIssueList")]
        public IHttpActionResult GetIssueList()
        {
            List<Issue> finallist = new List<Issue>();
            var httpRequest = HttpContext.Current.Request;

            //Insert into WebAPITransctions table

            var transactionUid = Guid.NewGuid();
            var BaseURL = HttpContext.Current.Request.Url.ToString();
            string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + ";WorkPackageName=" + httpRequest.Params["WorkPackageName"];

            db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

            if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["WorkPackageName"]))
            {
                //db.WebAPITransctionUpdate(transactionUid, "Failure", "Error:Mandatory fields are missing");
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:Mandatory fields are missing"
                });
            }


            DataSet ds1 = db.GetProjectUIDfromName(httpRequest.Params["ProjectName"].ToString());

            if (ds1.Tables[0].Rows.Count == 0)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:Wrong Project Name"
                });
            }

            string workpackage_uid = db.GetWorkPackageID_ProjectId_WorkPackageName(new Guid(ds1.Tables[0].Rows[0].ItemArray[0].ToString()), httpRequest.Params["WorkPackageName"].ToString());

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
                //if (workpackaeissues.Tables[0].Rows[l]["TaskUID"].ToString() == "00000000-0000-0000-0000-000000000000")
                //{
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
               // }
            }

            return Json(finallist.ToList().OrderBy(a => a.IssueDate));
        }


        [Authorize]
        [HttpPost]
        [Route("api/Financial/GetIssueStatus")]
        public IHttpActionResult GetIssueStatus()
        {
            List<IssueModel> finallist = new List<IssueModel>();

            var httpRequest = HttpContext.Current.Request;

            //Insert into WebAPITransctions table

            var transactionUid = Guid.NewGuid();
            var BaseURL = HttpContext.Current.Request.Url.ToString();
            string postData = "IssueUID=" + httpRequest.Params["IssueUID"];

            db.WebAPITransctionInsert(transactionUid, BaseURL, postData, "");

            if (String.IsNullOrEmpty(httpRequest.Params["IssueUID"]))
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error:Mandatory fields are missing"
                });
            }

            DataSet workpackageissues = db.getIssue_by_IssueUID(new Guid(httpRequest.Params["IssueUID"].ToString()));

            for (int l = 0; l < workpackageissues.Tables[0].Rows.Count; l++)
            {
                //if (workpackageissues.Tables[0].Rows[l]["TaskUID"].ToString() == "00000000-0000-0000-0000-000000000000")
                //{
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

                           //for blob
                           byte[] blobbytes;
                           string filename = string.Empty;
                           blobbytes = db.DownloadIssueDocument(Convert.ToInt32(attached["doc_id"].ToString()), out filename);
                           BinaryWriter Writer = null;
                           Writer = new BinaryWriter(File.OpenWrite(fileName));

                        // Writer raw data                
                           Writer.Write(blobbytes);
                           Writer.Flush();
                           Writer.Close();
                        //
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

                                //for blob
                                byte[] blobbytes;
                            
                                string filename = "";

                                blobbytes = db.DownloadDocument(attached["uploaded_doc_id"].ToString(), out filename);
                                BinaryWriter Writer = null;
                                Writer = new BinaryWriter(File.OpenWrite(fileName));

                                // Writer raw data                
                                Writer.Write(blobbytes);
                                Writer.Flush();
                                Writer.Close();
                                //
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
               // }

            }
            return Json(finallist.ToList());

        }


    }
}