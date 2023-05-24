using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PMTWebAPI.DAL;
using PMTWebAPI.Models;
using System.Net.Http;
namespace PMTWebAPI.Controllers
{
    public class RootObject
    {
        public List<Task> tName { get; set; }
    }

    public class Task
    {
        public string TaskName { get; set; }
    }

    public class FinalData
    {
        public Guid ProjectCategoryUID { get; set; }
        public string ProjectCategoryName { get; set; }
        public List<ProjectDetails> projectDetails { get; set; } = new List<ProjectDetails>();
        //public List<WorkPackages> workPackages { get; set; } = new List<WorkPackages>();
        //public List<WorkpackageSelectedOptions> selectedOptions { get; set; } = new List<WorkpackageSelectedOptions>();
        //public List<Tasks> Tasks { get; set; } = new List<Tasks>();
        //public FinalData()
        //{
        //    projectDetails = new List<ProjectDetails>();
        //    workPackages = new List<WorkPackages>();
        //    selectedOptions = new List<WorkpackageSelectedOptions>();
        //    Tasks = new List<Tasks>();
        //}
    }
    public class ActivityController : ApiController
    {
        DBGetData db = new DBGetData();
        TableProperties tp = new TableProperties();

        [HttpPost]
        public IHttpActionResult FetchData()
        {
            List<FinalData> fData = new List<FinalData>();
           
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
                  

                    FinalData fd = new FinalData();
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
                      
                        ProjectDetails pd = new ProjectDetails
                        {
                            ProjectUID = new Guid(dsProject.Tables[0].Rows[j]["ProjectUID"].ToString()),
                            ProjectClass_UID = new Guid(dsProject.Tables[0].Rows[j]["ProjectClass_UID"].ToString()),
                            ProjectName = dsProject.Tables[0].Rows[j]["Name"].ToString(),
                            ProjectAbbrevation = dsProject.Tables[0].Rows[j]["ProjectAbbrevation"].ToString()
                        };
                        fd.projectDetails.Add(pd);
                      
                        DataSet dsworkPackage = db.GetWorkPackages_By_ProjectUID(new Guid(dsProject.Tables[0].Rows[j]["ProjectUID"].ToString()));
                        for (int k = 0; k < dsworkPackage.Tables[0].Rows.Count; k++)
                        {
                            WorkPackages wp = new WorkPackages();
                            wp.WorkPackageUID = new Guid(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString());
                            wp.ProjectUID = new Guid(dsworkPackage.Tables[0].Rows[k]["ProjectUID"].ToString());
                            wp.Name = dsworkPackage.Tables[0].Rows[k]["Name"].ToString();
                           
                            pd.workPackages.Add(wp);
                            
                            DataSet dsoption = db.GetSelectedOption_By_WorkpackageUID(new Guid(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString()));
                            for (int l = 0; l < dsoption.Tables[0].Rows.Count; l++)
                            {
                                WorkpackageSelectedOptions so = new WorkpackageSelectedOptions();
                                so.WorkpackageSelectedOption_UID = new Guid(dsoption.Tables[0].Rows[l]["WorkpackageSelectedOption_UID"].ToString());
                                so.WorkPackageUID = new Guid(dsoption.Tables[0].Rows[l]["WorkPackageUID"].ToString());
                                so.Workpackage_OptionUID = new Guid(dsoption.Tables[0].Rows[l]["Workpackage_OptionUID"].ToString());
                                so.WorkpackageSelectedOption_Name = dsoption.Tables[0].Rows[l]["WorkpackageSelectedOption_Name"].ToString();
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

                                    DataSet Milsestones = db.getTaskMileStones(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()));
                                    for (int n = 0; n < Milsestones.Tables[0].Rows.Count; n++)
                                    {
                                        ActivityMileStone AM = new ActivityMileStone()
                                        {
                                            MileStoneUID = new Guid(Milsestones.Tables[0].Rows[n]["MileStoneUID"].ToString()),
                                            TaskUID = new Guid(Milsestones.Tables[0].Rows[n]["TaskUID"].ToString()),
                                            Description= Milsestones.Tables[0].Rows[n]["Description"].ToString(),
                                            Status= Milsestones.Tables[0].Rows[n]["Status"].ToString()
                                        };
                                        tk.activityMileStones.Add(AM);
                                    }
                                    
                                    DataSet Resources = db.getTaskResourceAllocated(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()));
                                    for (int p = 0; p< Resources.Tables[0].Rows.Count; p++)
                                    {
                                        ActivityResourcesAllocated ARA = new ActivityResourcesAllocated()
                                        {
                                            UID = new Guid(Resources.Tables[0].Rows[p]["UID"].ToString()),
                                            TaskUID = new Guid(Resources.Tables[0].Rows[p]["TaskUID"].ToString()),
                                            ResourceUID = new Guid(Resources.Tables[0].Rows[p]["ResourceUID"].ToString()),
                                            AllocatedUnits = Convert.ToInt32(Resources.Tables[0].Rows[p]["AllocatedUnits"].ToString())
                                        };
                                        tk.activityResourcesAllocateds.Add(ARA);
                                    }

                                    DataSet fMilestone = db.GetFinance_MileStonesDetails_By_TaskUID(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()));
                                    for (int q = 0; q < fMilestone.Tables[0].Rows.Count; q++)
                                    {
                                        FinancialMileStones fm = new FinancialMileStones()
                                        {
                                            Finance_MileStoneUID=new Guid(fMilestone.Tables[0].Rows[q]["Finance_MileStoneUID"].ToString()),
                                            TaskUID = new Guid(fMilestone.Tables[0].Rows[q]["TaskUID"].ToString()),
                                            Finance_MileStoneName= fMilestone.Tables[0].Rows[q]["Finance_MileStoneName"].ToString(),
                                            Finance_AllowedPayment=Convert.ToDouble(fMilestone.Tables[0].Rows[q]["Finance_AllowedPayment"].ToString()),
                                            Finance_MileStoneCreatedDate=Convert.ToDateTime(fMilestone.Tables[0].Rows[q]["Finance_MileStoneCreatedDate"].ToString())
                                        };
                                        tk.financialMileStones.Add(fm);
                                    }
                                    TaskDataAdd(new Guid(dstask.Tables[0].Rows[m]["TaskUID"].ToString()), tk);
                                }
                            }
                        }
                    }
                    
                    
                    fData.Add(fd);
                }
               
                return Json(fData.ToList());
              
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

        public void TaskDataAdd(Guid ParentTaskUID,Tasks tkparent)
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

                DataSet Milsestones = db.getTaskMileStones(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()));
                for (int n = 0; n < Milsestones.Tables[0].Rows.Count; n++)
                {
                    ActivityMileStone AM = new ActivityMileStone()
                    {
                        MileStoneUID = new Guid(Milsestones.Tables[0].Rows[n]["MileStoneUID"].ToString()),
                        TaskUID = new Guid(Milsestones.Tables[0].Rows[n]["TaskUID"].ToString()),
                        Description = Milsestones.Tables[0].Rows[n]["Description"].ToString(),
                        Status = Milsestones.Tables[0].Rows[n]["Status"].ToString()
                    };
                    tk.activityMileStones.Add(AM);
                }

                DataSet Resources = db.getTaskResourceAllocated(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()));
                for (int p = 0; p < Resources.Tables[0].Rows.Count; p++)
                {
                    ActivityResourcesAllocated ARA = new ActivityResourcesAllocated()
                    {
                        UID = new Guid(Resources.Tables[0].Rows[p]["UID"].ToString()),
                        TaskUID = new Guid(Resources.Tables[0].Rows[p]["TaskUID"].ToString()),
                        ResourceUID = new Guid(Resources.Tables[0].Rows[p]["ResourceUID"].ToString()),
                        AllocatedUnits = Convert.ToInt32(Resources.Tables[0].Rows[p]["AllocatedUnits"].ToString())
                    };
                    tk.activityResourcesAllocateds.Add(ARA);
                }

                DataSet fMilestone = db.GetFinance_MileStonesDetails_By_TaskUID(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()));
                for (int q = 0; q < fMilestone.Tables[0].Rows.Count; q++)
                {
                    FinancialMileStones fm = new FinancialMileStones()
                    {
                        Finance_MileStoneUID = new Guid(fMilestone.Tables[0].Rows[q]["Finance_MileStoneUID"].ToString()),
                        TaskUID = new Guid(fMilestone.Tables[0].Rows[q]["TaskUID"].ToString()),
                        Finance_MileStoneName = fMilestone.Tables[0].Rows[q]["Finance_MileStoneName"].ToString(),
                        Finance_AllowedPayment = Convert.ToDouble(fMilestone.Tables[0].Rows[q]["Finance_AllowedPayment"].ToString()),
                        Finance_MileStoneCreatedDate = Convert.ToDateTime(fMilestone.Tables[0].Rows[q]["Finance_MileStoneCreatedDate"].ToString())
                    };
                    tk.financialMileStones.Add(fm);

                }

                TaskDataAdd(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()), tk);
            }
        }

        [HttpPost]
        public IHttpActionResult MileStoneUpdate()
        {
            var httpRequest = HttpContext.Current.Request;
            var UserName = httpRequest.Params["UserName"];
            var Password = Security.Encrypt(httpRequest.Params["Password"]);
            DataSet ds = new DataSet();

            try
            {
                ds = db.CheckLogin(UserName, Password);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
                    var MileStoneUID = httpRequest.Params["MileStoneUID"];
                    var Status = httpRequest.Params["Status"];

                    if (Status == "Not Completed" || Status == "Completed")
                    {
                        ds.Clear();
                        ds = db.getMileStonesDetails(new Guid(MileStoneUID));
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows[0]["MileStoneUID"].ToString() == "Completed" && Status == "Not Completed")
                            {
                                bool ret = db.InsertorUpdateMileStone(new Guid(ds.Tables[0].Rows[0]["MileStoneUID"].ToString()), new Guid(ds.Tables[0].Rows[0]["TaskUID"].ToString()), ds.Tables[0].Rows[0]["Description"].ToString(), DateTime.Now, Status, DateTime.Now, Convert.ToDateTime(ds.Tables[0].Rows[0]["ProjectedDate"].ToString()), new Guid(UserUID));
                                if (ret)
                                {
                                    return Json(new
                                    {
                                        Status = "Success",
                                        Message = "Milestone Status Updated Successfully.",
                                    });
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Status = "Failure",
                                        Message = "Error occured.",
                                    });
                                }
                            }
                            else
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Milestone is already completed.",
                                });
                            }

                        }
                        else
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Invalid Milestone UID",
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Invalid Status",
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error : "+ex.Message,
                });
            }
        }

        [HttpPost]
        public IHttpActionResult MileStoneUpdatewithName()
        {
            var httpRequest = HttpContext.Current.Request;
            var UserName = httpRequest.Params["UserName"];
            var Password = Security.Encrypt(httpRequest.Params["Password"]);
            DataSet ds = new DataSet();

            ds = db.CheckLogin(UserName, Password);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
                var MileStoneUID = httpRequest.Params["MileStoneUID"];
                var Status = httpRequest.Params["Status"];

                if (Status == "Not Completed" || Status == "Completed")
                {
                    ds.Clear();
                    ds = db.getMileStonesDetails(new Guid(MileStoneUID));
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["MileStoneUID"].ToString() == "Completed" && Status == "Not Completed")
                        {
                            bool ret = db.InsertorUpdateMileStone(new Guid(ds.Tables[0].Rows[0]["MileStoneUID"].ToString()), new Guid(ds.Tables[0].Rows[0]["TaskUID"].ToString()), ds.Tables[0].Rows[0]["Description"].ToString(), DateTime.Now, Status, DateTime.Now, Convert.ToDateTime(ds.Tables[0].Rows[0]["ProjectedDate"].ToString()), new Guid(UserUID));
                            if (ret)
                            {
                                return Json(new
                                {
                                    Status = "Success",
                                    Message = "Milestone Updated Successfully.",
                                });
                            }
                            else
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error occured.",
                                });
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Milestone is already completed.",
                            });
                        }

                    }
                    else
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Invalid Milestone UID",
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Invalid Status",
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Invalid UserName or Password",
                });
            }
        }

        [HttpPost]
        public IHttpActionResult ResourceAllocationUsageUpdate()
        {
            var httpRequest = HttpContext.Current.Request;
            var UserName = httpRequest.Params["UserName"];
            var Password = Security.Encrypt(httpRequest.Params["Password"]);
            DataSet ds = new DataSet();

            try
            {
                ds = db.CheckLogin(UserName, Password);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
                    var ResourceAllocationUID = httpRequest.Params["ResourceAllocationUID"];
                    var Usage = httpRequest.Params["Usage"];
                    var ToDate = httpRequest.Params["ToDate"];
                    ds.Clear();
                    ds = db.getTaskResourceAllocatedDetails(new Guid(ResourceAllocationUID));
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        double TotalUsed = 0;
                        double TotalUsedUsage = 0;
                        double AllocatedUsage = Convert.ToDouble(ds.Tables[0].Rows[0]["AllocatedUnits"].ToString());
                        DataSet UsedUsage = db.getTotalResourceUsage(new Guid(ResourceAllocationUID), new Guid(ds.Tables[0].Rows[0]["TaskUID"].ToString()));
                        if (UsedUsage.Tables[0].Rows.Count > 0)
                        {
                            TotalUsedUsage = Convert.ToDouble(UsedUsage.Tables[0].Rows[0]["TotalUsage"].ToString());
                        }

                        TotalUsed = AllocatedUsage - TotalUsedUsage;

                        if (Convert.ToDouble(Usage) > TotalUsed)
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Current usage exceeded the limit",
                            });
                        }
                        else
                        {
                            bool result = db.InsertorUpdateResourceUsage(Guid.NewGuid(), new Guid(ResourceAllocationUID), new Guid(ds.Tables[0].Rows[0]["ProjectUID"].ToString()), new Guid(ds.Tables[0].Rows[0]["WorkPackageUID"].ToString()), new Guid(ds.Tables[0].Rows[0]["TaskUID"].ToString()), Convert.ToDouble(Usage), DateTime.Now, DateTime.Now, new Guid(UserUID));
                            if (result)
                            {
                                return Json(new
                                {
                                    Status = "Success",
                                    Message = "Resource Usage Updated Successfully.",
                                });
                            }
                            else
                            {
                                return Json(new
                                {
                                    Status = "Failure",
                                    Message = "Error Occured.",
                                });
                            }
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Invalid Resource Allocation UID",
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error : "+ex.Message,
                });
            }
            
        }

        [HttpPost]
        public IHttpActionResult GetIssues()
        {
            List<Issues> finallist = new List<Issues>();
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
                        DataSet dsworkPackage = db.GetWorkPackages_By_ProjectUID(new Guid(dsProject.Tables[0].Rows[j]["ProjectUID"].ToString()));
                        for (int k = 0; k < dsworkPackage.Tables[0].Rows.Count; k++)
                        {
                            DataSet workpackaeissues=db.getIssuesList_by_WorkPackageUID(new Guid(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString()));
                            for (int l = 0; l < workpackaeissues.Tables[0].Rows.Count; l++)
                            {
                                if (workpackaeissues.Tables[0].Rows[l]["TaskUID"].ToString() == "00000000-0000-0000-0000-000000000000")
                                {
                                    Issues issue = new Issues()
                                    {
                                        Issue_Uid = new Guid(workpackaeissues.Tables[0].Rows[l]["Issue_Uid"].ToString()),
                                        ProjectUID = new Guid(workpackaeissues.Tables[0].Rows[l]["ProjectUID"].ToString()),
                                        WorkPackagesUID = new Guid(workpackaeissues.Tables[0].Rows[l]["WorkPackagesUID"].ToString()),
                                        TaskUID = new Guid(workpackaeissues.Tables[0].Rows[l]["TaskUID"].ToString()),
                                        Issue_Description = workpackaeissues.Tables[0].Rows[l]["Issue_Description"].ToString(),
                                        Issue_Status = workpackaeissues.Tables[0].Rows[l]["Issue_Status"].ToString(),
                                        Issue_Date = Convert.ToDateTime(workpackaeissues.Tables[0].Rows[l]["Issue_Date"].ToString()),
                                    };
                                    finallist.Add(issue);
                                }
                            }

                            DataSet task = db.GetTasksForWorkPackages(dsworkPackage.Tables[0].Rows[k]["WorkPackageUID"].ToString());
                            for (int m = 0; m < task.Tables[0].Rows.Count; m++)
                            {
                                DataSet taskissues = db.getIssuesList_by_TaskUID(new Guid(task.Tables[0].Rows[m]["TaskUID"].ToString()));
                                for (int n = 0; n < taskissues.Tables[0].Rows.Count; n++)
                                {
                                    Issues taskissue = new Issues()
                                    {
                                        Issue_Uid = new Guid(taskissues.Tables[0].Rows[n]["Issue_Uid"].ToString()),
                                        ProjectUID = new Guid(taskissues.Tables[0].Rows[n]["ProjectUID"].ToString()),
                                        WorkPackagesUID = new Guid(taskissues.Tables[0].Rows[n]["WorkPackagesUID"].ToString()),
                                        TaskUID = new Guid(taskissues.Tables[0].Rows[n]["TaskUID"].ToString()),
                                        Issue_Description = taskissues.Tables[0].Rows[n]["Issue_Description"].ToString(),
                                        Issue_Status = taskissues.Tables[0].Rows[n]["Issue_Status"].ToString(),
                                        Issue_Date = Convert.ToDateTime(taskissues.Tables[0].Rows[n]["Issue_Date"].ToString()),
                                    };
                                    finallist.Add(taskissue);

                                    IssuesAdd(new Guid(task.Tables[0].Rows[m]["TaskUID"].ToString()), finallist);
                                }

                            }
                        }
                    }
                }

                return Json(finallist.ToList());
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


        public void IssuesAdd(Guid ParentTaskUID,List<Issues> iList)
        {
            DataSet ds = db.GetTask_by_ParentTaskUID(ParentTaskUID);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataSet Issues = db.getIssuesList_by_TaskUID(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()));
                for (int n = 0; n < Issues.Tables[0].Rows.Count; n++)
                {
                    Issues taskissue = new Issues()
                    {
                        Issue_Uid = new Guid(Issues.Tables[0].Rows[n]["Issue_Uid"].ToString()),
                        ProjectUID = new Guid(Issues.Tables[0].Rows[n]["ProjectUID"].ToString()),
                        WorkPackagesUID = new Guid(Issues.Tables[0].Rows[n]["WorkPackagesUID"].ToString()),
                        TaskUID = new Guid(Issues.Tables[0].Rows[n]["TaskUID"].ToString()),
                        Issue_Description = Issues.Tables[0].Rows[n]["Issue_Description"].ToString(),
                        Issue_Status = Issues.Tables[0].Rows[n]["Issue_Status"].ToString(),
                        Issue_Date = Convert.ToDateTime(Issues.Tables[0].Rows[n]["Issue_Date"].ToString()),
                    };
                    iList.Add(taskissue);
                }

                IssuesAdd(new Guid(ds.Tables[0].Rows[i]["TaskUID"].ToString()), iList);
            }
        }

        [HttpPost]
        public IHttpActionResult IssueStatusUpdate()
        
