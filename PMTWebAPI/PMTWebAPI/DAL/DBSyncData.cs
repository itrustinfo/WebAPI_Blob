using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PMTWebAPI.DAL
{
    public class DBSyncData
    {
        DBUtility db = new DBUtility();

        public int Submittal_Insert_or_Update_Flow(Guid DocumentUID, Guid WorkPackageUID, Guid ProjectUID, Guid TaskUID, string DocName,
          Guid Doc_Category, string Doc_RefNumber, string Doc_Type, double Doc_Budget, Guid FlowUID, DateTime Flow_StartDate,
          Guid FlowStep1_UserUID, DateTime FlowStep1_TargetDate, string FlowStep2_UserUID, string FlowStep2_TargetDate,
          string FlowStep3_UserUID, string FlowStep3_TargetDate, string FlowStep4_UserUID, string FlowStep4_TargetDate, string FlowStep5_UserUID, string FlowStep5_TargetDate, int EstimatedDocuments, string Remarks, string DocumentSearchType, DateTime CreatedDate, string DeletedFlag,string IsSync,
          string FlowStep6_UserUID, string FlowStep6_TargetDate,
          string FlowStep7_UserUID, string FlowStep7_TargetDate,
          string FlowStep8_UserUID, string FlowStep8_TargetDate,
          string FlowStep9_UserUID, string FlowStep9_TargetDate,
          string FlowStep10_UserUID, string FlowStep10_TargetDate,
          string FlowStep11_UserUID, string FlowStep11_TargetDate,
          string FlowStep12_UserUID, string FlowStep12_TargetDate,
          string FlowStep13_UserUID, string FlowStep13_TargetDate,
          string FlowStep14_UserUID, string FlowStep14_TargetDate,
          string FlowStep15_UserUID, string FlowStep15_TargetDate,
          string FlowStep16_UserUID, string FlowStep16_TargetDate,
          string FlowStep17_UserUID, string FlowStep17_TargetDate,
          string FlowStep18_UserUID, string FlowStep18_TargetDate,
          string FlowStep19_UserUID, string FlowStep19_TargetDate,
          string FlowStep20_UserUID, string FlowStep20_TargetDate,
          string FlowStep1_IsMUser, string FlowStep2_IsMUser, string FlowStep3_IsMUser,
          string FlowStep4_IsMUser, string FlowStep5_IsMUser, string FlowStep6_IsMUser,
          string FlowStep7_IsMUser, string FlowStep8_IsMUser, string FlowStep9_IsMUser,
          string FlowStep10_IsMUser, string FlowStep11_IsMUser, string FlowStep12_IsMUser,
          string FlowStep13_IsMUser, string FlowStep14_IsMUser, string FlowStep15_IsMUser,
          string FlowStep16_IsMUser, string FlowStep17_IsMUser, string FlowStep18_IsMUser,
          string FlowStep19_IsMUser, string FlowStep20_IsMUser)
        {

            int sresult = 0;
            try

            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdateDocuments"))
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
                        if (!string.IsNullOrEmpty(FlowStep2_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep2_UserUID", new Guid(FlowStep2_UserUID));
                            cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", Convert.ToDateTime(FlowStep2_TargetDate));
                        }
                        if (!string.IsNullOrEmpty(FlowStep3_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep3_UserUID", FlowStep3_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep4_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep4_UserUID", FlowStep4_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep4_TargetDate", FlowStep4_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep5_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep5_UserUID", FlowStep5_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep5_TargetDate", FlowStep5_TargetDate);
                        }
                        cmd.Parameters.AddWithValue("@EstimatedDocuments", EstimatedDocuments);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@DocumentSearchType", DocumentSearchType);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
                        cmd.Parameters.AddWithValue("@IsSync", IsSync);
                        if (!string.IsNullOrEmpty(FlowStep6_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep6_UserUID", FlowStep6_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep6_TargetDate", FlowStep6_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep7_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep7_UserUID", FlowStep7_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep7_TargetDate", FlowStep7_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep8_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep8_UserUID", FlowStep8_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep8_TargetDate", FlowStep8_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep9_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep9_UserUID", FlowStep9_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep9_TargetDate", FlowStep9_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep10_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep10_UserUID", FlowStep10_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep10_TargetDate", FlowStep10_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep11_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep11_UserUID", FlowStep11_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep11_TargetDate", FlowStep11_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep12_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep12_UserUID", FlowStep12_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep12_TargetDate", FlowStep12_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep13_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep13_UserUID", FlowStep13_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep13_TargetDate", FlowStep13_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep14_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep14_UserUID", FlowStep14_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep14_TargetDate", FlowStep14_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep15_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep15_UserUID", FlowStep15_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep15_TargetDate", FlowStep15_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep16_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep16_UserUID", FlowStep16_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep16_TargetDate", FlowStep16_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep17_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep17_UserUID", FlowStep17_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep17_TargetDate", FlowStep17_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep18_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep18_UserUID", FlowStep18_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep18_TargetDate", FlowStep18_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep19_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep19_UserUID", FlowStep19_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep19_TargetDate", FlowStep19_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep20_UserUID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep20_UserUID", FlowStep20_UserUID);
                            cmd.Parameters.AddWithValue("@FlowStep20_TargetDate", FlowStep20_TargetDate);
                        }
                        cmd.Parameters.AddWithValue("@FlowStep1_IsMUser", FlowStep1_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep2_IsMUser", FlowStep2_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep3_IsMUser", FlowStep3_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep4_IsMUser", FlowStep4_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep5_IsMUser", FlowStep5_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep6_IsMUser", FlowStep6_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep7_IsMUser", FlowStep7_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep8_IsMUser", FlowStep8_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep9_IsMUser", FlowStep9_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep10_IsMUser", FlowStep10_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep11_IsMUser", FlowStep11_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep12_IsMUser", FlowStep12_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep13_IsMUser", FlowStep13_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep14_IsMUser", FlowStep14_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep15_IsMUser", FlowStep15_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep16_IsMUser", FlowStep16_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep17_IsMUser", FlowStep17_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep18_IsMUser", FlowStep18_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep19_IsMUser", FlowStep19_IsMUser);
                        cmd.Parameters.AddWithValue("@FlowStep20_IsMUser", FlowStep20_IsMUser);
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

       


     

       

        public int Document_Insert_or_Update_ActualDocuments(Guid ActualDocumentUID, Guid ProjectUID, Guid WorkPackageUID, Guid DocumentUID, string ProjectRef_Number,
         string Ref_Number, string Doc_Type, DateTime IncomingRec_Date, Guid FlowUID, string ActualDocument_Name, string Description, double ActualDocument_Version, string ActualDocument_Type,
         string Media_HC, string Media_SC, string Media_SCEF, string Media_HCR, string Media_SCR, string Media_NA, string ActualDocument_Path, string Remarks,
         string FileRef_Number, string ActualDocument_CurrentStatus,
          string FlowStep1_TargetDate, string FlowStep2_TargetDate, string FlowStep3_TargetDate, string FlowStep4_TargetDate, string FlowStep5_TargetDate, string ActualDocument_Originator, string Document_Date, string ActualDocument_RelativePath, string ActualDocument_DirectoryName, string ActualDocument_CreatedDate, string Delete_Flag,string CoverLetterUID,string SubmissionType,
          string FlowStep6_TargetDate, string FlowStep7_TargetDate, string FlowStep8_TargetDate, string FlowStep9_TargetDate, string FlowStep10_TargetDate,
          string FlowStep11_TargetDate, string FlowStep12_TargetDate, string FlowStep13_TargetDate, string FlowStep14_TargetDate,
          string FlowStep15_TargetDate, string FlowStep16_TargetDate, string FlowStep17_TargetDate, string FlowStep18_TargetDate,
          string FlowStep19_TargetDate, string FlowStep20_TargetDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdate_ActualDocuments"))
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
                        if (!string.IsNullOrEmpty(FlowStep1_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep2_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep3_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep4_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep4_TargetDate", FlowStep4_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep5_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep5_TargetDate", FlowStep5_TargetDate);

                        }
                        cmd.Parameters.AddWithValue("@ActualDocument_Originator", ActualDocument_Originator);
                        if (!string.IsNullOrEmpty(Document_Date))
                        {
                            cmd.Parameters.AddWithValue("@Document_Date", Document_Date);
                        }

                        cmd.Parameters.AddWithValue("@ActualDocument_RelativePath", ActualDocument_RelativePath);
                        cmd.Parameters.AddWithValue("@ActualDocument_DirectoryName", ActualDocument_DirectoryName);

                        cmd.Parameters.AddWithValue("@ActualDocument_CreatedDate", ActualDocument_CreatedDate);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
                        if (!string.IsNullOrEmpty(CoverLetterUID))
                        {
                            cmd.Parameters.AddWithValue("@CoverLetterUID", CoverLetterUID);
                        }
                        cmd.Parameters.AddWithValue("@SubmissionType", SubmissionType);
                        if (!string.IsNullOrEmpty(FlowStep6_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep6_TargetDate", FlowStep6_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep7_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep7_TargetDate", FlowStep7_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep8_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep8_TargetDate", FlowStep8_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep9_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep9_TargetDate", FlowStep9_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep10_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep10_TargetDate", FlowStep10_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep11_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep11_TargetDate", FlowStep11_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep12_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep12_TargetDate", FlowStep12_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep13_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep13_TargetDate", FlowStep13_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep14_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep14_TargetDate", FlowStep14_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep15_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep15_TargetDate", FlowStep15_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep16_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep16_TargetDate", FlowStep16_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep17_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep17_TargetDate", FlowStep17_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep18_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep18_TargetDate", FlowStep18_TargetDate);

                        }
                        if (!string.IsNullOrEmpty(FlowStep19_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep19_TargetDate", FlowStep19_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep20_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep20_TargetDate", FlowStep20_TargetDate);

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

        

       

        public int InsertorUpdateDocumentStatus(Guid StatusUID, Guid DocumentUID, double Version, string ActivityType, string Activity_Budget, DateTime ActivityDate,
          string LinkToReviewFile, Guid AcivityUserUID, string Status_Comments, string Current_Status, string Ref_Number, string DocumentDate, string CoverLetterFile, string Delete_Flag,string Origin,string CreatedDate,string Forwarded)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_DocumentStatus"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@Version", Version);
                        cmd.Parameters.AddWithValue("@ActivityType", ActivityType);

                        cmd.Parameters.AddWithValue("@ActivityDate", ActivityDate);
                        cmd.Parameters.AddWithValue("@LinkToReviewFile", LinkToReviewFile);
                        cmd.Parameters.AddWithValue("@AcivityUserUID", AcivityUserUID);
                        cmd.Parameters.AddWithValue("@Status_Comments", Status_Comments);
                        cmd.Parameters.AddWithValue("@Current_Status", Current_Status);
                        if (!string.IsNullOrEmpty(Ref_Number))
                        {
                            cmd.Parameters.AddWithValue("@Ref_Number", Ref_Number);
                        }
                        if (!string.IsNullOrEmpty(Activity_Budget))
                        {
                            cmd.Parameters.AddWithValue("@Activity_Budget", double.Parse(Activity_Budget));
                        }
                        if (!string.IsNullOrEmpty(DocumentDate))
                        {
                            cmd.Parameters.AddWithValue("@DocumentDate", DocumentDate);
                        }

                        cmd.Parameters.AddWithValue("@CoverLetterFile", CoverLetterFile);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        if (!string.IsNullOrEmpty(CreatedDate))
                        {
                            cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        }
                        cmd.Parameters.AddWithValue("@Forwarded", Forwarded);
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

        public int InsertDocumentorUpdateVersion(Guid DocVersion_UID, Guid DocStatus_UID, Guid DocumentUID, string Doc_Type, string Doc_FileName, string Doc_Comments, int Doc_Version, string Doc_Status, string Doc_StatusDate, string Delete_Flag,string Doc_CoverLetter)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_DocumentVersion"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocVersion_UID", DocVersion_UID);
                        cmd.Parameters.AddWithValue("@DocStatus_UID", DocStatus_UID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@Doc_Version", Doc_Version);
                        cmd.Parameters.AddWithValue("@Doc_Type", Doc_Type);
                        cmd.Parameters.AddWithValue("@Doc_FileName", Doc_FileName);
                        cmd.Parameters.AddWithValue("@Doc_Status", Doc_Status);
                        cmd.Parameters.AddWithValue("@Doc_StatusDate", Doc_StatusDate);
                        cmd.Parameters.AddWithValue("@Doc_Comments", Doc_Comments);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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


      


        public int InsertorUpdateDocumentflowdata(Guid DocumentFlow_UID, Guid FlowMasterUID, Guid DocumentUID, Guid FlowStep1_UserID, Guid FlowStep1_UserRole, DateTime FlowStep1_TargetDate, string FlowStep1_DisplayName,
            string FlowStep2_UserID, string FlowStep2_UserRole, string FlowStep2_TargetDate, string FlowStep2_DisplayName,
            string FlowStep3_UserID, string FlowStep3_UserRole, string FlowStep3_TargetDate, string FlowStep3_DisplayName,
            string FlowStep4_UserID, string FlowStep4_UserRole, string FlowStep4_TargetDate, string FlowStep4_DisplayName,
            string FlowStep5_UserID, string FlowStep5_UserRole, string FlowStep5_TargetDate, string FlowStep5_DisplayName,
            string FlowStep6_UserID, string FlowStep6_UserRole, string FlowStep6_TargetDate, string FlowStep6_DisplayName, string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_Documentflowdata"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentFlow_UID", DocumentFlow_UID);
                        cmd.Parameters.AddWithValue("@FlowMasterUID", FlowMasterUID);
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        
                        cmd.Parameters.AddWithValue("@FlowStep1_UserID", FlowStep1_UserID);
                        cmd.Parameters.AddWithValue("@FlowStep1_UserRole", FlowStep1_UserRole);
                        cmd.Parameters.AddWithValue("@FlowStep1_TargetDate", FlowStep1_TargetDate);
                        cmd.Parameters.AddWithValue("@FlowStep1_DisplayName", FlowStep1_DisplayName);
                        if (!string.IsNullOrEmpty(FlowStep2_UserID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep2_UserID", FlowStep2_UserID);
                        }
                        if (!string.IsNullOrEmpty(FlowStep2_UserRole))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep2_UserRole", FlowStep2_UserRole);
                        }
                        if (!string.IsNullOrEmpty(FlowStep2_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep2_TargetDate", FlowStep2_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep2_DisplayName))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep2_DisplayName", FlowStep2_DisplayName);
                        }
                        //
                        if (!string.IsNullOrEmpty(FlowStep3_UserID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep3_UserID", FlowStep3_UserID);
                        }
                        if (!string.IsNullOrEmpty(FlowStep3_UserRole))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep3_UserRole", FlowStep3_UserRole);
                        }
                        if (!string.IsNullOrEmpty(FlowStep3_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep3_TargetDate", FlowStep3_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep3_DisplayName))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep3_DisplayName", FlowStep3_DisplayName);
                        }
                        //
                        if (!string.IsNullOrEmpty(FlowStep4_UserID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep4_UserID", FlowStep4_UserID);
                        }
                        if (!string.IsNullOrEmpty(FlowStep4_UserRole))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep4_UserRole", FlowStep4_UserRole);
                        }
                        if (!string.IsNullOrEmpty(FlowStep4_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep4_TargetDate", FlowStep4_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep4_DisplayName))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep4_DisplayName", FlowStep4_DisplayName);
                        }
                        //
                        if (!string.IsNullOrEmpty(FlowStep5_UserID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep5_UserID", FlowStep5_UserID);
                        }
                        if (!string.IsNullOrEmpty(FlowStep5_UserRole))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep5_UserRole", FlowStep5_UserRole);
                        }
                        if (!string.IsNullOrEmpty(FlowStep5_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep5_TargetDate", FlowStep5_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep5_DisplayName))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep5_DisplayName", FlowStep5_DisplayName);
                        }
                        //
                        if (!string.IsNullOrEmpty(FlowStep6_UserID))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep6_UserID", FlowStep6_UserID);
                        }
                        if (!string.IsNullOrEmpty(FlowStep6_UserRole))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep6_UserRole", FlowStep6_UserRole);
                        }
                        if (!string.IsNullOrEmpty(FlowStep6_TargetDate))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep6_TargetDate", FlowStep6_TargetDate);
                        }
                        if (!string.IsNullOrEmpty(FlowStep6_DisplayName))
                        {
                            cmd.Parameters.AddWithValue("@FlowStep6_DisplayName", FlowStep6_DisplayName);
                        }
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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


        public int InsertWordocRead(Guid UID, string Doc_path, string Status,string HTML_Text, string CreatedDate, Guid DocumemtUID, string Encrypted)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertWordocRead"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@Doc_path", Doc_path);
                        cmd.Parameters.AddWithValue("@Status", "Pending");
                        cmd.Parameters.AddWithValue("@HTML_Text", "");
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
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


        public int InsertDocumentsUploadLog(Guid ActualDocumentUID, string UploadStartDate, string UploadEndDate, Guid UploadUserUID, string Duration)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertDocumentsUploadLog"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@UploadStartDate", UploadStartDate);
                        cmd.Parameters.AddWithValue("@UploadEndDate", UploadEndDate);
                        cmd.Parameters.AddWithValue("@UploadUserUID", UploadUserUID);
                        cmd.Parameters.AddWithValue("@Duration", Duration);
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

        //public int InsertOrUpdateRABill_Abstract(Guid RABillUid, Guid WorkpackageUID, string RABillNumber, string RABill_Date, string DeleteFlag)
        //{
        //    int sresult = 0;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
        //        {

        //            using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdateRABill_Abstract"))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Connection = con;
        //                cmd.Parameters.AddWithValue("@RABillUid", RABillUid);
        //                cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
        //                cmd.Parameters.AddWithValue("@RABillNumber", RABillNumber);
        //                cmd.Parameters.AddWithValue("@RABill_Date", RABill_Date);
        //                cmd.Parameters.AddWithValue("@DeleteFlag", DeleteFlag);
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


        public int InsertOrUpdateRABills(Guid itemUId, Guid ProjectUID, Guid WorkpackageUID, string item_number,string item_desc,decimal current_cost,decimal cumulative_cost,string created_date,Guid RABillUid,Guid UID,string DeleteFlag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_RABills"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@itemUId", itemUId);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@item_number", item_number);
                        cmd.Parameters.AddWithValue("@item_desc", item_desc);
                        cmd.Parameters.AddWithValue("@current_cost", current_cost);
                        cmd.Parameters.AddWithValue("@cumulative_cost", cumulative_cost);
                        cmd.Parameters.AddWithValue("@created_date", created_date);
                        cmd.Parameters.AddWithValue("@RABillUid", RABillUid);
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@DeleteFlag", DeleteFlag);
                        
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


        public int InsertOrUpdateDeductionsMaster(Guid UID, string DeductionsDescription, decimal Maxpercentage,Int16 Order_By)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_DeductionsMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@DeductionsDescription", DeductionsDescription);
                        cmd.Parameters.AddWithValue("@Maxpercentage", Maxpercentage);
                        cmd.Parameters.AddWithValue("@Order_By", Order_By);
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

        public int InsertorUpdate_InvoiceMaster(Guid InvoiceMaster_UID, Guid ProjectUID,Guid WorkpackageUID, string Invoice_Number, string Invoice_Desc,string Invoice_Date,decimal Invoice_TotalAmount,decimal Invoice_DeductionAmount,decimal Invoice_NetAmount,string Currency,string Currency_CultureInfo,string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_InvoiceMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@InvoiceMaster_UID", InvoiceMaster_UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@Invoice_Number", Invoice_Number);
                        cmd.Parameters.AddWithValue("@Invoice_Desc", Invoice_Desc);
                        cmd.Parameters.AddWithValue("@Invoice_Date", Invoice_Date);
                        cmd.Parameters.AddWithValue("@Invoice_TotalAmount", Invoice_TotalAmount);
                        cmd.Parameters.AddWithValue("@Invoice_DeductionAmount", Invoice_DeductionAmount);
                        cmd.Parameters.AddWithValue("@Invoice_NetAmount", Invoice_NetAmount);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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

        public int InsertorUpdate_InvoiceRABills(Guid InvoiceRABill_UID, Guid InvoiceMaster_UID, Guid RABillUid, string InvoiceRABill_Date,string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_InvoiceRABills"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@InvoiceRABill_UID", InvoiceRABill_UID);
                        cmd.Parameters.AddWithValue("@InvoiceMaster_UID", InvoiceMaster_UID);
                        cmd.Parameters.AddWithValue("@RABillUid", RABillUid);
                        cmd.Parameters.AddWithValue("@InvoiceRABill_Date", InvoiceRABill_Date);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
                       
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

        public int InsertorUpdate_InvoiceDeduction(Guid Invoice_DeductionUID, Guid WorkpackageUID, Guid InvoiceMaster_UID, Guid Deduction_UID, decimal Amount,string Currency,string Currency_CultureInfo,float Percentage,string Deduction_Mode,Int16 Order_By,string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_InvoiceDeduction"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Invoice_DeductionUID", Invoice_DeductionUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@InvoiceMaster_UID", InvoiceMaster_UID);
                        cmd.Parameters.AddWithValue("@Deduction_UID", Deduction_UID);
                        cmd.Parameters.AddWithValue("@Amount", Amount);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Percentage", Percentage);
                        cmd.Parameters.AddWithValue("@Deduction_Mode", Deduction_Mode);
                        cmd.Parameters.AddWithValue("@Order_By", Order_By);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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

        public int InsertorUpdate_AssignJointInspectiontoRAbill(Guid AssignJointInspectionUID, Guid RABill_UID, Guid RABill_ItemUID, Guid InspectionUID, string Assign_Date)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdate_AssignJointInspectiontoRAbill"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AssignJointInspectionUID", AssignJointInspectionUID);
                        cmd.Parameters.AddWithValue("@RABill_UID", RABill_UID);
                        cmd.Parameters.AddWithValue("@RABill_ItemUID", RABill_ItemUID);
                        cmd.Parameters.AddWithValue("@InspectionUID", InspectionUID);
                        cmd.Parameters.AddWithValue("@Assign_Date", Assign_Date);

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

        public int GetStatusDocCount(Guid WorkPackageUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_GetStatusDocCount"))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        SqlParameter parmOUT = new SqlParameter("@Count", SqlDbType.Int);
                        parmOUT.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(parmOUT);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        int returnVALUE = (int)cmd.Parameters["@Count"].Value;
                        sresult = returnVALUE;
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

        public int InsertOrUpdateDocCount(Guid ProjectUID, Guid WorkPackageUID, Int64 SourceDocCount, Int64 DestDocCount)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdateDocCount"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@SourceDocCount", SourceDocCount);
                        cmd.Parameters.AddWithValue("@DestDocCount", DestDocCount);
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

        public int InsertOrUpdate_UserDetails(Guid UserUID, string FirstName, string LastName, string EmailID, string Phonenumber, string Mobilenumber, string Address1, string Address2, string Username, string password,string CreatedDate,string DeletedFlag,string DeletedDate,string TypeOfUser,Guid Admin_Under,string Project_Under,string Profile_Pic)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdate_UserDetails"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@EmailID", EmailID);
                        if (!string.IsNullOrEmpty(Phonenumber))
                        {
                            cmd.Parameters.AddWithValue("@Phonenumber", Phonenumber);
                        }
                        cmd.Parameters.AddWithValue("@Mobilenumber", Mobilenumber);
                        cmd.Parameters.AddWithValue("@Address1", Address1);
                        cmd.Parameters.AddWithValue("@Address2", Address2);

                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
                        if (!string.IsNullOrEmpty(DeletedDate))
                        {
                            cmd.Parameters.AddWithValue("@DeletedDate", DeletedDate);
                        }
                        cmd.Parameters.AddWithValue("@TypeOfUser", TypeOfUser);
                        cmd.Parameters.AddWithValue("@Admin_Under", Admin_Under);
                        if (!string.IsNullOrEmpty(Project_Under))
                        {
                            cmd.Parameters.AddWithValue("@Project_Under", new Guid(Project_Under));
                        }
                        if (!string.IsNullOrEmpty(Profile_Pic))
                        {
                            cmd.Parameters.AddWithValue("@Profile_Pic", Profile_Pic);
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

        public int InsertOrUpdate_UserProjects(Guid AssignID, Guid UserUID, Guid ProjectUID, Guid UserRole,string AssignDate,string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdate_UserProjects"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@AssignID", AssignID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@UserRole", UserRole);
                        cmd.Parameters.AddWithValue("@AssignDate", AssignDate);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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


        public int InsertorUpdate_UserWorkPackages(Guid UID, Guid ProjectUID, Guid UserUID, Guid WorkPackageUID, string Status, string CreatedDate, string UpdatedDate, string Activity_Type, string Activity_Id, string UserRole_ID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_UserWorkPackages"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                     
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        if (!string.IsNullOrEmpty(UpdatedDate))
                        {
                            cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
                        }
                        if (!string.IsNullOrEmpty(Activity_Type))
                        {
                            cmd.Parameters.AddWithValue("@Activity_Type", Activity_Type);
                        }
                        if (!string.IsNullOrEmpty(Activity_Id))
                        {
                            cmd.Parameters.AddWithValue("@Activity_Id", Activity_Id);
                        }
                        if (!string.IsNullOrEmpty(UserRole_ID))
                        {
                            cmd.Parameters.AddWithValue("@UserRole_ID", UserRole_ID);
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

        public int InsertorUpdate_UserRolesMaster(Guid UserRole_ID, string UserRole_Desc, string UserRole_Name)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_UserRolesMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserRole_ID", UserRole_ID);
                        cmd.Parameters.AddWithValue("@UserRole_Desc", UserRole_Desc);
                        cmd.Parameters.AddWithValue("@UserRole_Name", UserRole_Name);
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

        public int InsertorUpdate_UserType_Functionality_Master(Guid UID, string Functionality, string Code)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_UserType_Functionality_Master"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@Functionality", Functionality);
                        cmd.Parameters.AddWithValue("@Code", Code);
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

        public int InsertorUpdate_UserType_Functionality_Mapping(Guid UID, string UserType, Guid FunctionalityUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_UserType_Functionality_Mapping"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@UserType", UserType);
                        cmd.Parameters.AddWithValue("@FunctionalityUID", FunctionalityUID);
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

        public int InsertorUpdate_Issues(Guid Issue_Uid, Guid ProjectUID, Guid WorkPackagesUID, Guid TaskUID, string Issue_Description, string Issue_Date, string Issued_User, string Assigned_User, string Assigned_Date,string Issue_ProposedCloser_Date,string Approving_User,string Actual_Closer_Date,string Issue_Status,string Issue_Remarks,string Issue_Document,string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_Issues"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackagesUID", WorkPackagesUID);
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

                        if (!string.IsNullOrEmpty(Issue_Document))
                        {
                            cmd.Parameters.AddWithValue("@Issue_Document", Issue_Document);
                        }
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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

        public int InsertorUpdate_IssueRemarks(Guid IssueRemarksUID, Guid Issue_Uid, string Issue_Status,string Issue_Remarks,string Issue_Document,string IssueRemark_Date,string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_IssueRemarks"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@IssueRemarksUID", IssueRemarksUID);
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
                        cmd.Parameters.AddWithValue("@Issue_Status", Issue_Status);

                        cmd.Parameters.AddWithValue("@Issue_Remarks", Issue_Remarks);
                        if (!string.IsNullOrEmpty(Issue_Document))
                        {
                            cmd.Parameters.AddWithValue("@Issue_Document", Issue_Document);
                        }
                        cmd.Parameters.AddWithValue("@IssueRemark_Date", IssueRemark_Date);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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

        public int InsertorUpdate_ActivityDeleteLogs(Guid DeleteLog_UID, Guid Activity_UID, Guid Activity_UserUID, string Activity_For, string Activity_Date)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_ActivityDeleteLogs"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DeleteLog_UID", DeleteLog_UID);
                        cmd.Parameters.AddWithValue("@Activity_UID", Activity_UID);
                        cmd.Parameters.AddWithValue("@Activity_UserUID", Activity_UserUID);
                        cmd.Parameters.AddWithValue("@Activity_For", Activity_For);
                        cmd.Parameters.AddWithValue("@Activity_Date", Activity_Date);
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

        public int InsertorUpdate_FinanceMileStones(Guid Finance_MileStoneUID, Guid TaskUID, string Finance_MileStoneName, string Finance_MileStoneCreatedDate, string User_Created, string Delete_Flag, string IsMonth)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_FinanceMileStones"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@Finance_MileStoneName", Finance_MileStoneName);

                        cmd.Parameters.AddWithValue("@Finance_MileStoneCreatedDate", Finance_MileStoneCreatedDate);
                        if (!string.IsNullOrEmpty(User_Created))
                        {
                            cmd.Parameters.AddWithValue("@User_Created", new Guid(User_Created));
                        }
                        cmd.Parameters.AddWithValue("@IsMonth", IsMonth);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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

        public int InsertorUpdate_FinanceMileStoneMonth(Guid FinMileStoneMonthUID, Guid Finance_MileStoneUID, decimal AllowedPayment, string UserCreated, string CreatedDate, string DeletedFlag, string Month,Int16 Year,Int64 MultiplyingFactor,Guid WorkPackageUID,int OrderBy,string DeletedBy,string DeletedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_FinanceMileStoneMonth"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@FinMileStoneMonthUID", FinMileStoneMonthUID);
                        cmd.Parameters.AddWithValue("@Finance_MileStoneUID", Finance_MileStoneUID);
                        cmd.Parameters.AddWithValue("@AllowedPayment", AllowedPayment);

                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        if (!string.IsNullOrEmpty(UserCreated))
                        {
                            cmd.Parameters.AddWithValue("@User_Created", new Guid(UserCreated));
                        }
                        cmd.Parameters.AddWithValue("@Month", Month);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
                        cmd.Parameters.AddWithValue("@Year", Year);
                        cmd.Parameters.AddWithValue("@MultiplyingFactor", MultiplyingFactor);
                        //
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@OrderBy", OrderBy);
                        //
                        if (!string.IsNullOrEmpty(DeletedBy))
                        {
                            cmd.Parameters.AddWithValue("@DeletedBy", new Guid(DeletedBy));
                        }
                        if (!string.IsNullOrEmpty(DeletedDate))
                        {
                            cmd.Parameters.AddWithValue("@DeletedDate", DeletedDate);
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

        public int InsertorUpdate_FinanceMileStoneMonth_EditedValues(Guid UID, Guid FinMileStoneMonthUID, decimal OldPaymentValue, decimal NewPaymentValue, string CreatedDate, Guid EditedBy)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbysnc_InsertorUpdate_FinanceMileStoneMonth_EditedValues"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@FinMileStoneMonthUID", FinMileStoneMonthUID);
                        cmd.Parameters.AddWithValue("@OldPaymentValue", OldPaymentValue);

                        cmd.Parameters.AddWithValue("@NewPaymentValue", NewPaymentValue);
                      
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@EditedBy", EditedBy);
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

        public int InsertorUpdate_TaskSchedule(Guid TaskScheduleUID, Guid WorkpacageUID, Guid TaskUID, string StartDate, string EndDate, decimal Schedule_Value, string Schedule_Type, string Created_Date, float TaskScheduleVersion, string TaskSchedule_Approved, string Delete_Flag, string Achieved_Value,string Achieved_Date,string Schedule_Per,string Achieved_Per, string revised_scheduled_value, string revised_acheivedvalue)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_TaskSchedule"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskScheduleUID", TaskScheduleUID);
                        cmd.Parameters.AddWithValue("@WorkpacageUID", WorkpacageUID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);

                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        if (!string.IsNullOrEmpty(EndDate))
                        {
                            cmd.Parameters.AddWithValue("@EndDate", EndDate);
                        }
                        cmd.Parameters.AddWithValue("@Schedule_Type", Schedule_Type);
                        cmd.Parameters.AddWithValue("@Schedule_Value", Schedule_Value);
                        cmd.Parameters.AddWithValue("@Created_Date", Created_Date);
                        cmd.Parameters.AddWithValue("@TaskScheduleVersion", TaskScheduleVersion);
                        //
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
                        cmd.Parameters.AddWithValue("@TaskSchedule_Approved", TaskSchedule_Approved);
                        
                        if (!string.IsNullOrEmpty(Achieved_Value))
                        {
                            cmd.Parameters.AddWithValue("@Achieved_Value",decimal.Parse(Achieved_Value));
                        }
                        if (!string.IsNullOrEmpty(Achieved_Date))
                        {
                            cmd.Parameters.AddWithValue("@Achieved_Date", Achieved_Date);
                        }
                        //
                        if (!string.IsNullOrEmpty(Schedule_Per))
                        {
                            cmd.Parameters.AddWithValue("@Schedule_Per", decimal.Parse(Schedule_Per));
                        }
                        if (!string.IsNullOrEmpty(Achieved_Per))
                        {
                            cmd.Parameters.AddWithValue("@Achieved_Per", decimal.Parse(Achieved_Per));
                        }
                        //
                        if (!string.IsNullOrEmpty(revised_scheduled_value))
                        {
                            cmd.Parameters.AddWithValue("@revised_scheduled_value", decimal.Parse(revised_scheduled_value));
                        }
                        if (!string.IsNullOrEmpty(revised_acheivedvalue))
                        {
                            cmd.Parameters.AddWithValue("@revised_acheivedvalue", decimal.Parse(revised_acheivedvalue));
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

        public int InsertorUpdate_TaskScheduleVersion(Guid TaskScheduleVersion_UID, Guid TaskUID, float TaskScheduleVersion, string TaskScheduleType, string TaskSchedule_Approved, string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_TaskScheduleVersion"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskScheduleVersion_UID", TaskScheduleVersion_UID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@TaskScheduleVersion", TaskScheduleVersion);
                        cmd.Parameters.AddWithValue("@TaskScheduleType", TaskScheduleType);
                        cmd.Parameters.AddWithValue("@TaskSchedule_Approved", TaskSchedule_Approved);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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


        public int InsertorUpdate_Task(Guid TaskUID, Guid WorkPackageUID, Guid ProjectUID, Guid Workpackage_Option, string Owner, string Task_Section, string Name, string Description, string RFPReference, string POReference, string StartDate, string PlannedEndDate, string ProjectedEndDate, string Status, string Currency, string Currency_CultureInfo,float Basic_Budget,float GST,float Total_Budget,float ActualExpenditure,string RFPDocument,string NoOfDocuments,
            int TaskLevel,string ParentTaskID,string UpdatedDate,float StatusPer,string UnitforProgress,string UnitQuantity,string PlannedStartDate,string ProjectedStartDate,string ActualEndDate,string Discipline,
            string MileStone,string Task_Weightage,string Task_Type,string Delete_Flag,string Task_Order,string BOQDetailsUID,string GroupBOQItems, string Task_CulumativePercentage,string CumulativeAchvQuantity,string InGraph,string Report1, string Report2, string Report3, string Report4, string Report5,string revised_unitquantity, string revised_cumulariveacheivedquantity, string revised_weightage)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_Tasks"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        //
                        cmd.Parameters.AddWithValue("@Workpackage_Option", Workpackage_Option);
                        cmd.Parameters.AddWithValue("@Owner", Owner);
                        if (!string.IsNullOrEmpty(Task_Section))
                        {
                            cmd.Parameters.AddWithValue("@Task_Section", Task_Section);
                        }
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        if (!string.IsNullOrEmpty(RFPReference))
                        {
                            cmd.Parameters.AddWithValue("@RFPReference", RFPReference);
                        }
                        if (!string.IsNullOrEmpty(RFPReference))
                        {
                            cmd.Parameters.AddWithValue("@POReference", POReference);
                        }
                        //
                        if (!string.IsNullOrEmpty(StartDate))
                        {
                            cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        }
                        if (!string.IsNullOrEmpty(PlannedEndDate))
                        {
                            cmd.Parameters.AddWithValue("@PlannedEndDate", PlannedEndDate);
                        }
                        //
                        if (!string.IsNullOrEmpty(ProjectedEndDate))
                        {
                            cmd.Parameters.AddWithValue("@ProjectedEndDate", ProjectedEndDate);
                        }
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@ActualExpenditure", ActualExpenditure);
                        cmd.Parameters.AddWithValue("@Total_Budget", Total_Budget);
                        cmd.Parameters.AddWithValue("@RFPDocument", RFPDocument);
                        if (!string.IsNullOrEmpty(NoOfDocuments))
                        {
                            cmd.Parameters.AddWithValue("@NoOfDocuments", int.Parse(NoOfDocuments));
                        }
                        cmd.Parameters.AddWithValue("@TaskLevel", TaskLevel);
                        if (!string.IsNullOrEmpty(ParentTaskID))
                        {
                            cmd.Parameters.AddWithValue("@ParentTaskID", ParentTaskID);
                        }
                      

                        if (!string.IsNullOrEmpty(UpdatedDate))
                        {
                            cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
                        }
                        cmd.Parameters.AddWithValue("@StatusPer", StatusPer);

                        if (!string.IsNullOrEmpty(UnitforProgress))
                        {
                            cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        }

                        if (!string.IsNullOrEmpty(UnitQuantity))
                        {
                            cmd.Parameters.AddWithValue("@UnitQuantity", float.Parse(UnitQuantity));
                        }
                        if (!string.IsNullOrEmpty(PlannedStartDate))
                        {
                            cmd.Parameters.AddWithValue("@PlannedStartDate", PlannedStartDate);
                        }
                        if (!string.IsNullOrEmpty(ProjectedStartDate))
                        {
                            cmd.Parameters.AddWithValue("@ProjectedStartDate", ProjectedStartDate);
                        }
                        if (!string.IsNullOrEmpty(ActualEndDate))
                        {
                            cmd.Parameters.AddWithValue("@ActualEndDate", ActualEndDate);
                        }
                        if (!string.IsNullOrEmpty(Discipline))
                        {
                            cmd.Parameters.AddWithValue("@Discipline", Discipline);
                        }
                        if (!string.IsNullOrEmpty(MileStone))
                        {
                            cmd.Parameters.AddWithValue("@MileStone",MileStone);
                        }
                        if (!string.IsNullOrEmpty(Task_Weightage))
                        {
                            cmd.Parameters.AddWithValue("@Task_Weightage",float.Parse(Task_Weightage));
                        }
                        if (!string.IsNullOrEmpty(Task_Type))
                        {
                            cmd.Parameters.AddWithValue("@Task_Type", Task_Type);
                        }

                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
                        if (!string.IsNullOrEmpty(Task_Order))
                        {
                            cmd.Parameters.AddWithValue("@Task_Order", int.Parse(Task_Order));
                        }
                        if (!string.IsNullOrEmpty(BOQDetailsUID))
                        {
                            cmd.Parameters.AddWithValue("@BOQDetailsUID", new Guid(BOQDetailsUID));
                        }
                        if (!string.IsNullOrEmpty(GroupBOQItems))
                        {
                            cmd.Parameters.AddWithValue("@GroupBOQItems", GroupBOQItems);
                        }
                        //
                        if (!string.IsNullOrEmpty(Task_CulumativePercentage))
                        {
                            cmd.Parameters.AddWithValue("@Task_CulumativePercentage", float.Parse(Task_CulumativePercentage));
                        }
                        if (!string.IsNullOrEmpty(CumulativeAchvQuantity))
                        {
                            cmd.Parameters.AddWithValue("@CumulativeAchvQuantity", float.Parse(CumulativeAchvQuantity));
                        }
                        if (!string.IsNullOrEmpty(InGraph))
                        {
                            cmd.Parameters.AddWithValue("@InGraph", InGraph);
                        }

                        if (!string.IsNullOrEmpty(Report1))
                        {
                            cmd.Parameters.AddWithValue("@Report1", Report1);
                        }
                        if (!string.IsNullOrEmpty(Report2))
                        {
                            cmd.Parameters.AddWithValue("@Report2", Report2);
                        }
                        if (!string.IsNullOrEmpty(Report3))
                        {
                            cmd.Parameters.AddWithValue("@Report3", Report3);
                        }
                        if (!string.IsNullOrEmpty(Report4))
                        {
                            cmd.Parameters.AddWithValue("@Report4", Report4);
                        }
                        if (!string.IsNullOrEmpty(Report5))
                        {
                            cmd.Parameters.AddWithValue("@Report5", Report5);
                        }
                        //added by nikhil on Dec-31st-22
                        if (!string.IsNullOrEmpty(revised_unitquantity))
                        {
                            cmd.Parameters.AddWithValue("@revised_unitquantity", float.Parse(revised_unitquantity));
                        }
                        if (!string.IsNullOrEmpty(revised_cumulariveacheivedquantity))
                        {
                            cmd.Parameters.AddWithValue("@revised_cumulariveacheivedquantity", float.Parse(revised_cumulariveacheivedquantity));
                        }
                        if (!string.IsNullOrEmpty(revised_weightage))
                        {
                            cmd.Parameters.AddWithValue("@revised_weightage", float.Parse(revised_weightage));
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



        //added on 08/04/2022
        public int InsertorUpdateReferencNoHistory(Guid UID, Guid ActualDocumentUID, string OriginatorReferenceNo, string ONTBRefNo, string CreatedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdateReferencNoHistory"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@OriginatorReferenceNo", OriginatorReferenceNo);
                        cmd.Parameters.AddWithValue("@ONTBRefNo", ONTBRefNo);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
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

        public int InsertOrUpdateSubmittal_MultipleUsers(Guid UID, Guid SubmittalUID, Int16 Step, Guid UserUID)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdateSubmittal_MultipleUsers"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@SubmittalUID", SubmittalUID);
                        cmd.Parameters.AddWithValue("@Step", Step);
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

        public int InsertorUpdate_Flow_Master_Users(Guid UID, Guid FlowUID,Guid ProjectUID,Guid WorkpackageUID,Guid WorkpackagecategoryUID,Int16 step, Guid UserUID,string CreatedDate,string DeletedFlag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_Flow_Master_Users"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@FlowUID", FlowUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@WorkpackagecategoryUID", WorkpackagecategoryUID);
                        cmd.Parameters.AddWithValue("@step", step);
                        cmd.Parameters.AddWithValue("@UserUID", UserUID);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
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


        public int InsertorUpdate_MeasurementBook(Guid UID, Guid TaskUID, string UnitforProgress, string Quantity, string Description, string Upload_File, string CreatedDate, string CreatedByUID,string Remarks,string Delete_Flag,string Achieved_Date)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_MeasurementBook"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@TaskUID", TaskUID);
                        cmd.Parameters.AddWithValue("@UnitforProgress", UnitforProgress);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@Upload_File", Upload_File);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        if (!string.IsNullOrEmpty(CreatedByUID))
                        {
                            cmd.Parameters.AddWithValue("@CreatedByUID", CreatedByUID);
                        }
                        
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
                        if (!string.IsNullOrEmpty(Achieved_Date))
                        {
                            cmd.Parameters.AddWithValue("@Achieved_Date", Achieved_Date);
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

        //added on 06/05/2022
        public int InsertOrUpdate_IssueDocs(int uploaded_doc_id, string doc_name, string doc_path, string issue_remarks_uid)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_IssueDocs"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@uploaded_doc_id", uploaded_doc_id);
                        cmd.Parameters.AddWithValue("@doc_name", doc_name);
                        cmd.Parameters.AddWithValue("@doc_path", doc_path);
                        cmd.Parameters.AddWithValue("@issue_remarks_uid", issue_remarks_uid);
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


        //added on 31/07/2022
        public int InsertOrUpdate_IssueDocs_Main(int doc_id, string doc_name, string doc_path, string Issue_Uid)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_IssueDocsMain"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@doc_id", doc_id);
                        cmd.Parameters.AddWithValue("@doc_name", doc_name);
                        cmd.Parameters.AddWithValue("@doc_path", doc_path);
                        cmd.Parameters.AddWithValue("@Issue_Uid", Issue_Uid);
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


        // added on 15/06/2022
        public int InsertOrUpdateDailyProgressReportMaster(Guid DPR_UID, string Description, string CreatedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdateDailyProgressReportMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DPR_UID", DPR_UID);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
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

        public int InsertOrUpdateDailyProgress(Guid UID, Guid DPR_UID, Guid ProjectUID,Guid WorkPackageUID,string VillageName,string PipeDia,decimal Quantity,decimal RevisedQuantity,decimal PipesReceived,decimal PreviousQuantity,decimal TodaysQuantity,decimal TotalQuantity,decimal Balance,string Remarks,string CreatedDate,string ZoneName,string DeletedFlag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdateDailyProgress"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@DPR_UID", DPR_UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@VillageName", VillageName);
                        cmd.Parameters.AddWithValue("@PipeDia", PipeDia);
                        cmd.Parameters.AddWithValue("@Quantity", Quantity);
                        cmd.Parameters.AddWithValue("@RevisedQuantity", RevisedQuantity);
                        cmd.Parameters.AddWithValue("@PipesReceived", PipesReceived);
                        cmd.Parameters.AddWithValue("@PreviousQuantity", PreviousQuantity);
                        cmd.Parameters.AddWithValue("@TodaysQuantity", TodaysQuantity);
                        cmd.Parameters.AddWithValue("@TotalQuantity", TotalQuantity);
                        cmd.Parameters.AddWithValue("@Balance", Balance);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@ZoneName", ZoneName);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
                        
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


        //added on 17/08/2022
        public int InsertorUpdate_GFCReportMaster(Guid GFC_UID, string Description, string CreatedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_GFCReportMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@GFC_UID", GFC_UID);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
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

        public int InsertorUpdate_GFCStatus(Guid UID, Guid GFC_UID, Guid ProjectUID, Guid WorkPackageUID, string SubmittedLocation, decimal SubmittedLength,string SubmittedDate, decimal ONTBReleasedLength,string ONTBReleasedDate, decimal ONTBReleasedBalance,string ONTBReleasedBalanceDate, decimal GFCApproved, string Remarks, string EEOfficeApproval,string NineSetsApproval, string Approved, string DeletedFlag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_GFCStatus"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@GFC_UID", GFC_UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@SubmittedLocation", SubmittedLocation);
                        cmd.Parameters.AddWithValue("@SubmittedLength", SubmittedLength);
                        cmd.Parameters.AddWithValue("@SubmittedDate", SubmittedDate);
                        cmd.Parameters.AddWithValue("@ONTBReleasedLength", ONTBReleasedLength);
                        cmd.Parameters.AddWithValue("@ONTBReleasedDate", ONTBReleasedDate);
                        cmd.Parameters.AddWithValue("@ONTBReleasedBalance", ONTBReleasedBalance);
                        cmd.Parameters.AddWithValue("@ONTBReleasedBalanceDate", ONTBReleasedBalanceDate);
                        cmd.Parameters.AddWithValue("@GFCApproved", GFCApproved);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                       
                        cmd.Parameters.AddWithValue("@EEOfficeApproval", EEOfficeApproval);
                        cmd.Parameters.AddWithValue("@NineSetsApproval", NineSetsApproval);
                        cmd.Parameters.AddWithValue("@Approved", Approved);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
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

        //added on 19/08/2022
        public int InsertorUpdate_design_and_drawing_A_master(Guid DPR_UID, string Description, string CreatedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_design_and_drawing_A_master"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DPR_UID", DPR_UID);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
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

        public int InsertorUpdate_Design_and_drawing_works_A(Guid UID,Guid DPR_UID, Guid ProjectUID, Guid WorkPackageUID,string Heading,string subheading,string Units,string SiteWorks,string CreatedDate,string DeletedFlag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_Design_and_drawing_works_A"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@DPR_UID", DPR_UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@Heading", Heading);
                        cmd.Parameters.AddWithValue("@subheading", subheading);
                        cmd.Parameters.AddWithValue("@Units", Units);
                        cmd.Parameters.AddWithValue("@SiteWorks", SiteWorks);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
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

        //added on 20/08/2022 for nikhil dbsync
        public int InsertorUpdate_design_and_drawing_dwg_issue_master(Guid DPR_UID, string Description, string CreatedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_design_and_drawing_dwg_issue_master"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DPR_UID", DPR_UID);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
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

       

       public int InsertorUpdate_Design_and_drawing_works_dwg_issue(Guid UID, Guid DPR_UID, Guid ProjectUID, Guid WorkPackageUID, string Location, decimal submittedbycontrator, string submitteddate, decimal approverbyontb, string approverdate, string CreatedDate, string ZoneName, string DeletedFlag, decimal GFCApprovedByBWSSB, string GFCApprovedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_Design_and_drawing_works_dwg_issue"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@DPR_UID", DPR_UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@Location", Location);
                        cmd.Parameters.AddWithValue("@submittedbycontrator", submittedbycontrator);
                        cmd.Parameters.AddWithValue("@submitteddate", submitteddate);
                        cmd.Parameters.AddWithValue("@approverbyontb", approverbyontb);
                        cmd.Parameters.AddWithValue("@approverdate", approverdate);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@ZoneName", ZoneName);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
                        cmd.Parameters.AddWithValue("@GFCApprovedByBWSSB", GFCApprovedByBWSSB);
                        cmd.Parameters.AddWithValue("@GFCApprovedDate", GFCApprovedDate);
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



        public int InsertorUpdate_design_and_drawing_works_b_tt_master(Guid DPR_UID, string Description, string CreatedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_design_and_drawing_works_b_tt_master"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DPR_UID", DPR_UID);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
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

        public int InsertorUpdate_Design_and_drawing_works_b_tt(Guid UID, Guid DPR_UID, Guid ProjectUID, Guid WorkPackageUID, string VillageName, string Crossing, string GFCStaus, decimal Length, decimal Diameter, decimal StartDepth, decimal StopDepth, string Remarks, string CreatedDate, string ZoneName, string DeletedFlag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_Design_and_drawing_works_b_tt"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@DPR_UID", DPR_UID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkPackageUID", WorkPackageUID);
                        cmd.Parameters.AddWithValue("@VillageName", VillageName);
                        cmd.Parameters.AddWithValue("@Crossing", Crossing);
                        cmd.Parameters.AddWithValue("@GFCStaus", GFCStaus);
                        cmd.Parameters.AddWithValue("@Length", Length);
                        cmd.Parameters.AddWithValue("@Diameter", Diameter);
                        cmd.Parameters.AddWithValue("@StartDepth", StartDepth);
                        cmd.Parameters.AddWithValue("@StopDepth", StopDepth);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@ZoneName", ZoneName);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
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

        public int InsertoUpdate_RABillPayments(Guid PaymentUID, Guid InvoiceUID, string RABillDesc, decimal Amount, decimal TotalDeductions,decimal NetAmount, string CreatedDate, string DeletedFlag, Guid FinMileStoneMonthUID, string PaymentDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertoUpdate_RABillPayments"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PaymentUID", PaymentUID);
                        cmd.Parameters.AddWithValue("@InvoiceUID", InvoiceUID);
                        cmd.Parameters.AddWithValue("@RABillDesc", RABillDesc);
                        cmd.Parameters.AddWithValue("@Amount", Amount); 
                        cmd.Parameters.AddWithValue("@TotalDeductions", TotalDeductions);
                        cmd.Parameters.AddWithValue("@NetAmount", NetAmount);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
                        cmd.Parameters.AddWithValue("@DeletedFlag", DeletedFlag);
                        cmd.Parameters.AddWithValue("@FinMileStoneMonthUID", FinMileStoneMonthUID);
                        cmd.Parameters.AddWithValue("@PaymentDate", PaymentDate);
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

        //added on 29/08/2022
        // Edited by Nikhil on 26-08-2022 -Line 669 & 686  for RABill_Amount
        public int InsertOrUpdateRABill_Abstract(Guid RABillUid, Guid WorkpackageUID, string RABillNumber, string RABill_Date, string DeleteFlag, string RABill_Amount,string RABill_SubmissionAmount,string RABill_SubmissionDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdateRABill_Abstract"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@RABillUid", RABillUid);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@RABillNumber", RABillNumber);
                        cmd.Parameters.AddWithValue("@RABill_Date", RABill_Date);
                        cmd.Parameters.AddWithValue("@DeleteFlag", DeleteFlag);
                        cmd.Parameters.AddWithValue("@RABill_Amount", RABill_Amount);
                        cmd.Parameters.AddWithValue("@RABill_SubmissionAmount", RABill_SubmissionAmount);
                        if (!string.IsNullOrEmpty(RABill_SubmissionDate))
                        {
                            cmd.Parameters.AddWithValue("@RABill_SubmissionDate", RABill_SubmissionDate);
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



        // added on 26-08-2022 by Nikhil
        public int InsertOrUpdate_RABills_Documents(Guid DocumentUID, string created_date, Guid RABillUid, Guid WorkpackageUID, Guid UserID, string FilePath, string Description, string DeleteFlag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdate_RABills_Documents"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@DocumentUID", DocumentUID);
                        cmd.Parameters.AddWithValue("@created_date", created_date);
                        cmd.Parameters.AddWithValue("@RABillUid", RABillUid);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.Parameters.AddWithValue("@FilePath", FilePath);
                        cmd.Parameters.AddWithValue("@Description", Description);
                        cmd.Parameters.AddWithValue("@DeleteFlag", DeleteFlag);


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

        //added on 05/01/2022
        public int InsertOrUpdate_ResourceMaster(Guid ResourceUID,Guid ProjectUID, Guid WorkpackageUID, string ResourceName, string CostType,string Currency,string Currency_CultureInfo, string Resource_Owner, float Basic_Budget, float GST,float Total_Budget,int ResourceLevel,string ParentResourceID,string Resource_Description,string Unit_for_Measurement,Guid ResourceType_UID,string Delete_Flag)
        {
            int sresult = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertOrUpdate_ResourceMaster"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                        cmd.Parameters.AddWithValue("@ProjectUID", ProjectUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@ResourceName", ResourceName);
                        cmd.Parameters.AddWithValue("@CostType", CostType);
                        cmd.Parameters.AddWithValue("@Currency", Currency);
                        cmd.Parameters.AddWithValue("@Currency_CultureInfo", Currency_CultureInfo);
                        cmd.Parameters.AddWithValue("@Resource_Owner", Resource_Owner);
                        cmd.Parameters.AddWithValue("@Basic_Budget", Basic_Budget);
                        cmd.Parameters.AddWithValue("@GST", GST);
                        cmd.Parameters.AddWithValue("@Total_Budget", Total_Budget);
                        cmd.Parameters.AddWithValue("@ResourceLevel", ResourceLevel);
                        if (!string.IsNullOrEmpty(ParentResourceID))
                        {
                            cmd.Parameters.AddWithValue("@ParentResourceID", Guid.Parse(ParentResourceID));
                        }
                        cmd.Parameters.AddWithValue("@Resource_Description", Resource_Description);
                        cmd.Parameters.AddWithValue("@Unit_for_Measurement", Unit_for_Measurement);
                        cmd.Parameters.AddWithValue("@ResourceType_UID", ResourceType_UID);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);

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


        //added on 05/01/2022 for nikhil
        public int InsertorUpdate_ResourceDeployment(Guid ReourceDeploymentUID, Guid WorkpackageUID, Guid ResourceUID, string StartDate, string EndDate, string DeploymentType, decimal Planned, string PlannedDate, string Deployed, string DeployedDate, string Remarks, string Created_Date, string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_ResourceDeployment"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ReourceDeploymentUID", ReourceDeploymentUID);
                        cmd.Parameters.AddWithValue("@WorkpackageUID", WorkpackageUID);
                        cmd.Parameters.AddWithValue("@ResourceUID", ResourceUID);
                        cmd.Parameters.AddWithValue("@StartDate", StartDate);
                        cmd.Parameters.AddWithValue("@EndDate", EndDate);
                        cmd.Parameters.AddWithValue("@DeploymentType", DeploymentType);
                        cmd.Parameters.AddWithValue("@Planned", Planned);
                        cmd.Parameters.AddWithValue("@PlannedDate", PlannedDate);
                        if (!string.IsNullOrEmpty(Deployed))
                        {
                            cmd.Parameters.AddWithValue("@Deployed", decimal.Parse(Deployed));
                        }
                        if (!string.IsNullOrEmpty(DeployedDate))
                        {
                            cmd.Parameters.AddWithValue("@DeployedDate", DateTime.Parse(DeployedDate));
                        }
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@Created_Date", Created_Date);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);

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

        //added by nikhil -05-01-2023
        public int InsertorUpdate_ResourceDeploymentUpdate(Guid UID, Guid ReourceDeploymentUID, decimal Deployed, string DeployedDate, string Remarks, string Delete_Flag)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_ResourceDeploymentUpdate"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@ReourceDeploymentUID", ReourceDeploymentUID);
                        cmd.Parameters.AddWithValue("@Deployed", Deployed);
                        cmd.Parameters.AddWithValue("@DeployedDate", DeployedDate);
                        cmd.Parameters.AddWithValue("@Remarks", Remarks);
                        cmd.Parameters.AddWithValue("@Delete_Flag", Delete_Flag);
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

        //added on 17/01/2023 for nikhil
        public int InsertorUpdate_form_task_update(Guid Form_task_update_uid, Guid Taskuid, string TaskName, Guid projectuid, Guid workpackageuid)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_form_task_update"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@Form_task_update_uid", Form_task_update_uid);
                        cmd.Parameters.AddWithValue("@Taskuid", Taskuid);
                        cmd.Parameters.AddWithValue("@TaskName", TaskName);
                        cmd.Parameters.AddWithValue("@projectuid", projectuid);
                        cmd.Parameters.AddWithValue("@workpackageuid", workpackageuid);

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



 public int InsertorUpdate_CorrespondenceCCToUsers(Guid UID, Guid ActualDocumentUID, Guid StatusUID, string UserType, string CreatedDate)
        {
            int sresult = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(db.GetConnectionString()))
                {

                    using (SqlCommand cmd = new SqlCommand("dbsync_InsertorUpdate_CorrespondenceCCToUsers"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UID", UID);
                        cmd.Parameters.AddWithValue("@ActualDocumentUID", ActualDocumentUID);
                        cmd.Parameters.AddWithValue("@StatusUID", StatusUID);
                        cmd.Parameters.AddWithValue("@UserType", UserType);
                        cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
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
    }
}