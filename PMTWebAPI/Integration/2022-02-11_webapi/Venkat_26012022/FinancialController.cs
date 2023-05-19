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
using Newtonsoft.Json;
using PMTWebAPI.DAL;

namespace PMTWebAPI.Controllers
{
    public class FinancialController : ApiController
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
        [Route("api/Financial/AddEditRABills")]
        public IHttpActionResult AddEditRABills()
        {
            bool sError = false;
            string ErrorText = "";
            string rabillUid = "";
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] + "&RAbillno=" + httpRequest.Params["RAbillno"] + "&Date=" + httpRequest.Params["Date"] + "&numberofJIRs=" + httpRequest.Params["numberofJIRs"] + "&JIR=" + httpRequest.Params["JIR"];
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {

                    DataSet dsWorkPackages = new DataSet();
                    var projectName = httpRequest.Params["ProjectName"];
                    var RAbillno = httpRequest.Params["RAbillno"];
                    int cnt = 0;
                    if (DateTime.TryParse(httpRequest.Params["Date"], out DateTime sDate))
                    {
                        var NoOfJIR = httpRequest.Params["NoOfJIR"];
                        var InspectionUID = httpRequest.Params["JIR"];//InspectionUID
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
                                    cnt = db.InsertJointInspectiontoRAbill(AssignJointInspectionUID, new Guid(rabillUid), new Guid(ds.Tables[0].Rows[0]["BOQDetailsUid"].ToString()), new Guid(InspectionUID));
                                    if (cnt == 0)
                                    {
                                        sError = true;
                                        ErrorText = "Join Inspection to RABill is not inserted";
                                    }
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
                        ErrorText = "Date is not correct format";
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
           
            try
            {
                var httpRequest = HttpContext.Current.Request;
                //Insert into WebAPITransctions table
                var BaseURL = HttpContext.Current.Request.Url.ToString();
                string postData = "ProjectName=" + httpRequest.Params["ProjectName"] ;
                db.WebAPITransctionInsert(Guid.NewGuid(), BaseURL, postData, "");

                var identity = (ClaimsIdentity)User.Identity;
                if (db.CheckGetWebApiSettings(identity.Name, GetIp()) > 0)
                {
                    var projectName = httpRequest.Params["ProjectName"];
                    DataTable dtWorkPackages = db.GetWorkPackages_ProjectName(projectName);
                    if (dtWorkPackages.Rows.Count > 0)
                    {
                        var invoiceNumber = httpRequest.Params["Invoice number"];
                        var invoiceDate = httpRequest.Params["Invoice Date"];
                        var currency = httpRequest.Params["Currency"];
                        var numberOfRaBills = httpRequest.Params["Number-of-RaBills"];
                        var raBillNumber = httpRequest.Params["Rabill number"];
                        var numberofdeductions = httpRequest.Params["Number of deductions"];
                        var deductionuid = httpRequest.Params["Deductionuid"];
                        var deductionMode = httpRequest.Params["Deduction mode "];
                        var value = httpRequest.Params["Value"];
                        string sDate1 = "";
                        DateTime CDate1 = DateTime.Now;

                        if (DateTime.TryParse(invoiceDate, out DateTime sDate))
                        {
                            sDate1 = sDate.ToString("dd/MM/yyyy");
                            sDate1 = db.ConvertDateFormat(sDate1);
                            CDate1 = Convert.ToDateTime(sDate1);

                            Guid InvoiceMaster_UID= Guid.NewGuid();
                            
                            string Currecncy_CultureInfo = "";
                            if (currency == "₹ (RUPEE)")
                            {
                                Currecncy_CultureInfo = "en-IN";
                            }
                            else if (currency == "$ (USD)")
                            {
                                Currecncy_CultureInfo = "en-US";
                            }
                            else
                            {
                                Currecncy_CultureInfo = "ja-JP";
                            }
                            int cnt = db.InvoiceMaster_InsertorUpdate(InvoiceMaster_UID, new Guid(dtWorkPackages.Rows[0]["PrjoectUID"].ToString()), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), invoiceNumber, "", CDate1, (currency == "₹ (RUPEE)") ? "&#x20B9;" : (currency == "$ (USD)") ? "&#36;" : "&#165;", Currecncy_CultureInfo);
                            if (cnt > 0)
                            {

                            }
                        }
                        else
                        {

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
                NoOflistitems = "",
                DeductionDescription = "",

            });
        }
    }
}