{
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var Issue_Uid = httpRequest.Params["Issue_Uid"];
                var Status = httpRequest.Params["Status"];
                var Remarks = httpRequest.Params["Remarks"];
                DataSet ds = db.getIssuesList_by_UID(new Guid(Issue_Uid));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["Issue_Status"].ToString() == "Close")
                    {
                        return Json(new
                        {
                            Status = "Failure",
                            Message = "Issue alreday closed.Cannot update any other status.",
                        });
                    }
                    else
                    {
                        int cnt = db.Issues_Status_Remarks_Insert(Guid.NewGuid(), new Guid(Issue_Uid), Status, Remarks, "");
                        if (cnt > 0)
                        {
                            return Json(new
                            {
                                Status = "Success",
                                Message = "Issue Status Updated Successfully.",
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                Status = "Failure",
                                Message = "Error occured.",
                            });
                        }
                    }

                }
                else
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Invalid Issue UID.",
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Error : "+ex.Message,
                });
            }
            
        }

        //[HttpPost]
        //public IHttpActionResult MeasurementUpdate()
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
        //            return Json(new
        //            {
        //                Status = "Failure",
        //                Message = "Invalid UserName or Password",
        //            });
        //        }

        //        string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
        //        var Quantity = httpRequest.Params["Quantity"];
        //        var Unit = httpRequest.Params["Unit"];
        //        var TaskUID = httpRequest.Params["TaskUID"];
        //        if (Quantity == "")
        //        {
        //            return Json(new
        //            {
        //                Status = "Success",
        //                Message = "Current Quantity cannot be empty.",
        //            });
        //        }
        //        if (Quantity == "0")
        //        {
        //            return Json(new
        //            {
        //                Status = "Success",
        //                Message = "Current Quantity cannot be zero.",
        //            });
        //        }
        //        ds = db.GetTaskDetails(TaskUID);
        //        if (ds.Tables[0].Rows[0]["UnitforProgress"].ToString() != "")
        //        {
        //            double CumulativeQuan = db.GetMeasurementCumulativeQuantity(new Guid(TaskUID));
        //            if (Convert.ToDouble(ds.Tables[0].Rows[0]["UnitQuantity"].ToString()) - CumulativeQuan < Convert.ToDouble(Quantity))
        //            {
        //                return Json(new
        //                {
        //                    Status = "Success",
        //                    Message = "Current Quantity should be less than max Quantity.",
        //                });
        //            }
        //        }

        //        int ret = db.InsertorUpdateTaskMeasurementBook(Guid.NewGuid(), new Guid(TaskUID), Unit, Quantity, "", DateTime.Now, "", new Guid(UserUID));
        //        if (ret > 0)
        //        {
        //            return Json(new
        //            {
        //                Status = "Success",
        //                Message = "Measurement Updated Successfully.",
        //            });
        //        }


        //        return Json(new
        //        {
        //            Status = "Success",
        //            Message = "Measurement Not Updated Successfully.",
        //        });
        //    }
        //    catch(Exception ex)
        //    {
        //        var result1 = new
        //        {
        //            Status = "Failure",
        //            Message = ex.Message

        //        };
        //        return Json(result1);
        //    }
        //}

        [HttpPost]
        public IHttpActionResult StatusUpdate()
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
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    });
                }
                string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
                var Percentage = Convert.ToDouble(httpRequest.Params["Percentage"]);
                var Status = httpRequest.Params["Status"];
                var TaskUID = httpRequest.Params["TaskUID"];
                var WorkPackageID = httpRequest.Params["WorkPackageID"];
                var Comments = httpRequest.Params["Comments"];
                if (Status == "Not Started")
                {
                    Percentage = 0;
                }
                else if (Status != "In-Progress")
                {
                    Percentage = 100;
                }

                int ret = db.UpdateTaskStatus(WorkPackageID, TaskUID, Status, DateTime.Now, Percentage, Comments);
                if (ret > 0)
                {
                    return Json(new
                    {
                        Status = "Success",
                        Message = "Status Updated Successfully.",
                    });
                }

                return Json(new
                {
                    Status = "Success",
                    Message = "Status Not Updated Successfully.",
                });
            }
            catch (Exception ex)
            {
                var result1 = new
                {
                    Status = "Failure",
                    Message = ex.Message

                };
                return Json(result1);
            }
        }

        [HttpPost]
        public IHttpActionResult AddFinancialMileStone()
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
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    });
                }
                string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
                var AllowedPayment = Convert.ToDouble(httpRequest.Params["AllowedPayment"].Replace(",", "").Replace("₹ ", "").Replace("$ ", "").Replace("¥ ", "").Replace("₹", "").Replace("$", "").Replace("¥", "").Trim());
                var GST = httpRequest.Params["GST"];
                var TaskUID = httpRequest.Params["TaskUID"];
                double AE = db.GetTask_ActualExpenditure_by_TaskUID(new Guid(TaskUID));
                var TaskBudget = httpRequest.Params["TaskBudget"];
                var MileStoneName = httpRequest.Params["MileStoneName"];
                var FinanceMileStoneUID = new Guid();
                if ((AE + AllowedPayment) > Convert.ToDouble(AllowedPayment + ((Convert.ToDouble(GST) / 100) * AllowedPayment)))
                {
                    return Json(new
                    {
                        Status = "Success",
                        Message = "Allowed payment should be less than total task budget",
                    });

                }
                var dtPlannedDate = httpRequest.Params["dtPlannedDate"];
                string sDate1 = dtPlannedDate;
                sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                DateTime CDate1 = Convert.ToDateTime(sDate1);
                bool ret = db.InsertorUpdateFinanceMileStone(FinanceMileStoneUID, new Guid(TaskUID), MileStoneName, AllowedPayment, Convert.ToDouble(GST), CDate1, new Guid(UserUID));
                if (ret == true)
                {
                    return Json(new
                    {
                        Status = "Success",
                        Message = "Financial Status Updated Successfully.",
                    });
                }
                else
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Financial Status not Updated.",
                    });
                }
            }
            catch (Exception ex)
            {
                var result1 = new
                {
                    Status = "Failure",
                    Message = ex.Message

                };
                return Json(result1);
            }


        }

        [HttpPost]
        public IHttpActionResult FinancialUpdate()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var UserName = httpRequest.Params["UserName"];
                var Password = Security.Encrypt(httpRequest.Params["Password"]);
                DataSet ds = db.CheckLogin(UserName, Password);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Invalid UserName or Password",
                    });
                }

                string UserUID = ds.Tables[0].Rows[0]["UserUID"].ToString();
                var financialMileStoneId = httpRequest.Params["FinanceMileStoneUID"];
                string Allowedpayment = db.FinanceMileStoneAllowedPayment_Finance_MileStoneUID(new Guid(financialMileStoneId));
                var ActualPayment = httpRequest.Params["ActualPayment"];
                var ActualPaymentDate = httpRequest.Params["ActualPaymentDate"];
                if (Convert.ToDouble(ActualPayment) > Convert.ToDouble(Allowedpayment))
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Actual Payment Exceeded.",
                    });
                }
                Guid FinanceMileStoneUpdate_UID = Guid.NewGuid();
                string sDate1 = "";
                DateTime CDate1 = DateTime.Now;
                if (ActualPaymentDate != "" && ActualPaymentDate != "dd/mm/yyyy")
                {
                    sDate1 = ActualPaymentDate;
                    sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                    CDate1 = Convert.ToDateTime(sDate1);
                }
                bool ret = db.FinanceMileStonePaymentUpdate_Insert(FinanceMileStoneUpdate_UID, new Guid(financialMileStoneId), Convert.ToDouble(Allowedpayment), Convert.ToDouble(ActualPayment), CDate1, new Guid(UserUID));
                if (ret == true)
                {
                    return Json(new
                    {
                        Status = "Success",
                        Message = "Data updated successfully.",
                    });
                }
                else
                {
                    return Json(new
                    {
                        Status = "Failure",
                        Message = "Date not Updated.",
                    });
                }
            }
            catch (Exception ex)
            {
                var result1 = new
                {
                    Status = "Failure",
                    Message = ex.Message

                };
                return Json(result1);
            }


        }

        [HttpPost]
        public IHttpActionResult AddFinancialMileStoneUpdatewithProject()
        {
            var httpRequest = HttpContext.Current.Request;
            var ProjectCategory = httpRequest.Params["ProjectCategory"];
            var categoryUId = db.GetProjectCategoryUID_ProjectCategoryName(ProjectCategory);
            if (categoryUId == "")
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Category is not found"
                });
            }

            var projectUid = db.GetProjectUID_ProjectName_Project_CategoryUID(new Guid(categoryUId), httpRequest.Params["ProjectName"]);
            if (projectUid == "")
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Project is not found"
                });
            }

            var workpackageUid = db.GetWorkPackageID_ProjectId_WorkPackageName(new Guid(projectUid), httpRequest.Params["WorkPackageName"]);
            if (workpackageUid == "")
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Workpackage is not found"
                });
            }

            var taskUid = db.GetTaskUID_workPackageUId_TaskName(new Guid(workpackageUid), httpRequest.Params["TaskName"]);
            if (taskUid == "")
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Task is not found"
                });
            }
            var financialMileStoneId = db.GetfinancialMileStoneID_workPackageUId_TaskName(new Guid(taskUid), httpRequest.Params["MileStoneName"]);
            if (financialMileStoneId == "")
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Task is not found"
                });
            }

            string Allowedpayment = db.FinanceMileStoneAllowedPayment_Finance_MileStoneUID(new Guid(financialMileStoneId));
            var ActualPayment = httpRequest.Params["ActualPayment"];
            var ActualPaymentDate = httpRequest.Params["ActualPaymentDate"];
            if (Convert.ToDouble(ActualPayment) > Convert.ToDouble(Allowedpayment))
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Actual Payment Exceeded.",
                });
            }
            Guid FinanceMileStoneUpdate_UID = Guid.NewGuid();
            string sDate1 = "";
            DateTime CDate1 = DateTime.Now;
            if (ActualPaymentDate != "" && ActualPaymentDate != "dd/mm/yyyy")
            {
                sDate1 = ActualPaymentDate;
                sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                CDate1 = Convert.ToDateTime(sDate1);
            }
            bool ret = false;// db.FinanceMileStonePaymentUpdate_Insert(FinanceMileStoneUpdate_UID, new Guid(financialMileStoneId), Convert.ToDouble(Allowedpayment), Convert.ToDouble(ActualPayment), CDate1, new Guid(UserUID));
            if (ret == true)
            {
                return Json(new
                {
                    Status = "Success",
                    Message = "Data updated successfully.",
                });
            }
            else
            {
                return Json(new
                {
                    Status = "Failure",
                    Message = "Date not Updated.",
                });
            }


        }


        [HttpPost]
        public IHttpActionResult TaskMeasurementUpdate()
        {
            var httpRequest = HttpContext.Current.Request;
            var MeasurementUID = httpRequest.Params["MeasurementUID"];
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var TaskParam = httpRequest.Params["Tasks"];
            var Quantity= httpRequest.Params["Quantity"];
            var Remarks = httpRequest.Params["Remarks"];
            var UserEmail = httpRequest.Params["UserEmail"];
            var UnitforProgress= httpRequest.Params["UnitofProgress"];
            var AchievedDate = httpRequest.Params["AchievedDate"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                if (float.TryParse(Quantity, out float Quant))
                {
                    if (DateTime.TryParse(AchievedDate, out DateTime sDate))
                    {
                        var pExists = db.ProjectNameExists(ProjectName);
                        if (pExists != "")
                        {
                            string[] TaskList = TaskParam.Split(',');
                            //var objects = JArray.Parse(TaskParam); // parse as array 
                            //var result = JsonConvert.DeserializeObject<RootObject>(TaskParam);

                            var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                            if (wExists != "")
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
                                    if (TaskUID != "")
                                    {
                                        DateTime CDate1 = DateTime.Now;

                                        string sDate1 = Convert.ToDateTime(AchievedDate).ToString("dd/MM/yyyy");
                                        DateTime aDate = new DateTime();
                                        //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        aDate = Convert.ToDateTime(sDate1);

                                        int rs = db.InsertorUpdateTaskMeasurementBook(new Guid(MeasurementUID), new Guid(TaskUID), UnitforProgress, Quantity, "", CDate1, "", new Guid(db.GetUserUIDfrom_UserEmail(UserEmail)), Remarks, "Y", aDate);
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
                        ErrorText = "Invalid Achieved date format";
                    }
                        
                }
                else
                {
                    sError = true;
                    ErrorText = "Invalid Quantity";
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


        [HttpPost]
        public IHttpActionResult TaskMeasurementUpdatewithoutGrouping()
        {
            var httpRequest = HttpContext.Current.Request;
            var MeasurementUID = httpRequest.Params["MeasurementUID"];
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var TaskParam = httpRequest.Params["Tasks"];
            var Quantity = httpRequest.Params["Quantity"];
            var Remarks = httpRequest.Params["Remarks"];
            var UserEmail = httpRequest.Params["UserEmail"];
            var UnitforProgress = httpRequest.Params["UnitforProgress"];
            var AchievedDate = httpRequest.Params["SelectedDate"];
            bool sError = false;
            string ErrorText = "";
            try
            {
                if (float.TryParse(Quantity, out float Quant))
                {
                    if (DateTime.TryParse(AchievedDate, out DateTime sDate))
                    {
                        var pExists = db.ProjectNameExists(ProjectName);
                        if (pExists != "")
                        {
                            string[] TaskList = TaskParam.Split(',');
                            //var objects = JArray.Parse(TaskParam); // parse as array 
                            //var result = JsonConvert.DeserializeObject<RootObject>(TaskParam);

                            var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                            if (wExists != "")
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
                                    if (TaskUID != "")
                                    {
                                        DateTime CDate1 = DateTime.Now;

                                        string sDate1 = Convert.ToDateTime(AchievedDate).ToString("dd/MM/yyyy");
                                        DateTime aDate = new DateTime();
                                        //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                        aDate = Convert.ToDateTime(sDate1);

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
                    ErrorText = "Invalid Quantity";
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

        [HttpPost]
        public IHttpActionResult MeasurementBookEdit()
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
                if (float.TryParse(Quantity, out float Quant))
                {
                    if (DateTime.TryParse(MeasurementDate, out DateTime sDate))
                    {
                        DataSet ds = db.GetMeasurementBook_By_UID(new Guid(MeasurementUID));
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int rs = db.MeasurementBookUpdate(new Guid(MeasurementUID), new Guid(ds.Tables[0].Rows[0]["TaskUID"].ToString()), ds.Tables[0].Rows[0]["UnitforProgress"].ToString(), Quantity, "", sDate, "", new Guid(db.GetUserUIDfrom_UserEmail(UserEmail)), Remarks);
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
                            ErrorText = "Invalid Measurement UID";
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

        [HttpPost]
        public IHttpActionResult TaskMeasurementDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var MeasurementUID = httpRequest.Params["MeasurementUID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";

            try
            {
                DataSet ds = db.GetMeasurementBook_By_UID(new Guid(MeasurementUID));
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int cnt = db.Measurement_Delete(new Guid(MeasurementUID), new Guid(UserUID));
                    if (cnt > 0)
                    {
                        sError = false;
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Measurement UID.";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Measurement book not deleted.";
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
                    Message = "Successfully Deleted Measurement."
                });
            }
        }


        [HttpPost]
        public IHttpActionResult ResourceDeploymentAdd()
        {
            var httpRequest = HttpContext.Current.Request;
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var ResourceName = httpRequest.Params["ResourceName"];
            var sMonth = httpRequest.Params["sMonth"];
            var dType = httpRequest.Params["DeploymentType"];
            var Planned = httpRequest.Params["Planned"];
            var pExists = db.ProjectNameExists(ProjectName);
            bool sError = false;
            string ErrorText = "";
            if (pExists !="")
            {
                var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                if (wExists != "")
                {
                    var rUID = db.RsourceNameExists(ResourceName, wExists);
                    if (rUID != "")
                    {
                        if (DateTime.TryParse(sMonth, out DateTime sDate))
                        {
                            DateTime d = sDate;
                            string sDate1 = "", sDate2 = "";
                            DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now;
                            sDate1 = d.ToString("dd/MM/yyyy");
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);

                            int Days = DateTime.DaysInMonth(Convert.ToInt32(d.Month), Convert.ToInt32(d.Year)) - 1;
                            CDate2 = CDate1.AddDays(Days);

                            if (float.TryParse(Planned, out float p))
                            {
                                int sresult = db.InsertorUpdateResourceDeploymentPlanned(Guid.NewGuid(), new Guid(wExists), new Guid(rUID), CDate1, CDate2, "Month", p, DateTime.Now, "N");
                                if (sresult > 0)
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
                    ErrorText = "Workpackage Name is not found";
                }
            }
            else
            {
                sError = true;
                ErrorText = "Project Name is not found";
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
                    Status = "Success",
                    Message = "Successfully Add Resource Deployment"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult ResourceDeploymentUpdate()
        {
            var httpRequest = HttpContext.Current.Request;
            var UID = httpRequest.Params["UID"];
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var ResourceName = httpRequest.Params["ResourceName"];
            //var sMonth = httpRequest.Params["sMonth"];          
            var Deployed = httpRequest.Params["Deployed"];
            var Remarks = httpRequest.Params["Remarks"];
            var DeployedDate = httpRequest.Params["DeployedDate"];

            var pExists = db.ProjectNameExists(ProjectName);
            bool sError = false;
            string ErrorText = "";
            if (pExists != "")
            {
                var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                if (wExists != "")
                {
                    var rUID = db.RsourceNameExists(ResourceName, wExists);
                    if (rUID != "")
                    {
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
                    Message = "Successfully Updated Resource Deployment"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult ResourceDeploymentDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var UID = httpRequest.Params["UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";

            DataSet ds = db.GetResourceDeploymentUpdate_by_UID(new Guid(UID));
            if (ds.Tables[0].Rows.Count > 0)
            {
                int cnt = db.ResourceDeploymentUpdate_Delete(new Guid(UID), new Guid(UserUID));
                if (cnt > 0)
                {
                    sError = false;
                }
                else
                {
                    sError = true;
                    ErrorText = "Invalid Resource Deployment UID.";
                }
            }
            else
            {
                sError = true;
                ErrorText = "Resource Deployment not deleted.";
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

        [HttpPost]
        public IHttpActionResult BOQAdd()
        {
            var httpRequest = HttpContext.Current.Request;
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var ItemNumber = httpRequest.Params["ItemNumber"];
            var Description = httpRequest.Params["Description"];
            var Quantity = httpRequest.Params["Quantity"];
            var INR_Rate = httpRequest.Params["INRRate"];
            var Unit = httpRequest.Params["Unit"];
            var INR_Amount = httpRequest.Params["INRAmount"];
            var Currency = "&#x20B9;";
            var Currency_CultureInfo = "en-IN";
            var ItemLevel = httpRequest.Params["ItemLevel"];

            var pExists = db.ProjectNameExists(ProjectName);
            bool sError = false;
            string ErrorText = "";
            if (pExists != "")
            {
                var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                if (wExists != "")
                {
                    var rUID = db.RsourceNameExists(Description, wExists);
                    if (rUID != "")
                    {
                        int sresult = db.InsertBOQDetails(ItemNumber, Description, Quantity, Unit, INR_Rate, "0",
                    "0", INR_Amount, "0", "0", "",
                    new Guid(pExists), "N");
                        if (sresult > 0)
                        {
                            sError = false;
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "BOQ Data not updated.";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "BOQ Name is not found";
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
                    Status = "Success",
                    Message = "Successfully Updated Resource Deployment"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult BOQUpdate()
        {
            var httpRequest = HttpContext.Current.Request;
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var ItemNumber = httpRequest.Params["ItemNumber"];
            var Description = httpRequest.Params["Description"];
            var Quantity = httpRequest.Params["Quantity"];
            var INR_Rate = httpRequest.Params["INRRate"];
            var Unit = httpRequest.Params["Unit"];
            var INR_Amount = httpRequest.Params["INRAmount"];

            var ItemLevel = httpRequest.Params["ItemLevel"];

            var pExists = db.ProjectNameExists(ProjectName);
            bool sError = false;
            string ErrorText = "";
            if (pExists != "")
            {
                var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                if (wExists != "")
                {
                    var tLevel = Convert.ToInt32(httpRequest.Params["TLevel"]);
                    if (tLevel > 0)
                    {
                        string[] BOQList = Description.Split(',');

                        string BOQUID = string.Empty;
                        string ParentBOQUID = "";
                        for (int i = 0; i < tLevel; i++)
                        {
                            if (i == 0)
                            {
                                BOQUID = db.BOQNameExists(new Guid(wExists), BOQList[i], "Workpackage");
                                ParentBOQUID = BOQUID;
                            }
                            else
                            {
                                BOQUID = db.BOQNameExists(new Guid(ParentBOQUID), BOQList[i], "BOQ");
                            }

                            if (BOQUID == null || BOQUID == "")
                            {
                                sError = true;
                                ErrorText = "Invalid BOQ Name";
                                break;
                            }
                        }
                        if (BOQUID != "")
                        {
                            int sresult = db.UpdateBOQDetails(ItemNumber, Description, Quantity, Unit, INR_Rate, INR_Amount, new Guid(BOQUID));
                            if (sresult > 0)
                            {
                                sError = false;
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "BOQ Data not updated.";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "BOQ Name is not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid BOQ Item Level";
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
                    Status = "Success",
                    Message = "Successfully Updated Resource Deployment"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult AddJointInspectiontoBOQItem()
        {
            var httpRequest = HttpContext.Current.Request;
            var ProjectName = httpRequest.Params["ProjectName"];
            var WorkpackageName = httpRequest.Params["WorkpackageName"];
            var BOQName = httpRequest.Params["BOQName"];
            //var BOQUID = httpRequest.Params["BOQUID"];
            //var InspectionUID = httpRequest.Params["InspectionUID"];
            var DiaPipe = httpRequest.Params["DiaPipe"];
            var PipeNumber = httpRequest.Params["PipeNumber"];
            var invoice_number = httpRequest.Params["invoice_number"];
            var invoicedate = httpRequest.Params["invoicedate"];
            var unit = httpRequest.Params["unit"];
            var quantity = httpRequest.Params["quantity"];
            var Inspection_Type= httpRequest.Params["InspectionType"];
            var ItemLevel = httpRequest.Params["ItemLevel"];
            var StartingPointVal= httpRequest.Params["StartingPoint"];
            var LenghtVal = httpRequest.Params["Lenght"];
            var Chainage_Number= httpRequest.Params["Chainage_Number"];
            var Chainage_Desc= httpRequest.Params["Chainage_Desc"];
            var Qty_in_RMTVal = httpRequest.Params["Qty_in_RMT"];
            var Qty_for_UnitVal = httpRequest.Params["Qty_for_Unit"];
            

            var pExists = db.ProjectNameExists(ProjectName);
            bool sError = false;
            string ErrorText = "";
            if (pExists != "")
            {
                var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                if (wExists != "")
                {
                    var tLevel = Convert.ToInt32(httpRequest.Params["TLevel"]);
                    if (tLevel > 0)
                    {
                        string[] BOQList = BOQName.Split(',');

                        string BOQUID = string.Empty;
                        string ParentBOQUID = "";
                        for (int i = 0; i < tLevel; i++)
                        {
                            if (i == 0)
                            {
                                BOQUID = db.BOQNameExists(new Guid(wExists), BOQList[i], "Workpackage");
                                ParentBOQUID = BOQUID;
                            }
                            else
                            {
                                BOQUID = db.BOQNameExists(new Guid(ParentBOQUID), BOQList[i], "BOQ");
                            }

                            if (BOQUID == null || BOQUID == "")
                            {
                                sError = true;
                                ErrorText = "Invalid BOQ Name";
                                break;
                            }
                        }

                        //var rUID = db.BOQNameExists(BOQName, wExists);
                        if (BOQUID != "")
                        {

                            string sDate1 = "";
                            DateTime CDate1 = DateTime.Now;
                            sDate1 = invoicedate;
                            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);

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

                            int sresult = db.InsertjointInspection(Guid.NewGuid(), new Guid(BOQUID), DiaPipe, unit, invoice_number, CDate1.ToString(), quantity, Inspection_Type, Chainage_Number, Chainage_Desc, StartingPoint, Length, Qty_in_RMT, Qty_for_Unit, Deductions, "", new Guid(pExists), PipeNumber, "Y");
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
                            ErrorText = "BOQ Name is not found";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid BOQ Item Level";
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
                    Status = "Success",
                    Message = "Successfully Updated Resource Deployment"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult AddJointInspectionReport()
        {
            var httpRequest = HttpContext.Current.Request;
            var inspectionUid = httpRequest.Params["inspectionUid"];
            var ProjectName = httpRequest.Params["ProjectName"];
            var BOQUID = httpRequest.Params["BOQUID"];
            var DiaPipe = httpRequest.Params["DiaPipe"];
            var PipeNumber = httpRequest.Params["PipeNumber"];
            var invoice_number = httpRequest.Params["invoice_number"];
            var invoicedate = httpRequest.Params["invoicedate"];
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
            //var rUID = db.BOQNameExists(BOQName, wExists);
            try
            {
                if (inspectionUid != "")
                {
                    var pExists = db.ProjectNameExists(ProjectName);
                    if (pExists != "")
                    {
                        if (BOQUID != "")
                        {
                            string sDate1 = "";
                            DateTime CDate1 = DateTime.Now;
                            sDate1 = Convert.ToDateTime(invoicedate).ToString("dd/MM/yyyy");
                            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);

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
                            ErrorText = "BOQ Name is not found";
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
                    Message = "Successfully Added Joint Inspection"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult EditJointInspectionReport()
        {
            var httpRequest = HttpContext.Current.Request;
            var inspectionUid = httpRequest.Params["inspectionUid"];
            var ProjectName = httpRequest.Params["ProjectName"];
            var BOQUID = httpRequest.Params["BOQUID"];
            var DiaPipe = httpRequest.Params["DiaPipe"];
            var PipeNumber = httpRequest.Params["PipeNumber"];
            var invoice_number = httpRequest.Params["invoice_number"];
            var invoicedate = httpRequest.Params["invoicedate"];
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
            //var rUID = db.BOQNameExists(BOQName, wExists);
            try
            {
                if (inspectionUid != "")
                {
                    var pExists = db.ProjectNameExists(ProjectName);
                    if (pExists != "")
                    {
                        if (BOQUID != "")
                        {
                            string sDate1 = "";
                            DateTime CDate1 = DateTime.Now;
                            sDate1 = Convert.ToDateTime(invoicedate).ToString("dd/MM/yyyy");
                            //sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);

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
                            ErrorText = "BOQ Name is not found";
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

        [HttpPost]
        public IHttpActionResult AddJointInspectionDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var UID = httpRequest.Params["UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";

            try
            {
                DataTable ds = db.getJointInspection_by_inspectionUid(UID);
                if (ds.Rows.Count > 0)
                {
                    int cnt = db.JointInspection_Delete(new Guid(UID), new Guid(UserUID));
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
                        ErrorText = "Invalid Joint Inspection UID.";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Joint Inspection not deleted.";
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

        [HttpPost]
        public IHttpActionResult AddJointInspectionDocuments()
        {
            var httpRequest = HttpContext.Current.Request;
            var ProjectUID = httpRequest.Params["ProjectUID"];
            var InspectionDocumentUID= httpRequest.Params["InspectionDocumentUID"];
            var inspectionUid = httpRequest.Params["inspectionUid"];
            var FileName = httpRequest.Params["FileName"];
            var Extension = httpRequest.Params["Extension"];
            var UploadFilePhysicalpath = httpRequest.Params["UploadFilePhysicalpath"];
            var RelativePath = httpRequest.Params["RelativePath"];

            bool sError = false;
            string ErrorText = "";
            try
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
                    Message = "Successfully Added Inspection Report Document."
                });
            }
        }

        [HttpPost]
        public IHttpActionResult AddorUpdateBankGuarantee()
        {
            var httpRequest = HttpContext.Current.Request;
            var Bank_GuaranteeUID = httpRequest.Params["Bank_GuaranteeUID"];
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
                var pExists = db.ProjectNameExists(ProjectName);
                if (pExists != "")
                {
                    var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                    if (wExists != "")
                    {
                        if (DateTime.TryParse(Date_of_Guarantee, out DateTime sDate))
                        {
                            DateTime d = sDate;
                            string sDate1 = "", sDate2 = "", sDate3 = "";
                            DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now;
                            sDate1 = d.ToString("dd/MM/yyyy");
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);

                            sDate2 = Convert.ToDateTime(Date_of_Expiry).ToString("dd/MM/yyyy");
                            sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                            CDate2 = Convert.ToDateTime(sDate2);

                            sDate3 = Convert.ToDateTime(Cliam_Date).ToString("dd/MM/yyyy");
                            sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                            CDate3 = Convert.ToDateTime(sDate3);

                            if (float.TryParse(Amount, out float p))
                            {
                                bool sresult = db.Dbsync_InsertorUpdateBankGuarantee(new Guid(Bank_GuaranteeUID), Vendor_Name, Vendor_Address, Convert.ToDouble(Amount), Validity, CDate1, Convert.ToInt32(No_of_Collaterals), Bank_Name, Bank_Branch, IFSC_Code, new Guid(wExists), new Guid(pExists), BG_Number, CDate2, Currency, Currency_CultureInfo, CDate3, "Y", Bank_Address);
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
                            ErrorText = "Invalid Date of Guarantee Datetime format.";
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
                    Message = "Successfully Updated Bank Guarantee"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult BankGuaranteeDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var Bank_GuaranteeUID = httpRequest.Params["Bank_GuaranteeUID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;

            string ErrorText = "";
            try
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
                        ErrorText = "Invalid Bank Guarantee UID.";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Bank Guarantee not deleted.";
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

        [HttpPost]
        public IHttpActionResult AddBankGuaranteeDocuments()
        {
            var httpRequest = HttpContext.Current.Request;
            var BankDoc_UID = httpRequest.Params["BankDoc_UID"];
            var Bank_GuaranteeUID = httpRequest.Params["Bank_GuaranteeUID"];
            var Document_Name = httpRequest.Params["Document_Name"];
            var Document_Type = httpRequest.Params["Document_Type"];
            var Document_File = httpRequest.Params["Document_File"];
            var RelativePath = httpRequest.Params["RelativePath"];

            bool sError = false;
            string ErrorText = "";
            try
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
                    httpPostedFile.SaveAs(sDocumentPath + sFileName + Extn);

                    string Fullpath = sDocumentPath + sFileName + Extn;

                    bool count = db.InsertorUpdateBankDocuments(new Guid(BankDoc_UID), new Guid(Bank_GuaranteeUID), Document_Name, Document_Type, Document_File, "Y");
                    if (count)
                    {
                        sError = false;

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
                    Message = "Successfully Added Bank Guarantee Document."
                });
            }
        }

        [HttpPost]
        public IHttpActionResult BankGuaranteeDocumentsDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var BankDoc_UID = httpRequest.Params["BankDoc_UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";

            try
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
                        ErrorText = "Invalid Bank Guarantee Document UID.";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Bank Guarantee Document not deleted.";
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

        [HttpPost]
        public IHttpActionResult AddInsurance()
        {
            var httpRequest = HttpContext.Current.Request;
            var InsuranceUID = httpRequest.Params["InsuranceUID"];
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
                var pExists = db.ProjectNameExists(ProjectName);
                if (pExists != "")
                {
                    var wExists = db.WorkpackageNameExists(WorkpackageName, pExists);
                    if (wExists != "")
                    {
                        if (DateTime.TryParse(Maturity_Date, out DateTime sDate))
                        {
                            DateTime d = sDate;
                            string sDate1 = "", sDate2 = "";
                            DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now;
                            sDate1 = d.ToString("dd/MM/yyyy");
                            sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                            CDate1 = Convert.ToDateTime(sDate1);

                            sDate2 = Convert.ToDateTime(Insured_Date).ToString("dd/MM/yyyy");
                            sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                            CDate2 = Convert.ToDateTime(sDate2);

                            if (float.TryParse(Insured_Amount, out float p))
                            {
                                if (float.TryParse(Premium_Amount, out float pAmount))
                                {
                                    bool sresult = db.DbSyncInsertorUpdateInsurance(new Guid(InsuranceUID), Vendor_Name, Vendor_Address, Name_of_InsuranceCompany, Branch, Policy_Number, Policy_Status, CDate1, Nominee, new Guid(pExists), new Guid(wExists), CDate2, p, pAmount, Convert.ToInt32(Frequency), Currency, Currency_CultureInfo, FirstPremium_Duedate, "Y");
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
                            ErrorText = "Invalid Maturity Date Datetime format.";
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
                    Message = "Successfully Updated Insurance"
                });
            }
        }

        [HttpPost]
        public IHttpActionResult InsuranceDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var InsuranceUID = httpRequest.Params["InsuranceUID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;

            string ErrorText = "";
            try
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
                        ErrorText = "Insurance not deleted. Please contact system admin.";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Insurance UID not found.";
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
                    Message = "Successfully Deleted Insurance."
                });
            }
        }

        [HttpPost]
        public IHttpActionResult AddInsuranceDocuments()
        {
            var httpRequest = HttpContext.Current.Request;
            var InsuranceDoc_UID = httpRequest.Params["InsuranceDoc_UID"];
            var InsuranceUID = httpRequest.Params["InsuranceUID"];
            var InsuranceDoc_Name = httpRequest.Params["InsuranceDoc_Name"];
            var InsuranceDoc_Type = httpRequest.Params["InsuranceDoc_Type"];
            var InsuranceDoc_FilePath = httpRequest.Params["InsuranceDoc_FilePath"];
            var RelativePath = httpRequest.Params["RelativePath"];

            bool sError = false;
            string ErrorText = "";

            try
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

                    string Fullpath = sDocumentPath + sFileName + "_1" + Extn;

                    bool count = db.DbSync_InsertorUpdateInsuranceDocuments(new Guid(InsuranceDoc_UID), new Guid(InsuranceUID), InsuranceDoc_Name, InsuranceDoc_Type, InsuranceDoc_FilePath, "Y");
                    if (count)
                    {
                        sError = false;
                        byte[] filetobytes = null;
                        filetobytes = db.FileToByteArray(Fullpath);

                        Guid new_guid = Guid.NewGuid();
                        db.InsertUploadedInsuranceDocumentBlob(new_guid, InsuranceDoc_UID.ToString(), filetobytes, httpPostedFile.FileName, RelativePath);

                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Insurance Document not Updated";
                    }
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
                    Message = "Successfully Added Insurance Document."
                });
            }
        }

        [HttpPost]
        public IHttpActionResult InsuranceDocumentsDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var InsuranceDoc_UID = httpRequest.Params["InsuranceDoc_UID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;

            string ErrorText = "";
            try
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

        [HttpPost]
        public IHttpActionResult AddInsurancePremium()
        {
            var httpRequest = HttpContext.Current.Request;
            var PremiumUID = httpRequest.Params["PremiumUID"];
            var InsuranceUID = httpRequest.Params["InsuranceUID"];
            var Premium_Paid = httpRequest.Params["Premium_Paid"];
            var Interest = httpRequest.Params["Interest"];
            var Penalty = httpRequest.Params["Penalty"];
            var Premium_PaidDate = httpRequest.Params["Premium_PaidDate"];
            var Premium_DueDate = httpRequest.Params["Premium_DueDate"];
            var Next_PremiumDate = httpRequest.Params["Next_PremiumDate"];
            var Premium_Receipt = httpRequest.Params["Premium_Receipt"];
            var Remarks = httpRequest.Params["Remarks"];
            var RelativePath = httpRequest.Params["RelativePath"];

            bool sError = false;
            string ErrorText = "";
            try
            {
                if (float.TryParse(Premium_Paid, out float pPaidAmount))
                {
                    if (float.TryParse(Interest, out float Inte))
                    {
                        if (DateTime.TryParse(Premium_PaidDate, out DateTime pPaidDate))
                        {
                            if (DateTime.TryParse(Premium_DueDate, out DateTime pDueDate))
                            {
                                bool FileSaved = false;

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
                                    FileSaved = true;
                                }

                                if (Premium_Receipt == "")
                                {
                                    FileSaved = true;
                                }
                                if (FileSaved)
                                {
                                    string sDate1 = "", sDate2 = "", sDate3 = "";
                                    DateTime CDate1 = DateTime.Now, CDate2 = DateTime.Now, CDate3 = DateTime.Now;

                                    sDate1 = pPaidDate.ToString("dd/MM/yyyy");
                                    sDate1 = sDate1.Split('/')[1] + "/" + sDate1.Split('/')[0] + "/" + sDate1.Split('/')[2];
                                    CDate1 = Convert.ToDateTime(sDate1);

                                    sDate2 = pDueDate.ToString("dd/MM/yyyy");
                                    sDate2 = sDate2.Split('/')[1] + "/" + sDate2.Split('/')[0] + "/" + sDate2.Split('/')[2];
                                    CDate2 = Convert.ToDateTime(sDate2);


                                    sDate3 = Convert.ToDateTime(Next_PremiumDate).ToString("dd/MM/yyyy");
                                    sDate3 = sDate3.Split('/')[1] + "/" + sDate3.Split('/')[0] + "/" + sDate3.Split('/')[2];
                                    CDate3 = Convert.ToDateTime(sDate3);
                                                                       

                                    byte[] filetobytes = null;
                                    filetobytes = db.FileToByteArray(Premium_Receipt);

                                    bool result = db.InsertorUpdateInsurancePremium(new Guid(PremiumUID), new Guid(InsuranceUID), pPaidAmount, Inte, float.Parse(Penalty), CDate1, CDate2, CDate3, Premium_Receipt, Remarks, filetobytes);
                                    
                                    if (result)
                                    {
                                        sError = false;
                                    }
                                    else
                                    {
                                        sError = true;
                                        ErrorText = "Insurance Premium not Updated";

                                    }
                                }
                                else
                                {
                                    sError = true;
                                    ErrorText = "Insurance Premium Receipt document not saved";
                                }
                            }
                            else
                            {
                                sError = true;
                                ErrorText = "Invalid Premium Due date";
                            }
                        }
                        else
                        {
                            sError = true;
                            ErrorText = "Invalid Premium Paid date";
                        }
                    }
                    else
                    {
                        sError = true;
                        ErrorText = "Invalid Interest amount";
                    }

                }
                else
                {
                    sError = true;
                    ErrorText = "Invalid Premium amount";
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
                    Message = "Successfully Added Insurance Premium."
                });
            }
        }

        [HttpPost]
        public IHttpActionResult InsurancePremiumDelete()
        {
            var httpRequest = HttpContext.Current.Request;
            var PremiumUID = httpRequest.Params["PremiumUID"];
            var UserUID = httpRequest.Params["UserUID"];
            bool sError = false;
            string ErrorText = "";
            try
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
                        ErrorText = "Insurance Premium not deleted. Please contact system admin.";
                    }
                }
                else
                {
                    sError = true;
                    ErrorText = "Insurance Premium not found.";
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
    }
}