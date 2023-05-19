using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using PMTWebAPI.DAL;
using PMTWebAPI.Models;

namespace PMTWebAPI.Controllers
{
    public class UserDetailsController :ApiController
    {
        DBGetData db = new DBGetData();
        TableProperties tp = new TableProperties();

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

        [HttpGet]
        public IHttpActionResult GetUsersList()
        {
            try
            {
                
                DataSet ds = new DataSet();
                ds = db.getAllUsers();
                return Json(new
                {
                    Success = true,
                    Ipaddress = GetIp()
                   // Users = ds.Tables[0]

                });
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

        [HttpPost]
        public IHttpActionResult LoginUser()
        {
            var httpRequest = HttpContext.Current.Request;
            var UserName = httpRequest.Params["UserName"];
            var Password = Security.Encrypt(httpRequest.Params["Password"]);
            DataSet ds = db.CheckLogin(UserName, Password);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Json(new
                {
                    Status = "Success",
                    UserUID= ds.Tables[0].Rows[0]["UserUID"].ToString(),
                    TypeofUser= ds.Tables[0].Rows[0]["TypeOfUser"].ToString(),
                    UserName= ds.Tables[0].Rows[0]["FirstName"].ToString()
                });
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
    }
}