using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Security.Cryptography;
using System.Web.Configuration;
using System.Reflection;

namespace PMTWebAPI.DAL
{
    public class DBGetData
    {
        DBUtility db = new DBUtility();
        public string getProjectNameby_ProjectUID(Guid ProjectUID)
        {
            string sUser = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_getProjectNameby_ProjectUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                sUser = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sUser = "Error : " + ex.Message;
            }
            return sUser;
        }

        public string GetProjectRef_Number(Guid ProjectUID)
        {
            string PRefNumber = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetProjectRef_Number", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                PRefNumber = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                PRefNumber = "Error : " + ex.Message;
            }
            return PRefNumber;
        }

        public string GetDocumentSubmittleRef_Number(Guid ProjectUID)
        {
            string PRefNumber = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetDocumentSubmittleRef_Number", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                PRefNumber = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                PRefNumber = "Error : " + ex.Message;
            }
            return PRefNumber;
        }

        public int InsertorUpdateProjects(Guid ProjectUID, Guid ProjectClass_UID, string ProjectName, string ProjectAbbrevation,string Funding_Agency, string OwnerName, DateTime StartDate, 
            DateTime PlannedEndDate, DateTime ProjectedEndDate, string Status, double Budget, double ActualExpenditure,string Currency,string Currency_CultureInfo)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Projects_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@ProjectClass_UID", ProjectClass_UID);
                        cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
                        cmd.Parameters.AddWithValue("@ProjectAbbrevation", ProjectAbbrevation);
                        cmd.Parameters.AddWithValue("@Funding_Agency", Funding_Agency);
                        cmd.Parameters.AddWithValue("@OwnerName", OwnerName);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@PlannedEndDate", PlannedEndDate);
                        cmd.Parameters.AddWithValue("@ProjectedEndDate", ProjectedEndDate);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Budget", Budget);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }
        public DataSet GetAllProjects()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAllProjects", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetProject_by_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getProjectDetails_by_ProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int Project_Delete(Guid ProjectUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Project_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public int InsertorUpdateWorkPackage(Guid WorkPackageUID, Guid ProjectUID, Guid Workpackage_OptionUID, Guid Contractor_UID,string Name, string WorkPackage_Location, string WorkPackage_Client, DateTime StartDate, 
            DateTime PlannedEndDate, DateTime ProjectedEndDate, string Status, double Budget, double ActualExpenditure,string Currency,string Currency_CultureInfo)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateWorkPackage"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                        cmd.Parameters.AddWithValue("@Contractor_UID", Contractor_UID);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@WorkPackage_Location", WorkPackage_Location);
                        cmd.Parameters.AddWithValue("@WorkPackage_Client", WorkPackage_Client);
                        //cmd.Parameters.AddWithValue("@RFPReference", RFPReference);
                        //cmd.Parameters.AddWithValue("@POReference", POReference);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@PlannedEndDate", PlannedEndDate);
                        cmd.Parameters.AddWithValue("@ProjectedEndDate", ProjectedEndDate);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Budget", Budget);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        con.Open();
                        cnt=cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public int Workpackge_Order_Update(Guid WorkPackageUID, string Action)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Workpackge_Order_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@Action", Action);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public DataSet GetWorkPackages_By_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Usp_getWorkPackge_by_ProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        


        public string getProjectUIDby_WorkpackgeUID(Guid WorkPackageUID)
        {
            string ProjectUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("getProjectUIDby_WorkpackgeUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                ProjectUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                ProjectUID = "Error : " + ex.Message;
            }
            return ProjectUID;
        }

        public DataSet GetWorkPackages_By_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Usp_getWorkPackge_by_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetWorkPackages(String sUseruid)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackagesForUser", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUseruid);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetAllWorkPackages()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetAllWorkpackages", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;                
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetWorkPackages_by_UserUID(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkpackage_by_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetWorkPackages_by_UserUID_Without_Selected(Guid UserUID,Guid WorkpackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkpackage_by_UserUID_Without_Selcted", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetWorkPackages_By_ProjectUID_witout_Selected(Guid ProjectUID, Guid WorkpackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackages_By_ProjectUID_witout_Selected", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string getWorkPackageNameby_WorkPackageUID(Guid WorkPackageUID)
        {
            string sUser = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_getWorkPackageNameby_WorkPackageUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                sUser = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sUser = "Error : " + ex.Message;
            }
            return sUser;
        }

        public DataSet GetWorkPackages1(String sUseruid)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackagesForUser_1", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUseruid);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public DataSet GetWorkPackages_ForUser_by_ProjectUID(Guid UserUID, Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                //SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackages_ForUser_by_ProjectUID", con);
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackages_by_UserID_ProjectID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUser_WorkPackage_By_ProjectUID(Guid UserUID, Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                //SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackages_ForUser_by_ProjectUID", con);
                SqlDataAdapter cmd = new SqlDataAdapter("GetUser_WorkPackage_By_ProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetWorkPackages(String sUseruid,string WorkPackageID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackagesForUser_by_WorkPackageID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUseruid);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable GetTaskDetails_TaskUID(string TaskUID)
        {
            DataTable  ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getTaskDetails_By_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetWorkPackages1(String sUseruid, string WorkPackageID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackagesForUser_by_WorkPackageID_1", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUseruid);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetWorkPackages_by_UserID_ProjectID(String sUseruid, string ProjectID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkPackages_by_UserID_ProjectID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUseruid);
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int Workpackage_Delete(Guid WorkPackageUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Workpackage_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int Insert_UserWorkPackages(Guid ProjectUID, Guid UserUID, Guid WorkPackageUID, string Status)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdate_UserWorkPackages"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public DataSet GetAssignedUsers_for_WorkPackage(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getAssignedUsers_for_WorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        internal int InserUserAppTokenDetails(string userUid, string deviceid)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_Insert_AppUserToken"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@userUid", userUid);
                        cmd.Parameters.AddWithValue("@deviceid", deviceid);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public DataSet GetTasks_by_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetTasks_by_ProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTasksForWorkPackages(String sWorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTasksForWorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", sWorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

       
        public DataSet GetTasks_by_WorkpackageOptionUID(Guid WorkPackageUID, Guid Workpackage_Option)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetTasks_by_WorkpackageOptionUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Workpackage_Option", Workpackage_Option);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTasks_For_WorkPackages_UserUID(Guid sWorkPackageUID,Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", sWorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTasks_by_WorkPackageUID(String sWorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTasks_By_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", sWorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int Task_Order_Update(Guid TaskUID, string Action)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Task_Order_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Action", Action);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public int Task_Order_Update_by_TaskUID(Guid TaskUID,int Order)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Task_Order_Update_by_TaskUID"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Order", Order);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public DataSet GetTasksForWorkPackages_and_Discipline(String sWorkPackageUID, string Discipline)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTasksForWorkPackage_and_Discipline", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", sWorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Discipline", Discipline);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUser_TasksForWorkPackages(Guid UserUID, String sWorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetUser_TasksForWorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", sWorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUser_TasksForWorkPackages_Orderby_Level(Guid UserUID, String sWorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetUser_TasksForWorkPackage_Level1", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", sWorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTask_by_ParentTaskUID(Guid ParentTaskID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetTask_by_ParentTaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ParentTaskID", ParentTaskID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetUserTask_by_ParentTaskUID(Guid ParentTaskID,Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUserTask_by_ParentTaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ParentTaskID", ParentTaskID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int GetTaskCount_by_ParentTaskID(Guid ParentTaskID)
        {
            int cCount = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetTaskCount_by_ParentTaskID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ParentTaskID", ParentTaskID);
                cCount = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                cCount = 0;
            }
            return cCount;
        }

        public int Check_Task_is_Delayed(Guid TaskUID)
        {
            int cCount = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("Check_Task_is_Delayed", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                cCount = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                cCount = 0;
            }
            return cCount;
        }

        public string getTaskParents(Guid TaskUID)
        {
            string ParentTasks = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_getTaskParentNames", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                ParentTasks = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                ParentTasks = "Error : " + ex.Message;
            }
            return ParentTasks;
        }

        public DataSet GetTasksForWorkPackages_Without(String sWorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTasksForWorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", sWorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDelayedTasks_by_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDelayedTasks_by_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTasksby_TaskUiD(Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTasksby_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int TaskExists_for_User_byUserUID(Guid TaskUID,Guid UserUID)
        {
            int cnt = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("TaskExists_for_User_byUserUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Parameters.AddWithValue("@UserUID", UserUID);
                cnt = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                cnt = 0;
            }
            return cnt;
        }

        public DataSet GetTask_By_TaskID_UserUID(Guid TaskUID,Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetTask_By_TaskID_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTasksForWorkPackages_by_UserUID(String UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTasksForWorkPackages_by_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTasksForWorkPackages_by_UserUID(String UserUID,string WorkPackageID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTasksForWorkPackages_by_UserUID_WorkPackageID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetAllTasksForWorkPackages_by_UserUID(String UserUID, string WorkPackageID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetAllTasksForWorkPackages_by_UserUID_WorkPackageID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public DataSet GetAllTasks_by_WorkPackagesUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetAllTasks_by_WorkPackagesUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTask_Status_Count(string UserUID, string ActivityType, string ActivityID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTask_Status_Count", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityType", ActivityType);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityID", ActivityID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTask_Status_Percentage(string UserUID, string ActivityType, string ActivityID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTask_Status_Percentage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityType", ActivityType);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityID", ActivityID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTask_by_Status(Guid ProjectUID, Guid WorkPackageUID, string Status)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getTask_by_Status", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Status", Status);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTask_by_Name(Guid ProjectUID, Guid WorkPackageUID, string Name)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getTask_by_TaskName", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Name", Name);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Get_WorkPackage_Budget(string UserUID, string ActivityType, string ActivityID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_Get_WorkPackage_Budget", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityType", ActivityType);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityID", ActivityID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public DataSet Get_WorkPackage_TimePercentage(string UserUID, string ActivityType, string ActivityID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_Get_WorkPackage_Time_Percentage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityType", ActivityType);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityID", ActivityID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Get_WorkPackage_WorkLoad(string UserUID, string ActivityType, string ActivityID, string ResourceUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Get_WorkPackage_WorkLoad", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityType", ActivityType);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityID", ActivityID);
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Get_WorkPackage_WorkLoad_ForUser(string UserUID, string ActivityType, string ActivityID,string ResourceUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Get_WorkPackage_WorkLoad_ForUser", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityType", ActivityType);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityID", ActivityID);
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetSubTasksForWorkPackages(String sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetSubTasksForWorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ParentTaskID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetSubTasksForWorkPackages_NotIn_Dependencies(String sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetSubTasksForWorkPackage_NotIn_Dependencies", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ParentTaskID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Get_DelayedTasksForWorkPackages(String sTaskUID, int TaskLevel)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDelayed_TasksForWorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ParentTaskID", sTaskUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TaskLevel", TaskLevel);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetSubtoSubTasksForWorkPackages(String sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetSubtoSubTasksForWorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ParentTaskID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetSubtoSubtoSubTasksForWorkPackages(String sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetSubtoSubtoSubTasksForWorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ParentTaskID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetSubtoSubtoSubtoSubTasksForWorkPackages(String sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetSubtoSubtoSubtoSubTasksForWorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ParentTaskID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet CheckLogin(String sUsername, String sPassword)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_CheckLogin", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Username", sUsername);
                cmd.SelectCommand.Parameters.AddWithValue("@Password", sPassword);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetProjectDetailsForUser(String sUserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetProjectDetailsForUser", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTaskDetails(String sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskDetails", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetFirstLevelTask()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetFirstlevelTasks", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int getTaskLevel_By_TaskUID(Guid TaskUID)
        {
            int tLevel = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_getTaskLevel_By_TaskUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                tLevel = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tLevel;
        }

        //added on 28/03/2019
        public DataSet getUsers(string Type)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetUsers", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TypeOfUser", Type);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 28/03/2019
        public DataSet getAllUsers()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetAllUsers", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getUsers_by_AdminUnder(Guid Admin_Under)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getUsers_by_AdminUnder", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Admin_Under", Admin_Under);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

      
        public DataSet getUsers_by_ProjectUnder(Guid Project_Under)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getUsers_by_Project_Under", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Project_Under", Project_Under);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getAllUsers_Roles()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAll_UserRoles", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string GetUserRoleByID(Guid UserRole_ID)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("UserRoleDesc_Selectby_UID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserRole_ID", UserRole_ID);                
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }
        // added on 03/05/2019

        public Boolean InsertorUpdateUsers(Guid UserUID, string FirstName, string LastName, string EmailID, string Mobilenumber, string Address1, string Address2, string Username, string password, string TypeOfUser, Guid Admin_Under, string Profile_Pic)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    //string query = "INSERT INTO UserDetails " +
                    // "(UserUID,FirstName,LastName,EmailID,Mobilenumber,Address1,Address2,Username,password,TypeOfUser,CreatedDate,Admin_Under,Project_Under,Profile_Pic) " +
                    // "VALUES (@Id,  @Name, @IsActive, @IsActive)";
                    using (SqlCommand cmd = new SqlCommand("ups_InserorUpdateUser"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@EmailID", EmailID);
                        cmd.Parameters.AddWithValue("@Mobilenumber", Mobilenumber);
                        cmd.Parameters.AddWithValue("@Address1", Address1);
                        cmd.Parameters.AddWithValue("@Address2", Address2);
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@password", Security.Encrypt(password));
                        cmd.Parameters.AddWithValue("@TypeOfUser", TypeOfUser);
                        cmd.Parameters.AddWithValue("@Admin_Under", Admin_Under);
                        //cmd.Parameters.AddWithValue("@Project_Under", Project_Under);
                        cmd.Parameters.AddWithValue("@Profile_Pic", Profile_Pic);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //added on 27/03/2019
        public Boolean InsertorUpdateDocuments(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName, 
            string Doc_Category, string Doc_ProjectRefNo, string Doc_RefNumber, string Doc_Type,double Doc_Budget,DateTime IncomingRec_Date,
            Guid FlowUID, DateTime Flow_StartDate, string Doc_Media_HC, string Doc_Media_SC, string Doc_Media_SCEF, string Doc_Media_HCR,string Doc_Media_SCR,string Doc_Media_NA,
            string Doc_FileRefNumber, string Doc_Remarks)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    
                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateDocuments"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_ProjectRefNo", Doc_ProjectRefNo);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@Doc_Media_HC", Doc_Media_HC);
                        cmd.Parameters.AddWithValue("@Doc_Media_SC", Doc_Media_SC);
                        cmd.Parameters.AddWithValue("@Doc_Media_SCEF", Doc_Media_SCEF);
                        cmd.Parameters.AddWithValue("@Doc_Media_HCR", Doc_Media_HCR);
                        cmd.Parameters.AddWithValue("@Doc_Media_SCR", Doc_Media_SCR);
                        cmd.Parameters.AddWithValue("@Doc_Media_NA", Doc_Media_NA);
                        cmd.Parameters.AddWithValue("@Doc_FileRefNumber", Doc_FileRefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Remarks", Doc_Remarks);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }


        public int DoumentMaster_Insert_or_Update_Flow_0(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
            Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
            Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DoumentMaster_Insert_or_Update_Flow_0"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserUID", FlowStep1_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@IsSync", "N");
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int DoumentMaster_Insert_or_Update_Flow_1(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
            Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
            Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, Guid FlowStep2_UserUID, DateTime FlowStep2_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DoumentMaster_Insert_or_Update_Flow_1"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserUID", FlowStep1_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        //if (DateTime.TryParse(FlowStep1_TargetDate, out DateTime f1TragetDate))
                        //{
                        //    cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", f1TragetDate);
                        //}
                        //else
                        //{
                        //    cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", DBNull.Value);
                        //}

                        cmd.Parameters.AddWithValue("@FlowStep2_UserUID", FlowStep2_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@IsSync", "N");
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int DoumentMaster_Insert_or_Update_Flow_2(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
            Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
            Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, Guid FlowStep2_UserUID, DateTime FlowStep2_TargetDate, Guid FlowStep3_UserUID, DateTime FlowStep3_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DoumentMaster_Insert_or_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserUID", FlowStep1_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep2_UserUID", FlowStep2_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep3_UserUID", FlowStep3_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@IsSync", "N");
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

       

        public int DoumentMaster_Insert_or_Update_Flow_3(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
            Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
            Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, Guid FlowStep2_UserUID, DateTime FlowStep2_TargetDate,
            Guid FlowStep3_UserUID, DateTime FlowStep3_TargetDate, Guid FlowStep4_UserUID, DateTime FlowStep4_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DoumentMaster_Insert_or_Update_Flow_3"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserUID", FlowStep1_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep2_UserUID", FlowStep2_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep3_UserUID", FlowStep3_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep4_UserUID", FlowStep4_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep4_TargetDate", FlowStep4_TargetDate);
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@IsSync", "N");
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public int DoumentMaster_Insert_or_Update_Flow_4(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
            Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
            Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, Guid FlowStep2_UserUID, DateTime FlowStep2_TargetDate,
            Guid FlowStep3_UserUID, DateTime FlowStep3_TargetDate, Guid FlowStep4_UserUID, DateTime FlowStep4_TargetDate, Guid FlowStep5_UserUID, DateTime FlowStep5_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DoumentMaster_Insert_or_Update_Flow_4"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserUID", FlowStep1_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep2_UserUID", FlowStep2_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep3_UserUID", FlowStep3_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep4_UserUID", FlowStep4_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep4_TargetDate", FlowStep4_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep5_UserUID", FlowStep5_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep5_TargetDate", FlowStep5_TargetDate);
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@IsSync", "N");
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }
        //added on 28/03/2019
        public DataSet getDocumentsForTasks(Guid sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDocumentsForTask", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getUser_DocumentsForTasks(Guid sTaskUID,Guid UserUID,string TypeofUser)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetUser_DocumentsForTask", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDocuments_For_WorkPackage(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocuments_For_WorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUser_Documents_For_WorkPackage(Guid WorkPackageUID,Guid UserUID,string TypeofUser)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string GetTaskUID_By_WorkPackageID_TName(Guid WorkPackageUID, string Name)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetTaskUID_By_WorkPackageID_TName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Parameters.AddWithValue("@Name", Name);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public string GetTaskUID_By_ParentID_TName(Guid WorkPackageUID,Guid ParentTaskID, string Name)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetTaskUID_By_ParentID_TName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Parameters.AddWithValue("@ParentTaskID", ParentTaskID);
                cmd.Parameters.AddWithValue("@Name", Name);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        internal DataTable getDocumentStatusCodes(Guid guid)
        {
            DataTable dtdocumentStatus = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("sp_getDocument_Status_codes", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(dtdocumentStatus);
            }
            catch (Exception ex)
            {
                dtdocumentStatus = null;
            }
            return dtdocumentStatus;
        }

        public DataSet GetWorkpackageDocuments_For_Category(Guid WorkPackageUID, string Doc_Category)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocuments_For_Category", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUser_WorkpackageDocuments_For_Category(Guid WorkPackageUID, string Doc_Category,Guid UserUID,string TypeofUser)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUser_Documents_For_Category", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetTaskDocuments_For_Category(Guid TaskUID, string Doc_Category)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetTaskDocuments_For_Category", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetSubmittals_For_Category(string Doc_Category)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetSubmittals_For_Category", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUser_TaskDocuments_For_Category(Guid TaskUID, string Doc_Category, Guid UserUID, string TypeofUser)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUser_TaskDocuments_For_Category", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDocuments_For_Project(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocuments_For_Project", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUser_Documents_For_Project(Guid ProjectUID,Guid UserUID,string TypeofUser)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUser_Documents_For_Project", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDocumentsBy_Date(Guid ProjectUID, Guid WorkPackageUID, DateTime StartDate, DateTime EndDate)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentsBy_Date", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@StartDate", StartDate);
                cmd.SelectCommand.Parameters.AddWithValue("@EndDate", EndDate);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataTable getDocumentsForProject(Guid ProjectUID)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDocumentsForProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getDocumentsbyDocID(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDocumentsbyDocID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string getDocumentName_by_DocumentUID(Guid DocumentUID)
        {
            string sUser = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_getDocumentName_by_DocumentUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                sUser = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sUser = "Error : " + ex.Message;
            }
            return sUser;
        }

        public string GetSubmittal_Submitter_By_DocumentUID(Guid DocumentUID)
        {
            string sUser = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetSubmittal_Submitter_By_DocumentUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                sUser = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sUser = "Error : " + ex.Message;
            }
            return sUser;
        }

        //added on 02/04/2019
        public DataSet getDocuments()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentList", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable getDocuments_For_User(Guid UserUID, string TypeofUser)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentList_for_User", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable getDocuments_For_ProjectUID_and_WorkPackage(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentList_For_ProjectUID_and_WorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable getDocuments_For_ProjectUID_and_WorkPackage_and_Discipline(Guid ProjectUID, Guid WorkPackageUID, string Discipline)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentList_For_ProjectUID_and_WorkPackage_and_Discipline", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Discipline", Discipline);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable getDocuments_For_ProjectUID_and_WorkPackage_and_TaskUID(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentList_For_ProjectUID_and_WorkPackage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                //cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public DataTable getDocuments_For_ProjectUID_and_WorkPackage_and_TaskUID(Guid ProjectUID, Guid WorkPackageUID, Guid TaskUID)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentList_For_ProjectUID_and_WorkPackage_and_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getDocumentList_Delayed(Guid DocumentUID, string ActivityType, Guid AcivityUserUID, string TypeofUser)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentList_Delayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActivityType", ActivityType);
                cmd.SelectCommand.Parameters.AddWithValue("@AcivityUserUID", AcivityUserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable getDocuments_by_FreeText(string FreeText)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocuments_by_FreeText", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocText", FreeText);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable GetUserDocuments_by_FreeText(Guid UserUID,string TypeofUser,string FreeText)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUserDocuments_by_FreeText", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.SelectCommand.Parameters.AddWithValue("@DocText", FreeText);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getDocumentStatusList(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentStatusList", con);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet getActualDocumentStatusList(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("getActualDocumentStatusList", con);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDocumentProcess_in_Days(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentProcess_in_Days", con);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet getDocumentStatusList_by_StatusUID(Guid StatusUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentStatusList_by_StatusUID", con);
                cmd.SelectCommand.Parameters.AddWithValue("@StatusUID", StatusUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getTop1_DocumentStatusSelect(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getTop1_DocumentStatusSelect", con);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string GetCurrentDocumentStatus_by_DocumentUID(Guid DocumentUID)
        {
            string StatusUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetCurrentDocumentStatus_by_DocumentUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                StatusUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                StatusUID = "Error : " + ex.Message;
            }
            return StatusUID;
        }


        public string GetSubmittal_Reviewer_By_DocumentUID(Guid ActualDocumentUID)
        {
            string StatusUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetSubmittal_Reviewer_By_DocumentUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                StatusUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                StatusUID = "Error : " + ex.Message;
            }
            return StatusUID;
        }

        public string GetSubmittal_Approver_By_DocumentUID(Guid ActualDocumentUID)
        {
            string StatusUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetSubmittal_Approver_By_DocumentUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                StatusUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                StatusUID = "Error : " + ex.Message;
            }
            return StatusUID;
        }

        public DataSet getDocumentStatusCount()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_GetDocumentsCount", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getDocumentCount_by_ProjectUID_WorkPackageUID(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_getDocumentCount_by_ProjectUID_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getLatest_DocumentVerisonSelect(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetLatestDocumentVersion_DocumentUID", con);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 28/03/2019
        public DataSet getUserDetails(Guid sUserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetUserDetails", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet get_UserDetails(Guid sUserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_Get_UserDetails", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 09/04/2019
        public DataSet getStatusMaster()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetStatusMaster", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 27/03/2019

        public Boolean InsertorUpdateMainTask(Guid TaskUID, Guid WorkPackageUID, Guid ProjectUID, string Owner, string Name, string Description, string RFPReference, string POReference, DateTime StartDate, DateTime PlannedEndDate, DateTime ProjectedEndDate, DateTime PlannedStartDate, 
            DateTime ProjectedStartDate, DateTime ActualEndDate, string Status, Double Basic_Budget, Double ActualExpenditure, string RFPDocument, int TaskLevel, Double GST, Double TotalBudget, Double StatusPer, string Discipline, string UnitforProgress,string Currency,string Currency_CultureInfo,
            double Task_Weightage,string Task_Type,Guid Workpackage_Option,double UnitQuantity)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateMainTasks"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Owner", Owner);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@RFPReference", RFPReference);
                        cmd.Parameters.AddWithValue("@POReference", POReference);
                        if (StartDate == DateTime.MinValue)
                        {
                            cmd.Parameters.AddWithValue("@StartDate", System.Data.SqlTypes.SqlDateTime.MinValue.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        }
                        
                        cmd.Parameters.AddWithValue("@PlannedEndDate", PlannedEndDate);

                        if (ProjectedEndDate == DateTime.MinValue)
                        {
                            cmd.Parameters.AddWithValue("@ProjectedEndDate", System.Data.SqlTypes.SqlDateTime.MinValue.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ProjectedEndDate", ProjectedEndDate);
                        }
                        if (PlannedStartDate == DateTime.MinValue)
                        {
                            cmd.Parameters.AddWithValue("@PlannedStartDate", System.Data.SqlTypes.SqlDateTime.MinValue.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@PlannedStartDate", PlannedStartDate);
                        }
                        
                        cmd.Parameters.AddWithValue("@ProjectedStartDate", ProjectedStartDate);
                        if (ActualEndDate == DateTime.MinValue)
                        {
                            cmd.Parameters.AddWithValue("@ActualEndDate", System.Data.SqlTypes.SqlDateTime.MinValue.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ActualEndDate", ActualEndDate);
                        }
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@Budget", TotalBudget);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@RFPDocument", RFPDocument);
                        cmd.Parameters.AddWithValue("@TaskLevel", TaskLevel);
                        cmd.Parameters.AddWithValue("@StatusPer", StatusPer);
                        cmd.Parameters.AddWithValue("@Discipline", Discipline);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Task_Weightage", Task_Weightage);
                        cmd.Parameters.AddWithValue("@Task_Type", Task_Type);
                        cmd.Parameters.AddWithValue("@Workpackage_Option", Workpackage_Option);
                        cmd.Parameters.AddWithValue("@UnitQuantity", UnitQuantity);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public Boolean InsertorUpdateMainTask_CopyData(Guid TaskUID, Guid WorkPackageUID, Guid ProjectUID, string Owner, string Name, string Description, string RFPReference, string POReference, DateTime StartDate, DateTime PlannedEndDate, DateTime ProjectedEndDate, DateTime PlannedStartDate,
            DateTime ProjectedStartDate, DateTime ActualEndDate, string Status, Double Basic_Budget, Double ActualExpenditure, string RFPDocument, int TaskLevel, Double GST, Double TotalBudget, Double StatusPer, string Discipline, string UnitforProgress, string Currency, string Currency_CultureInfo,object Task_Order)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateMainTasks_CopyData"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Owner", Owner);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@RFPReference", RFPReference);
                        cmd.Parameters.AddWithValue("@POReference", POReference);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@PlannedEndDate", PlannedEndDate);
                        cmd.Parameters.AddWithValue("@ProjectedEndDate", ProjectedEndDate);

                        cmd.Parameters.AddWithValue("@PlannedStartDate", PlannedStartDate);
                        cmd.Parameters.AddWithValue("@ProjectedStartDate", ProjectedStartDate);
                        cmd.Parameters.AddWithValue("@ActualEndDate", ActualEndDate);

                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@Budget", TotalBudget);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@RFPDocument", RFPDocument);
                        cmd.Parameters.AddWithValue("@TaskLevel", TaskLevel);
                        cmd.Parameters.AddWithValue("@StatusPer", StatusPer);
                        cmd.Parameters.AddWithValue("@Discipline", Discipline);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Task_Order", Task_Order);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public Boolean InsertorUpdateSubTask(Guid TaskUID, Guid WorkPackageUID, Guid ProjectUID, string Owner, string Name, string Description, string RFPReference, string POReference, DateTime StartDate, DateTime PlannedEndDate, DateTime ProjectedEndDate, DateTime PlannedStartDate, DateTime ProjectedStartDate, DateTime ActualEndDate, 
            string Status, Double Basic_Budget, Double ActualExpenditure, string RFPDocument, int TaskLevel, string ParentTaskID, Double GST, Double TotalBudget, Double StatusPer, string Discipline, string UnitforProgress,
            string Currency,string Currency_CultureInfo,double Task_Weightage, string Task_Type,Guid Workpackage_Option,double UnitQuantity)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateSubTasks"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Owner", Owner);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@RFPReference", RFPReference);
                        cmd.Parameters.AddWithValue("@POReference", POReference);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@PlannedEndDate", PlannedEndDate);
                        cmd.Parameters.AddWithValue("@ProjectedEndDate", ProjectedEndDate);

                        cmd.Parameters.AddWithValue("@PlannedStartDate", PlannedStartDate);
                        cmd.Parameters.AddWithValue("@ProjectedStartDate", ProjectedStartDate);
                        cmd.Parameters.AddWithValue("@ActualEndDate", ActualEndDate);

                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@Budget", TotalBudget);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@RFPDocument", RFPDocument);
                        cmd.Parameters.AddWithValue("@TaskLevel", TaskLevel);
                        cmd.Parameters.AddWithValue("@ParentTaskID", ParentTaskID);
                        cmd.Parameters.AddWithValue("@StatusPer", StatusPer);
                        cmd.Parameters.AddWithValue("@Discipline", Discipline);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Task_Weightage", Task_Weightage);
                        cmd.Parameters.AddWithValue("@Task_Type", Task_Type);
                        cmd.Parameters.AddWithValue("@Workpackage_Option", Workpackage_Option);
                        cmd.Parameters.AddWithValue("@UnitQuantity", UnitQuantity);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }


        public Boolean InsertorUpdateSubTask_CopyData(Guid TaskUID, Guid WorkPackageUID, Guid ProjectUID, string Owner, string Name, string Description, string RFPReference, string POReference, DateTime StartDate, DateTime PlannedEndDate, DateTime ProjectedEndDate, DateTime PlannedStartDate, DateTime ProjectedStartDate, DateTime ActualEndDate,
            string Status, Double Basic_Budget, Double ActualExpenditure, string RFPDocument, int TaskLevel, string ParentTaskID, Double GST, Double TotalBudget, Double StatusPer, string Discipline, string UnitforProgress, string Currency, string Currency_CultureInfo, object Task_Order)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateSubTasks_CopyData"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Owner", Owner);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@RFPReference", RFPReference);
                        cmd.Parameters.AddWithValue("@POReference", POReference);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@PlannedEndDate", PlannedEndDate);
                        cmd.Parameters.AddWithValue("@ProjectedEndDate", ProjectedEndDate);

                        cmd.Parameters.AddWithValue("@PlannedStartDate", PlannedStartDate);
                        cmd.Parameters.AddWithValue("@ProjectedStartDate", ProjectedStartDate);
                        cmd.Parameters.AddWithValue("@ActualEndDate", ActualEndDate);

                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@Budget", TotalBudget);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@RFPDocument", RFPDocument);
                        cmd.Parameters.AddWithValue("@TaskLevel", TaskLevel);
                        cmd.Parameters.AddWithValue("@ParentTaskID", ParentTaskID);
                        cmd.Parameters.AddWithValue("@StatusPer", StatusPer);
                        cmd.Parameters.AddWithValue("@Discipline", Discipline);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Task_Order", Task_Order);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }


        public Boolean InsertorUpdateMainTask_From_Master(Guid TaskUID, Guid WorkPackageUID, Guid ProjectUID, Guid Workpackage_Option, string Owner, string Name, string Description, string Status, Double Basic_Budget, 
            Double ActualExpenditure, int TaskLevel, Double GST, Double TotalBudget, Double StatusPer, string Currency, string Currency_CultureInfo,int Task_Order)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateMainTasks_From_Master"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Workpackage_Option", Workpackage_Option);
                        cmd.Parameters.AddWithValue("@Owner", Owner);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@Budget", TotalBudget);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@TaskLevel", TaskLevel);
                        cmd.Parameters.AddWithValue("@StatusPer", StatusPer);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Task_Order", Task_Order);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public Boolean InsertorUpdateSubTask_From_Master(Guid TaskUID, Guid WorkPackageUID, Guid ProjectUID, Guid Workpackage_Option, string Owner, string Name, string Description, string Status, Double Basic_Budget,
            Double ActualExpenditure, int TaskLevel, Double GST, Double TotalBudget, Double StatusPer, string Currency, string Currency_CultureInfo, int Task_Order,string ParentTaskID)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateSubTasks_From_Master"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Workpackage_Option", Workpackage_Option);
                        cmd.Parameters.AddWithValue("@Owner", Owner);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@Budget", TotalBudget);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@TaskLevel", TaskLevel);
                        cmd.Parameters.AddWithValue("@StatusPer", StatusPer);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Task_Order", Task_Order);
                        cmd.Parameters.AddWithValue("@ParentTaskID", ParentTaskID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //added on 10/04/2019
        public DataSet getTaskMileStones(Guid sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetMilestonesForTask", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 10/04/2019
        public Boolean InsertorUpdateMileStone(Guid MileStoneUID, Guid TaskUID, string Description, DateTime MileStoneDate, string Status, DateTime CreatedDate, DateTime ProjectedDate, Guid UserUID)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateMileStones"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@MileStoneUID", MileStoneUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Description ", Description);
                        cmd.Parameters.AddWithValue("@MileStoneDate", MileStoneDate);
                        cmd.Parameters.AddWithValue("@Status ", Status);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@ProjectedDate", ProjectedDate);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //added on 10/04/2019
        public DataSet getMileStonesDetails(Guid sMileStoneUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetMileStoneDetails", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@MileStoneUID", sMileStoneUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        //added on 11/04/2019

        public Boolean InsertorUpdateFinanceMileStone(Guid Finance_MileStoneUID, Guid TaskUID, string Finance_MileStoneName, double Finance_AllowedPayment, double Finance_GST, 
            DateTime Finance_PlannedDate,Guid User_Created)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("FinanceMileStone_Insert_or_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Finance_MileStoneName ", Finance_MileStoneName);
                        cmd.Parameters.AddWithValue("@Finance_AllowedPayment", Finance_AllowedPayment);
                        cmd.Parameters.AddWithValue("@Finance_GST ", Finance_GST);
                        cmd.Parameters.AddWithValue("@Finance_PlannedDate", Finance_PlannedDate);
                        cmd.Parameters.AddWithValue("@User_Created", User_Created);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public DataSet GetFinance_MileStonesDetails_By_Finance_MileStoneUID(Guid Finance_MileStoneUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("FinanceMileStone_Selecctby_Finance_MileStoneUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public DataSet GetFinance_MileStonesDetails_By_TaskUID(Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("FinanceMileStone_Selecctby_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int FinanceMileStone_Delete(Guid Finance_MileStoneUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("FinanceMileStone_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public double FinanceMileStone_CulumativeAmount(Guid Finance_MileStoneUID)
        {
            double Percentage = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("FinanceMileStone_CulumativeAmount", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                Percentage = (double)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                Percentage = 0;
            }
            return Percentage;
        }

        public Boolean FinanceMileStonePaymentUpdate_Insert(Guid FinanceMileStoneUpdate_UID, Guid Finance_MileStoneUID, double Allowed_Payment, double Actual_Payment, 
            DateTime Actual_PaymentDate, Guid Updated_User)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("FinanceMileStonePaymentUpdate_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@FinanceMileStoneUpdate_UID", FinanceMileStoneUpdate_UID);
                        cmd.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                        cmd.Parameters.AddWithValue("@Allowed_Payment ", Allowed_Payment);
                        cmd.Parameters.AddWithValue("@Actual_Payment", Actual_Payment);
                        cmd.Parameters.AddWithValue("@Actual_PaymentDate ", Actual_PaymentDate);
                        cmd.Parameters.AddWithValue("@Updated_User", Updated_User);
                        
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public DataSet FinanceMileStonePaymentUpdate_Selectby_Finance_MileStoneUID(Guid Finance_MileStoneUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("FinanceMileStonePaymentUpdate_Selectby_Finance_MileStoneUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string FinanceMileStoneAllowedPayment_Finance_MileStoneUID(Guid Finance_MileStoneUID)
        {
            string val = "0";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("FinanceMileStoneAllowedPayment_Finance_MileStoneUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                val = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                val = "0";
            }
            return val;
        }

        public int InsertorUpdateDependency(Guid Dependency_UID, Guid WorkPackageUID, Guid TaskUID, Guid Dependent_TaskUID, string Dependency_Name, DateTime Dependency_StartDate, DateTime Dependency_PlannedEndDate, string Dependency_Desc, string Dependency_Type, Double Dependency_PercentageComplete)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_Dependency_Insert_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Dependency_UID", Dependency_UID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Dependent_TaskUID", Dependent_TaskUID);
                        cmd.Parameters.AddWithValue("@Dependency_Name", Dependency_Name);
                        cmd.Parameters.AddWithValue("@Dependency_StartDate", Dependency_StartDate);
                        cmd.Parameters.AddWithValue("@Dependency_PlannedEndDate", Dependency_PlannedEndDate);
                        cmd.Parameters.AddWithValue("@Dependency_Desc", Dependency_Desc);
                        cmd.Parameters.AddWithValue("@Dependency_Type", Dependency_Type);
                        cmd.Parameters.AddWithValue("@Dependency_PercentageComplete", Dependency_PercentageComplete);
                        con.Open();
                        sresult=(int)cmd.ExecuteNonQuery();
                        con.Close();
                        
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        //added on 11/04/2019
        public DataSet getDependencies(Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_Dependency_selectBy_TaskID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getDependencies_by_Dependent_TaskUID(Guid Dependent_TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_Dependency_selectBy_Dependent_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Dependent_TaskUID", Dependent_TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 11/04/2019
        public DataSet getDependencies_by_UID(Guid Dependency_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_Dependency_selectBy_Dependency_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Dependency_UID", Dependency_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 11/04/2019
        public DataSet getTaskPayments(Guid sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskPayments", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public string GetAllowedPayment_TaskUID(Guid TaskUID)
        {
            string val = "0";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetAllowedPayment_TaskUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                val = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                val = "0";
            }
            return val;
        }

       

        public DataSet getMonthly_TaskPayments_by_WorkPackageUID(Guid WorkPackageUID,int Year)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getTaskPayment_by_WorkPackageID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Year", Year);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 11/04/2019
        public DataSet getTaskPaymentsDetails(Guid sPaymentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskPaymentDetails", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@PaymentUID", sPaymentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 11/04/2019
        public Boolean InsertorUpdateTaskPayment(Guid PaymentUID, Guid TaskUID, Guid MileStoneUID, Double AllowedPayment, Double ActualPayment, DateTime CreatedDate, DateTime ActualPaymentDate, Guid UserUID)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateTaskPayments"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@PaymentUID", PaymentUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@MileStoneUID", MileStoneUID);
                        cmd.Parameters.AddWithValue("@AllowedPayment", AllowedPayment);
                        cmd.Parameters.AddWithValue("@ActualPayment", ActualPayment);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@ActualPaymentDate", ActualPaymentDate);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        //cmd.Parameters.AddWithValue("@InVoiceUID", InVoiceUID);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }


        public Boolean InsertorUpdateMileStonePayment(Guid MileStonePaymentUID, Guid TaskUID, Guid MileStoneUID, string Payment_Type, string Payment_by, int Payment_Tenture, Double AllowedPayment, Double ActualPayment, DateTime Payment_Date, DateTime ActualPaymentDate, Guid UserUID, Guid InVoiceUID)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateMilestonePayments"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@PaymentUID", MileStonePaymentUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@MileStoneUID", MileStoneUID);
                        cmd.Parameters.AddWithValue("@Payment_Type", Payment_Type);
                        cmd.Parameters.AddWithValue("@Payment_by", Payment_by);
                        cmd.Parameters.AddWithValue("@Payment_Tenture", Payment_Tenture);
                        cmd.Parameters.AddWithValue("@AllowedPayment", AllowedPayment);
                        cmd.Parameters.AddWithValue("@ActualPayment", ActualPayment);
                        cmd.Parameters.AddWithValue("@Payment_Date", Payment_Date);
                        cmd.Parameters.AddWithValue("@ActualPaymentDate", ActualPaymentDate);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@InVoiceUID", InVoiceUID);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public DataSet getMilestonePayment_by_UID(Guid MileStonePaymentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getMilestonePayment_by_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@MileStonePaymentUID", MileStonePaymentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 11/04/2019
        public DataSet getTotalPaymentTask(Guid sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTotalPaymentTask", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        //added on 11/04/2019
        public DataSet getIssuesList()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_GetIssuesList", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        //added on 12/04/2019
        public DataSet getIssuesList_by_UID(Guid Issue_Uid)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_GetIssuesList_by_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getIssuesList_by_WorkPackageUID(Guid WorkPackagesUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_GetIssuesList_by_WorkPackageUID_Android", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", WorkPackagesUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getIssuesList_by_TaskUID(Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_GetIssuesList_by_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Get_Open_Closed_Rejected_Issues_by_WorkPackageUID(Guid WorkPackagesUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_Get_Open_Closed_Rejected_Issues_by_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackagesUID", WorkPackagesUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Get_Open_Closed_Rejected_Issues_by_TaskUID(Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_Get_Open_Closed_Rejected_Issues_by_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int Issues_Status_Update(Guid Issue_Uid, string Issue_Status, string Issue_Remarks)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Issues_Status_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                        cmd.Parameters.AddWithValue("@Issue_Status", Issue_Status);
                        cmd.Parameters.AddWithValue("@Issue_Remarks", Issue_Remarks);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }
        //added on 12/04/2019
        public int InsertorUpdateIssues(Guid Issue_Uid, Guid TaskUID, string Issue_Description, DateTime Issue_Date, Guid Issued_User, Guid Assigned_User, DateTime Assigned_Date, DateTime Issue_ProposedCloser_Date, Guid Approving_User, DateTime Actual_Closer_Date, string Issue_Status, string Issue_Remarks, Guid WorkPackagesUID, Guid ProjectUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_IsuuesInsertUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Issue_Description", Issue_Description);
                        cmd.Parameters.AddWithValue("@Issue_Date", Issue_Date);
                        cmd.Parameters.AddWithValue("@Issued_User", Issued_User);
                        cmd.Parameters.AddWithValue("@Assigned_User", Assigned_User);
                        cmd.Parameters.AddWithValue("@Assigned_Date", Assigned_Date);
                        cmd.Parameters.AddWithValue("@Issue_ProposedCloser_Date", Issue_ProposedCloser_Date);
                        cmd.Parameters.AddWithValue("@Approving_User", Approving_User);
                        cmd.Parameters.AddWithValue("@Actual_Closer_Date", Actual_Closer_Date);
                        cmd.Parameters.AddWithValue("@Issue_Status", Issue_Status);
                        cmd.Parameters.AddWithValue("@Issue_Remarks", Issue_Remarks);
                        cmd.Parameters.AddWithValue("@WorkPackagesUID", WorkPackagesUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int Issues_Status_Remarks_Insert(Guid IssueRemarksUID, Guid Issue_Uid, string Issue_Status, string Issue_Remarks, string Issue_Document)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_Issue_Status_Remarks_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@IssueRemarksUID", IssueRemarksUID);
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                        cmd.Parameters.AddWithValue("@Issue_Status", Issue_Status);
                        cmd.Parameters.AddWithValue("@Issue_Remarks", Issue_Remarks);
                        cmd.Parameters.AddWithValue("@Issue_Document", Issue_Document);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }
        public string getDocumentStatus_by_StatusUID(Guid StatusUID)
        {
            string sUser = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_getDocumentStatus_by_StatusUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                sUser = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sUser = "Error : " + ex.Message;
            }
            return sUser;
        }

        //added on 15/04/2019
        public DataSet getDocumentVersions_by_StatusUID(Guid DocStatus_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_DocumentVersion_Selectby_StatusUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocStatus_UID", DocStatus_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 15/04/2019
        public DataSet getDocumentVersions_by_VersioUID(Guid DocVersion_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_DocumentVersion_Selectby_VersionUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocVersion_UID", DocVersion_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        
        //added on 16/04/2019
        public string getUserNameby_UID(Guid sUserUID)
        {
            string sUser = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetUserName_by_UID", con);
                cmd.CommandType=CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserUID", sUserUID);
                sUser = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sUser = "Error : " + ex.Message;
            }
            return sUser;
        }

        //added on 16/04/2019
        public DataSet getAlerts()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_getAlerts", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getAlerts_by_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_getAlrets_by_ProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getAlerts_by_WorkPackageUID(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_getAlrets_by_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getAlerts_by_TaskUID(Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_getAlrets_by_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 16/04/2019

        //public int InsertorUpdateDocumentStatus(Guid StatusUID, Guid DocumentUID, double Version, string ActivityType, double Activity_Budget, DateTime ActivityDate, 
        //    string LinkToReviewFile, Guid AcivityUserUID, string Status_Comments, string Current_Status,string Ref_Number,DateTime DocumentDate,string CoverLetterFile)
        //{
        //    int sresult = 0;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
        //        {

        //            using (SqlCommand cmd = new SqlCommand("ups_DocumentStatus_Insert_Update"))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
        //                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
        //                cmd.Parameters.AddWithValue("@Version", Version);
        //                cmd.Parameters.AddWithValue("@ActivityType", ActivityType);
        //                cmd.Parameters.AddWithValue("@Activity_Budget", Activity_Budget);
        //                cmd.Parameters.AddWithValue("@ActivityDate", ActivityDate);
        //                cmd.Parameters.AddWithValue("@LinkToReviewFile", LinkToReviewFile);
        //                cmd.Parameters.AddWithValue("@AcivityUserUID", AcivityUserUID);
        //                cmd.Parameters.AddWithValue("@Status_Comments", Status_Comments);
        //                cmd.Parameters.AddWithValue("@Current_Status", Current_Status);
        //                cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
        //                cmd.Parameters.AddWithValue("@DocumentDate", DocumentDate);
        //                cmd.Parameters.AddWithValue("@CoverLetterFile", CoverLetterFile);
        //                con.Open();
        //                sresult = (int)cmd.ExecuteNonQuery();
        //                con.Close();

        //            }
        //        }
        //        return sresult;
        //    }
        //    catch (Exception ex)
        //    {
        //        return sresult = 0;
        //    }
        //}

        public int InsertorUpdateDocumentStatus(Guid StatusUID, Guid DocumentUID, double Version, string ActivityType, double Activity_Budget, DateTime ActivityDate,
          string LinkToReviewFile, Guid AcivityUserUID, string Status_Comments, string Current_Status, string Ref_Number, DateTime DocumentDate, string CoverLetterFile,
          string UpdateStatusto, string Origin)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_DocumentStatus_Insert_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@Version", Version);
                        cmd.Parameters.AddWithValue("@ActivityType", ActivityType);
                        cmd.Parameters.AddWithValue("@Activity_Budget", Activity_Budget);
                        cmd.Parameters.AddWithValue("@ActivityDate", ActivityDate);
                        cmd.Parameters.AddWithValue("@LinkToReviewFile", LinkToReviewFile);
                        cmd.Parameters.AddWithValue("@AcivityUserUID", AcivityUserUID);
                        cmd.Parameters.AddWithValue("@Status_Comments", Status_Comments);
                        cmd.Parameters.AddWithValue("@Current_Status", Current_Status);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@DocumentDate", DocumentDate);
                        cmd.Parameters.AddWithValue("@CoverLetterFile", CoverLetterFile);
                        cmd.Parameters.AddWithValue("@UpdateStatusto", UpdateStatusto);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        //added on 16/04/2019

        public int InsertDocumentVersion(Guid DocVersion_UID, Guid DocStatus_UID, Guid DocumentUID, string Doc_Type, string Doc_FileName,string Doc_Comments)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_DocumentVesrion_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocVersion_UID", DocVersion_UID);
                        cmd.Parameters.AddWithValue("@DocStatus_UID", DocStatus_UID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_FileName", Doc_FileName);
                        cmd.Parameters.AddWithValue("@Doc_Comments", Doc_Comments);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int InsertDocumentVersion(Guid DocVersion_UID, Guid DocStatus_UID, Guid DocumentUID, string Doc_Type, string Doc_FileName, string Doc_Comments, string Doc_CoverLetter)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_DocumentVesrion_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocVersion_UID", DocVersion_UID);
                        cmd.Parameters.AddWithValue("@DocStatus_UID", DocStatus_UID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_FileName", Doc_FileName);
                        cmd.Parameters.AddWithValue("@Doc_Comments", Doc_Comments);
                        cmd.Parameters.AddWithValue("@Doc_CoverLetter", Doc_CoverLetter);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        //added on 16/04/2019
        public int getDocumentStatusVersion(Guid DocStatus_UID, Guid DocumentUID)
        {
            int ver = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("ups_GetDocumentStatusVersion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocStatus_UID", DocStatus_UID);
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                ver = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                ver = 0;
            }
            return ver;
        }



        public DateTime GetDocumentMax_ActualDate(Guid DocumentUID)
        {
            DateTime ver = DateTime.MinValue;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetDocument_Last_ActivityDate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                ver = (DateTime)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                ver = DateTime.MinValue;
            }
            return ver;
        }


        //added on 15/04/2019
        public DataSet getResourceMaster(Guid sWorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetResourceMaster", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", sWorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        //added on 15/04/2019
        public DataSet getResourceMasterDetails(Guid sResourceUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetResourceMasterByUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceUID", sResourceUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getResourceCostType()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getRespurceCostTypeList", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 15/04/2019
        public Boolean InsertorUpdateResourceMaster(Guid ResourceUID, Guid ProjectUID, Guid WorkPackageUID, string ResourceName, string CostType, Double Basic_Budget, Double GST, Double Total_Budget, 
            string Resource_Description, string Unit_for_Measurement, Guid ResourceType_UID,string Currency,string Currency_CultureInfo)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateResourceMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ResourceName", ResourceName);
                        cmd.Parameters.AddWithValue("@CostType", CostType);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@Total_Budget", Total_Budget);
                        cmd.Parameters.AddWithValue("@Resource_Description", Resource_Description);
                        cmd.Parameters.AddWithValue("@Unit_for_Measurement", Unit_for_Measurement);
                        cmd.Parameters.AddWithValue("@ResourceType_UID", ResourceType_UID);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //added on 16/04/2019
        public DataSet getTaskResourceAllocated(Guid sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskResourceAllocated", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        //added on 16/04/2019
        public Boolean InsertorUpdateResourceAllocated(Guid UID, Guid ResourceUID, Guid ProjectUID, Guid WorkPackageUID, Guid TaskUID, Double AllocatedUnits, Guid UserUID)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateResourceAllocation"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@AllocatedUnits", AllocatedUnits);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //added on 16/04/2019
        public DataSet getTaskResourceAllocatedDetails(Guid ResourceAllocationUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetResourceAllocationDetails", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceAllocationUID", ResourceAllocationUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        //added on 22/04/2019
        public DataSet getTotalResourceUsage(Guid sResourceAllocationUID, Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTotalResourceUsage", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceAllocationUID", sResourceAllocationUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 22/04/2019
        public Boolean InsertorUpdateResourceUsage(Guid UID, Guid ResourceAllocatedUID, Guid ProjectUID, Guid WorkPackageUID, Guid TaskUID, Double Usage, DateTime ToDate, DateTime CreatedDate, Guid UserUID)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateResourceUsage"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@ResourceAllocatedUID", ResourceAllocatedUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Usage", Usage);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //added on 22/04/2019
        public DataSet GetTaskMeasurementBook(Guid sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskMeasurementBook", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public double GetMeasurementCumulativeQuantity(Guid TaskUID)
        {
            double Percentage = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetMeasurementCumulativeQuantity", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                Percentage = (double)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                Percentage = 0;
            }
            return Percentage;
        }

        public DataSet GetMeasurementBook_By_UID(Guid UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetMeasurementBook_By_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UID", UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 22/04/2019
        //public int InsertorUpdateTaskMeasurementBook(Guid UID, Guid TaskUID, string UnitforProgress, string Quantity, string Description, DateTime CreatedDate, string Upload_File, Guid CreatedByUID)
        //{
        //    int sresult =0;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
        //        {

        //            using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateMeasurementBook"))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                con.Open();
        //                cmd.Parameters.AddWithValue("@UID", UID);
        //                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
        //                cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
        //                cmd.Parameters.AddWithValue("@Quantity", Quantity);
        //                cmd.Parameters.AddWithValue("@Description", Description);
        //                cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
        //                cmd.Parameters.AddWithValue("@Upload_File", Upload_File);
        //                cmd.Parameters.AddWithValue("@CreatedByUID", CreatedByUID);
        //                sresult= cmd.ExecuteNonQuery();
        //                con.Close();                        
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        sresult = 0;
        //    }
        //    return sresult;
        //}

            // added on 07/12/2021
        public int InsertorUpdateTaskMeasurementBook(Guid UID, Guid TaskUID, string UnitforProgress, string Quantity, string Description, DateTime CreatedDate, string Upload_File, Guid CreatedByUID, string Remarks, string ServerCopiedAdd, DateTime Achieved_Date)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateMeasurementBook"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@Upload_File", Upload_File);
                        cmd.Parameters.AddWithValue("@CreatedByUID", CreatedByUID);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@Achieved_Date", Achieved_Date);
                        sresult = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                sresult = 0;
            }
            return sresult;
        }

        public int InsertMeasurementBookWithoutTaskGrouping(Guid UID, Guid TaskUID, string UnitforProgress, string Quantity, string Description, DateTime SelectedDate, string Upload_File, Guid CreatedByUID, string Remarks, DateTime CreatedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertMeasurementBook_WithoutTaskgrouping"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@SelectedDate", SelectedDate);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@Upload_File", Upload_File);
                        cmd.Parameters.AddWithValue("@CreatedByUID", CreatedByUID);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        sresult = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                sresult = 0;
            }
            return sresult;
        }

        public DataSet GetTaskStatus(Guid sTaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskStatus", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", sTaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public double GetTaskPercentage(Guid TaskUID)
        {
            double Percentage = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_getTaskPercentage", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                Percentage = (double)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                Percentage = 0;
            }
            return Percentage;
        }


        public double GetTask_ActualExpenditure_by_TaskUID(Guid TaskUID)
        {
            double actualexpenditure = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetTask_ActualExpenditure_by_TaskUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                actualexpenditure = (double)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                actualexpenditure = 0;
            }
            return actualexpenditure;
        }

        //added on 22/04/2019
        public string getTaskNameby_TaskUID(Guid TaskUID)
        {
            string sUser = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("ups_getTaskNameby_TaskUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                sUser = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sUser = "Error : " + ex.Message;
            }
            return sUser;
        }

        public string getTaskName_From_DocumentID(Guid DocumentID)
        {
            string sTaskName = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetTaskName_From_DocumentID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentID", DocumentID);
                sTaskName = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sTaskName = "Error : " + ex.Message;
            }
            return sTaskName;
        }
        //added on 24/04/2019
        public void StoreEmaildataToMailQueue(Guid MailUID, Guid UserUID, string FromEmailID, string ToEmailID, string Subject, string Body, string MailSentDate, string CCTo,string Attachment)
        {
            try
            {
                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlCommand SqlCmd = new SqlCommand("ups_Insert_Mails", SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@MailUID", MailUID);
                    SqlCmd.Parameters.AddWithValue("@UserUID", UserUID);
                    SqlCmd.Parameters.AddWithValue("@FromEmailID", FromEmailID);
                    SqlCmd.Parameters.AddWithValue("@ToEmailID", ToEmailID);
                    SqlCmd.Parameters.AddWithValue("@Subject", Subject);
                    SqlCmd.Parameters.AddWithValue("@Body", Body);
                    SqlCmd.Parameters.AddWithValue("@CCTo", CCTo);
                    SqlCmd.Parameters.AddWithValue("@MailSentDate", MailSentDate);
                    SqlCmd.Parameters.AddWithValue("@Attachment", Attachment);
                    SqlConn.Open();
                    SqlCmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //30/04/2019
        public Boolean InsertorUpdateBankGuarantee(Guid Bank_GuaranteeUID, string Vendor_Name, string Vendor_Address, double Amount, string Validity, DateTime Date_of_Guarantee, int No_of_Collaterals, 
            string Bank_Name, string Bank_Branch, string IFSC_Code, Guid WorkPackageUID, Guid ProjectUID,string BG_Number,DateTime Date_of_Expiry,string Currency,string Currency_CultureInfo)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_InsertorUpdate_BankGuarantee"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@Bank_GuaranteeUID", Bank_GuaranteeUID);
                        cmd.Parameters.AddWithValue("@Vendor_Name", Vendor_Name);
                        cmd.Parameters.AddWithValue("@Vendor_Address", Vendor_Address);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@Validity", Validity);
                        cmd.Parameters.AddWithValue("@Date_of_Guarantee", Date_of_Guarantee);
                        cmd.Parameters.AddWithValue("@No_of_Collaterals", No_of_Collaterals);
                        cmd.Parameters.AddWithValue("@Bank_Name", Bank_Name);
                        cmd.Parameters.AddWithValue("@Bank_Branch", Bank_Branch);
                        cmd.Parameters.AddWithValue("@IFSC_Code", IFSC_Code);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@BG_Number", BG_Number);
                        cmd.Parameters.AddWithValue("@Date_of_Expiry", Date_of_Expiry);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }
        //30/04/2019
        public DataSet GetBankGuarantee()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_BankGuaranteeSelect", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        //30/04/2019
        public DataSet GetBankGuarantee_by_Bank_GuaranteeUID(Guid Bank_GuaranteeUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_GetBankGuaranteeSelect_by_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Bank_GuaranteeUID", Bank_GuaranteeUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetBankGuarantee_by_Bank_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_GetBankGuarantee_by_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //30/04/2019

        
        //30/04/2019
        public DataSet GetBankDocuments_by_BankGuarantee_UID(Guid Bank_GuaranteeUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_BankDocuments_selectBy_BankUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Bank_GuaranteeUID", Bank_GuaranteeUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetBankDocuments_by_BankDoc_UID(Guid BankDoc_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_getBankDocuments_by_BankDocUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@BankDoc_UID", BankDoc_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //30/04/2019

        public Boolean InsertorUpdateInsurance(Guid InsuranceUID, string Vendor_Name, string Vendor_Address, string Name_of_InsuranceCompany, string Branch, string Policy_Number, string Policy_Status, DateTime Maturity_Date, string Nominee, Guid ProjectUID, Guid WorkPackageUID)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_InsertorUpdateInsurance"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                        cmd.Parameters.AddWithValue("@Vendor_Name", Vendor_Name);
                        cmd.Parameters.AddWithValue("@Vendor_Address", Vendor_Address);
                        cmd.Parameters.AddWithValue("@Name_of_InsuranceCompany", Name_of_InsuranceCompany);
                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@Policy_Number", Policy_Number);
                        cmd.Parameters.AddWithValue("@Policy_Status", Policy_Status);
                        cmd.Parameters.AddWithValue("@Maturity_Date", Maturity_Date);
                        cmd.Parameters.AddWithValue("@Nominee", Nominee);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //30/04/2019
        public DataSet GetInsurance()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_InsuranceSelect", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //30/04/2019
        public DataSet GetInsuranceSelect_by_InsuranceUID(Guid InsuranceUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_InsuranceSelect_by_InsuranceUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public Boolean InsertorUpdateInsuranceDocuments(Guid InsuranceDoc_UID, Guid InsuranceUID, string InsuranceDoc_Name, string InsuranceDoc_Type, string InsuranceDoc_FilePath)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_Insert_Update_InsuranceDocuments"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@InsuranceDoc_UID", InsuranceDoc_UID);
                        cmd.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                        cmd.Parameters.AddWithValue("@InsuranceDoc_Name", InsuranceDoc_Name);
                        cmd.Parameters.AddWithValue("@InsuranceDoc_Type", InsuranceDoc_Type);
                        cmd.Parameters.AddWithValue("@InsuranceDoc_FilePath", InsuranceDoc_FilePath);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public DataSet GetInsurenceDocuments_by_BankInsuranceUID(Guid InsuranceUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getInsurenceDocumentsBy_InsuranceUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetInsuranceDocuments_by_InsuranceDoc_UID(Guid InsuranceDoc_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getInsurenceDocumentsBy_InsuranceDoc_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@InsuranceDoc_UID", InsuranceDoc_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable GetEmailCredentials()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Usp_GetMailCredentials", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(dt);
            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;
        }

        public string SendMail(string toEmailID,string Subject,string sHtmlString,string cc,string Attachment)
        {
            try
            {
                MailMessage mm = new MailMessage();
                DataTable dtemailCred = GetEmailCredentials();
                mm.To.Add(toEmailID);
                mm.From = new MailAddress(dtemailCred.Rows[0][0].ToString(), "Project Monitoring Tool");
                mm.Subject = Subject;

                string[] CCId = cc.Split(',');
                foreach (string CCEmail in CCId)
                {
                    mm.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                }
                if (Attachment != "")
                {
                    mm.Attachments.Add(new Attachment(Attachment));
                }
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.UseDefaultCredentials = false;
                //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(dtemailCred.Rows[0][0].ToString(), dtemailCred.Rows[0][1].ToString());
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("itrustinfo.ipmt@gmail.com", "itrust@123");
                client.Credentials = credentials;
                mm.IsBodyHtml = true;
                //mm.Body = string.Format(sHtmlString);
                mm.Body = sHtmlString;
                client.Send(mm);
                return "Success";
            }
            catch (Exception ex)
            {
                return "Failure : " + ex.Message;
            }
        }

        public string GetDocumentPlannedDate(Guid DocumentUID, Guid AcivityUserUID, string TypeofUser, string Status)
        {
            string sDate = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_getDocumentPlannedDate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.Parameters.AddWithValue("@AcivityUserUID", AcivityUserUID);
                cmd.Parameters.AddWithValue("@TypeofUser", TypeofUser);
                cmd.Parameters.AddWithValue("@Status", Status);
                sDate = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                sDate = "";
            }
            return sDate;
        }

        public DataSet GetUserResource_by_TaskUID_AllocationUID(Guid ResourceAllocatedUID, Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getUserResource_by_TaskUID_AllocationUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceAllocatedUID", ResourceAllocatedUID);
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetStatus_by_UserType(string UserType, string Current_Status)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetStatus_by_UserType", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserType", UserType);
                cmd.SelectCommand.Parameters.AddWithValue("@CurrentStatus", Current_Status);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int InsertorUpdateReviews(Guid Reviews_UID, Guid WorkPackageUID, Guid User_UID, string Review_Type, string Review_freq, string Review_Description, Guid ProjectUID,DateTime Review_Date)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateReviews"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Reviews_UID", Reviews_UID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@User_UID", User_UID);
                        cmd.Parameters.AddWithValue("@Review_Type", Review_Type);
                        cmd.Parameters.AddWithValue("@Review_freq", Review_freq);
                        cmd.Parameters.AddWithValue("@Review_Description", Review_Description);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Review_Date", Review_Date);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public DataSet getReviewList_by_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewList_by_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getReviewList_by_Reviews_UID(Guid Reviews_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewList_by_Reviews_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Reviews_UID", Reviews_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getReviewList_by_User_UID(Guid User_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewList_by_User_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@User_UID", User_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertReviewTaskss(Guid ReviewTask_UID, Guid Review_UID, Guid Task_UID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertReviewTasks"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ReviewTask_UID", ReviewTask_UID);
                        cmd.Parameters.AddWithValue("@Review_UID", Review_UID);
                        cmd.Parameters.AddWithValue("@Task_UID", Task_UID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getReviewTasks_by_Review_UID(Guid Review_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewTasks_by_Review_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Review_UID", Review_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertorUpdateReviewRecords(Guid Review_RecordUID, Guid Review_ID, DateTime Review_Date, string ReviewRecord_Desc, string ReviewRecord_Summary)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateReviewRecords"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Review_RecordUID", Review_RecordUID);
                        cmd.Parameters.AddWithValue("@Review_ID", Review_ID);
                        cmd.Parameters.AddWithValue("@Review_Date", Review_Date);
                        cmd.Parameters.AddWithValue("@ReviewRecord_Desc", ReviewRecord_Desc);
                        cmd.Parameters.AddWithValue("@ReviewRecord_Summary", ReviewRecord_Summary);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getReviewRecords_by_ReviewUID(Guid Review_ID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewRecords_by_ReviewUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Review_ID", Review_ID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getReviewRecords_by_Review_RecordUID(Guid Review_RecordUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewRecords_by_Review_RecordUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Review_RecordUID", Review_RecordUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertReviewRecord_Attendies(Guid ReviewRecord_AttendiesUID, Guid Review_RecordUID, Guid User_UID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertReviewRecord_Attendies"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ReviewRecord_AttendiesUID", ReviewRecord_AttendiesUID);
                        cmd.Parameters.AddWithValue("@Review_RecordUID", Review_RecordUID);
                        cmd.Parameters.AddWithValue("@User_UID", User_UID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getReviewRecord_by_Review_RecordUID(Guid Review_RecordUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewRecord_by_Review_RecordUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Review_RecordUID", Review_RecordUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int InsertorUpdateReviewRecordActions(Guid ReviewRecordAction_UID, Guid Review_RecordUID, string Action_Desc, DateTime Planned_Closer_Date, string Action_Status, string Responsible_Person, string Responsible_Person1, string Responsible_Person2, string Responsible_Person3, string Responsible_Person4, string Responsible_Person5, string Action_Document)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpDateReviewRecordActions"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ReviewRecordAction_UID", ReviewRecordAction_UID);
                        cmd.Parameters.AddWithValue("@Review_RecordUID", Review_RecordUID);
                        cmd.Parameters.AddWithValue("@Action_Desc", Action_Desc);
                        cmd.Parameters.AddWithValue("@Planned_Closer_Date", Planned_Closer_Date);
                        cmd.Parameters.AddWithValue("@Action_Status", Action_Status);
                        cmd.Parameters.AddWithValue("@Responsible_Person", Responsible_Person);
                        cmd.Parameters.AddWithValue("@Responsible_Person1", Responsible_Person1);
                        cmd.Parameters.AddWithValue("@Responsible_Person2", Responsible_Person2);
                        cmd.Parameters.AddWithValue("@Responsible_Person3", Responsible_Person3);
                        cmd.Parameters.AddWithValue("@Responsible_Person4", Responsible_Person4);
                        cmd.Parameters.AddWithValue("@Responsible_Person5", Responsible_Person5);
                        cmd.Parameters.AddWithValue("@Action_Document", Action_Document);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public DataSet getReviewRecordActions_by_Review_RecordUID(Guid Review_RecordUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewRecordActions_by_Review_RecordUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Review_RecordUID", Review_RecordUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getReviewRecordActions_by_ReviewRecordAction_UID(Guid ReviewRecordAction_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewRecordActions_by_ReviewRecordAction_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ReviewRecordAction_UID", ReviewRecordAction_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertorUpdateReviewRecordActionUpdates(Guid UpdateRecordActionUID, Guid ReviewRecordAction_UID, string Action_Status, DateTime StatusRecord_Date, DateTime Revised_CloserDate, string Description)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateReviewRecordActionUpdates"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UpdateRecordActionUID", UpdateRecordActionUID);
                        cmd.Parameters.AddWithValue("@ReviewRecordAction_UID", ReviewRecordAction_UID);
                        cmd.Parameters.AddWithValue("@Action_Status", Action_Status);
                        cmd.Parameters.AddWithValue("@StatusRecord_Date", StatusRecord_Date);
                        cmd.Parameters.AddWithValue("@Revised_CloserDate", Revised_CloserDate);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet ReviewRecordActionUpdates_by_ReviewRecordAction_UID(Guid ReviewRecordAction_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewRecordActionUpdates_by_ReviewRecordAction_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ReviewRecordAction_UID", ReviewRecordAction_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getReviewRecordActionUpdates_by_UpdateRecordActionUID(Guid UpdateRecordActionUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewRecordActionUpdates_by_UpdateRecordActionUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UpdateRecordActionUID", UpdateRecordActionUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertReview_Attendies(Guid ReviewAttendiesUID, Guid Reviews_UID, Guid User_UID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertReview_Attendies"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ReviewAttendiesUID", ReviewAttendiesUID);
                        cmd.Parameters.AddWithValue("@Reviews_UID", Reviews_UID);
                        cmd.Parameters.AddWithValue("@User_UID", User_UID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getReviewAttendies_by_Reviews_UID(Guid Reviews_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getReviewAttendies_by_Reviews_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Reviews_UID", Reviews_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string User_PasswordChange(string Username, string password, string Old_password)
        {
            string sresult = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_UserPasswordChange"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@Old_password", Old_password);
                        con.Open();
                        int Cnt = (int)cmd.ExecuteNonQuery();
                        if (Cnt > 0)
                        {
                            sresult = "Success";
                        }
                        else
                        {
                            sresult = "Invalid Old Password";
                        }
                        con.Close();

                    }
                }

                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = ex.Message;
            }
        }

        public int CheckUserEmail_Exists(string EmailID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_CheckUserEmail_Exists"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@EmailID", EmailID);
                        con.Open();
                        sresult = (int)cmd.ExecuteScalar();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getUserDetails_by_EmailID(string EmailID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getUserDetails_by_EmailID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@EmailID", EmailID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string ForgotPasswordChange(string EmailID, string NewPassword)
        {
            string sresult = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_ForgotPasswordChange"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@EmailID", EmailID);
                        cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
                        con.Open();
                        int Cnt = (int)cmd.ExecuteNonQuery();
                        if (Cnt > 0)
                        {
                            sresult = "Success";
                        }
                        else
                        {
                            sresult = "Invalid Email-ID";
                        }
                        con.Close();

                    }
                }

                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = ex.Message;
            }
        }

        public DataSet getAllDocumentSchedules()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getAllDocumentSchedules", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getDocumentSchedules_by_DocScheduleUID(Guid DocScheduleUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getDocumentSchedules_by_DocScheduleUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocScheduleUID", DocScheduleUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getUnitMaster_List()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getUnits_List", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getResourceTypeMaster_List()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getResourceTypeMaster_List", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertorUpdateResourceProperty(Guid Property_UID, Guid ResourceUID, Guid ResourceType_UID, string Name_of_the_Property, string Table_Name, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_Insert_Update_ResourceProperties"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Property_UID", Property_UID);
                        cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                        cmd.Parameters.AddWithValue("@ResourceType_UID", ResourceType_UID);
                        cmd.Parameters.AddWithValue("@Name_of_the_Property", Name_of_the_Property);
                        cmd.Parameters.AddWithValue("@Table_Name", Table_Name);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getResourceProperties_by_ResourceUID(Guid ResourceUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getResourceProperties_by_ResourceUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getResourceProperties_by_ResourceType_UID(Guid ResourceType_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getResourceProperties_by_ResourceType_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceType_UID", ResourceType_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getSkillMaterList()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getSkillMaterList", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertorUpdateResourcePropertySkills(Guid PropertySkillUID, Guid PropertyUID, Guid Skill_UID, Guid TableUID, string No_of_Years_Exp)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateResourcePropertySkills"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PropertySkillUID", PropertySkillUID);
                        cmd.Parameters.AddWithValue("@PropertyUID", PropertyUID);
                        cmd.Parameters.AddWithValue("@Skill_UID", Skill_UID);
                        cmd.Parameters.AddWithValue("@TableUID", TableUID);
                        cmd.Parameters.AddWithValue("@No_of_Years_Exp", No_of_Years_Exp);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getResourcePropertySkills_by_PropertyUID(Guid PropertyUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getResourcePropertySkills_by_PropertyUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@PropertyUID", PropertyUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertorUpdateUnit_Master(Guid Unit_UID, string Unit_Name)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdate_UnitsMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Unit_UID", Unit_UID);
                        cmd.Parameters.AddWithValue("@Unit_Name", Unit_Name);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public DataSet getUnitMasters_by_Unit_UID(Guid Unit_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getUnitMasters_by_Unit_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Unit_UID", Unit_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertorUpdateResourceCostType(Guid ResourceCostType_UID, string ResourceCost_Type)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdate_ResourceCostType"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ResourceCostType_UID", ResourceCostType_UID);
                        cmd.Parameters.AddWithValue("@ResourceCost_Type", ResourceCost_Type);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getResourceCostTypeMaster_by_Type_UID(Guid ResourceCostType_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getResourceCostTypeMaster_by_Type_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceCostType_UID", ResourceCostType_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int InsertorUpdateResourceType(Guid ResourceType_UID, string ResourceType_Name)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdate_ResourceTypes"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ResourceType_UID", ResourceType_UID);
                        cmd.Parameters.AddWithValue("@ResourceType_Name", ResourceType_Name);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet getResourceType_by_Type_UID(Guid ResourceType_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getResourceTypeMaster_by_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ResourceType_UID", ResourceType_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetAllAssignedProjects()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAssignedProjects", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetAssignedProjects_by_UserUID(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAssignedProject_by_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        internal System.Data.DataSet GetProjects()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetProjects", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetAssignedProjects_by_UserUID_Except_Selected(Guid UserUID,Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAssignedProjects_by_UserUID_Except_Selected", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetAssignedProjectsData_by_UserUID(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAssignedProjectData_by_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetAssignedProjects_by_AssignUID(Guid AssignID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAssignedProjects_by_AssignID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@AssignID", AssignID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public int InsertorUpdateAssignProjects(Guid AssignID, Guid UserUID,Guid ProjectUID, Guid UserRole)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("InsertorUpdateAssignProjects"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AssignID", AssignID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@UserRole", UserRole);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet GetUserRolesMaster()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAllUserRolesMaster", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public string GetUserRolesMasterDesc_by_UID(Guid UserRole_ID)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetUserRolesMaster_by_UID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserRole_ID", UserRole_ID);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public string GetUserRoleID_by_UserRoleName(string UserRole_Name)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetUserRoleID_by_UserRoleName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserRole_Name", UserRole_Name);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public DataSet GetDocumentFlows()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentFlows", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetDocumentFlows_by_UID(Guid FlowMasterUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentFlows_by_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@FlowMasterUID", FlowMasterUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUsers_under_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUsers_under_ProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetWorkPackage_By_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetWorkPackage_By_ProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertorUpdateDocumentTypeMaster(Guid DocumentTypeMasterID, string DocumentType, string DocumentExtension, string DocumentIcon)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("InsertorUpdateDocumentTypeMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentTypeMasterID", DocumentTypeMasterID);
                        cmd.Parameters.AddWithValue("@DocumentType", DocumentType);
                        cmd.Parameters.AddWithValue("@DocumentExtension", DocumentExtension);
                        cmd.Parameters.AddWithValue("@DocumentIcon", DocumentIcon);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet GetDocumentMasterTypes()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentMasterTypes", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetDocumentMasterType_by_UID(Guid DocumentTypeMasterID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentMasterType_by_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentTypeMasterID", DocumentTypeMasterID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string GetDocumentMasterType_by_Extension(string DocumentExtension)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetDocumentMasterType_by_Extension", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentExtension", DocumentExtension);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public string GetDocumentTypeMasterIcon_by_Extension(string DocumentExtension)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetDocumentTypeMasterIcon_by_Extension", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentExtension", DocumentExtension);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }


        public string GetDocumentType_By_Text(string DocumentFor_Text)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetDocumentType_By_Text", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentFor_Text", DocumentFor_Text);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public string GetFlowName_by_SubmittalID(Guid DocumentUID)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetFlowName_by_SubmittalID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public int InsertorUpdateContractor(Guid Contractor_UID, string Contractor_Name, string Contractor_Representatives, string Type_of_Contract,double Contract_Value,int Contract_Duration,
            DateTime Letter_of_Acceptance,DateTime Contract_Completion_Date, DateTime Contract_StartDate,DateTime Contract_Agreement_Date,string Currency,string Currency_CultureInfo,
            string Contractor_Code,string NJSEI_Number,string ProjectSpecific_Number)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Contractor_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Contractor_UID", Contractor_UID);
                        cmd.Parameters.AddWithValue("@Contractor_Name", Contractor_Name);
                        cmd.Parameters.AddWithValue("@Contractor_Representatives", Contractor_Representatives);
                        cmd.Parameters.AddWithValue("@Type_of_Contract", Type_of_Contract);
                        cmd.Parameters.AddWithValue("@Contract_Value", Contract_Value);
                        cmd.Parameters.AddWithValue("@Contract_Duration", Contract_Duration);
                        cmd.Parameters.AddWithValue("@Letter_of_Acceptance", Letter_of_Acceptance);
                        cmd.Parameters.AddWithValue("@Contract_Agreement_Date", Contract_Agreement_Date);
                        cmd.Parameters.AddWithValue("@Contract_StartDate", Contract_StartDate);
                        cmd.Parameters.AddWithValue("@Contract_Completion_Date", Contract_Completion_Date);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Contractor_Code", Contractor_Code);
                        cmd.Parameters.AddWithValue("@NJSEI_Number", NJSEI_Number);
                        cmd.Parameters.AddWithValue("@ProjectSpecific_Number", ProjectSpecific_Number);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet GetContractors()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetContractors", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int Contractor_Delete(Guid Contractor_UID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Contractor_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Contractor_UID", Contractor_UID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet GetContractors_by_ContractorUID(Guid Contractor_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetContractors_by_ContractorUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Contractor_UID", Contractor_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //Date : 18 Jun 2020

        public int ActualDocuments_InsertORUpdate(Guid ActualDocumentUID,Guid DocumentUID,Guid FlowUID,string ActualDocument_Name,
            double ActualDocument_Version,string ActualDocument_Type,string ActualDocument_For,string ActualDocument_Path,string ActualDocument_CurrentStatus,
            Guid SubmissionUserUID,DateTime Sub_TargetDate,Guid Step2UserUID,DateTime Step2User_TargetDate,Guid Step3UserUID, DateTime Step3User_TargetDate, 
            Guid ProjectUID,Guid WorkPackageUID,string FlowStep1_DisplayName,string FlowStep2_DisplayName,string FlowStep3_DisplayName,string CurrentStatus,string Ref_Number)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ActualDocuments_InsertORUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@ActualDocument_For", ActualDocument_For);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@SubmissionUserUID", SubmissionUserUID);
                        cmd.Parameters.AddWithValue("@Sub_TargetDate", Sub_TargetDate);
                        cmd.Parameters.AddWithValue("@Step2UserUID", Step2UserUID);
                        cmd.Parameters.AddWithValue("@Step2User_TargetDate", Step2User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step3UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step3User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep2_DisplayName", FlowStep2_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep3_DisplayName", FlowStep3_DisplayName);
                        cmd.Parameters.AddWithValue("@CurrentStatus", CurrentStatus);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int ActualDocuments_InsertORUpdate_Flow_5(Guid ActualDocumentUID, Guid DocumentUID, Guid FlowUID, string ActualDocument_Name,
            double ActualDocument_Version, string ActualDocument_Type, string ActualDocument_For, string ActualDocument_Path, string ActualDocument_CurrentStatus,
            Guid SubmissionUserUID, DateTime Sub_TargetDate, Guid Step2UserUID, DateTime Step2User_TargetDate, Guid Step3UserUID, DateTime Step3User_TargetDate,
            Guid Step4UserUID, DateTime Step4User_TargetDate, Guid Step5UserUID, DateTime Step5User_TargetDate, Guid ProjectUID, Guid WorkPackageUID,
            string FlowStep1_DisplayName, string FlowStep2_DisplayName, string FlowStep3_DisplayName, string FlowStep4_DisplayName, string FlowStep5_DisplayName, string CurrentStatus,string Ref_Number)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ActualDocuments_InsertORUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@ActualDocument_For", ActualDocument_For);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@SubmissionUserUID", SubmissionUserUID);
                        cmd.Parameters.AddWithValue("@Sub_TargetDate", Sub_TargetDate);
                        cmd.Parameters.AddWithValue("@Step2UserUID", Step2UserUID);
                        cmd.Parameters.AddWithValue("@Step2User_TargetDate", Step2User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step3UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step3User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step4UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step4User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step5UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step5User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep2_DisplayName", FlowStep2_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep3_DisplayName", FlowStep3_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep4_DisplayName", FlowStep3_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep5_DisplayName", FlowStep3_DisplayName);
                        cmd.Parameters.AddWithValue("@CurrentStatus", CurrentStatus);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public int Document_Insert_or_Update(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
          string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
          string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
          string FileRef_Number, string ActualDocument_CurrentStatus, Guid Step1UserUID, DateTime Step1User_TargetDate, Guid Step2UserUID, DateTime Step2User_TargetDate,
          Guid Step3UserUID, DateTime Step3User_TargetDate, string FlowStep1_DisplayName, string FlowStep2_DisplayName, string FlowStep3_DisplayName, string ActualDocument_Originator,
          DateTime Document_Date, string UploadFilePhysicalpath, string CoverLetterUID, string SubmissionType, Guid StatusUID, Guid DocVersionUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Document_Insert_or_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@Step1UserUID", Step1UserUID);
                        cmd.Parameters.AddWithValue("@Step1User_TargetDate", Step1User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step2UserUID", Step2UserUID);
                        cmd.Parameters.AddWithValue("@Step2User_TargetDate", Step2User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step3UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step3User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep2_DisplayName", FlowStep2_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep3_DisplayName", FlowStep3_DisplayName);
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        cmd.Parameters.AddWithValue("@UploadFilePhysicalpath", UploadFilePhysicalpath);

                        if (CoverLetterUID != "")
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@SubmissionType", SubmissionType);
                        //
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@DocVersionUID", DocVersionUID);
                        //
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public int Document_Insert_or_Update_with_RelativePath_Flow1(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
   string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
   string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
   string FileRef_Number, string ActualDocument_CurrentStatus, Guid Step1UserUID, DateTime Step1User_TargetDate, string FlowStep1_DisplayName, string ActualDocument_Originator, DateTime Document_Date,
   string ActualDocument_RelativePath, string ActualDocument_DirectoryName, string UploadFilePhysicalpath, string CoverLetterUID, string SubmissionType)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Document_Insert_or_Update_with_RelativePath_Flow1"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@Step1UserUID", Step1UserUID);
                        cmd.Parameters.AddWithValue("@Step1User_TargetDate", Step1User_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        cmd.Parameters.AddWithValue("@ActualDocument_RelativePath", ActualDocument_RelativePath);
                        cmd.Parameters.AddWithValue("@ActualDocument_DirectoryName", ActualDocument_DirectoryName);
                        cmd.Parameters.AddWithValue("@UploadFilePhysicalpath", UploadFilePhysicalpath);
                        if (CoverLetterUID != "")
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@SubmissionType", SubmissionType);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int Document_Insert_or_Update_with_RelativePath_Flow2(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
          string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
          string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
          string FileRef_Number, string ActualDocument_CurrentStatus, Guid Step1UserUID, DateTime Step1User_TargetDate, Guid Step2UserUID, DateTime Step2User_TargetDate,
          string FlowStep1_DisplayName, string FlowStep2_DisplayName, string ActualDocument_Originator, DateTime Document_Date, string ActualDocument_RelativePath, string ActualDocument_DirectoryName, string UploadFilePhysicalpath, string CoverLetterUID, string SubmissionType, Guid StatusUID, Guid DocVersionUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Document_Insert_or_Update_with_RelativePath_Flow2"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@Step1UserUID", Step1UserUID);
                        cmd.Parameters.AddWithValue("@Step1User_TargetDate", Step1User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step2UserUID", Step2UserUID);
                        cmd.Parameters.AddWithValue("@Step2User_TargetDate", Step2User_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep2_DisplayName", FlowStep2_DisplayName);
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        cmd.Parameters.AddWithValue("@ActualDocument_RelativePath", ActualDocument_RelativePath);
                        cmd.Parameters.AddWithValue("@ActualDocument_DirectoryName", ActualDocument_DirectoryName);
                        cmd.Parameters.AddWithValue("@UploadFilePhysicalpath", UploadFilePhysicalpath);
                        if (CoverLetterUID != "")
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@SubmissionType", SubmissionType);
                        //
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@DocVersionUID", DocVersionUID);
                        //
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int Document_Insert_or_Update_with_RelativePath(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
           string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
           string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
           string FileRef_Number, string ActualDocument_CurrentStatus, Guid Step1UserUID, DateTime Step1User_TargetDate, Guid Step2UserUID, DateTime Step2User_TargetDate,
           Guid Step3UserUID, DateTime Step3User_TargetDate, string FlowStep1_DisplayName, string FlowStep2_DisplayName, string FlowStep3_DisplayName, string ActualDocument_Originator,
           DateTime Document_Date, string ActualDocument_RelativePath, string ActualDocument_DirectoryName, string UploadFilePhysicalpath, string CoverLetterUID, string SubmissionType)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Document_Insert_or_Update_with_RelativePath"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@Step1UserUID", Step1UserUID);
                        cmd.Parameters.AddWithValue("@Step1User_TargetDate", Step1User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step2UserUID", Step2UserUID);
                        cmd.Parameters.AddWithValue("@Step2User_TargetDate", Step2User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step3UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step3User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep2_DisplayName", FlowStep2_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep3_DisplayName", FlowStep3_DisplayName);
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        cmd.Parameters.AddWithValue("@ActualDocument_RelativePath", ActualDocument_RelativePath);
                        cmd.Parameters.AddWithValue("@ActualDocument_DirectoryName", ActualDocument_DirectoryName);
                        cmd.Parameters.AddWithValue("@UploadFilePhysicalpath", UploadFilePhysicalpath);
                        if (CoverLetterUID != "")
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@SubmissionType", SubmissionType);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int Document_Insert_or_Update_with_RelativePath_Flow4(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
          string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
          string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
          string FileRef_Number, string ActualDocument_CurrentStatus, Guid Step1UserUID, DateTime Step1User_TargetDate, Guid Step2UserUID, DateTime Step2User_TargetDate,
          Guid Step3UserUID, DateTime Step3User_TargetDate, Guid Step4UserUID, DateTime Step4User_TargetDate, string FlowStep1_DisplayName, string FlowStep2_DisplayName, string FlowStep3_DisplayName,
          string FlowStep4_DisplayName, string ActualDocument_Originator, DateTime Document_Date, string ActualDocument_RelativePath, string ActualDocument_DirectoryName, string UploadFilePhysicalpath, string CoverLetterUID, string SubmissionType, Guid StatusUID, Guid DocVersionUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Document_Insert_or_Update_with_RelativePath_Flow4"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@Step1UserUID", Step1UserUID);
                        cmd.Parameters.AddWithValue("@Step1User_TargetDate", Step1User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step2UserUID", Step2UserUID);
                        cmd.Parameters.AddWithValue("@Step2User_TargetDate", Step2User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step3UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step3User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step4UserUID", Step4UserUID);
                        cmd.Parameters.AddWithValue("@Step4User_TargetDate", Step4User_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep2_DisplayName", FlowStep2_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep3_DisplayName", FlowStep3_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep4_DisplayName", FlowStep4_DisplayName);
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        cmd.Parameters.AddWithValue("@ActualDocument_RelativePath", ActualDocument_RelativePath);
                        cmd.Parameters.AddWithValue("@ActualDocument_DirectoryName", ActualDocument_DirectoryName);
                        cmd.Parameters.AddWithValue("@UploadFilePhysicalpath", UploadFilePhysicalpath);
                        if (CoverLetterUID != "")
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@SubmissionType", SubmissionType);
                        //
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@DocVersionUID", DocVersionUID);
                        //
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int Document_Insert_or_Update_with_RelativePath_Flow5(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
          string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
          string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
          string FileRef_Number, string ActualDocument_CurrentStatus, Guid Step1UserUID, DateTime Step1User_TargetDate, Guid Step2UserUID, DateTime Step2User_TargetDate,
          Guid Step3UserUID, DateTime Step3User_TargetDate, Guid Step4UserUID, DateTime Step4User_TargetDate, Guid Step5UserUID, DateTime Step5User_TargetDate, string FlowStep1_DisplayName, string FlowStep2_DisplayName,
          string FlowStep3_DisplayName, string FlowStep4_DisplayName, string FlowStep5_DisplayName, string ActualDocument_Originator, DateTime Document_Date, string ActualDocument_RelativePath, string ActualDocument_DirectoryName, string UploadFilePhysicalpath, string CoverLetterUID, string SubmissionType, Guid StatusUID, Guid DocVersionUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Document_Insert_or_Update_with_RelativePath_Flow5"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@Step1UserUID", Step1UserUID);
                        cmd.Parameters.AddWithValue("@Step1User_TargetDate", Step1User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step2UserUID", Step2UserUID);
                        cmd.Parameters.AddWithValue("@Step2User_TargetDate", Step2User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step3UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step3User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step4UserUID", Step4UserUID);
                        cmd.Parameters.AddWithValue("@Step4User_TargetDate", Step4User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step5UserUID", Step5UserUID);
                        cmd.Parameters.AddWithValue("@Step5User_TargetDate", Step5User_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep2_DisplayName", FlowStep2_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep3_DisplayName", FlowStep3_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep4_DisplayName", FlowStep4_DisplayName);
                        cmd.Parameters.AddWithValue("@FlowStep5_DisplayName", FlowStep4_DisplayName);
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        cmd.Parameters.AddWithValue("@ActualDocument_RelativePath", ActualDocument_RelativePath);
                        cmd.Parameters.AddWithValue("@ActualDocument_DirectoryName", ActualDocument_DirectoryName);
                        cmd.Parameters.AddWithValue("@UploadFilePhysicalpath", UploadFilePhysicalpath);
                        if (CoverLetterUID != "")
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@SubmissionType", SubmissionType);
                        //
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@DocVersionUID", DocVersionUID);
                        //
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int DocumentCoverLetter_Insert_or_Update(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
           string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
           string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
           string FileRef_Number, string ActualDocument_CurrentStatus, string ActualDocument_Originator, DateTime Document_Date)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DocumentCoverLetter_Insert_or_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }



        public int Document_Update(Guid ActualDocumentUID, string ProjectRef_Number,string Ref_Number,DateTime IncomingRec_Date, string ActualDocument_Name,string Description,
            string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string Remarks,string FileRef_Number,
            string ActualDocument_Originator)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Document_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        //cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public DataSet ActualDocuments_SelectBy_DocumentUID(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_SelectBy_DocumentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                //Added code by Venkat on 13 May 2022
                cmd.SelectCommand.CommandTimeout = 900;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet ActualDocuments_SelectBy_DirectoryName(Guid DocumentUID,string ActualDocument_DirectoryName)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_SelectBy_DirectoryName", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActualDocument_DirectoryName", ActualDocument_DirectoryName);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet ActualDocuments_SelectBy_DocID_FileName(Guid DocumentUID, string ActualDocument_Name)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_SelectBy_DocID_FileName", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string ActualDocumentName_By_ActualDocumentUID(Guid ActualDocumentUID)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("ActualDocumentName_By_ActualDocumentUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public string ActualDocumentUID_By_ActualDocumentName(string ActualDocument_Name,Guid SubmittalUID)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("ActualDocumentUID_By_ActualDocumentName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                cmd.Parameters.AddWithValue("@DocumentUID", SubmittalUID);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public DataSet ActualDocuments_Not_In_WordDocRead(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_Not_In_WordDocRead", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int Document_Exists_DocumentUID_Name(Guid DocumentUID,string ActualDocument_Name)
        {
            int retval = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("Document_Exists_DocumentUID_Name", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                retval = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = 0;
            }
            return retval;
        }
        public int WordDocRead_InsertorUpdate(Guid UID, string Doc_path, Guid DocumemtUID, string Encrypted)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("WordDocRead_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@Doc_path", Doc_path);
                        cmd.Parameters.AddWithValue("@DocumemtUID", DocumemtUID);
                        cmd.Parameters.AddWithValue("@Encrypted", Encrypted);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet Submitted_ActualDocuments_SelectBy_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Submitted_ActualDocuments_SelectBy_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Submitted_ActualDocuments_SelectBy_WorkPackageUID_NotDelayed(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Submitted_ActualDocuments_SelectBy_WorkPackageUID_NotDelayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Submitted_ActualDocuments_SelectBy_WorkPackageUID_Delayed(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Submitted_ActualDocuments_SelectBy_WorkPackageUID_Delayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int GetDelayed_Actual_Documents(Guid ActualDocumentUID, string CurrentStatus)
        {
            int retcnt = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetDelayed_Actual_Documents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                cmd.Parameters.AddWithValue("@CurrentStatus", CurrentStatus);
                retcnt = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retcnt = 0;
            }
            return retcnt;
        }

        public DataSet Reviewed_ActualDocuments_SelectBy_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Reviewed_ActualDocuments_SelectBy_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Reviewed_ActualDocuments_SelectBy_WorkPackageUID_NotDelayed(Guid WorkPackageUID,string Status)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Reviewed_ActualDocuments_SelectBy_WorkPackageUID_NotDelayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Status", Status);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Reviewed_ActualDocuments_SelectBy_WorkPackageUID_Delayed(Guid WorkPackageUID, string Status)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Reviewed_ActualDocuments_SelectBy_WorkPackageUID_Delayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Status", Status);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Approved_ActualDocuments_SelectBy_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Approved_ActualDocuments_SelectBy_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Approved_ActualDocuments_SelectBy_WorkPackageUID_NotDelayed(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Approved_ActualDocuments_SelectBy_WorkPackageUID_NotDelayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Approved_ActualDocuments_SelectBy_WorkPackageUID_Delayed(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Approved_ActualDocuments_SelectBy_WorkPackageUID_Delayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public DataSet ClientApproved_ActualDocuments_SelectBy_WorkPackageUID_NotDelayed(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ClientApproved_ActualDocuments_SelectBy_WorkPackageUID_NotDelayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet ClientApproved_ActualDocuments_SelectBy_WorkPackageUID_Delayed(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ClientApproved_ActualDocuments_SelectBy_WorkPackageUID_Delayed", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public string GetSubmittalUID_By_ActualDocumentUID(Guid DocumentUID)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetSubmittalUID_by_DocumentUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID",  DocumentUID);
                retval = cmd.ExecuteScalar().ToString();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public DataSet GetActualProjectCommunicationDocuments(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetProjectCommunicationDocuments", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet ActualDocuments_SelectBy_ActualDocumentUID(Guid ActualDocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_SelectBy_ActualDocumentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet ActualDocuments_SelectBy_WorkpackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_SelectBy_WorkpackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDocumentsForProject_by_UserUID(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentsForProject_by_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int Documents_Delete_by_DocID(Guid DocumentUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Documents_Delete_by_DocID"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DateTime GetDocumentReviewedDateByID(Guid ActualDocumentUID,string CurrentStatus)
        {
            DateTime sresult = DateTime.MinValue;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetDocumentReviewedDateByID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                cmd.Parameters.AddWithValue("@CurrentStatus", CurrentStatus);
                sresult = (DateTime)cmd.ExecuteScalar();
                con.Close();
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = DateTime.MinValue;
            }
        }

        public int GetDocumentReviewedinDays(Guid DocumentUID, string CurrentStatus)
        {
            int retcnt = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetDocumentReviewedinDays", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.Parameters.AddWithValue("@CurrentStatus", CurrentStatus);
                retcnt = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retcnt = 0;
            }
            return retcnt;
        }

        public int ActualDocuments_Delete_by_DocID(Guid ActualDocumentUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ActualDocuments_Delete_by_DocID"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int Tasks_Delete_by_TaskUID(Guid TaskUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Tasks_Delete_by_TaskUID"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

       


        public DataSet GetDocument_For_Master()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocument_For_Master", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetActualDocument_Type_Master()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetActualDocument_Type_Master", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int WorkPackageCategory_Insert_or_Update(Guid WorkPackageCategory_UID,Guid WorkPackageUID,string WorkPackageCategory_Name)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("WorkPackage_Category_Insert_or_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@WorkPackageCategory_UID", WorkPackageCategory_UID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@WorkPackageCategory_Name", WorkPackageCategory_Name);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet WorkPackageCategory_Selectby_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("WorkPackage_Category_Selectby_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        
        public string GetWorkPackageCategoryUID_By_Name(Guid WorkPackageUID, string WorkPackageCategory_Name)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetWorkpackageUID_By_Name", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Parameters.AddWithValue("@WorkPackageCategory_Name", WorkPackageCategory_Name);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public string GetWorkpackageCategory_By_UID(Guid WorkPackageCategory_UID)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("GetWorkpackageCategory_By_UID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkPackageCategory_UID", WorkPackageCategory_UID);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public DataSet WorkPackageCategory_Selectby_CategoryID(Guid WorkPackageCategory_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("WorkPackage_Category_Selectby_CategoryID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageCategory_UID", WorkPackageCategory_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int Alerts_Config_InsertorUpdate(Guid AlertsConfig_ID, Guid User_Role, string Alert_For)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Alerts_Config_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AlertsConfig_ID", AlertsConfig_ID);
                        cmd.Parameters.AddWithValue("@User_Role", User_Role);
                        cmd.Parameters.AddWithValue("@Alert_For", Alert_For);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet Alerts_Config_Select()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Alert_Config_Select", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int MasterWorkpackages_InsertorUpdate(Guid MasterWorkPackageUID, Guid ProjectUID, string MasterWorkPackageName,string MasterWorkPackageCode)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("MasterWorkpackages_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MasterWorkPackageUID", MasterWorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@MasterWorkPackageName", MasterWorkPackageName);
                        cmd.Parameters.AddWithValue("@MasterWorkPackageCode", MasterWorkPackageCode);                        
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet MasterWorkpackage_selectBy_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterWorkpackage_selectBy_ProjectUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterWorkpackage_selectBy_MasterWorkPackageUID(Guid MasterWorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterWorkpackage_selectBy_MasterWorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@MasterWorkPackageUID", MasterWorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterWorkpackage_select_All()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterWorkPackages_select_all", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterWorkpackage_SelectBy_UserUID(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterWorkpackage_SelectBy_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int MasterWorkpackage_Delete(Guid MasterWorkPackageUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("MasterWorkpackage_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@MasterWorkPackageUID", MasterWorkPackageUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public int MasterLocation_InsertorUpdate(Guid LocationMasterUID, Guid ProjectUID, string LocationMaster_Name, string LocationMaster_Code)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("MasterLocation_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@LocationMasterUID", LocationMasterUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@LocationMaster_Name", LocationMaster_Name);
                        cmd.Parameters.AddWithValue("@LocationMaster_Code", LocationMaster_Code);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet MasterLocation_selectBy_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterLocation_SelectBy_ProjectID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterLocation_selectBy_LocationMasterUID(Guid LocationMasterUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterLocation_SelectBy_LocationMasterUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@LocationMasterUID", LocationMasterUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterLocation_Select_All()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterLocation_Select_All", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterLocation_SelectBy_UserUID(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterLocation_SelectBy_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int MasterLocation_Delete(Guid LocationMasterUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("MasterLocation_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@LocationMasterUID", LocationMasterUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int MasterClient_InsertorUpdate(Guid ClientMasterUID, Guid ProjectUID, string ClientMaster_Name, string ClientMaster_Code)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("MasterClient_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ClientMasterUID", ClientMasterUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@ClientMaster_Name", ClientMaster_Name);
                        cmd.Parameters.AddWithValue("@ClientMaster_Code", ClientMaster_Code);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet MasterClient_selectBy_ProjectUID(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterClient_SelectBy_ProjectID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterClient_selectBy_ClientMasterUID(Guid ClientMasterUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterClient_selectBy_ClientMasterUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ClientMasterUID", ClientMasterUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterClient_Select_All()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterClient_Select_All", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet MasterClient_SelectBy_UserUID(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("MasterClient_SelectBy_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int MasterClient_Delete(Guid ClientMasterUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("MasterClient_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ClientMasterUID", ClientMasterUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int ProjectClass_InsertorUpdate(Guid ProjectClass_UID, string ProjectClass_Name, string ProjectClass_Description)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ProjectClass_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProjectClass_UID", ProjectClass_UID);
                        cmd.Parameters.AddWithValue("@ProjectClass_Name", ProjectClass_Name);
                        cmd.Parameters.AddWithValue("@ProjectClass_Description", ProjectClass_Description);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet ProjectClass_Select_All()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ProjectClass_Select_All", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet ProjectClass_SelectBy_pClassUID(Guid ProjectClass_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ProjectClass_Select_By_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectClass_UID", ProjectClass_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet ProjectClass_Select_By_UserUID(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ProjectClass_Select_By_UserUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int ProjectClass_Delete(Guid ProjectClass_UID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ProjectClass_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProjectClass_UID", ProjectClass_UID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int DocumentHistroy_InsertorUpdate(Guid DocumentHistroryUID, Guid DocumentUID,
            Guid UserUID, string Action_Type)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DocumentHistroy_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentHistroryUID", DocumentHistroryUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@Action_Type", Action_Type);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet GetDocumentHistoryBy_WorkpackgeUID(Guid WorkpackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentHistoryBy_WorkpackgeUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDoucmentHistory_by_DoucmentUID(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDoucmentHistory_by_DoucmentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDoucmentHistory_For_Uesr_by_DoucmentUID(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDoucmentHistory_For_Uesr_by_DoucmentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetDocumentActionHistory_For_Uesr_by_DoucmentUID(Guid DocumentUID,Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetDocumentActionHistory_For_Uesr_by_DoucmentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public void DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                string password = @"myKey123"; // Your Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();
                //RMCrypto.Padding = PaddingMode.None;
                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                {
                    int data;
                    while ((data = cs.ReadByte()) != -1)
                        fsOut.WriteByte((byte)data);
                }

                    

                //fsOut.Close();
                //fsOut.Dispose();
                cs.Close();
                fsCrypt.Close();
                fsCrypt.Dispose();

            }
            catch (Exception ex)
            {
                string exmsg = ex.Message;
            }
        }

        public void EncryptFile(string inputFile, string outputFile)
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


        public DataSet GetWordDocumentFilePath_DocumentUID(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetWordDocumentFilePath_DocumentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int Workpackageoption_InsertorUpdate(Guid Workpackage_OptionUID, string Workpackage_OptionName)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Workpackageoption_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                        cmd.Parameters.AddWithValue("@Workpackage_OptionName", Workpackage_OptionName);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet Workpackageoption_Select()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Workpackageoption_select", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;                
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Workpackageoption_SelectBy_UID(Guid Workpackage_OptionUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Workpackageoption_SelectBy_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Workpackageoption_SelectBy_OptionFor(string Workpackage_OptionFor)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Workpackageoption_SelectBy_OptionFor", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Workpackage_OptionFor", Workpackage_OptionFor);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int WorkpackageSelectedOptions_Insert(Guid WorkpackageSelectedOption_UID, Guid WorkPackageUID,Guid Workpackage_OptionUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("WorkpackageSelectedOptions_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@WorkpackageSelectedOption_UID", WorkpackageSelectedOption_UID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public string WorkpackageoptionName_SelectBy_UID(Guid Workpackage_OptionUID)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("WorkpackageoptionName_SelectBy_UID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public int WorkpackageActivityMasterMainActivity_Insert(Guid WorkpakageActivity_MasterUID, Guid Workpackage_OptionUID,string WorkpakageActivity_Name,int WorkpakageActivity_Order)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("WorkpackageActivityMasterMainActivity_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@WorkpakageActivity_MasterUID", WorkpakageActivity_MasterUID);
                        cmd.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                        cmd.Parameters.AddWithValue("@WorkpakageActivity_Name", WorkpakageActivity_Name);
                        cmd.Parameters.AddWithValue("@WorkpakageActivity_Order", WorkpakageActivity_Order);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int WorkpackageActivityMasterSubActivity_Insert(Guid WorkpakageActivity_MasterUID, Guid Workpackage_OptionUID, string WorkpakageActivity_Name, Guid WorkpakageActivity_ParentUID,int WorkpakageActivity_Order,int WorkpakageActivity_Level)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("WorkpackageActivityMasterSubActivity_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@WorkpakageActivity_MasterUID", WorkpakageActivity_MasterUID);
                        cmd.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                        cmd.Parameters.AddWithValue("@WorkpakageActivity_Name", WorkpakageActivity_Name);
                        cmd.Parameters.AddWithValue("@WorkpakageActivity_ParentUID", WorkpakageActivity_ParentUID);
                        cmd.Parameters.AddWithValue("@WorkpakageActivity_Order", WorkpakageActivity_Order);
                        cmd.Parameters.AddWithValue("@WorkpakageActivity_Level", WorkpakageActivity_Level);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public string WorkpackageActivityMaster_SelectBy_Name(Guid Workpackage_OptionUID, string WorkpakageActivity_Name)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("WorkpackageActivityMaster_SelectBy_Name", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                cmd.Parameters.AddWithValue("@WorkpakageActivity_Name", WorkpakageActivity_Name);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public string WorkpackageActivityMasterParent_SelectBy_Name(Guid Workpackage_OptionUID, Guid WorkpakageActivity_ParentUID, string WorkpakageActivity_Name)
        {
            string retval = string.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("WorkpackageActivityMasterParent_SelectBy_Name", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                cmd.Parameters.AddWithValue("@WorkpakageActivity_ParentUID", WorkpakageActivity_ParentUID);
                cmd.Parameters.AddWithValue("@WorkpakageActivity_Name", WorkpakageActivity_Name);
                retval = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = "";
            }
            return retval;
        }

        public DataSet WorkpackageActivityMaster_SelectBy_OptionUID(Guid Workpackage_OptionUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("WorkpackageActivityMaster_SelectBy_OptionUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet WorkpackageMainActivityMaster_SelectBy_OptionUID(Guid Workpackage_OptionUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("WorkpackageMainActivityMaster_SelectBy_OptionUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Workpackage_OptionUID", Workpackage_OptionUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet WorkpackageActivityMaster_SelectBy_ParentUID(Guid WorkpakageActivity_ParentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("WorkpackageActivityMaster_SelectBy_ParentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkpakageActivity_ParentUID", WorkpakageActivity_ParentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int Camera_InsertorUpdate(Guid Camera_UID, Guid ProjectUID, Guid WorkpackageUID, string Camera_Name, string Camera_IPAddress, string Camera_Description)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Camera_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Camera_UID", Camera_UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@Camera_Name", Camera_Name);
                        cmd.Parameters.AddWithValue("@Camera_IPAddress", Camera_IPAddress);
                        cmd.Parameters.AddWithValue("@Camera_Description", Camera_Description);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet Camera_Selectby_WorkpackageUID(Guid WorkpackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Camera_Selectby_WorkpackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet Camera_Selectby_Camera_UID(Guid Camera_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Camera_Selectby_Camera_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Camera_UID", Camera_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int Camera_Delete(Guid Camera_UID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Camera_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Camera_UID", Camera_UID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet WorkpackageSelectedOptions_by_WorkPackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("WorkpackageSelectedOptions_by_WorkPackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetSelectedOption_By_WorkpackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetSelectedOption_By_WorkpackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet WorkpackageSelectedOptions_by_UID(Guid SelectedOptionUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Workpackage_SelectOption_by_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@SelectedOptionUID", SelectedOptionUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int WorkpackageSelectedOption_Enabled(Guid SelectedOptionUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("WorkpackageSelectedOption_Enabled"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@SelectedOptionUID", SelectedOptionUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public DataSet Critical_Task_Selectby_WorkpackageUID(Guid WorkpackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("Critical_Task_Selectby_WorkpackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int Is_Task_Critical(Guid TaskUID)
        {
            int retval = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("Is_Task_Critical", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                retval = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                retval = 0;
            }
            return retval;
        }
        //Venkat 

        public DataTable GetHelpCategories()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("sp_GetHelpCategories", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(dt);
            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;
        }

        //public DataSet CheckLogin(String sUsername, String sPassword)
        //{
        //    DataSet ds = new DataSet();
        //    try
        //    {
        //        SqlConnection con = new SqlConnection(db.GetConnectionString());
        //        SqlDataAdapter cmd = new SqlDataAdapter("usp_CheckLogin", con);
        //        cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
        //        cmd.SelectCommand.Parameters.AddWithValue("@Username", sUsername);
        //        cmd.SelectCommand.Parameters.AddWithValue("@Password", sPassword);
        //        cmd.Fill(ds);
        //    }
        //    catch (Exception ex)
        //    {
        //        ds = null;
        //    }
        //    return ds;
        //}

        public void UsersLogOutStatus(string Userid, string SessionID)
        {
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("Sp_LogoutUsersLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", Userid);
                cmd.Parameters.AddWithValue("@sessionId", SessionID);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void UsersLogInStatus(string Userid, string SessionID, string Status)
        {
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("Sp_LoggedInUsersLog", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", Userid);
                cmd.Parameters.AddWithValue("@sessionId", SessionID);
                cmd.Parameters.AddWithValue("@status", Status);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {

            }
        }

        // added on 22/10/2020 zuber
        public DataSet ActualDocuments_SelectBy_DirectoryName_New(Guid DocumentUID, string ParentDirectoryName)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_SelectBy_DirectoryName_New", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ParentDirectoryName", ParentDirectoryName);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public void StoreEmaildataToMailQueue(Guid MailUID, Guid UserUID, string FromEmailID, string ToEmailID, string Subject, string Body, string CCTo, string Attachment)
        {
            try
            {
                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlCommand SqlCmd = new SqlCommand("ups_Insert_Mails", SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@MailUID", MailUID);
                    SqlCmd.Parameters.AddWithValue("@UserUID", UserUID);
                    SqlCmd.Parameters.AddWithValue("@FromEmailID", FromEmailID);
                    SqlCmd.Parameters.AddWithValue("@ToEmailID", ToEmailID);
                    SqlCmd.Parameters.AddWithValue("@Subject", Subject);
                    SqlCmd.Parameters.AddWithValue("@Body", Body);
                    SqlCmd.Parameters.AddWithValue("@CCTo", CCTo);
                    SqlCmd.Parameters.AddWithValue("@MailSentDate", DateTime.Now);
                    SqlCmd.Parameters.AddWithValue("@Attachment", Attachment);
                    SqlCmd.Parameters.AddWithValue("@MailSent", "N");
                    SqlConn.Open();
                    SqlCmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        // added on 04/11/2020
        public DataSet GetAllUserTypeFunctionality_Master()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetUserTypeFunctionalityMaster", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        // added on 04/11/2020
        public void InsertIntoUsertypeFunctionality_Mapping(string UerType, Guid FunctionalityUID)
        {
            try
            {
                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlCommand SqlCmd = new SqlCommand("usp_InsertUserTypeFunctionality_Mapping", SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@UID", Guid.NewGuid());
                    SqlCmd.Parameters.AddWithValue("@UserType", UerType);
                    SqlCmd.Parameters.AddWithValue("@FunctionalityUID", FunctionalityUID);
                    SqlConn.Open();
                    SqlCmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
            }
            catch (Exception ex)
            {

            }

        }

        // added on 04/11/2020
        public void DeleteUsertypeFunctionality_Mapping(string UserType)
        {
            try
            {

                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlCommand SqlCmd = new SqlCommand("Delete From UserType_Functionality_Mapping Where UserType='" + UserType + "'", SqlConn);
                    SqlConn.Open();
                    SqlCmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        // added on 04/11/2020
        public DataSet GetUsertypeFunctionality_Mapping(string UserType)
        {
            DataSet ds = new DataSet();
            try
            {

                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlConnection con = new SqlConnection(db.GetConnectionString());
                    SqlDataAdapter cmd = new SqlDataAdapter("usp_GetUserTypeFunctionality_Mapping", con);
                    cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    cmd.SelectCommand.Parameters.AddWithValue("@UserType", UserType);
                    cmd.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        // added on 11/11/2020
        public int CheckUserName_Exists(string Username)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_CheckUsernameExists"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Username", Username);
                        con.Open();
                        sresult = (int)cmd.ExecuteScalar();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        // added on 12/11/2020
        public int UpdatePassword(Guid UserUID, string password)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_UpdatePassword"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@Password", password);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        // added on 23/11/2020
        public void InsertIntoDocumentUploadLog(Guid ActualDocumentUID, DateTime StartDate, DateTime EndDate, Guid UserUID, float Duration)
        {
            try
            {
                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlCommand SqlCmd = new SqlCommand("usp_InsertDocumentUploadLog", SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                    SqlCmd.Parameters.AddWithValue("@UploadStartDate", StartDate);
                    SqlCmd.Parameters.AddWithValue("@UploadEndDate", EndDate);
                    SqlCmd.Parameters.AddWithValue("@UploadUserUID", UserUID);
                    SqlCmd.Parameters.AddWithValue("@Duration", Duration);
                    SqlConn.Open();
                    SqlCmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
            }
            catch (Exception ex)
            {

            }

        }

        internal System.Data.DataSet GetProjects_by_ClassUID(Guid ProjectClass_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetProjects_by_ClassUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectClass_UID", ProjectClass_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        internal System.Data.DataSet GetUserProjects_by_ClassUID(Guid ProjectClass_UID, Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetProject_by_ClassUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectClass_UID", ProjectClass_UID);
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //venkat

        internal string GetProjectCategoryUID_ProjectCategoryName(string ProjectCategoryName)
        {
            string projectCategoryUID = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlDataAdapter cmd = new SqlDataAdapter("ups_GetProjectCategoryID_ProjectCategoryName", con))
                    {

                        cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                        cmd.SelectCommand.Parameters.AddWithValue("@projectCategoryName", ProjectCategoryName);
                        DataTable dtProjects = new DataTable();
                        cmd.Fill(dtProjects);
                        if (dtProjects.Rows.Count > 0) projectCategoryUID = dtProjects.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return projectCategoryUID;
        }

        internal string GetProjectUID_ProjectName_Project_CategoryUID(Guid Project_CategoryUID, string ProjectName)
        {
            string projectUID = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlDataAdapter cmd = new SqlDataAdapter("ups_GetProjectCategoryID_ProjectCategoryName", con))
                    {
                        cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                        cmd.SelectCommand.Parameters.AddWithValue("@projectName", ProjectName);
                        cmd.SelectCommand.Parameters.AddWithValue("@projectCategoryUID", Project_CategoryUID);
                        DataTable dtProjects = new DataTable();
                        cmd.Fill(dtProjects);
                        if (dtProjects.Rows.Count > 0) projectUID = dtProjects.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return projectUID;
        }

        internal string GetWorkPackageID_ProjectId_WorkPackageName(Guid ProjectId, string WorkPackageName)
        {
            string workPackageUID = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlDataAdapter cmd = new SqlDataAdapter("ups_GetWorkPackageID_ProjectId_WorkPackageName", con))
                    {
                        cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                        cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectId);
                        cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageName", WorkPackageName);
                        DataTable dtProjects = new DataTable();
                        cmd.Fill(dtProjects);
                        if (dtProjects.Rows.Count > 0) workPackageUID = dtProjects.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return workPackageUID;
        }

        internal string GetTaskUID_workPackageUId_TaskName(Guid workPackageUId, string TaskName)
        {
            string TaskUID = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlDataAdapter cmd = new SqlDataAdapter("ups_GetTaskUID_workPackageUId_TaskName", con))
                    {
                        cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                        cmd.SelectCommand.Parameters.AddWithValue("@workPackageUID", workPackageUId);
                        cmd.SelectCommand.Parameters.AddWithValue("@TaskName", TaskName);
                        DataTable dtProjects = new DataTable();
                        cmd.Fill(dtProjects);
                        if (dtProjects.Rows.Count > 0) TaskUID = dtProjects.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return TaskUID;
        }

        internal string GetfinancialMileStoneID_workPackageUId_TaskName(Guid TaskUid, string FinMileStoneName)
        {
            string TaskUID = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlDataAdapter cmd = new SqlDataAdapter("ups_GetfinancialMileStoneID_workPackageUId_TaskName", con))
                    {
                        cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                        cmd.SelectCommand.Parameters.AddWithValue("@TaskUid", TaskUid);
                        cmd.SelectCommand.Parameters.AddWithValue("@FinancialMileStoneName", FinMileStoneName);
                        DataTable dtProjects = new DataTable();
                        cmd.Fill(dtProjects);
                        if (dtProjects.Rows.Count > 0) TaskUID = dtProjects.Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return TaskUID;
        }

        internal int UpdateTaskStatus(string WorkPackageUID, string TaskUID, string Status, DateTime CDate1, double StatusPer, string Comment)
        {
            int p = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_UpdateTaskStatus"))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@CDate1", CDate1);
                        cmd.Parameters.AddWithValue("@StatusPer", StatusPer);
                        cmd.Parameters.AddWithValue("@Comments", Comment);
                        con.Open();
                        p = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return p;
        }

        // from arun for web api db syncing on 31/08/2021
        public string ProjectNameExists(string ProjectName)
        {
            string pUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetProjectUIDFromName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjectName", ProjectName);
                pUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                pUID = "";
            }
            return pUID;
        }

        public string WorkpackageNameExists(string WorkpackageName, string ProjectUID)
        {
            string wUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetWorkpackageUIDFromProjectUID_wName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkpackageName", WorkpackageName);
                cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                wUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                wUID = "";
            }
            return wUID;
        }

        public string TaskNameExists(Guid ActivityUID, string TaskName, string From)
        {
            string tUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetTaskUIDFromParent_tName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActivityUID", ActivityUID);
                cmd.Parameters.AddWithValue("@TaskName", TaskName);
                cmd.Parameters.AddWithValue("@From", From);
                tUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                tUID = "";
            }
            return tUID;
        }

        //public string GetUserUIDfrom_UserEmail(string EmailID)
        //{
        //    string tUID = "";
        //    try
        //    {
        //        SqlConnection con = new SqlConnection(db.GetConnectionString());
        //        if (con.State == ConnectionState.Closed) con.Open();
        //        SqlCommand cmd = new SqlCommand("usp_GetUserUIDfrom_UserEmail", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@EmailID", EmailID);
        //        tUID = (string)cmd.ExecuteScalar();
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        tUID = "";
        //    }
        //    return tUID;
        //}

        public int InsertorUpdateTaskMeasurementBook(Guid UID, Guid TaskUID, string UnitforProgress, string Quantity, string Description, DateTime CreatedDate, string Upload_File, Guid CreatedByUID, string Remarks, string ServerCopiedAdd)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdateMeasurementBook"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@Upload_File", Upload_File);
                        cmd.Parameters.AddWithValue("@CreatedByUID", CreatedByUID);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        sresult = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                sresult = 0;
            }
            return sresult;
        }

        public string RsourceNameExists(string ResourceName, string WorkpackageUID)
        {
            string wUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetResourecUID_by_ResourceName_WorkpackageUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ResourceName", ResourceName);
                cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                wUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                wUID = "";
            }
            return wUID;
        }

        public int InsertorUpdateResourceDeploymentPlanned(Guid ReourceDeploymentUID, Guid WorkpackageUID, Guid ResourceUID, DateTime StartDate,
                   DateTime EndDate, string DeploymentType, float Planned, DateTime PlannedDate, string ServerCopiedAdd)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdateResourceDeploymentPlan"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@ReourceDeploymentUID", ReourceDeploymentUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate);
                        cmd.Parameters.AddWithValue("@DeploymentType", DeploymentType);
                        cmd.Parameters.AddWithValue("@Planned", Planned);
                        cmd.Parameters.AddWithValue("@PlannedDate", PlannedDate);
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        //public int ResourceDeployment_Update(Guid WorkpackageUID, Guid ResourceUID, float Deployed, DateTime DeployedDate, string Remarks, string ServerCopiedUpdate)
        //{
        //    int sresult = 0;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
        //        {

        //            using (SqlCommand cmd = new SqlCommand("dbsync_ResourceDeployment_Update"))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                con.Open();
        //                cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
        //                cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
        //                cmd.Parameters.AddWithValue("@Deployed", Deployed);
        //                cmd.Parameters.AddWithValue("@DeployedDate", DeployedDate);
        //                cmd.Parameters.AddWithValue("@Remarks", Remarks);
        //                cmd.Parameters.AddWithValue("@ServerCopiedUpdate", ServerCopiedUpdate);
        //                sresult = (int)cmd.ExecuteNonQuery();
        //                con.Close();

        //            }
        //        }
        //        return sresult;
        //    }
        //    catch (Exception ex)
        //    {
        //        return sresult = 0;
        //    }
        //}


        public DataSet ResourceDeployment_by_ReourceDeploymentUID(Guid ReourceDeploymentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ReourceDeploymentUID", ReourceDeploymentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int WebAPIStatusInsert(Guid WebAPIUID, string WebAPIURL, string WebAPIParameters, string WebAPI_Error, string WebAPIStatus)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertWebAPIStatus"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@WebAPIUID", WebAPIUID);
                        cmd.Parameters.AddWithValue("@WebAPIURL", WebAPIURL);
                        cmd.Parameters.AddWithValue("@WebAPIParameters", WebAPIParameters);
                        cmd.Parameters.AddWithValue("@WebAPI_Error", WebAPI_Error);
                        cmd.Parameters.AddWithValue("@WebAPIStatus", WebAPIStatus);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public string BOQNameExists(Guid ActivityUID, string BOQName, string From)
        {
            string tUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetBOQUID_From_Name", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ActivityUID", ActivityUID);
                cmd.Parameters.AddWithValue("@BOQName", BOQName);
                cmd.Parameters.AddWithValue("@From", From);
                tUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                tUID = "";
            }
            return tUID;
        }

        internal int InsertBOQDetails(string itemNo, string description, string quantity, string unit,
string inrRate, string jpyRate, string usdRate, string inrAmount, string jpyAmount, string usdAmount, string parentId, Guid projectuid, string ServerCopiedAdd)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand(""))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@uid", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@itemNo", itemNo);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@unit", unit);
                        cmd.Parameters.AddWithValue("@inrRate", inrRate);
                        cmd.Parameters.AddWithValue("@jpyRate", jpyRate);
                        cmd.Parameters.AddWithValue("@usdRate", usdRate);
                        cmd.Parameters.AddWithValue("@inrAmount", inrAmount);
                        cmd.Parameters.AddWithValue("@jpyAmount", jpyAmount);
                        cmd.Parameters.AddWithValue("@usdAmount", usdAmount);
                        cmd.Parameters.AddWithValue("@parentId", parentId);
                        cmd.Parameters.AddWithValue("@projectuid", projectuid);
                        con.Open();
                        cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
                        con.Close();

                    }
                }

            }
            catch (Exception ex)
            {

            }
            return cnt;
        }

        internal int UpdateBOQDetails(string itemNo, string description, string quantity, string unit, string inrRate, string inrAmount, Guid uid)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_UpdateBOQDetails"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@uid", uid);
                        cmd.Parameters.AddWithValue("@itemNo", itemNo);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@unit", unit);
                        cmd.Parameters.AddWithValue("@inrRate", inrRate);
                        cmd.Parameters.AddWithValue("@inrAmount", inrAmount);
                        con.Open();
                        cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
                        con.Close();
                        //sresult = true;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return cnt;
        }

    //    internal int InsertjointInspection(Guid inspectionUid, Guid BOQUid, string DiaPipe, string unit, string invoice_number, string invoicedate, string quantity, string Inspection_Type,
    //string Chainage_Number, string Chainage_Desc, float Chainage_StartPoint, float Chainage_Length, float Qty_in_RMT, double Qty_for_Unit, double Deduction, string Remarks, Guid ProjectUID, string PipeNumber, string ServerCopiedAdd)
    //    {
    //        int cnt = 0;
    //        try
    //        {

    //            using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
    //            {

    //                using (SqlCommand cmd = new SqlCommand(""))
    //                {
    //                    cmd.CommandType = CommandType.StoredProcedure;
    //                    cmd.Connection = con;
    //                    cmd.Parameters.AddWithValue("@inspectionUid", inspectionUid);
    //                    cmd.Parameters.AddWithValue("@BOQUid", BOQUid);
    //                    cmd.Parameters.AddWithValue("@DiaPipe", DiaPipe);
    //                    cmd.Parameters.AddWithValue("@unit", unit);
    //                    cmd.Parameters.AddWithValue("@invoice_number", invoice_number);
    //                    cmd.Parameters.AddWithValue("@invoicedate", invoicedate);
    //                    cmd.Parameters.AddWithValue("@quantity", quantity);
    //                    cmd.Parameters.AddWithValue("@Inspection_Type", Inspection_Type);
    //                    cmd.Parameters.AddWithValue("@Chainage_Number", Chainage_Number);
    //                    cmd.Parameters.AddWithValue("@Chainage_Desc", Chainage_Desc);
    //                    cmd.Parameters.AddWithValue("@Chainage_StartPoint", Chainage_StartPoint);
    //                    cmd.Parameters.AddWithValue("@Chainage_Length", Chainage_Length);
    //                    cmd.Parameters.AddWithValue("@Qty_in_RMT", Qty_in_RMT);
    //                    cmd.Parameters.AddWithValue("@Qty_for_Unit", Qty_for_Unit);
    //                    cmd.Parameters.AddWithValue("@Deduction", Deduction);
    //                    cmd.Parameters.AddWithValue("@Remarks", Remarks);
    //                    cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
    //                    cmd.Parameters.AddWithValue("@PipeNumber", PipeNumber);
    //                    cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
    //                    con.Open();
    //                    cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
    //                    con.Close();

    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //        return cnt;
    //    }

        internal string GetBOQDiaofPipe_by_BOQDetailsUID(Guid BOQDetailsUID)
        {
            string DeductionsDescription = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_GetBOQDiaofPipe_by_BOQDetailsUID"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@BOQDetailsUID", BOQDetailsUID);
                        con.Open();
                        DeductionsDescription = (string)cmd.ExecuteScalar();
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                DeductionsDescription = "Error1:";
            }
            return DeductionsDescription;
        }

        // Arun changes on 01/09/2021

        public string GetProjectName_by_BOQUID(Guid BOQDetailsUID)
        {
            string pUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetProjectName_by_BOQUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BOQDetailsUID", BOQDetailsUID);
                pUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                pUID = "";
            }
            return pUID;
        }

        public string GetUserUIDfrom_UserEmail(string EmailID)
        {
            string tUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetUserUIDfrom_UserEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmailID", EmailID);
                tUID = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                tUID = "";
            }
            return tUID;
        }

        //internal int InsertjointInspection(Guid inspectionUid, Guid BOQUid, string DiaPipe, string unit, string invoice_number, string invoicedate, string quantity, string Inspection_Type,
        //    string Chainage_Number, string Chainage_Desc, float Chainage_StartPoint, float Chainage_Length, float Qty_in_RMT, double Qty_for_Unit, double Deduction, string Remarks, Guid ProjectUID, string PipeNumber, string ServerCopiedAdd)
        //{
        //    int cnt = 0;
        //    try
        //    {

        //        using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
        //        {

        //            using (SqlCommand cmd = new SqlCommand("dbsync_JointInspectionAddorUpdate"))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                cmd.Parameters.AddWithValue("@inspectionUid", inspectionUid);
        //                cmd.Parameters.AddWithValue("@BOQUid", BOQUid);
        //                cmd.Parameters.AddWithValue("@DiaPipe", DiaPipe);
        //                cmd.Parameters.AddWithValue("@unit", unit);
        //                cmd.Parameters.AddWithValue("@invoice_number", invoice_number);
        //                cmd.Parameters.AddWithValue("@invoicedate", invoicedate);
        //                cmd.Parameters.AddWithValue("@quantity", quantity);
        //                cmd.Parameters.AddWithValue("@Inspection_Type", Inspection_Type);
        //                cmd.Parameters.AddWithValue("@Chainage_Number", Chainage_Number);
        //                cmd.Parameters.AddWithValue("@Chainage_Desc", Chainage_Desc);
        //                cmd.Parameters.AddWithValue("@Chainage_StartPoint", Chainage_StartPoint);
        //                cmd.Parameters.AddWithValue("@Chainage_Length", Chainage_Length);
        //                cmd.Parameters.AddWithValue("@Qty_in_RMT", Qty_in_RMT);
        //                cmd.Parameters.AddWithValue("@Qty_for_Unit", Qty_for_Unit);
        //                cmd.Parameters.AddWithValue("@Deduction", Deduction);
        //                cmd.Parameters.AddWithValue("@Remarks", Remarks);
        //                cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
        //                cmd.Parameters.AddWithValue("@PipeNumber", PipeNumber);
        //                cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
        //                con.Open();
        //                cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
        //                con.Close();

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return cnt;
        //}


        //public int ResourceDeployment_Update(Guid WorkpackageUID, Guid ResourceUID, float Deployed, DateTime DeployedDate, string Remarks, string ServerCopiedAdd)
        //{
        //    int sresult = 0;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
        //        {

        //            using (SqlCommand cmd = new SqlCommand("dbsync_ResourceDeployment_Update"))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                con.Open();
        //                cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
        //                cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
        //                cmd.Parameters.AddWithValue("@Deployed", Deployed);
        //                cmd.Parameters.AddWithValue("@DeployedDate", DeployedDate);
        //                cmd.Parameters.AddWithValue("@Remarks", Remarks);
        //                cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
        //                sresult = (int)cmd.ExecuteNonQuery();
        //                con.Close();

        //            }
        //        }
        //        return sresult;
        //    }
        //    catch (Exception ex)
        //    {
        //        return sresult = 0;
        //    }
        //}

        public int ServerFlagsUpdate(string UID, int Type, string TabelName, string Flag, string PrimaryKey)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_ServerFlagsUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@Type", Type);
                        cmd.Parameters.AddWithValue("@TabelName", TabelName);
                        cmd.Parameters.AddWithValue("@Flag", Flag);
                        cmd.Parameters.AddWithValue("@PrimaryKey", PrimaryKey);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        //public int ResourceDeployment_Update(Guid WorkpackageUID, Guid ResourceUID, float Deployed, DateTime DeployedDate, string Remarks, string ServerCopiedAdd)
        //{
        //    int sresult = 0;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
        //        {

        //            using (SqlCommand cmd = new SqlCommand("dbsync_ResourceDeployment_Update"))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                con.Open();
        //                cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
        //                cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
        //                cmd.Parameters.AddWithValue("@Deployed", Deployed);
        //                cmd.Parameters.AddWithValue("@DeployedDate", DeployedDate);
        //                cmd.Parameters.AddWithValue("@Remarks", Remarks);
        //                cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
        //                sresult = (int)cmd.ExecuteNonQuery();
        //                con.Close();

        //            }
        //        }
        //        return sresult;
        //    }
        //    catch (Exception ex)
        //    {
        //        return sresult = 0;
        //    }
        //}


        //public int ResourceDeploymentUpdate_Delete(Guid UID, Guid UserUID)
        //{
        //    int sresult = 0;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
        //        {

        //            using (SqlCommand cmd = new SqlCommand("usp_ResourceDeploymentUpdate_Delete"))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                cmd.Parameters.AddWithValue("@UID", UID);
        //                cmd.Parameters.AddWithValue("@UserUID", UserUID);
        //                con.Open();
        //                sresult = (int)cmd.ExecuteNonQuery();
        //                con.Close();

        //            }
        //        }
        //        return sresult;
        //    }
        //    catch (Exception ex)
        //    {
        //        return sresult = 0;
        //    }
        //}

        public int MeasurementBookUpdate(Guid UID, Guid TaskUID, string UnitforProgress, string Quantity, string Description, DateTime CreatedDate, string Upload_File, Guid CreatedByUID, string Remarks)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_MeasurementUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@Upload_File", Upload_File);
                        cmd.Parameters.AddWithValue("@CreatedByUID", CreatedByUID);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        sresult = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                sresult = 0;
            }
            return sresult;
        }

        public int Measurement_Delete(Guid UID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_Measurement_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }
        internal int InsertjointInspection(Guid inspectionUid, Guid BOQUid, string DiaPipe, string unit, string invoice_number, string invoicedate, string quantity, string Inspection_Type,
            string Chainage_Number, string Chainage_Desc, float Chainage_StartPoint, float Chainage_Length, float Qty_in_RMT, double Qty_for_Unit, double Deduction, string Remarks, Guid ProjectUID, string PipeNumber, string Number, string Lenght, string Width, string Depth,Guid workpackageUid)
        {
            int cnt = 0;
            try
            {

                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertJointInspection"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@inspectionUid", inspectionUid);
                        cmd.Parameters.AddWithValue("@BOQUid", BOQUid);
                        cmd.Parameters.AddWithValue("@DiaPipe", DiaPipe);
                        cmd.Parameters.AddWithValue("@unit", unit);
                        cmd.Parameters.AddWithValue("@invoice_number", invoice_number);
                        cmd.Parameters.AddWithValue("@invoicedate", invoicedate);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@Inspection_Type", Inspection_Type);
                        cmd.Parameters.AddWithValue("@Chainage_Number", Chainage_Number == null ? (object)DBNull.Value : Chainage_Number);
                        cmd.Parameters.AddWithValue("@Chainage_Desc", Chainage_Desc == null ? (object)DBNull.Value : Chainage_Desc);
                        cmd.Parameters.AddWithValue("@Chainage_StartPoint", Chainage_StartPoint);
                        cmd.Parameters.AddWithValue("@Chainage_Length", Chainage_Length);
                        cmd.Parameters.AddWithValue("@Qty_in_RMT", Qty_in_RMT);
                        cmd.Parameters.AddWithValue("@Qty_for_Unit", Qty_for_Unit);
                        cmd.Parameters.AddWithValue("@Deduction", Deduction);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks == null ? (object)DBNull.Value : Remarks);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@PipeNumber", PipeNumber == null ? (object)DBNull.Value : PipeNumber);
                        cmd.Parameters.AddWithValue("@Number", Number == null ? (object)DBNull.Value : Number);//Venkat added code on 27 July 2022 (== null ? (object)DBNull.Value : Number)
                        cmd.Parameters.AddWithValue("@Lenght", Lenght);
                        cmd.Parameters.AddWithValue("@Width", Width == null ? (object)DBNull.Value : Width);
                        cmd.Parameters.AddWithValue("@Depth", Depth == null ? (object)DBNull.Value : Depth);
                        cmd.Parameters.AddWithValue("@WorkpackgeUID", workpackageUid);
                        con.Open();
                        cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
                        con.Close();

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return cnt;
        }

        internal int InsertjointInspection(Guid inspectionUid, Guid BOQUid, string DiaPipe, string unit, string invoice_number, string invoicedate, string quantity, string Inspection_Type,
           string Chainage_Number, string Chainage_Desc, float Chainage_StartPoint, float Chainage_Length, float Qty_in_RMT, double Qty_for_Unit, double Deduction, string Remarks, Guid ProjectUID, string PipeNumber, string ServerCopiedAdd)
        {
            int cnt = 0;
            try
            {

                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_JointInspectionAddorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@inspectionUid", inspectionUid);
                        cmd.Parameters.AddWithValue("@BOQUid", BOQUid);
                        cmd.Parameters.AddWithValue("@DiaPipe", DiaPipe);
                        cmd.Parameters.AddWithValue("@unit", unit);
                        cmd.Parameters.AddWithValue("@invoice_number", invoice_number);
                        cmd.Parameters.AddWithValue("@invoicedate", invoicedate);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@Inspection_Type", Inspection_Type);
                        cmd.Parameters.AddWithValue("@Chainage_Number", Chainage_Number);
                        cmd.Parameters.AddWithValue("@Chainage_Desc", Chainage_Desc);
                        cmd.Parameters.AddWithValue("@Chainage_StartPoint", Chainage_StartPoint);
                        cmd.Parameters.AddWithValue("@Chainage_Length", Chainage_Length);
                        cmd.Parameters.AddWithValue("@Qty_in_RMT", Qty_in_RMT);
                        cmd.Parameters.AddWithValue("@Qty_for_Unit", Qty_for_Unit);
                        cmd.Parameters.AddWithValue("@Deduction", Deduction);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@PipeNumber", PipeNumber);
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        con.Open();
                        cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
                        con.Close();

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return cnt;
        }

        public int ResourceDeploymentUpdate_Delete(Guid UID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_ResourceDeploymentUpdate_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet GetResourceDeploymentUpdate_by_UID(Guid UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetResourceDeploymentUpdate_by_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UID", UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public Boolean Dbsync_InsertorUpdateBankGuarantee(Guid Bank_GuaranteeUID, string Vendor_Name, string Vendor_Address, double Amount, string Validity, DateTime Date_of_Guarantee, int No_of_Collaterals,
            string Bank_Name, string Bank_Branch, string IFSC_Code, Guid WorkPackageUID, Guid ProjectUID, string BG_Number, DateTime Date_of_Expiry, string Currency, string Currency_CultureInfo, DateTime Cliam_Date, string ServerCopiedAdd, string Bank_Address)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_BankGuarantee"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@Bank_GuaranteeUID", Bank_GuaranteeUID);
                        cmd.Parameters.AddWithValue("@Vendor_Name", Vendor_Name);
                        cmd.Parameters.AddWithValue("@Vendor_Address", Vendor_Address);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@Validity", Validity);
                        cmd.Parameters.AddWithValue("@Date_of_Guarantee", Date_of_Guarantee);
                        cmd.Parameters.AddWithValue("@No_of_Collaterals", No_of_Collaterals);
                        cmd.Parameters.AddWithValue("@Bank_Name", Bank_Name);
                        cmd.Parameters.AddWithValue("@Bank_Branch", Bank_Branch);
                        cmd.Parameters.AddWithValue("@IFSC_Code", IFSC_Code);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@BG_Number", BG_Number);
                        cmd.Parameters.AddWithValue("@Date_of_Expiry", Date_of_Expiry);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Cliam_Date", Cliam_Date);
                        cmd.Parameters.AddWithValue("@Bank_Address", Bank_Address);
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public int Insurance_Delete(Guid InsuranceUID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsuranceDelete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public Boolean DbSyncInsertorUpdateInsurance(Guid InsuranceUID, string Vendor_Name, string Vendor_Address, string Name_of_InsuranceCompany, string Branch, string Policy_Number, string Policy_Status, DateTime Maturity_Date, string Nominee, Guid ProjectUID, Guid WorkPackageUID,
            DateTime Insured_Date, float Insured_Amount, float Premium_Amount, int Frequency, string Currency, string Currency_CultureInfo, string FirstPremium_Duedate, string ServerCopiedAdd)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdateInsurance"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                        cmd.Parameters.AddWithValue("@Vendor_Name", Vendor_Name);
                        cmd.Parameters.AddWithValue("@Vendor_Address", Vendor_Address);
                        cmd.Parameters.AddWithValue("@Name_of_InsuranceCompany", Name_of_InsuranceCompany);
                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@Policy_Number", Policy_Number);
                        cmd.Parameters.AddWithValue("@Policy_Status", Policy_Status);
                        cmd.Parameters.AddWithValue("@Maturity_Date", Maturity_Date);
                        cmd.Parameters.AddWithValue("@Nominee", Nominee);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@Insured_Date", Insured_Date);
                        cmd.Parameters.AddWithValue("@Insured_Amount", Insured_Amount);
                        cmd.Parameters.AddWithValue("@Premium_Amount", Premium_Amount);
                        cmd.Parameters.AddWithValue("@Frequency", Frequency);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        if (DateTime.TryParse(FirstPremium_Duedate, out DateTime DueDate))
                        {
                            cmd.Parameters.AddWithValue("@FirstPremium_Duedate", DueDate);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@FirstPremium_Duedate", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public Boolean DbSync_InsertorUpdateInsuranceDocuments(Guid InsuranceDoc_UID, Guid InsuranceUID, string InsuranceDoc_Name, string InsuranceDoc_Type, string InsuranceDoc_FilePath, string ServerCopiedAdd)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Dbsync_InsertorUpdate_InsuranceDocuments"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@InsuranceDoc_UID", InsuranceDoc_UID);
                        cmd.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                        cmd.Parameters.AddWithValue("@InsuranceDoc_Name", InsuranceDoc_Name);
                        cmd.Parameters.AddWithValue("@InsuranceDoc_Type", InsuranceDoc_Type);
                        cmd.Parameters.AddWithValue("@InsuranceDoc_FilePath", InsuranceDoc_FilePath);
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public Boolean DbSync_InsertorUpdateInsurancePremium(Guid PremiumUID, Guid InsuranceUID, float Premium_Paid, float Interest, float Penalty, DateTime Premium_PaidDate, DateTime Premium_DueDate, DateTime Next_PremiumDate, string Premium_Receipt, string Remarks, string ServerCopiedAdd)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Dbsync_InsertorUpdateInsurancePremium"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@PremiumUID", PremiumUID);
                        cmd.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                        cmd.Parameters.AddWithValue("@Premium_Paid", Premium_Paid);
                        cmd.Parameters.AddWithValue("@Interest", Interest);
                        cmd.Parameters.AddWithValue("@Penalty", Penalty);
                        cmd.Parameters.AddWithValue("@Premium_PaidDate", Premium_PaidDate);
                        cmd.Parameters.AddWithValue("@Premium_DueDate", Premium_DueDate);
                        cmd.Parameters.AddWithValue("@Next_PremiumDate", Next_PremiumDate);
                        cmd.Parameters.AddWithValue("@Premium_Receipt", Premium_Receipt);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public int BankDocuments_Delete(Guid BankDoc_UID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_BankDocuments_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@BankDoc_UID", BankDoc_UID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int InsuranceDocuments_Delete(Guid InsuranceDoc_UID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsuranceDocuments_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@InsuranceDoc_UID", InsuranceDoc_UID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int InsurancePremium_Delete(Guid PremiumUID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsurancePremium_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PremiumUID", PremiumUID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public DataSet GetInsurancePremiumSelect_by_PremiumUID(Guid PremiumUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_InsurancePremiumSelect_by_PremiumUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@PremiumUID", PremiumUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertJointInspectionDocuments(Guid InspectionDocumentUID, Guid inspectionUid, string InspectionDocument_Name, string InspectionDocumentType, string InspectionDocument_FilePath, string ServerCopiedAdd)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertJointInspectionDocuments"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@InspectionDocumentUID", InspectionDocumentUID);
                        cmd.Parameters.AddWithValue("@inspectionUid", inspectionUid);
                        cmd.Parameters.AddWithValue("@InspectionDocument_Name", InspectionDocument_Name);
                        cmd.Parameters.AddWithValue("@InspectionDocumentType", InspectionDocumentType);
                        cmd.Parameters.AddWithValue("@InspectionDocument_FilePath", InspectionDocument_FilePath);
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public int BankGuarantee_Delete_by_Bank_GuaranteeUID(Guid Bank_GuaranteeUID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("BankGuarantee_Delete_by_Bank_GuaranteeUID"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Bank_GuaranteeUID", Bank_GuaranteeUID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public Boolean InsertorUpdateBankDocuments(Guid BankDoc_UID, Guid Bank_GuaranteeUID, string Document_Name, string Document_Type, string Document_File, string ServerCopiedAdd)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdateBankGuarantee_Documents"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@BankDoc_UID", BankDoc_UID);
                        cmd.Parameters.AddWithValue("@Bank_GuaranteeUID", Bank_GuaranteeUID);
                        cmd.Parameters.AddWithValue("@Document_Name", Document_Name);
                        cmd.Parameters.AddWithValue("@Document_Type", Document_Type);
                        cmd.Parameters.AddWithValue("@Document_File", Document_File);
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //added on 14/10/2021
        public int CheckGetWebApiSettings(string Username, string Ipaddress)
        {
            int cnt = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand();
                if (Username.ToLower() == "exto1")
                {
                    cmd = new SqlCommand("usp_GetWebApiSettings", con);
                }
                else
                {
                    cmd = new SqlCommand("usp_GetWebAndriodSettings", con);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Ipaddress", Ipaddress);
                cnt = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                cnt = 0;
            }
            return cnt;
        }

        //added on 18/10/2021
        public int checkWebAPIUser(string Username, string Password)
        {
            int cnt = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = null;
                if (Username.ToLower() == "exto1")
                {
                    cmd = new SqlCommand("usp_checkAPIUser", con);
                }
                else
                {
                    cmd = new SqlCommand("usp_checkAndriodAPIUser", con);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Password", Password);
                cnt = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                cnt = 0;
            }
            return cnt;
        }

        // added on 21/10/2021
        public string CheckSubmittalCategoryExists(Guid UID, string Name,string Type,Guid WorkPackageUID)
        {
            Guid result = Guid.NewGuid();
            string Sresult = string.Empty;
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
               
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_CheckSubmittalCategoryExists", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UID", UID);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                result  =(Guid) cmd.ExecuteScalar();
                Sresult = result.ToString();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                Sresult = string.Empty;
            }
            return Sresult;
        }

        public string CheckSubmittalFlowExists(Guid UID, string Name, string Type)
        {
            Guid result = Guid.NewGuid();
            string Sresult = string.Empty;
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
               
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_CheckSubmittalFlowExists", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UID", @UID);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Type", Type);
                result = (Guid)cmd.ExecuteScalar();
                Sresult = result.ToString();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                Sresult = string.Empty;
            }
            return Sresult;
        }

        public string CheckUserExists(Guid UID, string Name, string Type)
        {
            Guid result = Guid.NewGuid();
            string Sresult = string.Empty;
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_CheckUserExists", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UID", @UID);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Type", Type);
                result = (Guid)cmd.ExecuteScalar();
                Sresult = result.ToString();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                Sresult = string.Empty;
            }
            return Sresult;
        }

        // added for Arun  on 24/12/2021
        internal DataTable getJointInspection_by_inspectionUid(string inspectionUid)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetJointInspection_by_inspectionUid", con);
                cmd.SelectCommand.Parameters.AddWithValue("@inspectionUid", inspectionUid);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public DataSet GetResourceDeployment_by_ReourceDeploymentUID(Guid ReourceDeploymentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetResourceDeployment_by_ReourceDeploymentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ReourceDeploymentUID", ReourceDeploymentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }


        public int ResourceDeployment_Delete(Guid UID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_ResourceDeployment_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public DataSet GetJointInspectionDocuments_by_InspectionDocumentUID(Guid InspectionDocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetJointInspectionDocuments_by_InspectionDocumentUID", con);
                cmd.SelectCommand.Parameters.AddWithValue("@InspectionDocumentUID", InspectionDocumentUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int JointInspectionDocument_Delete(Guid InspectionDocumentUID, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_JointInspectionDocument_Delete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@InspectionDocumentUID", InspectionDocumentUID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public int WebAPITransctionInsert(Guid TranscationUID, string WebAPI_URL, string Parameters, string TranscationStatus)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertWebAPITransction"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TranscationUID", TranscationUID);
                        cmd.Parameters.AddWithValue("@WebAPI_URL", WebAPI_URL);
                        cmd.Parameters.AddWithValue("@Parameters", Parameters);
                        cmd.Parameters.AddWithValue("@TranscationStatus", TranscationStatus);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public bool TaskUIDExists(Guid ActivityUID)
        {
            bool tExists = false;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetTaskUIDExists", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaskUID", ActivityUID);
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    tExists = true;
                }
                con.Close();
            }
            catch (Exception ex)
            {
                tExists = false;
            }
            return tExists;
        }

        public int JointInspection_Delete(Guid inspectionUid, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_JointInspectionDelete"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@inspectionUid", inspectionUid);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int ResourceDeployment_Update(Guid UID, Guid WorkpackageUID, Guid ResourceUID, float Deployed, DateTime DeployedDate, string Remarks, string ServerCopiedAdd)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_ResourceDeployment_Update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                        cmd.Parameters.AddWithValue("@Deployed", Deployed);
                        cmd.Parameters.AddWithValue("@DeployedDate", DeployedDate);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@ServerCopiedAdd", ServerCopiedAdd);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public string ConvertDateFormat(string dFormat)
        {
            if (WebConfigurationManager.AppSettings["ServerDateFormat"] == "MM/dd/yyyy")
            {
                dFormat = dFormat.Split('/')[1] + "/" + dFormat.Split('/')[0] + "/" + dFormat.Split('/')[2];
            }
            else if (WebConfigurationManager.AppSettings["ServerDateFormat"] == "MM-dd-yyyy")
            {
                dFormat = dFormat.Split('-')[1] + "-" + dFormat.Split('-')[0] + "-" + dFormat.Split('-')[2];
                dFormat = dFormat.Replace("-", "/");
            }
            else
            {
                dFormat = dFormat.Replace("-", "/");
            }
            return dFormat;
        }


        public string GetBOQUID_FromName(Guid ActivityUID, string BOQName, string From)
        {
            string tUID = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetBOQUID_FromName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
                cmd.Parameters.AddWithValue("@ActivityUID", ActivityUID);
                cmd.Parameters.AddWithValue("@BOQName", BOQName);
                cmd.Parameters.AddWithValue("@From", From);
                tUID = cmd.ExecuteScalar().ToString();
                con.Close();
            }
            catch (Exception ex)
            {
                tUID = "";
            }
            return tUID;
        }

        // added on 26/01/2022 for Venkat
        public DataTable GetDeductionDesc()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDeductionDesc", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(dt);
            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;
        }

        public int InvoiceMaster_InsertorUpdate(Guid InvoiceMaster_UID, Guid ProjectUID, Guid WorkpackageUID, string Invoice_Number, string Invoice_Desc, DateTime Invoice_Date, string Currency, string Currency_CultureInfo)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateInvoiceMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@InvoiceMaster_UID", InvoiceMaster_UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@Invoice_Number", Invoice_Number);
                        cmd.Parameters.AddWithValue("@Invoice_Desc", Invoice_Desc);
                        cmd.Parameters.AddWithValue("@Invoice_Date", Invoice_Date);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public DataTable GetWorkPackages_ProjectName(string ProjectName)
        {
            DataTable dtWorkPackages = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlDataAdapter cmd = new SqlDataAdapter("usp_getWorkPackage_ProjectName", con))
                    {

                        cmd.SelectCommand.Parameters.AddWithValue("@ProjectName", ProjectName);
                        cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                        cmd.Fill(dtWorkPackages);
                    }
                }
                return dtWorkPackages;
            }
            catch (Exception ex)
            {
                return dtWorkPackages;
            }
        }

        internal string AddRABillNumber(string rabillnumber, Guid WorkpackageUID, DateTime RABill_Date)
        {
            string rabillUid = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_AddRaBill_API"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@rabillnumber", rabillnumber);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@RABill_Date", RABill_Date);
                        con.Open();
                        rabillUid = Convert.ToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                rabillUid = "Error1";
            }
            return rabillUid;
        }

        public DataSet GetBOQDetails_by_projectuid(Guid projectuid)
        {
            DataSet ds = new DataSet();
            try
            {

                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlConnection con = new SqlConnection(db.GetConnectionString());
                    SqlDataAdapter cmd = new SqlDataAdapter("usp_GetBOQDetails_by_projectuid", con);
                    cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    cmd.SelectCommand.CommandTimeout = 900;
                    cmd.SelectCommand.Parameters.AddWithValue("@projectuid", projectuid);
                    cmd.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertJointInspectiontoRAbill(Guid AssignJointInspectionUID, Guid RABill_UID, Guid RABill_ItemUID, Guid InspectionUID)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertJointInspectiontoRAbill"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AssignJointInspectionUID", AssignJointInspectionUID);
                        cmd.Parameters.AddWithValue("@RABill_UID", RABill_UID);
                        cmd.Parameters.AddWithValue("@RABill_ItemUID", RABill_ItemUID);
                        cmd.Parameters.AddWithValue("@InspectionUID", InspectionUID);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public DataSet GetDeductionFromDesc(string DeductionsDescription)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDeductionFromDesc", con);
                cmd.SelectCommand.Parameters.AddWithValue("@DeductionsDescription", DeductionsDescription);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public decimal GetRAbillPresentTotalAmount_by_RABill_UID(Guid RABill_UID)
        {
            decimal BillValue = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetRAbillPresentTotalAmount_by_RABill_UID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RABill_UID", RABill_UID);
                BillValue = (decimal)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                BillValue = 0;
            }
            return BillValue;
        }

        internal int InsertRABillsItems(object RABillUId, string item_number, string item_desc, string Date, string current_cost, Guid ProjectUID, Guid WorkpackageUID, Guid BOQUID)
        {
            int cnt = 0;
            try
            {
                if (double.TryParse(current_cost, out double currentcost))
                {

                    using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                    {

                        using (SqlCommand cmd = new SqlCommand("usp_InsertUpdateRABills"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@item_number", item_number);
                            cmd.Parameters.AddWithValue("@item_desc", item_desc);
                            cmd.Parameters.AddWithValue("@uid", BOQUID);
                            cmd.Parameters.AddWithValue("@currentcost", currentcost);
                            cmd.Parameters.AddWithValue("@raBillNo", RABillUId);
                            cmd.Parameters.AddWithValue("@CreatedDate", Date);
                            cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                            cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                            con.Open();
                            cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                cnt = 0;
            }
            return cnt;
        }

        public int InvoiceDeduction_InsertorUpdate(Guid Invoice_DeductionUID, Guid WorkpackageUID, Guid InvoiceMaster_UID, Guid Deduction_UID, float Amount, float Percentage, string Currency, string Currency_CultureInfo, string Deduction_Mode)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateInvoiceDeductions"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Invoice_DeductionUID", Invoice_DeductionUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@InvoiceMaster_UID", InvoiceMaster_UID);
                        cmd.Parameters.AddWithValue("@Deduction_UID", Deduction_UID);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@Percentage", Percentage);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Deduction_Mode", Deduction_Mode);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        // added on 27/01/2022
        public string GetFinMonthUID(Guid WorkPackageUID, string Month, int Year)
        {
            string result = "";
            Guid tUID = Guid.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetFinMonthUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Parameters.AddWithValue("@Month", Month);
                cmd.Parameters.AddWithValue("@Year", Year);
                tUID = (Guid)cmd.ExecuteScalar();
                result = tUID.ToString();
                con.Close();
            }
            catch (Exception ex)
            {
                
                result = tUID.ToString();
            }
            return result;
        }

        public string GetInvoiceUIDFromInvoiceNo(Guid WorkPackageUID, string InvoiceNo)
        {
            string result = "";
            Guid tUID = Guid.Empty;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetInvoiceUIDFromInvoiceNo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                tUID = (Guid)cmd.ExecuteScalar();
                result = tUID.ToString();
                con.Close();
            }
            catch (Exception ex)
            {
                result = tUID.ToString();
            }
            return result;
        }

        public DataSet GetInvoiceDeductions(Guid InvoiceMasterUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("[usp_GetInvoiceDeductions]", con);
                cmd.SelectCommand.Parameters.AddWithValue("@InvoiceMasterUID", InvoiceMasterUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        internal int InsertRABillsDeductions(Guid UID, Guid PaymentUID, Guid DeductionUID, float Amount, float Percentage)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_InsertRABillsDeductions"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@PaymentUID", PaymentUID);
                        cmd.Parameters.AddWithValue("@DeductionUID", DeductionUID);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@Percentage", Percentage);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }

        }

        internal int InsertRABillPayments(Guid PaymentUID, Guid InvoiceUID, string RABillDesc, float Amount, float TotalDeductions, float NetAmount, Guid FinMonthUID, DateTime PaymentDate,float TotalAdditions)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertRABillPayments"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PaymentUID", PaymentUID);
                        cmd.Parameters.AddWithValue("@InvoiceUID", InvoiceUID);
                        cmd.Parameters.AddWithValue("@RABillDesc", RABillDesc);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@TotalDeductions", TotalDeductions);
                        cmd.Parameters.AddWithValue("@NetAmount", NetAmount);
                        cmd.Parameters.AddWithValue("@FinMonthUID", FinMonthUID);
                        cmd.Parameters.AddWithValue("@PaymentDate", PaymentDate);
                        cmd.Parameters.AddWithValue("@TotalAdditions", TotalAdditions); 
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }

        }

        public int checkInvoicePaid(Guid InvoiceUID)
        {
            int cnt = 0;
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_checkInvoicePaid", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceUID", InvoiceUID);
               
                cnt = (int)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                cnt = 0;
            }
            return cnt;
        }

        //added on  03/02/2022
        public DataSet GetOriginatorMaster()
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_OriginatorSelect", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 04/02/2022
        public DataSet GetInsurancePremium_DueDate_NextDueDate(Guid InsuranceUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetInsurancePremium_DueDate_NextDueDate", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        // added on 06/02/2022 for venkat
        public DataSet getLatesDocumentStatus(Guid DocumentUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("getActualDocumentLatestStatus", con);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 11/02/2022 for  venkat
        internal string gbGetBoqDetailsUId_InspectionUId(Guid inspectionUid)
        {
            string boqDetailsUid = "";
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getBoqDetailsUid_by_InspectionUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@inspectionUid", inspectionUid);
                DataSet ds = new DataSet();
                cmd.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    boqDetailsUid = ds.Tables[0].Rows[0]["BOQUid"].ToString();
                }
            }
            catch (Exception ex)
            {
                boqDetailsUid = "";
            }
            return boqDetailsUid;
        }

        // added on 18/02/2022 for venkat  //changed on 15/09/2022 by zuber to add same as webapp
        public int Issues_Status_Remarks_Insert(Guid IssueRemarksUID, Guid Issue_Uid, string Issue_Status, string Issue_Remarks, string Issue_Document, DateTime approved_date)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_Issue_Status_Remarks_Insert"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@IssueRemarksUID", IssueRemarksUID);
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                        cmd.Parameters.AddWithValue("@Issue_Status", Issue_Status);
                        cmd.Parameters.AddWithValue("@Issue_Remarks", Issue_Remarks);
                        cmd.Parameters.AddWithValue("@Issue_Document", Issue_Document);
                        cmd.Parameters.AddWithValue("@ClosureDate", approved_date);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        // added on 18/02/2022 for venkat
        public int InsertorUpdateIssues(Guid Issue_Uid, Guid TaskUID, string Issue_Description, DateTime Issue_Date, Guid Issued_User, 
           string Issue_Status, string Issue_Remarks, Guid WorkPackagesUID, Guid ProjectUID, string Issue_Document)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_IssuesInsertUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Issue_Description", Issue_Description);
                        cmd.Parameters.AddWithValue("@Issue_Date", Issue_Date);
                        cmd.Parameters.AddWithValue("@Issued_User", Issued_User);
                      //  cmd.Parameters.AddWithValue("@Assigned_User", Assigned_User);
                       // cmd.Parameters.AddWithValue("@Assigned_Date", Assigned_Date);
                        //cmd.Parameters.AddWithValue("@Issue_ProposedCloser_Date", Issue_ProposedCloser_Date);
                       // cmd.Parameters.AddWithValue("@Approving_User", Approving_User);
                       // cmd.Parameters.AddWithValue("@Actual_Closer_Date", Actual_Closer_Date);
                        cmd.Parameters.AddWithValue("@Issue_Status", Issue_Status);
                        cmd.Parameters.AddWithValue("@Issue_Remarks", Issue_Remarks);
                        cmd.Parameters.AddWithValue("@WorkPackagesUID", WorkPackagesUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Issue_Document", Issue_Document);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }

        public int InsertorUpdateIssues(Guid Issue_Uid, Guid TaskUID, string Issue_Description, DateTime Issue_Date, Guid Issued_User,Guid Assigned_User,DateTime Assigned_Date,DateTime Issue_ProposedCloser_Date,
        Guid Approving_User,DateTime Actual_Closer_Date,string Issue_Status, string Issue_Remarks, Guid WorkPackagesUID, Guid ProjectUID, string Issue_Document)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("ups_IssuesInsertUpdate_approvedUser"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Issue_Description", Issue_Description);
                        cmd.Parameters.AddWithValue("@Issue_Date", Issue_Date);
                        cmd.Parameters.AddWithValue("@Issued_User", Issued_User);
                         cmd.Parameters.AddWithValue("@Assigned_User", Assigned_User);
                         cmd.Parameters.AddWithValue("@Assigned_Date", Assigned_Date);
                        cmd.Parameters.AddWithValue("@Issue_ProposedCloser_Date", Issue_ProposedCloser_Date);
                         cmd.Parameters.AddWithValue("@Approving_User", Approving_User);
                         cmd.Parameters.AddWithValue("@Actual_Closer_Date", Actual_Closer_Date);
                        cmd.Parameters.AddWithValue("@Issue_Status", Issue_Status);
                        cmd.Parameters.AddWithValue("@Issue_Remarks", Issue_Remarks);
                        cmd.Parameters.AddWithValue("@WorkPackagesUID", WorkPackagesUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@Issue_Document", Issue_Document);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }
        // added on 19/02/2022 for venkat
        public DataSet GetIssueStatus_by_Issue_Uid(Guid Issue_Uid)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_GetIssueStatus_by_Issue_Uid", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        // added on 22/022022 for aRun
        public DataSet GetTaskSchedule_By_TaskUID(Guid TaskUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskSchedule_By_TaskUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUID", TaskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //Added by venkat on 23/02/2022
        public DataSet ActualDocuments_SelectBy_WorkpackageUID_Search(Guid ProjectUID, Guid WorkPackageUID, string DocumentName, string Doctype, string SubmittalName, string Status, DateTime DocDate, DateTime DocumentDate, DateTime DocToDate, DateTime DocumentToDate, int Type)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_SelectBy_WorkpackageUID_Search1", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentName", DocumentName);
                cmd.SelectCommand.Parameters.AddWithValue("@Doctype", Doctype);
                cmd.SelectCommand.Parameters.AddWithValue("@SubmittalName", SubmittalName);
                cmd.SelectCommand.Parameters.AddWithValue("@Status", Status);
                cmd.SelectCommand.Parameters.AddWithValue("@Type", Type);
                cmd.SelectCommand.Parameters.AddWithValue("@DocDate", DocDate);// this is incomiung recv date
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentDate", DocumentDate);
                cmd.SelectCommand.Parameters.AddWithValue("@DocDateTo", DocToDate);// this is incomiung recv date
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentDateTo", DocumentToDate);
                cmd.SelectCommand.Parameters.AddWithValue("@OntbRefNumber", DBNull.Value);
                cmd.SelectCommand.Parameters.AddWithValue("@OriginatorRefNumber", DBNull.Value);// this is incomiung recv date
                cmd.SelectCommand.Parameters.AddWithValue("@FlowUID", DBNull.Value);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
               // LogWrite("Error:" + ex.Message);
                ds = null;
            }
            //LogWrite("Documents Count:" + ds.Tables[0].Rows.Count);
            return ds;
        }
        public DataSet GetDocumentSummary_by_WorkpackgeUID(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDocumentSummary_by_WorkpackgeUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetTaskScheduleDatesforGraph(Guid WorkpackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskScheduleDatesforGraph", con);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DataSet GetTaskScheduleValuesForGraph(Guid WorkpackageUID, DateTime startdate, DateTime enddate)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetTaskScheduleValuesForGraph", con);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkpackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@StartDate", startdate);
                cmd.SelectCommand.Parameters.AddWithValue("@EndDate", enddate);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public int WebAPITransctionUpdate(Guid TranscationUID, string TranscationStatus, string TransactionResult)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_UpdateWebAPITransction"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TranscationUID", TranscationUID);
                        cmd.Parameters.AddWithValue("@TranscationStatus", TranscationStatus);
                        cmd.Parameters.AddWithValue("@TransactionResult", TransactionResult);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }
        public DataTable GetTasks_by_ProjectName(string projectName)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getGetTasks_ByProjectUid", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectName", projectName);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = new DataTable();
            }
            return ds;
        }
        public DataTable getNextLevelTaskDetails(Guid taskUID)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getGetTaskNextLevel_BytaskUid", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@TaskUid", taskUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = new DataTable();
            }
            return ds;
        }


        internal DataTable getapi_submittal_users(string projectName)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getapi_submittal_users", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@projectName", projectName);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = new DataTable();
            }
            return ds;
        }
        internal DataTable getActualDocumentStatus(Guid projectuid, Guid workpackageuid, Guid documentuid)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_getActualDocumentStatus", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@projectuid", projectuid);
                cmd.SelectCommand.Parameters.AddWithValue("@workpackageuid", workpackageuid);
                cmd.SelectCommand.Parameters.AddWithValue("@documentuid", documentuid);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = new DataTable();
            }
            return ds;
        }
        public DataTable GetNextUserDocuments(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataTable ds = new DataTable();
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetNextUserDocuments", con);
                cmd.SelectCommand.CommandTimeout = 600;
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);

            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        //added on 31/03/2022
        public DataSet getUserDetails_Contractor(Guid sUserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetUserDetails_Contractor", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", sUserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        internal DataTable getIssuesByDescription(Guid workpackageUid , string issueDesc)
        {

            DataTable dtIssues = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetIssues_Description", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@issueDesc", issueDesc);
                cmd.SelectCommand.Parameters.AddWithValue("@workpackageUid", workpackageUid);
                cmd.Fill(dtIssues);
            }
            catch (Exception ex)
            {
                dtIssues = null;
            }
            return dtIssues;
        }
        //public void LogWrite(string logMessage)
        //{
        //    string m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    try
        //    {
        //        using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
        //        {
        //            w.WriteLine("{0}", DateTime.Now);
        //            w.WriteLine("{0}", logMessage);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}



        // added on 05/05/2022
        
        public int InsertUploadedDocument(string name, string path, string issue_remarks_uid)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("InsertUploadedDocument"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@path", path);
                        cmd.Parameters.AddWithValue("@issue_remarks_uid", issue_remarks_uid);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }

        //Added by Venkat on 08 may 2022
        internal bool UpdatePremiumReceipt(string insurancePremiumUID, string receiptPath)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_UpdatePremiumReceipt"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@insurancePremiumUID", insurancePremiumUID);
                        cmd.Parameters.AddWithValue("@receiptPath", receiptPath);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        //Added for nakib on 24/05/2022
        public bool IsSubmitalExist(string workpackageUID, string TaskUID, string Name)
        {
            bool isExist = false;
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select Count(1) from Documents where DocName = '" + Name + "' and WorkPackageUID = '" + workpackageUID + "' and TaskUID = '" + TaskUID + "' and Delete_Flag='N'";
                int response = (int)cmd.ExecuteScalar();
                if (response > 0)
                    isExist = true;

                con.Close();
            }
            catch (Exception ex)
            {
                isExist = false;
                if (con.State == ConnectionState.Open) con.Close();
            }
            return isExist;
        }

        public int DeleteSubmittal_MultipleUsers(Guid SubmittalUID, Int16 Steps)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_DeleteSubmittal_MultipleUsers"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@SubmittalUID", SubmittalUID);
                        cmd.Parameters.AddWithValue("@Step", Steps);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }
        public int InsertSubmittal_MultipleUsers(Guid UID, Guid SubmittalUID, Int16 Step, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_InsertSubmittal_MultipleUsers"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@SubmittalUID", SubmittalUID);
                        cmd.Parameters.AddWithValue("@Step", Step);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        sresult = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                sresult = 0;
            }
            return sresult;
        }


        public int DoumentMaster_Insert_or_Update_FlowAll(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
          Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
          Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, Guid FlowStep2_UserUID, DateTime FlowStep2_TargetDate,
          Guid FlowStep3_UserUID, DateTime FlowStep3_TargetDate, Guid FlowStep4_UserUID, DateTime FlowStep4_TargetDate, Guid FlowStep5_UserUID, DateTime FlowStep5_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType, string IsSynch,
          Guid FlowStep6_UserUID, DateTime FlowStep6_TargetDate,
          Guid FlowStep7_UserUID, DateTime FlowStep7_TargetDate,
          Guid FlowStep8_UserUID, DateTime FlowStep8_TargetDate,
          Guid FlowStep9_UserUID, DateTime FlowStep9_TargetDate,
          Guid FlowStep10_UserUID, DateTime FlowStep10_TargetDate,
          Guid FlowStep11_UserUID, DateTime FlowStep11_TargetDate,
          Guid FlowStep12_UserUID, DateTime FlowStep12_TargetDate,
          Guid FlowStep13_UserUID, DateTime FlowStep13_TargetDate,
          Guid FlowStep14_UserUID, DateTime FlowStep14_TargetDate,
          Guid FlowStep15_UserUID, DateTime FlowStep15_TargetDate,
          Guid FlowStep16_UserUID, DateTime FlowStep16_TargetDate,
          Guid FlowStep17_UserUID, DateTime FlowStep17_TargetDate,
          Guid FlowStep18_UserUID, DateTime FlowStep18_TargetDate,
          Guid FlowStep19_UserUID, DateTime FlowStep19_TargetDate,
          Guid FlowStep20_UserUID, DateTime FlowStep20_TargetDate, int step, string FlowStep1_IsMUser,
          string FlowStep2_IsMUser,
          string FlowStep3_IsMUser,
          string FlowStep4_IsMUser,
          string FlowStep5_IsMUser,
          string FlowStep6_IsMUser,
          string FlowStep7_IsMUser,
          string FlowStep8_IsMUser,
          string FlowStep9_IsMUser,
          string FlowStep10_IsMUser,
          string FlowStep11_IsMUser,
          string FlowStep12_IsMUser,
          string FlowStep13_IsMUser,
          string FlowStep14_IsMUser,
          string FlowStep15_IsMUser,
          string FlowStep16_IsMUser,
          string FlowStep17_IsMUser,
          string FlowStep18_IsMUser,
          string FlowStep19_IsMUser,
          string FlowStep20_IsMUser)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DoumentMaster_Insert_or_Update_FlowAll"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserUID", FlowStep1_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep2_UserUID", FlowStep2_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep3_UserUID", FlowStep3_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep4_UserUID", FlowStep4_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep4_TargetDate", FlowStep4_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep5_UserUID", FlowStep5_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep5_TargetDate", FlowStep5_TargetDate);
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@IsSync", IsSynch);
                        //
                        cmd.Parameters.AddWithValue("@FlowStep1_IsMUser", FlowStep1_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep2_IsMUser", FlowStep2_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep3_IsMUser", FlowStep3_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep4_IsMUser", FlowStep4_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep5_IsMUser", FlowStep5_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep6_IsMUser", FlowStep6_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep6_UserUID", FlowStep6_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep6_TargetDate", FlowStep6_TargetDate);
                        if (step >= 7)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep7_UserUID", FlowStep7_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep7_TargetDate", FlowStep7_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep7_IsMUser", FlowStep7_IsMUser);
                        }
                        if (step >= 8)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep8_UserUID", FlowStep8_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep8_TargetDate", FlowStep8_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep8_IsMUser", FlowStep8_IsMUser);
                        }
                        if (step >= 9)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep9_UserUID", FlowStep9_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep9_TargetDate", FlowStep9_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep9_IsMUser", FlowStep9_IsMUser);
                        }
                        if (step >= 10)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep10_UserUID", FlowStep10_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep10_TargetDate", FlowStep10_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep10_IsMUser", FlowStep10_IsMUser);
                        }
                        if (step >= 11)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep11_UserUID", FlowStep11_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep11_TargetDate", FlowStep11_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep11_IsMUser", FlowStep11_IsMUser);
                        }
                        if (step >= 12)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep12_UserUID", FlowStep12_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep12_TargetDate", FlowStep12_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep12_IsMUser", FlowStep12_IsMUser);
                        }
                        if (step >= 13)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep13_UserUID", FlowStep13_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep13_TargetDate", FlowStep13_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep13_IsMUser", FlowStep13_IsMUser);
                        }
                        if (step >= 14)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep14_UserUID", FlowStep14_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep14_TargetDate", FlowStep14_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep14_IsMUser", FlowStep14_IsMUser);
                        }
                        if (step >= 15)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep15_UserUID", FlowStep15_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep15_TargetDate", FlowStep15_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep15_IsMUser", FlowStep15_IsMUser);
                        }
                        if (step >= 16)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep16_UserUID", FlowStep16_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep16_TargetDate", FlowStep16_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep16_IsMUser", FlowStep16_IsMUser);
                        }
                        if (step >= 17)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep17_UserUID", FlowStep17_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep17_TargetDate", FlowStep17_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep17_IsMUser", FlowStep17_IsMUser);
                        }
                        if (step >= 18)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep18_UserUID", FlowStep18_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep18_TargetDate", FlowStep18_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep18_IsMUser", FlowStep18_IsMUser);
                        }
                        if (step >= 19)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep19_UserUID", FlowStep19_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep19_TargetDate", FlowStep19_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep19_IsMUser", FlowStep19_IsMUser);
                        }
                        if (step == 20)
                        {
                            cmd.Parameters.AddWithValue("@FlowStep20_UserUID", FlowStep20_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep20_TargetDate", FlowStep20_TargetDate);
                            cmd.Parameters.AddWithValue("@FlowStep20_IsMUser", FlowStep20_IsMUser);
                        }
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }
        public DataTable GetWorkpackageCategory(Guid WorkPackageUID)
        {
            DataTable ds = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    SqlDataAdapter cmd = new SqlDataAdapter("usp_GetWorkpackageCategory", con);
                    cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                    cmd.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        //public DataTable FlowMasterUser_Select(Guid FlowUID, Guid WorkPackageUID, Guid WorkPackageCategory_UID, int Step)
        //{
        //    DataTable ds = new DataTable();
        //    SqlConnection con = new SqlConnection(db.GetConnectionString());
        //    try
        //    {
        //        SqlDataAdapter cmd = new SqlDataAdapter("usp_Flow_Master_User_Select", con);
        //        cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
        //        cmd.SelectCommand.Parameters.AddWithValue("@FlowUID", FlowUID);
        //        cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
        //        cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageCategory_UID", WorkPackageCategory_UID);
        //        cmd.SelectCommand.Parameters.AddWithValue("@Step", Step);
        //        cmd.Fill(ds);

        //    }
        //    catch (Exception ex)
        //    {
        //        ds = null;

        //    }
        //    return ds;
        //}

        public DataTable FlowMasterUser_Select(Guid FlowUID, Guid WorkPackageUID, Guid WorkPackageCategory_UID, int Step, DataTable dtUpdatedUser)
        {
            DataTable ds = new DataTable();
            if (dtUpdatedUser != null && dtUpdatedUser.Rows.Count > 0)
            {
                var updatedUserForCurrentStep = dtUpdatedUser.AsEnumerable().Where(r => r.Field<int>("StepNo") == Step);
                if (updatedUserForCurrentStep.FirstOrDefault() != null)
                {
                    ds = updatedUserForCurrentStep.CopyToDataTable();
                    return ds;
                }
            }

            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter("usp_Flow_Master_User_Select", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@FlowUID", FlowUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageCategory_UID", WorkPackageCategory_UID);
                cmd.SelectCommand.Parameters.AddWithValue("@Step", Step);
                cmd.Fill(ds);

            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        public DataTable GetWorksTaskUpdatedStepUsers(Guid ProjectUID, Guid FlowUID)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("USP_GetWorksTaskUpdatedStepUsers", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@FlowUID", FlowUID);

                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public DateTime? CalculateHolidayOffset(DateTime fromDate, int offset)
        {
            DateTime? sresult = null;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("USP_CalculateHolidayOffset"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@Offset", offset);
                        con.Open();
                        sresult = (DateTime)cmd.ExecuteScalar();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = null;
            }
        }
        public DataSet GetUsers_under_WorkpackageUID(Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUsers_under_WorkpackageUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int DoumentMaster_Insert_or_Update_Flow_3(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
            Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
            Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, Guid FlowStep2_UserUID, DateTime FlowStep2_TargetDate,
            Guid FlowStep3_UserUID, DateTime FlowStep3_TargetDate, Guid FlowStep4_UserUID, DateTime FlowStep4_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType, string IsSynch)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DoumentMaster_Insert_or_Update_Flow_3"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserUID", FlowStep1_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep2_UserUID", FlowStep2_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep3_UserUID", FlowStep3_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep4_UserUID", FlowStep4_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep4_TargetDate", FlowStep4_TargetDate);
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@IsSync", IsSynch);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public int DoumentMaster_Insert_or_Update_Flow_4(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
            Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
            Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, Guid FlowStep2_UserUID, DateTime FlowStep2_TargetDate,
            Guid FlowStep3_UserUID, DateTime FlowStep3_TargetDate, Guid FlowStep4_UserUID, DateTime FlowStep4_TargetDate, Guid FlowStep5_UserUID, DateTime FlowStep5_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType, string IsSynch)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("DoumentMaster_Insert_or_Update_Flow_4"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@DocName", DocName);
                        cmd.Parameters.AddWithValue("@Doc_Category", Doc_Category);
                        cmd.Parameters.AddWithValue("@Doc_RefNumber", Doc_RefNumber);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_Budget", Doc_Budget);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@Flow_StartDate", Flow_StartDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserUID", FlowStep1_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep2_UserUID", FlowStep2_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep3_UserUID", FlowStep3_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep4_UserUID", FlowStep4_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep4_TargetDate", FlowStep4_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep5_UserUID", FlowStep5_UserUID);
                        cmd.Parameters.AddWithValue("@FlowStep5_TargetDate", FlowStep5_TargetDate);
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@IsSync", IsSynch);
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }



        public int Document_Insert_or_Update_with_RelativePath_FlowAll(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
           string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
           string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
           string FileRef_Number, string ActualDocument_CurrentStatus, Guid Step1UserUID, DateTime Step1User_TargetDate, Guid Step2UserUID, DateTime Step2User_TargetDate,
           Guid Step3UserUID, DateTime Step3User_TargetDate, Guid Step4UserUID, DateTime Step4User_TargetDate, Guid Step5UserUID, DateTime Step5User_TargetDate,
           Guid Step6UserUID, DateTime Step6User_TargetDate,
           Guid Step7UserUID, DateTime Step7User_TargetDate,
           Guid Step8UserUID, DateTime Step8User_TargetDate,
           Guid Step9UserUID, DateTime Step9User_TargetDate,
           Guid Step10UserUID, DateTime Step10User_TargetDate,
           Guid Step11UserUID, DateTime Step11User_TargetDate,
             Guid Step12UserUID, DateTime Step12User_TargetDate,
           Guid Step13UserUID, DateTime Step13User_TargetDate,
           Guid Step14UserUID, DateTime Step14User_TargetDate,
           Guid Step15UserUID, DateTime Step15User_TargetDate,
           Guid Step16UserUID, DateTime Step16User_TargetDate,
           Guid Step17UserUID, DateTime Step17User_TargetDate,
           Guid Step18UserUID, DateTime Step18User_TargetDate,
           Guid Step19UserUID, DateTime Step19User_TargetDate,
           Guid Step20UserUID, DateTime Step20User_TargetDate,
 string ActualDocument_Originator, DateTime Document_Date, string ActualDocument_RelativePath, string ActualDocument_DirectoryName, string UploadFilePhysicalpath, string CoverLetterUID, string SubmissionType, int steps, Guid StatusUID, Guid DocVersionUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("Document_Insert_or_Update_with_RelativePath_FlowAll"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@ProjectRef_Number", ProjectRef_Number);
                        cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@IncomingRec_Date", IncomingRec_Date);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@ActualDocument_Version", ActualDocument_Version);
                        cmd.Parameters.AddWithValue("@ActualDocument_Type", ActualDocument_Type);
                        cmd.Parameters.AddWithValue("@Media_HC", Media_HC);
                        cmd.Parameters.AddWithValue("@Media_SC", Media_SC);
                        cmd.Parameters.AddWithValue("@Media_SCEF", Media_SCEF);
                        cmd.Parameters.AddWithValue("@Media_HCR", Media_HCR);
                        cmd.Parameters.AddWithValue("@Media_SCR", Media_SCR);
                        cmd.Parameters.AddWithValue("@Media_NA", Media_NA);
                        cmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@FileRef_Number", FileRef_Number);
                        cmd.Parameters.AddWithValue("@ActualDocument_CurrentStatus", ActualDocument_CurrentStatus);
                        cmd.Parameters.AddWithValue("@Step1UserUID", Step1UserUID);
                        cmd.Parameters.AddWithValue("@Step1User_TargetDate", Step1User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step2UserUID", Step2UserUID);
                        cmd.Parameters.AddWithValue("@Step2User_TargetDate", Step2User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step3UserUID", Step3UserUID);
                        cmd.Parameters.AddWithValue("@Step3User_TargetDate", Step3User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step4UserUID", Step4UserUID);
                        cmd.Parameters.AddWithValue("@Step4User_TargetDate", Step4User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step5UserUID", Step5UserUID);
                        cmd.Parameters.AddWithValue("@Step5User_TargetDate", Step5User_TargetDate);
                        cmd.Parameters.AddWithValue("@Step6UserUID", Step6UserUID);
                        cmd.Parameters.AddWithValue("@Step6User_TargetDate", Step6User_TargetDate);

                        if (steps >= 7)
                        {
                            cmd.Parameters.AddWithValue("@Step7UserUID", Step7UserUID);
                            cmd.Parameters.AddWithValue("@Step7User_TargetDate", Step7User_TargetDate);
                        }

                        if (steps >= 8)
                        {
                            cmd.Parameters.AddWithValue("@Step8UserUID", Step8UserUID);
                            cmd.Parameters.AddWithValue("@Step8User_TargetDate", Step8User_TargetDate);
                        }

                        if (steps >= 9)
                        {
                            cmd.Parameters.AddWithValue("@Step9UserUID", Step9UserUID);
                            cmd.Parameters.AddWithValue("@Step9User_TargetDate", Step9User_TargetDate);
                        }

                        if (steps >= 10)
                        {
                            cmd.Parameters.AddWithValue("@Step10UserUID", Step10UserUID);
                            cmd.Parameters.AddWithValue("@Step10User_TargetDate", Step10User_TargetDate);
                        }

                        if (steps >= 11)
                        {
                            cmd.Parameters.AddWithValue("@Step11UserUID", Step11UserUID);
                            cmd.Parameters.AddWithValue("@Step11User_TargetDate", Step11User_TargetDate);
                        }

                        if (steps >= 12)
                        {
                            cmd.Parameters.AddWithValue("@Step12UserUID", Step12UserUID);
                            cmd.Parameters.AddWithValue("@Step12User_TargetDate", Step12User_TargetDate);
                        }

                        if (steps >= 13)
                        {
                            cmd.Parameters.AddWithValue("@Step13UserUID", Step13UserUID);
                            cmd.Parameters.AddWithValue("@Step13User_TargetDate", Step13User_TargetDate);
                        }

                        if (steps >= 14)
                        {
                            cmd.Parameters.AddWithValue("@Step14UserUID", Step14UserUID);
                            cmd.Parameters.AddWithValue("@Step14User_TargetDate", Step14User_TargetDate);
                        }

                        if (steps >= 15)
                        {
                            cmd.Parameters.AddWithValue("@Step15UserUID", Step15UserUID);
                            cmd.Parameters.AddWithValue("@Step15User_TargetDate", Step15User_TargetDate);
                        }

                        if (steps >= 16)
                        {
                            cmd.Parameters.AddWithValue("@Step16UserUID", Step16UserUID);
                            cmd.Parameters.AddWithValue("@Step16User_TargetDate", Step16User_TargetDate);
                        }

                        if (steps >= 17)
                        {
                            cmd.Parameters.AddWithValue("@Step17UserUID", Step17UserUID);
                            cmd.Parameters.AddWithValue("@Step17User_TargetDate", Step17User_TargetDate);
                        }

                        if (steps >= 18)
                        {
                            cmd.Parameters.AddWithValue("@Step18UserUID", Step18UserUID);
                            cmd.Parameters.AddWithValue("@Step18User_TargetDate", Step18User_TargetDate);
                        }

                        if (steps >= 19)
                        {
                            cmd.Parameters.AddWithValue("@Step19UserUID", Step19UserUID);
                            cmd.Parameters.AddWithValue("@Step19User_TargetDate", Step19User_TargetDate);
                        }

                        if (steps >= 20)
                        {
                            cmd.Parameters.AddWithValue("@Step20UserUID", Step20UserUID);
                            cmd.Parameters.AddWithValue("@Step20User_TargetDate", Step20User_TargetDate);
                        }


                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        cmd.Parameters.AddWithValue("@ActualDocument_RelativePath", ActualDocument_RelativePath);
                        cmd.Parameters.AddWithValue("@ActualDocument_DirectoryName", ActualDocument_DirectoryName);
                        cmd.Parameters.AddWithValue("@UploadFilePhysicalpath", UploadFilePhysicalpath);
                        if (CoverLetterUID != "")
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@SubmissionType", SubmissionType);
                        cmd.Parameters.AddWithValue("@Steps", steps);
                        //
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@DocVersionUID", DocVersionUID);
                        //
                        con.Open();
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = 0;
            }
        }


        public int InsertorUpdateRefNoHistroy(Guid UID, Guid ActualDocumentUID, string OriginatorReferenceNo, string ONTBRefNo)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateRefNoHistroy"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@OriginatorReferenceNo", OriginatorReferenceNo);
                        cmd.Parameters.AddWithValue("@ONTBRefNo", ONTBRefNo);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        internal int RABill_Document_InsertUpdate(Guid Document_UID, Guid RABillUid, Guid WorkpackageUID, string DocumentPath, string Description, Guid UploadedBy)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_RABill_Document_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Document_UID", Document_UID);
                        cmd.Parameters.AddWithValue("@RABillUid", RABillUid);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@Document_Path", DocumentPath);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@Uploaded_Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", UploadedBy);
                        con.Open();
                        cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                //  return sresult = false;
            }
            return cnt;
        }
        internal string AddRABillNumber(string rabillnumber, Guid WorkpackageUID, DateTime RABill_Date, string RABill_Amount)
        {
            string rabillUid = "";
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_AddRaBill"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@rabillnumber", rabillnumber);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@RABill_Date", RABill_Date);
                        cmd.Parameters.AddWithValue("@RABill_Amount", RABill_Amount);
                        con.Open();
                        rabillUid = Convert.ToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                rabillUid = "Error1";
            }
            return rabillUid;
        }

        //added on 08/08/2022
        public DataSet GetStatusForReconciliation(Guid FlowUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetStatusForReconciliation", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@FlowUID", @FlowUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public string GetFlowUIDBySubmittalUID(Guid SubmittalUID)
        {
            string Type = "";
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetFlowUIDBySubmittalUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SubmittalUID", SubmittalUID);
                Type = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {

                if (con.State == ConnectionState.Open) con.Close();
            }
            return Type;
        }
        public string GetFlowTypeBySubmittalUID(Guid DocumentUID)
        {
            string Type = "";
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetFlowTypeBySubmittalUID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SubmittalUID", DocumentUID);
                Type = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                Type = "Error : " + ex.Message;
                if (con.State == ConnectionState.Open) con.Close();
            }
            return Type;
        }
        public DataTable GetPhaseAndCurrentStatusByDocument(Guid DocumentUID)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("USP_GetPhaseAndCurrentStatusByDocument", con);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }
        public string GetPhaseforStatus(Guid FlowUID, string Current_Status)
        {
            string Type = "";
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_GetPhaseforStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Current_Status", Current_Status);
                cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                Type = (string)cmd.ExecuteScalar();
                con.Close();
            }
            catch (Exception ex)
            {
                Type = "Error : " + ex.Message;
                if (con.State == ConnectionState.Open) con.Close();
            }
            return Type;
        }

        public DataSet GetConsolidatedPMCComments(Guid ActualDocumentUID, string FlowName)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetConsolidatedPMCComments", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                cmd.SelectCommand.Parameters.AddWithValue("@FlowName", FlowName);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 15/09/2022
        public DataSet GetIssueByIssueUid(string issue_uid)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetIssueByIssueUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@issue_uid", issue_uid);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetWorkPackageEmails(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetWorkPackageEmails", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@project_uid", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@workpackage_uid", WorkPackageUID);

                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 15/09/2022 for Saji
        public DataSet GetUploadedIssueDocuments(string issue_uid)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUploadedIssueDocuments", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@issue_uid", issue_uid);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetUploadedDocuments(string issue_remarks_uid)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetUploadedDocuments", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@issue_remarks_uid", issue_remarks_uid);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet GetProjectUIDfromName(string ProjectName)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetProjectUIDfromName", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectName", ProjectName);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataSet getIssue_by_IssueUID(Guid IssueUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetIssueWithIssueUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@IssueUID", IssueUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 30/08/2022
        public int InsertDocumentsAttachments(Guid AttachmentUID, Guid ActualDocumentUID, Guid StatusUID, string AttachmentFileName, string AttachmentFile, Guid CreatedUserUID, DateTime CreatedDate)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlCommand SqlCmd = new SqlCommand("usp_InsertDocumentsAttachments", SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@AttachmentUID", AttachmentUID);
                    SqlCmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                    SqlCmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                    SqlCmd.Parameters.AddWithValue("@AttachmentFileName", AttachmentFileName);
                    SqlCmd.Parameters.AddWithValue("@AttachmentFile", AttachmentFile);
                    SqlCmd.Parameters.AddWithValue("@CreatedUserUID", CreatedUserUID);
                    SqlCmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                    SqlConn.Open();
                    cnt = SqlCmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        //added on 20/09/2022
        public DataSet GetDocumentAttachments(Guid StatusUID)
        {
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDocumentAttachments", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@StatusUID", StatusUID);
                cmd.Fill(ds);

            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        public DataSet GetDocumentAttachment_ByUID(Guid AttachmentUID)
        {
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDocumentAttachment_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@AttachmentUID", AttachmentUID);
                cmd.Fill(ds);

            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        //added on 29/09/2022
        public DataSet GetAdditionFromDesc(string Description)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetAdditionFromDesc", con);
                cmd.SelectCommand.Parameters.AddWithValue("@Description", Description);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int InsertorUpdateInvoiceAdditions(Guid Invoice_AdditionUID, Guid WorkpackageUID, Guid InvoiceMaster_UID, Guid Addition_UID, float Amount, float Percentage, string Currency, string Currency_CultureInfo, string Addition_Mode)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateInvoiceAdditions"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Invoice_AdditionUID", Invoice_AdditionUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@InvoiceMaster_UID", InvoiceMaster_UID);
                        cmd.Parameters.AddWithValue("@Addition_UID", Addition_UID);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@Percentage", Percentage);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Addition_Mode", Addition_Mode);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public DataSet GetInvoiceAdditions_by_InvoiceMaster_UID(Guid InvoiceMaster_UID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetInvoiceAddition_by_InvoiceMaster_UID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@InvoiceMaster_UID", InvoiceMaster_UID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public int GenerateRandomNo()
        {
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public bool IsDocNameExists(string workpackageUID, string FileName)
        {
            bool isExist = false;
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select Count(*) from ActualDocuments where WorkPackageUID='" + workpackageUID + "' and ActualDocument_Name='" + FileName + "' and Doc_Type<>'Cover Letter'";
                int response = (int)cmd.ExecuteScalar();
                if (response > 0)
                    isExist = true;

                con.Close();
            }
            catch (Exception ex)
            {
                isExist = false;
                if (con.State == ConnectionState.Open) con.Close();
            }
            return isExist;
        }

        //added on 1/11/2022 venkats code for logs
        public void processHttpRequest(HttpRequest httpRequest)
        {
            try
            {
                HttpPostedFile httpPostedFile;
                string filePath;

                for (int j = 0; j < httpRequest.Params.Count - 52; j++)
                {
                    requestTransctionInsert(httpRequest.Url.ToString(), httpRequest.Params.Keys[j].ToString() + "---" + httpRequest.Params[j].ToString());
                }
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    httpPostedFile = httpRequest.Files[i];
                    filePath = AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyyMMddhhmmss");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    httpPostedFile.SaveAs(filePath + "\\" + httpPostedFile.FileName);
                    requestTransctionInsert(httpRequest.Url.ToString(), filePath + "\\" + httpPostedFile.FileName);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void requestTransctionInsert(string WebAPI_URL, string Parameters)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("sp_insertRequestLogs"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@requestUrl", WebAPI_URL);
                        cmd.Parameters.AddWithValue("@parameter", Parameters);

                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        public int InsertUploadedIssueDocument(string name, string path, string issue_uid)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("InsertUploadedIssueDocument"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@path", path);
                        cmd.Parameters.AddWithValue("@issue_uid", issue_uid);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }

        public DataSet getDocumentCount_by_ProjectUID_WorkPackageUID2(Guid ProjectUID, Guid WorkPackageUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ups_getDocumentCount_by_ProjectUID_WorkPackageUID_Test1", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.CommandTimeout = 900;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        public DataTable GetDocumentFlow()
        {
            DataTable ds = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDocumentFlow", con);
                    cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                    cmd.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 19th Dec for Venkat

        public DataSet ActualDocuments_SelectBy_WorkpackageUID_Search(Guid ProjectUID, Guid WorkPackageUID, string DocumentName, string Doctype, string SubmittalName, string Status, DateTime DocDate, DateTime DocumentDate, DateTime DocToDate, DateTime DocumentToDate, int Type, string OntbRefNo, string OriginatorRefNo, string FlowMasterUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("ActualDocuments_SelectBy_WorkpackageUID_Search1", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentName", DocumentName);
                cmd.SelectCommand.Parameters.AddWithValue("@Doctype", Doctype);
                cmd.SelectCommand.Parameters.AddWithValue("@SubmittalName", SubmittalName);
                cmd.SelectCommand.Parameters.AddWithValue("@Status", Status);
                cmd.SelectCommand.Parameters.AddWithValue("@Type", Type);
                cmd.SelectCommand.Parameters.AddWithValue("@DocDate", DocDate);// this is incomiung recv date
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentDate", DocumentDate);
                cmd.SelectCommand.Parameters.AddWithValue("@DocDateTo", DocToDate);// this is incomiung recv date
                cmd.SelectCommand.Parameters.AddWithValue("@DocumentDateTo", DocumentToDate);
                cmd.SelectCommand.Parameters.AddWithValue("@OntbRefNumber", OntbRefNo);
                cmd.SelectCommand.Parameters.AddWithValue("@OriginatorRefNumber", OriginatorRefNo);
                cmd.SelectCommand.Parameters.AddWithValue("@FlowUID", FlowMasterUID);
                cmd.SelectCommand.CommandTimeout = 900;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 19th Dec for Venkat

        public DataSet GetAssignedProjects_by_UserUID2(Guid UserUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("GetAssignedProject_by_UserUID2", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@UserUID", UserUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }



        //added on 21/12/2022 for Venkat

        public DataSet GetFlow2Olddocs(Guid ProjectUID, String Type)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetFlow2Olddocs", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.Parameters.AddWithValue("@Type", Type);
                cmd.SelectCommand.CommandTimeout = 900;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 21/12/2022 for Venkat
        public int ReplaceDocsFlow2old_API(Guid ActualDocumentUID, string ActualDocument_Name, string ActualDocument_Path, string CoverLetter_Name, string CoverLetter_Path, DateTime DocumentDate, string refNumber)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlCommand SqlCmd = new SqlCommand("usp_ReplaceDocsFlow2old_API", SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@ActualDocument_Name", ActualDocument_Name);
                    SqlCmd.Parameters.AddWithValue("@ActualDocument_Path", ActualDocument_Path);
                    SqlCmd.Parameters.AddWithValue("@CoverLetter_Name", CoverLetter_Name);
                    SqlCmd.Parameters.AddWithValue("@CoverLetter_Path", CoverLetter_Path);
                    SqlCmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                    SqlCmd.Parameters.AddWithValue("@DocumentDate", DocumentDate);
                    SqlCmd.Parameters.AddWithValue("@OriginatorRefNo", refNumber);
                    SqlCmd.CommandTimeout = 900;
                    SqlConn.Open();
                    cnt = SqlCmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        //added on 12/01/2023
        public int InsertIntoCorrespondenceCCToUsers(Guid UID, Guid ActualDocumentUID, Guid StatusUID, string UserType)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection SqlConn = new SqlConnection(db.GetConnectionString()))
                {
                    SqlCommand SqlCmd = new SqlCommand("usp_InsertIntoCorrespondenceCCToUsers", SqlConn);
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@UID", UID);
                    SqlCmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                    SqlCmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                    SqlCmd.Parameters.AddWithValue("@UserType", UserType);
                    SqlConn.Open();
                    cnt = SqlCmd.ExecuteNonQuery();
                    SqlConn.Close();
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        //added on 18/01/2023
        public bool CheckCCtoUser(Guid StatusUID, string Type)
        {
            bool result = false;
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand("usp_CheckCCtoUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                cmd.Parameters.AddWithValue("@Type ", Type);
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                    result = true;
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                result = false;
            }
            return result;
        }

        //added on 14/02/2023 for venkat
        internal DataTable getAndroidCameraDetails(Guid ProjectUID)
        {
            DataTable ds = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetAndriodCameraDetails", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                cmd.SelectCommand.CommandTimeout = 900;
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 16/05/2023 for blob
        public string getProjectConnectionString(Guid ProjectUID)
        {
            string Connectionstring = string.Empty;
            try
            {
                DataSet dsstring = GetProjectdb(ProjectUID);
                if (dsstring.Tables[0].Rows.Count > 0)
                {
                    Connectionstring = dsstring.Tables[0].Rows[0]["DbConnectionString"].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return Connectionstring;
        }

        public DataSet GetProjectdb(Guid ProjectUID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetProjectdb", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ProjectUID", ProjectUID);

                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;
            }
            return ds;
        }

        //added on 16/05/2023
        public byte[] FileToByteArray(string fileName)
        {
            byte[] fileData = null;

            using (FileStream fs = File.OpenRead(fileName))
            {
                var binaryReader = new BinaryReader(fs);
                fileData = binaryReader.ReadBytes((int)fs.Length);
            }
            return fileData;
        }

        public int InsertActualDocumentsBlob(Guid ActualDocumentUID, byte[] Blob_Data, string ConnectionString)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertActualDocumentsBlob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID ", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@Blob_Data", Blob_Data);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public int InsertDocumentStatusBlob_FirstTime(Guid StatusUID, Guid ActualDocumentUID, Guid CoverLetterUID, string ConnectionString)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertDocumentStatusBlob_FirstTime"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public int InsertDocumentVersionBlob(Guid DocVersionUID, Guid DocumentUID, byte[] Blob_Data, byte[] CoverLetter_Blob, string ConnectionString)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertDocumentVersionBlob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocVersionUID", DocVersionUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@Blob_Data", Blob_Data);
                        if (CoverLetter_Blob != null && CoverLetter_Blob.Length > 0)
                        {
                            cmd.Parameters.AddWithValue("@CoverLetter_Blob", CoverLetter_Blob);
                        }
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        public int InsertDocumentsAttachmentsBlob(Guid AttachmentUID, Guid ActualDocumentUID, Guid StatusUID, byte[] Blob_Data, string ConnectionString)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertDocumentsAttachmentsBlob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AttachmentUID", AttachmentUID);
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@BlobData", Blob_Data);
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        //added on 16/05/2023
        public DataSet GetReplaceDocsFlow2old_UIDs_ForBlob(Guid ActualDocumentUID)
        {
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(db.GetConnectionString());
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetReplaceDocsFlow2old_UIDs_ForBlob", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ActualDocumentUID ", ActualDocumentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        public DataSet ReplaceDocsFlow2old_Blob(Guid ActualDocumentUID, Guid CoverLetterUID, Guid StatusUID, Guid DocVersionUID, string ConnectionString, byte[] ActualDocument_Blob, byte[] CoverLetter_Blob)
        {
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter("usp_ReplaceDocsFlow2old_Blob", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                cmd.SelectCommand.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                cmd.SelectCommand.Parameters.AddWithValue("@StatusUID", StatusUID);
                cmd.SelectCommand.Parameters.AddWithValue("@DocVersionUID", DocVersionUID);
                cmd.SelectCommand.Parameters.AddWithValue("@ActualDocument_Blob", ActualDocument_Blob);
                cmd.SelectCommand.Parameters.AddWithValue("@CoverLetter_Blob", CoverLetter_Blob);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        //added on 17/05/2023
        public int InsertDocumentStatusBlob(Guid StatusUID, Guid ActualDocumentUID, byte[] CoverLetterBlob, byte[] ReviewFileBlob_Data, string ConnectionString)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertDocumentStatusBlob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@StatusUID ", StatusUID);
                        cmd.Parameters.AddWithValue("@ActualDocumentUID ", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@CoverLetterBlob", CoverLetterBlob);
                        if (ReviewFileBlob_Data != null && ReviewFileBlob_Data.Length > 0)
                        {
                            cmd.Parameters.AddWithValue("@ReviewFileBlob_Data", ReviewFileBlob_Data);
                        }
                        con.Open();
                        cnt = cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return cnt;
            }
            catch (Exception ex)
            {
                return cnt;
            }
        }

        //added on 18/05/2023
        public DataSet GetDocumentStatusBlob_by_StatusUID(Guid StatusUID, string ConnectionString)
        {
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetDocumentStatusBlob_by_StatusUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@StatusUID", StatusUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        //added on 14/04/2023
        public DataSet GetAttachmentBlob_by_attachmentUID(Guid AttachmentUID, string ConnectionString)
        {
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {
                SqlDataAdapter cmd = new SqlDataAdapter("usp_GetAttachmentBlob_by_attachmentUID", con);
                cmd.SelectCommand.CommandType = CommandType.StoredProcedure;
                cmd.SelectCommand.Parameters.AddWithValue("@AttachmentUID", AttachmentUID);
                cmd.Fill(ds);
            }
            catch (Exception ex)
            {
                ds = null;

            }
            return ds;
        }

        //added on 19/05/2023
        public byte[] DownloadIssueDocument(int issue_doc_uid, out string file_name)
        {

            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());

                SqlDataReader sdr = null;

                byte[] file_in_bytes = null;


                using (SqlCommand cmd = new SqlCommand("DownloadIssueDoc"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", issue_doc_uid);
                    cmd.Connection = con;
                    con.Open();
                    sdr = cmd.ExecuteReader();
                    sdr.Read();
                    file_in_bytes = (byte[])sdr["DocBlob"];
                    file_name = sdr["docPath"].ToString();

                    con.Close();
                }

                return file_in_bytes;
            }
            catch (Exception ex)
            {
                file_name = "";
                return null;
            }
        }

        public byte[] DownloadDocument(string issue_remarks_doc_uid, out string file_name)
        {

            try
            {
                SqlConnection con = new SqlConnection(db.GetConnectionString());

                SqlDataReader sdr = null;

                byte[] file_in_bytes = null;

                using (SqlCommand cmd = new SqlCommand("DownloadIssueRemarksDoc"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", issue_remarks_doc_uid);
                    cmd.Connection = con;
                    con.Open();
                    sdr = cmd.ExecuteReader();
                    sdr.Read();
                    file_in_bytes = (byte[])sdr["DocBlob"];
                    file_name = sdr["DocPath"].ToString();

                    con.Close();
                }

                return file_in_bytes;
            }
            catch (Exception ex)
            {
                file_name = "";
                return null;
            }
        }

        public int InsertUploadedInsuranceDocumentBlob(Guid InsuranceDocBlobUID, string insurance_doc_uid, byte[] docBytes, string flName, string flPath)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("InsertUploadedInsuranceDocumentBlob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@docName", flName);
                        cmd.Parameters.AddWithValue("@docPath", flPath);
                        cmd.Parameters.AddWithValue("@InsuranceBlobUID", InsuranceDocBlobUID);
                        cmd.Parameters.AddWithValue("@insurance_doc_uid", insurance_doc_uid);
                        cmd.Parameters.AddWithValue("@docBytes", docBytes);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }

        //added on 22/05/2023
        public int InsertUploadedBankDocumentBlob(Guid BankDocBlobUID, string bank_doc_uid, byte[] docBytes, string flName, string flPath)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("InsertUploadedBankDocumentBlob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@docName", flName);
                        cmd.Parameters.AddWithValue("@docPath", flPath);
                        cmd.Parameters.AddWithValue("@BankBlobUID", BankDocBlobUID);
                        cmd.Parameters.AddWithValue("@bank_doc_uid", bank_doc_uid);
                        cmd.Parameters.AddWithValue("@docBytes", docBytes);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }

        internal bool UpdatePremiumReceipt(string insurancePremiumUID, string receiptPath, byte[] RBlob)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_UpdatePremiumReceipt"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@insurancePremiumUID", insurancePremiumUID);
                        cmd.Parameters.AddWithValue("@receiptPath", receiptPath);
                        cmd.Parameters.AddWithValue("@docBlob", RBlob);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        internal int RABill_Document_InsertUpdate(Guid Document_UID, Guid RABillUid, Guid WorkpackageUID, string DocumentPath, string Description, Guid UploadedBy, byte[] billBlob)
        {
            int cnt = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_RABill_Document_InsertorUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Document_UID", Document_UID);
                        cmd.Parameters.AddWithValue("@RABillUid", RABillUid);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@Document_Path", DocumentPath);
                        cmd.Parameters.AddWithValue("@DocBlob", billBlob);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@Uploaded_Date", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", UploadedBy);

                        con.Open();
                        cnt = Convert.ToInt32(cmd.ExecuteNonQuery());
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                //  return sresult = false;
            }
            return cnt;
        }

        public Boolean InsertorUpdateInsurancePremium(Guid PremiumUID, Guid InsuranceUID, float Premium_Paid, float Interest, float Penalty, DateTime Premium_PaidDate, DateTime Premium_DueDate, DateTime Next_PremiumDate, string Premium_Receipt, string Remarks, byte[] receipt_blob)
        {
            Boolean sresult = false;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("usp_InsertorUpdateInsurancePremium"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@PremiumUID", PremiumUID);
                        cmd.Parameters.AddWithValue("@InsuranceUID", InsuranceUID);
                        cmd.Parameters.AddWithValue("@Premium_Paid", Premium_Paid);
                        cmd.Parameters.AddWithValue("@Interest", Interest);
                        cmd.Parameters.AddWithValue("@Penalty", Penalty);
                        cmd.Parameters.AddWithValue("@Premium_PaidDate", Premium_PaidDate);
                        cmd.Parameters.AddWithValue("@Premium_DueDate", Premium_DueDate);
                        cmd.Parameters.AddWithValue("@Next_PremiumDate", Next_PremiumDate);
                        cmd.Parameters.AddWithValue("@Premium_Receipt", Premium_Receipt);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@BlobData", receipt_blob);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        sresult = true;
                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult = false;
            }
        }

        public int UpdateUploadedIssueDocument_Blob(int doc_id, byte[] doc_blob)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("UpdateUploadedIssueDocument_Blob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@doc_id", doc_id);
                        cmd.Parameters.AddWithValue("@docBlob", doc_blob);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }

        public int UpdateUploadedIssueReamrksDocument_Blob(int doc_id, byte[] doc_blob)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("UpdateUploadedIssueReamrksDocument_Blob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@doc_id", doc_id);
                        cmd.Parameters.AddWithValue("@docBlob", doc_blob);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }

        public int UpdateUploadedRABillDocument_Blob(Guid doc_id, byte[] doc_blob)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("UpdateUploadedRABillDocument_Blob"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@doc_id", doc_id);
                        cmd.Parameters.AddWithValue("@docBlob", doc_blob);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }

        //added on 15/06/2023 for saji
        public int InsertUploadedIssueDocument(string name, string path, string issue_uid, byte[] doc_blob)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("InsertUploadedIssueDocument"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@path", path);
                        cmd.Parameters.AddWithValue("@issue_uid", issue_uid);
                        cmd.Parameters.AddWithValue("@docBlob", doc_blob);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }


        public int InsertUploadedDocument(string name, string path, string issue_remarks_uid, byte[] doc_blob)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("InsertUploadedDocument"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@path", path);
                        cmd.Parameters.AddWithValue("@issue_remarks_uid", issue_remarks_uid);
                        cmd.Parameters.AddWithValue("@docBlob", doc_blob);
                        sresult = (int)cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
                return sresult;
            }
            catch (Exception ex)
            {
                return sresult;
            }
        }

    }
}

