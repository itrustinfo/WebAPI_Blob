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
                if (String.IsNullOrEmpty(httpRequest.Params["ProjectName"]) || String.IsNullOrEmpty(httpRequest.Params["RAbillno"]) || String.IsNullOrEmpty(httpRequest.Params["Date"]) ||
                    String.IsNullOrEmpty(httpRequest.Params["numberofJIRs"]) || String.IsNullOrEmpty(httpRequest.Params["JIR"]))
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Error:Mandatory fields are missing"
                    }); ;
                }
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
                        DataTable dtjoininspection = db.getJointInspection_by_inspectionUid(InspectionUID);
                        if(dtjoininspection.Rows.Count==0)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error:InspectionUid is not exists"
                            }) ;
                        }
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
                     String.IsNullOrEmpty(httpRequest.Params["Currency"]) || String.IsNullOrEmpty(httpRequest.Params["Rabill number"]) || String.IsNullOrEmpty(httpRequest.Params["Deductiondetails"]))
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
                    ";Currency=" + httpRequest.Params["Currency"] + ";Rabill number=" + httpRequest.Params["Rabill number"] + ";Deductiondetails=" + httpRequest.Params["Deductiondetails"];
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
                        var raBillNumber = httpRequest.Params["Rabill number"];
                        //var numberofdeductions = httpRequest.Params["Number of deductions"];
                        var Deductiondetails = httpRequest.Params["Deductiondetails"];
                        decimal invoiceAmount = 0;
                        decimal invoiceforRaBill = 0;
                        for (int t = 0; t < raBillNumber.ToString().Split(',').Length; t++)
                        {
                            invoiceforRaBill= db.GetRAbillPresentTotalAmount_by_RABill_UID(new Guid(raBillNumber.ToString().Split(',')[t].ToString()));
                            if(invoiceforRaBill == -123)
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error:Rabilluid-" + raBillNumber.ToString().Split(',')[t].ToString() + " not exists"
                                }) ;
                            }
                            invoiceAmount += db.GetRAbillPresentTotalAmount_by_RABill_UID(new Guid(raBillNumber.ToString().Split(',')[t].ToString()));
                        }
                        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                        deductionListClass deductionObj = jsonSerializer.Deserialize<deductionListClass>(Deductiondetails);
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
                                        db.InvoiceDeduction_InsertorUpdate(Guid.NewGuid(), new Guid(dtWorkPackages.Rows[0]["WorkpackageUID"].ToString()), InvoiceMaster_UID, new Guid(deductionDs.Tables[0].Rows[0]["UID"].ToString()), float.Parse(deductionObj.deductionList[i].Value), float.Parse(percentage), (currency == "₹ (RUPEE)") ? "&#x20B9;" : (currency == "$ (USD)") ? "&#36;" : "&#165;", Currecncy_CultureInfo, deductionObj.deductionList[i].deductionMode);
                                    }
                                }
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
                                    string sDate2 = httpRequest.Params["PaymentDate"];
                                   //
                                    sDate2 = db.ConvertDateFormat(sDate2);
                                    DateTime CDate2 = Convert.ToDateTime(sDate2);
                                    db.InsertRABillPayments(PaymentUID, InvoiceUID, "Invoice Amount", Amount, TotalDeductions, NetAmnt, FinMonthUID, CDate2);
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
    }
}