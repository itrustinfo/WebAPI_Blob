using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMTWebAPI.DAL
{
    public class UserModel
    {
        public Guid UserUID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class Submitals
    {
        DBGetData GetData = new DBGetData();
        public string FlowStep1_DisplayName = string.Empty;
        public string FlowStep1_Date = string.Empty;
        public string FlowStep2_DisplayName = string.Empty;
        public string FlowStep2_Date = null;
        public string FlowStep3_DisplayName = string.Empty;
        public string FlowStep3_Date = string.Empty;
        public string FlowStep4_DisplayName = string.Empty;
        public string FlowStep4_Date = string.Empty;
        public string FlowStep5_DisplayName = string.Empty;
        public string FlowStep5_Date = string.Empty;
        public string FlowStep6_DisplayName = string.Empty;
        public string FlowStep6_Date = string.Empty;
        public string FlowStep7_DisplayName = string.Empty;
        public string FlowStep7_Date = string.Empty;
        public string FlowStep8_DisplayName = string.Empty;
        public string FlowStep8_Date = string.Empty;
        public string FlowStep9_DisplayName = string.Empty;
        public string FlowStep9_Date = string.Empty;
        public string FlowStep10_DisplayName = string.Empty;
        public string FlowStep10_Date = string.Empty;
        public string FlowStep11_DisplayName = string.Empty;
        public string FlowStep11_Date = string.Empty;
        public string FlowStep12_DisplayName = string.Empty;
        public string FlowStep12_Date = string.Empty;
        public string FlowStep13_DisplayName = string.Empty;
        public string FlowStep13_Date = string.Empty;
        public string FlowStep14_DisplayName = string.Empty;
        public string FlowStep14_Date = string.Empty;
        public string FlowStep15_DisplayName = string.Empty;
        public string FlowStep15_Date = string.Empty;
        public string FlowStep16_DisplayName = string.Empty;
        public string FlowStep16_Date = string.Empty;
        public string FlowStep17_DisplayName = string.Empty;
        public string FlowStep17_Date = string.Empty;
        public string FlowStep18_DisplayName = string.Empty;
        public string FlowStep18_Date = string.Empty;
        public string FlowStep19_DisplayName = string.Empty;
        public string FlowStep19_Date = string.Empty;
        public string FlowStep20_DisplayName = string.Empty;
        public string FlowStep20_Date = string.Empty;
        public int StepsCount = 0;

        public bool IsValidationFailed = false;
        public string ErrorMessage = string.Empty;
        public List<UserModel> lstSubmissionUser = new List<UserModel>();
        public List<UserModel> lstQualityEngg = new List<UserModel>();
        public List<UserModel> lstReviewer_B = new List<UserModel>();
        public List<UserModel> lstReviewer = new List<UserModel>();
        public List<UserModel> lstApproval = new List<UserModel>();
        public List<UserModel> lstUser6 = new List<UserModel>();
        public List<UserModel> lstUser7 = new List<UserModel>();
        public List<UserModel> lstUser8 = new List<UserModel>();
        public List<UserModel> lstUser9 = new List<UserModel>();
        public List<UserModel> lstUser10 = new List<UserModel>();
        public List<UserModel> lstUser11 = new List<UserModel>();
        public List<UserModel> lstUser12 = new List<UserModel>();
        public List<UserModel> lstUser13 = new List<UserModel>();
        public List<UserModel> lstUser14 = new List<UserModel>();
        public List<UserModel> lstUser15 = new List<UserModel>();
        public List<UserModel> lstUser16 = new List<UserModel>();
        public List<UserModel> lstUser17 = new List<UserModel>();
        public List<UserModel> lstUser18 = new List<UserModel>();
        public List<UserModel> lstUser19 = new List<UserModel>();
        public List<UserModel> lstUser20 = new List<UserModel>();

        public void BindFlowMasterUsers(Guid WorkPackageUID, Guid flowUID, Guid submitalCategoryUID, string flowName, DataTable dtUpdatedUser)
        {
            IsValidationFailed = false;
            ErrorMessage = string.Empty;
            DataSet ds = GetData.GetUsers_under_WorkpackageUID(WorkPackageUID);
            DataTable dsMusers = null;
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 1, dtUpdatedUser);

            if (dsMusers.Rows.Count > 0)
            {
                lstSubmissionUser = GetUsers(dsMusers);
            }
            else
            {
                if (1 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 1";
                }
            }

            //
            //ddlQualityEngg.DataSource = GetData.getUsers("C"); /step 2
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 2, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstQualityEngg = GetUsers(dsMusers);
            }
            else
            {
                if (2 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 2";
                }
            }
            // ddlQualityEngg.Items.Insert(0, new ListItem("--Select--", ""));
            //
            //ddlReviewer.DataSource = GetData.getUsers("R"); /step 3
            //for step 3 STP Flows Works A and Works B   -step 3 all users of all discipline to be taken


            if (flowName == "STP" || flowName == "Works A")
            {
                //WorkPackageCategory_UID
                DataTable dsAllCategories = GetData.GetWorkpackageCategory(WorkPackageUID);
                foreach (DataRow dr in dsAllCategories.Rows)
                {
                    dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, new Guid(dr["WorkPackageCategory_UID"].ToString()), 3, dtUpdatedUser);
                    lstReviewer_B.AddRange(GetUsers(dsMusers));
                }
            }
            else
            {
                dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 3, dtUpdatedUser);
                if (dsMusers.Rows.Count > 0)
                {
                    lstReviewer_B.AddRange(GetUsers(dsMusers));
                }
                else
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 3";
                }
            }

            //  ddlReviewer.Items.Insert(0, new ListItem("--Select--", ""));

            //
            //ddlReviewer_B.DataSource = GetData.getUsers("R"); //step 4
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 4, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstReviewer = GetUsers(dsMusers);
            }
            else
            {
                if (4 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 4";
                }
            }
            //  ddlReviewer_B.Items.Insert(0, new ListItem("--Select--", ""));
            //
            //ddlApproval.DataSource = GetData.getUsers("A"); //step 5
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 5, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstApproval = GetUsers(dsMusers);
            }
            else
            {
                if (5 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 5";
                }
            }
            // ddlApproval.Items.Insert(0, new ListItem("--Select--", ""));

            // added on 03/03/2022 for new 15 entries
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 6, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser6 = GetUsers(dsMusers);
            }
            else
            {
                if (6 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 6";
                }
            }
            // dlUser6.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 7, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser7 = GetUsers(dsMusers);
            }
            else
            {
                if (7 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 7";
                }
            }
            //  dlUser7.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 8, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser8 = GetUsers(dsMusers);
            }
            else
            {
                if (8 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 8";
                }
            }
            // dlUser8.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 9, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser9 = GetUsers(dsMusers);
            }
            else
            {
                if (9 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 9";
                }
            }
            // dlUser9.Items.Insert(0, new ListItem("--Select--", ""));

            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 10, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser10 = GetUsers(dsMusers);
            }
            else
            {
                if (10 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 10";
                }
            }
            // }
            // dlUser10.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 11, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser11 = GetUsers(dsMusers);
            }
            else
            {
                if (11 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 11";
                }
            }
            // dlUser11.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 12, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser12 = GetUsers(dsMusers);
            }
            else
            {
                if (12 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 12";
                }
            }
            //  dlUser12.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 13, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser13 = GetUsers(dsMusers);
            }
            else
            {
                if (13 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 13";
                }
            }
            // dlUser13.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 14, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser14 = GetUsers(dsMusers);
            }
            else
            {
                if (14 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 14";
                }
            }
            // dlUser14.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 15, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser15 = GetUsers(dsMusers);
            }
            else
            {
                if (15 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 15";
                }
            }
            // dlUser15.Items.Insert(0, new ListItem("--Select--", ""));

            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 16, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser16 = GetUsers(dsMusers);
            }
            else
            {
                if (16 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 16";
                }
            }
            // dlUser16.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 17, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser17 = GetUsers(dsMusers);
            }
            else
            {
                if (17 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 17";
                }
            }
            // dlUser17.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 18, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser18 = GetUsers(dsMusers);
            }
            else
            {
                if (18 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 18";
                }
            }
            // dlUser18.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 19, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser19 = GetUsers(dsMusers);
            }
            else
            {
                if (19 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 19";
                }
            }
            // dlUser19.Items.Insert(0, new ListItem("--Select--", ""));
            dsMusers = GetData.FlowMasterUser_Select(flowUID, WorkPackageUID, submitalCategoryUID, 20, dtUpdatedUser);
            if (dsMusers.Rows.Count > 0)
            {
                lstUser20 = GetUsers(dsMusers);
            }
            else
            {
                if (20 <= StepsCount)
                {
                    IsValidationFailed = true;
                    ErrorMessage = "Flow master user does not setup for step 20";
                }
            }


        }


        public void SetDatesForFlow(DateTime inputDate, string flowUID)
        {
            DataSet ds = GetData.GetDocumentFlows_by_UID(new Guid(flowUID));
            if (ds == null && ds.Tables[0].Rows.Count == 0)
            {
                return;
            }
            string sDate1 = "";
            DateTime CDate1 = DateTime.Now;
            if (inputDate > CDate1)
            {
                CDate1 = inputDate;
            }


            sDate1 = DateTime.Now.ToString("dd/MM/yyyy");
            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
            sDate1 = GetData.ConvertDateFormat(sDate1);
            CDate1 = Convert.ToDateTime(sDate1);

            //FlowStep1_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString())).ToString("yyyy-MM-dd");
            //FlowStep1_Date = CDate1.ToString("dd/MM/yyyy");
            //FlowStep2_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString())).ToString("dd/MM/yyyy");

            //FlowStep3_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString())).ToString("dd/MM/yyyy");

            //string dt = Convert.ToDateTime((dtSubTargetDate.FindControl("txtDate") as TextBox).Text).ToString("dd/MM/yyyy");
            //string dt = CDate1;

            if (ds.Tables[0].Rows[0]["Steps_Count"].ToString() == "-1")
            {

            }
            else if (ds.Tables[0].Rows[0]["Steps_Count"].ToString() == "1")
            {
                if (ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString() != "-1")
                {
                    // FlowStep1_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep1_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }
            else if (ds.Tables[0].Rows[0]["Steps_Count"].ToString() == "2")
            {
                if (ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString() != "-1")
                {
                    //FlowStep1_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep1_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString() != "-1")
                {
                    // FlowStep2_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep2_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
            }
            else if (ds.Tables[0].Rows[0]["Steps_Count"].ToString() == "3")
            {
                if (ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString() != "-1")
                {
                    // FlowStep1_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep1_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString() != "-1")
                {
                    // FlowStep2_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep2_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString() != "-1")
                {
                    //FlowStep3_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep3_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                //Page.ClientScript.RegisterStartupScript(Page.GetType(), "CLOSE", "<script language='javascript'>alert('Success-8');</script>");
            }
            else if (ds.Tables[0].Rows[0]["Steps_Count"].ToString() == "4")
            {
                if (ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString() != "-1")
                {
                    // FlowStep1_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep1_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString() != "-1")
                {
                    // FlowStep2_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep2_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString() != "-1")
                {
                    // FlowStep3_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep3_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString() != "-1")
                {
                    // FlowStep4_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep4_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }
            else if (ds.Tables[0].Rows[0]["Steps_Count"].ToString() == "5")
            {
                if (ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString() != "-1")
                {
                    //FlowStep1_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep1_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString() != "-1")
                {
                    // FlowStep2_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep2_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString() != "-1")
                {
                    //FlowStep3_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep3_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString() != "-1")
                {
                    // FlowStep4_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep4_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                }
                if (ds.Tables[0].Rows[0]["dtAppTargetDate"].ToString() != "-1")
                {
                    // FlowStep5_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep5_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep5_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep5_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

            }
            else // if greater than 5
            {
                if (ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString() != "-1")
                {
                    // FlowStep1_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep1_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep1_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                if (ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString() != "-1")
                {
                    // FlowStep2_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep2_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep2_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString() != "-1")
                {
                    //FlowStep3_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep3_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep3_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString() != "-1")
                {
                    //FlowStep4_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep4_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep4_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                }
                if (ds.Tables[0].Rows[0]["FlowStep5_Duration"].ToString() != "-1")
                {
                    // FlowStep5_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep5_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep5_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep5_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                }
                //
                if (ds.Tables[0].Rows[0]["FlowStep6_Duration"] != DBNull.Value)
                {
                    // FlowStep6_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep6_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep6_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep6_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                }
                if (ds.Tables[0].Rows[0]["FlowStep7_Duration"] != DBNull.Value)
                {
                    // FlowStep7_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep7_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep7_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep7_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep8_Duration"] != DBNull.Value)
                {
                    //  FlowStep8_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep8_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep8_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep8_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep9_Duration"] != DBNull.Value)
                {
                    // FlowStep9_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep9_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep9_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep9_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep10_Duration"] != DBNull.Value)
                {
                    //  FlowStep10_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep10_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep10_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep10_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep11_Duration"] != DBNull.Value)
                {
                    //  FlowStep11_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep11_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep11_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep11_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep12_Duration"] != DBNull.Value)
                {
                    // FlowStep12_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep12_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep12_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep12_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep13_Duration"] != DBNull.Value)
                {
                    //FlowStep13_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep13_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep13_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep13_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep14_Duration"] != DBNull.Value)
                {
                    //FlowStep14_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep14_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep14_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep14_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep15_Duration"] != DBNull.Value)
                {
                    //  FlowStep15_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep15_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep15_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep15_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep16_Duration"] != DBNull.Value)
                {
                    // FlowStep16_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep16_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep16_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep16_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep17_Duration"] != DBNull.Value)
                {
                    // FlowStep17_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep17_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep17_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep17_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                }
                if (ds.Tables[0].Rows[0]["FlowStep18_Duration"] != DBNull.Value)
                {
                    // FlowStep18_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep18_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep18_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep18_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep19_Duration"] != DBNull.Value)
                {
                    // FlowStep19_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep19_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep19_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep19_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
                if (ds.Tables[0].Rows[0]["FlowStep20_Duration"] != DBNull.Value)
                {
                    //FlowStep20_Date = CDate1.AddDays(Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep20_Duration"].ToString())).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FlowStep20_Date = Convert.ToDateTime(GetData.CalculateHolidayOffset(CDate1, Convert.ToInt32(ds.Tables[0].Rows[0]["FlowStep20_Duration"].ToString()))).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                }
            }
        }

        private void GlobalVariableReset()
        {
            FlowStep1_DisplayName = string.Empty;
            FlowStep1_Date = string.Empty;
            FlowStep2_DisplayName = string.Empty;
            FlowStep2_Date = null;
            FlowStep3_DisplayName = string.Empty;
            FlowStep3_Date = string.Empty;
            FlowStep4_DisplayName = string.Empty;
            FlowStep4_Date = string.Empty;
            FlowStep5_DisplayName = string.Empty;
            FlowStep5_Date = string.Empty;
            FlowStep6_DisplayName = string.Empty;
            FlowStep6_Date = string.Empty;
            FlowStep7_DisplayName = string.Empty;
            FlowStep7_Date = string.Empty;
            FlowStep8_DisplayName = string.Empty;
            FlowStep8_Date = string.Empty;
            FlowStep9_DisplayName = string.Empty;
            FlowStep9_Date = string.Empty;
            FlowStep10_DisplayName = string.Empty;
            FlowStep10_Date = string.Empty;
            FlowStep11_DisplayName = string.Empty;
            FlowStep11_Date = string.Empty;
            FlowStep12_DisplayName = string.Empty;
            FlowStep12_Date = string.Empty;
            FlowStep13_DisplayName = string.Empty;
            FlowStep13_Date = string.Empty;
            FlowStep14_DisplayName = string.Empty;
            FlowStep14_Date = string.Empty;
            FlowStep15_DisplayName = string.Empty;
            FlowStep15_Date = string.Empty;
            FlowStep16_DisplayName = string.Empty;
            FlowStep16_Date = string.Empty;
            FlowStep17_DisplayName = string.Empty;
            FlowStep17_Date = string.Empty;
            FlowStep18_DisplayName = string.Empty;
            FlowStep18_Date = string.Empty;
            FlowStep19_DisplayName = string.Empty;
            FlowStep19_Date = string.Empty;
            FlowStep20_DisplayName = string.Empty;
            FlowStep20_Date = string.Empty;
            StepsCount = 0;

            IsValidationFailed = false;
            ErrorMessage = string.Empty;
            lstSubmissionUser = new List<UserModel>();
            lstQualityEngg = new List<UserModel>();
            lstReviewer_B = new List<UserModel>();
            lstReviewer = new List<UserModel>();
            lstApproval = new List<UserModel>();
            lstUser6 = new List<UserModel>();
            lstUser7 = new List<UserModel>();
            lstUser8 = new List<UserModel>();
            lstUser9 = new List<UserModel>();
            lstUser10 = new List<UserModel>();
            lstUser11 = new List<UserModel>();
            lstUser12 = new List<UserModel>();
            lstUser13 = new List<UserModel>();
            lstUser14 = new List<UserModel>();
            lstUser15 = new List<UserModel>();
            lstUser16 = new List<UserModel>();
            lstUser17 = new List<UserModel>();
            lstUser18 = new List<UserModel>();
            lstUser19 = new List<UserModel>();
            lstUser20 = new List<UserModel>();

        }

        private List<UserModel> GetUsers(DataTable dt)
        {
            List<UserModel> users = new List<UserModel>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string name = string.Empty;
                    string email = "";
                    if (dt.Columns.Contains("UserName"))
                        name = dr["UserName"].ToString();
                    if (dt.Columns.Contains("Name"))
                        name = dr["Name"].ToString();
                    if (dt.Columns.Contains("Email"))
                        email = dr["Email"].ToString();
                    users.Add(new UserModel() { UserUID = new Guid(dr["UserUID"].ToString()), Name = name, Email = email });
                }
            }
            return users;
        }


    }
}
